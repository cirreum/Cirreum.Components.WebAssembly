namespace Cirreum.Components;
using System.Text.Json.Serialization;

/// <summary>
/// Defines the possible options for mouse buttons.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum MouseButton {
	/// <summary>
	/// No button or unknown button.
	/// </summary>
	[JsonPropertyName("none")]
	None = -1,

	/// <summary>
	/// Left mouse button.
	/// </summary>
	[JsonPropertyName("left")]
	Left = 0,

	/// <summary>
	/// Middle mouse button.
	/// </summary>
	[JsonPropertyName("middle")]
	Middle = 1,

	/// <summary>
	/// Right mouse button.
	/// </summary>
	[JsonPropertyName("right")]
	Right = 2,

	/// <summary>
	/// Browser back button (if present).
	/// </summary>
	[JsonPropertyName("back")]
	Back = 3,

	/// <summary>
	/// Browser forward button (if present).
	/// </summary>
	[JsonPropertyName("forward")]
	Forward = 4
}