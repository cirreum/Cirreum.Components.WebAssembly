namespace Cirreum.Components;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

/// <summary>
/// A component to display a hierarchical collection of items.
/// </summary>
public partial class TreeView : BaseAfterRenderComponent {

	private readonly List<TreeViewNode> InitiallySelectedChildren = [];
	internal readonly TreeViewState State = new TreeViewState();
	internal readonly List<TreeViewNode> ChildNodes = [];
	private string ScrollbarBehaviorTargetId = "";
	private IJSInProcessObjectReference? module;

	internal string ActiveDescendant { get; set; } = "";
	private string TreeViewWrapperStyle => StyleBuilder
		.Empty()
			.AddStyleIfNotEmpty("--bs-treeview-height", this.Height)
			.AddStyleIfNotEmpty("--bs-treeview-width", this.Width)
		.Build();

	private string HeaderClassList => CssBuilder
		.Default("treeview-header")
			.AddClass("d-none", when: this.HeaderTemplate is null || this.ShowHeader is false)
		.Build();

	private string FooterClassList => CssBuilder
		.Default("treeview-footer")
			.AddClass("d-none", when: this.FooterTemplate is null || this.ShowFooter is false)
		.Build();

	/// <summary>
	/// The hierarchical collection of items to render.
	/// </summary>
	[Parameter, EditorRequired]
	public ICollection<TreeViewModel>? Model { get; set; }

	/// <summary>
	/// EventCallback when a Node is selected or deselected.
	/// </summary>
	/// <remarks>
	/// When being deselected, the model will be <see langword="null"/>.
	/// </remarks>
	[Parameter]
	public EventCallback<TreeViewModel?> OnNodeSelected { get; set; }

	/// <summary>
	/// EventCallback when a Node is expanded.
	/// </summary>
	[Parameter]
	public EventCallback<TreeViewModel?> OnNodeExpanded { get; set; }

	/// <summary>
	/// EventCallback when a Node is collapsed.
	/// </summary>
	[Parameter]
	public EventCallback<TreeViewModel?> OnNodeCollapsed { get; set; }

	/// <summary>
	/// The optional template containing the context-menu items for each node.
	/// </summary>
	[Parameter]
	public RenderFragment<TreeViewModel>? NodeContextMenu { get; set; }

	/// <summary>
	/// EventCallback when a Node Context Menu item is selected.
	/// </summary>
	[Parameter]
	public EventCallback<MenuItemEventArgs<TreeViewModel>> OnNodeContextMenuItemSelected { get; set; }

	/// <summary>
	/// Set to <see langword="false"/> to not display node icon.
	/// </summary>
	[Parameter]
	public bool ShowIcons { get; set; } = true;

	/// <summary>
	/// The Icon Css class when <see cref="ShowIcons"/> is <see langword="true"/>. Default: 'bi bi-folder'
	/// </summary>
	/// <remarks>
	/// Allows setting the value for all nodes, preventing each node from having to 
	/// specify a value.
	/// </remarks>
	[Parameter]
	public string DefaultIconCss { get; set; } = "bi bi-folder";

	/// <summary>
	/// The Badge Css class when <see cref="TreeViewModel.DisplayChildrenCount"/> or
	/// <see cref="TreeViewModel.DisplayCustomCount"/> is <see langword="true"/>
	/// </summary>
	/// <remarks>
	/// Allows setting the value for all nodes, preventing each node from having to 
	/// specify a value.
	/// </remarks>
	[Parameter]
	public string DefaultBadgeCss { get; set; } = "text-bg-primary";

	/// <summary>
	/// An optional template, rendered if the <see cref="Model"/> contains no items.
	/// </summary>
	[Parameter]
	public RenderFragment? EmptyTemplate { get; set; }

	/// <summary>
	/// An optional template, to render a Header above the TreeView nodes.
	/// </summary>
	[Parameter]
	public RenderFragment? HeaderTemplate { get; set; }

	/// <summary>
	/// When <see langword="true"/>, and the <see cref="HeaderTemplate"/> is not null, renders the <see cref="HeaderTemplate"/>.
	/// </summary>
	[Parameter]
	public bool ShowHeader { get; set; } = true;

	/// <summary>
	/// An optional template, to render a Footer below the TreeView nodes.
	/// </summary>
	[Parameter]
	public RenderFragment? FooterTemplate { get; set; }

