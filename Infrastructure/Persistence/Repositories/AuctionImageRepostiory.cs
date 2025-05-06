using Application.Contracts.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Repositories.Common;

namespace Infrastructure.Persistence.Repositories {
	public class AuctionImageRepostiory : BaseRepository<AuctionImage>, IAuctionImageRepostiory {
		public AuctionImageRepostiory(DatabaseContext context) : base(context) {
		}
	}
}
