namespace GPW
{
	public enum ServerType
	{
		Lobby = 0,
		Battle = 1,
		Max = 2
	}

	public class NetworkService : Service<NetworkService>
	{
		public SocketConnection LobbyServerConnection { get; private set; }
		public SocketConnection BattleServerConnection { get; private set; }

		public override void Init()
		{
			LobbyServerConnection = new SocketConnection();
			BattleServerConnection = new SocketConnection();
		}
	}
}