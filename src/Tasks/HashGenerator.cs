namespace TodoCLI.Tasks;

public class HashGenerator : IHashGenerator
{
  public string GenerateHash()
  {
    return Guid.NewGuid().ToString("N")[..12];
  }

  public HashResult ResolveHashPrefix(string prefix, IEnumerable<string> existingHashes)
  {
    if (string.IsNullOrEmpty(prefix))
      throw new ArgumentException("Prefix cannot be null or empty.", nameof(prefix));

    if (prefix.Length < 3)
      throw new ArgumentException("Prefix must be at least 3 characters long.", nameof(prefix));

    var matches = existingHashes
        .Where(hash => hash.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        .ToList();

    return matches.Count switch
    {
      0 => new HashResult
      {
        Success = false,
        ErrorMessage = $"No task found with hash prefix '{prefix}'."
      },
      1 => new HashResult
      {
        Success = true,
        FullHash = matches[0]
      },
      _ => new HashResult
      {
        Success = false,
        ErrorMessage = $"Hash prefix '{prefix}' matches multiple tasks. Please use more characters."
      }
    };
  }
}
