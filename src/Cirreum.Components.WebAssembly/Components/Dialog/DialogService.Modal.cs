namespace Cirreum.Components;

using Microsoft.AspNetCore.Components;

internal sealed partial class DialogService : IDialogServiceInternal {

	/// <inheritdoc/>
	public IDialogReference ShowModal<TComponent>() where TComponent : IComponent {
		return this.ShowModalPrivate<TComponent>();
	}

	/// <inheritdoc/>
	public IDialogReference ShowModal<TComponent>(StringOrRenderFragment title) where TComponent : IComponent {
		return this.ShowModalPrivate<TComponent>(title);
	}

	/// <inheritdoc/>
	public IDialogReference ShowModal<TComponent>(StringOrRenderFragment title, string? width = null, DialogSize? maxWidth = null, bool scrollable = false) where TComponent : IComponent {
		return this.ShowModalPrivate<TComponent>(title, null, width, maxWidth, scrollable);
	}

	/// <inheritdoc/>
	public IDialogReference ShowModal<TComponent>(StringOrRenderFragment title, ComponentParameters parameters) where TComponent : IComponent {
		return this.ShowModalPrivate<TComponent>(title, parameters);
	}

	/// <inheritdoc/>
	public IDialogReference ShowModal<TComponent>(StringOrRenderFragment title, ComponentParameters parameters, string? width = null, DialogSize? maxWidth = null, bool scrollable = false) where TComponent : IComponent {
		return this.ShowModalPrivate<TComponent>(title, parameters, width, maxWidth, scrollable);
	}

	private IDialogReference ShowModalPrivate<TComponent>(StringOrRenderFragment? title = null, ComponentParameters? parameters = null, string? width = null, DialogSize? maxWidth = null, bool scrollable = false) where TComponent : IComponent {
		var opt = DialogOptions.AsModal;
		if (width is not null) {
			opt.WithFixedWidth(width);
		}
		if (maxWidth is not null) {
			opt.WithMaxWidth(maxWidth.Value);
		}
		if (scrollable) {
			opt.WithScrollability();
		}
		if (parameters is null) {
			return this.Show<TComponent>(title ?? string.Empty, opt);
		}
		return this.Show<TComponent>(title ?? string.Empty, parameters, opt);
	}


	/// <inheritdoc/>
	public IDialogReference ShowModal(Type component) {
		return this.Show(component, string.Empty, [], DialogOptions.AsModal);
	}

	/// <inheritdoc/>
	public IDialogReference ShowModal(Type component, StringOrRenderFragment title) {
		return this.Show(component, title, [], DialogOptions.AsModal);
	}

	/// <inheritdoc/>
	public IDialogReference ShowModal(Type component, StringOrRenderFragment title, ComponentParameters parameters) {
		return this.Show(component, title, parameters, DialogOptions.AsModal);
	}

}