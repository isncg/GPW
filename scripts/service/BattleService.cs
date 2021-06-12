using Godot;
using System.Collections.Generic;

namespace GPW
{
	public class BattleService : Service<BattleService>
	{
		public BattleRoot battleRoot;
		//public List<BulletManager> bulletManagers = new List<BulletManager>();
		public void InitBattleRoot()
		{
			battleRoot = GameRoot.Instance.battleRootPacked.Instance<BattleRoot>();
			GameRoot.Instance.GetNode<Node>("BattleRoot").AddChild(battleRoot);
			battleRoot.Init();
		}

		public void ReleaseBattleRoot()
		{
			GameRoot.Instance.GetNode<Node>("BattleRoot").RemoveChild(battleRoot);
			battleRoot = null;
		}
		NetworkMsgHandler cbEnterBattle = null;

		public override void Init()
		{
			base.Init();
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