namespace Cirreum.Components;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Reflection.Metadata;

public partial class Dialog : IDisposable {

	private bool IsDisplayStatic;
	private ElementReference dialogInstanceElementRef;
	private readonly Dictionary<string, object> _closeBtnAttributes = new() {
		{
			"tabindex", "-1"
		}
	};

	[CascadingParameter]
	private DialogManager? Manager { get; set; }

	[CascadingParameter]
	private DialogOptions? GlobalDialogOptions { get; set; }


	[Parameter]
	public string Id { get; set; } = IdGenerator.Next;
	private string ContainerId => $"{this.Id}-container";
	private string OverlayId => $"{this.Id}-overlay";
	internal string HeaderId => $"{this.Id}_header";
	private string ContentId => $"{this.Id}_content";

	[Parameter]
	public RenderFragment? ChildContent { get; set; }

	[Parameter]
	public RenderFragment? DialogContent { get; set; }

	/// <summary>
	/// The Title (string or <see cref="RenderFragment"/>) to display in the header. Default: ""
	/// </summary>
	[Parameter]
	public string Title { get; set; } = "";
	/// <summary>
	/// Set the dialog's <see cref="Title"/> value.
	/// </summary>
	/// <param name="value">The new value to set.</param>
	public void SetTitle(string value) {
		this.TitleOrFragment = this.Title = value;
		this.StateHasChanged();
	}

	/// <summary>
	/// The Title (string or <see cref="RenderFragment"/>) to display in the header. Default: ""
	/// </summary>
	[Parameter]
	public RenderFragment? DialogTitle { get; set; }
	/// <summary>
	/// Set the dialog's <see cref="Title"/> value.
	/// </summary>
	/// <param name="value">The new value to set.</param>
	public void SetTitleFragment(RenderFragment value) {
		this.TitleOrFragment = this.DialogTitle = value;
		this.StateHasChanged();
	}

	internal StringOrRenderFragment TitleOrFragment { get; set; } = "";


	/// <summary>
	/// The optional icon to display.
	/// </summary>
	/// <remarks>
	/// This should be a class. Example: bi bi-person
	/// </remarks>
	[Parameter]
	public string? TitleIcon { get; set; }

	/// <summary>
	/// The optional <see cref="DialogOptions"/> specific to
	/// this dialog instance, overriding the global defaults.
	/// </summary>
	[Parameter]
	public DialogOptions Options { get; set; } = DialogOptions.AsStaticDisplay;

	/// <summary>
	/// Is the Dialog Modal (has a background overlay). Can also be set via Options.
	/// </summary>
	/// <remarks>
	/// <see cref="DialogOptions.Modal"/>
	/// </remarks>
	[Parameter]
	public bool? Modal { get; set; }
	internal bool IsModal;
	private string AriaIsModal => this.IsModal.ToAttributeValue();

	/// <summary>
	/// Gets or sets the initial visibility state of the dialog.
	/// </summary>
	/// <remarks>
	/// This parameter is used only for initialization. To control dialog visibility at runtime,
	/// use <see cref="ShowAsync"/> and <see cref="HideAsync"/> methods.
	/// Changes to this parameter after initialization will be ignored.
	/// <para>
	/// You can use two-way binding (@bind-Visible) to receive notifications when the dialog's
	/// visibility changes due to internal actions (ShowAsync/HideAsync calls, user interactions, etc.).
	/// </para>
	/// </remarks>
	/// <example>
	/// <code>
	/// <!-- Monitor visibility changes -->
	/// &lt;Dialog @bind-Visible="@isDialogVisible" /&gt;
	/// 
	/// <!-- Control visibility programmatically -->
	/// &lt;Dialog @ref="dialogRef" /&gt;
	/// &lt;button @onclick="() => dialogRef.ShowAsync()"&gt;Show&lt;/button&gt;
	/// </code>
	/// </example>
	[Parameter]
	public bool Visible { get; set; }
	private bool IsVisible;
	private async Task SetVisible(bool value) {
		if (value != this.IsVisible) {
			this.IsVisible = value;
			await this.VisibleChanged.InvokeAsync(value);
			this.StateHasChanged();
		}
	}

