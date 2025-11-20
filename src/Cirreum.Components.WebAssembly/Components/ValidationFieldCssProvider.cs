namespace Cirreum.Components;

using Microsoft.AspNetCore.Components.Forms;

/// <summary>
/// Supplies CSS class names for form fields to represent their validation state or other
/// state information from an <see cref="EditContext"/>.
/// </summary>
/// <param name="validClass"></param>
/// <param name="invalidClass"></param>
/// <param name="modified"></param>
public class ValidationFieldCssProvider(
	string validClass = "",
	string invalidClass = "is-invalid",
	string modified = "modified")
	: FieldCssClassProvider {

	/// <summary>
	/// Gets a string that indicates the status of the specified field as a CSS class.
	/// </summary>
	/// <param name="editContext">The <see cref="EditContext"/>.</param>
	/// <param name="fieldIdentifier">The <see cref="FieldIdentifier"/>.</param>
	/// <returns>A CSS class name string.</returns>
	public override string GetFieldCssClass(EditContext editContext, in FieldIdentifier fieldIdentifier) {

		var isValid = editContext.IsValid(fieldIdentifier);
		var isModified = editContext.IsModified(fieldIdentifier);

		// Blazor vs. Bootstrap:
		// isvalid = is-valid
		// isinvalid = is-invalid
		return CssBuilder.Empty()
				.AddClass(modified, when: isModified)
				.AddClass(validClass, when: isValid)
				.AddClass(invalidClass, when: !isValid)
			.Build();

	}

}