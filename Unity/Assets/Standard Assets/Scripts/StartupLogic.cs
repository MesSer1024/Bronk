using UnityEngine;
using System.Collections.Generic;
using Bronk;

public class StartupLogic : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{	
		//implement different languages... maybe...
		Dictionary<string, string> foo = new Dictionary<string, string> ();
		foo.Add ("ID_GUI_SELECTION", "SelectionBox");
		
		Translate.init (foo);
		Game.state = Game.States.Playing;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
