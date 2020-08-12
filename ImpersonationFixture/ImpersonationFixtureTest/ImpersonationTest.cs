// Copyright 2019-2020 Rik Essenius
//
//   Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file 
//   except in compliance with the License. You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software distributed under the License 
//   is distributed on an "AS IS" BASIS WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Security;
using AdysTech.CredentialManager;
using ImpersonationFixture;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ImpersonationFixtureTest
{
    [TestClass]
    public class ImpersonationTest
    {
        private const string Domain = "fitnesse.org";
        private const string Password = "'6HcF!gN$[Vf{3ag";
        private const string Target = "ImpersonationFixtureTest";
        private const string UserName = "FitNesseUser";

        [ClassCleanup]
        public static void ClassCleanup() => CredentialManager.RemoveCredentials(Target);

        [ClassInitialize, SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required signature")]
        public static void ClassInitialize(TestContext testContext)
        {
            var credential = new NetworkCredential(UserName, Password, Domain);
            CredentialManager.SaveCredentials(Target, credential);
        }

        [TestMethod]
        public void ImpersonationCredentialNotFoundTest()
        {
            const string unknownTarget = "@Unknown!Target$";
            try
            {
                Impersonation.Impersonate(unknownTarget);
                Assert.Fail("No security exception thrown after specifying unknown target");
            }
            catch (SecurityException se)
            {
                Assert.IsTrue(se.Message.StartsWith($"Could not find target '{unknownTarget}' in Generic section of Credential Manager for user '",
                    StringComparison.InvariantCultureIgnoreCase));
            }
        }


        [TestMethod]
        public void ImpersonationFailedLogonTest()
        {
            try
            {
                Impersonation.Impersonate(Target);
                Assert.Fail("No security exception thrown after impersonating with wrong credentials");
            }
            catch (SecurityException se)
            {
                Assert.AreEqual($"Could not impersonate user '{UserName}' for domain '{Domain}'", se.Message);
            }
        }

        [TestMethod]
        public void ImpersonationSuccessfulLogonTest()
        {
            // For this test to work, ensure there is a generic Credential Manager entry called ImpersonationUser with working credentials
            const string workingTarget = "ImpersonationUser";
            var currentUser = Impersonation.CurrentUser;
            Impersonation.Impersonate(workingTarget);
            Assert.AreNotEqual(currentUser, Impersonation.CurrentUser, "Users are different after impersonating");
            Impersonation.StopImpersonating();
            Assert.AreEqual(currentUser, Impersonation.CurrentUser, "Orginal user restored after stopping impersonation");
        }
    }
}
