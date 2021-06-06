using Godot;

namespace GPW
{
	public abstract class Scene
	{
		public virtual void OnUpdate()
		{

		}

		public virtual void OnEnter()
		{

		}

		public virtual void BeforeLeave()
		{

		}

		public virtual void AfterLeave()
		{

		}

		public virtual void OnInput(InputEvent inputEvent)
		{

		}
	}
}