﻿using Application.Common.ErrorMessages;
using Application.Common.ResultPattern;
using Application.Contracts.Repositories;
using Application.UseCases.Administrator.DTOs;
using FluentValidation;
using MediatR;

namespace Application.UseCases.Administrator.Queries {
	public class ViewPermissionQuery : IRequest<Result<PermissionDTO>> {
		public Guid PermissionId { get; set; }
	}

	public class ViewPermissionQueryHandler : IRequestHandler<ViewPermissionQuery, Result<PermissionDTO>> {
		private readonly IPermissionRepository _permissionRepository;

		public ViewPermissionQueryHandler(IPermissionRepository permissionRepository) {
			_permissionRepository = permissionRepository;
		}

		public async Task<Result<PermissionDTO>> Handle(ViewPermissionQuery request, CancellationToken cancellationToken) {

			// Get permission by ID
			var permission = await _permissionRepository.GetByIdAsync(request.PermissionId, cancellationToken);

			// Check if the permission exists
			if (permission == null)
				return Result<PermissionDTO>.Failure(Errors.PermissionNotFound(request.PermissionId));

			// Map result to DTO
			var permissionDTO = new PermissionDTO {
				Id = permission.Id,
				Key = permission.Key,
				Name = permission.Name,
				Description = permission.Description
			};

			return Result<PermissionDTO>.Success(permissionDTO);
		}
	}

	public class ViewPermissionQueryValidator : AbstractValidator<ViewPermissionQuery> {
		public ViewPermissionQueryValidator() {
			RuleFor(x => x.PermissionId)
				.NotEmpty().WithMessage("PermissionId is required");
		}
	}
}
