namespace Cirreum.Components;

/// <remarks>
/// 
/// </remarks>
/// <param name="isActive"></param>
/// <param name="changedBreakpoint"></param>
public class BreakpointChangeEventArgs(bool isActive, Breakpoint changedBreakpoint) : EventArgs {

	/// <summary>
	/// Is the <see cref="ChangedBreakpoint"/> active.
	/// </summary>
	public bool IsActive { get; } = isActive;

	/// <summary>
	/// The <see cref="Breakpoint"/> that changed.
	/// </summary>
	public Breakpoint ChangedBreakpoint { get; } = changedBreakpoint;

}