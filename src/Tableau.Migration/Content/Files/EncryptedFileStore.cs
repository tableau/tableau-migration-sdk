//
//  Copyright (c) 2024, Salesforce, Inc.
//  SPDX-License-Identifier: Apache-2
//  
//  Licensed under the Apache License, Version 2.0 (the "License") 
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//  http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

using System;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Config;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Content.Files
{
    /// <summary>
    /// <see cref="IContentFileStore"/> implementation that
    /// transparently encrypts file content.
    /// </summary>
    public class EncryptedFileStore : IContentFileStore
    {
        private readonly ISymmetricEncryptionFactory _encryptionFactory;
        private readonly IContentFileStore _innerStore;
        private readonly bool _encrypt;

        private byte[] _encryptionKey; //Mutable so we can clear the key during disposal.

        /// <inheritdoc />
        public bool HasOpenTableauFileEditor => _innerStore.HasOpenTableauFileEditor;

        /// <summary>
        /// Creates a new <see cref="EncryptedFileStore"/> object.
        /// </summary>
        /// <param name="services">The service provider for DI services.</param>
        /// <param name="innerStore">A file store to wrap with encryption.</param>
        public EncryptedFileStore(IServiceProvider services, IContentFileStore innerStore)
            : this(services.GetRequiredService<ISymmetricEncryptionFactory>(),
                  innerStore,
                  services.GetRequiredService<IConfigReader>(),
                  services.GetRequiredService<ILogger<EncryptedFileStore>>(),
                  services.GetRequiredService<ISharedResourcesLocalizer>())
        { }

        /// <summary>
        /// Creates a new <see cref="EncryptedFileStore"/> object.
        /// </summary>
        /// <param name="encryptionFactory">An encryption factory.</param>
        /// <param name="innerStore">A file store to wrap with encryption.</param>
        /// <param name="configReader">A configuration reader.</param>
        /// <param name="logger">A logger.</param>
        /// <param name="localizer">A string localizer.</param>
        public EncryptedFileStore(ISymmetricEncryptionFactory encryptionFactory,
            IContentFileStore innerStore, IConfigReader configReader,
            ILogger<EncryptedFileStore> logger, ISharedResourcesLocalizer localizer)
        {
            _encryptionFactory = encryptionFactory;
            _innerStore = innerStore;

            var config = configReader.Get();
            _encrypt = !config.Files.DisableFileEncryption;

            //Warn the user if encryption is disabled so they don't
            //forget to re-enable it for production migrations.
            if (!_encrypt)
            {
                logger.LogWarning(localizer[SharedResourceKeys.FileEncryptionDisabledLogMessage]);
            }

            //Generate a new encryption key per instance.
            using var encryption = _encryptionFactory.Create();
            _encryptionKey = encryption.Key;
        }

        #region - IContentFileStore Implementation -

        /// <inheritdoc />
        public IContentFileHandle Create(string relativeStorePath, string originalFileName)
            => new EncryptedFileHandle(this, _innerStore.Create(relativeStorePath, originalFileName));

        /// <inheritdoc />
        public IContentFileHandle Create<TContent>(TContent contentItem, string originalFileName)
            => new EncryptedFileHandle(this, _innerStore.Create(contentItem, originalFileName));

        /// <inheritdoc />
        public async Task DeleteAsync(IContentFileHandle handle, CancellationToken cancel)
            => await _innerStore.DeleteAsync(handle, cancel).ConfigureAwait(false);


        /// <inheritdoc />
        public async Task<ITableauFileEditor> GetTableauFileEditorAsync(IContentFileHandle handle,
            CancellationToken cancel, bool? zipFormatOverride = null)
            => await _innerStore.GetTableauFileEditorAsync(handle, cancel, zipFormatOverride).ConfigureAwait(false);

        /// <inheritdoc />
        public async Task CloseTableauFileEditorAsync(IContentFileHandle handle, CancellationToken cancel)
            => await _innerStore.CloseTableauFileEditorAsync(handle, cancel).ConfigureAwait(false);

        /// <inheritdoc />
        public async Task<IContentFileStream> OpenReadAsync(IContentFileHandle handle, CancellationToken cancel)
        {
            var stream = await _innerStore.OpenReadAsync(handle, cancel).ConfigureAwait(false);
            if (_encrypt)
            {
                var encryption = _encryptionFactory.Create(); //Disposed by file stream wrapper.

                var iv = await stream.Content.ReadInitializationVectorAsync(encryption.IV.Length, cancel)
                    .ConfigureAwait(false);

                var transform = encryption.CreateDecryptor(_encryptionKey, iv); //Disposed by file stream wrapper.
                var cryptoStream = new SeekableCryptoStream(stream.Content, transform, CryptoStreamMode.Read, false); //Disposed by file stream wrapper.

                stream = new EncryptedFileStream(stream, encryption, transform, cryptoStream);
            }

            return stream;
        }

        /// <inheritdoc />
        public async Task<IContentFileStream> OpenWriteAsync(IContentFileHandle handle, CancellationToken cancel)
        {
            var stream = await _innerStore.OpenWriteAsync(handle, cancel).ConfigureAwait(false);
            if (_encrypt)
            {
                var encryption = _encryptionFactory.Create(); //Disposed by file stream wrapper.
                encryption.GenerateIV();

                await stream.Content.WriteInitializationVectorAsync(encryption.IV, cancel)
                    .ConfigureAwait(false);

                var transform = encryption.CreateEncryptor(_encryptionKey, encryption.IV); //Disposed by file stream wrapper.
                var cryptoStream = new SeekableCryptoStream(stream.Content, transform, CryptoStreamMode.Write, false); //Disposed by file stream wrapper.

                stream = new EncryptedFileStream(stream, encryption, transform, cryptoStream);
            }

            return stream;
        }

        #endregion

        #region - IAsyncDisposable Implementation -

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            // Perform async cleanup.
            await _innerStore.DisposeAsync().ConfigureAwait(false);

            _encryptionKey = Array.Empty<byte>();

            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
