namespace Cirreum.Components;
using Microsoft.AspNetCore.Components;

internal class RadioContext(
	IRadioValueProvider valueProvider,
	RadioContext? parentContext,
	EventCallback<ChangeEventArgs> changeEventCallback) {

	private readonly IRadioValueProvider _valueProvider = valueProvider;

	public RadioContext? ParentContext { get; } = parentContext;
	public EventCallback<ChangeEventArgs> ChangeEventCallback { get; } = changeEventCallback;
	public object? CurrentValue => this._valueProvider.CurrentValue;

	// Mutable properties that may change any time an InputRadioGroup is rendered
	public string? GroupName { get; set; }
	public string? FieldClass { get; set; }

	/// <summary>
	/// Finds an <see cref="RadioContext"/> in the context's ancestors with the matching <paramref name="groupName"/>.
	/// </summary>
	/// <param name="groupName">The group name of the ancestor <see cref="RadioContext"/>.</param>
	/// <returns>The <see cref="RadioContext"/>, or <c>null</c> if none was found.</returns>
	public RadioContext? FindContextInAncestors(string groupName)
		=> string.Equals(this.GroupName, groupName) ? this : this.ParentContext?.FindContextInAncestors(groupName);

}
