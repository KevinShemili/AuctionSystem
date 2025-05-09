using Application.Common.ErrorMessages;
using Application.Common.ResultPattern;
using Application.Common.Tools.Time;
using Application.Contracts.Repositories;
using Application.Contracts.Repositories.UnitOfWork;
using Domain.Entities;
using FluentValidation;
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
		public DateTime EndTime { get; set; }
		public IEnumerable<string> NewImages { get; set; }
	}

	public class UpdateAuctionCommandHandler : IRequestHandler<UpdateAuctionCommand, Result<bool>> {
		private readonly IUnitOfWork _unitOfWork;
		private readonly IAuctionRepository _auctionRepository;
		private readonly ILogger<UpdateAuctionCommandHandler> _logger;
		private readonly IWebHostEnvironment _webHostEnvironment;

		public UpdateAuctionCommandHandler(IUnitOfWork unitOfWork,
									 IAuctionRepository auctionRepository,
									 ILogger<UpdateAuctionCommandHandler> logger,
									 IWebHostEnvironment webHostEnvironment) {
			_unitOfWork = unitOfWork;
			_auctionRepository = auctionRepository;
			_logger = logger;
			_webHostEnvironment = webHostEnvironment;
		}

		public async Task<Result<bool>> Handle(UpdateAuctionCommand request, CancellationToken cancellationToken) {

			var auction = await _auctionRepository.GetAuctionWithBidsAsync(request.AuctionId, cancellationToken);

			if (auction is null) {
				_logger.LogWarning("Update Auction attempt failed, auction not found. AuctionId: {AuctionId}.", request.AuctionId);
				return Result<bool>.Failure(Errors.AuctionNotFound(request.AuctionId));
			}

			if (auction.SellerId != request.UserId)
				return Result<bool>.Failure(Errors.ChangeOnlyOwnAuctions);

			if (auction.Bids.Any())
				return Result<bool>.Failure(Errors.AuctionHasBids);

			var startTime = TruncateTime.ToMinute(DateTime.UtcNow);
			request.EndTime = TruncateTime.ToMinute(request.EndTime);

			if (request.EndTime <= startTime) {
				_logger.LogWarning("Create Auction attempt failed, invalid end time. StartTime: {StartTime} EndTime: {EndTime}", startTime, request.EndTime);
				return Result<bool>.Failure(Errors.EndSmallerEqualStart);
			}

			var uploadRoot = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "auctions");

			var oldImages = auction.Images.ToList();
			foreach (var img in oldImages) {
				try {
					var diskPath = Path.Combine(_webHostEnvironment.WebRootPath, img.FilePath.Replace("/", Path.DirectorySeparatorChar.ToString()));

					if (File.Exists(diskPath))
						File.Delete(diskPath);
				}
				catch (Exception ex) {
					_logger.LogWarning(ex, "Failed deleting old image file {FilePath}", img.FilePath);
				}
			}

			auction.Name = request.Name;
			auction.Description = request.Description;
			auction.BaselinePrice = request.BaselinePrice;
			auction.StartTime = startTime;
			auction.EndTime = request.EndTime;

			if (request.NewImages != null && request.NewImages.Any()) {
				auction.Images.Clear();
				foreach (var img in request.NewImages) {
					auction.Images.Add(new AuctionImage {
						FilePath = img,
						DateCreated = DateTime.UtcNow
					});
				}
			}

			_ = await _auctionRepository.UpdateAsync(auction, cancellationToken: cancellationToken);
			_ = await _unitOfWork.SaveChangesAsync(cancellationToken);

			return Result<bool>.Success(true);
		}

		public class UpdateAuctionCommandValidator : AbstractValidator<UpdateAuctionCommand> {
			public UpdateAuctionCommandValidator() {
				RuleFor(x => x.Name)
					.NotEmpty().WithMessage("Auction title is required.");
				RuleFor(x => x.BaselinePrice)
					.NotEmpty().WithMessage("Baseline price must be > 0.");
				RuleFor(x => x.EndTime)
					.NotEmpty().WithMessage("End time must be after start time.");
			}
		}

	}
}