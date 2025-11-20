namespace Cirreum.Components;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Represents the visual style types for toast notifications.
/// </summary>
public enum ToastStyleType {
	/// <summary>
	/// Default toast style.
	/// </summary>
	[Display(Name = "")]
	Default,

	/// <summary>
	/// Primary toast style.
	/// </summary>
	[Display(Name = "primary")]
	Primary,

	/// <summary>
	/// Secondary toast style.
	/// </summary>
	[Display(Name = "secondary")]
	Secondary,

	/// <summary>
	/// Danger toast style, typically used for error messages.
	/// </summary>
	[Display(Name = "danger")]
	Danger,

	/// <summary>
	/// Warning toast style, used for alerting users to potential issues.
	/// </summary>
	[Display(Name = "warning")]
	Warning,

	/// <summary>
	/// Info toast style, used for general information messages.
	/// </summary>
	[Display(Name = "info")]
	Info,

	/// <summary>
	/// Success toast style, typically used for confirmation messages.
	/// </summary>
	[Display(Name = "success")]
	Success,

	/// <summary>
	/// Dark toast style.
	/// </summary>
	[Display(Name = "dark")]
	Dark,

	/// <summary>
	/// Light toast style.
	/// </summary>
	[Display(Name = "light")]
	Light
}