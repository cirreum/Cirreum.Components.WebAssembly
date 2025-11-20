namespace Cirreum.Components;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

public partial class Tooltip {

	private static readonly string[] fallbackOptions = [
		"top-start",
		"top",
		"top-end",
		"bottom-start",
		"bottom",
		"bottom-end",
		"right",
		"left"
	];

	private readonly string TooltipArrowId = IdGenerator.Next;
	private volatile bool IsOpening;
	private bool IsOpened;
	private bool IsEntered;
	private bool IsPointerDown; // Track pointer state (mouse, touch, pen)
	private bool RecentPointerInteraction; // Track if pointer interaction just happened
	private string? _currentPopperId;
	private ElementReference TriggerElement;
	private ElementReference PopoverElement;
	private Timer? OpenDelayTimer;
	private Timer? PointerResetTimer;

	/// <summary>
	/// Controls whether the tooltip is enabled and therefore showable.
	/// </summary>
	[Parameter]
	public bool IsEnabled { get; set; } = true;

	/// <summary>
	/// Your content that you want to have a tooltip.
	/// </summary>
	[Parameter, EditorRequired]
	public RenderFragment ChildContent { get; set; } = default!;

	/// <summary>
	/// The text to display for the tooltip.
	/// </summary>
	[Parameter]
	public string Title { get; set; } = "";

	/// <summary>
	/// Optional template to display custom content instead of the <see href="Title"/>.
	/// </summary>
	[Parameter]
	public RenderFragment? TitleTemplate { get; set; }

	/// <summary>
	/// Sets the name of the Section Outlet where the Tooltip will be located within the DOM. Default: tooltip-container-section
	/// </summary>
	/// <remarks>
	/// This requires the <see cref="TooltipContainer"/> be included within your app (usually App.razor).
	/// Or you can use your own custom section outlet and section name. Or you can set to <see langword="null"/>
	/// to have it render in-line.
	/// </remarks>
	[Parameter]
	public string? TooltipSectionName { get; set; } = TooltipConfig.DefaultSectionName;

	/// <summary>
	/// When <see langword="true"/>, the tooltip will be shown on-focus in addition to hover. Default: <see langword="false"/>
	/// </summary>
	[Parameter]
	public bool ShowOnFocus { get; set; }

	/// <summary>
	/// Any custom styles to apply to the tooltip.
	/// </summary>
	[Parameter]
	public string? Style { get; set; }

	/// <summary>
	/// Any additional classes to apply to the tooltip.
	/// </summary>
	[Parameter]
	public string? Css { get; set; }

	/// <summary>
	/// Any custom styles to apply to the container that wraps your content.
	/// </summary>
	[Parameter]
	public string? ContainerStyle { get; set; }

	/// <summary>
	/// Any additional classes to apply to the container that wraps your content.
	/// </summary>
	[Parameter]
	public string? ContainerCss { get; set; }

	/// <summary>
	/// Bindable property to control the tooltips open status.
	/// </summary>
	[Parameter]
	public bool IsOpen { get; set; }
	private void SetIsOpen(bool value) {
		if (this.IsOpen != value) {
			this.IsOpen = value;
			if (this.IsOpenChanged.HasDelegate) {
				this.IsOpenChanged.InvokeAsync(this.IsOpen);
			}
		}
	}

	[Parameter]
	public EventCallback<bool> IsOpenChanged { get; set; }

	[Parameter]
	public Placement Placement { get; set; } = Placement.Top;
	private string ResolvedPlacement => this.Placement.ToName();

	/// <summary>
	/// The number of milliseconds to delay before showing the tooltip.
	/// </summary>
	/// <remarks>
	/// The default: is 1000ms (1s). If the value is less than 100ms, then
	/// disables the delay altogether. The maximum is 5000ms.
	/// </remarks>
	[Parameter]
	public int Delay { get; set; } = 1000;
	private int ResolvedDelay {
		get {
			if (this.Delay < 100) {
				return 0;
			}
			if (this.Delay > 5000) {
				return 5000;
			}
			return this.Delay;
		}
	}

	/// <summary>
	/// Default: <see langword="true"/>
	/// </summary>
	[Parameter]
	public bool ShowArrow { get; set; } = true;

	/// <summary>
	/// The optional padding for the Arrow. Default: 0
	/// </summary>
	[Parameter]
	public int ArrowPadding { get; set; } = 0;

	/// <summary>
	/// Default: [0, 4]
	/// </summary>
	[Parameter]
	public int[] Offset { get; set; } = [0, 6];

	/// <summary>
	/// The optional padding for the overflow prevention. Default: 0
	/// </summary>
	[Parameter]
	public int OverflowPadding { get; set; } = 0;

	private string? ResolvedSectionName => this.TooltipSectionName.NullIfWhiteSpace();

	private string? ContainerClass;

