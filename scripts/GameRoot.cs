using Godot;
using System;
using System.Collections.Generic;

namespace GPW
{
	public class GameRoot : Node
	{
		// Declare member variables here. Examples:
		// private int a = 2;
		// private string b = "text";

		// Called when the node enters the scene tree for the first time.

		public static GameRoot Instance { get; private set; }
		//public Node2D UIroot;
		public List<IService> services = new List<IService>();

		[Export] public PackedScene battleRootPacked;
		public override void _Ready()
		{
			Instance = this;

			services.Add(EventService.Instance);
			services.Add(SceneService.Instance);
			services.Add(UIService.Instance);
			services.Add(ConfigService.Instance);

			foreach (var service in services)
				service.Init();
			//UIroot = GetNode<Node2D>("UIRoot");
			SceneService.Instance.RunScene(SceneType.Launching);
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(float delta)
		{
			foreach (var service in services)
				service.Update();
		}
	}
}
