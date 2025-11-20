namespace Cirreum.Components;

public class DialogOptions {

	/// <summary>
	/// Gets or sets if a dialog is initially visible or hidden.
	/// </summary>
	public bool? Visible { get; set; }

	/// <summary>
	/// Useful to display a Dialog component inline like any other component
	/// instead of as a popup.
	/// </summary>
	/// <remarks>
	/// NOTE: This overrides and disables the <see cref="Modal"/> option.
	/// </remarks>
	public bool? DisplayStatic { get; set; }

	/// <summary>
	/// Determines if the Dialog covers the other page content with a semi-transparent overlay.
	/// </summary>
	/// <remarks>
	/// If <see cref="Modal"/> is true, then <see cref="UseFocusTrap"/> will always be true.
	/// </remarks>
	public bool? Modal { get; set; }

	/// <summary>
	/// The desired position (location) when initially rendered. Default: <see cref="DialogPosition.Center"/>
	/// </summary>
	/// <remarks>
	/// See: <see cref="DialogPosition"/> for more information.
	/// </remarks>
	public DialogPosition? Position { get; set; }

	/// <summary>
	/// What type of animation to use when showing and hiding your dialog.
	/// </summary>
	public DialogAnimation? Animation { get; set; }

	/// <summary>
	/// Retain focus only within your dialog.
	/// </summary>
	/// <remarks>
	/// If <see cref="Modal"/> is true, then <see cref="UseFocusTrap"/> will always be true.
	/// </remarks>
	public bool? UseFocusTrap {
		get {
			return (this._useFocusTrap ?? false) || (this.Modal ?? false);
		}
		set {
			_useFocusTrap = value;
		}
	}
	bool? _useFocusTrap;

	/// <summary>
	/// Which element is focused, when <see cref="UseFocusTrap"/>
	/// is enabled.
	/// </summary>
	public DefaultFocusType? DefaultFocus { get; set; }

	/// <summary>
	/// When <see cref="Modal"/> is <see langword="true"/>, sets if a Dialog will close when the user
	/// clicks on the modal overlay that covers the rest of the page content.
	/// </summary>
	/// <remarks>
	/// If <see cref="Modal"/> is <see langword="true"/>, and this is <see langword="false"/> and <see cref="HideHeader"/>
	/// or <see cref="HideCloseButton"/> are <see langword="true"/>
	/// the user will have no way to dismiss the dialog.
	/// </remarks>
	public bool? BackgroundCancel { get; set; }

	/// <summary>
	/// When <see langword="true"/>, allows the dialog to be draggable.
	/// </summary>
	/// <remarks>
	/// <para>
	/// If <see cref="HideHeader"/> is <see langword="true"/>, then the entire dialog
	/// becomes the handle for dragging. When <see langword="false"/>, only the header
	/// is enabled for dragging.
	/// </para>
	/// <para>
	/// Also, when <see langword="true"/>, <see cref="Position"/> value is ignored
	/// and the dialog will always be displayed centered, or offset if nested.
	/// </para>
	/// </remarks>
	public bool? Draggable { get; set; }

	/// <summary>
	/// When <see langword="true"/>, hides the header/title bar.
	/// </summary>
	public bool? HideHeader { get; set; }

	/// <summary>
	/// When <see langword="true"/>, hides the header/title bar's close button.
	/// </summary>
	public bool? HideCloseButton { get; set; }

	internal StringOrRenderFragment? TitleOrFragment { get; set; } = "";

	/// <summary>
	/// The optional icon to display.
	/// </summary>
	/// <remarks>
	/// This should be a class. Example: bi bi-person
	/// </remarks>
	public string TitleIcon { get; set; } = "";

	/// <summary>
	/// Should your content support scrolling.
	/// </summary>
	public bool? ContentScrollable { get; set; }

	/// <summary>
	/// The optional desired width. Must be a valid CSS width value like '600px' or '3em'
	/// </summary>
	public string? DialogWidth { get; set; }

	/// <summary>
	/// The optional max-width
	/// </summary>
	public DialogSize? DialogSizing { get; set; } = DialogSize.Default;


	/// <summary>
	/// A new <see cref="DialogAnimation"/> with FadeInOut = 0.25s
	/// </summary>
	public static DialogAnimation StandardAnimation => DialogAnimation.FadeInOut(0.25);

	/// <summary>
	/// A convenient default set of options.
	/// </summary>
	/// <remarks>
	/// Uses the <see cref="StandardAnimation"/> and
	/// <see cref="DialogPosition.Center"/>.
	/// </remarks>
	public static DialogOptions Default =>
		new DialogOptions() {
			Animation = StandardAnimation,
			Position = DialogPosition.Center
		};

