using UnityEngine;

namespace Bronk
{
public static class Logger {
	
	public static void Info (string s)
	{
		Debug.Log("[info]:" + s);
	}
	
	public static void Log (string s)
	{
		Debug.Log("[log]:" + s);
	}
	
	public static void Warning (string s)
	{
		Debug.LogWarning ("[warning]:" + s);
		Debug.LogWarning ("[warning]: StackTrace=" + StackTraceUtility.ExtractStackTrace ());
		//#TODO: Trigger assert
	}
	
	public static void Error (string s)
	{		
		Debug.LogError ("[error]:" + s);
		Debug.LogError ("[error]: StackTrace=" + StackTraceUtility.ExtractStackTrace ());
		//#TODO: Trigger assert
	}
	
	public static void Fatal (string s)
	{
		Debug.LogError ("[fatal]:" + s);
		throw new UnityException(s);
	}
}
}