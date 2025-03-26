namespace Infrastructure.CsvManager.Errors;

public class CsvProcessException : Exception
{
    public BadData BadDataList { get; set; }

    public CsvProcessException()
    {
        
    }

    public CsvProcessException(string message, BadData badDataList) : base(message)
    {
        this.BadDataList = badDataList;
    }
}