// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage(
	"Style",
	"IDE0130:Namespace does not match folder structure",
	Justification = "We purposely collocate assets and component in a folder that's not part of the namespace",
	Scope = "namespace",
	Target = "~N:Cirreum.Components")]

[assembly: SuppressMessage(
	"Usage",
	"CA1816:Dispose methods should call SuppressFinalize",
	Justification = "It's blazor... it doesn't have a finalizer")]