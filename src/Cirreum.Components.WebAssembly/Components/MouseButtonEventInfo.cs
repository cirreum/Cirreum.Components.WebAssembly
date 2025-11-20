namespace Cirreum.Components;

/// <summary>
/// Contains information about a trigger event, including its location, type, and the mouse button involved.
/// </summary>
public class MouseButtonEventInfo {
	/// <summary>
	/// Gets or sets the X-coordinate of the event relative to the whole document.
	/// </summary>
	/// <value>The X-coordinate in pixels.</value>
	public double PageX { get; set; }

	/// <summary>
	/// Gets or sets the Y-coordinate of the event relative to the whole document.
	/// </summary>
	/// <value>The Y-coordinate in pixels.</value>
	public double PageY { get; set; }

	/// <summary>
	/// Gets or sets the type of the trigger event.
	/// </summary>
	/// <value>The type of the event, either <see cref="MouseButtonEventType.Click"/> or <see cref="MouseButtonEventType.ContextMenu"/>.</value>
	public MouseButtonEventType Type { get; set; }

	/// <summary>
	/// Gets or sets the mouse button that triggered the event.
	/// </summary>
	/// <value>The mouse button involved in the event, as defined in the <see cref="MouseButton"/> enum.</value>
	public MouseButton Button { get; set; }

}