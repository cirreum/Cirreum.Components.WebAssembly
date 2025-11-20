namespace Cirreum.Components;

using System;

public class ToastOptions {

	/// <summary>
	/// The callback to be called on user click, supplying the <see cref="ToastInstance"/>
	/// associated with the <see cref="ToastElement"/> that was clicked.
	/// </summary>
	public Action<ToastInstance>? OnClick { get; set; }

	/// <summary>
	/// The callback to be called when the toast is closed, supplying the <see cref="ToastInstance"/>
	/// associated with the <see cref="ToastElement"/> that was closed.
	/// </summary>
	public Action<ToastInstance>? OnClosed { get; set; }

	/// <summary>
	/// Gets or sets the ToastId
	/// </summary>
	public Guid ToastId { get; set; } = Guid.NewGuid();

	/// <summary>
	/// Reference to the owning ToastInstance, set internally to enable OnClick modifications
	/// </summary>
	internal ToastInstance? Instance { get; set; }

	/// <summary>
	/// The <c>AdditionalClasses</c> property used to specify additional Css classes
	/// to be applied to the toast div. 
	/// </summary>
	/// <remarks>
	/// By setting this property, you can customize the appearance of the toast notification
	/// and apply custom styles to it. Note that the value of the <c>AdditionalClasses</c>
	/// property should be a string containing one or more Css class names separated by spaces.
	/// </remarks>
	public string? AdditionalClasses { get; set; }

	/// <summary>
	/// The main style type of Toast (Danger, Success etc.)
	/// </summary>
	public ToastStyleType StyleType { get; set; }
	internal string TitleStyle {
		get {
			if (this.StyleType == ToastStyleType.Default) {
				return string.Empty;
			}
			return $"var(--bs-{this.StyleType.ToName()}-text-emphasis)";
		}
	}
	internal string BorderStyle {
		get {
			if (this.StyleType == ToastStyleType.Default) {
				return "var(--bs-border-color)";
			}
			return $"var(--bs-{this.StyleType.ToName()}-border-subtle)";
		}
	}
	internal string HeaderBackgroundStyle {
		get {
			if (this.StyleType == ToastStyleType.Default) {
				return "var(--bs-border-color)";
			}
			return $"var(--bs-{this.StyleType.ToName()}-bg-subtle)";
		}
	}
	internal string ProgressStyle {
		get {
			if (this.StyleType == ToastStyleType.Default) {
				return "var(--bs-border-color)";
			}
			return $"var(--bs-{this.StyleType.ToName()}-border-subtle)";
		}
	}

	/// <summary>
	/// The css class for the message
	/// </summary>
	/// <remarks>
	/// <para>
	/// Default: <see cref="ToastStyles.Classes.MessageCss"/>
	/// </para>
	/// </remarks>
	public string ToastMessageCss { get; set; }

	/// <summary>
	/// The css class for the close button
	/// </summary>
	/// <remarks>
	/// <para>
	/// Default: <see cref="ToastStyles.Classes.CloseButtonCss"/>
	/// </para>
	/// </remarks>
	public string CloseButtonCss { get; set; }

	/// <summary>
	/// Setting this property will override the global toast position property and
	/// allows you to set a specific position for this toast notification. The position
	/// can be set to one of the predefined values in the <see cref="ToastPosition"/> enumeration.
	/// </summary>
	public ToastPosition Position { get; set; }
	internal string PositionClass => this.Position.ToName();

	/// <summary>
	/// How long the toast remains visible without user interaction.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Default: <see cref="TimeoutDuration.Default"/> (5s)
	/// </para>
	/// <para>
	/// If the value is zero (0), a close button will be shown to allow
	/// the user to dismiss the notification manually.
	/// </para>
	/// </remarks>
	public TimeoutDuration Timeout { get; set; } = TimeoutDuration.Default;

	/// <summary>
	/// Set to <see langword="true"/> to prevent the toast
	/// notification from automatically closing, ignoring the <see cref="Timeout"/> property and
	/// showing a close button to allow the user to dismiss the notification manually.
	/// </summary>
	public bool? DisableTimeout { get; set; }

