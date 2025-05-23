﻿using Application.Contracts.Repositories.Common;
using Domain.Entities;

namespace Application.Contracts.Repositories {
	public interface IBidRepository : IRepository<Bid> {
		IQueryable<Bid> GetAllByBidderNoTracking(Guid userId);
	}
}
