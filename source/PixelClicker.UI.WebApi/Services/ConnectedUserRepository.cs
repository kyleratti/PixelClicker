namespace PixelClicker.UI.WebApi.Services;

public class ConnectedUserRepository
{
	private long _connectedUserCount = 0L;

	public long IncrementConnectedUserCount()
	{
		return Interlocked.Increment(ref _connectedUserCount);
	}

	public long DecrementConnectedUserCount()
	{
		return Interlocked.Decrement(ref _connectedUserCount);
	}
}
