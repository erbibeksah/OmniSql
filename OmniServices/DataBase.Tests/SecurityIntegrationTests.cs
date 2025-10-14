using Microsoft.Extensions.Configuration;
using System.Collections.Concurrent;
using System.Reflection;

namespace DataBase.Tests;

/// <summary>
/// Integration tests for Security class that test real-world scenarios and configuration handling.
/// </summary>
public class SecurityIntegrationTests : IDisposable
{
    private IConfiguration? _originalConfiguration;

    public void Dispose()
    {
        // Reset configuration if needed
        GC.SuppressFinalize(this);
    }

    [Fact]
    public void Security_WithMissingConfiguration_AcEncShouldHandleGracefully()
    {
        // Arrange - Setup configuration without encryption keys
        var emptyConfig = new ConfigurationBuilder().Build();
        AppSettingFile.Initialize(emptyConfig);

        // Act
        var result = Security.AcEnc("test data");

        // Assert
        // Should handle missing configuration gracefully
        result.Should().NotBeNull();
    }

    [Fact]
    public void Security_WithPartialConfiguration_ShouldHandleGracefully()
    {
        // Arrange - Setup configuration with only one key
        var partialConfig = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                {"AppSettings:EncryptionActionKey", "0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef"}
                // Missing EncryptionActionIV
            })
            .Build();
            
        AppSettingFile.Initialize(partialConfig);

        // Act
        var result = Security.AcEnc("test data");

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public void Security_StaticInitialization_ShouldWorkCorrectly()
    {
        // Arrange - Setup proper configuration
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                {"AppSettings:EncryptionActionKey", "0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef"},
                {"AppSettings:EncryptionActionIV", "TestIV123456"}
            })
            .Build();
            
        AppSettingFile.Initialize(config);

        // Act & Assert - Multiple calls should work consistently
        var plaintext = "Test static initialization";
        
        var encrypted1 = Security.AcEnc(plaintext);
        var encrypted2 = Security.AcEnc(plaintext);
        
        var decrypted1 = Security.AcDec(encrypted1);
        var decrypted2 = Security.AcDec(encrypted2);
        
        decrypted1.Should().Be(plaintext);
        decrypted2.Should().Be(plaintext);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t\n\r")]
    public void AcEnc_WithInvalidConfiguration_ShouldReturnEmptyForInvalidInput(string input)
    {
        // Arrange
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                {"AppSettings:EncryptionActionKey", ""},
                {"AppSettings:EncryptionActionIV", ""}
            })
            .Build();
            
        AppSettingFile.Initialize(config);

        // Act
        var result = Security.AcEnc(input);

        // Assert
        result.Should().Be(string.Empty);
    }

    [Fact]
    public void Security_HexToBytes_PrivateMethod_ShouldWorkCorrectly()
    {
        // This test verifies the hex conversion through public method behavior
        // Arrange
        var validHex = "0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef";
        var testIV = "TestIV123456";
        var testData = "Test hex conversion";

        // Act - If HexToBytes works correctly, this should not throw
        Action act = () =>
        {
            var encrypted = Security.Encrypt(testData, validHex, testIV);
            var decrypted = Security.Decrypt(encrypted, validHex, testIV);
        };

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Security_ConcurrentAccess_ShouldBeSafe()
    {
        // Arrange
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                {"AppSettings:EncryptionActionKey", "0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef"},
                {"AppSettings:EncryptionActionIV", "TestIV123456"}
            })
            .Build();
            
        AppSettingFile.Initialize(config);

        const string testData = "Concurrent test data";
        const int threadCount = 10;
        const int operationsPerThread = 50;

        var tasks = new List<Task>();
        var results = new ConcurrentBag<bool>();

        // Act
        for (int i = 0; i < threadCount; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                try
                {
                    for (int j = 0; j < operationsPerThread; j++)
                    {
                        var encrypted = Security.AcEnc(testData);
                        var decrypted = Security.AcDec(encrypted);
                        results.Add(decrypted == testData);
                    }
                }
                catch
                {
                    results.Add(false);
                }
            }));
        }

        Task.WaitAll(tasks.ToArray());

        // Assert
        results.Should().AllSatisfy(result => result.Should().BeTrue());
        results.Count.Should().Be(threadCount * operationsPerThread);
    }

    [Fact]
    public void Security_MemoryUsage_ShouldNotLeak()
    {
        // Arrange
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                {"AppSettings:EncryptionActionKey", "0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef"},
                {"AppSettings:EncryptionActionIV", "TestIV123456"}
            })
            .Build();
            
        AppSettingFile.Initialize(config);

        var initialMemory = GC.GetTotalMemory(true);

        // Act - Perform many operations
        for (int i = 0; i < 1000; i++)
        {
            var plaintext = $"Memory test iteration {i}";
            var encrypted = Security.AcEnc(plaintext);
            var decrypted = Security.AcDec(encrypted);
            
            if (decrypted != plaintext)
            {
                throw new InvalidOperationException($"Encryption/Decryption failed at iteration {i}");
            }
        }

        var finalMemory = GC.GetTotalMemory(true);

        // Assert - Memory usage should not grow significantly
        var memoryGrowth = finalMemory - initialMemory;
        memoryGrowth.Should().BeLessThan(1024 * 1024); // Less than 1MB growth
    }
}