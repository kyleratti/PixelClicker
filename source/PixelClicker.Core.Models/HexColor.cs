namespace PixelClicker.Core.Models;

/// <summary>
/// A HEX color.
/// </summary>
/// <param name="Value">The hex color <i>with</i> preceding <c>#</c>.</param>
public readonly record struct HexColor(string Value);
