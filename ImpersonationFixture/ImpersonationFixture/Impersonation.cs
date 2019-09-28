// Copyright 2019 Rik Essenius
//
//   Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file 
//   except in compliance with the License. You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software distributed under the License 
//   is distributed on an "AS IS" BASIS WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and limitations under the License.

using System.ComponentModel;
using System.Security;
using System.Security.Principal;
using AdysTech.CredentialManager;

namespace ImpersonationFixture
{
    public sealed class Impersonation
    {
        [Documentation("Return the currently impersonated user, or the default user if no impersonation")]
        public static string CurrentUser => WindowsIdentity.GetCurrent().Name;

        [Documentation("Impersonate using a Generic entry in Credential Manager")]
        public static bool Impersonate(string target)
        {
            // CredentialManager can only return passwords for Generic credentials as per CredentialManager's unit tests.
            var credential = CredentialManager.GetCredentials(target);
            if (credential == null) throw new SecurityException(Utilities.Message(Resources.TargetNotFound, target, CurrentUser));
            try
            {
                Impersonator.Impersonate(credential.UserName, credential.Domain, credential.SecurePassword);
            }
            catch (Win32Exception exception)
            {
                throw new SecurityException(Utilities.Message(Resources.CouldNotImpersonate, credential.UserName, credential.Domain), exception);
            }
            return true;
        }

        [Documentation("Stop impersonating a user")]
        public static bool StopImpersonating()
        {
            // Allow a SecurityException to kick in; it will be caught by FitSharp.
            Impersonator.UndoImpersonation();
            return true;
        }
    }
}