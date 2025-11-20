namespace Cirreum.Components;

using System;

/// <summary>
/// Optional Flag Enum indicate the supported or desired export format types.
/// </summary>
[Flags]
public enum ExportType {
	Csv,
	Excel,
	PDF,
	Word
}