namespace Cirreum.Components;

using Microsoft.AspNetCore.Components;

public partial class Accordion {

	/// <summary>
	/// Display flush within the containing element.
	/// </summary>
	[Parameter]
	public bool IsFlush { get; set; }

	/// <summary>
	/// The expand mode of accordion items - single or multiple.
	/// </summary>
	/// <remarks>
	/// <para>
	/// See <see cref="AccordionExpandMode"/>
	/// </para>
	/// </remarks>
	[Parameter]
	public AccordionExpandMode ExpandMode { get; set; }

	/// <summary>
	/// Gets or sets a callback when a accordion item is changed.
	/// </summary>
	[Parameter]
	public EventCallback<AccordionItem> OnAccordionItemChange { get; set; }

	/// <summary>
	/// The template for the Accordion body.
	/// </summary>
	[Parameter]
	public RenderFragment? ChildContent { get; set; }

	private string CssClass =>
		CssBuilder.Default("accordion")
			.AddClass("accordion-flush", when: this.IsFlush)
		.Build();

	public async Task CollapseAll() {
		foreach (var child in this._children) {
			await child.Collapse();
		}
	}

	public async Task ExpandAll() {
		foreach (var child in this._children) {
			await child.Expand();
		}
	}

	private readonly List<AccordionItem> _children = [];
	internal void RegisterChild(AccordionItem item) {
		this._children.Add(item);
	}

	internal async Task HandleChildItemChanged(AccordionItem item) {
		if (this.OnAccordionItemChange.HasDelegate) {
			await this.OnAccordionItemChange.InvokeAsync(item);
		}
	}


}