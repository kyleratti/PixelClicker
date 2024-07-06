namespace PixelClicker.Core.Models;

public record CanvasPixel(
	(int X, int Y) Coordinates,
	HexColor Color,
	int MaxPixelId);