	/// <summary>
	/// The event callback invoked when <see cref="IsVisible"/> has changed.
	/// </summary>
	[Parameter]
	public EventCallback<bool> VisibleChanged { get; set; }

	/// <summary>
	/// Specify a custom fixed width for the dialog.
	/// </summary>
	[Parameter]
	public string? Width { get; set; }
	private string? customWidth;

	/// <summary>
	/// Specify a maximum width for the dialog.
	/// </summary>
	[Parameter]
	public DialogSize? Size { get; set; }
	private DialogSize? customSizing;

	private bool IsNested;

	private RenderFragment CssAnimationStyle = default!;

	private string ResolvedDialogContainerClass = "";
	private string DialogContainerClass => CssBuilder
		.Default(this.ResolvedDialogContainerClass)
			.AddClass("d-none", when: !this.IsVisible)
		.Build();

	private DialogPosition AssignedPosition { get; set; }

	internal bool IsDraggable { get; private set; }
	private bool _hasDraggability;
	internal void RemoveDraggability() {
		if (this._hasDraggability) {
			this._hasDraggability = false;
			this.Manager?.RemoveDraggable(this.Id);
		}
	}

	private string DialogClass = "";

	private string DialogStyle = "";

	private bool HideHeader;

	internal bool HideCloseButton;

	private bool BackgroundCancel;

	private string OverlayClass => CssBuilder
		.Default("dialog-overlay")
			.AddClassIfNotEmpty(this.OverlayNestedClass)
			.AddClassIfNotEmpty(this.OverlayAnimationClass)
		.Build();

	private string OverlayNestedClass => this.IsNested ? "dialog-overlay-nested" : "";

	private string? OverlayAnimationClass { get; set; }

	private DialogAnimation Animation { get; set; } = DialogAnimation.None();

	private bool usingFocusTrap;

	private DefaultFocusType DefaultFocus;

	private string AnimationDuration {
		get {
			var duration = this.Animation?.Duration * 1000;
			return FormattableString.Invariant($"{duration}ms");
		}
	}

	private int AssignedPositionCount => (this.Manager is null) ? 0 : this.AssignedPosition switch {
		DialogPosition.Center => this.Manager.DialogsCentered,
		DialogPosition.CenterLeft => this.Manager.DialogsCenteredLeft,
		DialogPosition.CenterRight => this.Manager.DialogsCenteredRight,
		DialogPosition.Top => this.Manager.DialogsTop,
		DialogPosition.TopLeft => this.Manager.DialogsTopLeft,
		DialogPosition.TopRight => this.Manager.DialogsTopRight,
		DialogPosition.Bottom => this.Manager.DialogsBottom,
		DialogPosition.BottomLeft => this.Manager.DialogsBottomLeft,
		DialogPosition.BottomRight => this.Manager.DialogsBottomRight,
		_ => -1,
	};

	internal void CancelImmediately() {

		// CRITICAL: Disable focus trap first and ensure component gets updated
		this.usingFocusTrap = false;

		if (this.Manager is not null) {

			if (PositionDecrementActions.TryGetValue(this.AssignedPosition, out var decrementAction)) {
				decrementAction(this.Manager);
			}

			// Note: Don't call StateHasChanged here since DialogManager will handle it
			// after removing from DialogReferences

		} else if (this.IsDisplayStatic is false) {
			_ = this.HideAsync();
		}
	}

