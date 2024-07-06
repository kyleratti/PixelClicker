using FruityFoundation.DataAccess.Abstractions;

namespace PixelClicker.Infra.DatabaseMaintenance.Migrations;

public interface IDbScript
{
	public Task Execute(IDatabaseConnection<ReadWrite> connection);
}
