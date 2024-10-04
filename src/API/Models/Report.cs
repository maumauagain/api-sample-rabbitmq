namespace API.Models;

public class Report
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Status { get; set; }

    public Report()
    {
        Id = Guid.NewGuid();
        Status = "Em Processamento";
    }
}

public static class ReportList
{
    public static List<Report> Reports = new();
}
