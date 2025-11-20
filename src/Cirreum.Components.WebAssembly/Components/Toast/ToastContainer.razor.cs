namespace Cirreum.Components;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// Main component for hosting toast elements.
/// </summary>
public partial class ToastContainer : IDisposable {

	private readonly ILogger<ToastContainer> _logger;
	private readonly IToastService _toastService;
	private readonly RenderFragment _renderToasts;
	private readonly RenderFragment<IGrouping<string, ToastInstance>> _renderBottomUpToasts;
	private readonly RenderFragment<IGrouping<string, ToastInstance>> _renderTopDownToasts;
	private readonly NavigationManager _navManager;
	private readonly List<ToastElement> _toastElements = [];
	private readonly Dictionary<string, List<ToastInstance>> _toastDictionary = [];
	private Queue<PendingToast> _pendingToasts = [];

	private int TotalToastCount => this._toastDictionary.Values.Sum(list => list.Count);

	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="logger">The logger instance</param>
	/// <param name="toastService">The <see cref="IToastService"/></param>
	/// <param name="navManager">The <see cref="NavigationManager"/></param>
	public ToastContainer(
		ILogger<ToastContainer> logger,
		IToastService toastService,
		NavigationManager navManager) {

		this._logger = logger;
		this._toastService = toastService;
		this._navManager = navManager;

		this._renderToasts = this.RenderToasts;
		this._renderBottomUpToasts = this.RenderToastBottomUp;
		this._renderTopDownToasts = this.RenderToastTopDown;
	}

	#region Parameters Section

	/// <summary>
	/// Set the default <see cref="ToastPosition"/>. Default: <see cref="ToastPosition.TopRight"/>
	/// </summary>
	[Parameter] public ToastPosition Position { get; set; } = ToastPosition.TopRight;

	/// <summary>
	/// Set the default <see cref="ToastType"/>. Default: <see cref="ToastStyleType.Default"/>
	/// </summary>
	[Parameter] public ToastStyleType ToastType { get; set; } = ToastStyleType.Default;

	/// <summary>
	/// How long the toast remains visible without user interaction.
	/// </summary>
	[Parameter] public TimeoutDuration Timeout { get; set; } = TimeoutDuration.Default;

	/// <summary>
	/// Sets the default value for if the timeout should be disabled (show until manually closed). Default: false
	/// </summary>
	[Parameter] public bool DisableTimeout { get; set; }

	/// <summary>
	/// Sets the default number of displayable toasts before they're queued for render. Default: 7
	/// </summary>
	[Parameter] public int MaxToastCount { get; set; } = 7;

	/// <summary>
	/// Sets if visible toasts should automatically be removed upon application
	/// navigation. Default: true
	/// </summary>
	[Parameter] public bool RemoveToastsOnNavigation { get; set; } = true;

	/// <summary>
	/// Sets the default value for if the progressbard (count down bar) should be visible. Default: false
	/// </summary>
	[Parameter] public bool ShowProgressBar { get; set; }

	/// <summary>
	/// Sets the default value for is the Close button should be displaed. Default: true
	/// </summary>
	[Parameter] public bool ShowCloseButton { get; set; } = true;

	/// <summary>
	/// Allows the user to copy the message contents to the Clipboard. Defaults to true.
	/// </summary>
	[Parameter] public bool ShowCopyToClipboard { get; set; } = true;

	/// <summary>
	/// Sets the default value for if the progress (countdown timer) should pause when the toast is hovered. Default: false
	/// </summary>
	[Parameter] public bool PauseProgressOnHover { get; set; } = false;

	#endregion

	private ToastOptions ConfigureOptions(Action<ToastOptions>? configure) {
		// Defaults
		var toastInstanceOptions = new ToastOptions() {
			Position = this.Position,
			Timeout = this.Timeout,
			ShowProgressBar = this.ShowProgressBar,
			ShowCloseButton = this.ShowCloseButton,
			ShowCopyToClipboard = this.ShowCopyToClipboard,
			DisableTimeout = this.DisableTimeout,
			PauseProgressOnHover = this.PauseProgressOnHover,
			StyleType = this.ToastType,
			CloseButtonCss = ToastStyles.Classes.CloseButtonCss,
			ToastMessageCss = ToastStyles.Classes.MessageCss,
		};

		// Instance configuration
		configure?.Invoke(toastInstanceOptions);

		// Deep Clone
		return new ToastOptions(
			toastInstanceOptions.ToastId,
			toastInstanceOptions.ShowProgressBar ?? this.ShowProgressBar,
			toastInstanceOptions.ShowCloseButton ?? this.ShowCloseButton,
			toastInstanceOptions.ShowCopyToClipboard ?? this.ShowCopyToClipboard,
			toastInstanceOptions.DisableTimeout ?? this.DisableTimeout,
			toastInstanceOptions.PauseProgressOnHover ?? this.PauseProgressOnHover,
			toastInstanceOptions.Timeout == 0 ? this.Timeout : toastInstanceOptions.Timeout,
			toastInstanceOptions.StyleType,
			toastInstanceOptions.OnClick,
			toastInstanceOptions.OnClosed,
			toastInstanceOptions.AdditionalClasses,
			toastInstanceOptions.ToastMessageCss,
			toastInstanceOptions.CloseButtonCss,
			toastInstanceOptions.Position);
	}

	private IEnumerable<IGrouping<string, ToastInstance>> GetGroups() =>
		this._toastDictionary
			.SelectMany(kvp => kvp.Value)
			.GroupBy(x => x.Options.PositionClass);

