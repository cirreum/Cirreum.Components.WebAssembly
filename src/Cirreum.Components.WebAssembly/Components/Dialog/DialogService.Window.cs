namespace Cirreum.Components;

using Microsoft.AspNetCore.Components;

internal sealed partial class DialogService : IDialogServiceInternal {

	/// <inheritdoc/>
	public IDialogReference ShowWindow<TComponent>() where TComponent : IComponent {
		return this.ShowWindowPrivate<TComponent>();
	}

	/// <inheritdoc/>
	public IDialogReference ShowWindow<TComponent>(StringOrRenderFragment title, string? titleIcon = null, string? width = null, DialogSize? maxWidth = null, bool scrollable = false) where TComponent : IComponent {
		return this.ShowWindowPrivate<TComponent>(title, null, titleIcon, width, maxWidth, scrollable);
	}

	/// <inheritdoc/>
	public IDialogReference ShowWindow<TComponent>(StringOrRenderFragment title, ComponentParameters parameters, string? titleIcon = null, string? width = null, DialogSize? maxWidth = null, bool scrollable = false) where TComponent : IComponent {
		return this.ShowWindowPrivate<TComponent>(title, parameters, titleIcon, width, maxWidth, scrollable);
	}

	private IDialogReference ShowWindowPrivate<TComponent>(StringOrRenderFragment? title = null, ComponentParameters? parameters = null, string? titleIcon = null, string? width = null, DialogSize? maxWidth = null, bool scrollable = false) where TComponent : IComponent {
		var opt = DialogOptions.AsWindow;
		if (width is not null) {
			opt.WithFixedWidth(width);
		}
		if (maxWidth is not null) {
			opt.WithMaxWidth(maxWidth.Value);
		}
		if (scrollable) {
			opt.WithScrollability();
		}
		if (titleIcon is not null) {
			opt.WithTitleIcon(titleIcon);
		}
		if (parameters is null) {
			return this.Show<TComponent>(title ?? string.Empty, opt);
		}
		return this.Show<TComponent>(title ?? string.Empty, parameters, opt);
	}


	/// <inheritdoc/>
	public IDialogReference ShowWindow(Type component) {
		return this.Show(component, string.Empty, [], DialogOptions.AsWindow);
	}

	/// <inheritdoc/>
	public IDialogReference ShowWindow(Type component, StringOrRenderFragment title, string? titleIcon = null) {
		var opt = DialogOptions.AsWindow;
		if (titleIcon is not null) {
			opt.WithTitleIcon(titleIcon);
		}
		return this.Show(component, title, [], opt);
	}

	/// <inheritdoc/>
	public IDialogReference ShowWindow(Type component, StringOrRenderFragment title, ComponentParameters parameters, string? titleIcon = null) {
		var opt = DialogOptions.AsWindow;
		if (titleIcon is not null) {
			opt.WithTitleIcon(titleIcon);
		}
		return this.Show(component, title, parameters, opt);
	}

}