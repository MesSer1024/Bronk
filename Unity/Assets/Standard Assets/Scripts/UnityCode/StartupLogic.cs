using UnityEngine;
using System.Collections.Generic;
using Bronk;

public class StartupLogic : MonoBehaviour {

	void Awake() {	
		Translate.init();
		Game.state = Game.States.Playing;
        Game.init();
	}
	
	// Update is called once per frame
	void Update () {
        Game.update(Time.deltaTime);
	}
}
