using FruityFoundation.DataAccess.Abstractions;
using PixelClicker.Infra.DatabaseMaintenance.Migrations;
using PixelClicker.Infra.DatabaseMaintenance.Util;

namespace AYI.Core.DatabaseMaintenance.Migrations;

public class Script_2024_07_05T21_33_SetupDatabase : IDbScript
{
	/// <inheritdoc />
	public async Task Execute(IDatabaseConnection<ReadWrite> connection)
	{
		var canvasTableExists = await DbSchemaUtil.CheckIfTableExists(connection, "pixel_canvas");

		if (canvasTableExists)
			return;

		await connection.Execute(@"
			create table pixel_canvas
(
	pixel_id     integer not null
		constraint PK_pixel_canvas
			primary key autoincrement,
	x_coordinate integer not null,
	y_coordinate integer not null,
	color_hex    TEXT    not null,
	created_at   TEXT    not null
);

create index idx_pixel_coordinates
	on pixel_canvas (x_coordinate, y_coordinate, color_hex, pixel_id);

CREATE VIEW latest_pixel_canvas as
SELECT
    pc1.x_coordinate
    ,pc1.y_coordinate
    ,pc1.color_hex
    ,pc1.pixel_id AS max_pixel_id
FROM pixel_canvas pc1
LEFT JOIN pixel_canvas pc2
ON pc1.x_coordinate = pc2.x_coordinate 
AND pc1.y_coordinate = pc2.y_coordinate 
AND pc1.pixel_id < pc2.pixel_id
WHERE pc2.pixel_id IS NULL;
");
	}
}
