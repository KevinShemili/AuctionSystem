﻿using Application.Contracts.Repositories.Common;
using Domain.Entities;

namespace Application.Contracts.Repositories {
	public interface IAuthenticationTokenRepository : IRepository<AuthenticationToken> {
		Task<AuthenticationToken> GetByUserID(Guid userId, CancellationToken cancellationToken = default);
	}
}
