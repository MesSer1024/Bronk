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
			CubeLogic cube = msg.getCube ();
			Logger.Info (String.Format ("onMessage CubeClickedMessage cubeIndex= {0}", cube.Index));


			//mark all items in this range...
			cube.setSelected (!cube.selected);
		}
	}
}
