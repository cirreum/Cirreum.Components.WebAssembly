namespace Cirreum.Components;

using Cirreum.Components.Interop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;

public sealed partial class TreeViewNode() {

	private readonly string TreeViewNodeElementId = IdGenerator.Next;
	private readonly List<TreeViewNode> ChildNodes = [];
	private Menu? MenuRef;
	private string tabIndex = "-1";
	private string typeAheadBuffer = string.Empty;
	private CancellationTokenSource? _typeAheadCts;

	[Inject]
	private IJSAppModule JSApp { get; set; } = default!;

	[CascadingParameter]
	private TreeView ParentTreeView { get; set; } = default!;

	[Parameter, EditorRequired]
	public TreeViewModel Model { get; set; } = default!;

	[Parameter]
	public TreeViewNode? ParentNode { get; set; }

	[Parameter]
	public bool ShowNodeHeaders { get; set; } = true;


	/// <summary>
	/// Searches for a node matching specified criteria within the child nodes of the provided branch root node.
	/// </summary>
	/// <param name="branchRoot">The root node of the branch to search. This node itself is not included in the search.</param>
	/// <param name="text">The text to search for within node names.</param>
	/// <param name="contains">
	///     If <see langword="true"/>, matches nodes where the name contains the search text;
	///     if <see langword="false"/>, matches nodes where the name starts with the search text.
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
	///     A <see cref="CancellationToken"/> that can be used to cancel the search operation.
	///     Default is <see cref="CancellationToken.None"/>.
	/// </param>
	/// <returns>
	/// The first <see cref="TreeViewNode"/> that matches the search criteria, or <see langword="null"/> if no matching node is found.
	/// </returns>
	/// <remarks>
	/// <para>
	/// The search begins from the first child of <paramref name="branchRoot"/> and continues through all descendants.
	/// The <paramref name="branchRoot"/> itself is not included in the search.
	/// </para>
	/// <para>
	/// The search proceeds as follows:
	/// <list type="number">
	///     <item>Iterates through visible nodes (considering the <paramref name="includeDisabled"/> parameter).</item>
	///     <item>For each node, checks if its name matches the <paramref name="text"/> based on the <paramref name="contains"/> and <paramref name="caseSensitive"/> parameters.</item>
	///     <item>If a match is found and the node is either enabled or <paramref name="includeDisabled"/> is <see langword="true"/>, the node is returned.</item>
	///     <item>The search continues until a match is found, all nodes are checked, or the operation is cancelled.</item>
	/// </list>
	/// </para>
	/// <para>
	/// The method returns <see langword="null"/> if the <paramref name="text"/> is empty, the <paramref name="branchRoot"/> is <see langword="null"/>, or no matching node is found.
	/// </para>
	/// </remarks>
	internal static TreeViewNode? SearchBranch(
		TreeViewNode? branchRoot,
		string text,
		bool contains = false,
		bool includeSelected = false,
		bool includeHidden = false,
		bool includeDisabled = false,
		bool caseSensitive = false,
		CancellationToken token = default) {

		if (string.IsNullOrEmpty(text) || branchRoot == null) {
			return null;
		}

		var currentNode = branchRoot;

		do {
			// Move to the next visible node, considering the includeDisabled parameter
			currentNode = GetNextVisibleNode(currentNode, includeHidden, includeDisabled, token);

			// If we've wrapped around to the start or beyond, stop searching
			if (currentNode == null || currentNode == branchRoot) {
				break;
			}

			// Skip currently selected nodes
			if (includeSelected is false &&
				currentNode.State.SelectedNode is not null &&
				currentNode.Model.NodeId == currentNode.State.SelectedNode.Model.NodeId) {
				continue;
			}

			// Check if this node matches our criteria
			var nameMatches = contains
			   ? currentNode.Model.Name.Contains(text, caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase)
			   : currentNode.Model.Name.StartsWith(text, caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);

			if (nameMatches && (includeDisabled || !currentNode.Model.IsDisabled)) {
				return currentNode;
			}

		} while (!token.IsCancellationRequested);

		return null; // No matching node found

	}

