namespace Monstarlab.EntityFramework.Extension.Models;

public class ListWrapper<T>
{
    public IEnumerable<T> Data { get; set; }

    public MetaData Meta { get; set; }
}
