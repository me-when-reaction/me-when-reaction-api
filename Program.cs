using MeWhen.Import;
using MeWhen.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

internal class Program
{
    public static IConfiguration Config { get; private set; } = default!;
    
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddDbContext<MeWhenDBContext>(opt =>
        {
            opt.UseNpgsql(builder.Configuration.GetConnectionString("Default"));
        });

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof(Program).Assembly);
        });

        Config = builder.Configuration;
        var app = builder.Build();

        app.UseSwagger().UseSwaggerUI();
        app.MapControllers();

        // ImportScript.Import();
        app.Run();
    }
}


