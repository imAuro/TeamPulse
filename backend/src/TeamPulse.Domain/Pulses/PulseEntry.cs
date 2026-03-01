namespace TeamPulse.Domain.Pulses;

public sealed class PulseEntry
{
    public Guid Id { get; private set; }
    public int Score { get; private set; }
    public string? Comment { get; private set; }
    public Guid CategoryId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private PulseEntry()
    {
    }

    private PulseEntry(Guid id, int score, string? comment, Guid categoryId, DateTimeOffset createdAt)
    {
        Id = id;
        Score = score;
        Comment = comment;
        CategoryId = categoryId;
        CreatedAt = createdAt;
    }

    public const int MaxCommentLength = 500;

    public static PulseEntry Create(int score, string? comment, Guid categoryId, DateTimeOffset now)
    {
        if (score is < 1 or > 5) throw new DomainException("Score must be between 1 and 5.");
        if (categoryId == Guid.Empty) throw new DomainException("CategoryId is required.");

        if (comment is not null)
        {
            comment = comment.Trim();
            if (comment.Length == 0) comment = null;
            if (comment is not null && comment.Length > MaxCommentLength)
                throw new DomainException($"Comment max length is {MaxCommentLength}.");
        }

        return new PulseEntry(Guid.NewGuid(), score, comment, categoryId, now);
    }
}