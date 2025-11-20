namespace Cirreum.Components;

using Microsoft.AspNetCore.Components;

public partial class Progress {

	/// <summary>
	/// The id of this progress bar's element.
	/// </summary>
	public string ElementId { get; } = IdGenerator.Next;

	/// <summary>
	/// Gets or sets the color of the progress bar.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Default: <see cref="BackgroundColor.Primary"/>
	/// </para>
	/// </remarks>
	[Parameter]
	public BackgroundColor Color { get; set; } = BackgroundColor.Primary;

	/// <summary>
	/// Gets or sets if the progress is in Indeterminate mode.
	/// </summary>
	[Parameter]
	public bool Indeterminate { get; set; }

	/// <summary>
	/// Gets or sets the current percentage value of the progress bar.
	/// </summary>
	[Parameter]
	public int Percentage { get; set; } = 0;

	/// <summary>
	/// The desired width of the progress bar. Default: '240px'
	/// </summary>
	[Parameter]
	public string Width { get; set; } = "240px";

	/// <summary>
	/// Gets or sets the aria-label value
	/// </summary>
	[Parameter]
	public string AriaLabel { get; set; } = "";

	/// <summary>
	/// Gets or sets the aria-labelledby value
	/// </summary>
	[Parameter]
	public string AriaLabelledBy { get; set; } = "";

	private int validPercentage = 0;

	protected override void OnParametersSet() {
		base.OnParametersSet();
		var tempPercentage = this.Percentage;
		if (tempPercentage > 100) {
			tempPercentage = 100;
		}
		if (tempPercentage < 0) {
			tempPercentage = 0;
		}
		validPercentage = tempPercentage;
	}

}