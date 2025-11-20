namespace Cirreum.Components;

using Microsoft.AspNetCore.Components;
using System;

/// <summary>
/// Toast Service
/// </summary>
public interface IToastService {

	/// <summary>
	/// Associate the <see cref="ToastContainer"/> with the service.
	/// </summary>
	/// <param name="container">The <see cref="ToastContainer"/> for the service.</param>
	/// <remarks>
	/// <para>
	/// This for use with the system and not intended for direct use.
	/// </para>
	/// </remarks>
	void AssociateContainer(ToastContainer container);

	/// <summary>
	/// Displays a Primary colored toast.
	/// </summary>
	/// <param name="message">The toast main message</param>
	/// <param name="title">The optional toast tile</param>
	/// <param name="subtitle">The optional toast subtitle</param>
	void ShowPrimary(string message, string title, string? subtitle = null);

	/// <summary>
	/// Displays a Secondary colored toast.
	/// </summary>
	/// <param name="message">The toast main message</param>
	/// <param name="title">The optional toast tile</param>
	/// <param name="subtitle">The optional toast subtitle</param>
	void ShowSecondary(string message, string title, string? subtitle = null);

	/// <summary>
	/// Displays a Danger colored toast.
	/// </summary>
	/// <param name="message">The toast main message</param>
	/// <param name="title">The optional toast tile</param>
	/// <param name="subtitle">The optional toast subtitle</param>
	void ShowDanger(string message, string title, string? subtitle = null);

	/// <summary>
	/// Displays a Warning colored toast.
	/// </summary>
	/// <param name="message">The toast main message</param>
	/// <param name="title">The optional toast tile</param>
	/// <param name="subtitle">The optional toast subtitle</param>
	void ShowWarning(string message, string title, string? subtitle = null);

	/// <summary>
	/// Displays a Info colored toast.
	/// </summary>
	/// <param name="message">The toast main message</param>
	/// <param name="title">The optional toast tile</param>
	/// <param name="subtitle">The optional toast subtitle</param>
	void ShowInfo(string message, string title, string? subtitle = null);

	/// <summary>
	/// Displays a Success colored toast.
	/// </summary>
	/// <param name="message">The toast main message</param>
	/// <param name="title">The optional toast tile</param>
	/// <param name="subtitle">The optional toast subtitle</param>
	void ShowSuccess(string message, string title, string? subtitle = null);

	/// <summary>
	/// Displays a Dark colored toast.
	/// </summary>
	/// <param name="message">The toast main message</param>
	/// <param name="title">The optional toast tile</param>
	/// <param name="subtitle">The optional toast subtitle</param>
	void ShowDark(string message, string title, string? subtitle = null);

	/// <summary>
	/// Displays a Light colored toast.
	/// </summary>
	/// <param name="message">The toast main message</param>
	/// <param name="title">The optional toast tile</param>
	/// <param name="subtitle">The optional toast subtitle</param>
	void ShowLight(string message, string title, string? subtitle = null);


	/// <summary>
	/// Displays a Primary colored toast.
	/// </summary>
	/// <param name="message">The toast main message</param>
	/// <param name="title">The optional toast tile</param>
	/// <param name="subtitle">The optional toast subtitle</param>
	/// <param name="configure">An action for configuring a <see cref="ToastOptions"/> instance already containing the globally configured settings</param>
	void ShowPrimary(string message, string title, string? subtitle = null, Action<ToastOptions>? configure = null);

	/// <summary>
	/// Displays a Secondary colored toast.
	/// </summary>
	/// <param name="message">The toast main message</param>
	/// <param name="title">The optional toast tile</param>
	/// <param name="subtitle">The optional toast subtitle</param>
	/// <param name="configure">An action for configuring a <see cref="ToastOptions"/> instance already containing the globally configured settings</param>
	void ShowSecondary(string message, string title, string? subtitle = null, Action<ToastOptions>? configure = null);

	/// <summary>
	/// Displays a Danger colored toast.
	/// </summary>
	/// <param name="message">The toast main message</param>
	/// <param name="title">The optional toast tile</param>
	/// <param name="subtitle">The optional toast subtitle</param>
	/// <param name="configure">An action for configuring a <see cref="ToastOptions"/> instance already containing the globally configured settings</param>
	void ShowDanger(string message, string title, string? subtitle = null, Action<ToastOptions>? configure = null);

