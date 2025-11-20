namespace Cirreum.Components;

using System;
using System.Text;

/// <summary>
/// Builds up a string value from conditionals.
/// </summary>
/// <remarks>
/// This class is not thread-safe. Instances should not be shared between threads.
/// </remarks>
/// <remarks>
/// Initializes a new instance of the <see cref="ValueBuilder"/> class.
/// </remarks>
/// <param name="value">The initial value.</param>
public sealed class ValueBuilder(string? value) {

	private readonly StringBuilder stringBuilder = new StringBuilder(value ?? "");

	/// <summary>
	/// Creates a ValueBuilder with an initial default value.
	/// </summary>
	/// <param name="value">The initial value.</param>
	/// <returns>A new <see cref="ValueBuilder"/> instance.</returns>
	/// <example>
	/// var builder = ValueBuilder.Default("initial");
	/// </example>
	public static ValueBuilder Default(string value) => new ValueBuilder(value);

	/// <summary>
	/// Creates an empty ValueBuilder.
	/// </summary>
	/// <returns>A new empty <see cref="ValueBuilder"/> instance.</returns>
	/// <example>
	/// var builder = ValueBuilder.Empty();
	/// </example>
	public static ValueBuilder Empty() => new ValueBuilder("");

	/// <summary>
	/// Gets a value indicating whether the builder contains a non-whitespace value.
	/// </summary>
	public bool HasValue => !string.IsNullOrWhiteSpace(this.stringBuilder.ToString());

	/// <summary>
	/// Adds a space-separated value to the builder if the condition is true.
	/// Each value will be separated by a space in the final result.
	/// </summary>
	/// <param name="value">The value to add.</param>
	/// <param name="when">Condition determining if the value should be added.</param>
	/// <returns>The current instance for method chaining.</returns>
	/// <example>
	/// builder.AddValue("bold", isTextBold);
	/// </example>
	public ValueBuilder AddValue(string value, bool when = true) =>
		when ? this.AddRaw($"{value} ") : this;

	/// <summary>
	/// Adds a space-separated value from a function to the builder if the condition is true.
	/// Each value will be separated by a space in the final result.
	/// </summary>
	/// <param name="value">Function that returns the value to add.</param>
	/// <param name="when">Condition determining if the value should be added.</param>
	/// <returns>The current instance for method chaining.</returns>
	public ValueBuilder AddValue(Func<string> value, bool when = true) =>
		when ? this.AddRaw($"{value?.Invoke() ?? ""} ") : this;

	/// <summary>
	/// Adds a value to the builder only if the value is not null or empty.
	/// </summary>
	/// <param name="value">The value to add. Can be null.</param>
	/// <returns>The current instance for method chaining.</returns>
	public ValueBuilder AddValueIfNotEmpty(string? value) =>
		!string.IsNullOrWhiteSpace(value) ? this.AddValue(value) : this;

	/// <summary>
	/// Adds a raw string to the builder's internal buffer without adding a space.
	/// </summary>
	/// <param name="value">The raw string to append to the buffer.</param>
	/// <returns>The current instance for method chaining.</returns>
	private ValueBuilder AddRaw(string value) {
		this.stringBuilder.Append(value);
		return this;
	}

	/// <summary>
	/// Clears all values from the builder.
	/// </summary>
	/// <returns>The current instance for method chaining.</returns>
	public ValueBuilder Clear() {
		this.stringBuilder.Clear();
		return this;
	}

	/// <summary>
	/// Finalizes and returns the completed value as a trimmed string.
	/// </summary>
	/// <returns>The built string with trailing spaces removed.</returns>
	public string Build() {
		return this.stringBuilder.ToString().Trim();
	}

	/// <summary>
	/// Returns the built value string with trailing spaces removed.
	/// </summary>
	/// <returns>The built string.</returns>
	public override string ToString() => this.Build();
}