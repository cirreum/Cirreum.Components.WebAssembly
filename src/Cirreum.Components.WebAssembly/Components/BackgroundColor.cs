namespace Cirreum.Components;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Exposes Bootstrap background color classes, including emphasis classes. 
/// The Name property contains the full class (e.g., 'bg-primary'), while the
/// ShortName property contains the color suffix (e.g., '-primary').
/// </summary>
public enum BackgroundColor {
	/// <summary>
	/// Represents no background color.
	/// Name: "", ShortName: ""
	/// </summary>
	[Display(Name = "", ShortName = "")]
	None,

	/// <summary>
	/// Represents the primary background color.
	/// Name: "bg-primary", ShortName: "-primary"
	/// </summary>
	[Display(Name = "bg-primary", ShortName = "-primary")]
	Primary,

	/// <summary>
	/// Represents the subtle variant of the primary background color.
	/// Name: "bg-primary-subtle", ShortName: "-primary-subtle"
	/// </summary>
	[Display(Name = "bg-primary-subtle", ShortName = "-primary-subtle")]
	PrimarySubtle,

	/// <summary>
	/// Represents the secondary background color.
	/// Name: "bg-secondary", ShortName: "-secondary"
	/// </summary>
	[Display(Name = "bg-secondary", ShortName = "-secondary")]
	Secondary,

	/// <summary>
	/// Represents the subtle variant of the secondary background color.
	/// Name: "bg-secondary-subtle", ShortName: "-secondary-subtle"
	/// </summary>
	[Display(Name = "bg-secondary-subtle", ShortName = "-secondary-subtle")]
	SecondarySubtle,

	/// <summary>
	/// Represents the success background color.
	/// Name: "bg-success", ShortName: "-success"
	/// </summary>
	[Display(Name = "bg-success", ShortName = "-success")]
	Success,

	/// <summary>
	/// Represents the subtle variant of the success background color.
	/// Name: "bg-success-subtle", ShortName: "-success-subtle"
	/// </summary>
	[Display(Name = "bg-success-subtle", ShortName = "-success-subtle")]
	SuccessSubtle,

	/// <summary>
	/// Represents the danger background color.
	/// Name: "bg-danger", ShortName: "-danger"
	/// </summary>
	[Display(Name = "bg-danger", ShortName = "-danger")]
	Danger,

	/// <summary>
	/// Represents the subtle variant of the danger background color.
	/// Name: "bg-danger-subtle", ShortName: "-danger-subtle"
	/// </summary>
	[Display(Name = "bg-danger-subtle", ShortName = "-danger-subtle")]
	DangerSubtle,

	/// <summary>
	/// Represents the warning background color.
	/// Name: "bg-warning", ShortName: "-warning"
	/// </summary>
	[Display(Name = "bg-warning", ShortName = "-warning")]
	Warning,

	/// <summary>
	/// Represents the subtle variant of the warning background color.
	/// Name: "bg-warning-subtle", ShortName: "-warning-subtle"
	/// </summary>
	[Display(Name = "bg-warning-subtle", ShortName = "-warning-subtle")]
	WarningSubtle,

	/// <summary>
	/// Represents the info background color.
	/// Name: "bg-info", ShortName: "-info"
	/// </summary>
	[Display(Name = "bg-info", ShortName = "-info")]
	Info,

	/// <summary>
	/// Represents the subtle variant of the info background color.
	/// Name: "bg-info-subtle", ShortName: "-info-subtle"
	/// </summary>
	[Display(Name = "bg-info-subtle", ShortName = "-info-subtle")]
	InfoSubtle,

	/// <summary>
	/// Represents the light background color.
	/// Name: "bg-light", ShortName: "-light"
	/// </summary>
	[Display(Name = "bg-light", ShortName = "-light")]
	Light,

	/// <summary>
	/// Represents the subtle variant of the light background color.
	/// Name: "bg-light-subtle", ShortName: "-light-subtle"
	/// </summary>
	[Display(Name = "bg-light-subtle", ShortName = "-light-subtle")]
	LightSubtle,

	/// <summary>
	/// Represents the dark background color.
	/// Name: "bg-dark", ShortName: "-dark"
	/// </summary>
	[Display(Name = "bg-dark", ShortName = "-dark")]
	Dark,

	/// <summary>
	/// Represents the subtle variant of the dark background color.
	/// Name: "bg-dark-subtle", ShortName: "-dark-subtle"
	/// </summary>
	[Display(Name = "bg-dark-subtle", ShortName = "-dark-subtle")]
	DarkSubtle,

	/// <summary>
	/// Represents the body background color.
	/// Name: "bg-body", ShortName: "-body"
	/// </summary>
	[Display(Name = "bg-body", ShortName = "-body")]
	Body,

	/// <summary>
	/// Represents the secondary body background color.
	/// Name: "bg-body-secondary", ShortName: "-body-secondary"
	/// </summary>
	[Display(Name = "bg-body-secondary", ShortName = "-body-secondary")]
	BodySecondary,

	/// <summary>
	/// Represents the tertiary body background color.
	/// Name: "bg-body-tertiary", ShortName: "-body-tertiary"
	/// </summary>
	[Display(Name = "bg-body-tertiary", ShortName = "-body-tertiary")]
	BodyTertiary,

	/// <summary>
	/// Represents the white background color.
	/// Name: "bg-white", ShortName: "-white"
	/// </summary>
	[Display(Name = "bg-white", ShortName = "-white")]
	White,

	/// <summary>
	/// Represents the black background color.
	/// Name: "bg-black", ShortName: "-black"
	/// </summary>
	[Display(Name = "bg-black", ShortName = "-black")]
	Black,

	/// <summary>
	/// Represents a transparent background.
	/// Name: "bg-transparent", ShortName: "-transparent"
	/// </summary>
	[Display(Name = "bg-transparent", ShortName = "-transparent")]
	Transparent
}