namespace Aurum.DataProtection

module Windows =
    let encryptString (data: string) =
        let rawBytes = System.Text.Encoding.UTF8.GetBytes data

        System.Security.Cryptography.ProtectedData.Protect
            rawBytes
            System.Security.Cryptography.DataProtectionScope.CurrentUser

    let decryptString protected =
        let decryptedBytes = System.Security.Cryptography.ProtectedData.Unprotect
            protected
            System.Security.Cryptography.DataProtectionScope.CurrentUser
        System.Text.Encoding.UTF8.GetString decryptedBytes
