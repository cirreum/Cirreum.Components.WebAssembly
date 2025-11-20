namespace Cirreum.Components;

using Microsoft.AspNetCore.Components;

internal sealed partial class DialogService : IDialogServiceInternal {

	/// <inheritdoc/>
	public IDialogReference ShowPrompt<TComponent>() where TComponent : IComponent {
		return this.Show<TComponent>(string.Empty, [], DialogOptions.AsPrompt);
	}

	/// <inheritdoc/>
	public IDialogReference ShowPrompt<TComponent>(StringOrRenderFragment title) where TComponent : IComponent {
		return this.Show<TComponent>(title, [], DialogOptions.AsPrompt);
	}

	/// <inheritdoc/>
	public IDialogReference ShowPrompt<TComponent>(StringOrRenderFragment title, ComponentParameters parameters) where TComponent : IComponent {
		return this.Show<TComponent>(title, parameters, DialogOptions.AsPrompt);
	}


	/// <inheritdoc/>
	public IDialogReference ShowPrompt(Type component) {
		return this.Show(component, string.Empty, [], DialogOptions.AsPrompt);
	}

	/// <inheritdoc/>
	public IDialogReference ShowPrompt(Type component, StringOrRenderFragment title) {
		return this.Show(component, title, [], DialogOptions.AsPrompt);
	}

	/// <inheritdoc/>
	public IDialogReference ShowPrompt(Type component, StringOrRenderFragment title, ComponentParameters parameters) {
		return this.Show(component, title, parameters, DialogOptions.AsPrompt);
	}

}