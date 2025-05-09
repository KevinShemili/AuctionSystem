using Application.Common.ErrorMessages;
using Application.Common.ResultPattern;
using Application.Contracts.Repositories;
using Application.UseCases.Administrator.DTOs;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Administrator.Queries {
	public class ViewAuctionAdminQuery : IRequest<Result<AuctionAdminDTO>> {
		public Guid AuctionId { get; set; }
	}

	public class ViewAuctionAdminQueryHandler : IRequestHandler<ViewAuctionAdminQuery, Result<AuctionAdminDTO>> {

		private readonly IAuctionRepository _auctionRepository;
		private readonly ILogger<ViewAuctionAdminQueryHandler> _logger;

		public ViewAuctionAdminQueryHandler(IAuctionRepository auctionRepository, ILogger<ViewAuctionAdminQueryHandler> logger) {
			_auctionRepository = auctionRepository;
			_logger = logger;
		}

		public async Task<Result<AuctionAdminDTO>> Handle(ViewAuctionAdminQuery request, CancellationToken cancellationToken) {

			var auction = await _auctionRepository.GetAuctionWithBidsSellerNoTrackingAsync(request.AuctionId, cancellationToken);

			if (auction == null) {
				_logger.LogWarning("Auction with ID {AuctionId} not found.", request.AuctionId);
				return Result<AuctionAdminDTO>.Failure(Errors.AuctionNotFound(request.AuctionId));
			}

			return Result<AuctionAdminDTO>.Success(MapResponse(auction));
		}

		private static AuctionAdminDTO MapResponse(Auction auction) {

			var profile = new AuctionAdminDTO {
				Id = auction.Id,
				Status = auction.Status,
				BaselinePrice = auction.BaselinePrice,
				Description = auction.Description,
				EndTime = auction.EndTime,
				StartTime = auction.StartTime,
				Name = auction.Name,
				Images = auction.Images.Select(x => x.FilePath),
				Bids = new List<BidAdminDTO>()
			};

			foreach (var bid in auction.Bids) {
				profile.Bids.Add(new BidAdminDTO {
					Id = bid.Id,
					Amount = bid.Amount,
					BidderFirstName = bid.Bidder.FirstName,
					BidderLastName = bid.Bidder.LastName,
					BidderEmail = bid.Bidder.Email,
					IsWinningBid = bid.IsWinningBid
				});
			}

			return profile;
		}
	}

	public class ViewAuctionAdminQueryValidator : AbstractValidator<ViewAuctionAdminQuery> {
		public ViewAuctionAdminQueryValidator() {
			RuleFor(x => x.AuctionId)
				.NotEmpty().WithMessage("Auction ID cannot be empty.");
		}
	}
}
