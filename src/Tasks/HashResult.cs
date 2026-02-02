namespace TodoCLI.Tasks;

public class HashResult
{
    public bool Success { get; set; }
    public string? FullHash { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}