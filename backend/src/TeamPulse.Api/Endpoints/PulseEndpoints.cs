using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TeamPulse.Application.Pulses.GetCategories;
using TeamPulse.Application.Pulses.GetPulseSummary;
using TeamPulse.Application.Pulses.SubmitPulse;

namespace TeamPulse.Api.Endpoints;

public static class PulseEndpoints
{
    public static IEndpointRouteBuilder MapPulseEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/pulse").WithTags("Pulse");

        group.MapPost("", async (
                [FromBody] SubmitPulseRequest req,
                IMediator mediator,
                IValidator<SubmitPulseCommand> validator,
                CancellationToken ct) =>
            {
                var cmd = new SubmitPulseCommand(req.Score, req.Comment, req.CategoryId);

                var validation = await validator.ValidateAsync(cmd, ct);
                if (!validation.IsValid) throw new ValidationException(validation.Errors);

                await mediator.Send(cmd, ct);
                return Results.Created("/api/pulse", new { success = true });
            })
            .Produces(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithOpenApi();

        group.MapGet("/summary", async (IMediator mediator, CancellationToken ct) =>
            {
                var summary = await mediator.Send(new GetPulseSummaryQuery(), ct);
                var scores = summary.Scores.ToDictionary(k => k.Key.ToString(), v => v.Value);

                return Results.Ok(new
                {
                    count = summary.Count,
                    averageScore = summary.AverageScore,
                    scores,
                    categories = summary.Categories.Select(c => new { id = c.Id, name = c.Name, count = c.Count })
                });
            })
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithOpenApi();

        group.MapGet("/categories", async (IMediator mediator, CancellationToken ct) =>
            {
                var cats = await mediator.Send(new GetCategoriesQuery(), ct);
                return Results.Ok(cats);
            })
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithOpenApi();

        return app;
    }

    public sealed record SubmitPulseRequest(int Score, string? Comment, Guid CategoryId);
}