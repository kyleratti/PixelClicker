using PixelClicker.UI.WebApi.Hubs;
using PixelClicker.UI.WebApi.Options;
using PixelClicker.UI.WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<PixelCanvasOptions>(builder.Configuration.GetSection("PixelCanvas"));
builder.Services.AddSingleton<PixelDataService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSignalR();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseCors(x => x
	.WithOrigins("http://localhost:5173")
	.AllowAnyMethod()
	.AllowCredentials()
	.AllowAnyHeader());

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapHub<PixelHub>("/hub");

app.Run();
