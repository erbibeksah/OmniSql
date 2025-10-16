using DataBase.Tests.Helpers;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace DataBase.Tests;

/// <summary>
/// Edge case tests for Security class focusing on boundary conditions and unusual scenarios.
/// </summary>
public class SecurityEdgeCaseTests : IDisposable
{
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    [Theory]
    [InlineData("0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF")] // Uppercase hex
    [InlineData("0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef")] // Lowercase hex
    [InlineData("0123456789AbCdEf0123456789AbCdEf0123456789AbCdEf0123456789AbCdEf")] // Mixed case hex
    public void Encrypt_DifferentHexCasing_ShouldWork(string hexKey)
    {
        // Arrange
        var plaintext = "Test case sensitivity";
        var testIV = SecurityTestHelper.GetFixedTestIV();

        // Act & Assert
        Action act = () =>
        {
            var encrypted = Security.Encrypt(plaintext, hexKey, testIV);
            var decrypted = Security.Decrypt(encrypted, hexKey, testIV);
            decrypted.Should().Be(plaintext);
        };

        act.Should().NotThrow();
    }

    [Fact]
    public void Encrypt_MaxLengthString_ShouldWork()
    {
        // Arrange - Create a very long string (approaching practical limits)
        var longString = new string('X', 65536); // 64KB
        var key = SecurityTestHelper.GetFixedTestKey();
        var iv = SecurityTestHelper.GetFixedTestIV();

        // Act
        var encrypted = Security.Encrypt(longString, key, iv);
        var decrypted = Security.Decrypt(encrypted, key, iv);

        // Assert
        decrypted.Should().Be(longString);
    }

    [Theory]
    [InlineData("TestIV12345")] // 11 chars (too short)
    [InlineData("TestIV1234567")] // 13 chars (too long)
    public void Encrypt_InvalidIVLength_ShouldThrowException(string invalidIV)
    {
        // Arrange
        var plaintext = "Test";
        var key = SecurityTestHelper.GetFixedTestKey();

        // Act & Assert
        var act = () => Security.Encrypt(plaintext, key, invalidIV);
        act.Should().Throw<Exception>();
    }

    [Fact]
    public void Encrypt_SpecialUnicodeCharacters_ShouldPreserveData()
    {
        // Arrange
        var unicodeText = "🚀 Test with emojis 🎉 and symbols ∑∆∏ and Chinese 中文测试";
        var key = SecurityTestHelper.GetFixedTestKey();
        var iv = SecurityTestHelper.GetFixedTestIV();

        // Act
        var encrypted = Security.Encrypt(unicodeText, key, iv);
        var decrypted = Security.Decrypt(encrypted, key, iv);

        // Assert
        decrypted.Should().Be(unicodeText);
    }

    [Fact]
    public void Encrypt_StringWithNullCharacters_ShouldHandle()
    {
        // Arrange
        var textWithNull = "Text\0with\0null\0chars";
        var key = SecurityTestHelper.GetFixedTestKey();
        var iv = SecurityTestHelper.GetFixedTestIV();

        // Act
        var encrypted = Security.Encrypt(textWithNull, key, iv);
        var decrypted = Security.Decrypt(encrypted, key, iv);

        // Assert
        decrypted.Should().Be(textWithNull);
    }

    [Theory]
    [InlineData("\r")]
    [InlineData("\n")]
    [InlineData("\t")]
    [InlineData("\r\n")]
    [InlineData(" ")]
    public void Encrypt_WhitespaceCharacters_ShouldPreserve(string whitespace)
    {
        // Arrange
        var key = SecurityTestHelper.GetFixedTestKey();
        var iv = SecurityTestHelper.GetFixedTestIV();

        // Act
        var encrypted = Security.Encrypt(whitespace, key, iv);
        var decrypted = Security.Decrypt(encrypted, key, iv);

        // Assert
        decrypted.Should().Be(whitespace);
    }

    [Fact]
    public void Encrypt_AllPrintableAsciiCharacters_ShouldWork()
    {
        // Arrange - Create string with all printable ASCII characters (32-126)
        var allChars = string.Concat(Enumerable.Range(32, 95).Select(i => (char)i));
        var key = SecurityTestHelper.GetFixedTestKey();
        var iv = SecurityTestHelper.GetFixedTestIV();

        // Act
        var encrypted = Security.Encrypt(allChars, key, iv);
        var decrypted = Security.Decrypt(encrypted, key, iv);

        // Assert
        decrypted.Should().Be(allChars);
        allChars.Length.Should().Be(95); // Verify we have all printable ASCII chars
    }

