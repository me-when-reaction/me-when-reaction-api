using MeWhenAPI;
using MeWhenAPI.Import;

var builder = WebApplication.CreateBuilder(args);
builder.AddApplicationFlow();

// ImportScript.Import();
// return;

var app = builder.Build();

app.UseAuthentication();
app.UseCors(o => o.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
app.UseAuthorization();
app.UseSwagger().UseSwaggerUI();
app.MapControllers().RequireAuthorization();
app.UseStaticFiles();

app.Run();


