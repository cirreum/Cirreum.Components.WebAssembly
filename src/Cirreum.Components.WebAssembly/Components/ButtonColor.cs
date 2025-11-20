namespace Cirreum.Components;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Exposes Bootstrap button color classes. 
/// The Name property contains the full class name (e.g., 'btn-primary'),
/// and the ShortName property contains the suffix with hyphen (e.g., '-primary').
/// </summary>
public enum ButtonColor {
	/// <summary>
	/// Represents no specific button color.
	/// Name: "", ShortName: ""
	/// </summary>
	[Display(Name = "", ShortName = "")]
	None,

	/// <summary>
	/// Represents the primary button color.
	/// Name: "btn-primary", ShortName: "-primary"
	/// </summary>
	[Display(Name = "btn-primary", ShortName = "-primary")]
	Primary,

	/// <summary>
	/// Represents the secondary button color.
	/// Name: "btn-secondary", ShortName: "-secondary"
	/// </summary>
	[Display(Name = "btn-secondary", ShortName = "-secondary")]
	Secondary,

	/// <summary>
	/// Represents the success button color.
	/// Name: "btn-success", ShortName: "-success"
	/// </summary>
	[Display(Name = "btn-success", ShortName = "-success")]
	Success,

	/// <summary>
	/// Represents the danger button color.
	/// Name: "btn-danger", ShortName: "-danger"
	/// </summary>
	[Display(Name = "btn-danger", ShortName = "-danger")]
	Danger,

	/// <summary>
	/// Represents the warning button color.
	/// Name: "btn-warning", ShortName: "-warning"
	/// </summary>
	[Display(Name = "btn-warning", ShortName = "-warning")]
	Warning,

	/// <summary>
	/// Represents the info button color.
	/// Name: "btn-info", ShortName: "-info"
	/// </summary>
	[Display(Name = "btn-info", ShortName = "-info")]
	Info,

	/// <summary>
	/// Represents the light button color.
	/// Name: "btn-light", ShortName: "-light"
	/// </summary>
	[Display(Name = "btn-light", ShortName = "-light")]
	Light,

	/// <summary>
	/// Represents the dark button color.
	/// Name: "btn-dark", ShortName: "-dark"
	/// </summary>
	[Display(Name = "btn-dark", ShortName = "-dark")]
	Dark,

	/// <summary>
	/// Represents the link-styled button.
	/// Name: "btn-link", ShortName: "-link"
	/// </summary>
	[Display(Name = "btn-link", ShortName = "-link")]
	Link,

	/// <summary>
	/// Represents the outline primary button style.
	/// Name: "btn-outline-primary", ShortName: "-outline-primary"
	/// </summary>
	[Display(Name = "btn-outline-primary", ShortName = "-outline-primary")]
	OutlinePrimary,

	/// <summary>
	/// Represents the outline secondary button style.
	/// Name: "btn-outline-secondary", ShortName: "-outline-secondary"
	/// </summary>
	[Display(Name = "btn-outline-secondary", ShortName = "-outline-secondary")]
	OutlineSecondary,

	/// <summary>
	/// Represents the outline success button style.
	/// Name: "btn-outline-success", ShortName: "-outline-success"
	/// </summary>
	[Display(Name = "btn-outline-success", ShortName = "-outline-success")]
	OutlineSuccess,

	/// <summary>
	/// Represents the outline danger button style.
	/// Name: "btn-outline-danger", ShortName: "-outline-danger"
	/// </summary>
	[Display(Name = "btn-outline-danger", ShortName = "-outline-danger")]
	OutlineDanger,

	/// <summary>
	/// Represents the outline warning button style.
	/// Name: "btn-outline-warning", ShortName: "-outline-warning"
	/// </summary>
	[Display(Name = "btn-outline-warning", ShortName = "-outline-warning")]
	OutlineWarning,

	/// <summary>
	/// Represents the outline info button style.
	/// Name: "btn-outline-info", ShortName: "-outline-info"
	/// </summary>
	[Display(Name = "btn-outline-info", ShortName = "-outline-info")]
	OutlineInfo,

	/// <summary>
	/// Represents the outline light button style.
	/// Name: "btn-outline-light", ShortName: "-outline-light"
	/// </summary>
	[Display(Name = "btn-outline-light", ShortName = "-outline-light")]
	OutlineLight,

	/// <summary>
	/// Represents the outline dark button style.
	/// Name: "btn-outline-dark", ShortName: "-outline-dark"
	/// </summary>
	[Display(Name = "btn-outline-dark", ShortName = "-outline-dark")]
	OutlineDark
}