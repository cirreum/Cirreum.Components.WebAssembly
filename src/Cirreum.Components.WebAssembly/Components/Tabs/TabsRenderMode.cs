namespace Cirreum.Components;

using System.ComponentModel;

/// <summary>
/// Defines the Tabs Mode.
/// </summary>
public enum TabsRenderMode {
	/// <summary>
	/// Always renders all tabs to the DOM.
	/// </summary>
	[Description("Always renders all tabs to the DOM")]
	Default,
	/// <summary>
	/// Each tab will be loaded/rendered once, but not until it has been explicity selected.
	/// </summary>
	[Description("Each tab will be loaded/rendered once, but not until it has been explicity selected")]
	LazyLoad,
	/// <summary>
	/// Each tab will be reloaded/re-rendered every time it's selected, so only 1 single tab will be rendered in the DOM.
	/// </summary>
	[Description("Each tab will be reloaded/re-rendered every time it's selected, so only 1 single tab will be rendered in the DOM")]
	LazyReload
}