	/// <summary >
	/// Closes the dialog with the specified <paramref name="dialogResult"/>.
	/// </summary >
	/// <param name="dialogResult" > </param >
	public async Task CloseAsync(DialogResult dialogResult) {

		if (this.Manager is not null) {

			if (PositionDecrementActions.TryGetValue(this.AssignedPosition, out var decrementAction)) {
				decrementAction(this.Manager);
			}

			// Fade out the dialog, and after that actually remove it
			if (this.Animation?.Type is DialogAnimationType.FadeOut or DialogAnimationType.FadeInOut) {
				this.DialogClass += " dialog-fade-out";
				this.OverlayAnimationClass += " dialog-fade-out";
				this.StateHasChanged();
				if (this.Animation.Duration > 0) {
					await Task.Delay((int)(this.Animation.Duration * 1000));
				}
			}

			this.Manager.DismissInstance(this.Id, dialogResult);

		} else if (this.IsDisplayStatic is false) {
			await this.HideAsync();
		}
	}

	/// <summary >
	/// Closes the dialog with a default Ok result.
	/// </summary >
	public async Task OkAsync() => await this.CloseAsync(DialogResult.Ok());

	/// <summary >
	/// Closes the dialog with the specified Ok result object.
	/// </summary >
	public async Task OkAsync<TPayload>(TPayload payload) => await this.CloseAsync(DialogResult.Ok(payload));

	/// <summary >
	/// Closes the dialog and returns a cancelled DialogResult.
	/// </summary >
	public async Task CancelAsync() => await this.CloseAsync(DialogResult.Cancel());

	/// <summary >
	/// Closes the dialog returning the specified <paramref name="payload"/> in a cancelled DialogResult.
	/// </summary >
	public async Task CancelAsync<TPayload>(TPayload payload) => await this.CloseAsync(DialogResult.Cancel(payload));


	public async Task ShowAsync() {
		await this.SetVisible(true);
	}

	public async Task HideAsync() {
		await this.SetVisible(false);
	}

	private void Configure() {

		// Apply any directly-set parameters
		this.IsVisible = this.Visible;

		// Apply Options (which can override)
		this.ApplyDisplayStatic();
		this.ApplyIsVisible();
		this.ApplyModal();
		this.ApplyTitle();
		this.ApplyTitleIcon();
		this.ApplyCustomWidth();
		this.ApplyCustomSizing();
		this.ApplyAnimation();
		this.ApplyIsDraggable();
		this.ApplyAssignedPosition();
		this.ApplyHideHeader();
		this.ApplyHideCloseButton();
		this.ApplyBackgroundCancel();
		this.ApplyDefaultFocusType();
		this.ApplyActivateFocusTrap();
		this.ApplyIsNested();

		// Resolve Css and Styling
		this.ResolveContainerClass();
		this.ResolveAnimationClass();
		this.ResolveDialogClass();
		this.ResolveDialogStyle();

		// Generate Animation Style
		this.CssAnimationStyle = builder => {

			if (this.Animation.Type == DialogAnimationType.None) {
				builder.OpenElement(1, "style");
				builder.CloseElement();
				return;
			}

			var fadeIn =
				$"#{this.Id}.dialog-fade-in," +
				$"#{this.Id}-overlay.dialog-fade-in {{" +
					$"animation: {this.AnimationDuration} ease-out 0s DialogFadeIn;" +
				"}";

			var fadeOut =
				$"#{this.Id}.dialog-fade-out," +
				$"#{this.Id}-overlay.dialog-fade-out {{" +
					$"animation: {this.@AnimationDuration} ease-out 0s DialogFadeOut;" +
					"opacity: 0;" +
				"}";

			var keyFrameIn = "@keyframes DialogFadeIn { 0% { opacity: 0; } 100% { opacity: 1; } }";
			var keyFrameOut = "@keyframes DialogFadeOut { 0% { opacity: 1; }  100% { opacity: 0; } }";

			builder.OpenElement(1, "style");
			builder.AddContent(2, fadeIn);
			builder.AddContent(3, fadeOut);
			builder.AddContent(4, keyFrameIn);
			builder.AddContent(5, keyFrameOut);
			builder.CloseElement();

		};

	}

	private void ApplyDisplayStatic() {

		if (this.Options.DisplayStatic.HasValue) {
			this.IsDisplayStatic = this.Options.DisplayStatic.Value;
			return;
		}

		if (this.GlobalDialogOptions?.DisplayStatic.HasValue is true) {
			this.IsDisplayStatic = this.GlobalDialogOptions.DisplayStatic.Value;
			return;
		}

		this.IsDisplayStatic = false;

	}