	private bool NodeFound { get; set; }
	private TreeViewState State => this.ParentTreeView.State;
	private IMenuInternal? MenuInternal => this.MenuRef;
	private bool IsMenuShowing => this.MenuRef is not null && this.MenuRef.IsShowing;
	private ElementReference TreeViewNodeElement { get; set; }
	private string TreeViewNodeClassList => CssBuilder
		.Default("treeview-node")
			.AddClass("node-active", when: this.HasSelection)
			.AddClass("node-has-active", when: this.Model.HasSelectedChild)
			.AddClass("node-expanded", when: this.Model.HasChildren && this.Model.IsExpanded)
			.AddClass("disabled", when: this.Model.IsDisabled)
			.AddClass("treeview-found-node", when: this.NodeFound)
		.Build();
	private string TreeViewGroupClassList => CssBuilder
		.Default("user-select-none")
			.AddClass("expanded", when: this.Model.IsExpanded)
		.Build();
	private bool ShowDisplayCount => this.Model.DisplayCustomCount || this.Model.DisplayChildrenCount;
	private string? DisplayedCount {
		get {
			if (this.Model.DisplayCustomCount) {
				return $"{this.Model.CustomCount}";
			}
			if (this.Model.DisplayChildrenCount) {
				return $"{this.Model.Children.Count}";
			}
			return null;
		}
	}
	private bool HasSelection => this.Model.IsSelected ||
		(this.Model.IsExpanded is false &&
		this.Model.HasHiddenSelectedChild);
	private string ResolvedIconCss => this.Model.ItemIconCss is null ?
				this.State.DefaultIconCss :
				this.Model.ItemIconCss(this.HasSelection, this.Model.IsExpanded);
	private string ResolvedIconColorCss => this.Model.ItemIconColorCss is null ?
				"" :
				this.Model.ItemIconColorCss(this.HasSelection, this.Model.IsExpanded);
	private bool HasItemIconImageUrl => this.Model.ItemIconImageUrl is not null;
	private string ResolvedIconImageUrl => this.Model.ItemIconImageUrl is null ? "" :
				this.Model.ItemIconImageUrl(this.HasSelection, this.Model.IsExpanded);
	private string ResolvedBadgeClassList => this.Model.ItemBadgeColorCss is null ?
				this.State.DefaultBadgeColorCss :
				this.Model.ItemBadgeColorCss(this.HasSelection, this.Model.IsExpanded);


	private void RegisterChildNode(TreeViewNode childNode) {
		this.ChildNodes.Add(childNode);
	}

	private void UnregisterChildNode(TreeViewNode childNode) {
		this.ChildNodes.Remove(childNode);
	}

	private async Task SelectNodeFromClick(MouseEventArgs eventArgs) {
		if (this.Model.IsSelected) {
			if (eventArgs.CtrlKey) {
				await this.ParentTreeView.SelectTreeViewNodeAsync(null);
			}
			return;
		}
		if (eventArgs.CtrlKey || eventArgs.ShiftKey || eventArgs.AltKey || eventArgs.MetaKey) {
			return;
		}
		await this.ParentTreeView.SelectTreeViewNodeAsync(this);
	}

