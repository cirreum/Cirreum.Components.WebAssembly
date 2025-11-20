namespace Cirreum.Components;

using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

internal static class DelegateCache<T> {

	private static readonly ConcurrentDictionary<string, FuncDelegateItem> delegateCache
		= new ConcurrentDictionary<string, FuncDelegateItem>();

	public static Func<T, object> GetPropertyAccessor(string propertyName) {

		var exp = delegateCache.GetOrAdd(propertyName, (key) => {
			var expression = ExpressionCache<T>.GetPropertyExpression(key);
			return new FuncDelegateItem<T> {
				ItemFunc = expression.Compile()
			};
		}) as FuncDelegateItem<T>;

		return exp!.ItemFunc;

	}

}

internal abstract record FuncDelegateItem {
}

internal sealed record FuncDelegateItem<T> : FuncDelegateItem {
	[AllowNull]
	public Func<T, object> ItemFunc { get; init; }
}