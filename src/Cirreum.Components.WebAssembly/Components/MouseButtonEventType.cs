namespace Cirreum.Components;
using System.Text.Json.Serialization;

/// <summary>
/// Represents the type of event triggered by a long press interaction.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum MouseButtonEventType {

	/// <summary>
	/// Indicates that a context menu event has occurred.
	/// This event is typically triggered by a right-click or a long press, depending on the device and configuration.
	/// </summary>
	[JsonPropertyName("contextmenu")]
	ContextMenu,

	/// <summary>
	/// Indicates that a regular click event has occurred.
	/// </summary>
	[JsonPropertyName("click")]
	Click,

}