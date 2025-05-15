using Application.Common.ResultPattern;
using Application.Common.Tools.Pagination;
using Application.Contracts.Repositories;
using Application.UseCases.Auctions.DTOs;
using Domain.Entities;
using FluentValidation;
using MediatR;

namespace Application.UseCases.Auctions.Queries {
	public class GetAllAuctionsQuery : IRequest<Result<PagedResponse<AuctionDTO>>> {
		public int PageNumber { get; set; }
		public int PageSize { get; set; }
		public string Filter { get; set; }
		public string SortBy { get; set; }
		public bool SortDesc { get; set; }
		public bool ActiveOnly { get; set; }
	}

	public class GetAllAuctionsQueryHandler : IRequestHandler<GetAllAuctionsQuery, Result<PagedResponse<AuctionDTO>>> {

		private readonly IAuctionRepository _auctionRepository;

		// Injecting the dependencies through the constructor.
		public GetAllAuctionsQueryHandler(IAuctionRepository auctionRepository) {
			_auctionRepository = auctionRepository;
		}

		public async Task<Result<PagedResponse<AuctionDTO>>> Handle(GetAllAuctionsQuery request, CancellationToken cancellationToken) {

			// Based on the Active only, get either all auctions or only active auctions
			// With:
			// 1. Images
			// 2. Bids
			var pagedAuctions = await _auctionRepository.GetAllNoTracking(request.ActiveOnly)
														.ToPagedResponseAsync(request.Filter, request.PageNumber, request.PageSize,
																				request.SortBy, request.SortDesc);

			// Map the paged auctions to DTOs
			// In the dto we only want to show the fact there exists bids
			// But not the bid amount themselves
			return Result<PagedResponse<AuctionDTO>>.Success(Map(pagedAuctions));
		}

		private static PagedResponse<AuctionDTO> Map(PagedResponse<Auction> pagedAuctions) {

			var pagedDto = new PagedResponse<AuctionDTO> {
				PageNumber = pagedAuctions.PageNumber,
				PageSize = pagedAuctions.PageSize,
				TotalRecords = pagedAuctions.TotalRecords,
				Items = new List<AuctionDTO>()
			};

			foreach (var auction in pagedAuctions.Items) {
				var auctionDto = new AuctionDTO {
					Id = auction.Id,
					Name = auction.Name,
					Description = auction.Description,
					Status = auction.Status,
					BaselinePrice = auction.BaselinePrice,
					StartTime = auction.StartTime,
					EndTime = auction.EndTime,
					Images = auction.Images.Select(x => x.FilePath),
					NumberOfBids = auction.Bids.Count
				};

				pagedDto.Items.Add(auctionDto);
			}
			return pagedDto;
		}
	}

	public class GetAllAuctionsQueryValidator : AbstractValidator<GetAllAuctionsQuery> {
		public GetAllAuctionsQueryValidator() {
			RuleFor(x => x.ActiveOnly)
				.NotNull().WithMessage("Active only cannot be null.");
		}
	}
}
