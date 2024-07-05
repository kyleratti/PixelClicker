using Microsoft.AspNetCore.SignalR;
using PixelClicker.UI.WebApi.Services;

namespace PixelClicker.UI.WebApi.Hubs;

public class PixelHub : Hub
{
	private readonly PixelDataService _pixelData;

	public PixelHub(PixelDataService pixelData)
	{
		_pixelData = pixelData;
	}

	public class NewPixelData
	{
		public int? X { get; set; }
		public int? Y { get; set; }
		public string? HexColor { get; set; }
	}

	public async Task NewPixel(NewPixelData data)
	{
		if(data.X is not { } x)
			throw new InvalidOperationException();
		if (data.Y is not { } y)
			throw new InvalidOperationException();
		if (string.IsNullOrEmpty(data.HexColor))
			throw new InvalidOperationException();

		await _pixelData.SetPixel(x, y, data.HexColor);
		await Clients.All.SendAsync("NewPixel", data);
	}

	public async Task SendMessage(string message)
	{
		await Clients.All.SendAsync("ReceiveMessage", message);
	}
}
