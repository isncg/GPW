using Godot;
using System;
using GPW;
using System.Collections;

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

	private void OnProgressUpdate(object arg, object sender)
	{
		int value = (int)arg;
		GetNode<Label>("Progress/Label").Text = string.Format(Localization.Get(10001), value);
	}

	IEnumerator FadeOut()
	{
		Color colorBg = GetNode<ColorRect>("Bg").Color;
		Color colorText = Color.Color8(255, 255, 255, 255);
		int MAX = 10;
		for (int i = MAX; i >= 0; i--)
		{
			colorBg.a = (float)i / MAX;

			// colorText.r = (float)i / MAX;
			// colorText.g = (float)i / MAX;
			// colorText.b = (float)i / MAX;
			colorText.a = (float)i * i / (MAX * MAX);
			GetNode<ColorRect>("Bg").Color = colorBg;
			GetNode<Label>("Bg/Label").AddColorOverride("font_color", colorText);
			GetNode<Label>("Progress/Label").AddColorOverride("font_color", colorText);
			yield return null;
		}
		UIService.Instance.Destroy(this);
	}

	public override bool OnClose()
	{
		StartCoroutine(FadeOut());
		return false;
	}

	//  // Called every frame. 'delta' is the elapsed time since the previous frame.
	//  public override void _Process(float delta)
	//  {
	//      
	//  }
}
