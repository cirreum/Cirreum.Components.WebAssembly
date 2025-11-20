namespace Cirreum.Components;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

/// <summary>
/// Provides convenient methods for assembling conditional CSS class strings.
/// </summary>
/// <remarks>
/// This class is not thread-safe. Instances should not be shared between threads.
/// </remarks>
/// <remarks>
/// Initializes a new instance of the <see cref="CssBuilder"/> class with an initial value.
/// </remarks>
/// <param name="value">The initial CSS class or classes.</param>
public sealed class CssBuilder(string? value) {

	private readonly StringBuilder stringBuilder = new StringBuilder(value ?? "");

	/// <summary>
	/// Gets a value indicating whether the builder contains any content.
	/// </summary>
	public bool HasValue => this.stringBuilder.Length > 0;

	/// <summary>
	/// Creates a CssBuilder with an initial value.
	/// </summary>
	/// <param name="value">The initial CSS class or classes.</param>
	/// <returns>A new <see cref="CssBuilder"/> instance.</returns>
	/// <example>
	/// var builder = CssBuilder.Default("btn");
	/// </example>
	public static CssBuilder Default(string value) => new CssBuilder(value);

	/// <summary>
	/// Creates an empty CssBuilder.
	/// </summary>
	/// <returns>A new empty <see cref="CssBuilder"/> instance.</returns>
	/// <example>
	/// var builder = CssBuilder.Empty();
	/// </example>
	public static CssBuilder Empty() => new CssBuilder("");

	/// <summary>
	/// Adds a raw string to the builder without adding any spaces.
	/// </summary>
	/// <param name="value">The raw string to append to the builder.</param>
	/// <returns>The current instance for method chaining.</returns>
	public CssBuilder AddRawValue(string value) {
		this.stringBuilder.Append(value);
		return this;
	}

	/// <summary>
	/// Adds a CSS class to the builder with a space separator.
	/// </summary>
	/// <param name="value">CSS class to add.</param>
	/// <returns>The current instance for method chaining.</returns>
	/// <example>
	/// builder.AddClass("btn-primary");
	/// </example>
	public CssBuilder AddClass(string value) {
		this.stringBuilder.Append(' ').Append(value);
		return this;
	}

	/// <summary>
	/// Adds a CSS class to the builder with a space separator, if the value is not
	/// null, empty, or whitespace.
	/// </summary>
	/// <param name="value">CSS class to add.</param>
	/// <returns>The current instance for method chaining.</returns>
	public CssBuilder AddClassIfNotEmpty(string? value) {
		if (!string.IsNullOrWhiteSpace(value)) {
			this.AddClass(value);
		}
		return this;
	}

	/// <summary>
	/// Adds a CSS class to the builder with a space separator, if the function returns a value that is not
	/// null, empty, or whitespace.
	/// </summary>
	/// <param name="value">Function that returns the optional CSS class.</param>
	/// <returns>The current instance for method chaining.</returns>
	public CssBuilder AddClassIfNotEmpty(Func<string> value) {
		if (value != null) {
			var cssClass = value();
			if (!string.IsNullOrWhiteSpace(cssClass)) {
				this.AddClass(cssClass);
			}
		}
		return this;
	}

	/// <summary>
	/// Adds a CSS class only if the builder is currently empty (has no classes)
	/// and when the condition is true.
	/// </summary>
	/// <param name="value">The CSS class to add.</param>
	/// <param name="when">The condition to evaluate.</param>
	/// <returns>The current instance for method chaining.</returns>
	public CssBuilder AddClassIfBuilderIsEmpty(string value, bool when = true) {
		if (!this.HasValue && when && value != null) {
			this.AddClass(value);
		}
		return this;
	}

	/// <summary>
	/// Adds a CSS class only if the builder is currently empty (has no classes)
	/// and when the condition is true.
	/// </summary>
	/// <param name="value">Function that returns the CSS class to add.</param>
	/// <param name="when">The condition to evaluate.</param>
	/// <returns>The current instance for method chaining.</returns>
	public CssBuilder AddClassIfBuilderIsEmpty(Func<string> value, bool when = true) {
		if (!this.HasValue && when && value != null) {
			var cssClass = value();
			this.AddClass(cssClass);
		}
		return this;
	}

	/// <summary>
	/// Conditionally adds a CSS class to the builder with a space separator.
	/// </summary>
	/// <param name="value">CSS class to conditionally add.</param>
	/// <param name="when">Condition determining if the CSS class should be added.</param>
	/// <returns>The current instance for method chaining.</returns>
	/// <example>
	/// builder.AddClass("btn-danger", isError);
	/// </example>
	public CssBuilder AddClass(string value, bool when = true) {
		if (when && value != null) {
			this.AddClass(value);
		}
		return this;
	}

	/// <summary>
	/// Conditionally adds a CSS class to the builder with a space separator.
	/// </summary>
	/// <param name="value">CSS class to conditionally add.</param>
	/// <param name="when">Function that determines if the CSS class should be added.</param>
	/// <returns>The current instance for method chaining.</returns>
	public CssBuilder AddClass(string value, Func<bool>? when = null) {
		return this.AddClass(value, when != null && when());
	}

