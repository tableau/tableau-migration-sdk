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
using System.Linq;
using Xunit;

namespace Tableau.Migration.Tests.Unit
{
    public interface IBase
    {
        public int BaseProperty { get; set; }

        void BaseMethod();
    }

    public interface IOtherBase
    {
        public int OtherBaseProperty { get; set; }

        void OtherBaseMethod();
    }

    public interface IExtended : IBase
    {
        public int ExtendedProperty { get; set; }

        void ExtendedMethod();
    }

    public interface IVeryExtended : IBase, IOtherBase
    {
        public int VeryExtendedProperty { get; set; }

        void VeryExtendedMethod();
    }

    class BaseClass : IBase
    {
        public int BaseProperty
        {
            get => throw new System.NotImplementedException();
            set => throw new System.NotImplementedException();
        }

        public void BaseMethod()
        {
            throw new NotImplementedException();
        }
    }

    public class TypeExtensionTests
    {
        public class TestGetAllInterfaceMethods
        {

            [Fact]
            public void ClassThrows()
            {
                Assert.Throws<ArgumentException>(() => typeof(BaseClass).GetAllInterfaceMethods());
            }

            [Fact]
            public void Base()
            {
                var method = typeof(IBase).GetAllInterfaceMethods();
                var methodName = method.Select(p => p.Name).ToArray();

                Assert.Equal(3, method.Length); // get, set for properties included
                Assert.Contains("BaseMethod", methodName);
            }


            [Fact]
            public void Extended()
            {
                var methods = typeof(IExtended).GetAllInterfaceMethods();
                var methodNames = methods.Select(p => p.Name).ToArray();

                Assert.Equal(6, methods.Length); // get, set for properties included
                Assert.Contains("BaseMethod", methodNames);
                Assert.Contains("ExtendedMethod", methodNames);
            }

            [Fact]
            public void VeryExtended()
            {
                var methods = typeof(IVeryExtended).GetAllInterfaceMethods();
                var methodNames = methods.Select(p => p.Name).ToArray();

                Assert.Equal(9, methods.Length); // get, set for properties included

                Assert.Contains("BaseMethod", methodNames);
                Assert.Contains("OtherBaseMethod", methodNames);
                Assert.Contains("VeryExtendedMethod", methodNames);
            }
        }

        public class TestGetAllInterfaceProperties
        {
            [Fact]
            public void ClassThrows()
            {
                Assert.Throws<ArgumentException>(() => typeof(BaseClass).GetAllInterfaceProperties());
            }

            [Fact]
            public void Base()
            {
                var prop = typeof(IBase).GetAllInterfaceProperties();
                var propName = prop.Select(p => p.Name).ToArray();

                Assert.Single(prop);
                Assert.Contains("BaseProperty", propName);
            }


            [Fact]
            public void Extended()
            {
                var props = typeof(IExtended).GetAllInterfaceProperties();
                var propNames = props.Select(p => p.Name).ToArray();

                Assert.Equal(2, props.Length);
                Assert.Contains("BaseProperty", propNames);
                Assert.Contains("ExtendedProperty", propNames);
            }

            [Fact]
            public void VeryExtended()
            {
                var props = typeof(IVeryExtended).GetAllInterfaceProperties();
                var propNames = props.Select(p => p.Name).ToArray();

                Assert.Equal(3, props.Length);

                Assert.Contains("BaseProperty", propNames);
                Assert.Contains("OtherBaseProperty", propNames);
                Assert.Contains("VeryExtendedProperty", propNames);
            }
        }
    }
}