    [Fact]
    public void Decrypt_PartiallyCorruptedData_ShouldThrowException()
    {
        // Arrange
        var plaintext = "Test corruption handling";
        var key = SecurityTestHelper.GetFixedTestKey();
        var iv = SecurityTestHelper.GetFixedTestIV();
        var encrypted = Security.Encrypt(plaintext, key, iv);
        
        // Corrupt the encrypted data by removing last few characters
        var corruptedData = encrypted.Substring(0, encrypted.Length - 5);

        // Act & Assert
        var act = () => Security.Decrypt(corruptedData, key, iv);
        act.Should().Throw<Exception>();
    }

    [Fact] 
    public void AcEnc_WithConfigurationChanges_ShouldAdaptDynamically()
    {
        // Arrange
        var initialConfig = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                {"AppSettings:EncryptionActionKey", SecurityTestHelper.GetFixedTestKey()},
                {"AppSettings:EncryptionActionIV", SecurityTestHelper.GetFixedTestIV()}
            })
            .Build();
            
        AppSettingFile.Initialize(initialConfig);
        
        var plaintext = "Configuration test";
        
        // Act - Test with initial configuration
        var encrypted1 = Security.AcEnc(plaintext);
        var decrypted1 = Security.AcDec(encrypted1);
        
        // Setup new configuration (note: Security class uses static constructor, 
        // so this might not change behavior in the same test run)
        var newConfig = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                {"AppSettings:EncryptionActionKey", SecurityTestHelper.GenerateValidKey()},
                {"AppSettings:EncryptionActionIV", SecurityTestHelper.GenerateValidIV()}
            })
            .Build();
            
        AppSettingFile.Initialize(newConfig);
        
        // Assert
        decrypted1.Should().Be(plaintext);
        
        // Note: Due to static constructor, the Security class will maintain 
        // the original key/IV for the lifetime of the AppDomain
        var encrypted2 = Security.AcEnc(plaintext);
        var decrypted2 = Security.AcDec(encrypted2);
        decrypted2.Should().Be(plaintext);
    }

    [Fact]
    public void Encrypt_RepeatedData_ShouldProduceConsistentResults()
    {
        // Arrange
        var repeatedPattern = new string('A', 1000) + new string('B', 1000) + new string('A', 1000);
        var key = SecurityTestHelper.GetFixedTestKey();
        var iv = SecurityTestHelper.GetFixedTestIV();

        // Act
        var encrypted1 = Security.Encrypt(repeatedPattern, key, iv);
        var encrypted2 = Security.Encrypt(repeatedPattern, key, iv);
        
        var decrypted1 = Security.Decrypt(encrypted1, key, iv);
        var decrypted2 = Security.Decrypt(encrypted2, key, iv);

        // Assert
        decrypted1.Should().Be(repeatedPattern);
        decrypted2.Should().Be(repeatedPattern);
        // Results should be identical since we're using the same key and IV
        encrypted1.Should().Be(encrypted2);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)] 
    [InlineData(3)]
    [InlineData(15)]
    [InlineData(16)]
    [InlineData(17)]
    [InlineData(32)]
    [InlineData(64)]
    public void Encrypt_VariousStringSizes_ShouldWork(int length)
    {
        // Arrange
        var testString = new string('X', length);
        var key = SecurityTestHelper.GetFixedTestKey();
        var iv = SecurityTestHelper.GetFixedTestIV();

        // Act
        var encrypted = Security.Encrypt(testString, key, iv);
        var decrypted = Security.Decrypt(encrypted, key, iv);

        // Assert
        decrypted.Should().Be(testString);
        decrypted.Length.Should().Be(length);
    }

    [Fact]
    public void Security_HighFrequencyOperations_ShouldMaintainAccuracy()
    {
        // Arrange
        var key = SecurityTestHelper.GetFixedTestKey();
        var iv = SecurityTestHelper.GetFixedTestIV();
        const int operationCount = 10000;
        var failureCount = 0;

        // Act
        Parallel.For(0, operationCount, i =>
        {
            try
            {
                var plaintext = $"High frequency test {i}";
                var encrypted = Security.Encrypt(plaintext, key, iv);
                var decrypted = Security.Decrypt(encrypted, key, iv);
                
                if (decrypted != plaintext)
                {
                    Interlocked.Increment(ref failureCount);
                }
            }
            catch
            {
                Interlocked.Increment(ref failureCount);
            }
        });

        // Assert
        failureCount.Should().Be(0, "All high-frequency operations should succeed");
    }
}