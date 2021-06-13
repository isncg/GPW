using System.Net;
using System.Net.Sockets;

namespace GPW
{
	public static class SocketUtils
	{
		public static Socket CreateConectedTcpSocketFromConfig(string serverName)
		{
			var cfg = ConfigService.Instance.Find<Config.CfgServer>(i => i.name == serverName);
			if (null == cfg) return null;
			return CreateConnectedTcpSocket(cfg.host, cfg.port);
		}

		public static Socket CreateConnectedTcpSocket(string host, int port)
		{
			if (!IPAddress.TryParse(host, out var addr)) return null;
			IPEndPoint iep = new IPEndPoint(addr, port);
			Socket sock = new Socket(SocketType.Stream, ProtocolType.Tcp);
			sock.Connect(iep);
			return sock;
		}

		public static Socket CreateConnectedUdpSocket(string host, int port)
		{
			if (!IPAddress.TryParse(host, out var addr)) return null;
			IPEndPoint iep = new IPEndPoint(addr, port);
			Socket sock = new Socket(SocketType.Dgram, ProtocolType.Udp);
			sock.Connect(iep);
			return sock;
		}
	}
}