namespace Aurum.DataProtection

module Windows =
    let encryptString (data: string) =
        let rawBytes = System.Text.Encoding.UTF8.GetBytes data

        System.Security.Cryptography.ProtectedData.Protect(
            rawBytes,
            null,
            System.Security.Cryptography.DataProtectionScope.CurrentUser
        )

    let decryptString protectedData =
        let decryptedBytes =
            System.Security.Cryptography.ProtectedData.Unprotect(
                protectedData,
                null,
                System.Security.Cryptography.DataProtectionScope.CurrentUser
            )

        System.Text.Encoding.UTF8.GetString decryptedBytes
