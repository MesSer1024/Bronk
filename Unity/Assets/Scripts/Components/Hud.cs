using UnityEngine;
using Bronk;
using System.Collections.Generic;
using System;

public class Hud : MonoBehaviour, IMessageListener
{
	public WorldGameObject World;

	void Start ()
	{
		MessageManager.AddListener (this);
	}

	void Update ()
	{
	    
	}

	public void onMessage (IMessage message)
	{

	}
}