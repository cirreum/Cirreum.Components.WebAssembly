namespace Cirreum.Components;

using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

/// <summary>
/// Defines a Dropdown component.
/// </summary>
/// <typeparam name="TItemType">The 'Type' of item the dropdown list will represents.</typeparam>
public interface IDropdown<TItemType> : IDisposable {

	/// <summary>
	/// The text to display, if <see cref="TextTemplate"/> is null.
	/// </summary>
	string Text { get; }

	/// <summary>
	/// Gets or sets the optional template to render the text.
	/// </summary>
	RenderFragment? TextTemplate { get; set; }

	/// <summary>
	/// Gets or sets if the dropdown is disabled.
	/// </summary>
	bool IsDisabled { get; set; }

	/// <summary>
	/// Gets or sets if the dropdown can wrap it's text.
	/// </summary>
	bool TextWrap { get; set; }

	/// <summary>
	/// Gets or sets the <see cref="TextColor"/> for the dropdown button.
	/// </summary>
	ButtonColor ButtonColor { get; set; }

	/// <summary>
	/// Gets or sets the dropdown button size.
	/// </summary>
	ButtonSize ButtonSize { get; set; }

	/// <summary>
	/// Gets or sets if the dropdown button is an outline button.
	/// </summary>
	bool ButtonOutlined { get; set; }

	/// <summary>
	/// Gets or sets any additional classes applied to the button.
	/// </summary>
	string ButtonCss { get; set; }

	/// <summary>
	/// Gets or sets any additional styles applied to the button.
	/// </summary>
	string? ButtonStyle { get; set; }


	/// <summary>
	///	The text for the aria-label attribute.
	/// </summary>
	string AriaLabel { get; set; }

	/// <summary>
	/// Toggles the dropdown menu.
	/// </summary>
	/// <returns>An awaitable <see cref="Task"/></returns>
	Task ToggleAsync();

	/// <summary>
	/// Shows the dropdown menu.
	/// </summary>
	/// <returns>An awaitable <see cref="Task"/></returns>
	Task ShowAsync();

	/// <summary>
	/// Hides the dropdown menu.
	/// </summary>
	/// <returns>An awaitable <see cref="Task"/></returns>
	Task HideAsync();

	/// <summary>
	/// This event fires immediately when the show method is called.
	/// </summary>
	EventCallback OnDropdownShow { get; set; }
	/// <summary>
	/// This event is fired when the dropdown has been made visible to the user.
	/// </summary>
	EventCallback OnDropdownShown { get; set; }
	/// <summary>
	/// This event is fired immediately when the hide instance method has been called.
	/// </summary>
	EventCallback OnDropdownHide { get; set; }
	/// <summary>
	/// This event is fired when the dropdown has finished being hidden from the user.
	/// </summary>
	EventCallback OnDropdownHidden { get; set; }


	/// <summary>
	/// Gets or sets the dropdown content item(s).
	/// </summary>
	RenderFragment ChildContent { get; set; }

	/// <summary>
	/// Gets or sets the <see cref="DropdownAlignment"/>
	/// when <see cref="DropdownDisplayType"/> is <see cref="DropdownDisplay.Static"/>.
	/// </summary>
	DropdownAlignment ResponsiveAlignment { get; set; }

	/// <summary>
	/// Gets or sets any additional classes applied to the container div.
	/// </summary>
	string ContainerCss { get; set; }

	/// <summary>
	/// Gets or sets any additional styles applied to the container div.
	/// </summary>
	string? ContainerStyle { get; set; }

	/// <summary>
	/// The minimum width of the dropdown items, in rem.
	/// </summary>
	double DropdownMinWidth { get; set; }

	/// <summary>
	/// Gets or sets the <see cref="DropdownDirection"/> of the menu items.
	/// </summary>
	DropdownDirection DropdownDirection { get; set; }

	/// <summary>
	/// Gets or sets the display type, <see cref="DropdownDisplay.Dynamic"/> or
	/// <see cref="DropdownDisplay.Static"/>.
	/// </summary>
	DropdownDisplay DropdownDisplayType { get; set; }

	/// <summary>
	/// Gets or sets the <see cref="DropdownStaticSize"/> (breakpoints) when <see cref="DropdownDisplayType"/> is <see cref="DropdownDisplay.Static"/>.
	/// </summary>
	ResponsiveSize DropdownStaticSize { get; set; }

	/// <summary>
	/// The selected item's value.
	/// </summary>
	TItemType SelectedValue { get; set; }

	/// <summary>
	/// This event is fired when a dropdown item has been selected by the user.
	/// </summary>
	EventCallback<TItemType> SelectedValueChanged { get; set; }

	/// <summary>
	/// Sets the <see cref="SelectedValue"/> value.
	/// </summary>
	/// <param name="value">The value to set.</param>
	/// <param name="notifyValueChanged">Specify <see langword="true"/>, to call the <see cref="SelectedValueChanged"/> event callback. Default: <see langword="false"/></param>
	/// <returns>An awaitable task.</returns>
	/// <remarks>
	/// If <see cref="IsDropMenu"/> is <see langword="false"/>, will also synchronize the active item.
	/// </remarks>
	Task SetSelectedValue(TItemType value, bool notifyValueChanged = false);

	/// <summary>
	/// Set to <see langword="false"/>, to operate as a combobox selector
	/// </summary>
	/// <remarks>
	/// <para>
	/// Default: <see langword="true"/>
	/// </para>
	/// <para>
	/// By default a dropdown operates as a dropdown menu that does not retain
	/// the active selected item. When set to <see langword="false"/> it will
	/// then operate as dropdown selector and retain the active item.
	/// </para>
	/// </remarks>
	bool IsDropMenu { get; set; }

}