using Domain.Common;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Persistence {

	// Extension methods for ModelBuilder
	public static class ModelBuilderExtension {

		// Method 1. Soft Delete Support
		// Get all entities inheriting from EntityBase
		// - Add global query filter to automatically apply IsDeleted == false
		// - Create index on the IsDeleted column to speed up queries
		public static void AddSoftDeleteGlobalFilter(this ModelBuilder builder) {

			foreach (var entityType in builder.Model.GetEntityTypes()) {

				if (typeof(EntityBase).IsAssignableFrom(entityType.ClrType)) {

					// Apply query
					builder
						.Entity(entityType.ClrType)
						.HasQueryFilter(GetSoftDeleteFilter(entityType.ClrType));

					// Apply index
					builder
						.Entity(entityType.ClrType)
						.HasIndex(nameof(EntityBase.IsDeleted))
						.HasFilter("\"IsDeleted\" = false");
				}
			}
		}

		// Build a lambda expression equivalen to e => e.IsDeleted == false
		// needed by EF to translate into SQL
		private static LambdaExpression GetSoftDeleteFilter(Type entityType) {
			// Create lambda -> e => ... 
			var parameter = Expression.Parameter(entityType, "e");

			// Add IsDeleted property -> e.IsDeleted
			var property = Expression.Property(parameter, nameof(EntityBase.IsDeleted));

			// Compare -> e.IsDeleted == false
			var condition = Expression.Equal(property, Expression.Constant(false));

			// Finalize into lambda -> e => e.IsDeleted == false
			var lambda = Expression.Lambda(condition, parameter);

			return lambda;
		}
	}
}
