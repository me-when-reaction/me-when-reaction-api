using MeWhenAPI;
using MeWhenAPI.Service.Pipe;

var builder = WebApplication.CreateBuilder(args);
builder.AddApplicationFlow();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();
app.UseAuthentication();
app.UseCors(o => o.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
app.UseAuthorization();
app.UseSwagger().UseSwaggerUI();
app.MapControllers().RequireAuthorization();
app.UseStaticFiles();

app.Run();


