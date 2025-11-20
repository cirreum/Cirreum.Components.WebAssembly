namespace Cirreum.Components;

using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

public partial class ToastElement(IJSAppModule jsApp) {

	[CascadingParameter]
	private ToastContainer? ToastsContainer { get; set; }

	[Parameter, EditorRequired]
	public ToastOptions Options { get; set; } = default!;

	[Parameter]
	public Guid ToastId { get; set; } = Guid.NewGuid();

	[Parameter]
	public RenderFragment? Message { get; set; }
	private RenderFragment? MessageFragment {
		get {
			if (this.Options.Instance?.Message != null) {
				return this.Options.Instance.Message;
			}
			return this.Message;
		}
	}
	private bool HasMessageFragment => this.Options.Instance?.Message != null || this.Message != null;
	private bool HasRenderedMessage;

	[Parameter]
	public string? Title { get; set; }
	private string CurrentTitle {
		get {
			if (this.Options.Instance != null) {
				return this.Options.Instance.Title ?? "";
			}
			return this.Title ?? "";
		}
	}

	[Parameter]
	public string? Subtitle { get; set; }
	private string? CurrentSubtitle {
		get {
			if (this.Options.Instance != null) {
				return this.Options.Instance.Subtitle;
			}
			return this.Subtitle;
		}
	}

	private CountdownTimer? _countdownTimer;

	private string ToastTypeStyle => StyleBuilder.Empty()
		.AddStyle("--bs-toast-border-color", this.Options.BorderStyle)
		.AddStyle("--bs-toast-progress-color", this.Options.ProgressStyle)
		.AddStyle("--bs-toast-header-bg", this.Options.HeaderBackgroundStyle)
		.Build();
	private string ToastClass => CssBuilder.Default("toast")
		.AddClass("force-cursor", when: this.Options.ShowCloseButton is false)
		.AddClassIfNotEmpty(this.Options.AdditionalClasses)
		.Build();

	private string ToastTypeTitleStyle => StyleBuilder.Empty()
		.AddStyle("--bs-toast-title-color", this.Options.TitleStyle)
		.Build();


	protected override async Task OnInitializedAsync() {

		await base.OnInitializedAsync();

		this.ToastsContainer?.AssociateElement(this);

		this.HasRenderedMessage = this.HasMessageFragment || this.Options.Instance?.ContentType != null;

		if (this.Options.DisableTimeout.ValueOrFalse() || this.Options.Timeout == 0) {
			// If no timeout or timeout is disabled, ensure we show a Close Button
			this.Options.ShowCloseButton = true;
			return;
		}

		this._countdownTimer = this.Options.ShowProgressBar.ValueOrFalse()
			? new CountdownTimer(this.Options.Timeout)
				.OnTick(this.CalculateProgress)
				.OnElapsed(this.Close)
			: new CountdownTimer(this.Options.Timeout)
				.OnElapsed(this.Close);
		await this._countdownTimer.StartAsync();

	}

	/// <summary>
	/// Closes the toast
	/// </summary>
	public void Close() {
		this.ToastsContainer?.RemoveToast(this.ToastId);
	}

	private void TryPauseCountdown() {
		if (this.Options.PauseProgressOnHover.ValueOrFalse()) {
			this._countdownTimer?.Pause();
		}
	}

	private void TryResumeCountdown() {
		if (this.Options.PauseProgressOnHover.ValueOrFalse()) {
			this._countdownTimer?.UnPause();
		}
	}

	private void CalculateProgress(int percentComplete) {
		this.Options.Progress = 100 - percentComplete;
		this.StateHasChanged();
	}

	private readonly string MessageId = IdGenerator.Next;
	private string RenderedMessageString {
		get {
			if (this.Rendered && this.HasRenderedMessage) {
				return jsApp.GetElementTextContent($"#{MessageId}", true);
			}
			return "";
		}
	}

	private ButtonColor MapButtonColorFromToastType {
		get {
			return this.Options.StyleType switch {
				ToastStyleType.Default => ButtonColor.OutlinePrimary,
				ToastStyleType.Primary => ButtonColor.OutlinePrimary,
				ToastStyleType.Secondary => ButtonColor.OutlineSecondary,
				ToastStyleType.Danger => ButtonColor.OutlineDanger,
				ToastStyleType.Warning => ButtonColor.OutlineWarning,
				ToastStyleType.Info => ButtonColor.OutlineInfo,
				ToastStyleType.Success => ButtonColor.OutlineSuccess,
				ToastStyleType.Dark => ButtonColor.OutlineLight,
				ToastStyleType.Light => ButtonColor.OutlineDark,
				_ => ButtonColor.OutlinePrimary,
			};
		}
	}

	protected void ToastClick() {
		var clicked = false;
		if (this.Options.OnClick is not null && this.Options.Instance is not null) {
			this.Options.OnClick(this.Options.Instance);
			clicked = true;
		}
		if (clicked) {
			this.Update();
		}
	}

	protected override void Dispose(bool disposing) {
		this._countdownTimer?.Dispose();
		base.Dispose(disposing);
	}

}