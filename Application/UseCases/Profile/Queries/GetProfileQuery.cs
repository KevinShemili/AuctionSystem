using Application.Common.ErrorMessages;
using Application.Common.ResultPattern;
using Application.Contracts.Repositories;
using Application.DTOs;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Profile.Queries {
	public class GetProfileQuery : IRequest<Result<ProfileDTO>> {
		public Guid UserId { get; set; }
	}

	public class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, Result<ProfileDTO>> {

		private readonly IUserRepository _userRepository;
		private readonly ILogger<GetProfileQueryHandler> _logger;
		private readonly IAuctionRepository _auctionRepository;

		public GetProfileQueryHandler(ILogger<GetProfileQueryHandler> logger, IUserRepository userRepository, IAuctionRepository auctionRepository) {
			_logger = logger;
			_userRepository = userRepository;
			_auctionRepository = auctionRepository;
		}

		public async Task<Result<ProfileDTO>> Handle(GetProfileQuery request, CancellationToken cancellationToken) {

			var user = await _userRepository.GetUserWithWalletAndTransactionsNoTrackingAsync(request.UserId, cancellationToken);

			if (user is null) {
				_logger.LogCritical("User bypassed authorization. {UserId} not found", request.UserId);
				return Result<ProfileDTO>.Failure(Errors.Unauthorized);
			}

			var createdAuctions = await _auctionRepository.GetAuctionsCreatedByUserNoTrackingAsync(request.UserId, cancellationToken);

			var participatedAuctions = await _auctionRepository.GetAuctionsPartecipatedByUserNoTrackingAsync(request.UserId, cancellationToken);

			return Result<ProfileDTO>.Success(MapResponse(user, createdAuctions, participatedAuctions));
		}

		private static ProfileDTO MapResponse(User user, IEnumerable<Auction> createdAuctions, IEnumerable<Auction> participatedAuctions) {

			var profile = new ProfileDTO {
				FirstName = user.FirstName,
				LastName = user.LastName,
				Email = user.Email,
				Wallet = new WalletDTO {
					Balance = user.Wallet.Balance,
					FrozenBalance = user.Wallet.FrozenBalance,
					Transactions = user.Wallet.Transactions.Select(x => new TransactionsDTO {
						Amount = x.Amount,
						TransactionType = x.TransactionType
					})
				},
				CreatedAuctions = createdAuctions.Select(x => new AuctionDTO {
					Name = x.Name,
					BaselinePrice = x.BaselinePrice,
					StartTime = x.StartTime,
					Description = x.Description,
					EndTime = x.EndTime,
					Status = x.Status,
					Images = x.Images.Select(x => new AuctionImageDTO {
						Url = x.FilePath
					}),
				}),
				ParticipatedAuctions = participatedAuctions.Select(x => new AuctionDTO {
					Name = x.Name,
					BaselinePrice = x.BaselinePrice,
					StartTime = x.StartTime,
					Description = x.Description,
					EndTime = x.EndTime,
					Status = x.Status,
					Images = x.Images.Select(x => new AuctionImageDTO {
						Url = x.FilePath
					}),
					Bids = x.Bids.Select(x => new BidDTO {
						Amount = x.Amount,
						IsWinningBid = x.IsWinningBid
					})
				})
			};

			return profile;
		}
	}
}
