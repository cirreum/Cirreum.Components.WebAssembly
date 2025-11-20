namespace Cirreum.Extensions;

using Cirreum.Components;

public static class MouseButtonExtensions {
	/// <summary>
	/// Maps a <see cref="MouseButton"/> to the javascript mouse event button number.
	/// </summary>
	/// <param name="button">The <see cref="MouseButton"/> to map.</param>
	/// <returns>The javascript associated number.</returns>
	public static int GetButtonNumber(this MouseButton button) {
		return button switch {
			MouseButton.Left => 0,
			MouseButton.Middle => 1,
			MouseButton.Right => 2,
			MouseButton.Back => 3,
			MouseButton.Forward => 4,
			_ => -1,
		};
	}
}