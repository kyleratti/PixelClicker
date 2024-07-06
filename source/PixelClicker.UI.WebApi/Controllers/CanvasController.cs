using FruityFoundation.DataAccess.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PixelClicker.Core.Contracts;
using PixelClicker.Core.Models;
using PixelClicker.UI.WebApi.Options;
using PixelClicker.UI.WebApi.Services;

namespace PixelClicker.UI.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class CanvasController : ControllerBase
{
	private readonly IOptions<PixelCanvasOptions> _canvasOptions;
	private readonly IDbConnectionFactory _dbConnectionFactory;
	private readonly IPixelCanvasRepository _pixelCanvasRepo;

	public CanvasController(
		IOptions<PixelCanvasOptions> canvasOptions,
		IDbConnectionFactory dbConnectionFactory,
		IPixelCanvasRepository pixelCanvasRepo
	)
	{
		_canvasOptions = canvasOptions;
		_pixelCanvasRepo = pixelCanvasRepo;
		_dbConnectionFactory = dbConnectionFactory;
	}

	[HttpGet]
	[Route("")]
	public async Task<IActionResult> GetFullCanvas(CancellationToken cancellationToken)
	{
		await using var connection = _dbConnectionFactory.CreateReadOnlyConnection();
		var canvas = await _pixelCanvasRepo.GetCanvasSnapshot(connection, cancellationToken).ToArrayAsync(cancellationToken);

		return new JsonResult(new
		{
			canvas = ConvertToJaggedArray(canvas),
			width = _canvasOptions.Value.Width,
			height = _canvasOptions.Value.Height,
		});
	}

	private string[][] ConvertToJaggedArray(CanvasPixelDto[] pixels)
	{
		var maxX = _canvasOptions.Value.Width;
		var maxY = _canvasOptions.Value.Height;

		var newArray = new string[maxY + 1][];
		for (var i = 0; i < maxY + 1; i++)
		{
			newArray[i] = new string[maxX + 1];
		}

		foreach (var pixel in pixels)
		{
			newArray[pixel.Y][pixel.X] = pixel.ColorHex;
		}

		return newArray;
	}
}
