using System.Collections.Generic;

namespace Bronk
{
public static class Translate {
	private static Dictionary<string,string> _texts = new Dictionary<string, string>();
	
	public static void init() {
        var foo = new Dictionary<string, string>(100);
        foo.Add("ID_GUI_SELECTION", "SelectionBox");
        foo.Add("ID_HUD_MININGTOOL", "MiningTool");
        foo.Add("ID_ACTION_OK", "OK");
        foo.Add("ID_ACTION_CANCEL", "CANCEL");
            
		_texts = foo;
	}
	
	public static string text (string id)
	{		
		if (!_texts.ContainsKey (id)) {
			Logger.Error("Unable to find text identifier: " + id);
			return id;
		}
		return _texts[id];
	}
	
}
}