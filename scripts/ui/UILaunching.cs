using Godot;
using System;
using GPW;
public class UILaunching : UI
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		RegisterEvent(GPW.EventType.UI_LOADING_PROGRESS_UPDATE, OnProgressUpdate);
	}

	private void OnProgressUpdate(object e)
	{
		int value = (int)e;
		GetNode<Label>("Content/LabelProgress").Text = string.Format("{0}%", value);
	}

	//  // Called every frame. 'delta' is the elapsed time since the previous frame.
	//  public override void _Process(float delta)
	//  {
	//      
	//  }
}
