namespace Cirreum.Components;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

/// <summary>
/// A Blazor component that adds auto-hiding scrollbar behavior to a specified target element.
/// Shows scrollbars on mouse enter and hides them after mouse leave with configurable delays.
/// </summary>
/// <remarks>
/// <para>
/// This component uses JavaScript interop to manage scrollbar visibility through data attributes.
/// The target element will have <c>data-scrollbar="show"</c> or <c>data-scrollbar="hide"</c> 
/// attributes applied based on mouse interactions.
/// </para>
/// <para>
/// The component requires corresponding CSS rules to style the scrollbar based on these data attributes.
/// </para>
/// </remarks>
public partial class ScrollbarBehavior {

	/// <summary>
	/// Gets or sets a value indicating whether the auto-hiding scrollbar behavior is disabled.
	/// </summary>
	/// <value>
	/// <see langword="true"/> if the behavior is disabled; otherwise, <see langword="false"/>.
	/// Default is <see langword="false"/>.
	/// </value>
	[Parameter]
	public bool Disabled { get; set; }

	/// <summary>
	/// Gets or sets the CSS selector for the target element that will receive the auto-hiding scrollbar behavior.
	/// </summary>
	/// <value>
	/// A valid CSS selector string (e.g., "#myElement", ".my-class", "[data-target]").
	/// This parameter is required.
	/// </value>
	[Parameter, EditorRequired]
	public string Target { get; set; } = default!;

	/// <summary>
	/// Gets or sets the delay in milliseconds before showing the scrollbars after mouse enter.
	/// </summary>
	/// <value>
	/// The delay in milliseconds. Default is 500ms.
	/// </value>
	/// <remarks>
	/// <para>
	/// The value is automatically clamped between 100ms and 3000ms.
	/// </para>
	/// <list type="bullet">
	/// <item><description>Values less than 100ms are set to 100ms</description></item>
	/// <item><description>Values greater than 3000ms are set to 3000ms</description></item>
	/// </list>
	/// </remarks>
	[Parameter]
	public int ShowDelay { get; set; } = 500;

	/// <summary>
	/// Gets or sets the delay in milliseconds before hiding the scrollbars after mouse leave.
	/// </summary>
	/// <value>
	/// The delay in milliseconds. Default is 100ms.
	/// </value>
	/// <remarks>
	/// <para>
	/// The value is automatically clamped between 10ms and 3000ms.
	/// </para>
	/// <list type="bullet">
	/// <item><description>Values less than 10ms are set to 10ms</description></item>
	/// <item><description>Values greater than 3000ms are set to 3000ms</description></item>
	/// </list>
	/// </remarks>
	[Parameter]
	public int HideDelay { get; set; } = 100;

	/// <summary>
	/// Gets or sets whether the data-scrollbar attribute should be preserved when the target changes.
	/// The attribute is always removed during component disposal.
	/// </summary>
	/// <value>
	/// <see langword="true"/> to preserve the attribute during target changes; otherwise, <see langword="false"/>.
	/// Default is <see langword="false"/>.
	/// </value>
	[Parameter]
	public bool PreserveDataAttributeOnTargetChange { get; set; } = false;



	/// <summary>
	/// The JavaScript module reference for interop operations.
	/// </summary>
	private IJSInProcessObjectReference? module;

	/// <summary>
	/// JavaScript function name for adding scrollbar behavior.
	/// </summary>
	private const string ADD = "addScrollbarBehavior";

	/// <summary>
	/// Adds the scrollbar behavior to the target element if not disabled and target is valid.
	/// </summary>
	private void AddScrollbarBehavior() {
		if (this.Disabled || string.IsNullOrWhiteSpace(this.Target)) {
			return;
		}
		this.module?.InvokeVoid(ADD, this.Target, this.ShowDelay, this.HideDelay);
	}

	/// <summary>
	/// JavaScript function name for removing scrollbar behavior.
	/// </summary>
	private const string REMOVE = "removeScrollbarBehavior";

	/// <summary>
	/// Removes the scrollbar behavior from the target element if target is valid.
	/// </summary>
	private void RemoveScrollbarBehavior(bool preserveDataAttribute = false) {
		if (string.IsNullOrWhiteSpace(this.Target)) {
			return;
		}
		this.module?.InvokeVoid(REMOVE, this.Target, preserveDataAttribute);
	}


	/// <summary>
	/// Minimum allowed value for ShowDelay.
	/// </summary>
	private const int MIN_SHOW_DELAY = 100;

	/// <summary>
	/// Maximum allowed value for ShowDelay.
	/// </summary>
	private const int MAX_SHOW_DELAY = 3000;

	/// <summary>
	/// Ensures the ShowDelay value is within the allowed range.
	/// </summary>
	private void EnsureShowDelay() {
		if (this.ShowDelay < MIN_SHOW_DELAY) {
			this.ShowDelay = MIN_SHOW_DELAY;
		}
		if (this.ShowDelay > MAX_SHOW_DELAY) {
			this.ShowDelay = MAX_SHOW_DELAY;
		}
	}

