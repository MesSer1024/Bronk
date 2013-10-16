using UnityEngine;
using System.Collections.Generic;
using System;

namespace Bronk
{
public static class MessageManager {
	private static List<IMessageListener> _listeners = new List<IMessageListener>();
	private static Queue<IMessage> _queue = new Queue<IMessage>();
	
	//these items are there for handling adding/removing items during execution of messages
	private static bool isExecuting = false;
	private static List<IMessageListener> _removables = new List<IMessageListener>(2);
	private static List<IMessageListener> _addables = new List<IMessageListener>(2);
	//-----
	
	public static void AddListener (IMessageListener listener)
	{
		if (isExecuting)
			_addables.Add (listener);
		else
			_listeners.Add (listener);
	}
	
	public static void RemoveListener (IMessageListener listener)
	{
		if (isExecuting)
			_removables.Add(listener);
		else
			_listeners.RemoveAll (a => a == listener);
	}
	
	public static void RemoveAllListeners ()
	{
		if (isExecuting)
			_removables.AddRange (_listeners);
		else
			_listeners.Clear ();
	}
	
	/// <summary>
	/// Do not use this when it can be avoided
	/// Executing message DIRECTLY, not delaying for a frame etc
	/// </summary>
	public static void ExecuteMessage (IMessage msg)
	{
		if (isExecuting) {
			ErrorHandler.Error ("LOGIC_ERROR: Cannot execute a message while another message is being Executed");
		}
		
		isExecuting = true;
		foreach (var listener in _listeners) {
			if(!_removables.Contains(listener))
				listener.onMessage (msg);
		}
		isExecuting = false;
	}
	
	/// <summary>
	/// Will execute the message whenever it is possible to do so (usually the next frame)
	/// </summary>
	public static void QueueMessage (IMessage msg)
	{
		_queue.Enqueue(msg);
	}
	
	public static void Update ()
	{
		syncLists ();
		while (_queue.Count > 0) {
			ExecuteMessage (_queue.Dequeue ());
		}
		syncLists();
	}
	
	private static void syncLists ()
	{
		if (_removables.Count > 0) {
			foreach (var listener in _removables) {
				_listeners.RemoveAll (a => a == listener);
			}
			_removables.Clear ();
		}
		
		if (_addables.Count > 0) {
			foreach (var listener in _addables) {
				_listeners.Add (listener);
			}
			_addables.Clear ();
		}
	}
	
}
}