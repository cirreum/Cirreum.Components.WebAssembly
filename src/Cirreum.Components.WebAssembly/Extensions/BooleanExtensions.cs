namespace Cirreum.Extensions;

internal static class BooleanExtensions {

	/// <summary>
	/// Returns the <paramref name="value"/> if it is not null, otherwise <see langword="false"/>.
	/// </summary>
	/// <param name="value">The nullable boolean to evaluate.</param>
	/// Returns the <paramref name="value"/> if it is not null, otherwise <see langword="false"/>.
	public static bool ValueOrFalse(this bool? value) {
		return value ?? false;
	}

	public static string ToAttributeValue(this bool value) => value ? "true" : "false";

}