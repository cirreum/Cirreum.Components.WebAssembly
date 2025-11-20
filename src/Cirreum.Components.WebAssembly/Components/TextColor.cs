namespace Cirreum.Components;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Exposes Bootstrap text color classes, including emphasis and body variants. 
/// The Name property contains the full class name (e.g., 'text-primary'),
/// and the ShortName property contains the suffix with hyphen (e.g., '-primary').
/// </summary>
public enum TextColor {
	/// <summary>
	/// Represents no specific text color.
	/// Name: "", ShortName: ""
	/// </summary>
	[Display(Name = "", ShortName = "")]
	None,

	/// <summary>
	/// Represents the primary text color.
	/// Name: "text-primary", ShortName: "-primary"
	/// </summary>
	[Display(Name = "text-primary", ShortName = "-primary")]
	Primary,

	/// <summary>
	/// Represents the primary emphasis text color.
	/// Name: "text-primary-emphasis", ShortName: "-primary-emphasis"
	/// </summary>
	[Display(Name = "text-primary-emphasis", ShortName = "-primary-emphasis")]
	PrimaryEmphasis,

	/// <summary>
	/// Represents the secondary text color.
	/// Name: "text-secondary", ShortName: "-secondary"
	/// </summary>
	[Display(Name = "text-secondary", ShortName = "-secondary")]
	Secondary,

	/// <summary>
	/// Represents the secondary emphasis text color.
	/// Name: "text-secondary-emphasis", ShortName: "-secondary-emphasis"
	/// </summary>
	[Display(Name = "text-secondary-emphasis", ShortName = "-secondary-emphasis")]
	SecondaryEmphasis,

	/// <summary>
	/// Represents the success text color.
	/// Name: "text-success", ShortName: "-success"
	/// </summary>
	[Display(Name = "text-success", ShortName = "-success")]
	Success,

	/// <summary>
	/// Represents the success emphasis text color.
	/// Name: "text-success-emphasis", ShortName: "-success-emphasis"
	/// </summary>
	[Display(Name = "text-success-emphasis", ShortName = "-success-emphasis")]
	SuccessEmphasis,

	/// <summary>
	/// Represents the danger text color.
	/// Name: "text-danger", ShortName: "-danger"
	/// </summary>
	[Display(Name = "text-danger", ShortName = "-danger")]
	Danger,

	/// <summary>
	/// Represents the danger emphasis text color.
	/// Name: "text-danger-emphasis", ShortName: "-danger-emphasis"
	/// </summary>
	[Display(Name = "text-danger-emphasis", ShortName = "-danger-emphasis")]
	DangerEmphasis,

	/// <summary>
	/// Represents the warning text color.
	/// Name: "text-warning", ShortName: "-warning"
	/// </summary>
	[Display(Name = "text-warning", ShortName = "-warning")]
	Warning,

	/// <summary>
	/// Represents the warning emphasis text color.
	/// Name: "text-warning-emphasis", ShortName: "-warning-emphasis"
	/// </summary>
	[Display(Name = "text-warning-emphasis", ShortName = "-warning-emphasis")]
	WarningEmphasis,

	/// <summary>
	/// Represents the info text color.
	/// Name: "text-info", ShortName: "-info"
	/// </summary>
	[Display(Name = "text-info", ShortName = "-info")]
	Info,

	/// <summary>
	/// Represents the info emphasis text color.
	/// Name: "text-info-emphasis", ShortName: "-info-emphasis"
	/// </summary>
	[Display(Name = "text-info-emphasis", ShortName = "-info-emphasis")]
	InfoEmphasis,

	/// <summary>
	/// Represents the light text color.
	/// Name: "text-light", ShortName: "-light"
	/// </summary>
	[Display(Name = "text-light", ShortName = "-light")]
	Light,

	/// <summary>
	/// Represents the light emphasis text color.
	/// Name: "text-light-emphasis", ShortName: "-light-emphasis"
	/// </summary>
	[Display(Name = "text-light-emphasis", ShortName = "-light-emphasis")]
	LightEmphasis,

	/// <summary>
	/// Represents the dark text color.
	/// Name: "text-dark", ShortName: "-dark"
	/// </summary>
	[Display(Name = "text-dark", ShortName = "-dark")]
	Dark,

	/// <summary>
	/// Represents the dark emphasis text color.
	/// Name: "text-dark-emphasis", ShortName: "-dark-emphasis"
	/// </summary>
	[Display(Name = "text-dark-emphasis", ShortName = "-dark-emphasis")]
	DarkEmphasis,

	/// <summary>
	/// Represents the body text color.
	/// Name: "text-body", ShortName: "-body"
	/// </summary>
	[Display(Name = "text-body", ShortName = "-body")]
	Body,

	/// <summary>
	/// Represents the body emphasis text color.
	/// Name: "text-body-emphasis", ShortName: "-body-emphasis"
	/// </summary>
	[Display(Name = "text-body-emphasis", ShortName = "-body-emphasis")]
	BodyEmphasis,

	/// <summary>
	/// Represents the body secondary text color.
	/// Name: "text-body-secondary", ShortName = "-body-secondary"
	/// </summary>
	[Display(Name = "text-body-secondary", ShortName = "-body-secondary")]
	BodySecondary,

	/// <summary>
	/// Represents the body tertiary text color.
	/// Name: "text-body-tertiary", ShortName: "-body-tertiary"
	/// </summary>
	[Display(Name = "text-body-tertiary", ShortName = "-body-tertiary")]
	BodyTertiary,

	/// <summary>
	/// Represents the white text color.
	/// Name: "text-white", ShortName: "-white"
	/// </summary>
	[Display(Name = "text-white", ShortName = "-white")]
	White,

	/// <summary>
	/// Represents the black text color.
	/// Name: "text-black", ShortName: "-black"
	/// </summary>
	[Display(Name = "text-black", ShortName = "-black")]
	Black,

	/// <summary>
	/// Represents the black text color with 50% opacity.
	/// Name: "text-black-50", ShortName: "-black-50"
	/// </summary>
	[Display(Name = "text-black-50", ShortName = "-black-50")]
	BlackHalfOpacity,

	/// <summary>
	/// Represents the white text color with 50% opacity.
	/// Name: "text-white-50", ShortName: "-white-50"
	/// </summary>
	[Display(Name = "text-white-50", ShortName = "-white-50")]
	WhiteHalfOpacity
}