namespace Cirreum.Components;

using Cirreum.Components.Interop;
using Microsoft.JSInterop;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

public sealed class DefaultBreakpointMonitor(
	IJSAppModule js)
	: IBreakpointMonitor,
		IDisposable {

	private bool _initialized;

	/// <inheritdoc/>
	public event Func<BreakpointChangeEventArgs, ValueTask>? MinBreakPointChanged;

	/// <inheritdoc/>
	public bool CheckForBreakPoint(Breakpoint breakpoint) {
		return js.GetCurrentBreakPoint(breakpoint.ToName());
	}

	/// <inheritdoc/>
	public ValueTask InitializeAsync() {
		if (this._initialized) {
			return ValueTask.CompletedTask; // Prevent reinitialization
		}
		this._initialized = true;
		var breakPoints = Enum.GetValues<Breakpoint>();
		foreach (var breakpoint in breakPoints) {
			var bp = breakpoint.ToName();
			var bpm = new BreakPointMonitor(breakpoint, this.HandleBreakpointChange);
			var bpmRef = DotNetObjectReference.Create(bpm);
			js.MonitorBreakpoint(bpmRef, bp);
			monitors.Add(bpmRef);
		}
		return ValueTask.CompletedTask;
	}

	private readonly List<DotNetObjectReference<BreakPointMonitor>> monitors = [];

	private async ValueTask HandleBreakpointChange(Breakpoint breakpoint, bool isActive) {
#if DEBUG
		Console.WriteLine($"HandleBreakpointChange[{breakpoint}={isActive}]");
#endif
		if (this.MinBreakPointChanged is not null) {
			await this.MinBreakPointChanged.Invoke(new(isActive, breakpoint));
		}
	}

	bool isDisposed;
	public void Dispose() {
		if (isDisposed) {
			return;
		}
		isDisposed = true;
		foreach (var monitor in this.monitors) {
			monitor.Dispose();
		}
		this.monitors.Clear();
	}

	private record BreakPointMonitor {

		[DynamicDependency(nameof(OnBreakpointChange))]
		public BreakPointMonitor(Breakpoint BreakpointMonitored, Func<Breakpoint, bool, ValueTask> Callback) {
			this.BreakpointMonitored = BreakpointMonitored;
			this.Callback = Callback;
		}

		public Breakpoint BreakpointMonitored { get; }
		public Func<Breakpoint, bool, ValueTask> Callback { get; }

		[JSInvokable("OnBreakpointChange")]
		public async ValueTask OnBreakpointChange(bool isActive) {
			await this.Callback(this.BreakpointMonitored, isActive);
		}
	}

}