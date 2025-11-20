namespace Cirreum.Components;

public static class DialogOptionsExtensions {

	public static DialogOptions WithDisplayStatic(this DialogOptions options) {
		if (options.Modal is true) {
			throw new ArgumentException("Cannot set DisplayStatic to true, if Modal is also true.");
		}
		options.DisplayStatic = true;
		return options;
	}
	public static DialogOptions WithoutDisplayStatic(this DialogOptions options) {
		options.DisplayStatic = false;
		return options;
	}

	public static DialogOptions WithModal(this DialogOptions options) {
		if (options.DisplayStatic is true) {
			throw new ArgumentException("Cannot set Modal to true, if DisplayStatic is also true.");
		}
		options.Modal = true;
		return options;
	}
	public static DialogOptions WithoutModal(this DialogOptions options) {
		options.Modal = false;
		return options;
	}

	public static DialogOptions WithPosition(this DialogOptions options, DialogPosition position) {
		options.Position = position;
		return options;
	}

	public static DialogOptions WithAnimation(this DialogOptions options, DialogAnimation dialogAnimation) {
		options.Animation = dialogAnimation;
		return options;
	}

	public static DialogOptions WithFocusTrap(this DialogOptions options) {
		options.UseFocusTrap = true;
		return options;
	}
	public static DialogOptions WithoutFocusTrap(this DialogOptions options) {
		options.UseFocusTrap = false;
		return options;
	}

	public static DialogOptions WithBackgroundCancel(this DialogOptions options) {
		options.BackgroundCancel = true;
		return options;
	}
	public static DialogOptions WithoutBackgroundCancel(this DialogOptions options) {
		options.BackgroundCancel = false;
		return options;
	}

	public static DialogOptions WithDraggability(this DialogOptions options, DefaultFocusType defaulFocusType = DefaultFocusType.Default) {
		options.Draggable = true;
		options.DefaultFocus = defaulFocusType;
		return options;
	}
	public static DialogOptions WithoutDraggability(this DialogOptions options) {
		options.Draggable = false;
		options.DefaultFocus = DefaultFocusType.Default;
		return options;
	}

	public static DialogOptions WithHeader(this DialogOptions options) {
		options.HideHeader = false;
		return options;
	}
	public static DialogOptions WithoutHeader(this DialogOptions options) {
		options.HideHeader = true;
		return options;
	}

	public static DialogOptions WithCloseButton(this DialogOptions options) {
		options.HideCloseButton = false;
		return options;
	}
	public static DialogOptions WithoutCloseButton(this DialogOptions options) {
		options.HideCloseButton = true;
		return options;
	}

	public static DialogOptions WithTitleIcon(this DialogOptions options, string iconClass) {
		options.TitleIcon = iconClass;
		return options;
	}
	public static DialogOptions WithoutTitleIcon(this DialogOptions options) {
		options.TitleIcon = "";
		return options;
	}

	public static DialogOptions WithScrollability(this DialogOptions options) {
		options.ContentScrollable = true;
		return options;
	}
	public static DialogOptions WithoutScrollability(this DialogOptions options) {
		options.ContentScrollable = false;
		return options;
	}

	public static DialogOptions WithFixedWidth(this DialogOptions options, string width) {
		options.DialogWidth = width;
		return options;
	}
	public static DialogOptions WithoutFixedWidth(this DialogOptions options) {
		options.DialogWidth = null;
		return options;
	}

	public static DialogOptions WithMaxWidth(this DialogOptions options, DialogSize sizing) {
		options.DialogSizing = sizing;
		return options;
	}
	public static DialogOptions WithoutMaxWidth(this DialogOptions options) {
		options.DialogSizing = null;
		return options;
	}

	public static DialogOptions WithVisibility(this DialogOptions options, bool isVisible = true) {
		options.Visible = isVisible;
		return options;
	}
	public static DialogOptions WithoutVisibility(this DialogOptions options) {
		options.Visible = null;
		return options;
	}

}