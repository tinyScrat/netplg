namespace WebApp.Domain;

using WebApp.Domain.Abstractions;

public sealed class ProductReleaseStatus : Enumeration
{
    public static readonly ProductReleaseStatus Initiated = new(0, nameof(Initiated));
    public static readonly ProductReleaseStatus Submitted = new(1, "Submitted");
    public static readonly ProductReleaseStatus Approved = new(2, "Approved");
    public static readonly ProductReleaseStatus Released = new(3, "Released");
    public static readonly ProductReleaseStatus Reopened = new(4, "Reopened");
    public static readonly ProductReleaseStatus Archived = new(5, "Archived");

    private ProductReleaseStatus(int value, string name) : base(value, name) { }

    // Optional behavior inside enum
    public bool CanApprove() => this == Submitted;
    public bool CanSubmit() => this == Initiated;
    public bool CanRelease() => this == Approved;
    public bool CanReopen() => this == Released || this == Archived;
    public bool CanArchive() => this == Released;
}
