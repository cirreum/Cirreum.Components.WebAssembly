namespace Cirreum.Components;

using Cirreum.Extensions;
using Microsoft.AspNetCore.Components;
using System.Globalization;

public partial class CounterBadge {

	[Parameter]
	public string? Class { get; set; }
	protected string? ClassValue => CssBuilder
		.Empty()
			.AddClass("counterbadge")
			.AddClass(this.BackgroundColor.ToName(), when: this.BackgroundColor != BackgroundColor.None)
			.AddClass(this.Color.ToName(), when: this.Color != TextColor.None)
			.AddClass($"border border{this.BackgroundColor.ToShortName()}", when: this.BackgroundColor != BackgroundColor.None)
			.AddClassIfNotEmpty(this.Class)
		.Build();

	[Parameter]
	public string? Style { get; set; }
	protected string? StyleValue => StyleBuilder
		.Empty()
			.AddStyleIfNotEmpty(this.Style)
			.AddStyle("left", $"{this._horizontalPosition.ToString(CultureInfo.InvariantCulture)}%")
			.AddStyle("bottom", $"{this._verticalPosition.ToString(CultureInfo.InvariantCulture)}%")
		.NullIfEmpty();

	/// <summary>
	/// Gets or sets the child content of component, the content that the badge will apply to.
	/// </summary>
	[Parameter]
	public RenderFragment? ChildContent { get; set; }

	/// <summary>
	/// Gets or sets the number displayed inside the badge.
	/// Can be enriched with a plus sign with <see cref="ShowOverflow"/>
	/// </summary>
	[Parameter]
	public int? Count { get; set; }

	/// <summary>
	/// Gets or sets the content you want inside the badge, to customize the badge content.
	/// </summary>
	[Parameter]
	public RenderFragment<int?>? BadgeTemplate { get; set; }

	/// <summary>
	/// Gets or sets the maximum number that can be displayed inside the badge.
	/// Default is 99.
	/// </summary>
	[Parameter]
	public int? Max { get; set; } = 99;

	/// <summary>
	/// Gets or sets the horizontal position of the badge in percentage in relation to the left of the container (right in RTL).
	/// Default value is 60 (80 when Dot=true).
	/// </summary>
	[Parameter]
	public int HorizontalPosition { get; set; }
	private int _horizontalPosition;

	/// <summary>
	/// Gets or sets the vertical position of the badge in percentage in relation to the bottom of the container.
	/// Default value is 60 (80 when Dot=true).
	/// </summary>
	[Parameter]
	public int VerticalPosition { get; set; }
	private int _verticalPosition;

	/// <summary>
	/// Gets or sets the background color. Default is None/Empty.
	/// </summary>
	[Parameter]
	public BackgroundColor BackgroundColor { get; set; } = BackgroundColor.None;

	/// <summary>
	/// Gets or sets the font color. Default is None/Empty.
	/// </summary>
	[Parameter]
	public TextColor Color { get; set; } = TextColor.None;

	/// <summary>
	///  Gets or sets if just a dot should be displayed. Count will be ignored if this is set to true.
	///  Defaults to false.
	/// </summary>
	[Parameter]
	public bool Dot { get; set; } = false;

	/// <summary>
	/// Gets or sets if the counter badge is displayed based on the specified lambda expression.
	/// For example to show the badge when the count is greater than 4, use ShowWhen=@(Count => Count > 4)
	/// Default the badge shows when the count is greater than 0.
	/// </summary>
	[Parameter]
	public Func<int?, bool> ShowWhen { get; set; } = Count => Count > 0;

	/// <summary>
	/// If an plus sign should be displayed when the <see cref="Count"/> is greater than <see cref="Max"/>.
	/// Defaults to true.
	///</summary>
	[Parameter]
	public bool ShowOverflow { get; set; } = true;

	protected override void OnParametersSet() {
		base.OnParametersSet();
		this._horizontalPosition =
			this.HorizontalPosition == 0
				? this.Dot ? 80 : 60
				: this.HorizontalPosition;
		this._verticalPosition =
			this.VerticalPosition == 0
				? this.Dot ? 80 : 60
				: this.VerticalPosition;
	}

}
