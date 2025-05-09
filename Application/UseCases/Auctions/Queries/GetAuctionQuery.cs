using Application.Common.ErrorMessages;
using Application.Common.ResultPattern;
using Application.Contracts.Repositories;
using Application.UseCases.Auctions.DTOs;
using Domain.Entities;
using FluentValidation;
using MediatR;

namespace Application.UseCases.Auctions.Queries {
	public class GetAuctionQuery : IRequest<Result<AuctionDetailsDTO>> {
		public Guid AuctionId { get; set; }
	}

	public class GetAuctionQueryHandler : IRequestHandler<GetAuctionQuery, Result<AuctionDetailsDTO>> {

		private readonly IAuctionRepository _auctionRepository;

		public GetAuctionQueryHandler(IAuctionRepository auctionRepository) {
			_auctionRepository = auctionRepository;
		}

		public async Task<Result<AuctionDetailsDTO>> Handle(GetAuctionQuery request, CancellationToken cancellationToken) {

			var auction = await _auctionRepository.GetAuctionWithBidsSellerNoTrackingAsync(request.AuctionId, cancellationToken);

			if (auction is null) {
				return Result<AuctionDetailsDTO>.Failure(Errors.AuctionNotFound(request.AuctionId));
			}

			return Result<AuctionDetailsDTO>.Success(Map(auction));
		}

		private static AuctionDetailsDTO Map(Auction auction) {

			var dto = new AuctionDetailsDTO {
				Id = auction.Id,
				Name = auction.Name,
				Description = auction.Description,
				BaselinePrice = auction.BaselinePrice,
				StartTime = auction.StartTime,
				EndTime = auction.EndTime,
				Status = auction.Status,
				SellerFirstName = auction.Seller.FirstName,
				SellerLastName = auction.Seller.LastName,
				SellerEmail = auction.Seller.Email,
				Images = auction.Images.Select(x => x.FilePath),
				Bidders = auction.Bids.Select(x => new BiddersDTO {
					FirstName = x.Bidder.FirstName,
					LastName = x.Bidder.LastName
				})
			};

			return dto;
		}
	}

	public class GetAuctionQueryValidator : AbstractValidator<GetAuctionQuery> {
		public GetAuctionQueryValidator() {
			RuleFor(x => x.AuctionId)
				.NotEmpty().WithMessage("AuctionId is required");
		}
	}
}
