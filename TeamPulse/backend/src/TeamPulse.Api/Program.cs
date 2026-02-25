using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeamPulse.Api.Endpoints;
using TeamPulse.Application.Common.Abstractions;
using TeamPulse.Application.Pulses.SubmitPulse;
using TeamPulse.Domain;
using TeamPulse.Infrastructure.Persistence;
using TeamPulse.Infrastructure.Persistence.Seed;
using TeamPulse.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", p =>
        p.WithOrigins("http://localhost:3000")
         .AllowAnyHeader()
         .AllowAnyMethod());
});

builder.Services.AddDbContext<PulseDbContext>(opt =>
    opt.UseInMemoryDatabase("TeamPulseDb"));

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(SubmitPulseCommand).Assembly));
builder.Services.AddValidatorsFromAssembly(typeof(SubmitPulseCommand).Assembly);

builder.Services.AddScoped<IPulseRepository, PulseRepository>();

var app = builder.Build();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var feature = context.Features.Get<IExceptionHandlerFeature>();
        var ex = feature?.Error;
        var traceId = context.TraceIdentifier;

        ProblemDetails problem;

        switch (ex)
        {
            case ValidationException ve:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                problem = new ValidationProblemDetails(
                    ve.Errors.GroupBy(e => e.PropertyName)
                             .ToDictionary(g => g.Key, g => g.Select(x => x.ErrorMessage).ToArray()))
                {
                    Title = "Validation error",
                    Status = StatusCodes.Status400BadRequest,
                    Extensions = { ["traceId"] = traceId }
                };
                break;

            case DomainException de:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                problem = new ProblemDetails
                {
                    Title = "Domain rule violation",
                    Detail = de.Message,
                    Status = StatusCodes.Status400BadRequest,
                    Extensions = { ["traceId"] = traceId }
                };
                break;

            case ArgumentException ae:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                problem = new ProblemDetails
                {
                    Title = "Bad request",
                    Detail = ae.Message,
                    Status = StatusCodes.Status400BadRequest,
                    Extensions = { ["traceId"] = traceId }
                };
                break;

            default:
                if (ex is not null)
                    app.Logger.LogError(ex, "Unhandled exception. TraceId={TraceId}", traceId);

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                problem = new ProblemDetails
                {
                    Title = "Unexpected error",
                    Detail = "Something went wrong.",
                    Status = StatusCodes.Status500InternalServerError,
                    Extensions = { ["traceId"] = traceId }
                };
                break;
        }

        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsJsonAsync(problem);
    });
});

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("AllowFrontend");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PulseDbContext>();
    await CategorySeeder.SeedAsync(db, CancellationToken.None);
}

app.MapPulseEndpoints();

app.Run();