	/// <summary>
	/// When <see langword="true"/>, and the <see cref="FooterTemplate"/> is not null, renders the <see cref="FooterTemplate"/>.
	/// </summary>
	[Parameter]
	public bool ShowFooter { get; set; } = true;

	/// <summary>
	/// When <see langword="false"/>, hides any nodes that are of type <see cref="TreeViewHeaderNode"/>.
	/// </summary>
	[Parameter]
	public bool ShowNodeHeaders { get; set; } = true;

	/// <summary>
	/// Determines if all child nodes should be expanded during initialization.
	/// </summary>
	[Parameter]
	public bool AutoExpandAll { get; set; }

	/// <summary>
	/// Sets the desired height of the Treeview.
	/// </summary>
	/// <remarks>
	/// This is a string, so you must specify the unit (eg, 100px or 50%)
	/// </remarks>
	[Parameter]
	public string? Height { get; set; }

	/// <summary>
	/// Sets the desired width of the Treeview.
	/// </summary>
	/// <remarks>
	/// This is a string, so you must specify the unit (eg, 100px or 50%)
	/// </remarks>
	[Parameter]
	public string? Width { get; set; }

	/// <summary>
	/// Attempts to find a child model, recursively.
	/// </summary>
	/// <param name="parent">The parent to look within.</param>
	/// <param name="childId">The id of the child to find.</param>
	/// <returns>The <see cref="TreeViewModel"/> if found; otherwise null.</returns>
	public static TreeViewModel? FindChild(TreeViewModel parent, Guid childId) {

		foreach (var child in parent.Children) {
			if (child.NodeId.Equals(childId)) {
				return child;
			}
			if (child.HasChildren) {
				var found = FindChild(child, childId);
				if (found is not null) {
					return found;
				}
			}
		}

		return null;

	}
	/// <summary>
	/// For the given set of models, finds all parents if any, of the specified <paramref name="model"/>.
	/// </summary>
	/// <param name="model">The model to find the parents of.</param>
	/// <param name="branches">One of more models to search within.</param>
	/// <returns>A list containing any found parents.</returns>
	/// <remarks>
	/// This method is recursive, searching the hiearchy from top to bottom.
	/// </remarks>
	public static List<TreeViewModel> FindParents(TreeViewModel model, IEnumerable<TreeViewModel> branches) {
		var parents = new List<TreeViewModel>();
		foreach (var branch in branches) {
			var parent = FindParent(branch, model);
			if (parent != null) {
				while (parent != null) {
					parents.Add(parent);
					parent = FindParent(branch, parent);
				}
				break; // Once found in one branch, break out of the loop
			}
		}
		return parents;
	}
	/// <summary>
	/// For the given <paramref name="branch"/>, attempts to find the parent model of the specified <paramref name="model"/>.
	/// </summary>
	/// <param name="branch">The model with multiple children and levels to search within.</param>
	/// <param name="model">The model to find the parent of.</param>
	/// <returns>The found model, otherwise null.</returns>
	/// <remarks>
	/// This method is recursive, searching the hiearchy from top to bottom.
	/// </remarks>
	public static TreeViewModel? FindParent(TreeViewModel branch, TreeViewModel model) {

		if (branch.Children.Contains(model)) {
			return branch; // aka the model's Parent
		}

		foreach (var child in branch.Children) {
			if (child == model) {
				return branch;
			}
			var parent = FindParent(child, model);
			if (parent != null) {
				return parent;
			}
		}

		return null;

	}

	/// <summary>
	/// Expands all items.
	/// </summary>
	public void ExpandAll() {
		if (this.Model is not null) {
			var changed = this.ExpandAllCore();
			if (changed && this.Rendered && this.State.SelectedNode is not null) {
				this.RunAfterRender(async () => {
					if (this.State.SelectedNode is not null) {
						await Task.Delay(500); // allow time for the Expand HTML Animation to complete
						await this.State.SelectedNode.FocusAsync();
					}
				});
				this.Update();
				return;
			}
		}
	}
	private bool ExpandAllCore() {
		var changed = false;
		if (this.Model is not null) {
			MutateAll(this.Model, m => {
				m.HasHiddenSelectedChild = false;
				if (m.HasChildren && !m.IsExpanded) {
					m.IsExpanded = true;
					changed = true;
				}
			});
		}
		return changed;
	}

