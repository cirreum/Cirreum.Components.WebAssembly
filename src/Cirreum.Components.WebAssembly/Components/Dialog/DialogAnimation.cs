namespace Cirreum.Components;

/// <summary>
/// Create a new instance.
/// </summary>
/// <param name="Type">The <see cref="DialogAnimationType"/> to use</param>
/// <param name="Duration">The duration of the animation in decimal seconds. E.g., 0.25 for quarter second</param>
public record DialogAnimation(DialogAnimationType Type, double Duration) {
	/// <summary>
	/// Create a No-Fade animation
	/// </summary>
	/// <returns>A newly constructed <see cref="DialogAnimation"/> instance.</returns>
	public static DialogAnimation None() => new(DialogAnimationType.None, 0);
	/// <summary>
	/// Create a Fade-In animation
	/// </summary>
	/// <param name="duration">duration of the animation in decimal seconds. E.g., 0.25 for quarter second.</param>
	/// <returns>A newly constructed <see cref="DialogAnimation"/> instance.</returns>
	public static DialogAnimation FadeIn(double duration) => new(DialogAnimationType.FadeIn, duration);
	/// <summary>
	/// Create a Fade-Out animation
	/// </summary>
	/// <param name="duration">duration of the animation in decimal seconds. E.g., 0.25 for quarter second.</param>
	/// <returns>A newly constructed <see cref="DialogAnimation"/> instance.</returns>
	public static DialogAnimation FadeOut(double duration) => new(DialogAnimationType.FadeOut, duration);
	/// <summary>
	/// Create a Fade-In and Fade-Out animation
	/// </summary>
	/// <param name="duration">duration of the animation in decimal seconds. E.g., 0.25 for quarter second.</param>
	/// <returns>A newly constructed <see cref="DialogAnimation"/> instance.</returns>
	public static DialogAnimation FadeInOut(double duration) => new(DialogAnimationType.FadeInOut, duration);
}