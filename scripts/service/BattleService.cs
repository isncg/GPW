namespace GPW
{
	public class BattleService : Service<BattleService>
	{
		NetworkMsgHandler cbEnterBattle = null;

		public override void Init()
		{
			cbEnterBattle = NetworkMsgHandler.Create(
				msgType: NetMessage.MsgType.MsgItems,
				dataField: msg => msg.EnterBattle,
				onData: OnEnterBattle,
				onError: OnEnterBattleError
			);

			NetworkService.Instance.LobbyServerConnection.register.RegisterHandler(cbEnterBattle);
		}

		public override void Reset()
		{
			NetworkService.Instance.LobbyServerConnection.register.UnRegisterHandler(cbEnterBattle);
		}

		void OnEnterBattle(NetMessage.ServerMsg.Types.EnterBattle enterBattle)
		{

		}

		void OnEnterBattleError(NetMessage.ErrorType errorType) { }

	}
}