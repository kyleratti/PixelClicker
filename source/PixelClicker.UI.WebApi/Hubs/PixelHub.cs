using FruityFoundation.DataAccess.Abstractions;
using Microsoft.AspNetCore.SignalR;
using PixelClicker.Core.Contracts;
using PixelClicker.UI.WebApi.Services;

namespace PixelClicker.UI.WebApi.Hubs;

public class PixelHub : Hub
{
	private readonly ConnectedUserRepository _connectedUserRepository;
	private readonly IPixelCanvasRepository _pixelCanvasRepo;
	private readonly IDbConnectionFactory _dbConnectionFactory;

	public PixelHub(
		ConnectedUserRepository connectedUserRepository,
		IPixelCanvasRepository pixelCanvasRepo,
		IDbConnectionFactory dbConnectionFactory
	)
	{
		_connectedUserRepository = connectedUserRepository;
		_pixelCanvasRepo = pixelCanvasRepo;
		_dbConnectionFactory = dbConnectionFactory;
	}

	public class NewPixelData
	{
		public int? X { get; set; }
		public int? Y { get; set; }
		public string? HexColor { get; set; }
	}

	public async Task NewPixel(NewPixelData data)
	{
		if (data.X is not { } x)
			throw new InvalidOperationException();
		if (data.Y is not { } y)
			throw new InvalidOperationException();
		if (string.IsNullOrEmpty(data.HexColor))
			throw new InvalidOperationException();

		await using (var connection = _dbConnectionFactory.CreateConnection())
		{
			await _pixelCanvasRepo.SetPixelColor(connection, x, y, data.HexColor, CancellationToken.None);
		}

		await Clients.All.SendAsync("NewPixel", data);
	}

	/// <inheritdoc />
	public override async Task OnConnectedAsync()
	{
		var newUserCount = _connectedUserRepository.IncrementConnectedUserCount();

		await Clients.All.SendAsync("UserCount", newUserCount);

		await base.OnConnectedAsync();
	}

	/// <inheritdoc />
	public override async Task OnDisconnectedAsync(Exception? exception)
	{
		var newUserCount = _connectedUserRepository.DecrementConnectedUserCount();

		await Clients.AllExcept(Context.ConnectionId).SendAsync("UserCount", newUserCount);

		await base.OnDisconnectedAsync(exception);
	}
}
