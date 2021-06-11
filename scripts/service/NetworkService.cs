using System.Net.Sockets;
using System.Net;
using System;
using System.Collections.Generic;

using GPW.NetMessage;

namespace GPW
{
	public enum ServerType
	{
		Lobby = 0,
		Battle = 1,
		Max = 2
	}
	namespace NetConnections
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

	public abstract class NetworkMsgHandler
	{
		public NetMessage.MsgType msgType;
		public abstract void Handle(NetMessage.ServerMsg msg);

		private class ImplicitHandler<T> : CommonNetworkMsgHandler<T>
		{
			public Func<ServerMsg, T> dataField = null;
			public Action<T> onData = null;
			public Action<ErrorType> onError = null;
			public override T GetData(ServerMsg msg) => null == dataField ? default(T) : dataField.Invoke(msg);
			public override void Handle(T data) => onData?.Invoke(data);
			public override void HandleError(ErrorType err) => this.onError?.Invoke(err);

		}
		public static NetworkMsgHandler Create<T>(NetMessage.MsgType msgType, Func<ServerMsg, T> dataField, Action<T> onData, Action<ErrorType> onError)
		{ return new ImplicitHandler<T> { msgType = msgType, dataField = dataField, onData = onData, onError = onError }; }
	}

	public abstract class CommonNetworkMsgHandler<T> : NetworkMsgHandler
	{
		public override void Handle(ServerMsg msg)
		{
			if (null == msg || msg.DataType != msgType) return;
			if (msg.ErrorType != ErrorType.ErrNoerr)
				HandleError(msg.ErrorType);
			else
			{
				var data = GetData(msg);
				if (null == data) return;
				Handle(data);
			}
		}
		public abstract T GetData(NetMessage.ServerMsg msg);
		public abstract void Handle(T data);
		public abstract void HandleError(NetMessage.ErrorType err);
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