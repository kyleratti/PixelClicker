using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PixelClicker.UI.WebApi.Options;
using PixelClicker.UI.WebApi.Services;

namespace PixelClicker.UI.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class CanvasController : ControllerBase
{
	private readonly PixelDataService _pixelData;
	private readonly IOptions<PixelCanvasOptions> _canvasOptions;

	public CanvasController(
		PixelDataService pixelData,
		IOptions<PixelCanvasOptions> canvasOptions
	)
	{
		_pixelData = pixelData;
		_canvasOptions = canvasOptions;
	}

	[HttpGet]
	[Route("")]
	public Task<IActionResult> GetFullCanvas()
	{
		var canvas = _pixelData.GetCanvas();

		return Task.FromResult<IActionResult>(new JsonResult(new
		{
			canvas = ConvertToJaggedArray(canvas),
			width = _canvasOptions.Value.Width,
			height = _canvasOptions.Value.Height,
		}));
	}

	private static T[][] ConvertToJaggedArray<T>(T[,] multidimensionalArray)
	{
		var rows = multidimensionalArray.GetLength(0);
		var cols = multidimensionalArray.GetLength(1);

		var jaggedArray = new T[rows][];
		for (var i = 0; i < rows; i++)
		{
			jaggedArray[i] = new T[cols];
			for (var j = 0; j < cols; j++)
			{
				jaggedArray[i][j] = multidimensionalArray[i, j];
			}
		}

		return jaggedArray;
	}
}
