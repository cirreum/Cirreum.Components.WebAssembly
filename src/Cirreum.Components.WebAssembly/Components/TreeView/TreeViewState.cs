namespace Cirreum.Components;

/// <summary>
/// Maintains the state of the TreeView and its selected Node.
/// </summary>
public sealed class TreeViewState {

	internal TreeViewState() {

	}

	/// <summary>
	/// Gets the currently selected node, if any.
	/// </summary>
	public TreeViewNode? SelectedNode { get; internal set; }

	internal bool ShowIcons { get; set; }
	internal string DefaultIconCss { get; set; } = "bi bi-folder";
	internal string DefaultBadgeColorCss { get; set; } = "text-bg-primary";

}