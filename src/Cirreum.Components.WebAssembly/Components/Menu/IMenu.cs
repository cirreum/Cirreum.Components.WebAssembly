namespace Cirreum.Components;

using Microsoft.AspNetCore.Components;

/// <summary>
/// Defines the contract for a Menu
/// </summary>
public interface IMenu {

	/// <summary>
	/// Gets or set the ID of the trigger element to use.
	/// </summary>
	string Trigger { get; set; }

	/// <summary>
	/// Gets or sets the action that will open the menu. Default: <see cref="MouseButton.None"/>
	/// </summary>
	/// <remarks>
	/// Possible values are None, Left, Middle, Right, Back, Forward
	/// <para>
	/// See <see cref="MouseButton"/>
	/// </para>
	/// </remarks>
	MouseButton TriggerButton { get; set; }

	/// <summary>
	/// Gets or sets if the menu is anchored below the <see cref="Trigger"/> element
	/// or floating based on the mouse click coordinates. Default: <see langword="false"/>.
	/// </summary>
	bool Anchored { get; set; }

	/// <summary>
	/// One or more menu components.
	/// </summary>
	/// <remarks>
	/// Can be any of the following - <see cref="MenuItem"/> (interactive and nestable), 
	/// <see cref="MenuHeader"/>, <see cref="MenuDivider"/> and <see cref="MenuText"/>
	/// </remarks>
	RenderFragment ChildContent { get; set; }

	/// <summary>
	/// Event callback that is triggered when the menu is about to be shown.
	/// </summary>
	EventCallback OnMenuShow { get; set; }

	/// <summary>
	/// Event callback that is triggered after the menu has been fully shown.
	/// </summary>
	EventCallback OnMenuShown { get; set; }

	/// <summary>
	/// Event callback that is triggered when the menu is about to be hidden.
	/// </summary>
	EventCallback OnMenuHide { get; set; }

	/// <summary>
	/// Event callback that is triggered after the menu has been fully hidden.
	/// </summary>
	EventCallback OnMenuHidden { get; set; }

	/// <summary>
	/// Event fired when the user selects an <see cref="IMenuItem"/>
	/// </summary>
	EventCallback<IMenuItem> OnMenuItemSelected { get; set; }

	/// <summary>
	/// Opens the menu
	/// </summary>
	/// <param name="top">The top/Y position</param>
	/// <param name="left">The left/X position</param>
	/// <returns>An awaitable Task.</returns>
	/// <remarks>
	/// <para>
	/// If <see cref="Anchored"/> is <see langword="true"/>, then <paramref name="top"/>
	/// and <paramref name="left"/> are ignored.
	/// </para>
	/// <para>
	/// If either <paramref name="top"/> or <paramref name="left"/> is equal to -1, then
	/// will render as if <see cref="Anchored"/> was <see langword="true"/>.
	/// </para>
	/// </remarks>
	Task ShowAsync(double top, double left);

	/// <summary>
	/// Close the menu
	/// </summary>
	/// <returns>An awaitable Task.</returns>
	Task CloseAsync();

	/// <summary>
	/// Gets if the menu is currently showing.
	/// </summary>
	bool IsShowing { get; }

}