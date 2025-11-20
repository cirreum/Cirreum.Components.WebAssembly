namespace Cirreum.Components;

using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// Provides convenient methods for assembling conditional CSS style string values.
/// </summary>
/// <remarks>
/// This class is not thread-safe. Instances should not be shared between threads.
/// </remarks>
/// <example>
/// var styles = StyleBuilder.Default("color", "red")
///     .AddStyle("background-color", "blue", isBlueBackground)
///     .Build();
/// </example>
public sealed class StyleBuilder {

	private readonly StringBuilder stringBuilder;

	/// <summary>
	/// Initializes a new instance of the <see cref="StyleBuilder"/> class with a property and value.
	/// </summary>
	/// <param name="prop">The CSS property name.</param>
	/// <param name="value">The value to assign to the property.</param>
	public StyleBuilder(string prop, string value) {
		this.stringBuilder = new StringBuilder();
		if (!string.IsNullOrWhiteSpace(prop) && !string.IsNullOrWhiteSpace(value)) {
			this.stringBuilder.Append($"{prop}:{value};");
		}
	}

	/// <summary>
	/// Initializes a new empty instance of the <see cref="StyleBuilder"/> class.
	/// </summary>
	public StyleBuilder() {
		this.stringBuilder = new StringBuilder();
	}

	/// <summary>
	/// Gets a value indicating whether the builder contains a non-whitespace value.
	/// </summary>
	public bool HasValue => !string.IsNullOrWhiteSpace(this.stringBuilder.ToString());

	/// <summary>
	/// Creates a StyleBuilder with an initial property and value.
	/// </summary>
	/// <param name="prop">The CSS property name.</param>
	/// <param name="value">The value to assign to the property.</param>
	/// <returns>A new <see cref="StyleBuilder"/> instance.</returns>
	public static StyleBuilder Default(string prop, string value) => new StyleBuilder(prop, value);

	/// <summary>
	/// Creates a StyleBuilder with an initial style string.
	/// </summary>
	/// <param name="style">The style string to initialize with (e.g. "color:red;").</param>
	/// <returns>A new <see cref="StyleBuilder"/> instance.</returns>
	public static StyleBuilder Default(string style) => Empty().AddStyleIfNotEmpty(style);

	/// <summary>
	/// Creates an empty StyleBuilder.
	/// </summary>
	/// <returns>A new empty <see cref="StyleBuilder"/> instance.</returns>
	public static StyleBuilder Empty() => new StyleBuilder();

	/// <summary>
	/// Adds a style string to the builder if it's not empty or whitespace.
	/// </summary>
	/// <param name="style">The style string to add (e.g. "color:red").</param>
	/// <returns>The current instance for method chaining.</returns>
	public StyleBuilder AddStyleIfNotEmpty(string? style) {
		if (!string.IsNullOrWhiteSpace(style)) {
			// Ensure style ends with semicolon
			if (!style.TrimEnd().EndsWith(';')) {
				style = $"{style};";
			}
			return this.AddRaw(style);
		}
		return this;
	}

	/// <summary>
	/// Adds a raw string to the builder's internal buffer.
	/// </summary>
	/// <param name="style">The raw string to append to the buffer.</param>
	/// <returns>The current instance for method chaining.</returns>
	private StyleBuilder AddRaw(string style) {
		this.stringBuilder.Append(style);
		return this;
	}

	/// <summary>
	/// Adds a CSS property with value to the builder.
	/// </summary>
	/// <param name="prop">The CSS property name.</param>
	/// <param name="value">The value to assign to the property.</param>
	/// <returns>The current instance for method chaining.</returns>
	public StyleBuilder AddStyle(string prop, string value) {
		if (!string.IsNullOrWhiteSpace(prop) && value != null) {
			return this.AddRaw($"{prop}:{value};");
		}
		return this;
	}

	/// <summary>
	/// Conditionally adds a CSS property with value to the builder.
	/// </summary>
	/// <param name="prop">The CSS property name.</param>
	/// <param name="value">The value to assign to the property.</param>
	/// <param name="when">Condition determining if the style should be added.</param>
	/// <returns>The current instance for method chaining.</returns>
	public StyleBuilder AddStyle(string prop, string value, bool when = true) =>
		when ? this.AddStyle(prop, value) : this;

	/// <summary>
	/// Conditionally adds a CSS property with a value from a function to the builder.
	/// </summary>
	/// <param name="prop">The CSS property name.</param>
	/// <param name="value">Function that returns the value to assign to the property.</param>
	/// <param name="when">Condition determining if the style should be added.</param>
	/// <returns>The current instance for method chaining.</returns>
	public StyleBuilder AddStyle(string prop, Func<string> value, bool when = true) =>
		when && value != null ? this.AddStyle(prop, value.Invoke() ?? "") : this;

	/// <summary>
	/// Conditionally adds a CSS property with value to the builder using a function for the condition.
	/// </summary>
	/// <param name="prop">The CSS property name.</param>
	/// <param name="value">The value to assign to the property.</param>
	/// <param name="when">Function that determines if the style should be added.</param>
	/// <returns>The current instance for method chaining.</returns>
	public StyleBuilder AddStyle(string prop, string value, Func<bool> when) =>
		when != null && when.Invoke() ? this.AddStyle(prop, value) : this;

