using TodoCLI.Tasks;

namespace TodoCLI.Tests.Tasks;

public class HashGeneratorTests
{
    [Fact]
    public void GenerateHash_ShouldReturnNonEmptyString()
    {
        // Arrange
        var hashGenerator = new HashGenerator();

        // Act
        var hash = hashGenerator.GenerateHash();

        // Assert
        Assert.NotNull(hash);
        Assert.NotEmpty(hash);
    }

    [Fact]
    public void GenerateHash_CalledMultipleTimes_ShouldReturnUniqueHashes()
    {
        // Arrange
        var hashGenerator = new HashGenerator();

        // Act
        var hash1 = hashGenerator.GenerateHash();
        var hash2 = hashGenerator.GenerateHash();
        var hash3 = hashGenerator.GenerateHash();

        // Assert
        Assert.NotEqual(hash1, hash2);
        Assert.NotEqual(hash2, hash3);
        Assert.NotEqual(hash1, hash3);
    }

    [Fact]
    public void GenerateHash_ShouldReturnHashWithMinimumLength()
    {
        // Arrange
        var hashGenerator = new HashGenerator();

        // Act
        var hash = hashGenerator.GenerateHash();

        // Assert
        Assert.True(hash.Length >= 8, "Hash should be at least 8 characters long");
    }

    [Fact]
    public void ResolveHashPrefix_WithUniqueMatch_ShouldReturnFullHash()
    {
        // Arrange
        var hashGenerator = new HashGenerator();
        var existingHashes = new List<string> { "abc123def456", "xyz789uvw012", "mno345pqr678" };
        var prefix = "abc";

        // Act
        var result = hashGenerator.ResolveHashPrefix(prefix, existingHashes);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("abc123def456", result.FullHash);
        Assert.Empty(result.ErrorMessage);
    }

    [Fact]
    public void ResolveHashPrefix_WithNoMatch_ShouldReturnFailure()
    {
        // Arrange
        var hashGenerator = new HashGenerator();
        var existingHashes = new List<string> { "abc123def456", "xyz789uvw012", "mno345pqr678" };
        var prefix = "zzz";

        // Act
        var result = hashGenerator.ResolveHashPrefix(prefix, existingHashes);

        // Assert
        Assert.False(result.Success);
        Assert.Null(result.FullHash);
        Assert.NotEmpty(result.ErrorMessage);
    }

    [Fact]
    public void ResolveHashPrefix_WithMultipleMatches_ShouldReturnFailure()
    {
        // Arrange
        var hashGenerator = new HashGenerator();
        var existingHashes = new List<string> { "abc123def456", "abc789uvw012", "xyz345pqr678" };
        var prefix = "abc";

        // Act
        var result = hashGenerator.ResolveHashPrefix(prefix, existingHashes);

        // Assert
        Assert.False(result.Success);
        Assert.Null(result.FullHash);
        Assert.Contains("multiple", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ResolveHashPrefix_WithEmptyPrefix_ShouldThrowArgumentException()
    {
        // Arrange
        var hashGenerator = new HashGenerator();
        var existingHashes = new List<string> { "abc123def456" };

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            hashGenerator.ResolveHashPrefix("", existingHashes));
    }

    [Fact]
    public void ResolveHashPrefix_WithShortPrefix_ShouldThrowArgumentException()
    {
        // Arrange
        var hashGenerator = new HashGenerator();
        var existingHashes = new List<string> { "abc123def456" };

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            hashGenerator.ResolveHashPrefix("ab", existingHashes));
    }

    [Fact]
    public void ResolveHashPrefix_CaseInsensitive_ShouldWork()
    {
        // Arrange
        var hashGenerator = new HashGenerator();
        var existingHashes = new List<string> { "ABC123def456", "xyz789uvw012" };
        var prefix = "abc";

        // Act
        var result = hashGenerator.ResolveHashPrefix(prefix, existingHashes);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("ABC123def456", result.FullHash);
    }
}