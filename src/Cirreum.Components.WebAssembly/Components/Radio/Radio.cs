namespace Cirreum.Components;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

public class Radio<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TValue> : ComponentBase {

	bool _trueValueToggle;

	/// <summary>
	/// The Input's Id
	/// </summary>
	/// <remarks>
	/// A default value is assigned.
	/// </remarks>
	[Parameter]
	public string Id { get; set; } = IdGenerator.Next;

	/// <summary>
	/// Gets context for this <see cref="Radio{TValue}"/>.
	/// </summary>
	internal RadioContext? Context { get; private set; }

	[CascadingParameter]
	private RadioContext? CascadedContext { get; set; }


	/// <summary>
	/// Gets or sets a collection of additional attributes that will be applied to the input element.
	/// </summary>
	[Parameter(CaptureUnmatchedValues = true)]
	public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

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
	/// Gets or sets the value of this input.
	/// </summary>
	[Parameter]
	public TValue? Value { get; set; }

	/// <summary>
	/// Gets or sets the name of the parent radio group.
	/// </summary>
	[Parameter]
	public string? Name { get; set; }

	/// <summary>
	/// Gets or sets if the Radio is required.
	/// </summary>
	[Parameter]
	public bool Required { get; set; }

	/// <summary>
	/// Gets or sets if the Radio is disabled.
	/// </summary>
	[Parameter]
	public bool Disabled { get; set; }

	/// <summary>
	/// Gets or sets if the radio's should be inline, instead of stacked.
	/// </summary>
	[Parameter]
	public bool Inline { get; set; }

	/// <summary>
	/// Gets or sets the associated <see cref="ElementReference"/>.
	/// <para>
	/// May be <see langword="null"/> if accessed before the component is rendered.
	/// </para>
	/// </summary>
	[DisallowNull]
	public ElementReference? Element { get; protected set; }

	/// <inheritdoc />
	protected override void OnParametersSet() {

		this.Context = string.IsNullOrEmpty(this.Name)
			? this.CascadedContext
			: this.CascadedContext?.FindContextInAncestors(this.Name);

		if (this.Context == null) {
			throw new InvalidOperationException($"{this.GetType()} must have an ancestor {typeof(InputRadioGroup<TValue>)} " +
				$"with a matching 'Name' property, if specified.");
		}

	}

	/// <inheritdoc />
	protected override void BuildRenderTree(RenderTreeBuilder builder) {
		Debug.Assert(this.Context != null);

		/*
		 <div class="form-check">
		  <input class="form-check-input" type="radio" name="flexRadioDefault" id="flexRadioDefault1">
		  <label class="form-check-label" for="flexRadioDefault1">
			Default radio
		  </label>
		</div>
		 */

		builder.OpenElement(100, "div");
		var outerCss = CssBuilder.Default("form-check")
				.AddClass("form-check-inline", when: this.Inline)
			.Build();
		builder.AddAttribute(101, "class", outerCss);

		var isChecked = this.Context.CurrentValue?.Equals(this.Value) == true;
		var resolvedCss = CssBuilder.Default("form-check-input")
				.AddClassFromAttributes(this.AdditionalAttributes)
				.AddClassIfNotEmpty(this.Context.FieldClass)
			.Build();
		builder.OpenElement(200, "input");
		builder.AddMultipleAttributes(202, this.AdditionalAttributes);
		builder.AddAttributeIfNotNullOrEmpty(203, "class", resolvedCss);
		builder.AddAttribute(205, "disabled", this.Disabled);
		builder.AddAttribute(210, "id", this.Id);
		builder.AddAttribute(215, "type", "radio");
		builder.AddAttribute(220, "name", this.Context.GroupName);
		builder.AddAttribute(225, "value", BindConverter.FormatValue(this.Value?.ToString()));
		builder.AddAttributeIfNotNullOrEmpty(226, "aria-disabled", this.Disabled ? "true" : null);
		builder.AddAttributeIfNotNullOrEmpty(227, "aria-label", this.AriaLabel);
		builder.AddAttribute(228, "aria-checked", isChecked.ToAttributeValue());
		builder.AddAttribute(229, "checked", isChecked ? this.GetToggledTrueValue() : null);
		builder.AddAttribute(230, "required", this.Required);
		builder.AddAttribute(235, "onchange", this.Context.ChangeEventCallback);
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

	// This is an unfortunate hack, but is needed for the scenario described by test InputRadioGroupWorksWithMutatingSetter.
	// Radio groups are special in that modifying one <input type=radio> instantly and implicitly also modifies the previously
	// selected one in the same group. As such, our SetUpdatesAttributeName mechanism isn't sufficient to stay in sync with the
	// DOM, because the 'change' event will fire on the new <input type=radio> you just selected, not the previously-selected
	// one, and so the previously-selected one doesn't get notified to update its state in the old rendertree. So, if the setter
	// reverts the incoming value, the previously-selected one would produce an empty diff (because its .NET value hasn't changed)
	// and hence it would be left unselected in the DOM. If you don't understand why this is a problem, try commenting out the
	// line that toggles _trueValueToggle and see the E2E test fail.
	//
	// This hack works around that by causing InputRadio *always* to force its own 'checked' state to be true in the DOM if it's
	// true in .NET, whether or not it was true before, by continally changing the value that represents 'true'. This doesn't
	// really cause any significant increase in traffic because if we're rendering this InputRadio at all, sending one more small
	// attribute value is inconsequential.
	//
	// Ultimately, a better solution would be to make SetUpdatesAttributeName smarter still so that it knows about the special
	// semantics of radio buttons so that, when one <input type="radio"> changes, it treats any previously-selected sibling
	// as needing DOM sync as well. That's a more sophisticated change and might not even be useful if the radio buttons
	// aren't truly siblings and are in different DOM subtrees (and especially if they were rendered by different components!)
	private string GetToggledTrueValue() {
		this._trueValueToggle = !this._trueValueToggle;
		return this._trueValueToggle ? "a" : "b";
	}

}