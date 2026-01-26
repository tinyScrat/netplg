namespace MyApp.Domain;

using MyApp.Domain.Abstractions;

public sealed class ProductReleaseStatus : Enumeration
{
    public static readonly ProductReleaseStatus Initiated = new(0, nameof(Initiated));
    public static readonly ProductReleaseStatus Submitted = new(1, nameof(Submitted));
    public static readonly ProductReleaseStatus Approved = new(2, nameof(Approved));
    public static readonly ProductReleaseStatus Released = new(3, nameof(Released));
    public static readonly ProductReleaseStatus Reopened = new(4, nameof(Reopened));
    public static readonly ProductReleaseStatus Archived = new(5, nameof(Archived));

    private ProductReleaseStatus(int value, string name) : base(value, name) { }

    // Optional behavior inside enum
    public bool CanApprove() => this == Submitted;
    public bool CanSubmit() => this == Initiated;
    public bool CanRelease() => this == Approved;
    public bool CanReopen() => this == Released || this == Archived;
    public bool CanArchive() => this == Released;
}
