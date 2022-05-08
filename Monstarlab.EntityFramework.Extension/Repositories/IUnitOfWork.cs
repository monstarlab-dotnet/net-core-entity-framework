namespace Monstarlab.EntityFramework.Extension.Repositories;

public interface IUnitOfWork
{
    Task CommitAsync(CancellationToken cancellationToken = default);
    void Rollback();
}
