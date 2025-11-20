namespace Cirreum.Components;

/// <summary>
/// Specifies the desired initial focus.
/// </summary>
public enum DefaultFocusType {
	/// <summary>
	/// Ambient focus.
	/// </summary>
	Default,
	/// <summary>
	/// Focus the first focusable element.
	/// </summary>
	First,
	/// <summary>
	/// Focus the last focusable element.
	/// </summary>
	Last
}