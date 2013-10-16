using UnityEngine;
using System.Collections;

namespace Bronk
{
public interface IMessage {
	//Group: done for filtering on "bigger types" so we can easily find out in what category a message lies
		//Network
		//GUI
		//Game (unit died, out of resources
		//
	//Id: done so we can find unique messages inside a group (for instance some examples from Game-group)
		//unit_died
		//enemy_attacks
		//
		
	string getGroup();
	string getId();
}
}