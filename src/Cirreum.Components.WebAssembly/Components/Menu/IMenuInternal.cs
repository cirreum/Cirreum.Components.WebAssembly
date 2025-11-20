namespace Cirreum.Components;

using Microsoft.AspNetCore.Components;

internal interface IMenuInternal : IMenu {

	string MenuId { get; }

	RenderFragment? MenuTemplate { get; }

	/// <summary>
	/// The method accepts the user selected <see cref="IMenuItem"/> instance
	/// which in-turn hides the context-menu, and then calls OnItemSelected.
	/// </summary>
	/// <param name="item">The user selected <see cref="IMenuItem"/> instance.</param>
	/// <returns>An awaitable <see cref="Task"/></returns>
	Task HandleItemSelectionAsync(IMenuItem item);

	/// <summary>
	/// Register a child menu item.
	/// </summary>
	/// <param name="menuItem">The menu item.</param>
	void RegisterMenuItem(MenuItem menuItem);

	/// <summary>
	/// Unregister a child menu item.
	/// </summary>
	/// <param name="menuItem">The menu item.</param>
	void UnregisterMenuItem(MenuItem menuItem);

	/// <summary>
	/// Set focus to the first menu item.
	/// </summary>
	/// <returns></returns>
	Task FocusFirstAsync();

	/// <summary>
	/// Set focus to the last menu item.
	/// </summary>
	/// <returns></returns>
	Task FocusLastAsync();

	/// <summary>
	/// Move focus to the next menu item.
	/// </summary>
	/// <param name="direction"></param>
	/// <returns></returns>
	Task MoveFocusAsync(int direction);

	/// <summary>
	/// Move focus to the menu item that begins with the specified key.
	/// </summary>
	/// <param name="key">The key to evaluate.</param>
	/// <returns>An awaitable Task</returns>
	Task FocusByFirstCharacter(string key);

	/// <summary>
	/// Close the menu.
	/// </summary>
	/// <returns></returns>
	Task HideAsync(bool notifyContainer);

	/// <summary>
	/// Called when the Menu has been opened (rendered) in the <see cref="MenuContainer"/>.
	/// </summary>
	/// <returns></returns>
	Task OnOpenedAsync();

	/// <summary>
	/// Called when the Menu has been closed (removed) from the <see cref="MenuContainer"/>.
	/// </summary>
	/// <returns></returns>
	Task OnClosedAsync();

}