	/// <summary>
	/// Displays a Warning colored toast.
	/// </summary>
	/// <param name="message">The toast main message</param>
	/// <param name="title">The optional toast tile</param>
	/// <param name="subtitle">The optional toast subtitle</param>
	/// <param name="configure">An action for configuring a <see cref="ToastOptions"/> instance already containing the globally configured settings</param>
	void ShowWarning(string message, string title, string? subtitle = null, Action<ToastOptions>? configure = null);

	/// <summary>
	/// Displays a Info colored toast.
	/// </summary>
	/// <param name="message">The toast main message</param>
	/// <param name="title">The optional toast tile</param>
	/// <param name="subtitle">The optional toast subtitle</param>
	/// <param name="configure">An action for configuring a <see cref="ToastOptions"/> instance already containing the globally configured settings</param>
	void ShowInfo(string message, string title, string? subtitle = null, Action<ToastOptions>? configure = null);

	/// <summary>
	/// Displays a Success colored toast.
	/// </summary>
	/// <param name="message">The toast main message</param>
	/// <param name="title">The optional toast tile</param>
	/// <param name="subtitle">The optional toast subtitle</param>
	/// <param name="configure">An action for configuring a <see cref="ToastOptions"/> instance already containing the globally configured settings</param>
	void ShowSuccess(string message, string title, string? subtitle = null, Action<ToastOptions>? configure = null);

	/// <summary>
	/// Displays a Dark colored toast.
	/// </summary>
	/// <param name="message">The toast main message</param>
	/// <param name="title">The optional toast tile</param>
	/// <param name="subtitle">The optional toast subtitle</param>
	/// <param name="configure">An action for configuring a <see cref="ToastOptions"/> instance already containing the globally configured settings</param>
	void ShowDark(string message, string title, string? subtitle = null, Action<ToastOptions>? configure = null);

	/// <summary>
	/// Displays a Light colored toast.
	/// </summary>
	/// <param name="message">The toast main message</param>
	/// <param name="title">The optional toast tile</param>
	/// <param name="subtitle">The optional toast subtitle</param>
	/// <param name="configure">An action for configuring a <see cref="ToastOptions"/> instance already containing the globally configured settings</param>
	void ShowLight(string message, string title, string? subtitle = null, Action<ToastOptions>? configure = null);


	/// <summary>
	/// Displays a toast.
	/// </summary>
	/// <param name="message">The toast main message</param>
	/// <param name="title">The optional toast tile</param>
	/// <param name="subtitle">The optional toast subtitle</param>
	/// <param name="configure">An action for configuring a <see cref="ToastOptions"/> instance already containing the globally configured settings</param>
	void Show(string message, string title, string? subtitle = null, Action<ToastOptions>? configure = null);

	/// <summary>
	/// Shows a toast using the supplied settings
	/// </summary>
	/// <param name="messageFragment">RenderFragment to display on the toast</param>
	/// <param name="title">The optional toast tile</param>
	/// <param name="subtitle">The optional toast subtitle</param>
	/// <param name="configure">Settings to configure the toast instance</param>
	void Show(RenderFragment messageFragment, string title, string? subtitle = null, Action<ToastOptions>? configure = null);

	/// <summary>
	/// Shows a toast using the supplied settings
	/// </summary>
	/// <typeparam name="TContent">The Type of the component to render as the message body</typeparam>
	/// <param name="title">The optional toast tile</param>
	/// <param name="subtitle">The optional toast subtitle</param>
	/// <param name="configure">Settings to configure the toast instance</param>
	/// <param name="parameters">The optional parameters for the <typeparamref name="TContent"/> component</param>
	void Show<TContent>(string title, string? subtitle = null, Action<ToastOptions>? configure = null, ComponentParameters? parameters = null) where TContent : class, IComponent;

	/// <summary>
	/// Close the specified toast.
	/// </summary>
	/// <param name="toastId">The Id of the toast to close.</param>
	void Close(Guid toastId);

	/// <summary>
	/// Clear all displayed toasts.
	/// </summary>
	void Clear();

	/// <summary>
	/// Clear all queued toasts
	/// </summary>
	void ClearQueue();

	/// <summary>
	/// Clear all toasts, including toasts in-queue waiting to be displayed.
	/// </summary>
	void ClearAll();

	/// <summary>
	/// Updates the toast element.
	/// </summary>
	/// <param name="toastId">The id of the toast instance.</param>
	void UpdateToast(Guid toastId);

}