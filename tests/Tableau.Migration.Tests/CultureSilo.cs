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
using System.Globalization;
using System.Threading;

namespace Tableau.Migration.Tests
{
    /// <summary>
    /// <para>
    /// Sets the <see cref="Thread.CurrentCulture"/> and <see cref="Thread.CurrentUICulture"/>
    /// to a specific value and then reverts it when disposed.
    /// </para>
    /// <para>
    /// Useful for writing unit tests that execute a block of code under alternate cultures.
    /// </para>
    /// </summary>
    /// <example>
    /// using (CultureSilo.Finland())
    /// {
    ///     // Execute code in Finland's culture.
    /// }
    /// // Back to the original culture.
    /// </example>
    public class CultureSilo : IDisposable
    {
        private bool _disposed;

        private readonly CultureInfo _culture;

        private readonly CultureInfo _originalCurrentCulture;
        private readonly CultureInfo _originalCurrentUICulture;

        /// <summary>
        /// Creates a new <see cref="CultureSilo"/> using the specified <paramref name="culture"/>.
        /// </summary>
        public CultureSilo(CultureInfo culture)
        {
            _culture = culture;

            _originalCurrentCulture = Thread.CurrentThread.CurrentCulture;
            _originalCurrentUICulture = Thread.CurrentThread.CurrentUICulture;

            // Change the current thread's culture.
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }

        #region - IDisposable Implementation -

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!_disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    Thread.CurrentThread.CurrentCulture = _originalCurrentCulture;
                    Thread.CurrentThread.CurrentUICulture = _originalCurrentUICulture;
                }

                // Clean up unmanaged resources here.

                // Note disposing has been done.
                _disposed = true;
            }
        }

        /// <summary>
        /// Destructor.
        /// </summary>
        ~CultureSilo()
        {
            Dispose(false);
        }

        #endregion

        /// <summary>
        /// Converts the <see cref="CultureSilo"/> to its <see cref="String"/> representation.
        /// </summary>
        public override string ToString()
        {
            return _culture.DisplayName;
        }

        /// <summary>
        /// Creates a <see cref="CultureSilo"/> that uses the culture associated with
        /// the specified <paramref name="name"/>.
        /// </summary>
        /// <param name="name">A predefined <see cref="CultureInfo"/> name or the name of an existing <see cref="CultureInfo"/>.</param>
        public static CultureSilo FromName(string name)
        {
            var c = CultureInfo.CreateSpecificCulture(name);
            return new CultureSilo(c);
        }

        /// <summary>
        /// Creates a <see cref="CultureSilo"/> that uses the "fi-FI" culture.
        /// </summary>
        public static CultureSilo Finland()
        {
            return FromName("fi-FI");
        }

        /// <summary>
        /// Creates a <see cref="CultureSilo"/> that uses the "en-US" culture.
        /// </summary>
        public static CultureSilo UnitedStates()
        {
            return FromName("en-US");
        }
    }
}
