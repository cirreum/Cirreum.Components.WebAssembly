namespace Cirreum.Components;

using Microsoft.AspNetCore.Components;

[CascadingTypeParameter(nameof(TItemType))]
public abstract class DropdownInteractiveItem<TItemType> : ComponentBase, IDropdownItemInternal<TItemType> {

	[CascadingParameter]
	protected DropdownBase<TItemType> Dropdown { get; set; } = default!;

	[Parameter]
	public RenderFragment? ChildContent { get; set; }

	[Parameter]
	public RenderFragment<IDropdownInteractiveItem<TItemType>>? InteractiveTemplate { get; set; }

	[Parameter, EditorRequired]
	public TItemType Value { get; set; } = default!;

	/// <summary>
	/// Is this item disabled.
	/// </summary>
	[Parameter]
	public bool IsDisabled { get; set; }
	protected string AriaIsDisabled => this.IsDisabled.ToAttributeValue();
	protected string TabIndex => this.IsDisabled ? "-1" : "0";

	/// <summary>
	/// Gets the Id of the  item.
	/// </summary>
	public string Id { get; private set; } = IdGenerator.Next;
	protected ElementReference ItemReference { get; set; }
	protected string ItemClass => CssBuilder
		.Default("dropdown-item")
		.AddClass("active", when: this.IsActive)
		.AddClass("disabled", when: this.IsDisabled)
		.AddClass("focused", when: this._isFocused)
	.Build();

	/// <summary>
	/// Sets the value for the aria-current attribute.
	/// </summary>
	public string AriaCurrent { get; set; } = "";


	/// <inherit/>
	public bool IsActive { get; private set; }
	void IDropdownItemInternal<TItemType>.SetIsActive(bool value) {
		this.IsActive = value;
	}

	private bool _isFocused;
	bool IDropdownItemInternal<TItemType>.IsFocused => _isFocused;
	async ValueTask IDropdownItemInternal<TItemType>.FocusAsync(bool preventScroll) {
		await this.ItemReference.FocusAsync(preventScroll);
	}

	protected void HandleFocus() {
		this._isFocused = true;
	}
	protected void HandleBlur() {
		this._isFocused = false;
	}
	protected async Task HandleClick() {
		await this.Dropdown.HandleItemSelectionAsync(this);
	}

	protected override void OnInitialized() {
		base.OnInitialized();
		if (this.Dropdown is null) {
			throw new Exception("DropdownItem 'Value' doesn't match the parent's Dropdown 'TItemType'.");
		}
		this.Dropdown.RegisterItem(this);
	}

}