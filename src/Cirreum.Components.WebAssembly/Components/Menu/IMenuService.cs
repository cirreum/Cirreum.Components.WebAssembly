namespace Cirreum.Components;

using System;

public interface IMenuService {

	event Action? OnClose;
	void CloseMenu();

	event Action<Menu, string?>? OnShow;
	void ShowMenu(Menu menu, string? triggerElementId);

	event Action? OnRefresh;
	void RefreshMenu();


}