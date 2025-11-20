namespace Cirreum.Components;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using System.Diagnostics.CodeAnalysis;

public class Switch : InputBase<bool> {

	/// <summary>
	/// The Input's Id
	/// </summary>
	/// <remarks>
	/// A default value is assigned.
	/// </remarks>
	[Parameter]
	public string Id { get; set; } = IdGenerator.Next;

	/// <summary>
	/// Gets or sets the name of the form field.
	/// </summary>
	[Parameter]
	public string? Name { get; set; }

	/// <summary>
	/// Gets or sets if the Switch is required.
	/// </summary>
	[Parameter]
	public bool Required { get; set; }

	/// <summary>
	/// Gets or sets if the Switch is disabled.
	/// </summary>
	[Parameter]
	public bool Disabled { get; set; }

	/// <summary>
	/// Gets or sets if the radio's should be inline, instead of stacked.
	/// </summary>
	[Parameter]
	public bool Inline { get; set; }

	/// <summary>
	/// Gets or sets the value to use as the aria-label.
	/// </summary>
	[Parameter]
	public string? AriaLabel { get; set; }

	/// <summary>
	/// Gets or sets the text to use for Label.
	/// </summary>
	[Parameter]
	public string? Label { get; set; }

	/// <summary>
	/// Gets or sets the Labels Css class list.
	/// </summary>
	[Parameter]
	public string? LabelClass { get; set; }

	/// <summary>
	/// Gets or sets the Labels style attributes.
	/// </summary>
	[Parameter]
	public string? LabelStyle { get; set; }

	/// <summary>
	/// Gets or sets the custom text to use for Label.
	/// </summary>
	[Parameter]
	public RenderFragment? LabelContent { get; set; }

	/// <summary>
	/// Gets or sets the <see cref="RenderFragment"/> to provide your own custom Label.
	/// </summary>
	[Parameter]
	public RenderFragment? ChildContent { get; set; }

	/// <summary>
	/// Gets or sets the associated <see cref="ElementReference"/>.
	/// <para>
	/// May be <see langword="null"/> if accessed before the component is rendered.
	/// </para>
	/// </summary>
	[DisallowNull]
	public ElementReference? Element { get; protected set; }

	public override Task SetParametersAsync(ParameterView parameters) {

		parameters.SetParameterProperties(this);

		this.ValueExpression ??= () => this.CurrentValue;

		// For derived components, retain the usual lifecycle with OnInit/OnParametersSet/etc.
		return base.SetParametersAsync(ParameterView.Empty);

	}


	/// <inheritdoc />
	protected override void BuildRenderTree(RenderTreeBuilder builder) {

		builder.OpenElement(100, "div");
		var outerCss = CssBuilder.Default("form-check form-switch")
				.AddClass("form-check-inline", when: this.Inline)
			.Build();
		builder.AddAttribute(101, "class", outerCss);

		var name = this.Name.HasValue() ? this.Name : this.NameAttributeValue;
		var resolvedCss = CssBuilder.Default("form-check-input")
				.AddClassFromAttributes(this.AdditionalAttributes)
				.AddClassIfNotEmpty(this.CssClass)
			.Build();
		builder.OpenElement(200, "input");
		builder.AddMultipleAttributes(201, this.AdditionalAttributes);
		builder.AddAttribute(205, "id", this.Id);
		builder.AddAttribute(210, "type", "checkbox");
		builder.AddAttributeIfNotNullOrEmpty(215, "name", name);
		builder.AddAttribute(220, "class", resolvedCss);
		builder.AddAttribute(221, "role", "switch");
		builder.AddAttribute(222, "disabled", this.Disabled);
		builder.AddAttribute(223, "required", this.Required);
		builder.AddAttribute(225, "checked", BindConverter.FormatValue(this.CurrentValue));
		builder.AddAttribute(226, "aria-checked", this.CurrentValue.ToAttributeValue());
		builder.AddAttributeIfNotNullOrEmpty(227, "aria-disabled", this.Disabled ? "true" : null);
		builder.AddAttributeIfNotNullOrEmpty(228, "aria-label", this.AriaLabel);

		// Include the "value" attribute so that when this is posted by a form, "true"
		// is included in the form fields. That's how <input type="checkbox"> works normally.
		// It sends the "on" value when the checkbox is checked, and nothing otherwise.
		builder.AddAttribute(230, "value", bool.TrueString);

		builder.AddAttribute(235, "onchange", EventCallback.Factory.CreateBinder<bool>(this, __value => this.CurrentValue = __value, this.CurrentValue));
		builder.SetUpdatesAttributeName("checked");
		builder.AddElementReferenceCapture(240, __inputReference => this.Element = __inputReference);
		builder.CloseElement(); // close: 200

		if (this.ChildContent is not null) {
			builder.AddContent(300, this.ChildContent);
		} else if (this.Label.HasValue() || this.LabelContent is not null) {
			var lableCss = CssBuilder.Empty()
					.AddClassIfNotEmpty(this.LabelClass)
				.Build();
			builder.OpenElement(301, "label");
			builder.AddAttributeIfNotNullOrEmpty(302, "class", lableCss);
			builder.AddAttributeIfNotNullOrEmpty(303, "style", this.LabelStyle);
			builder.AddAttribute(305, "for", this.Id);
			builder.AddContent(310, this.Label);
			builder.AddContent(315, this.LabelContent);
			builder.CloseElement(); // close: 300
		}

		builder.CloseElement(); // close: 100

	}

	/// <inheritdoc />
	protected override bool TryParseValueFromString(string? value, out bool result, [NotNullWhen(false)] out string? validationErrorMessage)
		=> throw new NotSupportedException($"This component does not parse string inputs. Bind to the '{nameof(this.CurrentValue)}' property, not '{nameof(this.CurrentValueAsString)}'.");

}