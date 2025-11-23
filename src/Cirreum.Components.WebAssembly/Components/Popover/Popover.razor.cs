namespace Cirreum.Components;

using Cirreum.Components.Interop;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

public partial class Popover : IDisposable {

	private static readonly string[] fallbackOptions = [
		"bottom-start",
		"bottom",
		"bottom-end",
		"top-start",
		"top",
		"top-end",
		"right",
		"left"
	];

	private readonly string PopperArrowId = IdGenerator.Next;
	private bool IsRendering;
	private bool IsShowing;
	private string? _currentPopperId;
	private ElementReference PopoverElement;

	[Inject]
	protected IJSAppModule JSApp { get; set; } = default!;

	[Parameter]
	public string Style { get; set; } = "";

	public bool IsOpen { get; private set; }
	private void SetIsOpen(bool value) {
		this.IsOpen = value;
		if (this.OnOpenChanged is not null) {
			this.OnOpenChanged(value);
		}
	}

	[Parameter]
	public Func<bool, Task>? OnOpenChanged { get; set; }

	[Parameter]
	public Placement Placement { get; set; } = Placement.Auto;

	[Parameter]
	public ElementReference TriggerElement { get; set; }

	[Parameter]
	public RenderFragment ChildContent { get; set; } = default!;

	[Parameter]
	public RenderFragment PopoverHeader { get; set; } = default!;

	[Parameter]
	public RenderFragment PopoverBody { get; set; } = default!;

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
	/// Default: [0, 8]
	/// </summary>
	[Parameter]
	public int[] Offset { get; set; } = [0, 8];

	/// <summary>
	/// The optional padding for the overflow prevention. Default: 0
	/// </summary>
	[Parameter]
	public int OverflowPadding { get; set; } = 0;

	[Parameter]
	public Func<string, Task>? OnOutsideClicked { get; set; }

	private string ClassList =>
		CssBuilder
			.Default("popover")
			.AddClass("bs-popover-auto")
			.AddClass("fade")
			.AddClass("show", when: this.IsShowing)
		.Build();

	public void Open() {

		if (this.IsRendering) {
			return;
		}

		this.IsRendering = true;

		this.RunAfterRender(this.Show);

		this.Update();

	}
	private void Show() {

		this.IsShowing = true;

		// we connect PopperJs after rendering the popover
		this.ConnectPopperJs();

		this.RunAfterRender(() => {
			if (this.OnOutsideClicked is not null) {
				this.ClickDetector.Register(this.ElementId, this.OnOutsideClicked);
			}
			this.SetIsOpen(true);
		});

		this.Update();

	}
	public void Close() {

		if (this.IsOpen is false) {
			return;
		}

		// we disconnect elements and listeners before
		// not-rendering (hiding) the popover
		this.ClickDetector.Unregister(this.ElementId);
		this.DisconnectPopperJs();

		this.IsShowing = false;
		this.IsRendering = false;

		this.SetIsOpen(false);

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
		this._currentPopperId = this.JSApp.ShowPopperJS(options);
	}
	private void DisconnectPopperJs() {
		var id = this._currentPopperId;
		this._currentPopperId = null;
		if (id is not null) {
			this.JSApp.ClosePopperJS(id);
		}
	}

	protected override void Dispose(bool disposing) {
		this.DisconnectPopperJs();
	}

}