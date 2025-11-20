namespace Cirreum.Extensions;

using System.Diagnostics.CodeAnalysis;

internal static class StringExtensions {

	/// <summary>
	/// Determines if the specified <paramref name="value"/> is null, empty or white-space.
	/// </summary>
	/// <param name="value">The potentially null value to evaluate.</param>
	/// <returns><see langword="true"/> if the specified string <paramref name="value"/> is null, empty or white-space; otherwise <see langword="false"/>.</returns>
	public static bool IsEmpty(this string? value) => string.IsNullOrWhiteSpace(value);

	/// <summary>
	/// Evaluate the specified <paramref name="value"/> for <see langword="null" />
	/// or an empty string ("").
	/// </summary>
	/// <param name="value">The string value to evaluate.</param>
	/// <returns>
	/// <see langword="null" /> if the <paramref name="value"/> is <see langword="null" />
	/// or an empty string (""), otherwise <paramref name="value"/>.
	/// </returns>
	public static string? NullIfEmpty(this string? value) {

		if (string.IsNullOrEmpty(value)) {
			return null;
		}

		return value;

	}

	/// <summary>
	/// Evaluate the specified <paramref name="value"/> for <see langword="null" />
	/// or <see cref="string.Empty"/>, or if <paramref name="value"/> consists
	/// exclusively of white-space characters and if so returns <see langword="null" />.
	/// </summary>
	/// <param name="value">The string value to evaluate.</param>
	/// <returns>
	/// <see langword="null" /> if the <paramref name="value"/> is <see langword="null" />
	/// or <see cref="string.Empty"/>, or if <paramref name="value"/> consists
	/// exclusively of white-space characters, otherwise <paramref name="value"/>.
	/// </returns>
	public static string? NullIfWhiteSpace(this string? value) {

		if (string.IsNullOrWhiteSpace(value)) {
			return null;
		}

		return value;

	}

	/// <summary>
	/// Evaluate the specified <paramref name="value"/> for <see langword="null" />
	/// or <see cref="string.Empty"/>, or if <paramref name="value"/> consists
	/// exclusively of white-space characters. And if so, then evaluates the
	/// specified <c>string?[]</c> in order and returns the first <see langword="value"/>
	/// that is not <see langword="null" /> or <see cref="string.Empty"/>, or consists
	/// exclusively of white-space characters. 
	/// </summary>
	/// <param name="value"></param>
	/// <param name="strings"></param>
	/// <returns>
	/// The first <see cref="string"/> that is not <see langword="null" /> or <see cref="string.Empty"/>,
	/// or consists exclusively of white-space characters, otherwise <see langword="null"/>.
	/// </returns>
	/// <example>
	/// <code>
	/// // returns "third_value" because the third value is the first value that isn't null.
	/// var sourceStr = string.Empty;
	/// var r = sourceStr.Coalesce(<see langword="null"/>, <see langword="null"/>, 'third_value', 'fourth_value');
	/// //string.IsNullOrWhiteSpace(r) == false
	/// //(r == 'third_value') == true
	/// </code>
	/// <code>
	/// // returns null.
	/// var sourceStr = string.Empty;
	/// var r = sourceStr.Coalesce();
	/// //string.IsNullOrWhiteSpace(r) == true
	/// </code>
	/// /// <code>
	/// // returns myString.
	/// var sourceStr = "myString";
	/// var r = sourceStr.Coalesce();
	/// //(r == 'myString') == true
	/// </code>
	/// </example>
	public static string? Coalesce(this string? value, params string?[] strings) {
		if (string.IsNullOrWhiteSpace(value)) {
			return strings.FirstOrDefault(s => !string.IsNullOrWhiteSpace(s));
		}
		return value;
	}

	/// <summary>
	/// Determine if the specified value is not null and does not contain only whitespace.
	/// </summary>
	/// <param name="value">The string object to evaluate.</param>
	/// <returns><see langword="true"/> if the specified string object has a value; otherwise <see langword="false"/>.</returns>
	public static bool HasValue([NotNullWhen(true)] this string? value) {
		return string.IsNullOrWhiteSpace(value) is false;
	}

}