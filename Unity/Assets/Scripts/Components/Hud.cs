using UnityEngine;
using Bronk;
using System.Collections.Generic;
using System;

public class Hud : MonoBehaviour, IMessageListener
{
	public WorldGameObject World;
	public Rect MiningToolPos;
	public int ButtonSpacing = 5;

	void Start ()
	{
		MessageManager.AddListener (this);
	}

	void Update ()
	{
	    
	}

	public void onMessage (IMessage message)
	{
		if (message is CubeClickedMessage) {
			var msg = message as CubeClickedMessage;
			int cubeIndex = msg.getCubeIndex ();
			Logger.Info (String.Format ("onMessage CubeClickedMessage cubeIndex= {0}", cubeIndex));

			bool selected = Game.World.Cubes [cubeIndex].SelectedTimeline.GetValue(Time.time);
			
			Game.World.ViewComponent.SetBlockSelected (cubeIndex, Time.time, !selected);
		} 
	}
}