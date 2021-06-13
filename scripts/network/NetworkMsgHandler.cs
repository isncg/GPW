using System;
using GPW.NetMessage;

namespace GPW
{
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
}