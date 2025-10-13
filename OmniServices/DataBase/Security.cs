namespace DataBase;

/// <summary>
/// Cryptographic class for encryption and decryption of string values.
/// </summary>
public static class Security
{
    #region Declaration
    private static string actionKey;
    private static string actionIv;
    #endregion

    static Security()
    {
        actionKey = AppSettingFile.Get("EncryptionActionKey") ?? "";
        actionIv = AppSettingFile.Get("EncryptionActionIV") ?? "";
    }

    #region Function

    /// <summary>
    /// Converts a hex string to a byte array.
    /// </summary>
    /// <param name="hex">Hex string.</param>
    /// <returns>Byte array.</returns>
    private static byte[] HexToBytes(string hex)
    {
        byte[] bytes = new byte[hex.Length / 2];
        for (int i = 0; i < hex.Length / 2; i++)
        {
            string code = hex.Substring(i * 2, 2);
            bytes[i] = byte.Parse(code, System.Globalization.NumberStyles.HexNumber);
        }
        return bytes;
    }

    /// <summary>
    /// Encrypts a memory string (i.e. variable).
    /// </summary>
    /// <param name="data">String to be encrypted.</param>
    /// <param name="key">Encryption key.</param>
    /// <param name="iv">Encryption initialization vector.</param>
    /// <returns>Encrypted string.</returns>
    public static string Encrypt(string data, string key, string iv)
    {
        byte[] bdata = Encoding.ASCII.GetBytes(data);
        byte[] bkey = HexToBytes(key); // 32 bytes (AES-256)
        byte[] biv = Encoding.UTF8.GetBytes(iv); // 12 bytes (recommended for AES-GCM)
        byte[] ciphertext = new byte[bdata.Length];
        byte[] tag = new byte[16]; // 16 bytes

#pragma warning disable SYSLIB0053
        using var aesGcm = new AesGcm(bkey);
#pragma warning restore SYSLIB0053
        aesGcm.Encrypt(biv, bdata, ciphertext, tag);

        byte[] encryptedPackage = new byte[iv.Length + tag.Length + ciphertext.Length];
        Buffer.BlockCopy(biv, 0, encryptedPackage, 0, biv.Length);
        Buffer.BlockCopy(tag, 0, encryptedPackage, biv.Length, tag.Length);
        Buffer.BlockCopy(ciphertext, 0, encryptedPackage, biv.Length + tag.Length, ciphertext.Length);

        return Convert.ToBase64String(encryptedPackage);
    }

    /// <summary>
    /// Decrypts a memory string (i.e. variable).
    /// </summary>
    /// <param name="data">String to be decrypted.</param>
    /// <param name="key">Original encryption key.</param>
    /// <param name="iv">iv </param>
    /// <returns>Decrypted string.</returns>
    public static string Decrypt(string data, string key, string iv)
    {
        byte[] bdata = Convert.FromBase64String(data);
        byte[] bkey = HexToBytes(key);
        byte[] biv = Encoding.UTF8.GetBytes(iv);

        int ivLen = 12; // 96 bits
        int tagLen = 16; // 128 bits

        byte[] tag = new byte[tagLen];
        byte[] ciphertext = new byte[bdata.Length - ivLen - tagLen];

        Buffer.BlockCopy(bdata, 0, biv, 0, ivLen);
        Buffer.BlockCopy(bdata, ivLen, tag, 0, tagLen);
        Buffer.BlockCopy(bdata, ivLen + tagLen, ciphertext, 0, ciphertext.Length);

        byte[] plaintext = new byte[ciphertext.Length];

#pragma warning disable SYSLIB0053
        using var aesGcm = new AesGcm(bkey);
#pragma warning restore SYSLIB0053
        aesGcm.Decrypt(biv, ciphertext, tag, plaintext);

        return Encoding.UTF8.GetString(plaintext);
    }

    /// <summary>
    /// Standard encrypt method for Patterns in DoFactory.
    /// Uses the predefined key and iv.
    /// </summary>
    /// <param name="data">String to be encrypted.</param>
    /// <returns>Encrypted string</returns>
    public static string AcEnc(string data)
    {
        if (!string.IsNullOrEmpty(data) && !string.IsNullOrWhiteSpace(data))
        {
            return Encrypt(data, actionKey, actionIv);
        }
        return string.Empty;
    }

    public static string AcDec(string data)
    {
        if (!string.IsNullOrEmpty(data) && !string.IsNullOrWhiteSpace(data))
        {
            return Decrypt(data, actionKey, actionIv);
        }
        return string.Empty;
    }

    #endregion
}
