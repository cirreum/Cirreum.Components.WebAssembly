namespace Cirreum.Components;

using Microsoft.AspNetCore.Components.Forms;

/// <summary>
/// Gets or creates a <see cref="FieldIdentifier"/> for use with <see cref="EditContext"/>
/// </summary>
/// <remarks>
/// <para>
/// This interface enables view models to integrate with Blazor's validation system by providing
/// a way to map property names to their corresponding <see cref="FieldIdentifier"/>.
/// </para>
/// <para>
/// Implementations should ensure consistent field identification for validation tracking and error display.
/// </para>
/// </remarks>
public interface IFieldIdentifierProvider {

	/// <summary>
	/// Gets the <see cref="FieldIdentifier"/> for a specified property.
	/// </summary>
	/// <param name="propertyName">The name of the property to get the identifier for.</param>
	/// <returns>A <see cref="FieldIdentifier"/> for the property if found; otherwise, null.</returns>
	FieldIdentifier? GetFieldIdentifier(string propertyName);
}