	private static async Task FocusNextNodeStartingWith(
		TreeViewNode startNode,
		string typeAheadBuffer,
		CancellationToken token = default) {

		var foundNode = SearchBranch(startNode, typeAheadBuffer, false, true, false, includeDisabled: true, false, token);
		if (foundNode is null) {
			return;
		}

		await foundNode.FocusAsync();

	}
	private static TreeViewNode? GetNextVisibleNode(TreeViewNode node, bool includeHidden, bool includeDisabled, CancellationToken token = default) {
		var nextNode = node;

		do {
			// If the node is expanded and has children, move to the first child
			if (nextNode.ChildNodes.Count > 0 && (includeHidden || nextNode.Model.IsExpanded)) {
				nextNode = nextNode.ChildNodes[0];
			} else {
				// If no children or not expanded, move to the next sibling or up the tree
				while (true) {
					var nextSibling = GetNextSibling(nextNode);
					if (nextSibling != null) {
						nextNode = nextSibling;
						break;
					}

					// If no next sibling, move up to the parent
					if (nextNode.ParentNode == null) {
						// If we're at the root, wrap around to the first top-level node
						nextNode = nextNode.ParentTreeView?.ChildNodes.FirstOrDefault();
						return nextNode; // This might be null if the tree is empty
					}
					nextNode = nextNode.ParentNode;
				}
			}

			// If we're including disabled nodes or this node is enabled, return it
			if (includeDisabled || !nextNode.Model.IsDisabled) {
				return nextNode;
			}

			// If we're here, the node is disabled and we're not including disabled nodes.
			// Continue to the next node.
		} while (nextNode != node && !token.IsCancellationRequested); // Stop if we've wrapped around to the start

		return null; // No suitable node found
	}
	private static TreeViewNode? GetNextSibling(TreeViewNode node) {
		if (node.ParentNode != null) {
			var index = node.ParentNode.ChildNodes.IndexOf(node);
			if (index < node.ParentNode.ChildNodes.Count - 1) {
				return node.ParentNode.ChildNodes[index + 1];
			}
		} else if (node.ParentTreeView != null) {
			var index = node.ParentTreeView.ChildNodes.IndexOf(node);
			if (index < node.ParentTreeView.ChildNodes.Count - 1) {
				return node.ParentTreeView.ChildNodes[index + 1];
			}
		}
		return null;
	}

	private static async Task ExpandOrFocusChild(TreeViewNode node) {
		if (node.ChildNodes.Count > 0) {
			if (node.Model.IsExpanded is false) {
				if (node.Model.IsDisabled is false) {
					node.Model.IsExpanded = true;
					node.Update();
				}
				return;
			}

			var lastIndex = node.ChildNodes.Count - 1;
			var nextIndex = 0;
			// skip disabled nodes...
			while (nextIndex <= lastIndex && node.ChildNodes[nextIndex].Model.IsDisabled) {
				nextIndex++;
			}
			if (nextIndex <= lastIndex) {
				await node.ChildNodes[nextIndex].FocusAsync();
			}
			return;
		}
	}
	private static async Task CollapseOrFocusParent(TreeViewNode node) {
		if (node.ChildNodes.Count > 0 && node.Model.IsExpanded) {
			node.Model.IsExpanded = false;
			node.Update();
			return;
		}
		if (node.ParentNode is not null) {
			await node.ParentNode.FocusAsync();
			return;
		}
	}
	private async Task FocusNextSiblingNode(TreeViewNode node, bool includeExpanded = true) {

		// 1. We are expanded
		// 2. We are NOT expanded, and we are NOT the last child
		// 3. We are NOT expanded, and we are the last child

		var siblings = this.ParentTreeView.ChildNodes;
		if (node.ParentNode != null) {
			siblings = node.ParentNode.ChildNodes;
		}
		var nodeIndex = siblings.IndexOf(node);

		//
		// #1 Navigate to first non-disabled child
		//
		if (includeExpanded && node.Model.IsExpanded) {
			var lastIndex = node.ChildNodes.Count - 1;
			var nextIndex = 0;
			// skip disabled nodes...
			while (nextIndex <= lastIndex && node.ChildNodes[nextIndex].Model.IsDisabled) {
				nextIndex++;
			}
			if (nextIndex <= lastIndex) {
				await node.ChildNodes[nextIndex].FocusAsync();
				return;
			}
		}

		//
		// #2 Navigate to next sibling
		//
		if (nodeIndex < siblings.Count - 1) {

			var lastIndex = siblings.Count - 1;
			var nextIndex = nodeIndex + 1;
			// skip disabled nodes...
			while (nextIndex <= lastIndex && siblings[nextIndex].Model.IsDisabled) {
				nextIndex++;
			}
			if (nextIndex <= lastIndex) {
				await siblings[nextIndex].FocusAsync();
				return;
			}
		}

		//
		// #3 Navigate to parents next sibling
		//
		if (node.ParentNode != null) {
			await this.FocusNextSiblingNode(node.ParentNode, false);
		}

	}
	private async Task FocusPreviousSiblingNode(TreeViewNode node) {

		// 1. We are not the root of our branch, and our previous sibling is expanded
		// 2. We are not the root of our branch, and our previous sibling is NOT expanded
		// 3. We ARE the root of our branch, and have a parent

		var siblings = this.ParentTreeView.ChildNodes;
		if (node.ParentNode != null) {
			siblings = node.ParentNode.ChildNodes;
		}
		var nodeIndex = siblings.IndexOf(node);

		// skip disabled nodes...
		while (nodeIndex >= 0 && siblings[nodeIndex].Model.IsDisabled) {
			nodeIndex--;
		}

		if (nodeIndex > 0 && nodeIndex <= siblings.Count - 1) {

			var previousSibling = siblings[nodeIndex - 1];

			//
			// #1 Navigate to previous sibling's deepest visible child
			//
			if (previousSibling.Model.IsExpanded) {
				var lastIndex = previousSibling.ChildNodes.Count - 1;
				// skip disabled nodes...
				while (lastIndex >= 0 && previousSibling.ChildNodes[lastIndex].Model.IsDisabled) {
					lastIndex--;
				}
				if (lastIndex >= 0) {
					await FocusEndNode(previousSibling.ChildNodes[lastIndex]);
					return;
				}
			}

			//
			// #2 Navigate to previous sibling
			//
			if (previousSibling.Model.IsDisabled is false) {
				await previousSibling.FocusAsync();
				return;
			}

		}

		//
		// #3 Navigate to parent
		//
		if (node.ParentNode != null && node.ParentNode.Model.IsDisabled is false) {
			await node.ParentNode.FocusAsync();
		}

	}
	private static async Task FocusEndNode(TreeViewNode node) {

		//
		// #1 Recurse on nodes that are expanded
		//
		if (node.Model.IsExpanded) {
			var lastIndex = node.ChildNodes.Count - 1;
			// skip disabled nodes...
			while (lastIndex >= 0 && node.ChildNodes[lastIndex].Model.IsDisabled) {
				lastIndex--;
			}
			if (lastIndex >= 0) {
				await FocusEndNode(node.ChildNodes[lastIndex]);
				return;
			}
		}

		//
		// #2 Focus node
		//
		await node.FocusAsync();

	}

