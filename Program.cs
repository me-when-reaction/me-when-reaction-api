using MeWhen.Import;
using MeWhen.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using MediatR;
using MeWhen.Service.Pipe;
using MeWhen.Domain.Configuration;

internal class Program
{
    public static IConfiguration Config { get; private set; } = default!;

    public static Supabase.Interfaces.ISupabaseClient<Supabase.Gotrue.User, Supabase.Gotrue.Session, Supabase.Realtime.RealtimeSocket, Supabase.Realtime.RealtimeChannel, Supabase.Storage.Bucket, Supabase.Storage.FileObject> Supabase { get; private set; } = default!;
    
    private static async Task Main(string[] args)
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
        
        var supabaseConfig = Config.GetSection("Supabase:Service").Get<SupabaseConfiguration>() ?? throw new Exception("Supabase service not found");

        Config = builder.Configuration;
        Supabase = await new Supabase.Client(
            supabaseConfig.URL,
            supabaseConfig.Key,
            new()
            {
                AutoConnectRealtime = true
            }
        ).InitializeAsync();

        var app = builder.Build();

        app.UseSwagger().UseSwaggerUI();
        app.MapControllers();

        // ImportScript.Import();
        app.Run();
    }
}


