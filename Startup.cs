using System.Text;
using FluentValidation;
using MediatR;
using MeWhenAPI.Domain.Configuration;
using MeWhenAPI.Infrastructure.Context;
using MeWhenAPI.Infrastructure.Utilities;
using MeWhenAPI.Service.Pipe;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace MeWhenAPI
{
    public static class Startup
    {
        public static void AddApplicationFlow(this WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<MeWhenDBContext>(opt =>
            {
                opt.UseNpgsql(builder.Configuration.GetConnectionString("Default"));
            });

            builder.Services.AddControllers()
                .AddJsonOptions(opt => {
                    opt.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                });
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opt =>
                {
                    opt.TokenValidationParameters = new()
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("Supabase:JWT:Secret")!)),
                        ValidIssuer = builder.Configuration.GetValue<string>("Supabase:JWT:Issuer")!,
                        ValidAudience = builder.Configuration.GetValue<string>("Supabase:JWT:Audience")!
                    };
                });

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSingleton<Supabase.Client>(x => new(
                    builder.Configuration.GetValue<string>("Supabase:Service:URL")!,
                    builder.Configuration.GetValue<string>("Supabase:Service:Key")!,
                    new()
                    {
                        AutoConnectRealtime = true,
                        AutoRefreshToken = true
                    }
                )
            );

            builder.Services.AddScoped<IAuthUtilities, AuthUtilities>();
            builder.Services.AddScoped<IFileUtilities, FileUtilities>();

            builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly)
                .AddEndpointsApiExplorer()
                .AddSwaggerGen(config =>
                {
                    config.SwaggerDoc("v1", new OpenApiInfo()
                    {
                        Title = "Me When API",
                        Version = "v1"
                    });
                })
                .AddMediatR(config =>
                {
                    config.RegisterServicesFromAssembly(typeof(Program).Assembly);

                });
            builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidatorPipeline<,>));
            builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidatorNoResponsePipeline<,>));
            builder.Services.Configure<StorageConfiguration>(builder.Configuration.GetSection("Supabase:Storage"));
            builder.Services.Configure<SupabaseConfiguration>(builder.Configuration.GetSection("Supabase:Service"));
            builder.Services.AddCors(opt => {
                opt.AddPolicy(
                    name: "Default",
                    policy => {
                        policy.AllowAnyHeader()
                            .AllowAnyOrigin()
                            .AllowAnyMethod();
                    }
                );
            });
        }
    }
}