	/// <summary>
	/// Collapses all items.
	/// </summary>
	public void CollapseAll() {
		if (this.Model is not null) {
			MutateAll(this.Model, m => {
				m.IsExpanded = false;
			});
			if (this.State.SelectedNode is not null &&
				this.State.SelectedNode.Model.IsSelected) {
				var parents = FindParents(this.State.SelectedNode.Model, this.Model);
				foreach (var parent in parents) {
					parent.HasHiddenSelectedChild = true;
				}
			}
		}
	}

	/// <summary>
	/// Searches for a tree node matching the specified criteria, scrolls it into view and focuses it if found.
	/// </summary>
	/// <param name="text">The text to search for within the node names.</param>
	/// <param name="contains">
	///     If <see langword="true"/>, includes nodes where the name contains the search text;
	///     if <see langword="false"/>, only includes nodes where the name starts with the search text.
	///     Default is <see langword="false"/>.
	/// </param>
	/// <param name="includeSelected">
	///     If <see langword="true"/>, includes selected nodes in the search;
	///     if <see langword="false"/>, skips over selected nodes.
	///     Default is <see langword="false"/>.
	/// </param>
	/// <param name="includeHidden">
	///     If <see langword="true"/>, includes hidden (parent is not expanded) nodes in the search;
	///     if <see langword="false"/>, excludes hidden nodes.
	///     Default is <see langword="false"/>.
	/// </param>
	/// <param name="includeDisabled">
	///     If <see langword="true"/>, includes disabled nodes in the search;
	///     if <see langword="false"/>, excludes disabled nodes.
	///     Default is <see langword="false"/>.
	/// </param>
	/// <param name="caseSensitive">
	///     If <see langword="true"/>, performs a case-sensitive search;
	///     if <see langword="false"/>, performs a case-insensitive search.
	///     Default is <see langword="false"/>.
	/// </param>
	/// <param name="token">
	///     An optional <see cref="CancellationToken"/> to cancel the search operation.
	///     Default is <see cref="CancellationToken.None"/>.
	/// </param>
	/// <returns>
	///     A <see cref="Task{TResult}"/> representing the asynchronous operation, where TResult is <see cref="bool"/>.
	///     The task result is <see langword="true"/> if a matching node was found and focused, 
	///     or <see langword="false"/> if no matching node was found.
	/// </returns>
	/// <remarks>
	/// <para>
	/// This method searches for a node based on the provided criteria. If a matching node is found, 
	/// it scrolls the node into view and attempts to focus that node asynchronously. If no matching
	/// node is found, the method completes without changing the current focus.
	/// </para>
	/// <para>
	/// The search is performed immediately, but the focus operation is asynchronous, 
	/// allowing for any necessary UI updates or event handling to occur.
	/// </para>
	/// </remarks>
	public async Task<bool> FocusNodeByName(
		string text,
		bool contains = false,
		bool includeSelected = false,
		bool includeHidden = false,
		bool includeDisabled = false,
		bool caseSensitive = false,
		CancellationToken token = default) {
		var node = await this.SearchNodesAsync(text, contains, includeSelected, includeHidden, includeDisabled, caseSensitive, token);
		if (node is not null) {
			await node.FocusAsync();
			return true;
		}
		return false;
	}

	/// <summary>
	/// Attempts to focus a node based on the provided TreeViewModel.
	/// </summary>
	/// <param name="model">The <see cref="TreeViewModel"/> representing the node to focus.</param>
	/// <returns>
	/// A <see cref="Task"/> representing the asynchronous operation.
	/// The task completes when the node is focused or when the node is not found.
	/// </returns>
	/// <remarks>
	/// This method searches for a node using the NodeId from the provided model.
	/// If found, it attempts to focus that node asynchronously.
	/// If the node is not found, the method completes without changing the current focus.
	/// </remarks>
	public async Task FocusNode(TreeViewModel model) {
		var node = this.FindNode(model.NodeId);
		if (node is not null) {
			await node.FocusAsync();
		}
	}

	/// <summary>
	/// Attempts to focus a node based on the provided node ID.
	/// </summary>
	/// <param name="nodeId">The unique identifier of the node to focus.</param>
	/// <returns>
	/// A <see cref="Task"/> representing the asynchronous operation.
	/// The task completes when the node is focused or when the node is not found.
	/// </returns>
	/// <remarks>
	/// This method searches for a node using the provided nodeId.
	/// If found, it attempts to focus that node asynchronously.
	/// If the node is not found, the method completes without changing the current focus.
	/// </remarks>
	public async Task FocusNode(Guid nodeId) {
		var node = this.FindNode(nodeId);
		if (node is not null) {
			await node.FocusAsync();
		}
	}

