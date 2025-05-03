using Application.Contracts.Repositories.Common;
using Domain.Entities;

namespace Application.Contracts.Repositories {
	public interface IUserRepository : IRepository<User> {
		Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
		Task<bool> DoesEmailExistAsync(string email, CancellationToken cancellationToken = default);
	}
}
