namespace Cirreum.Extensions;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

internal static class TypeExtensions {

	/// <summary>
	/// Retrieves the default value for a given Value Type
	/// </summary>
	/// <param name="type">The Value Type for which to get the default value</param>
	/// <returns>The default value for <paramref name="type"/></returns>
	/// <remarks>
	/// If a null Type, a reference Type, or a System.Void Type is supplied, this method always returns null.  If a value type 
	/// is supplied which is not publicly visible or which contains generic parameters, this method will fail with an 
	/// exception.
	/// </remarks>
	/// <example>
	/// To use this method in its native, non-extension form, make a call like:
	/// <code>
	///     object Default = DefaultValue.GetDefault(someType);
	/// </code>
	/// To use this method in its Type-extension form, make a call like:
	/// <code>
	///     object Default = someType.GetDefault();
	/// </code>
	/// </example>
	/// <seealso cref="GetDefault&lt;T&gt;"/>
	public static object? GetDefault(this Type type) {

		// If no Type was supplied, if the Type was a reference type, or if the Type was a System.Void, return null
		if (type == null || !type.IsValueType || type == typeof(void)) {
			return null;
		}

		// If the supplied Type has generic parameters, its default value cannot be determined
		if (type.ContainsGenericParameters) {
			throw new ArgumentException(
				"{" + MethodBase.GetCurrentMethod() + "} Error:\n\nThe supplied value type <" + type +
				"> contains generic parameters, so the default value cannot be retrieved");
		}

		// If the Type is a primitive type, or if it is another publicly-visible value type (i.e. struct/enum), return a 
		//  default instance of the value type
		if (type.IsPrimitive || type.IsNotPublic is false) {
			try {
				return Activator.CreateInstance(type);
			} catch (Exception e) {
				throw new ArgumentException(
					"{" + MethodBase.GetCurrentMethod() + "} Error:\n\nThe Activator.CreateInstance method could not " +
					"create a default instance of the supplied value type <" + type +
					"> (Inner Exception message: \"" + e.Message + "\")", e);
			}
		}

		// Fail with exception
		throw new ArgumentException("{" + MethodBase.GetCurrentMethod() + "} Error:\n\nThe supplied value type <" + type +
			"> is not a publicly-visible type, so the default value cannot be retrieved");

	}

	/// <summary>
	/// Retrieves the default value for a given Value Type
	/// </summary>
	/// <typeparam name="T">The Value Type for which to get the default value</typeparam>
	/// <returns>The default value for Type T</returns>
	/// <remarks>
	/// If a reference Type or a System.Void Type is supplied, this method always returns null.  If a value type 
	/// is supplied which is not publicly visible or which contains generic parameters, this method will fail with an 
	/// exception.
	/// </remarks>
	/// <seealso cref="GetDefault(Type)"/>
	public static T GetDefault<T>() where T : struct {
		var value = typeof(T).GetDefault();
		if (value is not null) {
			return (T)value;
		}
		return default;
	}

	/// <summary>
	/// Retrieves the casted value for a given Value Type
	/// </summary>
	/// <typeparam name="T">The value type to cast to.</typeparam>
	/// <param name="obj">The object being casted.</param>
	/// <returns>The Casted value or the default value.</returns>
	public static T GetCastedInstanceOrDefault<T>(this object obj) where T : struct {
		if (obj == null) {
			return GetDefault<T>();
		}
		return (T)obj;
	}

