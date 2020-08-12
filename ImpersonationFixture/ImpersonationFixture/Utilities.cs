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
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security;

namespace ImpersonationFixture
{
    internal static class Utilities
    {
        private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr handle);

        public static void CloseToken(IntPtr token) => CloseHandle(token);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int DuplicateToken(IntPtr hToken, int impersonationLevel, ref IntPtr hNewToken);

        public static IntPtr DuplicateToken(IntPtr logonToken)
        {
            const int securityImpersonation = 2;

            var logonTokenDuplicate = IntPtr.Zero;
            if (DuplicateToken(logonToken, securityImpersonation, ref logonTokenDuplicate) != 0) return logonTokenDuplicate;
            throw new Win32Exception(Marshal.GetLastWin32Error(), Message(Resources.CallFailure, nameof(DuplicateToken)));
        }


        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool LogonUser(string lpszUserName, string lpszDomain, IntPtr lpszPassword, int dwLogonType, int dwLogonProvider,
            ref IntPtr phToken);

        public static IntPtr LogonUser(string userName, string domain, SecureString password)
        {
            const int logIn32ProviderDefault = 0;
            const int logOn32LogOnInteractive = 2;

            var passwordHandle = Marshal.SecureStringToGlobalAllocUnicode(password);
            var logonToken = IntPtr.Zero;
            if (LogonUser(userName, domain, passwordHandle, logOn32LogOnInteractive, logIn32ProviderDefault, ref logonToken)) return logonToken;
            throw new Win32Exception(Marshal.GetLastWin32Error(), Message(Resources.CallFailure, nameof(LogonUser)));
        }

        public static string Message(string messageTemplate, params object[] args) => string.Format(Culture, messageTemplate, args);
    }
}
