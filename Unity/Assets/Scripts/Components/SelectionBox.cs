using UnityEngine;
using System.Collections;
using Bronk;

public class SelectionBox : MonoBehaviour {
	public int MinSelection = 25;
    public WorldGameObject world;
	
	private Vector3 orgPos;
	private Vector3 currPos;
	private bool selecting;
	
	void Start ()
	{
		
	}
	
	bool selectionOverTreshold ()
	{
		return Vector3.Distance (orgPos, currPos) > MinSelection;
	}
	
	void drawGuiSelection ()
	{
		var selection = new Rect (
						Mathf.Min (orgPos.x, currPos.x) 
						, Mathf.Min (Screen.height - orgPos.y, Screen.height - currPos.y)
						, Mathf.Abs (orgPos.x - currPos.x)
						, Mathf.Abs (orgPos.y - currPos.y)
						);
		
		GUI.Box (selection, Translate.text ("ID_GUI_SELECTION"));
	}
	
	void OnGUI ()
	{
		if (selecting && Input.GetMouseButton (0)) {
			currPos = Input.mousePosition;
			GUI.color = Color.green;
			if (selectionOverTreshold ()) {
				drawGuiSelection ();
			}
		}

	}
	 
	void Update ()
	{
		if (Input.GetMouseButtonDown (0)) {			
			orgPos = Input.mousePosition;
			selecting = true;
		}
		
		if (Input.GetMouseButtonUp (0)) {
			selecting = false;
			if (selectionOverTreshold ()) {
				//multiple cubes selected (using selection box)
				var cubes = world.getCubes ();
				foreach (var cube in cubes) {                    
					var screenCoordinates = Camera.main.WorldToScreenPoint (cube.transform.position);	
					var selection = new Rect (
						Mathf.Min (orgPos.x, currPos.x) 
						, Mathf.Min (orgPos.y, currPos.y)
						, Mathf.Abs (orgPos.x - currPos.x)
						, Mathf.Abs (orgPos.y - currPos.y)
						);
					if (selection.Contains (screenCoordinates)) {
						cube.GetComponent<CubeLogic> ().setSelected (true);
					}
				}
			} else {
				//one cube selected (do not use selection box)
			}
			
		}
	}
}
