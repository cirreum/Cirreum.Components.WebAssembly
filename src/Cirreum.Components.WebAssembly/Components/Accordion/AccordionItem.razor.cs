namespace Cirreum.Components;

using Microsoft.AspNetCore.Components;

public partial class AccordionItem {

	private readonly string _elementId = IdGenerator.Next;
	private readonly string _toggleButtonId = IdGenerator.Next;

	[CascadingParameter]
	private Accordion Parent { get; set; } = default!;

	/// <summary>
	/// The text to display in the Accordion header.
	/// </summary>
	[Parameter]
	public string Header { get; set; } = string.Empty;

	/// <summary>
	/// Optional template for the Accordion header.
	/// </summary>
	[Parameter]
	public RenderFragment? HeaderTemplate { get; set; }

	/// <summary>
	/// The template for the Accordion body.
	/// </summary>
	[Parameter]
	public RenderFragment? ChildContent { get; set; }

	/// <summary>
	/// Is the content current expanded.
	/// </summary>
	[Parameter]
	public bool IsExpanded { get; set; }
	private bool _isExpanded;

	/// <summary>
	/// Event callback for when the <see cref="IsExpanded"/> value is changed.
	/// </summary>
	[Parameter]
	public EventCallback<bool> IsExpandedChanged { get; set; }

	private string ToggleCss =>
		CssBuilder.Default("accordion-button")
			.AddClass("collapsed", when: !this._isExpanded)
		.Build();
	internal async Task ToggleCollapsed() {
		if (!this._isExpanded && this.Parent.ExpandMode == AccordionExpandMode.Single) {
			// tell parent to collapse the other children...
			await this.Parent.CollapseAll();
		}
		if (this._isExpanded) {
			await this.Collapse();
		} else {
			await this.Expand();
		}
	}
	internal async Task Collapse() {
		if (this._isExpanded) {
			this.IsExpanded = this._isExpanded = false;
			this.StateHasChanged();
			await this.IsExpandedChanged.InvokeAsync(this._isExpanded);
			await this.Parent.HandleChildItemChanged(this);
		}
	}
	internal async Task Expand() {
		if (!this._isExpanded) {
			this.IsExpanded = this._isExpanded = true;
			this.StateHasChanged();
			await this.IsExpandedChanged.InvokeAsync(this._isExpanded);
			await this.Parent.HandleChildItemChanged(this);
		}
	}

	protected override void OnInitialized() {
		this._isExpanded = this.IsExpanded;
		this.Parent.RegisterChild(this);
	}

}