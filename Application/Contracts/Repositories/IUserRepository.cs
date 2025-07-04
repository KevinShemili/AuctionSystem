﻿using Application.Contracts.Repositories.Common;
using Domain.Entities;

namespace Application.Contracts.Repositories {
	public interface IUserRepository : IRepository<User> {
		Task<User> GetByEmailNoTrackingAsync(string email, CancellationToken cancellationToken = default);
		Task<bool> DoesEmailExistAsync(string email, CancellationToken cancellationToken = default);
		Task<User> GetUserWithAuthenticationTokensNoTrackingAsync(string email, CancellationToken cancellationToken = default);
		Task<User> GetUserWithAuthenticationTokensAsync(string email, CancellationToken cancellationToken = default);
		Task<User> GetUserWithTokensAsync(string email, CancellationToken cancellationToken = default);
		Task<User> GetUserWithRolesAndPermissionsNoTrackingAsync(string email, CancellationToken cancellationToken = default);
		Task<User> GetUserWithWalletAndTransactionsAsync(Guid id, CancellationToken cancellationToken = default);
		Task<User> GetUserWithAuctionWalletBidsRolesPermissionsNoTrackingAsync(Guid id, CancellationToken cancellationToken = default);
		IQueryable<User> GetAllWithRolesPermissionsWalletAuctionsBidsNoTrackingAsync();
		Task<User> GetUserWithRolesPermissionsWalletAuctionsBidsNoTrackingAsync(Guid id, CancellationToken cancellationToken = default);
		Task<User> GetUserWithUserRolesAsync(Guid id, CancellationToken cancellationToken = default);
		Task<User> GetUserWithWalletAndTransactionsNoTrackingAsync(Guid id, CancellationToken cancellationToken = default);
		Task<User> GetUserWithAuctionsBidsAsync(Guid id, CancellationToken cancellationToken = default);
		IQueryable<User> SetNoTracking(string filter);
	}
}
