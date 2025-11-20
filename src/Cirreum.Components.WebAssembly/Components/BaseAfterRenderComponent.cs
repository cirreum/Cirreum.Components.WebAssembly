namespace Cirreum.Components;

using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Base component that provides after-render action queuing and proper disposal patterns.
/// </summary>
/// <remarks>
/// <para>
/// This component provides a dual-queue system for executing actions after rendering:
/// actions queued before first render are held until the component is rendered, while
/// actions queued after first render execute immediately on the next render cycle.
/// </para>
/// <para>
/// Implements both <see cref="IDisposable"/> and <see cref="IAsyncDisposable"/> patterns
/// with cancellation token support for clean component lifecycle management.
/// </para>
/// </remarks>
public abstract class BaseAfterRenderComponent : ComponentBase, IAsyncDisposable, IDisposable {

	/// <summary>
	/// Gets or sets the unique id of the element. Default: <see cref="IdGenerator.Next"/>
	/// </summary>
	/// <remarks>
	/// Note that this ID is not defined for the component but instead for the underlined element that it represents.
	/// </remarks>
	public string ElementId { get; protected set; } = IdGenerator.Next;

	/// <summary>
	/// Rerenders the component by calling StateHasChanged().
	/// </summary>
	public void Update() {
		if (this.IsDisposing || this.IsDisposed) {
			return;
		}

		this.StateHasChanged();
	}

	/// <summary>
	/// Cancellation token source for managing component lifecycle
	/// </summary>
	private CancellationTokenSource? _cancellationTokenSource = new();

	/// <summary>
	/// Cancellation token that gets cancelled when component is disposing
	/// </summary>
	protected CancellationToken DisposalToken => this._cancellationTokenSource?.Token ?? CancellationToken.None;

	/// <summary>
	/// A queue of functions to execute after the rendering.
	/// </summary>
	[AllowNull]
	private Queue<Func<CancellationToken, Task>> executeAfterRenderQueue;

	/// <summary>
	/// A queue of functions to execute after the rendering. Used only during the component initialization.
	/// </summary>
	[AllowNull]
	private Queue<Func<CancellationToken, Task>> delayedExecuteAfterRenderQueue;

	/// <summary>
	/// Pushes an action to the stack to be executed after the rendering is done.
	/// </summary>
	/// <param name="action">Action to invoke after render</param>
	protected void RunAfterRender(Func<Task> action) {
		this.RunAfterRender((ct) => action());
	}

	/// <summary>
	/// Pushes an action to the stack to be executed after the rendering is done.
	/// </summary>
	/// <param name="action">Action to invoke after render</param>
	protected void RunAfterRender(Func<CancellationToken, Task> action) {
		if (this.IsDisposing || this.IsDisposed) {
			return;
		}

		if (!this.Rendered) {
			this.delayedExecuteAfterRenderQueue ??= new();
			this.delayedExecuteAfterRenderQueue.Enqueue(action);
		} else {
			this.executeAfterRenderQueue ??= new();
			this.executeAfterRenderQueue.Enqueue(action);
		}
	}

	/// <summary>
	/// Pushes an action to the stack to be executed after the rendering is done.
	/// </summary>
	/// <param name="action">Action to invoke after render</param>
	protected void RunAfterRender(Action action) {
		this.RunAfterRender((ct) => {
			if (!ct.IsCancellationRequested) {
				action();
			}

			return Task.CompletedTask;
		});
	}

	private bool PushDelayedExecuteAfterRender() {
		if (this.delayedExecuteAfterRenderQueue?.Count > 0) {
			while (this.delayedExecuteAfterRenderQueue.Count > 0) {
				var action = this.delayedExecuteAfterRenderQueue.Dequeue();
				this.RunAfterRender(action);
			}
			this.delayedExecuteAfterRenderQueue = null;
			return true;
		}
		return false;
	}

	protected override async Task OnAfterRenderAsync(bool firstRender) {
		if (this.IsDisposing || this.IsDisposed) {
			return;
		}

		if (firstRender) {
			await this.OnAfterFirstRenderAsync();
			this.Rendered = true;

			if (this.PushDelayedExecuteAfterRender()) {
				await this.InvokeAsync(this.StateHasChanged);
			}
		}

		this.Rendered = true;
		await this.ExecuteQueuedActions();
	}

	private async Task ExecuteQueuedActions() {
		if (this.executeAfterRenderQueue?.Count > 0 && !this.IsDisposing && !this.IsDisposed) {
			while (this.executeAfterRenderQueue.Count > 0 && !this.IsDisposing && !this.IsDisposed) {
				var action = this.executeAfterRenderQueue.Dequeue();
				try {
					await action(this.DisposalToken);
				} catch (OperationCanceledException) when (this.DisposalToken.IsCancellationRequested) {
					// Expected during disposal
					break;
				}
			}
		}
	}

	/// <summary>
	/// Method is called only once when component is first rendered.
	/// </summary>
	/// <returns>A task that represents the asynchronous operation.</returns>
	protected virtual Task OnAfterFirstRenderAsync() => Task.CompletedTask;

	/// <inheritdoc/>
	public async ValueTask DisposeAsync() {
		if (this.IsDisposing || this.IsDisposed) {
			return;
		}

		this.IsDisposing = true;
		this._cancellationTokenSource?.Cancel();

		await this.DisposeAsyncCore();
		this.Dispose(true);

		this.IsDisposed = true;
		GC.SuppressFinalize(this);
	}

	/// <summary>
	/// Override this method to provide async disposal logic for derived classes.
	/// </summary>
	protected virtual ValueTask DisposeAsyncCore() => ValueTask.CompletedTask;

	/// <inheritdoc/>
	public void Dispose() {

		if (this.IsDisposing || this.IsDisposed) {
			return;
		}

		this.IsDisposing = true;
		this._cancellationTokenSource?.Cancel();

		this.Dispose(true);
		this.IsDisposed = true;

		this._cancellationTokenSource?.Dispose();
		this._cancellationTokenSource = null;

		this.executeAfterRenderQueue?.Clear();
		this.delayedExecuteAfterRenderQueue?.Clear();

		GC.SuppressFinalize(this);
	}

	/// <summary>
	/// Override this method to provide disposal logic for derived classes.
	/// </summary>
	/// <param name="disposing">True if the component is in the disposing process.</param>
	protected virtual void Dispose(bool disposing) { }

	/// <summary>
	/// Indicates if component has been rendered in the browser.
	/// </summary>
	protected bool Rendered { get; private set; }

	/// <summary>
	/// Indicates if the component is disposed.
	/// </summary>
	protected bool IsDisposed { get; private set; }

	/// <summary>
	/// Indicates if the component is currently disposing.
	/// </summary>
	/// <remarks>
	/// Once set to true, this remains true for the lifetime of the component.
	/// </remarks>
	protected bool IsDisposing { get; private set; }

}