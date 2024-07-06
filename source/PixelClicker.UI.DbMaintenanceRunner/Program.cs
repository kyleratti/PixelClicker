// See https://aka.ms/new-console-template for more information

using FruityFoundation.DataAccess.Abstractions;
using FruityFoundation.DataAccess.Core;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PixelClicker.Infra.DatabaseMaintenance;
using PixelClicker.UI.DbMaintenanceRunner.Services;

var builder = Host.CreateDefaultBuilder(args)
	.ConfigureAppConfiguration((context, configBuilder) =>
	{
		configBuilder
			.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
			.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true);

		if (context.HostingEnvironment.IsDevelopment())
			configBuilder.AddUserSecrets<Program>();

		var config = configBuilder.Build();

		var appConfigConnectionString = config.GetConnectionString("AzureAppConfig");

		if (string.IsNullOrEmpty(appConfigConnectionString))
			return;

		configBuilder.AddAzureAppConfiguration(appConfig =>
		{
			appConfig.Connect(appConfigConnectionString)
				.Select(keyFilter: "AYI:*")
				.Select(keyFilter: "AYI:*", labelFilter: context.HostingEnvironment.EnvironmentName)
				.TrimKeyPrefix("AYI:");
		});
	})
	.ConfigureLogging((_, loggingBuilder) =>
	{
		loggingBuilder.ClearProviders();
		loggingBuilder.AddConsole();
		loggingBuilder.AddDebug();
	})
	.ConfigureServices((context, services) =>
	{
		services.AddSingleton<CliRunner>();

		services.AddSingleton<MigrationRunner>();
		services.AddDataAccessCore(
			readWriteConnectionImplementationFactory: serviceProvider => GetDbImplementation<ReadWrite>(serviceProvider, "Database"),
			readOnlyConnectionImplementationFactory: serviceProvider => GetDbImplementation<ReadOnly>(serviceProvider, "Database_ReadOnly"));
	});

var app = builder.Build();

var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
await using var scope = scopeFactory.CreateAsyncScope();

var cliRunner = scope.ServiceProvider.GetRequiredService<CliRunner>();

var exitCode = await cliRunner.Run();

return exitCode;

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