	private void ApplyIsVisible() {

		if (this.IsDisplayStatic) {
			this.IsVisible = true;
			this.Options.Visible = true;
			return;
		}

		if (this.Options.Visible.HasValue) {
			this.IsVisible = this.Options.Visible.Value;
			return;
		}

		if (this.GlobalDialogOptions?.Visible.HasValue is true) {
			this.IsVisible = this.GlobalDialogOptions.Visible.Value;
			return;
		}

	}

	private void ApplyModal() {

		if (this.IsDisplayStatic) {
			this.IsModal = false;
			return;
		}

		if (this.Modal is not null) {
			this.IsModal = this.Modal.Value;
			return;
		}

		if (this.Options.Modal.HasValue) {
			this.IsModal = this.Options.Modal.Value;
			return;
		}

		if (this.GlobalDialogOptions?.Modal.HasValue is true) {
			this.IsModal = this.GlobalDialogOptions.Modal.Value;
			return;
		}

		this.IsModal = false;

	}

	private void ApplyTitle() {

		if (this.Options.TitleOrFragment.HasValue) {
			this.TitleOrFragment = this.Options.TitleOrFragment.Value;
		}

		if (this.Title.HasValue()) {
			this.TitleOrFragment = this.Title;
			return;
		}

		if (this.DialogTitle is not null) {
			this.TitleOrFragment = this.DialogTitle;
			return;
		}

	}

	private void ApplyTitleIcon() {

		if (this.TitleIcon.HasValue()) {
			return;
		}

		if (this.Options.TitleIcon.HasValue()) {
			this.TitleIcon = this.Options.TitleIcon;
			return;
		}

		if (this.GlobalDialogOptions?.TitleIcon?.HasValue() is true) {
			this.TitleIcon = this.GlobalDialogOptions.TitleIcon;
			return;
		}

	}

	private void ApplyCustomWidth() {

		if (this.Width.HasValue()) {
			this.customWidth = this.Width;
			return;
		}

		if (this.Options.DialogWidth.HasValue()) {
			this.customWidth = this.Options.DialogWidth;
			return;
		}

		if (this.GlobalDialogOptions?.DialogWidth.HasValue() is true) {
			this.customWidth = this.GlobalDialogOptions?.DialogWidth;
			return;
		}

	}

	private void ApplyCustomSizing() {

		if (this.Size.HasValue) {
			this.customSizing = this.Size;
			return;
		}

		if (this.Options.DialogSizing.HasValue) {
			this.customSizing = this.Options.DialogSizing;
			return;
		}

		if (this.GlobalDialogOptions?.DialogSizing.HasValue is true) {
			this.customSizing = this.GlobalDialogOptions.DialogSizing;
			return;
		}

	}

	private void ApplyAnimation() {
		if (this.IsDisplayStatic) {
			this.Animation = DialogAnimation.None();
			return;
		}
		this.Animation = this.Options.Animation ?? this.GlobalDialogOptions?.Animation ?? DialogAnimation.None();
	}

	private void ApplyIsDraggable() {
		var requestedDraggability = this.Options.Draggable ?? this.GlobalDialogOptions?.Draggable ?? false;
		this.IsDraggable =
			requestedDraggability &&
			this.IsDisplayStatic is false;
	}

	private void ApplyAssignedPosition() {
		this.AssignedPosition =
			this.Options.Position ??
			this.GlobalDialogOptions?.Position ??
			DialogPosition.Center;
	}

	private void ApplyHideHeader() {

		if (this.Options.HideHeader.HasValue) {
			this.HideHeader = this.Options.HideHeader.Value;
			return;
		}

		if (this.GlobalDialogOptions?.HideHeader.HasValue is true) {
			this.HideHeader = this.GlobalDialogOptions.HideHeader.Value;
			return;
		}

		this.HideHeader = false;

	}

