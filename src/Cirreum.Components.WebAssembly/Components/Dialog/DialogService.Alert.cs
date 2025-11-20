namespace Cirreum.Components;

using Microsoft.AspNetCore.Components;

internal sealed partial class DialogService : IDialogServiceInternal {

	/// <inheritdoc/>
	public IDialogReference ShowAlert<TComponent>() where TComponent : IComponent {
		return this.Show<TComponent>(string.Empty, [], DialogOptions.AsAlert);
	}
	/// <inheritdoc/>
	public IDialogReference ShowAlert<TComponent>(ComponentParameters parameters) where TComponent : IComponent {
		return this.Show<TComponent>("", parameters, DialogOptions.AsAlert);
	}

	/// <inheritdoc/>
	public IDialogReference ShowAlert(Type component) {
		return this.Show(component, string.Empty, [], DialogOptions.AsAlert);
	}
	/// <inheritdoc/>
	public IDialogReference ShowAlert(Type component, ComponentParameters parameters) {
		return this.Show(component, "", parameters, DialogOptions.AsAlert);
	}


	/// <inheritdoc/>
	public IDialogReference ShowAlert(string message, BackgroundColor color = BackgroundColor.Info, string icon = "info-circle-fill") {
		var parameters = new ComponentParameters {
			{ nameof(AlertPrompt.Message), message },
			{ nameof(AlertPrompt.Color), color },
			{ nameof(AlertPrompt.Icon), icon }
		};
		return this.Show<AlertPrompt>("", parameters, DialogOptions.AsAlert);
	}

}