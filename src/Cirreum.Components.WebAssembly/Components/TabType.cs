namespace Cirreum.Components;

/// <summary>
/// Defines the visual style of tab components in Bootstrap.
/// This enum is used to specify the appearance of tab navigation elements.
/// </summary>
public enum TabType {
	/// <summary>
	/// Represents the default tab style with a bordered bottom for the active tab.
	/// Corresponds to Bootstrap's '.nav-tabs' class.
	/// </summary>
	Tabs,

	/// <summary>
	/// Represents a pill-shaped tab style with rounded corners and full background for the active tab.
	/// Corresponds to Bootstrap's '.nav-pills' class.
	/// </summary>
	Pills,

	/// <summary>
	/// Represents a tab style with an underline for the active tab.
	/// This style is often used for a more subtle tab appearance.
	/// </summary>
	Underlines
}