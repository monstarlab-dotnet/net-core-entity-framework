namespace Monstarlab.EntityFramework.Extension.Repositories;

public interface IUnitOfWork
{
    /// <summary>
    /// Save changes and start new transaction.
    /// </summary>
    Task CommitAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Rollback changes.
    /// </summary>
    void Rollback();
}
