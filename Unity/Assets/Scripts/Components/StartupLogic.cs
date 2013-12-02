using UnityEngine;
using System.Collections.Generic;
using Bronk;

public class StartupLogic : MonoBehaviour {

	void Start() {	
		Translate.init();
		Game.state = Game.States.Playing;
        Game.init();
		Game.Start ();
	}

	// Update is called once per frame
	void Update () {
        Game.update(Time.deltaTime);
	}
}
