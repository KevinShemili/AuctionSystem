﻿using Domain.Entities;
using Infrastructure.EntityConfigurations.Seeds;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations {
	public class UserRoleEntityConfiguration : IEntityTypeConfiguration<UserRole> {
		public void Configure(EntityTypeBuilder<UserRole> builder) {
			builder.HasKey(x => x.Id);

			builder.HasOne(x => x.User)
				   .WithMany(x => x.UserRoles)
				   .HasForeignKey(x => x.UserId);

			builder.HasOne(x => x.Role)
				   .WithMany(x => x.UserRoles)
				   .HasForeignKey(x => x.RoleId);

			// Seed Data
			builder.HasData(SeedData.UserRoles);
		}
	}
}
