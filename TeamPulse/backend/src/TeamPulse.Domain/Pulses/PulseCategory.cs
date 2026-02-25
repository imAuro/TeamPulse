namespace TeamPulse.Domain.Pulses;

public sealed class PulseCategory
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;

    private PulseCategory() { } 

    public PulseCategory(Guid id, string name)
    {
        if (id == Guid.Empty) throw new DomainException("Category id cannot be empty.");
        if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Category name is required.");
        if (name.Length > 100) throw new DomainException("Category name max length is 100.");

        Id = id;
        Name = name.Trim();
    }
}