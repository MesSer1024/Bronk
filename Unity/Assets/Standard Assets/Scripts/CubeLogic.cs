using UnityEngine;
using System.Collections;

public class CubeLogic : MonoBehaviour {
	public bool selected = false;
	private Color originalColor;
    private GameWorld.BlockType _data;
	
	// Use this for initialization
	void Start () {
		originalColor = gameObject.renderer.material.color;
	}
	
	// Update is called once per frame
	void Update ()
	{
        gameObject.renderer.enabled = true;
        if (selected && _data != GameWorld.BlockType.None) {
            gameObject.renderer.material.color = Color.green;
        } else {
            switch (_data) {
                case GameWorld.BlockType.None:
                    gameObject.renderer.enabled = false;
                    break;
                case GameWorld.BlockType.Dirt:
                    gameObject.renderer.material.color = new Color(0.545f, 0.271f, 0.075f);
                    break;
                case GameWorld.BlockType.Stone:
                    gameObject.renderer.material.color = new Color(0.827f, 0.827f, 0.827f);
                    break;
                default:
                    gameObject.renderer.material.color = originalColor;
                    break;
            }
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

    public void setData(GameWorld.BlockType block) {
        _data = block;
    }
}