	private string TooltipCss => CssBuilder
			.Default("tooltip")
				.AddClass("bs-tooltip-auto")
				.AddClass("fade")
				.AddClass("show", when: this.IsOpened)
				.AddClassIfNotEmpty(this.Css)
			.Build();

	private bool HasContent => this.Title.HasValue() || this.TitleTemplate is not null;
	private bool CanShow => this.IsEnabled && this.HasContent && this.ResolvedDelay > 0;

	private void OnPointerDown() {
		this.IsPointerDown = true;
		this.RecentPointerInteraction = true;
		this.CancelPointerResetTimer();
	}

	private void OnPointerUp() {
		this.IsPointerDown = false;
		this.CancelPointerResetTimer();
		this.PointerResetTimer = new Timer((_) => {
			this.CancelPointerResetTimer();
			this.RecentPointerInteraction = false;
		}, null, TimeSpan.FromMilliseconds(50), Timeout.InfiniteTimeSpan);
	}

	private void OnFocus() {
		if (this.ShowOnFocus && this.CanShow) {
			if (!this.IsPointerDown && !this.RecentPointerInteraction) {
				this.Open();
			}
		}
	}
	private void OnBlur() {
		if (this.ShowOnFocus || this.IsEntered) {
			this.Close();
		}
		this.IsEntered = false;
		this.IsPointerDown = false; // Reset pointer state on blur
		this.RecentPointerInteraction = false; // Reset recent interaction on blur
		this.CancelPointerResetTimer();
	}

	private void OnPointerEnter(PointerEventArgs e) {
		// Only show tooltip for mouse hover, not touch
		if (e.PointerType == "mouse") {
			this.IsEntered = true;
			if (this.CanShow) {
				this.DelayThenOpen();
			}
		}
	}
	private void OnPointerLeave(PointerEventArgs e) {
		this.IsEntered = false;
		this.Close();
	}


	private void DelayThenOpen() {
		if (this.OpenDelayTimer is null) {
			if (this.ResolvedDelay < 100) {
				this.Open();
				return;
			}
			this.OpenDelayTimer = new Timer((_) => {
				if (this.IsEntered) {
					this.Open();
				} else {
					this.CancelTimer();
				}
			}, null, this.ResolvedDelay, Timeout.Infinite);
		}
	}
	private void Open() {

		if (this.IsOpening) {
			return;
		}
		this.IsOpening = true;
		this.IsOpened = false;

		this.CancelTimer();

		this.RunAfterRender(this.Show);

		this.Update();

	}
	private void Show() {

		if (this.IsOpening is false) {
			return;
		}

		this.ConnectPopperJs();
		this.IsOpened = true;
		this.Update();

		this.SetIsOpen(true);

	}
	private void Close() {

		this.CancelTimer();

		if (this.IsOpened) {
			// we disconnect PopperJs before not-rendering (hiding) the tooltip
			this.DisconnectPopperJs();
			this.IsOpened = false;
			this.Update();
		}

		if (this.IsOpening) {
			this.IsOpening = false;
			this.Update();
		}

		this.SetIsOpen(false);

	}

	private void CancelTimer() {
		if (this.OpenDelayTimer is not null) {
			this.OpenDelayTimer.Dispose();
			this.OpenDelayTimer = null;
		}
	}
	private void CancelPointerResetTimer() {
		if (this.PointerResetTimer is not null) {
			this.PointerResetTimer.Dispose();
			this.PointerResetTimer = null;
		}
	}
	private PopperJSOptions ResolveOptions() {
		return new PopperJSOptions {
			TriggerElement = this.TriggerElement,
			PopperElement = this.PopoverElement,
			Placement = this.Placement,
			Modifiers = [
					new PopperModifer (
						name: "offset",
						options: new {
							offset = this.Offset
						}
					),
					new PopperModifer (
						name: "arrow",
						options: new {
							element = ".tooltip-arrow",
							padding = this.ArrowPadding
						}
					),
					new PopperModifer (
						name: "flip",
						options: new {
							fallbackPlacements = fallbackOptions
						}
					),
					new PopperModifer (
						name: "preventOverflow",
						options: new {
							padding = this.OverflowPadding
						}
					)
												]
		};
	}
	private void ConnectPopperJs() {
		var options = this.ResolveOptions();
		this._currentPopperId = this.JS.ShowPopperJS(options);
	}
	private void DisconnectPopperJs() {
		var id = this._currentPopperId;
		this._currentPopperId = null;
		if (id is not null) {
			this.JS.ClosePopperJS(id);
		}
	}

	protected override void Dispose(bool disposing) {
		this.DisconnectPopperJs();
		this.OpenDelayTimer?.Dispose();
		this.OpenDelayTimer = null;
		this.PointerResetTimer?.Dispose();
		this.PointerResetTimer = null;
	}

	protected override void OnParametersSet() {
		base.OnParametersSet();

		this.ContainerClass = CssBuilder
			.Default("tooltip-contents-container")
				.AddClassIfNotEmpty(this.ContainerCss)
			.Build();

	}

}