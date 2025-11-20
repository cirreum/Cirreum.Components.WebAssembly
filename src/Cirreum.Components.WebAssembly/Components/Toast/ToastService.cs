namespace Cirreum.Components;

using Microsoft.AspNetCore.Components;
using System;

internal class ToastService : IToastService {

	ToastContainer? _container;
	public void AssociateContainer(ToastContainer container) {
		if (this._container != container) {
			this._container = container;
		}
	}

	public void ShowPrimary(string message, string title, string? subtitle = null)
		=> this.ShowToast(ToastStyleType.Primary, message, title, subtitle);

	public void ShowSecondary(string message, string title, string? subtitle = null)
		=> this.ShowToast(ToastStyleType.Secondary, message, title, subtitle);

	public void ShowDanger(string message, string title, string? subtitle = null)
		=> this.ShowToast(ToastStyleType.Danger, message, title, subtitle);

	public void ShowWarning(string message, string title, string? subtitle = null)
		=> this.ShowToast(ToastStyleType.Warning, message, title, subtitle);

	public void ShowInfo(string message, string title, string? subtitle = null)
		=> this.ShowToast(ToastStyleType.Info, message, title, subtitle);

	public void ShowSuccess(string message, string title, string? subtitle = null)
		=> this.ShowToast(ToastStyleType.Success, message, title, subtitle);

	public void ShowDark(string message, string title, string? subtitle = null)
		=> this.ShowToast(ToastStyleType.Dark, message, title, subtitle);

	public void ShowLight(string message, string title, string? subtitle = null)
			=> this.ShowToast(ToastStyleType.Light, message, title, subtitle);


	public void ShowPrimary(string message, string title, string? subtitle = null, Action<ToastOptions>? settings = null)
		=> this.ShowToast(ToastStyleType.Primary, message, title, subtitle, settings);

	public void ShowSecondary(string message, string title, string? subtitle = null, Action<ToastOptions>? settings = null)
		=> this.ShowToast(ToastStyleType.Secondary, message, title, subtitle, settings);

	public void ShowDanger(string message, string title, string? subtitle = null, Action<ToastOptions>? settings = null)
		=> this.ShowToast(ToastStyleType.Danger, message, title, subtitle, settings);

	public void ShowWarning(string message, string title, string? subtitle = null, Action<ToastOptions>? settings = null)
		=> this.ShowToast(ToastStyleType.Warning, message, title, subtitle, settings);

	public void ShowInfo(string message, string title, string? subtitle = null, Action<ToastOptions>? settings = null)
		=> this.ShowToast(ToastStyleType.Info, message, title, subtitle, settings);

	public void ShowSuccess(string message, string title, string? subtitle = null, Action<ToastOptions>? settings = null)
		=> this.ShowToast(ToastStyleType.Success, message, title, subtitle, settings);

	public void ShowDark(string message, string title, string? subtitle = null, Action<ToastOptions>? settings = null)
		=> this.ShowToast(ToastStyleType.Dark, message, title, subtitle, settings);

	public void ShowLight(string message, string title, string? subtitle = null, Action<ToastOptions>? settings = null)
			=> this.ShowToast(ToastStyleType.Light, message, title, subtitle, settings);


	private void ShowToast(ToastStyleType toastType, string message, string title, string? subtitle = null, Action<ToastOptions>? settings = null) {
		this.Show(message, title, subtitle, o => {
			o.StyleType = toastType;
			settings?.Invoke(o);
		});
	}

	public void Show(string message, string title, string? subtitle = null, Action<ToastOptions>? settings = null)
		=> this.Show(builder => builder.AddContent(0, message), title, subtitle, settings);


	public void Show(RenderFragment messageFragment, string title, string? subtitle = null, Action<ToastOptions>? configure = null)
		=> this._container?.ShowToast(messageFragment, title, subtitle, configure);

	public void Show<TContent>(string title, string? subtitle = null, Action<ToastOptions>? configure = null, ComponentParameters? parameters = null) where TContent : class, IComponent
		=> this._container?.ShowToast(typeof(TContent), title, subtitle, configure, parameters);

	public void Close(Guid toastId) {
		this._container?.RemoveToast(toastId);
	}

	public void ClearAll()
		=> this._container?.ClearAll();

	public void Clear()
		=> this._container?.Clear();

	public void ClearQueue()
		=> this._container?.ClearQueue();

	public void UpdateToast(Guid toastId) {
		this._container?.UpdateToast(toastId);
	}

}