namespace Cirreum.Components;

using Microsoft.AspNetCore.Components;

internal sealed partial class DialogService {

	/// <inheritdoc/>
	public IDialogReference ShowStatic<TComponent>() where TComponent : IComponent {
		return this.Show<TComponent>(string.Empty, [], DialogOptions.AsStaticDisplay);
	}

	/// <inheritdoc/>
	public IDialogReference ShowStatic<TComponent>(StringOrRenderFragment title) where TComponent : IComponent {
		return this.Show<TComponent>(title, [], DialogOptions.AsStaticDisplay);
	}

	/// <inheritdoc/>
	public IDialogReference ShowStatic<TComponent>(StringOrRenderFragment title, ComponentParameters parameters) where TComponent : IComponent {
		return this.Show<TComponent>(title, parameters, DialogOptions.AsStaticDisplay);
	}


	/// <inheritdoc/>
	public IDialogReference ShowStatic(Type component) {
		return this.Show(component, string.Empty, [], DialogOptions.AsStaticDisplay);
	}

	/// <inheritdoc/>
	public IDialogReference ShowStatic(Type component, StringOrRenderFragment title) {
		return this.Show(component, title, [], DialogOptions.AsStaticDisplay);
	}

	/// <inheritdoc/>
	public IDialogReference ShowStatic(Type component, StringOrRenderFragment title, ComponentParameters parameters) {
		return this.Show(component, title, parameters, DialogOptions.AsStaticDisplay);
	}

}