	private void ApplyHideCloseButton() {

		if (this.Options.HideCloseButton.HasValue) {
			this.HideCloseButton = this.Options.HideCloseButton.Value;
			return;
		}

		if (this.GlobalDialogOptions?.HideCloseButton.HasValue is true) {
			this.HideCloseButton = this.GlobalDialogOptions.HideCloseButton.Value;
			return;
		}

		this.HideCloseButton = false;

	}

	private void ApplyBackgroundCancel() {

		if (this.IsDisplayStatic) {
			this.BackgroundCancel = false;
			return;
		}

		if (this.Options.BackgroundCancel.HasValue) {
			this.BackgroundCancel = this.Options.BackgroundCancel.Value;
			return;
		}

		if (this.GlobalDialogOptions?.BackgroundCancel.HasValue is true) {
			this.BackgroundCancel = this.GlobalDialogOptions.BackgroundCancel.Value;
			return;
		}

		this.BackgroundCancel = false;

	}

	private void ApplyDefaultFocusType() {

		if (this.Options.DefaultFocus.HasValue) {
			this.DefaultFocus = this.Options.DefaultFocus.Value;
			return;
		}

		if (this.GlobalDialogOptions?.DefaultFocus.HasValue is true) {
			this.DefaultFocus = this.GlobalDialogOptions.DefaultFocus.Value;
			return;
		}

		this.DefaultFocus = DefaultFocusType.Default;

	}

	private void ApplyActivateFocusTrap() {

		if (this.Options.UseFocusTrap.HasValue) {
			this.usingFocusTrap = this.Options.UseFocusTrap.Value;
			return;
		}

		if (this.GlobalDialogOptions?.UseFocusTrap.HasValue is true) {
			this.usingFocusTrap = this.GlobalDialogOptions.UseFocusTrap.Value;
			return;
		}

		this.usingFocusTrap = false;

	}

	private void ApplyIsNested() {
		if (this.IsDisplayStatic) {
			return;
		}
		this.IsNested = this.AssignedPositionCount > 0;
		if (this.Manager is null) {
			return;
		}
		if (PositionIncrementActions.TryGetValue(this.AssignedPosition, out var incrementAction)) {
			incrementAction(this.Manager);
		}
	}


	private void ResolveContainerClass() {
		this.ResolvedDialogContainerClass = CssBuilder.Default("dialog-container")
				.AddClass(this.ResolveContainerPositionClass())
				.AddClass("dialog-sm", when: this.customSizing is DialogSize.Small)
				.AddClass("dialog-md", when: this.customSizing is DialogSize.Medium)
				.AddClass("dialog-lg", when: this.customSizing is DialogSize.Large)
				.AddClass("dialog-xl", when: this.customSizing is DialogSize.ExtraLarge)
			.Build();
	}
	private string ResolveContainerPositionClass() {

		if (this.IsDisplayStatic) {
			return "static";
		}

		if (this.IsDraggable) {
			return "dialog-center";
		}

		return this.AssignedPosition switch {
			DialogPosition.Center => "dialog-center",
			DialogPosition.CenterLeft => "dialog-center-left",
			DialogPosition.CenterRight => "dialog-center-right",
			DialogPosition.Top => "dialog-top",
			DialogPosition.TopLeft => "dialog-topleft",
			DialogPosition.TopRight => "dialog-topright",
			DialogPosition.Bottom => "dialog-bottom",
			DialogPosition.BottomLeft => "dialog-bottomleft",
			DialogPosition.BottomRight => "dialog-bottomright",
			_ => "dialog-center"
		};

	}

	private void ResolveAnimationClass() {
		this.OverlayAnimationClass = CssBuilder.Empty()
			.AddClass("dialog-fade-in", when: this.Animation?.Type is DialogAnimationType.FadeInOut)
			.AddClass("dialog-fade-in", when: this.Animation?.Type is DialogAnimationType.FadeIn)
		.Build();
	}

