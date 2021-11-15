namespace Monstarlab.EntityFramework.Extension.Models;

public class MetaData
{
    public int Total { get; set; }
    public int PerPage { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)Total / PerPage);
    public int RecordsInDataset { get; set; }
}