	/// <summary>
	/// Allows the user to dismiss the toast
	/// </summary>
	/// <remarks>
	/// <para>
	/// Default: <see langword="true"/>
	/// </para>
	/// </remarks>
	public bool? ShowCloseButton { get; set; } = true;

	/// <summary>
	/// Allows the user to copy the message contents to the Clipboard
	/// </summary>
	/// <remarks>
	/// <para>
	/// Default: <see langword="true"/>
	/// </para>
	/// </remarks>
	public bool? ShowCopyToClipboard { get; set; } = true;

	/// <summary>
	/// Provide visual feedback of the remaining time the toast notification will be visible based
	/// on the <see cref="Timeout"/> property. 
	/// </summary>
	/// <remarks>
	/// <para>
	/// Default: <see langword="true"/>
	/// </para>
	/// </remarks>
	public bool? ShowProgressBar { get; set; } = true;

	/// <summary>
	/// Gets or sets the current percent of the progress or sets an initial Progress value (0-100)
	/// </summary>
	public int Progress { get; set; } = 100;

	/// <summary>
	/// When the <see cref="PauseProgressOnHover"/> property is enabled, the timeout period
	/// for the toast notification will be paused when the user hovers the mouse over the toast.
	/// </summary>    
	public bool? PauseProgressOnHover { get; set; } = true;

	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="toastId"></param>
	/// <param name="showProgressBar"></param>
	/// <param name="showCloseButton"></param>
	/// <param name="showCopyToClipboard"></param>
	/// <param name="disableTimeout"></param>
	/// <param name="pauseProgressOnHover"></param>
	/// <param name="timeout"></param>
	/// <param name="toastType"></param>
	/// <param name="onClick"></param>
	/// <param name="onClosed"></param>
	/// <param name="additionalClasses"></param>
	/// <param name="toastMessageClass"></param>
	/// <param name="closeButtonClass"></param>
	/// <param name="toastPosition"></param>
	public ToastOptions(
		Guid toastId,
		bool showProgressBar,
		bool showCloseButton,
		bool showCopyToClipboard,
		bool disableTimeout,
		bool pauseProgressOnHover,
		TimeoutDuration timeout,
		ToastStyleType toastType = ToastStyleType.Default,
		Action<ToastInstance>? onClick = null,
		Action<ToastInstance>? onClosed = null,
		string? additionalClasses = null,
		string? toastMessageClass = null,
		string? closeButtonClass = null,
		ToastPosition? toastPosition = null) {

		this.ToastId = toastId;

		this.StyleType = toastType;
		this.Position = toastPosition ?? ToastStyles.Position;
		this.CloseButtonCss = closeButtonClass ?? ToastStyles.Classes.CloseButtonCss;
		this.ToastMessageCss = toastMessageClass ?? ToastStyles.Classes.MessageCss;
		this.AdditionalClasses = additionalClasses;

		this.ShowProgressBar = showProgressBar;
		this.ShowCloseButton = showCloseButton;
		this.ShowCopyToClipboard = showCopyToClipboard;
		this.Timeout = timeout;
		this.DisableTimeout = disableTimeout;
		this.PauseProgressOnHover = pauseProgressOnHover;
		this.OnClick = onClick;
		this.OnClosed = onClosed;
	}

	/// <summary>
	/// Simple Constructor
	/// </summary>
	/// <param name="showProgressBar"></param>
	/// <param name="showCloseButton"></param>
	/// <param name="disableTimeout"></param>
	public ToastOptions(
		bool showProgressBar = true,
		bool showCloseButton = true,
		bool disableTimeout = false) {

		this.Position = ToastStyles.Position;
		this.CloseButtonCss = ToastStyles.Classes.CloseButtonCss;
		this.ToastMessageCss = ToastStyles.Classes.MessageCss;

		this.ShowProgressBar = showProgressBar;
		this.ShowCloseButton = showCloseButton;
		this.DisableTimeout = disableTimeout;
		this.PauseProgressOnHover = true;

	}

}