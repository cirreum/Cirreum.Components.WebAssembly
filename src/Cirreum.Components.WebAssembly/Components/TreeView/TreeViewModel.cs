namespace Cirreum.Components;

/// <summary>
/// A delegate used to allow node styling.
/// </summary>
/// <param name="IsSelected"><see langword="true"/> if the item is selected, or if not expanded yet the currently selected item is a child of this item.</param>
/// <param name="IsExpanded"><see langword="true"/> if the item has children and expanded.</param>
/// <returns>The string related to the use of this delegate.</returns>
/// <remarks>
/// Use of the <see cref="ItemStyleDelegate"/> indicates if this item is
/// currently selected or if not expanded if it contains the selected
/// node (a child), and if this item has children and is expanded and
/// can be used to customize the styling and appearance of a node item.
/// </remarks>
public delegate string ItemStyleDelegate(bool IsSelected, bool IsExpanded);

/// <summary>
/// The model that is used to populate a <see cref="TreeView"/>.
/// </summary>
public class TreeViewModel {

	/// <summary>
	/// Construct a new instance.
	/// </summary>
	public TreeViewModel() {

	}

	/// <summary>
	/// The ID (guid) of the node associated with this instance.
	/// </summary>
	public Guid NodeId { get; set; } = Guid.NewGuid();
	/// <summary>
	/// The ID (guid) of the nodes Parent Node.
	/// </summary>
	public Guid ParentId { get; set; } = Guid.NewGuid();
	/// <summary>
	/// The value to be displayed for the Node.
	/// </summary>
	public string Name { get; set; } = "";
	/// <summary>
	/// The value displayed in the Nodes html Title attribute.
	/// </summary>
	public string Description { get; set; } = "";

	/// <summary>
	/// Gets or sets a value indicating whether the current node is a header node.
	/// </summary>
	public bool IsHeaderNode { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether the header node should be rendered with a divider line.
	/// </summary>
	/// <remarks>
	/// Only applies when <see cref="IsHeaderNode"/> is <see langword="true"/>.
	/// If <see cref="ShowHeaderNodeDivider"/> is <see langword="true"/> and <see cref="Name"/> has a value, 
	/// the name will be displayed below the divider line. If <see cref="Name"/> is empty, only the divider
	/// line will be shown.
	/// </remarks>
	public bool ShowHeaderNodeDivider { get; set; }

	/// <summary>
	/// Is this item currently selected.
	/// </summary>
	public bool IsSelected { get; set; }

	/// <summary>
	/// Is this item currently expanded.
	/// </summary>
	public bool IsExpanded { get; set; }

	/// <summary>
	/// Is this item currently disabled.
	/// </summary>
	public bool IsDisabled { get; set; }

	/// <summary>
	/// Allows showing a different icon. Default: bi bi-folder
	/// </summary>
	/// <remarks>
	/// <para>
	/// The <see cref="ItemIconImageUrl"/> takes priority over of the <see cref="ItemIconCss"/>.
	/// </para>
	/// </remarks>
	public ItemStyleDelegate? ItemIconCss { get; set; }

	/// <summary>
	/// Allows adding additional css class names for the item icon. Default: empty string
	/// </summary>
	public ItemStyleDelegate? ItemIconColorCss { get; set; }

	/// <summary>
	/// Allows showing an image as the node icon. Default: null
	/// </summary>
	/// <remarks>
	/// <para>
	/// The <see cref="ItemIconImageUrl"/> takes priority over of the <see cref="ItemIconCss"/>.
	/// </para>
	/// </remarks>
	public ItemStyleDelegate? ItemIconImageUrl { get; set; }

	/// <summary>
	/// Allows adding additional css class names for the item badge. Default: text-bg-primary
	/// </summary>
	/// <remarks>
	/// Called when either <see cref="DisplayCustomCount"/> or
	/// <see cref="DisplayChildrenCount"/> is <see langword="true"/>.
	/// </remarks>
	public ItemStyleDelegate? ItemBadgeColorCss { get; set; }

	/// <summary>
	/// A user provided value to be displayed in the badge.
	/// </summary>
	public int CustomCount { get; set; }

	/// <summary>
	/// Indicate if a badge should be displayed, showing the <see cref="CustomCount"/>.
	/// </summary>
	public bool DisplayCustomCount { get; set; }

	/// <summary>
	/// Indicate if a badge should be displayed, showing the count of child nodes.
	/// </summary>
	/// <remarks>
	/// If <see cref="DisplayCustomCount"/> is true, then this value is ignored.
	/// </remarks>
	public bool DisplayChildrenCount { get; set; }

	/// <summary>
	/// If this instance contains any children.
	/// </summary>
	public bool HasChildren {
		get {
			return this.Children.Count != 0;
		}
	}

	/// <summary>
	/// The collection of children items associated with this instance.
	/// </summary>
	public ICollection<TreeViewModel> Children { get; set; } = [];

	internal bool HasSelectedChild => this.SelectedChildCount > 0;
	internal int SelectedChildCount { get; private set; }
	internal void DecrementSelectedChildCount() {
		if (this.SelectedChildCount > 0) {
			this.SelectedChildCount--;
		}
	}
	internal void IncrementSelectedChildCount() {
		this.SelectedChildCount++;
	}
	internal bool HasHiddenSelectedChild { get; set; }

}