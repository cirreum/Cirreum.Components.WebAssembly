namespace Cirreum.Components;

using Microsoft.AspNetCore.Components;

public partial interface IDialogService {

	/// <summary>
	/// Shows a dialog containing the <typeparamref name="TComponent"/> component.
	/// </summary>
	IDialogReference Show<TComponent>() where TComponent : IComponent;

	/// <summary>
	/// Shows a dialog containing the <typeparamref name="TComponent"/> component with the specified <paramref name="title"/> .
	/// </summary>
	/// <param name="title">Dialog title</param>
	IDialogReference Show<TComponent>(StringOrRenderFragment title) where TComponent : IComponent;

	/// <summary>
	/// Shows a dialog containing the <typeparamref name="TComponent"/> component with the specified <paramref name="title"/> and <paramref name="options"/>.
	/// </summary>
	/// <param name="title">Dialog title</param>
	/// <param name="options">Options to configure the dialog</param>
	IDialogReference Show<TComponent>(StringOrRenderFragment title, DialogOptions options) where TComponent : IComponent;

	/// <summary>
	/// Shows a dialog containing the <typeparamref name="TComponent"/> component with the specified <paramref name="title"/> and <paramref name="parameters"/>.
	/// </summary>
	/// <param name="title">Dialog title</param>
	/// <param name="parameters">Key/Value collection of the parameters to apply to the <typeparamref name="TComponent"/> component being displayed.</param>
	IDialogReference Show<TComponent>(StringOrRenderFragment title, ComponentParameters parameters) where TComponent : IComponent;

	/// <summary>
	/// Shows a dialog containing the <typeparamref name="TComponent"/> component with the specified <paramref name="title"/>,
	/// <paramref name="parameters"/> and <paramref name="options"/>.
	/// </summary>
	/// <param name="title">Dialog title.</param>
	/// <param name="parameters">Key/Value collection of the parameters to apply to the <typeparamref name="TComponent"/> component being displayed.</param>
	/// <param name="options">Options to configure the dialog.</param>
	IDialogReference Show<TComponent>(StringOrRenderFragment title, ComponentParameters parameters, DialogOptions options) where TComponent : IComponent;


	/// <summary>
	/// Shows a dialog containing the <paramref name="component"/>.
	/// </summary>
	/// <param name="component">Type of component to display.</param>
	IDialogReference Show(Type component);

	/// <summary>
	/// Shows a dialog containing the <paramref name="component"/> with the specified <paramref name="title"/>.
	/// </summary>
	/// <param name="component">Type of component to display.</param>
	/// <param name="title">Dialog title.</param>
	IDialogReference Show(Type component, StringOrRenderFragment title);

	/// <summary>
	/// Shows a dialog containing the <paramref name="component"/> with the specified <paramref name="title"/> and <paramref name="options"/>.
	/// </summary>
	/// <param name="component">Type of component to display.</param>
	/// <param name="title">Dialog title.</param>
	/// <param name="options">Options to configure the dialog.</param>
	IDialogReference Show(Type component, StringOrRenderFragment title, DialogOptions options);

	/// <summary>
	/// Shows a dialog containing the <paramref name="component"/> with the specified <paramref name="title"/> and <paramref name="parameters"/>.
	/// </summary>
	/// <param name="component">Type of component to display.</param>
	/// <param name="title">Dialog title.</param>
	/// <param name="parameters">Key/Value collection of the parameters to apply to the <paramref name="component"/> being displayed.</param>
	IDialogReference Show(Type component, StringOrRenderFragment title, ComponentParameters parameters);

	/// <summary>
	/// Shows a dialog containing the <paramref name="component"/> with the specified <paramref name="title"/>, <paramref name="parameters"/>
	/// and <paramref name="options"/>.
	/// </summary>
	/// <param name="component">Type of component to display.</param>
	/// <param name="title">Dialog title.</param>
	/// <param name="parameters">Key/Value collection of the parameters to apply to the <paramref name="component"/> being displayed.</param>
	/// <param name="options">Options to configure the dialog.</param>
	IDialogReference Show(Type component, StringOrRenderFragment title, ComponentParameters parameters, DialogOptions options);



	/// <summary>
	/// Shows a dialog containing the <paramref name="dialogContent"/> <see cref="RenderFragment"/> with the specified 
	/// <paramref name="title"/> and <paramref name="options"/>.
	/// </summary>
	/// <param name="dialogContent">The <see cref="RenderFragment"/> to render as the content of the dialog.</param>
	/// <param name="title">Dialog title.</param>
	/// <param name="options">Options to configure the dialog.</param>
	IDialogReference Show(RenderFragment dialogContent, StringOrRenderFragment title, DialogOptions options);

}