namespace Cirreum.Components;

/// <summary>
/// Ccontains the standard Styling and Css classes for Toast elements.
/// </summary>
public static class ToastStyles {

	/// <summary>
	/// The deafult position of a toast (top-right)
	/// </summary>
	public const ToastPosition Position = ToastPosition.TopRight;

	/// <summary>
	/// Css Classes
	/// </summary>
	public static class Classes {

		/// <summary>
		/// The Message Css class
		/// </summary>
		public const string MessageCss = "toast-message";
		/// <summary>
		/// The Close Button Css Classes
		/// </summary>
		public const string CloseButtonCss = "ms-2 mb-1 close btn-close toast-close-button";

		/// <summary>
		/// Available Css Classes for Toast positioning
		/// </summary>
		public static class Positions {
			public const string TopCenter = "toast-top-center";
			public const string BottomCenter = "toast-bottom-center";
			public const string TopFullWidth = "toast-top-full-width";
			public const string BottomFullWidth = "toast-bottom-full-width";
			public const string TopLeft = "toast-top-left";
			public const string TopRight = "toast-top-right";
			public const string BottomRight = "toast-bottom-right";
			public const string BottomLeft = "toast-bottom-left";
		}

	}

}