	/// <summary>
	/// Minimum allowed value for HideDelay.
	/// </summary>
	private const int MIN_HIDE_DELAY = 10;

	/// <summary>
	/// Maximum allowed value for HideDelay.
	/// </summary>
	private const int MAX_HIDE_DELAY = 3000;

	/// <summary>
	/// Ensures the HideDelay value is within the allowed range.
	/// </summary>
	private void EnsureHideDelay() {
		if (this.HideDelay < MIN_HIDE_DELAY) {
			this.HideDelay = MIN_HIDE_DELAY;
		}
		if (this.HideDelay > MAX_HIDE_DELAY) {
			this.HideDelay = MAX_HIDE_DELAY;
		}
	}

	/// <summary>
	/// Tracks whether the component should render.
	/// </summary>
	private bool shouldRender = true;

	/// <summary>
	/// Determines whether the component should render.
	/// </summary>
	/// <returns>
	/// <see langword="true"/> if the component should render; otherwise, <see langword="false"/>.
	/// </returns>
	/// <remarks>
	/// Implements a one-time render pattern where rendering is disabled after the first render
	/// unless explicitly re-enabled by parameter changes.
	/// </remarks>
	protected override bool ShouldRender() {
		if (this.shouldRender) {
			this.shouldRender = false;
			return true;
		}
		return false;
	}

	/// <summary>
	/// Sets the component parameters and handles parameter changes.
	/// </summary>
	/// <param name="parameters">The parameters to set.</param>
	/// <returns>A task that represents the asynchronous operation.</returns>
	/// <remarks>
	/// This method detects parameter changes and removes/re-adds the scrollbar behavior when necessary.
	/// It also ensures that delay values are within acceptable ranges.
	/// </remarks>
	public override async Task SetParametersAsync(ParameterView parameters) {

		if (this.Rendered) {

			var removeTarget = false;

			if (parameters.TryGetValue(nameof(this.Disabled), out bool newDisabled)) {
				if (this.Disabled != newDisabled) {
					removeTarget = true;
				}
			}

			if (parameters.TryGetValue(nameof(this.Target), out string? newTarget)) {
				if (this.Target != newTarget) {
					removeTarget = true;
				}
			}

			if (parameters.TryGetValue(nameof(this.ShowDelay), out int? newShowDelay)) {
				if (this.ShowDelay != newShowDelay) {
					removeTarget = true;
				}
			}

			if (parameters.TryGetValue(nameof(this.HideDelay), out int? newHideDelay)) {
				if (this.HideDelay != newHideDelay) { // Fixed: was comparing ShowDelay
					removeTarget = true;
				}
			}

			if (removeTarget) {
				this.RemoveScrollbarBehavior(this.PreserveDataAttributeOnTargetChange);
				this.shouldRender = true;
			}

			if (this.shouldRender) {
				this.Disabled = newDisabled;
				this.Target = newTarget ?? this.Target;
				this.ShowDelay = newShowDelay ?? this.ShowDelay;
				this.EnsureShowDelay();
				this.HideDelay = newHideDelay ?? this.HideDelay;
				this.EnsureHideDelay();
				if (!this.Disabled && !string.IsNullOrWhiteSpace(this.Target)) {
					this.RunAfterRender(this.AddScrollbarBehavior);
				}
			}

			await base.SetParametersAsync(ParameterView.Empty);

		} else {
			await base.SetParametersAsync(parameters);
		}

	}

	/// <summary>
	/// Called after the component is rendered for the first time.
	/// </summary>
	/// <returns>A task that represents the asynchronous operation.</returns>
	/// <remarks>
	/// Loads the JavaScript module and initializes the scrollbar behavior.
	/// </remarks>
	protected override async Task OnAfterFirstRenderAsync() {
		const string jsPath = $"./_content/Cirreum.Blazor.Components/Components/{nameof(ScrollbarBehavior)}/{nameof(ScrollbarBehavior)}.razor.js";
		this.module = await this.JS.InvokeAsync<IJSInProcessObjectReference>("import", jsPath);
		this.EnsureShowDelay();
		this.EnsureHideDelay();
		this.AddScrollbarBehavior();
	}

	/// <summary>
	/// Releases the unmanaged resources used by the component and optionally releases the managed resources.
	/// </summary>
	/// <param name="disposing">
	/// <see langword="true"/> to release both managed and unmanaged resources; 
	/// <see langword="false"/> to release only unmanaged resources.
	/// </param>
	/// <remarks>
	/// Removes the scrollbar behavior and disposes of the JavaScript module reference.
	/// Handles <see cref="JSDisconnectedException"/> gracefully during disposal.
	/// </remarks>
	protected override void Dispose(bool disposing) {
		try {
			this.RemoveScrollbarBehavior(false);
		} catch (JSDisconnectedException) { }
		this.module?.Dispose();
	}

}