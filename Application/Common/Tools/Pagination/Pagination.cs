using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Application.Common.Tools.Pagination {
	public static class Pagination {

		public static async Task<PagedResponse<T>> ToPagedResponseAsync<T>(this IQueryable<T> source, string filter, int pageNumber, int pageSize,
			string sortBy, bool isDesc) {

			if (pageNumber < 1)
				pageNumber = 1;

			if (pageSize < 1)
				pageSize = 10;

			if (pageSize > 100)
				pageSize = 100;

			// filter
			var filtered = source.ApplyGlobalFilter(filter);

			// sort
			var sorted = filtered.ApplySorting(sortBy, isDesc);

			// total count
			var total = await sorted.CountAsync();

			// paging
			var skip = (pageNumber - 1) * pageSize;
			var items = await sorted.Skip(skip).Take(pageSize).ToListAsync();

			return new PagedResponse<T> {
				PageNumber = pageNumber,
				PageSize = pageSize,
				TotalRecords = total,
				Items = items
			};
		}

		private static IQueryable<T> ApplyGlobalFilter<T>(this IQueryable<T> source, string filter) {
			if (string.IsNullOrWhiteSpace(filter))
				return source;

			var parameter = Expression.Parameter(typeof(T), "x");
			Expression predicate = null;

			foreach (var prop in typeof(T).GetProperties().Where(p => p.PropertyType == typeof(string))) {
				var member = Expression.Property(parameter, prop);
				var containsCall = Expression.Call(
					member,
					nameof(string.Contains),
					Type.EmptyTypes,
					Expression.Constant(filter, typeof(string)));

				predicate = predicate == null ? containsCall : Expression.OrElse(predicate, containsCall);
			}

			if (predicate == null)
				return source;

			var lambda = Expression.Lambda<Func<T, bool>>(predicate, parameter);
			return source.Where(lambda);
		}

		private static IQueryable<T> ApplySorting<T>(this IQueryable<T> source, string sortBy, bool desc) {
			if (string.IsNullOrWhiteSpace(sortBy))
				return source;

			var parameter = Expression.Parameter(typeof(T), "x");
			var property = Expression.PropertyOrField(parameter, sortBy);
			var lambda = Expression.Lambda(property, parameter);

			var methodName = desc ? nameof(Queryable.OrderByDescending) : nameof(Queryable.OrderBy);
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