	internal async Task NodeFoundAsync() {
		this.ParentTreeView.ScrollNodeIntoView(this);
		this.NodeFound = true;
		this.Update(); // Trigger UI update to apply the CSS class

		await Task.Delay(2000); // Wait for 2 seconds

		this.NodeFound = false;
		this.Update(); // Trigger UI update to apply the CSS class
	}

	internal async Task FocusAsync(bool preventScroll = false) {
		if (this.Model.IsDisabled) {
			await this.NodeFoundAsync();
			return;
		}
		this.ParentTreeView.ActiveDescendant = this.ElementId;
		if (this.TreeViewNodeElement.Context is not null) {
			await this.TreeViewNodeElement.FocusAsync(preventScroll);
		}
	}

	internal void ExpandParentBranch() {
		var p = this.ParentNode;
		while (p != null) {
			p.Model.IsExpanded = true;
			p = p.ParentNode;
		}
	}

	internal TreeViewNode? FindNode(Guid nodeId) {
		foreach (var node in this.ChildNodes) {
			if (node.Model.NodeId == nodeId) {
				return node;
			}
			if (node.ChildNodes.Count != 0) {
				var found = node.FindNode(nodeId);
				if (found != null) {
					return found;
				}
			}
		}
		return null;
	}

	private void HandleFocusIn() {

		if (this.Model.IsDisabled) {
			return;
		}

		this.tabIndex = "0";

	}

	private void HandleFocusOut() {
		this.tabIndex = "-1";
	}

	private async Task HandleClick(MouseEventArgs args) {

		if (this.Model.IsDisabled) {
			return;
		}

		if (args.Detail == 2) {
			try {
				this.ToggleNode();
			} catch {
				// swallow user errors
			}
			return; // don't select on dbl click
		}

		if (args.Detail == 1) {
			try {
				await this.SelectNodeFromClick(args);
			} catch {
				// swallow user errors
			}
		}

		if (this.IsMenuShowing) {
			await this.MenuRef!.CloseAsync();
		}

	}

