using System.Security.Cryptography;
using System.Text;

namespace DataBase.Tests.Helpers;

/// <summary>
/// Helper class for generating test data and utilities for Security class tests.
/// </summary>
public static class SecurityTestHelper
{
    /// <summary>
    /// Generates a valid 256-bit (32-byte) AES key in hex format.
    /// </summary>
    /// <returns>A 64-character hex string representing a 256-bit key.</returns>
    public static string GenerateValidKey()
    {
        using var aes = Aes.Create();
        aes.KeySize = 256;
        aes.GenerateKey();
        return Convert.ToHexString(aes.Key).ToLowerInvariant();
    }

    /// <summary>
    /// Generates a valid 12-byte initialization vector for AES-GCM.
    /// </summary>
    /// <returns>A 12-character string for use as IV.</returns>
    public static string GenerateValidIV()
    {
        var bytes = new byte[12];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Encoding.UTF8.GetString(bytes.Select(b => (byte)((b % 95) + 32)).ToArray());
    }

    /// <summary>
    /// Creates a fixed, predictable key for consistent testing.
    /// </summary>
    /// <returns>A fixed 64-character hex string.</returns>
    public static string GetFixedTestKey()
    {
        return "0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef";
    }

    /// <summary>
    /// Creates a fixed, predictable IV for consistent testing.
    /// </summary>
    /// <returns>A fixed 12-character string.</returns>
    public static string GetFixedTestIV()
    {
        return "TestIV123456";
    }

    /// <summary>
    /// Gets various test strings for encryption testing.
    /// </summary>
    /// <returns>Collection of test strings including edge cases.</returns>
    public static IEnumerable<string> GetTestStrings()
    {
        yield return "Hello World";
        yield return "Simple text";
        yield return "Text with special chars: !@#$%^&*()_+-=[]{}|;':\",./<>?";
        yield return "Unicode text: 你好世界 🌍 émojis";
        yield return "A longer text string that contains multiple words and should test the encryption with more data to ensure it handles various lengths properly.";
        yield return "1234567890";
        yield return "a";
        yield return "ABC123xyz";
    }

    /// <summary>
    /// Gets invalid hex keys for negative testing.
    /// </summary>
    /// <returns>Collection of invalid hex strings.</returns>
    public static IEnumerable<string> GetInvalidHexKeys()
    {
        yield return "invalidhex"; // Too short and invalid chars
        yield return "gg23456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef"; // Invalid hex char 'g'
        yield return "0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcd"; // Too short by 2 chars
        yield return "0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef00"; // Too long by 2 chars
    }

    /// <summary>
    /// Gets invalid IVs for negative testing.
    /// </summary>
    /// <returns>Collection of invalid IV strings.</returns>
    public static IEnumerable<string> GetInvalidIVs()
    {
        yield return "short"; // Too short
        yield return "toolongforaniv"; // Too long
        yield return ""; // Empty
    }
}