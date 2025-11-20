namespace Cirreum.Components;

using System.Threading;

internal class CountdownTimer : IDisposable {

	private int _percentComplete;
	private bool _isPaused;
	private Func<int, Task>? _tickAsyncDelegate;
	private Action<int>? _tickDelegate;
	private Action? _elapsedDelegate;
	private PeriodicTimer? _timer;
	private CancellationTokenSource? _cancellationTokenSource;

	internal CountdownTimer(TimeoutDuration timeoutDuration) {

		// Get the base milliseconds value
		int baseTimeoutMs = timeoutDuration; // implicit conversion to int (milliseconds)

		// There is a CSS Transition delay of 0.2s
		var offset = baseTimeoutMs * 0.000001;

		// val in seconds, converts to 1% in milliseconds
		// 1% = ((val*1000)*0.01) OR (val*10)
		// val in milliseconds, converts to 1% in milliseconds
		// 1% = (val*.01)
		var interval = (baseTimeoutMs * 0.01) + offset;

		this._timer = new PeriodicTimer(TimeSpan.FromMilliseconds(interval));
		this._cancellationTokenSource = new CancellationTokenSource();
	}

	internal CountdownTimer OnTickAsync(Func<int, Task> updateProgressDelegate) {
		this._tickAsyncDelegate = updateProgressDelegate;
		return this;
	}
	internal CountdownTimer OnTick(Action<int> updateProgressDelegate) {
		this._tickDelegate = updateProgressDelegate;
		return this;
	}

	internal CountdownTimer OnElapsed(Action elapsedDelegate) {
		this._elapsedDelegate = elapsedDelegate;
		return this;
	}

	internal async Task StartAsync() {
		ObjectDisposedException.ThrowIf(this._timer is null, this._timer);
		this._percentComplete = 0;
		await this.DoWorkAsync();
	}
	internal void Stop() {
		this._cancellationTokenSource?.Dispose();
		this._cancellationTokenSource = null;
		this._timer?.Dispose();
		this._timer = null;
	}

	internal void Pause() {
		ObjectDisposedException.ThrowIf(this._timer is null, this._timer);
		this._isPaused = true;
	}

	internal void UnPause() {
		ObjectDisposedException.ThrowIf(this._timer is null, this._timer);
		this._isPaused = false;
	}

	private async Task DoWorkAsync() {
		while (
			this._timer is not null &&
			this._cancellationTokenSource is not null &&
			this._cancellationTokenSource.IsCancellationRequested is false &&
			await this._timer.WaitForNextTickAsync(this._cancellationTokenSource.Token)) {
			if (this._isPaused is false) {
				this._percentComplete++;
			}
			if (this._tickAsyncDelegate is not null) {
				await this._tickAsyncDelegate(this._percentComplete);
			}
			this._tickDelegate?.Invoke(this._percentComplete);
			if (this._percentComplete == 100) {
				await Task.Delay(100);
				this._elapsedDelegate?.Invoke();
				return;
			}
		}
	}

	public void Dispose() => this.Stop();

}