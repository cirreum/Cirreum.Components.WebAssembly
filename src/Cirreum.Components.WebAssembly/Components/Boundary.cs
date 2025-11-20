namespace Cirreum.Components;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// The available Boundary values for use with the PopperJS plugin. Options: ClippingParents (default), ViewPort or Window
/// </summary>
public enum Boundary {

	/// <summary>
	/// ClippingParents Boundary
	/// </summary>
	[Display(Name = "clippingParents")]
	ClippingParents,

	/// <summary>
	/// ViewPort Boundary
	/// </summary>
	[Display(Name = "viewPort")]
	ViewPort,

	/// <summary>
	/// Document Boundary
	/// </summary>
	[Display(Name = "document")]
	Document

}