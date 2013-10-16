using UnityEngine;
using System.Collections.Generic;

namespace Bronk
{
public static class Translate {
	private static Dictionary<string,string> _texts = new Dictionary<string, string>();
	
	public static void init(Dictionary<string, string> texts) {
		_texts = texts;
	}
	
	public static string text (string id)
	{		
		if (!_texts.ContainsKey (id)) {
			Debug.LogError ("Unable to find text identifier: " + id);
			return id;
		}
		return _texts[id];
	}
	
}
}