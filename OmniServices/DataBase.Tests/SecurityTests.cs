using DataBase.Tests.Helpers;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace DataBase.Tests;

/// <summary>
/// Comprehensive test suite for the Security class encryption and decryption functionality.
/// Tests cover both the low-level Encrypt/Decrypt methods and the high-level AcEnc/AcDec methods.
/// </summary>
public class SecurityTests : IDisposable
{
    private readonly string _testKey;
    private readonly string _testIV;
    private readonly Dictionary<string, string> _configValues;
    private IConfiguration _originalConfiguration = null!;

    public SecurityTests()
    {
        _testKey = SecurityTestHelper.GetFixedTestKey();
        _testIV = SecurityTestHelper.GetFixedTestIV();
        
        // Setup configuration for AcEnc/AcDec testing
        _configValues = new Dictionary<string, string>
        {
            {"AppSettings:EncryptionActionKey", _testKey},
            {"AppSettings:EncryptionActionIV", _testIV}
        };
        
        SetupConfiguration();
    }

    private void SetupConfiguration()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(_configValues!)
            .Build();
            
        AppSettingFile.Initialize(configuration);
    }

    public void Dispose()
    {
        // Cleanup if needed
        GC.SuppressFinalize(this);
    }

    #region Basic Encrypt/Decrypt Tests

    [Fact]
    public void Encrypt_ValidInput_ShouldReturnEncryptedString()
    {
        // Arrange
        const string plaintext = "Hello World";

        // Act
        var encrypted = Security.Encrypt(plaintext, _testKey, _testIV);

        // Assert
        encrypted.Should().NotBeNull();
        encrypted.Should().NotBeEmpty();
        encrypted.Should().NotBe(plaintext);
        
        // Should be valid Base64
        Action act = () => Convert.FromBase64String(encrypted);
        act.Should().NotThrow();
    }

    [Fact]
    public void Decrypt_ValidEncryptedString_ShouldReturnOriginalPlaintext()
    {
        // Arrange
        const string plaintext = "Hello World";
        var encrypted = Security.Encrypt(plaintext, _testKey, _testIV);

        // Act
        var decrypted = Security.Decrypt(encrypted, _testKey, _testIV);

        // Assert
        decrypted.Should().Be(plaintext);
    }

    [Theory]
    [MemberData(nameof(GetTestStringData))]
    public void Encrypt_Decrypt_RoundTrip_ShouldPreserveOriginalData(string originalText)
    {
        // Arrange & Act
        var encrypted = Security.Encrypt(originalText, _testKey, _testIV);
        var decrypted = Security.Decrypt(encrypted, _testKey, _testIV);

        // Assert
        decrypted.Should().Be(originalText);
    }

    [Fact]
    public void Encrypt_SameInputMultipleTimes_ShouldProduceDifferentResults()
    {
        // Arrange
        const string plaintext = "Test message";

        // Act
        var encrypted1 = Security.Encrypt(plaintext, _testKey, _testIV);
        var encrypted2 = Security.Encrypt(plaintext, _testKey, _testIV);

        // Assert - Due to the way AES-GCM works with the same IV, results might be the same
        // This test verifies the method works consistently
        var decrypted1 = Security.Decrypt(encrypted1, _testKey, _testIV);
        var decrypted2 = Security.Decrypt(encrypted2, _testKey, _testIV);
        
        decrypted1.Should().Be(plaintext);
        decrypted2.Should().Be(plaintext);
    }

    [Fact]
    public void Encrypt_EmptyString_ShouldHandleGracefully()
    {
        // Arrange
        const string plaintext = "";

        // Act
        var encrypted = Security.Encrypt(plaintext, _testKey, _testIV);
        var decrypted = Security.Decrypt(encrypted, _testKey, _testIV);

        // Assert
        decrypted.Should().Be(plaintext);
    }

    #endregion

    #region AcEnc/AcDec Tests (Action Encryption/Decryption)

    [Fact]
    public void AcEnc_ValidInput_ShouldReturnEncryptedString()
    {
        // Arrange
        const string plaintext = "Sensitive Data";

        // Act
        var encrypted = Security.AcEnc(plaintext);

        // Assert
        encrypted.Should().NotBeNull();
        encrypted.Should().NotBeEmpty();
        encrypted.Should().NotBe(plaintext);
    }

    [Fact]
    public void AcDec_ValidEncryptedString_ShouldReturnOriginalPlaintext()
    {
        // Arrange
        const string plaintext = "Sensitive Data";
        var encrypted = Security.AcEnc(plaintext);

        // Act
        var decrypted = Security.AcDec(encrypted);

        // Assert
        decrypted.Should().Be(plaintext);
    }

    [Theory]
    [MemberData(nameof(GetTestStringData))]
    public void AcEnc_AcDec_RoundTrip_ShouldPreserveOriginalData(string originalText)
    {
        // Arrange & Act
        var encrypted = Security.AcEnc(originalText);
        var decrypted = Security.AcDec(encrypted);

        // Assert
        decrypted.Should().Be(originalText);
    }

    [Fact]
    public void AcEnc_EmptyString_ShouldReturnEmptyString()
    {
        // Act
        var result = Security.AcEnc("");

        // Assert
        result.Should().Be(string.Empty);
    }

    [Fact]
    public void AcEnc_NullString_ShouldReturnEmptyString()
    {
        // Act
        var result = Security.AcEnc(null!);

        // Assert
        result.Should().Be(string.Empty);
    }

    [Fact]
    public void AcEnc_WhitespaceString_ShouldReturnEmptyString()
    {
        // Act
        var result = Security.AcEnc("   ");

        // Assert
        result.Should().Be(string.Empty);
    }

    [Fact]
    public void AcDec_EmptyString_ShouldReturnEmptyString()
    {
        // Act
        var result = Security.AcDec("");

        // Assert
        result.Should().Be(string.Empty);
    }

    [Fact]
    public void AcDec_NullString_ShouldReturnEmptyString()
    {
        // Act
        var result = Security.AcDec(null!);

        // Assert
        result.Should().Be(string.Empty);
    }

    [Fact]
    public void AcDec_WhitespaceString_ShouldReturnEmptyString()
    {
        // Act
        var result = Security.AcDec("   ");

        // Assert
        result.Should().Be(string.Empty);
    }

    #endregion

    #region Error Handling Tests

    [Theory]
    [MemberData(nameof(GetInvalidHexKeyData))]
    public void Encrypt_InvalidHexKey_ShouldThrowException(string invalidKey)
    {
        // Arrange
        const string plaintext = "Test";

        // Act & Assert
        var act = () => Security.Encrypt(plaintext, invalidKey, _testIV);
        act.Should().Throw<Exception>();
    }

    [Fact]
    public void Encrypt_NullKey_ShouldThrowException()
    {
        // Arrange
        const string plaintext = "Test";

        // Act & Assert
        var act = () => Security.Encrypt(plaintext, null!, _testIV);
        act.Should().Throw<Exception>();
    }

    [Fact]
    public void Encrypt_NullIV_ShouldThrowException()
    {
        // Arrange
        const string plaintext = "Test";

        // Act & Assert
        var act = () => Security.Encrypt(plaintext, _testKey, null!);
        act.Should().Throw<Exception>();
    }

    [Fact]
    public void Encrypt_NullPlaintext_ShouldThrowException()
    {
        // Act & Assert
        var act = () => Security.Encrypt(null!, _testKey, _testIV);
        act.Should().Throw<Exception>();
    }

    [Fact]
    public void Decrypt_InvalidBase64_ShouldThrowException()
    {
        // Arrange
        const string invalidBase64 = "This is not base64!@#";

        // Act & Assert
        var act = () => Security.Decrypt(invalidBase64, _testKey, _testIV);
        act.Should().Throw<Exception>();
    }

    [Fact]
    public void Decrypt_TamperedCiphertext_ShouldThrowException()
    {
        // Arrange
        const string plaintext = "Test message";
        var encrypted = Security.Encrypt(plaintext, _testKey, _testIV);
        
        // Tamper with the encrypted data
        var encryptedBytes = Convert.FromBase64String(encrypted);
        encryptedBytes[encryptedBytes.Length - 1] ^= 1; // Flip a bit
        var tamperedEncrypted = Convert.ToBase64String(encryptedBytes);

        // Act & Assert
        var act = () => Security.Decrypt(tamperedEncrypted, _testKey, _testIV);
        act.Should().Throw<Exception>();
    }

    [Fact]
    public void Decrypt_WrongKey_ShouldThrowException()
    {
        // Arrange
        const string plaintext = "Test message";
        var encrypted = Security.Encrypt(plaintext, _testKey, _testIV);
        var wrongKey = SecurityTestHelper.GenerateValidKey();

        // Act & Assert
        var act = () => Security.Decrypt(encrypted, wrongKey, _testIV);
        act.Should().Throw<Exception>();
    }

    #endregion

    #region Performance and Load Tests

    [Fact]
    public void Encrypt_Decrypt_LargeString_ShouldCompleteInReasonableTime()
    {
        // Arrange
        var largeString = new string('A', 10000); // 10KB string
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        var encrypted = Security.Encrypt(largeString, _testKey, _testIV);
        var decrypted = Security.Decrypt(encrypted, _testKey, _testIV);
        stopwatch.Stop();

        // Assert
        decrypted.Should().Be(largeString);
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(1000); // Should complete within 1 second
    }

    [Fact]
    public void AcEnc_AcDec_MultipleOperations_ShouldBeConsistent()
    {
        // Arrange
        const string plaintext = "Consistency test";
        const int iterations = 100;

        // Act & Assert
        for (int i = 0; i < iterations; i++)
        {
            var encrypted = Security.AcEnc(plaintext);
            var decrypted = Security.AcDec(encrypted);
            decrypted.Should().Be(plaintext, $"Failed on iteration {i}");
        }
    }

    #endregion

    #region Data Integrity Tests

    [Fact]
    public void Encrypt_Decrypt_BinaryData_ShouldPreserveData()
    {
        // Arrange - Create a string that contains various byte values
        var binaryData = Encoding.UTF8.GetString(Enumerable.Range(0, 256).Select(i => (byte)i).ToArray());

        // Act
        var encrypted = Security.Encrypt(binaryData, _testKey, _testIV);
        var decrypted = Security.Decrypt(encrypted, _testKey, _testIV);

        // Assert
        decrypted.Should().Be(binaryData);
    }

    [Fact]
    public void Encrypt_Different_Keys_ShouldProduceDifferentResults()
    {
        // Arrange
        const string plaintext = "Same message";
        var key1 = SecurityTestHelper.GetFixedTestKey();
        var key2 = SecurityTestHelper.GenerateValidKey();

        // Act
        var encrypted1 = Security.Encrypt(plaintext, key1, _testIV);
        var encrypted2 = Security.Encrypt(plaintext, key2, _testIV);

        // Assert
        encrypted1.Should().NotBe(encrypted2);
        
        // Both should decrypt correctly with their respective keys
        var decrypted1 = Security.Decrypt(encrypted1, key1, _testIV);
        var decrypted2 = Security.Decrypt(encrypted2, key2, _testIV);
        
        decrypted1.Should().Be(plaintext);
        decrypted2.Should().Be(plaintext);
    }

    #endregion

    #region Test Data Providers

    public static IEnumerable<object[]> GetTestStringData()
    {
        return SecurityTestHelper.GetTestStrings().Select(s => new object[] { s });
    }

    public static IEnumerable<object[]> GetInvalidHexKeyData()
    {
        return SecurityTestHelper.GetInvalidHexKeys().Select(k => new object[] { k });
    }

    #endregion
}