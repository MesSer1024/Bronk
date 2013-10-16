using UnityEngine;
using System.Collections;

public class CubeLogic : MonoBehaviour {
	public bool selected = false;
	private Color colorOrg;
	
	// Use this for initialization
	void Start () {
		colorOrg = gameObject.renderer.material.color;
	}
	
	// Update is called once per frame
	void Update ()
	{		
		if (selected) {
			gameObject.renderer.material.color = new Color (0, 1, 0);
		} else {
			gameObject.renderer.material.color = colorOrg;
		}		
	}
	
	public void setSelected(bool flag) {
		selected = flag;
	}
	
	void OnMouseOver ()
	{
		//if (Input.GetMouseButton (0))			
		//	selected = !selected;
	}
}
