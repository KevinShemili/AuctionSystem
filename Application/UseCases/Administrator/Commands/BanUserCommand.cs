using Application.Common.Broadcast;
using Application.Common.ErrorMessages;
using Application.Common.ResultPattern;
using Application.Contracts.Repositories;
using Application.Contracts.Repositories.UnitOfWork;
using Domain.Entities;
using Domain.Enumerations;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Administrator.Commands {
	public class BanUserCommand : IRequest<Result<bool>> {
		public Guid UserId { get; set; }
		public string Reason { get; set; }
	}

	public class BanUserCommandHandler : IRequestHandler<BanUserCommand, Result<bool>> {

		private readonly IUserRepository _userRepository;
		private readonly ILogger<BanUserCommandHandler> _logger;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IBroadcastService _broadcastService;
		private readonly IBidRepository _bidRepository;
		private readonly IAuctionRepository _auctionRepository;

		public BanUserCommandHandler(IUnitOfWork unitOfWork,
							   ILogger<BanUserCommandHandler> logger,
							   IUserRepository userRepository,
							   IBroadcastService broadcastService,
							   IBidRepository bidRepository,
							   IAuctionRepository auctionRepository) {
			_unitOfWork = unitOfWork;
			_logger = logger;
			_userRepository = userRepository;
			_broadcastService = broadcastService;
			_bidRepository = bidRepository;
			_auctionRepository = auctionRepository;
		}

		public async Task<Result<bool>> Handle(BanUserCommand request, CancellationToken cancellationToken) {

			var user = await _userRepository.GetUserWithAuctionsBidsAsync(request.UserId, cancellationToken: cancellationToken);

			if (user is null) {
				_logger.LogWarning("BanUser attempt failed: User with ID {UserId} does not exist.", request.UserId);
				return Result<bool>.Failure(Errors.UserNotFound(request.UserId));
			}

			if (user.IsBlocked is true) {
				_logger.LogWarning("BanUser attempt failed: User {FirstName} {LastName} is already blocked.", user.FirstName, user.LastName);
				return Result<bool>.Failure(Errors.AlreadyBlocked);
			}

			if (user.IsAdministrator is true) {
				_logger.LogWarning("BanUser attempt failed: User {FirstName} {LastName} is an administrator and cannot be blocked.", user.FirstName, user.LastName);
				return Result<bool>.Failure(Errors.CannotBlockAnotherAdmin);
			}

			if (user.Bids.Any() is true) {
				foreach (var bid in user.Bids) {
					_ = await _bidRepository.DeleteAsync(bid, cancellationToken: cancellationToken);
				}
			}

			if (user.Auctions.Any() is true) {
				foreach (var auction in user.Auctions) {
					foreach (var bid in auction.Bids) {

						bid.Bidder.Wallet.FrozenBalance -= bid.Amount;

						bid.Bidder.Wallet.Transactions.Add(new WalletTransaction {
							Amount = bid.Amount,
							TransactionType = (int)WalletTransactionEnum.Unfreeze,
							DateCreated = DateTime.UtcNow,
						});

						_ = await _userRepository.UpdateAsync(bid.Bidder, cancellationToken: cancellationToken);
						_ = await _bidRepository.DeleteAsync(bid, cancellationToken: cancellationToken);
					}

					_ = await _auctionRepository.DeleteAsync(auction, cancellationToken: cancellationToken);
				}
			}

			user.IsBlocked = true;
			user.BlockReason = request.Reason;

			_ = await _userRepository.UpdateAsync(user, cancellationToken: cancellationToken);
			_ = await _unitOfWork.SaveChangesAsync(cancellationToken);

			// broadcast to client to force a logout if client is currently logged in.
			await Broadcast(request.UserId, request.Reason);

			return Result<bool>.Success(true);
		}

		private async Task Broadcast(Guid userId, string reason) {

			await _broadcastService.PublishAsync("BAN-USER", new {
				UserId = userId,
				Reason = reason
			});
		}
	}

	public class BanUserCommandValidator : AbstractValidator<BanUserCommand> {
		public BanUserCommandValidator() {
			RuleFor(x => x.UserId)
				.NotEmpty().WithMessage("UserId cannot be empty.");
			RuleFor(x => x.Reason)
				.NotEmpty().WithMessage("Reason cannot be empty.");
		}
	}
}