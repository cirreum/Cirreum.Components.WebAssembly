namespace Cirreum.Components;

using Cirreum.Startup;
using Microsoft.JSInterop;
using System.Diagnostics.CodeAnalysis;

public class ClickDetectorService(
	IJSRuntime JS)
	: IClickDetectorService,
		IAutoInitialize,
		IDisposable {

	const string MODULE_PATH = "./_content/Cirreum.Blazor.Components/js/clickDetector.js";
	const string REGISTER_METHOD = "registerElement";
	const string UNREGISTER_METHOD = "unregisterElement";
	const string OUTSIDECLICK_METHOD = "outsideClick";
	const string INSIDECLICK_METHOD = "insideClick";

	private bool isDisposed;
	private IJSInProcessObjectReference? module;
	private readonly Dictionary<string, DotNetObjectReference<TargetElementCallback>> references = [];
	private bool _initialized;

	public void Dispose() {
		if (this.isDisposed is false) {
			this.isDisposed = true;
			if (this.module is not null) {
				foreach (var r in references) {
					try {
						this.module.InvokeVoid(UNREGISTER_METHOD, r.Key);
						r.Value.Dispose();
					} catch (JSDisconnectedException) { }
				}
				try {
					this.module.Dispose();
				} catch (JSDisconnectedException) { }
			}
		}
	}

	public async ValueTask InitializeAsync() {
		if (this._initialized) {
			return; // Prevent reinitialization
		}
		this._initialized = true;
		ObjectDisposedException.ThrowIf(this.isDisposed, this);
		this.module ??= await JS.InvokeAsync<IJSInProcessObjectReference>("import", MODULE_PATH);
	}
	void ThrowIfNotInitialized() {
		if (this.module is null) {
			throw new InvalidOperationException("The ClickDetectorService has not been initialized.");
		}
	}

	public void Register(string elementId, Func<string, Task>? outsideClick, Func<string, Task>? insideClick = null, string[]? ignoreList = null) {
		ObjectDisposedException.ThrowIf(this.isDisposed, this);
		this.ThrowIfNotInitialized();
		if (outsideClick is null && insideClick is null) {
			throw new InvalidOperationException("You must provide at least one callback handler.");
		}
		if (references.ContainsKey(elementId)) {
			throw new InvalidOperationException("ElementId already registered.");
		}

		var targetElementCallbacks = new TargetElementCallback(outsideClick, insideClick);
		var callbackRef = DotNetObjectReference.Create(targetElementCallbacks);

		references.Add(elementId, callbackRef);

		this.module!.InvokeVoid(
			REGISTER_METHOD,
			elementId,
			callbackRef,
			ignoreList ?? []);

	}
	public void Unregister(string elementId) {
		ObjectDisposedException.ThrowIf(this.isDisposed, this);
		this.ThrowIfNotInitialized();
		if (references.TryGetValue(elementId, out var dotnetRef)) {
			references.Remove(elementId);
			this.module!.InvokeVoid(UNREGISTER_METHOD, elementId);
			dotnetRef.Dispose();
		}
	}

	internal record TargetElementCallback {

		public Func<string, Task>? OnOutsideClick { get; }
		public Func<string, Task>? OnInsideClick { get; }

		[DynamicDependency(nameof(ClickOutside))]
		[DynamicDependency(nameof(ClickInside))]
		public TargetElementCallback(Func<string, Task>? OnOutsideClick, Func<string, Task>? OnInsideClick) {
			this.OnOutsideClick = OnOutsideClick;
			this.OnInsideClick = OnInsideClick;
		}

		[JSInvokable(OUTSIDECLICK_METHOD)]
		public async Task ClickOutside(string id) {
			if (this.OnOutsideClick is not null) {
				await this.OnOutsideClick.Invoke(id);
			}
		}

		[JSInvokable(INSIDECLICK_METHOD)]
		public async Task ClickInside(string id) {
			if (this.OnInsideClick is not null) {
				await this.OnInsideClick.Invoke(id);
			}
		}

	}

}