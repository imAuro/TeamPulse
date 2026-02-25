using FluentAssertions;
using TeamPulse.Domain;
using TeamPulse.Domain.Pulses;
using Xunit;

namespace TeamPulse.Tests.Domain;

public class PulseEntryTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    [InlineData(-1)]
    public void Create_ShouldThrow_WhenScoreOutOfRange(int score)
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var now = DateTimeOffset.UtcNow;

        // Act
        var act = () => PulseEntry.Create(score, comment: null, categoryId, now);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*between 1 and 5*");
    }

    [Fact]
    public void Create_ShouldThrow_WhenCategoryIdEmpty()
    {
        var now = DateTimeOffset.UtcNow;

        var act = () => PulseEntry.Create(3, comment: null, Guid.Empty, now);

        act.Should().Throw<DomainException>()
            .WithMessage("*CategoryId is required*");
    }

    [Fact]
    public void Create_ShouldTrimComment_AndConvertEmptyToNull()
    {
        var categoryId = Guid.NewGuid();
        var now = DateTimeOffset.UtcNow;

        var entry = PulseEntry.Create(4, comment: "   ", categoryId, now);

        entry.Comment.Should().BeNull();
    }

    [Fact]
    public void Create_ShouldThrow_WhenCommentTooLong()
    {
        var categoryId = Guid.NewGuid();
        var now = DateTimeOffset.UtcNow;
        var tooLong = new string('a', PulseEntry.MaxCommentLength + 1);

        var act = () => PulseEntry.Create(4, tooLong, categoryId, now);

        act.Should().Throw<DomainException>()
            .WithMessage($"*{PulseEntry.MaxCommentLength}*");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(5)]
    public void Create_ShouldSucceed_WhenInputsValid(int score)
    {
        var categoryId = Guid.NewGuid();
        var now = DateTimeOffset.UtcNow;

        var entry = PulseEntry.Create(score, " ok ", categoryId, now);

        entry.Id.Should().NotBe(Guid.Empty);
        entry.Score.Should().Be(score);
        entry.CategoryId.Should().Be(categoryId);
        entry.CreatedAt.Should().Be(now);
        entry.Comment.Should().Be("ok"); 
    }
}