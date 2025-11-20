namespace Cirreum.Extensions;

using Cirreum.Components;

/// <summary>
/// Extension methods for builder classes to handle null conversion when empty.
/// </summary>
public static class BuilderExtensions {

	/// <summary>
	/// Converts a CssBuilder into null when it is empty.
	/// This allows the class attribute to be excluded when rendered if no CSS classes are present.
	/// </summary>
	/// <param name="builder">The CssBuilder instance to check and convert.</param>
	/// <returns>The built CSS string if the builder has a value, otherwise null.</returns>
	public static string? NullIfEmpty(this CssBuilder builder) =>
		builder.HasValue ? builder.Build() : null;

	/// <summary>
	/// Converts a StyleBuilder into null when it is empty.
	/// This allows the style attribute to be excluded when rendered if no styles are present.
	/// </summary>
	/// <param name="builder">The StyleBuilder instance to check and convert.</param>
	/// <returns>The built style string if the builder has a value, otherwise null.</returns>
	public static string? NullIfEmpty(this StyleBuilder builder) =>
		builder.HasValue ? builder.Build() : null;

	/// <summary>
	/// Converts a ValueBuilder into null when it is empty.
	/// This allows the attribute to be excluded when rendered if no value is present.
	/// </summary>
	/// <param name="builder">The ValueBuilder instance to check and convert.</param>
	/// <returns>The built value string if the builder has a value, otherwise null.</returns>
	public static string? NullIfEmpty(this ValueBuilder builder) =>
		builder.HasValue ? builder.Build() : null;

}