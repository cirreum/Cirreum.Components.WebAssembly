namespace Cirreum.Components;

using Microsoft.AspNetCore.Components;

public partial interface IDialogService {

	/// <summary>
	/// Shows a Modal dialog containing the <typeparamref name="TComponent"/> component.
	/// </summary>
	IDialogReference ShowModal<TComponent>() where TComponent : IComponent;

	/// <summary>
	/// Shows a Modal dialog containing the <typeparamref name="TComponent"/> component with the specified <paramref name="title"/>.
	/// </summary>
	/// <param name="title">Dialog title.</param>
	IDialogReference ShowModal<TComponent>(StringOrRenderFragment title) where TComponent : IComponent;

	/// <summary>
	/// Shows a Modal dialog containing the <typeparamref name="TComponent"/> component with the specified <paramref name="title"/>.
	/// </summary>
	/// <param name="title">Dialog title.</param>
	/// <param name="width">The optional desired width. Must be a valid CSS width value like '600px' or '3em'.</param>
	/// <param name="maxWidth">The optional max-width, see <see cref="DialogSize"/>.</param>
	/// <param name="scrollable">Is the dialog content vertically scrollable.</param>
	IDialogReference ShowModal<TComponent>(StringOrRenderFragment title, string? width = null, DialogSize? maxWidth = null, bool scrollable = false) where TComponent : IComponent;

	/// <summary>
	/// Shows a Modal dialog containing the <typeparamref name="TComponent"/> component with the specified <paramref name="title"/>,
	/// <paramref name="parameters"/>.
	/// </summary>
	/// <param name="title">Dialog title.</param>
	/// <param name="parameters">Key/Value collection of the parameters to apply to the <typeparamref name="TComponent"/> component being displayed.</param>
	IDialogReference ShowModal<TComponent>(StringOrRenderFragment title, ComponentParameters parameters) where TComponent : IComponent;

	/// <summary>
	/// Shows a Modal dialog containing the <typeparamref name="TComponent"/> component with the specified <paramref name="title"/>,
	/// <paramref name="parameters"/>.
	/// </summary>
	/// <param name="title">Dialog title.</param>
	/// <param name="parameters">Key/Value collection of the parameters to apply to the <typeparamref name="TComponent"/> component being displayed.</param>
	/// <param name="width">The optional desired width. Must be a valid CSS width value like '600px' or '3em'.</param>
	/// <param name="maxWidth">The optional max-width, see <see cref="DialogSize"/>.</param>
	/// <param name="scrollable">Is the dialog content vertically scrollable.</param>
	IDialogReference ShowModal<TComponent>(StringOrRenderFragment title, ComponentParameters parameters, string? width = null, DialogSize? maxWidth = null, bool scrollable = false) where TComponent : IComponent;


	/// <summary>
	/// Shows a Modaled dialog containing the <paramref name="component"/>.
	/// </summary>
	/// <param name="component">Type of component to display.</param>
	IDialogReference ShowModal(Type component);

	/// <summary>
	/// Shows a Modaled dialog containing the <paramref name="component"/> with the specified <paramref name="title"/>.
	/// </summary>
	/// <param name="component">Type of component to display.</param>
	/// <param name="title">Dialog title.</param>
	IDialogReference ShowModal(Type component, StringOrRenderFragment title);

	/// <summary>
	/// Shows a Modaled dialog containing the <paramref name="component"/> with the specified <paramref name="title"/> and <paramref name="parameters"/>.
	/// </summary>
	/// <param name="component">Type of component to display.</param>
	/// <param name="title">Dialog title.</param>
	/// <param name="parameters">Key/Value collection of the parameters to apply to the <paramref name="component"/> being displayed.</param>
	IDialogReference ShowModal(Type component, StringOrRenderFragment title, ComponentParameters parameters);

}