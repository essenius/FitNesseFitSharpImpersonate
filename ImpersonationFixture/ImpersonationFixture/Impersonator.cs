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

using System;
using System.Security;
using System.Security.Principal;

namespace ImpersonationFixture
{
    public static class Impersonator
    {
        private static WindowsImpersonationContext _windowsImpersonationContext;

        public static void Impersonate(string userName, string domainName, SecureString password)
        {
            // if we are already impersonating, first get back to the original user.
            UndoImpersonation();

            var logonToken = IntPtr.Zero;
            var logonTokenDuplicate = IntPtr.Zero;
            try
            {
                // Use the original identity to start with
                _windowsImpersonationContext = WindowsIdentity.Impersonate(IntPtr.Zero);

                logonToken = Utilities.LogonUser(userName, domainName, password);
                logonTokenDuplicate = Utilities.DuplicateToken(logonToken);
                using (var windowsIdentity = new WindowsIdentity(logonTokenDuplicate))
                {
                    windowsIdentity.Impersonate();
                }
            }
            finally
            {
                if (logonToken != IntPtr.Zero) Utilities.CloseToken(logonToken);
                if (logonTokenDuplicate != IntPtr.Zero) Utilities.CloseToken(logonTokenDuplicate);
            }
        }

        public static void UndoImpersonation()
        {
            // restore original identity
            _windowsImpersonationContext?.Undo();
            _windowsImpersonationContext?.Dispose();
            _windowsImpersonationContext = null;
        }
    }
}