	private void ResolveDialogClass() {
		this.DialogClass = CssBuilder.Default("dialog")
				.AddClass("dialog-scrollable", when: this.Options.ContentScrollable is true || this.GlobalDialogOptions?.ContentScrollable is true)
				.AddClass("border", when: this.HideHeader is false)
				.AddClassIfNotEmpty(this.OverlayAnimationClass)
			.Build();
	}

	private void ResolveDialogStyle() {

		if (this.customWidth.HasValue() &&
			this.customWidth.Equals("none", StringComparison.OrdinalIgnoreCase) is false) {
			this.DialogStyle = $"width: {this.customWidth};";
		}

		if (this.IsDisplayStatic) {
			return;
		}

		if (this.IsDraggable || this.AssignedPosition == DialogPosition.Center) {
			this.DialogStyle += "top: 50%;left: 50%;transform: translate(-50%, -50%);";
		}

	}


	protected bool onEscapeStopPropagation;
	private async Task HandleContainerKeyDown(KeyboardEventArgs e) {
		this.onEscapeStopPropagation = false;
		var key = e.Code ?? e.Key;
		if (key == "Escape") {
			this.onEscapeStopPropagation = true;
			await this.CancelAsync();
			return;
		}
	}

	private async Task HandleBackgroundClick() {
		if (this.BackgroundCancel is true) {
			await this.CancelAsync();
		}
	}

	private static readonly Dictionary<DialogPosition, Action<DialogManager>> PositionIncrementActions = new Dictionary<DialogPosition, Action<DialogManager>> {
		{ DialogPosition.Center, m => m.DialogsCentered++ },
		{ DialogPosition.CenterLeft, m => m.DialogsCenteredLeft++ },
		{ DialogPosition.CenterRight, m => m.DialogsCenteredRight++ },
		{ DialogPosition.Top, m => m.DialogsTop++ },
		{ DialogPosition.TopLeft, m => m.DialogsTopLeft++ },
		{ DialogPosition.TopRight, m => m.DialogsTopRight++ },
		{ DialogPosition.Bottom, m => m.DialogsBottom++ },
		{ DialogPosition.BottomLeft, m => m.DialogsBottomLeft++ },
		{ DialogPosition.BottomRight, m => m.DialogsBottomRight++ }
	};
	private static readonly Dictionary<DialogPosition, Action<DialogManager>> PositionDecrementActions = new Dictionary<DialogPosition, Action<DialogManager>> {
		{ DialogPosition.Center, m => m.DialogsCentered-- },
		{ DialogPosition.CenterLeft, m => m.DialogsCenteredLeft-- },
		{ DialogPosition.CenterRight, m => m.DialogsCenteredRight-- },
		{ DialogPosition.Top, m => m.DialogsTop-- },
		{ DialogPosition.TopLeft, m => m.DialogsTopLeft-- },
		{ DialogPosition.TopRight, m => m.DialogsTopRight-- },
		{ DialogPosition.Bottom, m => m.DialogsBottom-- },
		{ DialogPosition.BottomLeft, m => m.DialogsBottomLeft-- },
		{ DialogPosition.BottomRight, m => m.DialogsBottomRight-- }
	};

	/// <inheritdoc/>
	protected override void OnInitialized() => this.Configure();

	/// <inheritdoc/>
	protected override void OnAfterRender(bool firstRender) {
		if (firstRender) {

			this._closeBtnAttributes.Clear();

			if (this.IsDisplayStatic || this.Manager is null) {
				return;
			}

			if (this.IsNested) {
				this.Manager.AdjustNestedPosition((int)this.AssignedPosition, this.AssignedPositionCount - 1, this.dialogInstanceElementRef);
			}

			if (this.IsDraggable && (this.dialogInstanceElementRef.Context is not null)) {
				this._hasDraggability = this.HideHeader
					? this.Manager.AddDraggable(this.Id)
					: this.Manager.AddDraggable(this.Id, ".dialog-header");
			}

			if (this.IsModal) {
				this.Manager.OnModalShown();
			}

		}
	}

	void IDisposable.Dispose() {
		this.RemoveDraggability();
	}

}