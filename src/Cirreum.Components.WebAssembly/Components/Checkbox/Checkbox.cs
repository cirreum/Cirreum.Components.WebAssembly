namespace Cirreum.Components;

using Cirreum.Components.Interop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using System.Diagnostics.CodeAnalysis;

public class Checkbox : InputBase<bool> {

	private const bool VALUE_FOR_INDETERMINATE = false;
	private bool _intermediate = false;
	private bool _hasInitializedIndeterminate;

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
	/// Gets or sets if the Checkbox is required.
	/// </summary>
	[Parameter]
	public bool Required { get; set; }

	/// <summary>
	/// Gets or sets if the Checkbox is disabled.
	/// </summary>
	[Parameter]
	public bool Disabled { get; set; }

	/// <summary>
	/// Gets or sets if the checkbox's should be inline, instead of stacked.
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
	/// Does the checkbox support the 3-States mode.
	/// </summary>
	[Parameter]
	public bool ThreeState { get; set; }

	/// <summary>
	/// Gets or sets a value indicating the order of the three states of the Checkbox.
	/// False (by default), the order is Unchecked -> Checked -> Intermediate.
	/// True: the order is Unchecked -> Intermediate -> Checked.
	/// </summary>
	[Parameter]
	public bool ThreeStateOrderUncheckToIntermediate { get; set; } = false;

	/// <summary>
	/// Gets or sets a value indicating whether the user can display the indeterminate state by clicking the Checkbox.
	/// If this is not the case, the checkbox can be started in the indeterminate state, but the user cannot activate it with the mouse.
	/// Default is true.
	/// </summary>
	[Parameter]
	public bool ShowIndeterminate { get; set; } = true;

	/// <summary>
	/// Gets or sets the state of the Checkbox: true, false or null.
	/// </summary>
	[Parameter]
	public bool? CheckState { get; set; }

	/// <summary>
	/// Gets or sets a callback that updates the <see cref="CheckState"/>.
	/// </summary>
	[Parameter]
	public EventCallback<bool?> CheckStateChanged { get; set; }


	/// <summary>
	/// Gets or sets the associated <see cref="ElementReference"/>.
	/// <para>
	/// May be <see langword="null"/> if accessed before the component is rendered.
	/// </para>
	/// </summary>
	[DisallowNull]
	public ElementReference? Element { get; protected set; }

	[Inject]
	private IJSAppModule JSApp { get; set; } = default!;

	private async Task SetCurrentAndIntermediateAsync(bool? value) {
		switch (value) {
			// Checked
			case true:
				this.CurrentValue = true;
				await this.SetIntermediateAsync(false);
				break;

			// Unchecked
			case false:
				this.CurrentValue = false;
				await this.SetIntermediateAsync(false);
				break;

			// Indeterminate
			default:
				this.CurrentValue = VALUE_FOR_INDETERMINATE;
				await this.SetIntermediateAsync(true);
				break;
		}
	}

	private Task SetIntermediateAsync(bool intermediate) {
		this.JSApp.SetCheckBoxIndeterminate(this.Id, intermediate);
		this._intermediate = intermediate;
		return Task.CompletedTask;
	}
	private async Task SetCurrentCheckStateAsync(bool newChecked) {

		bool? newState;

		// Uncheck -> Indeterminate -> Check
		if (this.ThreeStateOrderUncheckToIntermediate) {
			// NewChecked  |  Intermediate  |  NewState
			//   True             False          [-]
			//   True             True           [x]
			//   False            False          [ ]

			// Uncheck -> Intermediate (or Check is ShowIndeterminate is false)
			newState = newChecked && !this._intermediate
				? this.ShowIndeterminate
				? null
				: true
				: newChecked && this._intermediate;
		}

		// Uncheck -> Check -> Indeterminate
		else {
			// NewChecked  |  Intermediate  |  NewState
			//   True             False          [x]
			//   False            False          [-]
			//   True             true           [ ]

			// Uncheck -> Check
			newState = newChecked && !this._intermediate
				? true
				: !newChecked && !this._intermediate
				? this.ShowIndeterminate
				? null
				: false
				: false;
		}

		await this.SetCurrentAndIntermediateAsync(newState);
		await this.UpdateAndRaiseCheckStateEventAsync(newState);

	}

	private async Task UpdateAndRaiseCheckStateEventAsync(bool? value) {
		if (this.CheckState != value) {
			this.CheckState = value;
			if (this.CheckStateChanged.HasDelegate) {
				await this.CheckStateChanged.InvokeAsync(value);
			}
		}
	}

	private async Task OnCheckedChangeHandlerAsync(bool newValue) {
		if (!this.ThreeState) {
			await Task.Delay(1);
		}

		if (this.ThreeState) {
			await this.SetCurrentCheckStateAsync(newValue);
		} else {
			this.CurrentValue = newValue;
			await this.SetIntermediateAsync(false);
			await this.UpdateAndRaiseCheckStateEventAsync(newValue);
		}
	}


	/// <inheritdoc />
	public override async Task SetParametersAsync(ParameterView parameters) {

		var checkStateEdited = false;
		var currentlyThreeState = this.ThreeState;
		if (parameters.TryGetValue<bool>(nameof(this.ThreeState), out var newThreeState)) {
			if (currentlyThreeState != newThreeState) {
				currentlyThreeState = newThreeState;
			}
		}

		if (parameters.TryGetValue<bool?>(nameof(this.CheckState), out var newCheckState)) {

			if (!currentlyThreeState) {
				throw new ArgumentException("Set the `ThreeState` attribute to True to use this `CheckState` property.");
			}

			if (this.CheckState != newCheckState) {
				checkStateEdited = true;
			}

		}

		this.ValueExpression ??= () => this.CurrentValue;

		await base.SetParametersAsync(parameters);

		if (checkStateEdited) {
			await this.SetCurrentAndIntermediateAsync(newCheckState);
		}

	}

	/// <inheritdoc />
	protected override void BuildRenderTree(RenderTreeBuilder builder) {

		builder.OpenElement(100, "div");
		var outerCss = CssBuilder.Default("form-check")
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

		builder.AddAttribute(235, "onchange", EventCallback.Factory.CreateBinder<bool>(this, this.OnCheckedChangeHandlerAsync, this.CurrentValue));
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

	protected override async Task OnAfterRenderAsync(bool firstRender) {
		if (this._hasInitializedIndeterminate is false) {
			this._hasInitializedIndeterminate = true;
			if (this.ThreeState && this.CheckState == null) {
				await this.SetIntermediateAsync(true);
			}
		}
	}

	/// <inheritdoc />
	protected override bool TryParseValueFromString(string? value, out bool result, [NotNullWhen(false)] out string? validationErrorMessage)
		=> throw new NotSupportedException($"This component does not parse string inputs. Bind to the '{nameof(this.CurrentValue)}' property, not '{nameof(this.CurrentValueAsString)}'.");

}