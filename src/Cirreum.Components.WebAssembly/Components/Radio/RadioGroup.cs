namespace Cirreum.Components;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Groups child <see cref="InputRadio{TValue}"/> components.
/// </summary>
public class RadioGroup<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TValue> : InputBase<TValue>, IRadioValueProvider {

	private readonly string _defaultGroupName = Guid.NewGuid().ToString("N");

	private RadioContext? _context;

	/// <summary>
	/// Gets or sets the child content to be rendering inside the <see cref="RadioGroup{TValue}"/>.
	/// </summary>
	[Parameter] public RenderFragment? ChildContent { get; set; }

	/// <summary>
	/// Gets or sets the name of the group.
	/// </summary>
	[Parameter] public string? Name { get; set; }

	[CascadingParameter] private RadioContext? CascadedContext { get; set; }

	object? IRadioValueProvider.CurrentValue => this.CurrentValue;

	/// <inheritdoc />
	protected override void OnParametersSet() {

		// On the first render, we can instantiate the RadioContext
		if (this._context is null) {
			var changeEventCallback = EventCallback.Factory.CreateBinder<string?>(this, __value => this.CurrentValueAsString = __value, this.CurrentValueAsString);
			this._context = new RadioContext(this, this.CascadedContext, changeEventCallback);
		} else if (this._context.ParentContext != this.CascadedContext) {
			// This should never be possible in any known usage pattern, but if it happens, we want to know
			throw new InvalidOperationException("An RadioGroup cannot change context after creation");
		}

		// Mutate the RadioContext instance in place. Since this is a non-fixed cascading parameter, the descendant
		// Radio/RadioGroup components will get notified to re-render and will see the new values.
		if (!string.IsNullOrEmpty(this.Name)) {
			// Prefer the explicitly-specified group name over anything else.
			this._context.GroupName = this.Name;
		} else if (!string.IsNullOrEmpty(this.NameAttributeValue)) {
			// If the user specifies a "name" attribute, or we're using "name" as a form field identifier, use that.
			this._context.GroupName = this.NameAttributeValue;
		} else {
			// Otherwise, just use a GUID to disambiguate this group's radio inputs from any others on the page.
			this._context.GroupName = this._defaultGroupName;
		}

		this._context.FieldClass = this.EditContext?.FieldCssClass(this.FieldIdentifier);

	}

	/// <inheritdoc />
	protected override void BuildRenderTree(RenderTreeBuilder builder) {

		// Note that we must not set IsFixed=true on the CascadingValue, because the mutations to _context
		// are what cause the descendant InputRadio components to re-render themselves
		builder.OpenComponent<CascadingValue<RadioContext>>(0);
		builder.AddComponentParameter(2, "Value", this._context);
		builder.AddComponentParameter(3, "ChildContent", this.ChildContent);
		builder.CloseComponent();
	}

	/// <inheritdoc />
	protected override bool TryParseValueFromString(string? value, [MaybeNullWhen(false)] out TValue result, [NotNullWhen(false)] out string? validationErrorMessage)
		=> this.TryParseSelectableValueFromString(value, out result, out validationErrorMessage);

}