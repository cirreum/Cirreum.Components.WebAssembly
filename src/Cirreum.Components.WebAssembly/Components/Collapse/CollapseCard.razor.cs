namespace Cirreum.Components;

using Microsoft.AspNetCore.Components;

public partial class CollapseCard {

	/// <summary>
	/// The text to display in the card-header.
	/// </summary>
	[Parameter]
	public string Title { get; set; } = string.Empty;

	/// <summary>
	/// Optional template for the card-header.
	/// </summary>
	[Parameter]
	public RenderFragment? TitleTemplate { get; set; }

	/// <summary>
	/// The template for the card-body.
	/// </summary>
	[Parameter]
	public RenderFragment? ChildContent { get; set; }

	/// <summary>
	/// Optional template for the card-footer.
	/// </summary>
	[Parameter]
	public RenderFragment? FooterContent { get; set; }

	/// <summary>
	/// Set to <see langword="true"/> to render <see cref="FooterContent"/>
	/// </summary>
	[Parameter]
	public bool ShowFooter { get; set; } = true;

	/// <summary>
	/// Set to <see langword="true"/> to render <see cref="FooterContent"/> when the body is collapsed.
	/// </summary>
	[Parameter]
	public bool ShowFooterWhenCollapsed { get; set; } = false;

	/// <summary>
	/// Set to <see langword="false"/> to disable the collapsing of the card-body.
	/// </summary>
	[Parameter]
	public bool Collapsable { get; set; } = true;

	/// <summary>
	/// The collapse toggle button color. Default: <see cref="ButtonColor.Primary"/>
	/// </summary>
	[Parameter]
	public ButtonColor CollapseButtonColor { get; set; } = ButtonColor.Primary;


	/// <summary>
	/// Is the content current collapsed.
	/// </summary>
	[Parameter]
	public bool IsCollapsed { get; set; }

	/// <summary>
	/// Event callback for when the <see cref="IsCollapsed"/> value is changed.
	/// </summary>
	[Parameter]
	public EventCallback<bool> IsCollapsedChanged { get; set; }


	/// <summary>
	/// Set to <see langword="true"/> to render <see cref="CopyToClipboardButton"/>
	/// </summary>
	[Parameter]
	public bool ShowCopyButton { get; set; } = true;

	/// <summary>
	/// Optional text that can be copied to the clipboard when <see cref="ShowCopyButton"/> is <see langword="true"/>.
	/// </summary>
	[Parameter]
	public string? ClipboardText { get; set; }

	/// <summary>
	/// The Header's Background <see cref="BackgroundColor"/>, e.g., Primary, Secondary etc. Default: <see cref="BackgroundColor.Primary"/>
	/// </summary>
	[Parameter]
	public BackgroundColor HeaderBackgroundColor { get; set; } = BackgroundColor.None;

	/// <summary>
	/// The Header's Background <see cref="TextColor"/>, e.g., Light, Dark etc. Default: <see cref="TextColor.Body"/>
	/// </summary>
	[Parameter]
	public TextColor HeaderTextColor { get; set; } = TextColor.None;

	/// <summary>
	/// The Header's custom css class list. Default: text-bg-primary
	/// </summary>
	[Parameter]
	public string HeaderClass { get; set; } = "text-bg-primary";

	private bool HasFooter => this.FooterContent is not null &&
		this.ShowFooter;
	private bool CanShowFooter => this.HasFooter &&
		(this.Collapsable is false || (this.IsCollapsed is false || this.ShowFooterWhenCollapsed));
	private bool HideFooter => this.Collapsable && this.HasFooter && this.CanShowFooter is false;
	private string CardCss =>
		CssBuilder.Default("card")
			.AddClass("has-footer", when: this.Collapsable && this.HasFooter)
			.AddClass("hide-footer", when: this.HideFooter)
		.Build();
	private string CardHeaderCss =>
		CssBuilder.Default("card-header")
			.AddClassIfNotEmpty(this.HeaderBackgroundColor.ToName())
			.AddClassIfNotEmpty(this.HeaderTextColor.ToName())
			.AddClassIfNotEmpty(this.HeaderClass)
			.AddClass("collapsable", when: this.Collapsable)
			.AddClass("collapsed", when: this.Collapsable && this.IsCollapsed)
		.Build();
	private string CardHeaderCollapseBtnCss =>
		CssBuilder.Default("btn")
			.AddClassIfNotEmpty(this.CollapseButtonColor.ToName())
			.AddClass("btn-xs")
			.AddClass("border-0")
			.AddClass("ms-1")
		.Build();
	private async Task ToggleCollapsed() {
		if (this.Collapsable) {
			this.IsCollapsed = !this.IsCollapsed;
			this.StateHasChanged();
			await this.IsCollapsedChanged.InvokeAsync(this.IsCollapsed);
		}
	}

}