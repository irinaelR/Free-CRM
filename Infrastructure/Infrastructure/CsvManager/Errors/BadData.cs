namespace Infrastructure.CsvManager.Errors;

public class BadData
{
    public string FileName { get; set; }
    public Dictionary<int, string> Rows { get; set; } = new Dictionary<int, string>();
}