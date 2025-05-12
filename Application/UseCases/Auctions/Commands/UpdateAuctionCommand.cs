using Application.Common.ErrorMessages;
using Application.Common.ResultPattern;
using Application.Common.Tools.Time;
using Application.Contracts.Repositories;
using Application.Contracts.Repositories.UnitOfWork;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Auctions.Commands {
	public class UpdateAuctionCommand : IRequest<Result<bool>> {
		public Guid AuctionId { get; set; }
		public Guid UserId { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public decimal BaselinePrice { get; set; }
		public DateTime? EndTime { get; set; }
		public IEnumerable<string> NewImages { get; set; }
		public IEnumerable<string> RemoveImages { get; set; }
	}

	public class UpdateAuctionCommandHandler : IRequestHandler<UpdateAuctionCommand, Result<bool>> {

		private readonly IUnitOfWork _unitOfWork;
		private readonly IAuctionRepository _auctionRepository;
		private readonly ILogger<UpdateAuctionCommandHandler> _logger;
		private readonly IWebHostEnvironment _webHostEnvironment;
		private readonly IAuctionImageRepostiory _auctionImageRepostiory;

		// Injecting the dependencies through the constructor.
		public UpdateAuctionCommandHandler(IUnitOfWork unitOfWork,
									 IAuctionRepository auctionRepository,
									 ILogger<UpdateAuctionCommandHandler> logger,
									 IWebHostEnvironment webHostEnvironment,
									 IAuctionImageRepostiory auctionImageRepostiory) {
			_unitOfWork = unitOfWork;
			_auctionRepository = auctionRepository;
			_logger = logger;
			_webHostEnvironment = webHostEnvironment;
			_auctionImageRepostiory = auctionImageRepostiory;
		}

		public async Task<Result<bool>> Handle(UpdateAuctionCommand request, CancellationToken cancellationToken) {

			// Get the auction with:
			// 1. Bids
			// 2. Images
			var auction = await _auctionRepository.GetAuctionWithBidsAsync(request.AuctionId, cancellationToken);

			// Check if the auction exists
			if (auction is null) {
				_logger.LogWarning("Update Auction attempt failed, auction not found. AuctionId: {AuctionId}.", request.AuctionId);
				return Result<bool>.Failure(Errors.AuctionNotFound(request.AuctionId));
			}

			// Check if the user performing the update is the seller of the auction
			if (auction.SellerId != request.UserId)
				return Result<bool>.Failure(Errors.ChangeOnlyOwnAuctions);

			// Check whether the auction has active bids
			if (auction.Bids.Any())
				return Result<bool>.Failure(Errors.AuctionHasBids);

			// Update the endtime if provided
			if (request.EndTime.HasValue is true) {

				// Check if the end time is in the future
				if (request.EndTime <= DateTime.UtcNow) {
					_logger.LogWarning("Create Auction attempt failed, invalid end time. StartTime: {StartTime} EndTime: {EndTime}", request.EndTime, request.EndTime);
					return Result<bool>.Failure(Errors.EndSmallerEqualStart);
				}

				auction.EndTime = TruncateTime.ToMinute((DateTime)request.EndTime);
			}

			// Check if user wants to remove images
			if (request.RemoveImages != null && request.RemoveImages.Any()) {

				// Ensure that after removal we sill have at least one image
				var existingImages = auction.Images.Count;
				var imagesToRemove = request.RemoveImages?.Count() ?? 0;
				var imagesToAdd = request.NewImages?.Count() ?? 0;

				if ((imagesToRemove > 0) &&
					(existingImages - imagesToRemove + imagesToAdd < 1))
					return Result<bool>.Failure(Errors.AtLeastOneImage);

				// Remove the images from the auction
				foreach (var img in request.RemoveImages) {
					try {
						// Delete them from disk
						var diskPath = Path.Combine(_webHostEnvironment.WebRootPath, img.Replace("/", Path.DirectorySeparatorChar.ToString()));

						if (File.Exists(diskPath))
							File.Delete(diskPath);

						// Delete file path from database
						var auctionImage = await _auctionImageRepostiory.GetByFilePathAsync(img, cancellationToken: cancellationToken);
						if (auctionImage is not null)
							_ = await _auctionImageRepostiory.DeleteAsync(auctionImage, cancellationToken: cancellationToken);
					}
					catch (Exception ex) {
						_logger.LogWarning(ex, "Failed deleting old image file {FilePath}", img);
					}
				}
			}

			// Update fields if provided
			if (string.IsNullOrWhiteSpace(request.Name) is false) {
				auction.Name = request.Name;
			}

			if (string.IsNullOrWhiteSpace(request.Description) is false) {
				auction.Description = request.Description;
			}

			if (request.BaselinePrice > 0) {
				auction.BaselinePrice = request.BaselinePrice;
			}

			// Add the new image filepaths to the database
			if (request.NewImages != null && request.NewImages.Any()) {

				foreach (var img in request.NewImages) {
					auction.Images.Add(new AuctionImage {
						FilePath = img,
						DateCreated = DateTime.UtcNow
					});
				}
			}

			// Update & Persist
			_ = await _auctionRepository.UpdateAsync(auction, cancellationToken: cancellationToken);
			_ = await _unitOfWork.SaveChangesAsync(cancellationToken);

			return Result<bool>.Success(true);
		}
	}
}