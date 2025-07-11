﻿using Application.Common.ResultPattern;
using Application.Common.Tools.Pagination;
using Application.Contracts.Repositories;
using Application.UseCases.Bidding.DTOs;
using Domain.Entities;
using Domain.Enumerations;
using FluentValidation;
using MediatR;

namespace Application.UseCases.Bidding.Queries {
	public class GetBidsQuery : IRequest<Result<PagedResponse<BidDTO>>> {
		public Guid UserId { get; set; }
		public int PageNumber { get; set; }
		public int PageSize { get; set; }
		public string SortBy { get; set; }
		public bool SortDesc { get; set; }
	}

	public class GetBidsQueryHandler : IRequestHandler<GetBidsQuery, Result<PagedResponse<BidDTO>>> {

		private readonly IBidRepository _bidRepository;

		// Injecting the dependencies through the constructor.
		public GetBidsQueryHandler(IBidRepository bidRepository) {
			_bidRepository = bidRepository;
		}

		public async Task<Result<PagedResponse<BidDTO>>> Handle(GetBidsQuery request, CancellationToken cancellationToken) {

			// Get all bids for the user + auctions
			var pagedBids = await _bidRepository.GetAllByBidderNoTracking(request.UserId)
												.ToPagedResponseAsync(request.PageNumber, request.PageSize, request.SortBy, request.SortDesc);


			// Map to DTO
			return Result<PagedResponse<BidDTO>>.Success(Map(pagedBids));
		}

		private static PagedResponse<BidDTO> Map(PagedResponse<Bid> pagedBids) {

			var pagedDto = new PagedResponse<BidDTO> {
				PageNumber = pagedBids.PageNumber,
				PageSize = pagedBids.PageSize,
				TotalRecords = pagedBids.TotalRecords,
				Items = new List<BidDTO>()
			};

			foreach (var bid in pagedBids.Items) {

				var bidDto = new BidDTO {
					Id = bid.Id,
					Amount = bid.Amount,
					AuctionId = bid.AuctionId,
					IsWinningBid = bid.IsWinningBid,
					IsExpired = bid.Auction.Status == (int)AuctionStatusEnum.Ended
				};
				pagedDto.Items.Add(bidDto);
			}

			return pagedDto;
		}
	}

	public class GetBidsQueryValidator : AbstractValidator<GetBidsQuery> {
		public GetBidsQueryValidator() {
			RuleFor(x => x.UserId)
				.NotEmpty().WithMessage("User ID cannot be empty.");
		}
	}
}
