using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using Google.Protobuf;

namespace GPW
{
	public class NetworkMsgHandlerRegister
	{
		public readonly Dictionary<int, HashSet<NetworkMsgHandler>> handlers = new Dictionary<int, HashSet<NetworkMsgHandler>>();
		public void RegisterHandler(NetworkMsgHandler handler)
		{
			if (handlers.TryGetValue((int)handler.msgType, out var handlerSet) && null != handlerSet)
				handlerSet.Add(handler);
			else
				handlers[(int)handler.msgType] = new HashSet<NetworkMsgHandler> { handler };
		}

		public void UnRegisterHandler(NetworkMsgHandler handler)
		{
			if (handlers.TryGetValue((int)handler.msgType, out var handlerSet) && null != handlerSet)
				handlerSet.Remove(handler);
		}

		public void OnServerData(NetMessage.ServerMsg serverData)
		{
			if (handlers.TryGetValue((int)serverData.DataType, out var handlerSet) && null != handlerSet)
			{
				foreach (var h in handlerSet)
					h.Handle(serverData);
			}
		}
	}
	public class SocketConnection
	{
		public ConcurrentQueue<NetMessage.ServerMsg> receiveQueue = new ConcurrentQueue<NetMessage.ServerMsg>();
		public ConcurrentQueue<NetMessage.ClientMsg> sendQueue = new ConcurrentQueue<NetMessage.ClientMsg>();
		public readonly NetworkMsgHandlerRegister register = new NetworkMsgHandlerRegister();
		public IPEndPoint endPoint;
		public enum ConnectionType
		{
			Tcp,
			Udp
		}
		public ConnectionType connectionType = ConnectionType.Tcp;
		public enum State
		{
			Disconnected,
			Connecting,
			Connected,
			Disconnecting,

		}
		public State state = State.Disconnected;
		object stateLock = new object();

		Socket socket;
		// public string host;
		// public int port;

		Thread threadOpen = null;
		Thread threadRead = null;
		Thread threadWrite = null;
		Thread threadClose = null;

		public void Start()
		{
			threadOpen = new Thread(Open);
			threadOpen.Start();
		}

		public void Stop()
		{
			threadClose = new Thread(Close);
			threadClose.Start();
		}

		protected virtual Socket CreateConnectedSocket() { return null; }

		private void Open()
		{
			lock (stateLock) { state = State.Connecting; }
			socket = CreateConnectedSocket();

			lock (stateLock)
			{
				if (state == State.Connecting)
					state = State.Connected;
			}
		}

		private void Close()
		{
			lock (stateLock) { state = State.Disconnecting; }
			threadOpen.Join();
			threadRead.Join();
			threadWrite.Join();
			lock (stateLock) { state = State.Disconnected; }
		}

		private void Read()
		{
			byte[] buffer = new byte[4096];
			while (socket != null && socket.Connected)
			{
				if (state != State.Connected) break;
				try
				{
					int len = socket.Receive(buffer);
					if (len > 0)
					{
						var serverData = NetMessage.ServerMsg.Parser.ParseFrom(buffer, 0, len);
						if (null != serverData)
							receiveQueue.Enqueue(serverData.Clone());
					}
				}
				catch
				{
					break;
				}
				Thread.Sleep(1);
			}
		}

		private void Write()
		{
			while (socket != null && socket.Connected)
			{
				if (state != State.Connected) break;
				if (sendQueue.TryDequeue(out var result) && null != result)
				{
					var byteArray = result.ToByteArray();
					socket.Send(byteArray);
				}
			}
		}

		public void Update()
		{
			if (state == State.Connected)
			{
				if (null == threadRead)
				{
					threadRead = new Thread(Read);
					threadRead.Start();
				}
				if (null == threadWrite)
				{
					threadWrite = new Thread(Write);
					threadWrite.Start();
				}

				int count = 3;
				while (count-- > 0)
				{
					if (receiveQueue.TryDequeue(out var result))
					{
						OnServerData(result);
					}
				}
			}
		}

		public void OnServerData(NetMessage.ServerMsg serverData)
		{
			register.OnServerData(serverData);
		}
	}
}