namespace Cirreum.Components;

/// <summary>
/// Provides debounced execution of asynchronous actions with configurable delay.
/// </summary>
/// <remarks>
/// The debouncer delays execution of an action until a specified time period has elapsed
/// since the last invocation. If <see cref="Invoke"/> is called again before the delay
/// expires, the previous execution is cancelled and a new delay period begins.
/// This is useful for scenarios like search-as-you-type where you want to avoid
/// excessive API calls while the user is still typing.
/// </remarks>
public sealed class Debouncer : IDisposable {

	private Timer? _timer;
	private Func<Task>? _action;
	private int _delay;
	private bool _disposed;
	private bool _initialized;

	/// <summary>
	/// Creates a new debouncer. Must call <see cref="Initialize"/> before using <see cref="Invoke"/>.
	/// </summary>
	public Debouncer() {
	}

	/// <summary>
	/// Creates a new debouncer with the specified delay and action.
	/// </summary>
	/// <param name="delayMs">The delay in milliseconds before executing the action.</param>
	/// <param name="action">The action to execute after the delay.</param>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when delayMs is not between 10 and 5000 milliseconds.</exception>
	/// <exception cref="ArgumentNullException">Thrown when action is null.</exception>
	public Debouncer(int delayMs, Func<Task> action) {
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(delayMs);
		ArgumentOutOfRangeException.ThrowIfGreaterThan(delayMs, 5000);
		ArgumentOutOfRangeException.ThrowIfLessThan(delayMs, 10);
		ArgumentNullException.ThrowIfNull(action);

		this._delay = delayMs;
		this._action = action;
		this._initialized = true;
	}

	/// <summary>
	/// Initializes the debouncer with the action to execute and delay.
	/// </summary>
	/// <param name="delayMs">The delay in milliseconds before executing the action.</param>
	/// <param name="action">The action to execute after the delay.</param>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when delayMs is not between 10 and 5000 milliseconds.</exception>
	/// <exception cref="ArgumentNullException">Thrown when action is null.</exception>
	/// <exception cref="ObjectDisposedException">Thrown when the debouncer has been disposed.</exception>
	/// <exception cref="InvalidOperationException">Thrown when the debouncer has already been initialized.</exception>
	public void Initialize(int delayMs, Func<Task> action) {
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(delayMs);
		ArgumentOutOfRangeException.ThrowIfGreaterThan(delayMs, 5000);
		ArgumentOutOfRangeException.ThrowIfLessThan(delayMs, 10);
		ArgumentNullException.ThrowIfNull(action);

		ObjectDisposedException.ThrowIf(this._disposed, this);

		if (this._initialized) {
			throw new InvalidOperationException("Debouncer has already been initialized.");
		}

		this._delay = delayMs;
		this._action = action;
		this._initialized = true;
	}

	/// <summary>
	/// Triggers the debounced action. Cancels any pending execution and starts a new delay.
	/// </summary>
	/// <exception cref="InvalidOperationException">Thrown when the debouncer has not been initialized.</exception>
	/// <exception cref="ObjectDisposedException">Thrown when the debouncer has been disposed.</exception>
	public void Invoke() {
		ObjectDisposedException.ThrowIf(this._disposed, this);

		if (!this._initialized || this._action is null) {
			throw new InvalidOperationException("Debouncer must be initialized before invoking.");
		}

		// Dispose existing timer
		this._timer?.Dispose();

		// Create new timer with the specified delay, firing only once
		this._timer = new Timer(async _ => {
			this._timer?.Dispose();
			this._timer = null;
			try {
				await this._action();
			} catch {
				// Silently catch exceptions
			}
		}, null, this._delay, Timeout.Infinite);
	}

	public void Dispose() {
		if (this._disposed) {
			return;
		}

		this._timer?.Dispose();
		this._timer = null;
		this._action = null;
		this._disposed = true;
	}
}