	/// <summary>
	/// Conditionally adds a CSS property with a value from a function to the builder using a function for the condition.
	/// </summary>
	/// <param name="prop">The CSS property name.</param>
	/// <param name="value">Function that returns the value to assign to the property.</param>
	/// <param name="when">Function that determines if the style should be added.</param>
	/// <returns>The current instance for method chaining.</returns>
	public StyleBuilder AddStyle(string prop, Func<string> value, Func<bool> when) =>
		when != null && when.Invoke() && value != null ? this.AddStyle(prop, value.Invoke() ?? "") : this;

	/// <summary>
	/// Adds all styles from another StyleBuilder to this builder.
	/// </summary>
	/// <param name="builder">The StyleBuilder whose styles should be added.</param>
	/// <returns>The current instance for method chaining.</returns>
	public StyleBuilder AddStyle(StyleBuilder builder) {
		if (builder != null && builder.HasValue) {
			return this.AddRaw(builder.Build());
		}
		return this;
	}

	/// <summary>
	/// Conditionally adds all styles from another StyleBuilder to this builder.
	/// </summary>
	/// <param name="builder">The StyleBuilder whose styles should be added.</param>
	/// <param name="when">Condition determining if the styles should be added.</param>
	/// <returns>The current instance for method chaining.</returns>
	public StyleBuilder AddStyle(StyleBuilder builder, bool when = true) =>
		when && builder != null ? this.AddStyle(builder) : this;

	/// <summary>
	/// Conditionally adds all styles from another StyleBuilder to this builder using a function for the condition.
	/// </summary>
	/// <param name="builder">The StyleBuilder whose styles should be added.</param>
	/// <param name="when">Function that determines if the styles should be added.</param>
	/// <returns>The current instance for method chaining.</returns>
	public StyleBuilder AddStyle(StyleBuilder builder, Func<bool> when) =>
		when != null && when.Invoke() ? this.AddStyle(builder) : this;

	/// <summary>
	/// Adds a CSS property with values created by a ValueBuilder action.
	/// </summary>
	/// <param name="prop">The CSS property name.</param>
	/// <param name="builder">Action that configures a ValueBuilder to create the property value.</param>
	/// <param name="when">Condition determining if the style should be added.</param>
	/// <returns>The current instance for method chaining.</returns>
	public StyleBuilder AddStyle(string prop, Action<ValueBuilder> builder, bool when = true) {
		if (when && builder != null) {
			var values = ValueBuilder.Empty();
			builder(values);
			return this.AddStyle(prop, values.ToString(), values.HasValue);
		}
		return this;
	}

	/// <summary>
	/// Adds styles from an attributes dictionary, extracting the "style" attribute if present.
	/// </summary>
	/// <param name="additionalAttributes">Dictionary of attributes that may contain a "style" key.</param>
	/// <returns>The current instance for method chaining.</returns>
	public StyleBuilder AddStyleFromAttributes(IReadOnlyDictionary<string, object> additionalAttributes) {
		if (additionalAttributes != null &&
			additionalAttributes.TryGetValue("style", out var style) &&
			style != null) {
			return this.AddStyleIfNotEmpty(style.ToString());
		}
		return this;
	}

	/// <summary>
	/// Adds a CSS property with value to the builder only if the value is not null or empty.
	/// </summary>
	/// <param name="prop">The CSS property name.</param>
	/// <param name="value">The value to assign to the property. Can be null.</param>
	/// <param name="when">Additional condition for adding the style.</param>
	/// <returns>The current instance for method chaining.</returns>
	public StyleBuilder AddStyleIfNotEmpty(string prop, string? value, bool when = true) {
		if (when && !string.IsNullOrWhiteSpace(value)) {
			return this.AddStyle(prop, value);
		}
		return this;
	}

	/// <summary>
	/// Checks if the builder contains a specific CSS property.
	/// </summary>
	/// <param name="prop">The CSS property name to check for.</param>
	/// <returns>True if the property exists in the builder, false otherwise.</returns>
	public bool HasStyle(string prop) {
		if (string.IsNullOrWhiteSpace(prop)) {
			return false;
		}

		return this.stringBuilder.ToString().Contains($"{prop}:", StringComparison.OrdinalIgnoreCase);
	}

	/// <summary>
	/// Clears all styles from the builder.
	/// </summary>
	/// <returns>The current instance for method chaining.</returns>
	public StyleBuilder Clear() {
		this.stringBuilder.Clear();
		return this;
	}

	/// <summary>
	/// Finalizes and returns the completed style string.
	/// </summary>
	/// <returns>The built CSS style string.</returns>
	public string Build() {
		return this.stringBuilder.ToString().Trim();
	}

	/// <summary>
	/// Returns the built style string.
	/// </summary>
	/// <returns>The CSS style string.</returns>
	public override string ToString() => this.Build();

}