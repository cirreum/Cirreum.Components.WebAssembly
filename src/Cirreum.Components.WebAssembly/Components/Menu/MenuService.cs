namespace Cirreum.Components;

internal class MenuService : IMenuService {

	public event Action<Menu, string?>? OnShow;
	public event Action? OnClose;

	public void ShowMenu(Menu menu, string? triggerElementId = null) {
		OnShow?.Invoke(menu, triggerElementId);
	}
	public void CloseMenu() {
		OnClose?.Invoke();
	}

	public event Action? OnRefresh;
	public void RefreshMenu() {
		OnRefresh?.Invoke();
	}

}