	/// <summary>
	/// Conditionally adds a CSS class from a function to the builder with a space separator.
	/// </summary>
	/// <param name="value">Function that returns a CSS class to conditionally add.</param>
	/// <param name="when">Condition determining if the CSS class should be added.</param>
	/// <returns>The current instance for method chaining.</returns>
	public CssBuilder AddClass(Func<string> value, bool when = true) {
		if (when && value != null) {
			this.AddClass(value());
		}
		return this;
	}

	/// <summary>
	/// Conditionally adds a CSS class from a function to the builder with a space separator.
	/// </summary>
	/// <param name="value">Function that returns a CSS class to conditionally add.</param>
	/// <param name="when">Function that determines if the CSS class should be added.</param>
	/// <returns>The current instance for method chaining.</returns>
	public CssBuilder AddClass(Func<string> value, Func<bool>? when = null) {
		return this.AddClass(value, when != null && when());
	}

	/// <summary>
	/// Adds all classes from another CssBuilder to the builder with a space separator.
	/// </summary>
	/// <param name="builder">CssBuilder instance whose classes should be added.</param>
	/// <returns>The current instance for method chaining.</returns>
	public CssBuilder AddBuilder(CssBuilder builder) {
		if (builder != null) {
			this.AddClassIfNotEmpty(builder.Build());
		}
		return this;
	}

	/// <summary>
	/// Conditionally adds all classes from another CssBuilder to the builder with a space separator.
	/// </summary>
	/// <param name="builder">CssBuilder instance whose classes should be added.</param>
	/// <param name="when">Condition determining if the classes should be added.</param>
	/// <returns>The current instance for method chaining.</returns>
	public CssBuilder AddBuilder(CssBuilder builder, bool when) {
		if (when && builder != null) {
			this.AddClassIfNotEmpty(builder.Build());
		}
		return this;
	}

	/// <summary>
	/// Conditionally adds all classes from another CssBuilder to the builder with a space separator.
	/// </summary>
	/// <param name="builder">CssBuilder instance whose classes should be added.</param>
	/// <param name="when">Function that determines if the classes should be added.</param>
	/// <returns>The current instance for method chaining.</returns>
	public CssBuilder AddBuilder(CssBuilder builder, Func<bool>? when = null) {
		return this.AddBuilder(builder, when != null && when());
	}

	/// <summary>
	/// Adds CSS classes from an attributes dictionary, extracting the "class" attribute if present.
	/// </summary>
	/// <param name="additionalAttributes">Dictionary of attributes that may contain a "class" key.</param>
	/// <returns>The current instance for method chaining.</returns>
	public CssBuilder AddClassFromAttributes(IReadOnlyDictionary<string, object>? additionalAttributes) {
		if (additionalAttributes != null &&
			additionalAttributes.TryGetValue("class", out var classObj) &&
			classObj != null) {
			var classValue = Convert.ToString(classObj, CultureInfo.InvariantCulture);
			this.AddClassIfNotEmpty(classValue);
		}
		return this;
	}

	/// <summary>
	/// Clears all CSS classes from the builder.
	/// </summary>
	/// <returns>The current instance for method chaining.</returns>
	public CssBuilder Clear() {
		this.stringBuilder.Clear();
		return this;
	}

	/// <summary>
	/// Finalizes and returns the completed CSS classes as a trimmed string.
	/// </summary>
	/// <returns>The built string of CSS classes with extra spaces removed.</returns>
	public string Build() {
		if (this.HasValue) {
			return this.stringBuilder.ToString().Trim();
		}
		return string.Empty;
	}

	/// <summary>
	/// Returns the built CSS class string.
	/// </summary>
	/// <returns>The CSS class string.</returns>
	public override string ToString() => this.Build();

	/// <summary>
	/// Combines two CSS class lists.
	/// </summary>
	/// <param name="additionalAttributes">A dictionary potentially containing a 'class' value.</param>
	/// <param name="classNames">The string containing CSS classes to merge with the attributes.</param>
	/// <returns>The merged class list as a single string, or null if both inputs are empty.</returns>
	/// <example>
	/// var combinedClasses = CssBuilder.CombineClassNames(attributes, "btn btn-primary");
	/// </example>
	public static string? CombineClassNames(IReadOnlyDictionary<string, object>? additionalAttributes, string? classNames) {
		if (additionalAttributes is null || !additionalAttributes.TryGetValue("class", out var classObj)) {
			return classNames;
		}

		var classAttributeValue = Convert.ToString(classObj, CultureInfo.InvariantCulture);

		if (string.IsNullOrEmpty(classAttributeValue)) {
			return classNames;
		}

		if (string.IsNullOrEmpty(classNames)) {
			return classAttributeValue;
		}

		return $"{classAttributeValue} {classNames}";
	}

}