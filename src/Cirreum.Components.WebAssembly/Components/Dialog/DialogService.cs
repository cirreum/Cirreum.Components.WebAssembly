namespace Cirreum.Components;

using Microsoft.AspNetCore.Components;
using System;
using System.Diagnostics.CodeAnalysis;

[method: DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(DialogAnimation))]
[method: DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(DialogAnimationType))]
[method: DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(DialogOptions))]
[method: DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ComponentParameters))]
[method: DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(DialogPosition))]
[method: DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(DialogReference))]
internal sealed partial class DialogService() : IDialogServiceInternal {

	public event Action<DialogReference>? OnDialogInstanceAdded;

	public event Action<DialogReference, DialogResult>? OnDialogCloseRequested;

	void IDialogServiceInternal.Close(DialogReference dialog) {
		OnDialogCloseRequested?.Invoke(dialog, DialogResult.Ok());
	}

	void IDialogServiceInternal.Close(DialogReference dialog, DialogResult result) {
		OnDialogCloseRequested?.Invoke(dialog, result);
	}

	/// <inheritdoc/>
	public IDialogReference Show<T>() where T : IComponent {
		return this.Show<T>(string.Empty, [], new DialogOptions());
	}

	/// <inheritdoc/>
	public IDialogReference Show<T>(StringOrRenderFragment title) where T : IComponent {
		return this.Show<T>(title, [], new DialogOptions());
	}

	/// <inheritdoc/>
	public IDialogReference Show<T>(StringOrRenderFragment title, DialogOptions options) where T : IComponent {
		return this.Show<T>(title, [], options);
	}

	/// <inheritdoc/>
	public IDialogReference Show<T>(StringOrRenderFragment title, ComponentParameters parameters) where T : IComponent {
		return this.Show<T>(title, parameters, new DialogOptions());
	}

	/// <inheritdoc/>
	public IDialogReference Show<T>(StringOrRenderFragment title, ComponentParameters parameters, DialogOptions options) where T : IComponent {
		return this.Show(typeof(T), title, parameters, options);
	}


	/// <inheritdoc/>
	public IDialogReference Show(Type component) {
		return this.Show(component, string.Empty, [], new DialogOptions());
	}

	/// <inheritdoc/>
	public IDialogReference Show(Type component, StringOrRenderFragment title) {
		return this.Show(component, title, [], new DialogOptions());
	}

	/// <inheritdoc/>
	public IDialogReference Show(Type component, StringOrRenderFragment title, DialogOptions options) {
		return this.Show(component, title, [], options);
	}

	/// <inheritdoc/>
	public IDialogReference Show(Type component, StringOrRenderFragment title, ComponentParameters parameters) {
		return this.Show(component, title, parameters, new DialogOptions());
	}

	/// <inheritdoc/>
	public IDialogReference Show(Type component, StringOrRenderFragment title, ComponentParameters parameters, DialogOptions options) {

		if (typeof(IComponent).IsAssignableFrom(component) is false) {
			throw new ArgumentException($"{component.FullName} must be a Component");
		}

		var hash = 17;
		hash = hash * 23 + parameters.GetHashCode();
		hash = hash * 23 + options.GetHashCode();
		var key = $"{component.Name}_{hash}";

		var dialogContent = new RenderFragment(builder => {
			builder.OpenComponent(0, component);
			builder.AddMultipleAttributes(1, parameters.GetDictionary());
			builder.CloseComponent();
		});

		return this.Show(key, dialogContent, title, options);

	}


	/// <inheritdoc/>
	public IDialogReference Show(RenderFragment dialogContent, StringOrRenderFragment title, DialogOptions options) {
		var key = IdGenerator.Next;
		return this.Show(key, dialogContent, title, options);
	}

	private DialogReference Show(string key, RenderFragment dialogContent, StringOrRenderFragment title, DialogOptions dialogOptions) {

		DialogReference? dialogReference = null;
		var dialogId = IdGenerator.Next;
		dialogOptions.TitleOrFragment = title;
		dialogOptions.Visible = true;
		var dialogInstanceFragment = new RenderFragment(builder => {
			builder.OpenComponent<Dialog>(0);
			builder.SetKey(key);
			builder.AddAttribute(1, "Id", dialogId);
			builder.AddAttribute(2, "ChildContent", dialogContent);
			builder.AddAttribute(3, "Options", dialogOptions);
			builder.AddComponentReferenceCapture(4, compRef => dialogReference!.Instance = (Dialog)compRef);
			builder.CloseComponent();
		});

		dialogReference = new DialogReference(dialogId, dialogInstanceFragment, this);

		this.OnDialogInstanceAdded?.Invoke(dialogReference);

		return dialogReference;

	}

}