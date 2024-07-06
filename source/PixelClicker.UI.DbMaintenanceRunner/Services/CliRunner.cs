using PixelClicker.Infra.DatabaseMaintenance;

namespace PixelClicker.UI.DbMaintenanceRunner.Services;

public class CliRunner(
	MigrationRunner _migrationRunner
)
{
	public async Task<int> Run()
	{
		var exitStatus = await _migrationRunner.RunMigrations();

		return exitStatus switch
		{
			ExitStatus.Successful => 0,
			ExitStatus.ScriptError => 1,
			ExitStatus.UnknownError => 2,
			_ => throw new NotImplementedException($"Unhandled exit status: {exitStatus:G} ({exitStatus:D})"),
		};
	}
}
