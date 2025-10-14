# DataBase.Tests

This test project provides comprehensive unit tests for the `DataBase` library, with a focus on the Security class encryption and decryption functionality.

## Test Coverage

### SecurityTests.cs
The main test class containing comprehensive tests for all Security class methods:

#### Basic Encrypt/Decrypt Tests
- ✅ `Encrypt_ValidInput_ShouldReturnEncryptedString` - Verifies basic encryption functionality
- ✅ `Decrypt_ValidEncryptedString_ShouldReturnOriginalPlaintext` - Verifies basic decryption functionality  
- ✅ `Encrypt_Decrypt_RoundTrip_ShouldPreserveOriginalData` - Tests various input strings for round-trip integrity
- ✅ `Encrypt_SameInputMultipleTimes_ShouldProduceDifferentResults` - Verifies encryption consistency
- ✅ `Encrypt_EmptyString_ShouldHandleGracefully` - Tests empty string handling

#### AcEnc/AcDec Tests (Action Encryption/Decryption)
- ✅ `AcEnc_ValidInput_ShouldReturnEncryptedString` - Tests high-level encryption method
- ✅ `AcDec_ValidEncryptedString_ShouldReturnOriginalPlaintext` - Tests high-level decryption method
- ✅ `AcEnc_AcDec_RoundTrip_ShouldPreserveOriginalData` - Round-trip testing with various inputs
- ✅ `AcEnc_EmptyString_ShouldReturnEmptyString` - Empty string handling
- ✅ `AcEnc_NullString_ShouldReturnEmptyString` - Null input handling
- ✅ `AcEnc_WhitespaceString_ShouldReturnEmptyString` - Whitespace handling

#### Error Handling Tests
- ✅ `Encrypt_InvalidHexKey_ShouldThrowException` - Tests invalid key formats
- ✅ `Encrypt_NullKey_ShouldThrowException` - Null key handling
- ✅ `Encrypt_NullIV_ShouldThrowException` - Null IV handling
- ✅ `Encrypt_NullPlaintext_ShouldThrowException` - Null plaintext handling
- ✅ `Decrypt_InvalidBase64_ShouldThrowException` - Invalid Base64 input handling
- ✅ `Decrypt_TamperedCiphertext_ShouldThrowException` - Data integrity verification
- ✅ `Decrypt_WrongKey_ShouldThrowException` - Wrong key detection

#### Performance and Load Tests
- ✅ `Encrypt_Decrypt_LargeString_ShouldCompleteInReasonableTime` - Performance with large data
- ✅ `AcEnc_AcDec_MultipleOperations_ShouldBeConsistent` - Consistency over multiple operations

#### Data Integrity Tests
- ✅ `Encrypt_Decrypt_BinaryData_ShouldPreserveData` - Binary data preservation
- ✅ `Encrypt_Different_Keys_ShouldProduceDifferentResults` - Key uniqueness verification

### SecurityIntegrationTests.cs
Integration tests focusing on real-world scenarios:

- ✅ `Security_WithMissingConfiguration_AcEncShouldHandleGracefully` - Missing config handling
- ✅ `Security_WithPartialConfiguration_ShouldHandleGracefully` - Partial config handling
- ✅ `Security_StaticInitialization_ShouldWorkCorrectly` - Static initialization testing
- ✅ `Security_ConcurrentAccess_ShouldBeSafe` - Thread safety verification
- ✅ `Security_MemoryUsage_ShouldNotLeak` - Memory leak prevention

### SecurityEdgeCaseTests.cs
Edge cases and boundary condition tests:

- ✅ `Encrypt_DifferentHexCasing_ShouldWork` - Hex case sensitivity
- ✅ `Encrypt_MaxLengthString_ShouldWork` - Large string handling (64KB)
- ✅ `Encrypt_InvalidIVLength_ShouldThrowException` - IV length validation
- ✅ `Encrypt_SpecialUnicodeCharacters_ShouldPreserveData` - Unicode support
- ✅ `Encrypt_StringWithNullCharacters_ShouldHandle` - Null character handling
- ✅ `Encrypt_WhitespaceCharacters_ShouldPreserve` - Whitespace preservation
- ✅ `Encrypt_AllPrintableAsciiCharacters_ShouldWork` - ASCII character set testing
- ✅ `Security_HighFrequencyOperations_ShouldMaintainAccuracy` - High-frequency operation testing

### SecurityTestHelper.cs
Helper class providing utilities for test data generation:

- `GenerateValidKey()` - Generates random 256-bit AES keys
- `GenerateValidIV()` - Generates random 12-byte IVs for AES-GCM
- `GetFixedTestKey()` - Provides consistent key for reproducible tests
- `GetFixedTestIV()` - Provides consistent IV for reproducible tests
- `GetTestStrings()` - Collection of various test strings including edge cases
- `GetInvalidHexKeys()` - Collection of invalid hex keys for negative testing
- `GetInvalidIVs()` - Collection of invalid IVs for negative testing

## Running the Tests

### Prerequisites
- .NET 9.0 SDK
- Visual Studio 2022 or compatible IDE
- xUnit test runner

### Command Line
```bash
# Navigate to the solution directory
cd OmniServices

# Restore packages
dotnet restore

# Build the solution
dotnet build

# Run all tests
dotnet test

# Run tests with detailed output
dotnet test --verbosity normal

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Visual Studio
1. Open the `OmniServices.sln` solution
2. Build the solution (Ctrl+Shift+B)
3. Open Test Explorer (Test → Test Explorer)
4. Run All Tests or select specific tests to run

## Test Categories

### Positive Tests
- Valid input scenarios
- Round-trip encryption/decryption
- Various data types and sizes
- Configuration scenarios

### Negative Tests
- Invalid keys and IVs
- Corrupted data
- Null inputs
- Configuration errors

### Performance Tests
- Large data handling
- High-frequency operations
- Memory usage verification
- Concurrent access

### Edge Cases
- Unicode characters
- Special characters
- Boundary conditions
- Threading scenarios

## Notes

- Tests use both fixed and random test data for reproducibility and coverage
- All tests are designed to be independent and can run in any order
- Configuration is mocked using in-memory configuration providers
- Thread safety is verified through concurrent access tests
- Performance benchmarks ensure operations complete within reasonable timeframes

## Contributing

When adding new tests:
1. Follow the existing naming conventions
2. Add appropriate test categories (Theory/Fact)
3. Include both positive and negative test cases
4. Document complex test scenarios
5. Ensure tests are deterministic and can be run in parallel