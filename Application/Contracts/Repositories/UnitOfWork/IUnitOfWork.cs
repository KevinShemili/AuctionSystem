namespace Application.Contracts.Repositories.UnitOfWork {
	public interface IUnitOfWork {
		Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default);
	}
}
