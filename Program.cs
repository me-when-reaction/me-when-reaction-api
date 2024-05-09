using MeWhen.Import;
using MeWhen.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using MediatR;
using MeWhen.Service.Pipe;

internal class Program
{
    public static IConfiguration Config { get; private set; } = default!;
    
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddDbContext<MeWhenDBContext>(opt =>
        {
            opt.UseNpgsql(builder.Configuration.GetConnectionString("Default"))
                .EnableSensitiveDataLogging(true);
        });

        builder.Services.AddControllers()
            .AddJsonOptions(opt => opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
        
        builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly)
            .AddEndpointsApiExplorer()
            .AddSwaggerGen(config => {
                config.SwaggerDoc("v1", new OpenApiInfo(){
                    Title = "Me When API",
                    Version = "v1"
                });
            })
            .AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(typeof(Program).Assembly);
                builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidatorPipeline<,>));
            });

        Config = builder.Configuration;
        var app = builder.Build();

        app.UseSwagger().UseSwaggerUI();
        app.MapControllers();

        // ImportScript.Import();
        app.Run();
    }
}


