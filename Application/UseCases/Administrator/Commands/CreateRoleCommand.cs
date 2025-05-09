using Application.Common.ResultPattern;
using Application.Contracts.Repositories;
using Application.Contracts.Repositories.UnitOfWork;
using Domain.Entities;
using FluentValidation;
using MediatR;

namespace Application.UseCases.Administrator.Commands {
	public class CreateRoleCommand : IRequest<Result<Guid>> {
		public string Name { get; set; }
		public string Description { get; set; }
	}

	public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, Result<Guid>> {

		private readonly IRoleRepository _roleRepository;
		private readonly IUnitOfWork _unitOfWork;

		public CreateRoleCommandHandler(IRoleRepository roleRepository, IUnitOfWork unitOfWork) {
			_roleRepository = roleRepository;
			_unitOfWork = unitOfWork;
		}

		public async Task<Result<Guid>> Handle(CreateRoleCommand request, CancellationToken cancellationToken) {

			var role = new Role {
				Name = request.Name,
				Description = request.Description
			};

			_ = await _roleRepository.CreateAsync(role, cancellationToken: cancellationToken);
			_ = await _unitOfWork.SaveChangesAsync(cancellationToken);

			return Result<Guid>.Success(role.Id);
		}
	}

	public class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand> {
		public CreateRoleCommandValidator() {
			RuleFor(x => x.Name)
				.NotEmpty().WithMessage("Role name cannot be empty.")
				.MaximumLength(50).WithMessage("Role name cannot exceed 50 characters.");
			RuleFor(x => x.Description)
				.NotEmpty().WithMessage("Role description cannot be empty.")
				.MaximumLength(200).WithMessage("Role description cannot exceed 200 characters.");
		}
	}
}
