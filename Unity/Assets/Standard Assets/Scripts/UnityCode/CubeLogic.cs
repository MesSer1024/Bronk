using UnityEngine;
using System.Collections;
using Bronk;

public class CubeLogic : MonoBehaviour {
	private static bool _InitializedMaterials;
	private static Material _DirtMaterial;
	private static Material _StoneMaterial;
	private static Material _DefaultMaterial;
	private static Material _SelectedMaterial;

	public bool selected = false;
	private Color originalColor;
    private CubeData _data;
    public int Index { get; set; }


	public static void InitializeMaterials(Material defaultMaterial)
	{
		if (_InitializedMaterials == false) {
			_InitializedMaterials = true;

			_DefaultMaterial = defaultMaterial;
			_DirtMaterial = new Material (defaultMaterial);
			_DirtMaterial.color = new Color (0.545f, 0.271f, 0.075f);
			_StoneMaterial = new Material (defaultMaterial);
			_StoneMaterial.color = new Color(0.827f, 0.827f, 0.827f);
			_SelectedMaterial = new Material (defaultMaterial);
			_SelectedMaterial.color = Color.green;
		}
	}


	void OnBecameVisible()
	{
		this.enabled = true;
	}


	void OnBecameInvisible()
	{
		this.enabled = false;
	}
	// Update is called once per frame
	void Update ()
	{
        gameObject.renderer.enabled = true;
        if (selected && _data.Type != GameWorld.BlockType.None) {
			gameObject.renderer.sharedMaterial = _SelectedMaterial;
        } else {
            switch (_data.Type) {
                case GameWorld.BlockType.None:
                    gameObject.renderer.enabled = false;
                    break;
                case GameWorld.BlockType.Dirt:
				gameObject.renderer.sharedMaterial = _DirtMaterial;
                    break;
                case GameWorld.BlockType.Stone:
				gameObject.renderer.sharedMaterial = _StoneMaterial;
                    break;
                default:
				gameObject.renderer.sharedMaterial = _DefaultMaterial;
                    break;
            }
        }
	}
	
	public void setSelected(bool flag) {
		selected = flag;
	}

    void OnMouseDown() {
        MessageManager.ExecuteMessage(new CubeClickedMessage("cube", this));
    }

    internal void setData(CubeData data) {
        _data = data;
    }
}
