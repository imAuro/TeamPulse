namespace TeamPulse.Application.Pulses.GetPulseSummary;

public sealed record PulseSummaryReadModel(
    int Count,
    decimal AverageScore,
    IReadOnlyDictionary<int, int> Scores,
    IReadOnlyList<CategoryCount> Categories);

public sealed record CategoryCount(Guid Id, string Name, int Count);