namespace Cirreum.Components;

using Cirreum.Components.Interop;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

public class MouseButtonListener : IDisposable {

	private readonly IJSAppModule _jsApp;
	private readonly ElementReference _element;
	private readonly string? _elementIdOrSelector;
	private readonly int _delay;
	private readonly MouseButton _triggerButton;
	private readonly DotNetObjectReference<MouseButtonListener> _dotNetRef;
	private readonly Action<MouseButtonEventInfo> _callback;

	public MouseButtonListener(IJSAppModule jsApp, ElementReference element, Action<MouseButtonEventInfo> callback, int delay = 500, MouseButton triggerButton = MouseButton.Left) {
		_jsApp = jsApp;
		_element = element;
		_callback = callback;
		_delay = delay;
		_triggerButton = triggerButton;
		_dotNetRef = DotNetObjectReference.Create(this);
		_jsApp.AddMouseButtonListener(_element, _dotNetRef, _delay, _triggerButton);
	}

	public MouseButtonListener(IJSAppModule jsRuntime, string elementIdOrSelector, Action<MouseButtonEventInfo> callback, int delay = 500, MouseButton triggerButton = MouseButton.Left) {
		_jsApp = jsRuntime;
		_elementIdOrSelector = elementIdOrSelector;
		_callback = callback;
		_delay = delay;
		_triggerButton = triggerButton;
		_dotNetRef = DotNetObjectReference.Create(this);
		_jsApp.AddMouseButtonListener(_elementIdOrSelector, _dotNetRef, _delay, _triggerButton);
	}

	[JSInvokable]
	public void OnEventCallback(MouseButtonEventInfo eventInfo) {
		this._callback.Invoke(eventInfo);
	}

	public void Dispose() {
		if (this._elementIdOrSelector.HasValue()) {
#if DEBUG
			Console.WriteLine($"MouseButtonListener::Dispose::selector[{this._elementIdOrSelector}]");
#endif
			this._jsApp.RemoveMouseButtonListener(this._elementIdOrSelector);
		} else {
#if DEBUG
			Console.WriteLine($"MouseButtonListener::Dispose::element[{this._element.Id}]");
#endif
			this._jsApp.RemoveMouseButtonListener(this._element);
		}
		this._dotNetRef?.Dispose();
	}

}

public static class MouseButtonListenerExtensions {

	/// <summary>
	/// Register a callback for a click/contextmenu with the specified <paramref name="element"/>.
	/// </summary>
	/// <param name="element"></param>
	/// <param name="jsApp">The <see cref="IJSAppModule"/> instance.</param>
	/// <param name="callback">Your event handler.</param>
	/// <param name="delay">Default: 800</param>
	/// <param name="triggerButton">The <see cref="MouseButton"/> to trigger on.</param>
	/// <returns>An <see cref="IDisposable"/> object.</returns>
	public static IDisposable AddMouseButtonListener(
		this ElementReference element,
		IJSAppModule jsApp,
		Action<MouseButtonEventInfo> callback,
		int delay = 800,
		MouseButton triggerButton = MouseButton.Left) {
		return new MouseButtonListener(jsApp, element, callback, delay, triggerButton);
	}

	/// <summary>
	/// Register a callback for a click/contextmenu with the specified <paramref name="elementIdOrSelector"/>.
	/// </summary>
	/// <param name="elementIdOrSelector">The Element ID, or am Element selector (#ID .CLASS)</param>
	/// <param name="jsApp">The <see cref="IJSAppModule"/> instance.</param>
	/// <param name="callback">Your event handler.</param>
	/// <param name="delay">Default: 800</param>
	/// <param name="triggerButton">The <see cref="MouseButton"/> to trigger on.</param>
	/// <returns>An <see cref="IDisposable"/> object.</returns>
	public static IDisposable AddMouseButtonListener(
		this string elementIdOrSelector,
		IJSAppModule jsApp,
		Action<MouseButtonEventInfo> callback,
		int delay = 800,
		MouseButton triggerButton = MouseButton.Left
		) {
		return new MouseButtonListener(jsApp, elementIdOrSelector, callback, delay, triggerButton);
	}

}