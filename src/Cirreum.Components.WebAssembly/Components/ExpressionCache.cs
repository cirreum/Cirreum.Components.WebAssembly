namespace Cirreum.Components;

using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

internal static class ExpressionCache<T> {

	private static readonly ConcurrentDictionary<string, ExpressionFuncItem> expressionCache
		= new ConcurrentDictionary<string, ExpressionFuncItem>();

	public static Expression<Func<T, object>> GetPropertyExpression(string propertyName) {

		var exp = expressionCache.GetOrAdd(propertyName, (key) => {

			var type = typeof(T);
			var param = Expression.Parameter(type, "x");
			Expression body = param;
			foreach (var member in key.Split('.')) {
				body = Expression.PropertyOrField(body, member);
			}
			var convertExp = Expression.Convert(body, typeof(object));

			return new ExpressionFuncItem<T> {
				ItemExpression = Expression.Lambda<Func<T, object>>(convertExp, param)
			};

		}) as ExpressionFuncItem<T>;

		return exp!.ItemExpression;

	}

}

internal abstract record ExpressionFuncItem {
}

internal sealed record ExpressionFuncItem<T> : ExpressionFuncItem {
	[AllowNull]
	public Expression<Func<T, object>> ItemExpression { get; init; }
}