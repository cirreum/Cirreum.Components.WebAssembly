namespace Cirreum.Components;

using Microsoft.AspNetCore.Components;

public partial interface IDialogService {

	/// <summary>
	/// Shows an Alert dialog containing the <typeparamref name="TComponent"/> component.
	/// </summary>
	IDialogReference ShowAlert<TComponent>() where TComponent : IComponent;
	/// <summary>
	/// Shows an Alert dialog containing the <typeparamref name="TComponent"/> component with the specified
	/// <paramref name="parameters"/>.
	/// </summary>
	/// <param name="parameters">Key/Value collection of the parameters to apply to the <typeparamref name="TComponent"/> component being displayed.</param>
	IDialogReference ShowAlert<TComponent>(ComponentParameters parameters) where TComponent : IComponent;


	/// <summary>
	/// Shows an Alerted dialog containing the <paramref name="component"/>.
	/// </summary>
	/// <param name="component">Type of component to display.</param>
	IDialogReference ShowAlert(Type component);
	/// <summary>
	/// Shows an Alerted dialog containing the <paramref name="component"/> with the specified <paramref name="parameters"/>.
	/// </summary>
	/// <param name="component">Type of component to display.</param>
	/// <param name="parameters">Key/Value collection of the parameters to apply to the <paramref name="component"/> being displayed.</param>
	IDialogReference ShowAlert(Type component, ComponentParameters parameters);


	/// <summary>
	/// Shows the <see cref="AlertPrompt"/> with the message, color and icon.
	/// </summary>
	/// <param name="message">The alert message.</param>
	/// <param name="color">The alert color. Default: <see cref="BackgroundColor.Info"/></param>
	/// <param name="icon">The alert icon using the Bootstrap name (excluding the bi-). Default: info-circle-fill</param>
	IDialogReference ShowAlert(string message, BackgroundColor color = BackgroundColor.Info, string icon = "info-circle-fill");

}