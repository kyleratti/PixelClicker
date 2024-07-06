using FruityFoundation.DataAccess.Abstractions;
using FruityFoundation.DataAccess.Core;
using Microsoft.Data.Sqlite;
using PixelClicker.Core.Contracts;
using PixelClicker.Infra.Repository;
using PixelClicker.UI.WebApi.Hubs;
using PixelClicker.UI.WebApi.Options;
using PixelClicker.UI.WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<PixelCanvasOptions>(builder.Configuration.GetSection("PixelCanvas"));
builder.Services.AddScoped<IPixelCanvasRepository, PixelCanvas.PixelCanvasRepository>();
builder.Services.AddSingleton<ConnectedUserRepository>();

builder.Services.AddDataAccessCore(
	readWriteConnectionImplementationFactory: serviceProvider => GetDbImplementation<ReadWrite>(serviceProvider, "Database"),
	readOnlyConnectionImplementationFactory: serviceProvider => GetDbImplementation<ReadOnly>(serviceProvider, "Database_ReadOnly"));

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

return;

static INonTransactionalDbConnection<TConnectionType> GetDbImplementation<TConnectionType>(IServiceProvider serviceProvider, string connectionStringName) where TConnectionType : ConnectionType
{
	var config = serviceProvider.GetRequiredService<IConfiguration>();
	var connectionString = config.GetConnectionString(connectionStringName);

	if(string.IsNullOrEmpty(connectionString))
		throw new InvalidOperationException($"Connection string '{connectionStringName}' not found.");

	var sqliteConnection = new SqliteConnection(connectionString);
	var dbConnection = new NonTransactionalDbConnection<TConnectionType>(sqliteConnection);

	return dbConnection;
}