	/// <summary>
	/// Removes a node from the tree based on the provided TreeViewModel.
	/// </summary>
	/// <param name="model">The <see cref="TreeViewModel"/> representing the node to remove.</param>
	/// <returns>
	/// <see langword="true"/> if the node was found and successfully removed; otherwise <see langword="false"/>.
	/// </returns>
	/// <remarks>
	/// This method delegates the removal operation to the overload that takes a <see cref="Guid"/>,
	/// using the NodeId from the provided model.
	/// </remarks>
	public bool RemoveNode(TreeViewModel model) {
		return this.RemoveNode(model.NodeId);
	}

	/// <summary>
	/// Removes a node from the tree based on the provided node ID.
	/// </summary>
	/// <param name="nodeId">The unique identifier of the node to remove.</param>
	/// <returns>
	/// <see langword="true"/> if the node was found and successfully removed; otherwise <see langword="false"/>.
	/// </returns>
	/// <remarks>
	/// This method performs the following steps:
	/// <list type="number">
	///     <item>Searches for the node with the given ID.</item>
	///     <item>If found, attempts to remove it from its parent's children collection.</item>
	///     <item>If the node has no parent, attempts to remove it from the root collection.</item>
	///     <item>Updates the parent node (or the root) after successful removal.</item>
	/// </list>
	/// The method returns <see langword="false"/> if the node is not found or if the removal operation fails.
	/// </remarks>
	public bool RemoveNode(Guid nodeId) {
		var node = this.FindNode(nodeId);
		if (node is not null) {
			if (node.ParentNode is not null) {
				if (node.ParentNode.Model.Children.Remove(node.Model) is true) {
					if (this.State.SelectedNode == node) {
						this.RunAfterRender(async () => {
							await this.SelectTreeViewNodeAsync(null);
						});
					}
					node.ParentNode.Update();
					return true;
				}
				return false;
			}
			if (this.Model?.Remove(node.Model) is true) {
				if (this.State.SelectedNode == node) {
					this.RunAfterRender(async () => {
						await this.SelectTreeViewNodeAsync(null);
					});
				}
				node.Update();
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Searches for a tree node matching the specified criteria, scrolls it into view, and selects it if found.
	/// </summary>
	/// <param name="text">The text to search for within the node names.</param>
	/// <param name="contains">
	///     If <see langword="true"/>, includes nodes where the name contains the search text;
	///     if <see langword="false"/>, only includes nodes where the name starts with the search text.
	///     Default is <see langword="false"/>.
	/// </param>
	/// <param name="includeSelected">
	///     If <see langword="true"/>, includes selected nodes in the search;
	///     if <see langword="false"/>, skips over selected nodes.
	///     Default is <see langword="false"/>.
	/// </param>
	/// <param name="includeHidden">
	///     If <see langword="true"/>, includes hidden (parent is not expanded) nodes in the search;
	///     if <see langword="false"/>, excludes hidden nodes.
	///     Default is <see langword="false"/>.
	/// </param>
	/// <param name="includeDisabled">
	///     If <see langword="true"/>, includes disabled nodes in the search;
	///     if <see langword="false"/>, excludes disabled nodes.
	///     Default is <see langword="false"/>.
	/// </param>
	/// <param name="caseSensitive">
	///     If <see langword="true"/>, performs a case-sensitive search;
	///     if <see langword="false"/>, performs a case-insensitive search.
	///     Default is <see langword="false"/>.
	/// </param>
	/// <param name="token">
	///     An optional <see cref="CancellationToken"/> to cancel the search operation.
	///     Default is <see cref="CancellationToken.None"/>.
	/// </param>
	/// <returns>
	///     A <see cref="Task{TResult}"/> representing the asynchronous operation, where TResult is <see cref="bool"/>.
	///     The task result is <see langword="true"/> if a matching node was found and selected, 
	///     or <see langword="false"/> if no matching node was found.
	/// </returns>
	/// <remarks>
	/// <para>
	/// This method searches for a node based on the provided criteria. If a matching node is found, 
	/// it scrolls the node into view and selects it, unless its disabled, then it is merely focused.
	/// If no matching node is found, the method completes without changing the current selection or
	/// scroll position.
	/// </para>
	/// <para>
	/// The search is performed immediately, but the selection process is asynchronous, 
	/// allowing for any necessary UI updates or event handling to occur.
	/// </para>
	/// </remarks>
	public async Task<bool> SelectNodeByName(
		string text,
		bool contains = false,
		bool includeSelected = false,
		bool includeHidden = false,
		bool includeDisabled = false,
		bool caseSensitive = false,
		CancellationToken token = default) {
		var node = await this.SearchNodesAsync(text, contains, includeSelected, includeHidden, includeDisabled, caseSensitive, token);
		if (node is not null) {
			this.ScrollNodeIntoView(node);
			if (node.Model.IsDisabled) {
				await node.NodeFoundAsync();
			} else {
				await this.SelectTreeViewNodeAsync(node);
			}
			return true;
		}
		return false;
	}

	/// <summary>
	/// Asynchronously selects a node in the tree view using a <see cref="TreeViewModel"/>.
	/// </summary>
	/// <param name="model">The <see cref="TreeViewModel"/> containing the node information to be selected. This model must include a valid NodeId.</param>
	/// <returns>A Task representing the asynchronous node selection operation.</returns>
	/// <remarks>
	/// This method serves as a convenience wrapper around the Guid-based SelectNodeAsync method.
	/// It extracts the NodeId from the provided <see cref="TreeViewModel"/> and delegates the actual selection process.
	/// </remarks>
	public Task SelectNodeAsync(TreeViewModel model) {
		return this.SelectNodeAsync(model.NodeId);
	}

	/// <summary>
	/// Asynchronously selects a node in the tree view based on its unique identifier.
	/// </summary>
	/// <param name="nodeId">The Guid that uniquely identifies the node to be selected.</param>
	/// <returns>A Task representing the asynchronous node selection operation.</returns>
	/// <remarks>
	/// This method attempts to locate and select a node based on the provided nodeId.
	/// If found, it expands the node's parent branch, scrolls it into view, and
	/// selects it if not disabled. No action is taken if the node is not found.
	/// </remarks>
	public async Task SelectNodeAsync(Guid nodeId) {
		var node = this.FindNode(nodeId);
		if (node is not null) {
			node.ExpandParentBranch();
			await Task.Delay(250);
			this.ScrollNodeIntoView(node);
			if (node.Model.IsDisabled) {
				await node.NodeFoundAsync();
			} else {
				await this.SelectTreeViewNodeAsync(node);
				await node.FocusAsync();
			}
		}
	}

	private TreeViewNode? FindNode(Guid nodeId) {

		foreach (var node in this.ChildNodes) {
			if (node.Model.NodeId == nodeId) {
				return node;
			}
			if (node.Model.HasChildren) {
				var found = node.FindNode(nodeId);
				if (found is not null) {
					return found;
				}
			}
		}

		return null;

	}

	private async Task<TreeViewNode?> SearchNodesAsync(
		string text,
		bool contains = false,
		bool includeSelected = false,
		bool includeHidden = false,
		bool includeDisabled = false,
		bool caseSensitive = false,
		CancellationToken token = default) {

		foreach (var node in this.ChildNodes) {

			if (token.IsCancellationRequested) {
				return null;
			}

			if (includeSelected is false &&
				this.State.SelectedNode is not null &&
				node.Model.NodeId == this.State.SelectedNode.Model.NodeId) {
				continue;
			}

			if (includeDisabled || !node.Model.IsDisabled) {
				var nameMatches = contains
				   ? node.Model.Name.Contains(text, caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase)
				   : node.Model.Name.StartsWith(text, caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);
				if (nameMatches) {
					return node;
				}
				if (token.IsCancellationRequested) {
					return null;
				}
				if (node.Model.HasChildren) {
					var found = TreeViewNode.SearchBranch(node, text, contains, includeSelected, includeHidden, includeDisabled, caseSensitive, token);
					if (found is not null) {
						if (includeHidden) {
							found.ExpandParentBranch();
							await Task.Delay(250, token);
						}
						return found;
					}
				}
			}
		}

		return null;

	}

	internal async Task SelectTreeViewNodeAsync(TreeViewNode? node) {

		if (this.Model is null) {
			return;
		}

		if (this.State.SelectedNode is not null) {
			this.State.SelectedNode.Model.IsSelected = false;
			var currentParents = FindParents(this.State.SelectedNode.Model, this.Model);
			MutateAll(currentParents, p => {
				p.DecrementSelectedChildCount();
				p.HasHiddenSelectedChild = false;
			});
		}

		if (node is null) {
			this.State.SelectedNode = null;
			if (this.OnNodeSelected.HasDelegate) {
				await this.OnNodeSelected.InvokeAsync(null);
			}
			return;
		}

		node.Model.IsSelected = true;
		this.State.SelectedNode = node;

		var parents = FindParents(node.Model, this.Model);
		parents.ForEach(p => p.IncrementSelectedChildCount());

		if (this.OnNodeSelected.HasDelegate) {
			await this.OnNodeSelected.InvokeAsync(node.Model);
		}

	}

	internal async Task NodeContextMenuItemSelected(TreeViewModel model, IMenuItem menuItem) {
		if (this.OnNodeContextMenuItemSelected.HasDelegate) {
			await this.OnNodeContextMenuItemSelected.InvokeAsync(new(model, menuItem));
		}
	}

	internal void RegisterChildNode(TreeViewNode childNode) {
		this.ChildNodes.Add(childNode);
	}

	internal void UnregisterChildNode(TreeViewNode childNode) {
		this.ChildNodes.Remove(childNode);
	}

	private const bool singleSelectionMode = true;
	internal void QueueInitiallySelected(TreeViewNode node) {
		if (singleSelectionMode) {
			if (this.InitiallySelectedChildren.Count != 0) {
				this.InitiallySelectedChildren[0].Model.IsSelected = false;
				this.InitiallySelectedChildren.Clear();
			}
		}
		this.InitiallySelectedChildren.Add(node);
		node.Model.IsSelected = false;
	}

	internal void ScrollNodeIntoView(TreeViewNode node) {
		this.module?.InvokeVoid(
			"scrollNodeIntoView",
			this.ElementId,
			node.ElementId,
			new { Debug = System.Diagnostics.Debugger.IsAttached, SmoothScroll = false });
	}

	private static void MutateAll(IEnumerable<TreeViewModel> data, Action<TreeViewModel> mutator) {
		foreach (var item in data) {
			MutateAll(item, mutator);
		}
	}

	private static void MutateAll(TreeViewModel model, Action<TreeViewModel> mutator) {
		mutator(model);
		MutateAll(model.Children, mutator);
	}


	public override async Task SetParametersAsync(ParameterView parameters) {

		var hadASelectedNode = this.State.SelectedNode != null;

		await base.SetParametersAsync(parameters);

		this.State.ShowIcons = this.ShowIcons;
		this.State.DefaultIconCss = this.DefaultIconCss;
		this.State.DefaultBadgeColorCss = this.DefaultBadgeCss;

		if (this.Model == null || this.Model.Count == 0) {
			if (hadASelectedNode) {
				this.State.SelectedNode!.Model.DecrementSelectedChildCount();
				this.State.SelectedNode!.Model.HasHiddenSelectedChild = false;
				this.State.SelectedNode = null;
				if (this.OnNodeSelected.HasDelegate) {
					await this.OnNodeSelected.InvokeAsync(null);
				}
			}
		}

	}

	protected override void OnInitialized() {
		this.ScrollbarBehaviorTargetId = $"#{this.ElementId}";
	}

	protected override async Task OnAfterFirstRenderAsync() {
		const string jsPath = "./_content/Cirreum.Components.WebAssembly/Components/TreeView/TreeView.razor.js";
		this.module = await this.JSApp.InvokeAsync<IJSInProcessObjectReference>("import", jsPath);
		if (this.AutoExpandAll && this.Model is not null) {
			this.ExpandAllCore();
		}
		if (this.InitiallySelectedChildren.Count > 0) {
			var initiallySelectedNode = this.InitiallySelectedChildren.Last();
			this.InitiallySelectedChildren.Clear();
			initiallySelectedNode.ExpandParentBranch();
			this.RunAfterRender(async () => {
				var delay = 200 + (this.Model!.Count * 4);
				if (delay > 4000) {
					delay = 4000;
				}
				await Task.Delay(delay); // wait for animation to complete...
				await this.SelectTreeViewNodeAsync(initiallySelectedNode);
				//await initiallySelectedNode.FocusAsync(true);
			});
		}
	}

	protected override void Dispose(bool disposing) {
		this.module?.Dispose();
		base.Dispose(disposing);
	}
}