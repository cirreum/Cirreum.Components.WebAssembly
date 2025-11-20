namespace Cirreum.Components;
using Cirreum.Startup;

/// <summary>
/// A service that monitors a media query for (screen and min-width)
/// for each <see cref="Breakpoint"/>.
/// </summary>
public interface IBreakpointMonitor : IAutoInitialize {
	/// <summary>
	/// The event triggered when a media-query changes, providing the
	/// <see cref="Breakpoint"/> that changed and if it's the active
	/// break-point.
	/// </summary>
	event Func<BreakpointChangeEventArgs, ValueTask>? MinBreakPointChanged;

	/// <summary>
	/// Evaluate if at least a given breakpoint.
	/// </summary>
	/// <param name="breakpoint">The breakpoint to check.</param>
	/// <returns>
	/// <see langword="true"/> if its at least the breakpoint requested, otherwise
	/// <see langword="false"/>.
	/// </returns>
	bool CheckForBreakPoint(Breakpoint breakpoint);
}