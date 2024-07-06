using PixelClicker.Infra.DatabaseMaintenance;

namespace PixelClicker.UI.WebApi.HostedServices;

public class MigrationRunnerService : BackgroundService
{
	private readonly ILogger<MigrationRunnerService> _logger;
	private readonly MigrationRunner _migrationRunner;

	public MigrationRunnerService(ILogger<MigrationRunnerService> logger, MigrationRunner migrationRunner)
	{
		_logger = logger;
		_migrationRunner = migrationRunner;
	}

	/// <inheritdoc />
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		try
		{
			var result = await _migrationRunner.RunMigrations();

			if (result is not ExitStatus.Successful)
				throw new Exception($"Migration failed with exit status: {result}");

			_logger.LogInformation("Migrations ran successfully");
		}
		catch (Exception ex)
		{
			_logger.LogCritical(ex, "Failed to run migrations");
		}
	}
}
