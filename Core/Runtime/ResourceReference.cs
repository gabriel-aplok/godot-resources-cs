namespace GodotResources.Core.Runtime;

/// <summary>
/// Represents a resource reference.
/// </summary>
public abstract record ResourceReference(string Id);

public sealed record ExtResourceReference(string Id) : ResourceReference(Id);

public sealed record SubResourceReference(string Id) : ResourceReference(Id);