	public static bool IsNumeric(this Type type) {
		switch (Type.GetTypeCode(type)) {
			case TypeCode.Byte:
			case TypeCode.SByte:
			case TypeCode.UInt16:
			case TypeCode.UInt32:
			case TypeCode.UInt64:
			case TypeCode.Int16:
			case TypeCode.Int32:
			case TypeCode.Int64:
			case TypeCode.Decimal:
			case TypeCode.Double:
			case TypeCode.Single:
				return true;
			case TypeCode.Object:
				if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)) {
					return Nullable.GetUnderlyingType(type)?.IsNumeric() ?? false;
				}
				return false;
			default:
				return false;
		}
	}

	public static bool IsDateAndOrTime(this Type type) {

		var isDT =
			typeof(DateTime) == type ||
			typeof(DateOnly) == type ||
			typeof(TimeOnly) == type ||
			typeof(DateTimeOffset) == type ||
			typeof(DateTime).IsAssignableFrom(type) ||
			typeof(DateOnly).IsAssignableFrom(type) ||
			typeof(TimeOnly).IsAssignableFrom(type) ||
			typeof(DateTimeOffset).IsAssignableFrom(type);

		if (isDT) {
			return true;
		}

		if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)) {
			return type.IsNullableDateAndOrTime();
		}

		return false;

	}
	public static bool IsNullableDateAndOrTime(this Type nullableType) {
		var underlyingType = Nullable.GetUnderlyingType(nullableType);
		if (underlyingType == null) {
			return false;
		}

		return underlyingType.IsDateAndOrTime();
	}

	public static object? GetMemberValue<T>(this T obj, string memberName) {
		var type = typeof(T);
		if (type == null) {
			return null;
		}
		var fieldInfo = type.GetField(memberName);
		if (fieldInfo != null) {
			return fieldInfo.GetValue(obj);
		}
		var propInfo = type.GetProperty(memberName);
		if (propInfo != null) {
			return propInfo.GetValue(obj);
		}
		return null;
	}

	public static string GetDescription<T>(this T e) where T : IConvertible {
		if (e is Enum) {

			var type = e.GetType();
			var values = Enum.GetValues(type);

			foreach (int val in values) {
				if (val == e.ToInt32(CultureInfo.InvariantCulture)) {
					var enumName = type.GetEnumName(val);
					if (string.IsNullOrWhiteSpace(enumName) is false) {
						var memInfo = type.GetMember(enumName);
						if (memInfo[0]
							.GetCustomAttributes(typeof(DescriptionAttribute), false)
							.FirstOrDefault() is DescriptionAttribute descriptionAttribute) {
							return descriptionAttribute.Description;
						}
					}
				}
			}

		}

		return e?.ToString() ?? "";

	}

	public static Type GetNonNullableType(this Type type) {
		return Nullable.GetUnderlyingType(type) ?? type;
	}

	public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1,
														Expression<Func<T, bool>> expr2) {
		var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
		return Expression.Lambda<Func<T, bool>>
			  (Expression.OrElse(expr1.Body, invokedExpr), expr1.Parameters);
	}

	public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1,
														 Expression<Func<T, bool>> expr2) {
		var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
		return Expression.Lambda<Func<T, bool>>
			  (Expression.AndAlso(expr1.Body, invokedExpr), expr1.Parameters);
	}

	public static Expression<Func<TDataType, bool>>? CreateObjectSearch<TDataType>(
		this IEnumerable<Expression<Func<TDataType, object>>?> memberExpressions,
		string value) {

		if (memberExpressions == null || memberExpressions.Any() is false) {
			return null;
		}

		Expression<Func<TDataType, bool>>? expression = null;

		foreach (var keyword in value.Trim().Split(" ")) {

			Expression<Func<TDataType, bool>>? tmp = null;

			foreach (var memberExpression in memberExpressions) {
				if (memberExpression != null) {
					var callBody = typeof(string).GetMethod(nameof(string.IndexOf), [typeof(string), typeof(StringComparison)]);
					if (callBody != null) {
						var newQuery = Expression.Lambda<Func<TDataType, bool>>(
							Expression.AndAlso(
								memberExpression.Body.CreateNullChecks(),
								Expression.GreaterThanOrEqual(
									Expression.Call(
										Expression.Call(memberExpression.Body, "ToString", Type.EmptyTypes),
										callBody,
										[
											Expression.Constant(keyword),
											Expression.Constant(StringComparison.OrdinalIgnoreCase)
										]),
								Expression.Constant(0))),
								memberExpression.Parameters[0]);
						tmp = tmp == null ? newQuery : tmp.Or(newQuery);
					}
				}
			}

			expression = expression == null ? tmp : tmp != null ? expression.And(tmp) : null;

		}

		return expression;

	}

	private static BinaryExpression CreateNullChecks(this Expression expression, bool skipFinalMember = false) {

		var parents = new Stack<BinaryExpression>();

		BinaryExpression? newExpression = null;

		if (expression is UnaryExpression unary) {
			expression = unary.Operand;
		}

		var temp = expression as MemberExpression;

		while (temp is MemberExpression member) {

			try {
				var nullCheck = Expression.NotEqual(temp, Expression.Constant(null));
				parents.Push(nullCheck);
			} catch (InvalidOperationException) {
			}

			temp = member.Expression as MemberExpression;

		}

		while (parents.Count > 0) {
			if (skipFinalMember && parents.Count == 1 && newExpression != null) {
				break;
			} else {
				newExpression = newExpression == null ?
					parents.Pop() :
					Expression.AndAlso(newExpression, parents.Pop());
			}
		}

		if (newExpression == null) {
			return Expression.Equal(Expression.Constant(true), Expression.Constant(true));
		}

		return newExpression;

	}

}