﻿using Application.Contracts.Repositories.Common;
using Domain.Entities;

namespace Application.Contracts.Repositories {
	public interface IAuctionImageRepostiory : IRepository<AuctionImage> {
		Task<AuctionImage> GetByFilePathAsync(string path, CancellationToken cancellationToken);
	}
}
