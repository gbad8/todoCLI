namespace TodoCLI.Tasks;

public interface IHashGenerator
{
    string GenerateHash();
    HashResult ResolveHashPrefix(string prefix, IEnumerable<string> existingHashes);
}