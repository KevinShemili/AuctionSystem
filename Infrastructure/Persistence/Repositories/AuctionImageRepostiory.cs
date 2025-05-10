using Application.Contracts.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Repositories.Common;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories {
	public class AuctionImageRepostiory : BaseRepository<AuctionImage>, IAuctionImageRepostiory {
		public AuctionImageRepostiory(DatabaseContext context) : base(context) {
		}

		public async Task<AuctionImage> GetByFilePathAsync(string path, CancellationToken cancellationToken) {
			var image = await SetTracking().FirstOrDefaultAsync(x => x.FilePath == path, cancellationToken);
			return image;
		}
	}
}