	private void ShowNextAvailableToast() {
		if (this._pendingToasts.Count > 0 && this.TotalToastCount < this.MaxToastCount) {
			var pendingToast = this._pendingToasts.Dequeue();
			if (!this._toastDictionary.TryGetValue(pendingToast.Uri, out var value)) {
				value = [];
				this._toastDictionary[pendingToast.Uri] = value;
			}
			value.Add(pendingToast.Toast);
			this.StateHasChanged();
		}
	}

	internal void AssociateElement(ToastElement toastElement) {
		this._toastElements.Add(toastElement);
	}

	internal void ShowToast(RenderFragment messageFragment, string title, string? subtitle, Action<ToastOptions>? configure) {
		var options = this.ConfigureOptions(configure);
		var toast = new ToastInstance(messageFragment, title, subtitle, options);
		var currentUri = this._navManager.Uri;

		if (this.TotalToastCount < this.MaxToastCount) {
			if (!this._toastDictionary.TryGetValue(currentUri, out var value)) {
				value = [];
				this._toastDictionary[currentUri] = value;
			}
			value.Add(toast);
			this.StateHasChanged();
		} else {
			this._pendingToasts.Enqueue(new PendingToast { Uri = currentUri, Toast = toast });
		}
	}

	internal void ShowToast(Type messageComponentType, string title, string? subtitle, Action<ToastOptions>? configure, ComponentParameters? parameters) {
		var options = this.ConfigureOptions(configure);
		var toastInstance = new ToastInstance(messageComponentType, title, subtitle, options, parameters);
		var currentUri = this._navManager.Uri;

		if (this.TotalToastCount < this.MaxToastCount) {
			if (!this._toastDictionary.TryGetValue(currentUri, out var value)) {
				value = [];
				this._toastDictionary[currentUri] = value;
			}
			value.Add(toastInstance);
			this.StateHasChanged();
		} else {
			this._pendingToasts.Enqueue(new PendingToast { Uri = currentUri, Toast = toastInstance });
		}
	}

	internal void UpdateToast(Guid toastId) {
		// In WASM, we're already on the UI thread, so no need for InvokeAsync
		var toastElement = this._toastElements.Find(t => t.ToastId == toastId);
		toastElement?.Update();
	}

	internal void RemoveToast(Guid toastId) {
		// First find and remove from ToastElements
		var toastElement = this._toastElements.Find(t => t.ToastId == toastId);
		if (toastElement is not null) {
			this._toastElements.Remove(toastElement);
		}

		// Find and extract the toast from the dictionary
		ToastInstance? removedToast = null;
		string? uriToCheck = null;

		foreach (var uri in this._toastDictionary.Keys.ToList()) {
			var toasts = this._toastDictionary[uri];
			var toastToRemove = toasts.FirstOrDefault(t => t.Id == toastId);
			if (toastToRemove != null) {
				removedToast = toastToRemove;
				toasts.Remove(toastToRemove);
				uriToCheck = uri;
				break;
			}
		}

		// If we found a URI with now-empty toast list, remove it
		if (uriToCheck != null && this._toastDictionary[uriToCheck].Count == 0) {
			this._toastDictionary.Remove(uriToCheck);
		}

		// If we removed a toast, update UI and trigger callback
		if (removedToast != null) {
			try {
				// Trigger the callback
				removedToast.Options.OnClosed?.Invoke(removedToast);

				// Update UI and check for pending toasts
				this.StateHasChanged();
				this.ShowNextAvailableToast();
			} catch (Exception ex) {
				this._logger.LogError(ex, "Exception encountered calling onClosed callback");
			}
		}
	}

	internal void Clear() {
		var idsToRemove = new List<Guid>();

		foreach (var (key, list) in this._toastDictionary) {
			foreach (var toast in list) {
				idsToRemove.Add(toast.Id);
			}
		}

		idsToRemove.ForEach(this.RemoveToast);
	}

	internal void ClearQueue() {
		this._pendingToasts.Clear();
	}

	internal void ClearAll() {
		this._pendingToasts.Clear();
		var idsToRemove = new List<Guid>();
		foreach (var (key, list) in this._toastDictionary) {
			foreach (var toast in list) {
				idsToRemove.Add(toast.Id);
			}
		}
		idsToRemove.ForEach(this.RemoveToast);
	}

	private void OnLocationChanged(object? sender, LocationChangedEventArgs e) {
		if (!this.RemoveToastsOnNavigation) {
			return;
		}

		var currentUri = this._navManager.Uri;

		// Collect toasts to remove
		var toastsToRemove = new List<Guid>();

		// Check each URI except the current one
		foreach (var uri in this._toastDictionary.Keys.ToList()) {
			if (uri != currentUri) {
				// For each toast in this URI, add its ID to the removal list
				foreach (var toast in this._toastDictionary[uri]) {
					toastsToRemove.Add(toast.Id);
				}
			}
		}

		// Remove each toast properly
		foreach (var toastId in toastsToRemove) {
			this.RemoveToast(toastId);
		}

		// Filter pending toasts
		var newPendingToasts = new Queue<PendingToast>();
		while (this._pendingToasts.Count > 0) {
			var pendingToast = this._pendingToasts.Dequeue();
			if (pendingToast.Uri == currentUri) {
				newPendingToasts.Enqueue(pendingToast);
			}
		}
		this._pendingToasts = newPendingToasts;
	}

	public override async Task SetParametersAsync(ParameterView parameters) {
		if (parameters.TryGetValue<int>(nameof(this.MaxToastCount), out var value) && value < 1) {
			throw new InvalidOperationException($"{nameof(this.MaxToastCount)} cannot be less than 1");
		}

		await base.SetParametersAsync(parameters);
	}

	protected override void OnInitialized() {
		this._toastService.AssociateContainer(this);
		this._navManager.LocationChanged += this.OnLocationChanged;
	}

	void IDisposable.Dispose() {
		this._navManager.LocationChanged -= this.OnLocationChanged;
	}

}