using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace Application.Common.Tools.Pagination {
	public static class Pagination {

		// The method applies sorting, and pagination to a queryable data source.
		// We use the IQueryable interface which is a type for holding collections.
		// With it we build up a query step by step without actually running it.
		// Of course therefore, the key benefit is the deferred execution it offers. Nothing runs until you we explicitly request it.
		// The method returns a generic paged response which contains data:
		// 1. Page Number
		// 2. Page Size
		// 3. Total Records
		// 4. Items
		public static async Task<PagedResponse<T>> ToPagedResponseAsync<T>(this IQueryable<T> data, int pageNumber, int pageSize, string sortBy, bool isDescending) {

			// Ensure the page number and page size are valid
			if (pageNumber < 1)
				pageNumber = 1;

			if (pageSize < 1)
				pageSize = 10;

			if (pageSize > 100)
				pageSize = 100;

			// Apply sorting based on the property name and direction
			var sorted = ApplySorting(data, sortBy, isDescending);

			var total = await sorted.CountAsync();

			// Calculate the number of items to skip based on the page number and size
			var skip = (pageNumber - 1) * pageSize;

			// Get the items only for the current page
			var items = await sorted.Skip(skip)
									.Take(pageSize)
									.ToListAsync();

			return new PagedResponse<T> {
				PageNumber = pageNumber,
				PageSize = pageSize,
				TotalRecords = total,
				Items = items
			};
		}

		// Applies sorting to the collection based on the property name.
		private static IQueryable<T> ApplySorting<T>(IQueryable<T> data, string sortBy, bool isDescending) {

			// If the sortBy is null or empty, return original
			if (string.IsNullOrWhiteSpace(sortBy))
				return data;

			// Check if provided sortBy exists
			var propertyInfo = typeof(T).GetProperty(sortBy, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

			if (propertyInfo == null)
				return data;

			var parameter = Expression.Parameter(data.ElementType, "");
			var property = Expression.PropertyOrField(parameter, sortBy);
			var lambda = Expression.Lambda(property, parameter);

			var result = Expression.Call(
				typeof(Queryable),
				isDescending == true ? nameof(Queryable.OrderByDescending) : nameof(Queryable.OrderBy),
				new Type[] { data.ElementType, property.Type },
				data.Expression,
				Expression.Quote(lambda));

			return data.Provider.CreateQuery<T>(result);
		}
	}
}