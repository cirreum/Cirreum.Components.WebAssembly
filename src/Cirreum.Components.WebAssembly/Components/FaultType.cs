namespace Cirreum.Components;

/// <summary>
/// Represents types of faults that can occur in a system.
/// </summary>
public enum FaultType {
	/// <summary>
	/// A temporary fault that may resolve itself over time or with retry attempts.
	/// </summary>
	/// <remarks>
	/// Transient faults are typically caused by temporary conditions such as network timeouts,
	/// momentary service unavailability, or resource contention that may clear up on subsequent attempts.
	/// </remarks>
	Transient,

	/// <summary>
	/// A persistent fault that requires intervention to resolve.
	/// </summary>
	/// <remarks>
	/// Permanent faults indicate systemic issues that will not resolve themselves,
	/// such as invalid configuration, missing dependencies, or hardware failures.
	/// </remarks>
	Permanent
}