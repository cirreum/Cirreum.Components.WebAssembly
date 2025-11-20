namespace Cirreum.Components;

using Microsoft.AspNetCore.Components;

/// <summary>
/// Represents an instance of a toast notification with configurable content and display options.
/// </summary>
public class ToastInstance {

	/// <summary>
	/// Initializes a new instance of the ToastInstance class.
	/// </summary>
	/// <param name="messageFragment">The content to be displayed within the toast notification.</param>
	/// <param name="title">The main heading of the toast notification.</param>
	/// <param name="subtitle">An optional secondary heading for the toast notification.</param>
	/// <param name="toastOptions">Configuration options for controlling the toast's behavior and appearance.</param>
	internal ToastInstance(RenderFragment messageFragment, string title, string? subtitle, ToastOptions toastOptions) {
		this.Message = messageFragment;
		this.Title = title;
		this.Subtitle = subtitle;
		this.Options = toastOptions;
		this.Options.Instance = this;
		this.Id = toastOptions.ToastId;
	}

	/// <summary>
	/// Initializes a new instance of the ToastInstance class.
	/// </summary>
	/// <param name="contentType">The content to be displayed within the toast notification.</param>
	/// <param name="title">The main heading of the toast notification.</param>
	/// <param name="subtitle">An optional secondary heading for the toast notification.</param>
	/// <param name="toastOptions">Configuration options for controlling the toast's behavior and appearance.</param>
	/// <param name="paramaters">The optional <see cref="ComponentParameters"/></param>
	internal ToastInstance(Type contentType, string title, string? subtitle, ToastOptions toastOptions, ComponentParameters? paramaters) {
		this.ContentType = contentType;
		this.Paramaters = paramaters;
		this.Title = title;
		this.Subtitle = subtitle;
		this.Options = toastOptions;
		this.Options.Instance = this;
		this.Id = toastOptions.ToastId;
	}

	/// <summary>
	/// Gets the unique identifier for this toast instance.
	/// </summary>
	public Guid Id { get; init; } = Guid.NewGuid();

	/// <summary>
	/// Gets the creation timestamp of this toast instance.
	/// </summary>
	public DateTime TimeStamp { get; init; } = DateTime.Now;

	/// <summary>
	/// Gets or sets the main heading text of the toast notification.
	/// </summary>
	public string Title { get; set; }

	/// <summary>
	/// Gets or sets the optional secondary heading text of the toast notification.
	/// </summary>
	public string? Subtitle { get; set; }

	/// <summary>
	/// Gets the content to be displayed within the toast notification.
	/// </summary>
	internal RenderFragment? Message { get; }

	/// <summary>
	/// Gets the message content type (aka a component)
	/// </summary>
	internal Type? ContentType { get; }

	/// <summary>
	/// Gets the configuration options that control this toast's behavior and appearance.
	/// </summary>
	internal ToastOptions Options { get; }

	internal ComponentParameters? Paramaters { get; }
	internal Dictionary<string, object> GetParameters() {
		return this.Paramaters?.ToDictionary() ?? [];
	}

}