	private async Task HandleContextMenu(MouseEventArgs e) {

		if (this.MenuRef is null || this.Model.IsDisabled) {
			return;
		}

		if (this.MenuRef.IsShowing) {
			await this.MenuRef.CloseAsync();
			return;
		}

		await this.MenuRef.ShowAsync(e.PageY, e.PageX);

	}

	private async Task HandleContextMenuItemSelected(IMenuItem item) {
		await this.ParentTreeView.NodeContextMenuItemSelected(this.Model, item);
	}

	private async Task HandleKeyDown(KeyboardEventArgs e) {

		if (this.Model.IsDisabled) {
			return;
		}

		var key = e.Code ?? e.Key;
		switch (key) {
			case null:
				break;
			case "Tab":
				this.JSApp.FocusElement(".treeview-node-container", true);
				break;
			case " ":
			case "Enter":
				if (this.Model.IsSelected) {
					return;
				}
				await this.ParentTreeView.SelectTreeViewNodeAsync(this);
				break;
			case "ArrowDown":
				await this.FocusNextSiblingNode(this);
				break;
			case "ArrowUp":
				await this.FocusPreviousSiblingNode(this);
				break;
			case "ArrowRight":
				await ExpandOrFocusChild(this);
				break;
			case "ArrowLeft":
				await CollapseOrFocusParent(this);
				break;
			default:
				if (key.Length == 1 && char.IsLetterOrDigit(key[0])) {
					await this.HandleTypeAhead(key[0]);
				}
				break;
		}

	}

	private async Task HandleTypeAhead(char key) {

		// Add to buffer
		this.typeAheadBuffer += char.ToLowerInvariant(key);

		// Cancel any previous typeahead operation
		this._typeAheadCts?.Cancel();
		this._typeAheadCts = new CancellationTokenSource();
		var token = this._typeAheadCts.Token;

		try {

			// Introduce a small delay to allow for rapid keystrokes
			await Task.Delay(250, token);

			if (!token.IsCancellationRequested && !string.IsNullOrEmpty(this.typeAheadBuffer)) {
				await FocusNextNodeStartingWith(this, this.typeAheadBuffer, token);
			}

			// Clear buffer either way
			this.typeAheadBuffer = "";

		} catch (OperationCanceledException) {
			// Operation was cancelled, which is expected behavior
		}

	}

	private void ToggleNode() {

		if (this.Model.IsDisabled) {
			return;
		}

		if (this.Model.HasChildren) {

			this.Model.IsExpanded = !this.Model.IsExpanded;

			if (this.Model.IsExpanded) {
				this.Model.HasHiddenSelectedChild = false;
				if (this.ParentTreeView.OnNodeExpanded.HasDelegate) {
					this.ParentTreeView.OnNodeExpanded.InvokeAsync(this.Model);
				}
			} else if (this.Model.HasChildren && this.State.SelectedNode is not null && this.State.SelectedNode != this) {
				var selectedNodeId = this.State.SelectedNode.Model.NodeId;
				var foundSelectedChild = TreeView.FindChild(this.Model, selectedNodeId);
				if (foundSelectedChild is not null) {
					this.Model.HasHiddenSelectedChild = true;
				}
				if (this.ParentTreeView.OnNodeCollapsed.HasDelegate) {
					this.ParentTreeView.OnNodeCollapsed.InvokeAsync(this.Model);
				}
			}

			this.Update();

		}

	}


	protected override void OnInitialized() {
		base.OnInitialized();

		if (this.Model.IsSelected) {
			this.ParentTreeView.QueueInitiallySelected(this);
		}

		if (this.ParentNode is null) {
			this.ParentTreeView.RegisterChildNode(this);
		} else {
			this.ParentNode.RegisterChildNode(this);
		}

	}

	protected override void Dispose(bool disposing) {
		if (this.ParentNode is null) {
			this.ParentTreeView.UnregisterChildNode(this);
		} else {
			this.ParentNode.UnregisterChildNode(this);
		}
		this._typeAheadCts?.Dispose();
		base.Dispose(disposing);
	}

}