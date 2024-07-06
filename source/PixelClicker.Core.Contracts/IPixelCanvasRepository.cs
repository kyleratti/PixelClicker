using FruityFoundation.DataAccess.Abstractions;
using PixelClicker.Core.Models;

namespace PixelClicker.Core.Contracts;

public interface IPixelCanvasRepository
{
	public Task SetPixelColor(IDatabaseConnection<ReadWrite> connection, int x, int y, string color, CancellationToken cancellationToken);
	public IAsyncEnumerable<CanvasPixelDto> GetCanvasSnapshot(IDatabaseConnection<ReadOnly> connection, CancellationToken cancellationToken);
}
