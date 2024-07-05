using FruityFoundation.Base.Structures;
using Microsoft.Extensions.Options;
using PixelClicker.UI.WebApi.Options;

namespace PixelClicker.UI.WebApi.Services;

public class PixelDataService
{
	private readonly IOptions<PixelCanvasOptions> _canvasOptions;

	private string[,] Canvas { get; }

	public PixelDataService(IOptions<PixelCanvasOptions> canvasOptions)
	{
		_canvasOptions = canvasOptions;
		Canvas = new string[canvasOptions.Value.Height, canvasOptions.Value.Width];
	}

	public string[,] GetCanvas()
	{
		// Return a duplicate of the canvas to prevent modification
		return (string[,])Canvas.Clone();
	}

	public Task<Maybe<string>> GetPixel(int x, int y)
	{
		if (x < 0 || x >= _canvasOptions.Value.Width || y < 0 || y >= _canvasOptions.Value.Height)
		{
			throw new ArgumentOutOfRangeException();
		}

		var value = Canvas[y, x];

		if (string.IsNullOrEmpty(value))
			return Task.FromResult(Maybe.Empty<string>());

		return Task.FromResult(Maybe.Create(value));
	}

	public Task SetPixel(int x, int y, string hexColor)
	{
		if(x < 0)
			throw new InvalidOperationException("Cannot use negative values for x");
		else if(x >= _canvasOptions.Value.Width)
			throw new InvalidOperationException("Cannot use values greater than the width of the canvas for x");
		else if(y < 0)
			throw new InvalidOperationException("Cannot use negative values for y");
		else if(y >= _canvasOptions.Value.Height)
			throw new InvalidOperationException("Cannot use values greater than the height of the canvas for y");
		else if (string.IsNullOrEmpty(hexColor))
			throw new InvalidOperationException("Cannot use a null or empty hex color");

		Canvas[y, x] = hexColor;

		return Task.CompletedTask;
	}
}
