using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Application.Common.Tools.Pagination {
	public static class Pagination {

		// This method applies filtering, sorting, and pagination to a queryable data source.
		// We use the IQueryable interface which is a type for holding collections.
		// With it we build up a query step by step without actually running it.
		// Of course therefore, the key benefit is the deferred execution it offers. Nothing runs until you we explicitly request it.
		// The method returns a generic paged response which contains data:
		// 1. Page Number
		// 2. Page Size
		// 3. Total Records
		// 4. Items
		public static async Task<PagedResponse<T>> ToPagedResponseAsync<T>(this IQueryable<T> source, string filter, int pageNumber, int pageSize,
			string sortBy, bool isDescending) {

			// Ensure the page number and page size are valid
			if (pageNumber < 1)
				pageNumber = 1;

			if (pageSize < 1)
				pageSize = 10;

			if (pageSize > 100)
				pageSize = 100;

			// Apply a filter all properties
			var filtered = source.ApplyGlobalFilter(filter);

			// Apply sorting based on the property name and direction
			var sorted = filtered.ApplySorting(sortBy, isDescending);

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

		// We apply a search filter to all the properties of the entity of type string.
		private static IQueryable<T> ApplyGlobalFilter<T>(this IQueryable<T> source, string filter) {

			// If the filter is null simply return
			if (string.IsNullOrWhiteSpace(filter))
				return source;

			// Create parameter for expression: x =>
			var parameter = Expression.Parameter(typeof(T), "x");
			Expression predicate = null;

			// Iterate all string properties of the entity
			foreach (var prop in typeof(T).GetProperties().Where(p => p.PropertyType == typeof(string))) {
				// Build x.Prop
				var member = Expression.Property(parameter, prop);

				// Build x.Prop.Contains(filter)
				var containsCall = Expression.Call(
					member,
					nameof(string.Contains),
					Type.EmptyTypes,
					Expression.Constant(filter, typeof(string)));

				// Combine conditions using OR for multiple string properties
				predicate = predicate == null ? containsCall : Expression.OrElse(predicate, containsCall);
			}

			// If no string properties were found, return the original
			if (predicate == null)
				return source;

			// Final expression: x => x.Prop1.Contains(...) || x.Prop2.Contains(...)
			var lambda = Expression.Lambda<Func<T, bool>>(predicate, parameter);

			// Apply the filter to the source
			return source.Where(lambda);
		}

		// Applies sorting to the collection based on the property name.
		private static IQueryable<T> ApplySorting<T>(this IQueryable<T> source, string sortBy, bool descending) {

			// If the sortBy is null or empty, return original
			if (string.IsNullOrWhiteSpace(sortBy))
				return source;

			// Build expression: x =>
			var parameter = Expression.Parameter(typeof(T), "x");

			// Access the property by name: x.SortBy
			var property = Expression.PropertyOrField(parameter, sortBy);

			// Build lambda: x => x.SortBy
			var lambda = Expression.Lambda(property, parameter);

			// Choose OrderBy or OrderByDescending based on direction
			var methodName = descending ? nameof(Queryable.OrderByDescending) : nameof(Queryable.OrderBy);

			// Build expression -> Queryable.OrderBy(x => x.SortBy)
			var result = Expression.Call(
				typeof(Queryable),
				methodName,
				new Type[] { typeof(T), property.Type },
				source.Expression,
				Expression.Quote(lambda));

			return source.Provider.CreateQuery<T>(result);
		}
	}
}
