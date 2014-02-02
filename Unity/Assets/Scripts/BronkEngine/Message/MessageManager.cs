using System.Collections.Generic;

namespace Bronk
{
public static class MessageManager {
    public const string GameMessage = "game";

	private static List<IMessageListener> _listeners = new List<IMessageListener>();
	private static Queue<IMessage> _queue = new Queue<IMessage>();

    private static bool isExecuting;
	
	public static void AddListener (IMessageListener listener)
	{
		_listeners.Add (listener);
	}
	
	public static void RemoveListener (IMessageListener listener)
	{
		_listeners.RemoveAll (a => a == listener);
	}
	
	public static void RemoveAllListeners ()
	{
		_listeners.Clear ();
	}
	
	/// <summary>
	/// Do not use this when it can be avoided
	/// Executing message DIRECTLY, not delaying for a frame etc
	/// </summary>
	public static void ExecuteMessage (IMessage msg)
	{
		if (isExecuting) {
			Logger.Error ("LOGIC_ERROR: You probably should not execute a message while another message is being executed!");
		}
		
		isExecuting = true;
        var listenerCopy = _listeners.ToArray();
        foreach (var listener in listenerCopy) {
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
        var queueCopy = _queue.ToArray();
        _queue.Clear();
        foreach (var item in queueCopy) {
            ExecuteMessage(item);
        }
        //TODO: Consider adding one or more additional passes since messages might have spawned more items in queue that needs to be resolved now?
	}
	
}
}