using FruityFoundation.DataAccess.Abstractions;
using PixelClicker.Infra.DatabaseMaintenance.Migrations;
using PixelClicker.Infra.DatabaseMaintenance.Util;

namespace AYI.Core.DatabaseMaintenance.Migrations;

public class Script_2024_07_05T21_32_SetupMigrations : IDbScript
{
	/// <inheritdoc />
	public async Task Execute(IDatabaseConnection<ReadWrite> connection)
	{
		var migrationsTableExists = await DbSchemaUtil.CheckIfTableExists(connection, "db_migrations");

		if (migrationsTableExists)
			return;

		await connection.Execute(@"
			create table db_migrations
			(
			    run_id            INTEGER
			        constraint PK_db_migrations_run_id
			            primary key autoincrement,
			    script_name       TEXT    not null,
			    is_success        INTEGER not null,
			    ran_at            TEXT    not null,
			    error_message     TEXT,
			    error_stack_trace TEXT
			);");
	}
}