	/// <summary>
	/// A convenient default set of options for a Modal Dialog with no animation.
	/// </summary>
	/// <remarks>
	/// <see cref="DialogPosition.Center"/>, <see cref="UseFocusTrap"/> set to <see langword="true"/>, 
	/// <see cref="Modal"/> set to <see langword="true"/>.
	/// and <see cref="BackgroundCancel"/> set to <see langword="true"/>.
	/// </remarks>
	public static DialogOptions NoAnimation =>
		new DialogOptions() {
			Animation = DialogAnimation.None(),
			Position = DialogPosition.Center,
			UseFocusTrap = true,
			Modal = true,
			BackgroundCancel = true
		};

	/// <summary>
	/// Uses the <see cref="StandardAnimation"/>, <see cref="DialogPosition.Center"/>,
	/// <see cref="UseFocusTrap"/> set to <see langword="true"/> and 
	/// <see cref="HideCloseButton"/> set to <see langword="true"/>.
	/// </summary>
	public static DialogOptions NoCloseButton =>
		new DialogOptions() {
			Animation = StandardAnimation,
			Position = DialogPosition.Center,
			UseFocusTrap = true,
			HideCloseButton = true
		};

	/// <summary>
	/// Uses the <see cref="StandardAnimation"/>, <see cref="DialogPosition.Center"/>,
	/// <see cref="HideHeader"/> set to <see langword="true"/>.
	/// </summary>
	public static DialogOptions NoHeader =>
		new DialogOptions() {
			Animation = StandardAnimation,
			Position = DialogPosition.Center,
			HideHeader = true
		};

	/// <summary>
	/// A convenient set of options for displaying a Dialog inline (not a window or popup)
	/// with <see cref="Animation"/> set to <see cref="DialogAnimation.None"/> and
	/// <see cref="DisplayStatic"/> set to <see langword="true"/>, <see cref="BackgroundCancel"/>
	/// set to <see langword="false"/>, and <see cref="HideCloseButton"/> set to <see langword="true"/>.
	/// </summary>
	public static DialogOptions AsStaticDisplay =>
		new DialogOptions() {
			Animation = DialogAnimation.None(),
			DisplayStatic = true,
			Modal = false,
			BackgroundCancel = false,
			HideCloseButton = true
		};


	/// <summary>
	/// Uses the <see cref="StandardAnimation"/>, <see cref="DialogPosition.Top"/>,
	/// <see cref="Modal"/> set to <see langword="true"/> and
	/// <see cref="HideHeader"/> set to <see langword="true"/>, and <see cref="BackgroundCancel"/>
	/// set to <see langword="false"/>
	/// </summary>
	public static DialogOptions AsAlert =>
		new DialogOptions() {
			Animation = StandardAnimation,
			Position = DialogPosition.Top,
			Modal = true,
			HideHeader = true,
			BackgroundCancel = false
		};

	/// <summary>
	/// Uses the <see cref="StandardAnimation"/>, <see cref="DialogPosition.Top"/> and
	/// <see cref="Modal"/> set to <see langword="true"/>, and <see cref="BackgroundCancel"/>
	/// set to <see langword="false"/>
	/// </summary>
	public static DialogOptions AsPrompt =>
		new DialogOptions() {
			Animation = StandardAnimation,
			Position = DialogPosition.Top,
			Modal = true,
			BackgroundCancel = false
		};

	/// <summary>
	/// Uses the <see cref="StandardAnimation"/>, <see cref="DialogPosition.Center"/>,
	/// <see cref="Modal"/> set to <see langword="true"/> and
	/// <see cref="BackgroundCancel"/> set to <see langword="true"/>.
	/// </summary>
	public static DialogOptions AsModal =>
		new DialogOptions() {
			Animation = StandardAnimation,
			Position = DialogPosition.Center,
			Modal = true,
			BackgroundCancel = true
		};

	/// <summary>
	/// <see cref="AsModal"/> with <see cref="ContentScrollable"/> set to <see langword="true"/>.
	/// </summary>
	public static DialogOptions AsScrollableModal => AsModal.WithScrollability();

	/// <summary>
	/// <see cref="AsModal"/> with <see cref="Draggable"/> set to <see langword="true"/>.
	/// </summary>
	public static DialogOptions AsWindow => AsModal.WithDraggability();

	/// <summary>
	/// <see cref="AsModal"/> with <see cref="Draggable"/> set to <see langword="true"/> and
	/// <see cref="ContentScrollable"/> set to <see langword="true"/>.
	/// </summary>
	public static DialogOptions AsScrollableWindow => AsWindow.WithScrollability();

}