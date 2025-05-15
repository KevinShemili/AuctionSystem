using Domain.Common;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Persistence {

	// Extension methods for ModelBuilder
	public static class ModelBuilderExtension {

		// Method implements soft delete support
		// Get all entities inheriting from EntityBase
		// - Add global query filter to automatically apply IsDeleted == false
		// - Create index on the IsDeleted column to speed up queries
		public static void SoftDeleteFilter(this ModelBuilder builder) {

			foreach (var entityType in builder.Model.GetEntityTypes()) {

				if (typeof(AbstractEntity).IsAssignableFrom(entityType.ClrType)) {

					// Apply query
					builder
						.Entity(entityType.ClrType)
						.HasQueryFilter(GetSoftDeleteFilter(entityType.ClrType));

					// Apply index
					builder
						.Entity(entityType.ClrType)
						.HasIndex(nameof(AbstractEntity.IsDeleted))
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
			var property = Expression.Property(parameter, nameof(AbstractEntity.IsDeleted));

			// Compare -> e.IsDeleted == false
			var condition = Expression.Equal(property, Expression.Constant(false));

			// Finalize into lambda -> e => e.IsDeleted == false
			var lambda = Expression.Lambda(condition, parameter);

			return lambda;
		}
	}
}
