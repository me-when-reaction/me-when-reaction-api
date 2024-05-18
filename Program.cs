using MeWhen;
using MeWhen.Import;

var builder = WebApplication.CreateBuilder(args);
builder.AddApplicationFlow();

// ImportScript.Import();
// return;

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.UseSwagger().UseSwaggerUI();
app.MapControllers().RequireAuthorization();

app.Run();

