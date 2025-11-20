namespace Cirreum.Components;

using Microsoft.AspNetCore.Components;

public partial interface IDialogService {

	/// <summary>
	/// Shows a Static dialog containing the <typeparamref name="TComponent"/> component.
	/// </summary>
	IDialogReference ShowStatic<TComponent>() where TComponent : IComponent;

	/// <summary>
	/// Shows a Static dialog containing the <typeparamref name="TComponent"/> component with the specified <paramref name="title"/>.
	/// </summary>
	/// <param name="title">Dialog title.</param>
	IDialogReference ShowStatic<TComponent>(StringOrRenderFragment title) where TComponent : IComponent;

	/// <summary>
	/// Shows a Static dialog containing the <typeparamref name="TComponent"/> component with the specified <paramref name="title"/>,
	/// <paramref name="parameters"/>.
	/// </summary>
	/// <param name="title">Dialog title.</param>
	/// <param name="parameters">Key/Value collection of the parameters to apply to the <typeparamref name="TComponent"/> component being displayed.</param>
	IDialogReference ShowStatic<TComponent>(StringOrRenderFragment title, ComponentParameters parameters) where TComponent : IComponent;


	/// <summary>
	/// Shows a Staticed dialog containing the <paramref name="component"/>.
	/// </summary>
	/// <param name="component">Type of component to display.</param>
	IDialogReference ShowStatic(Type component);

	/// <summary>
	/// Shows a Staticed dialog containing the <paramref name="component"/> with the specified <paramref name="title"/>.
	/// </summary>
	/// <param name="component">Type of component to display.</param>
	/// <param name="title">Dialog title.</param>
	IDialogReference ShowStatic(Type component, StringOrRenderFragment title);

	/// <summary>
	/// Shows a Staticed dialog containing the <paramref name="component"/> with the specified <paramref name="title"/> and <paramref name="parameters"/>.
	/// </summary>
	/// <param name="component">Type of component to display.</param>
	/// <param name="title">Dialog title.</param>
	/// <param name="parameters">Key/Value collection of the parameters to apply to the <paramref name="component"/> being displayed.</param>
	IDialogReference ShowStatic(Type component, StringOrRenderFragment title, ComponentParameters parameters);

}