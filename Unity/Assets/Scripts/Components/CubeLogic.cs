using UnityEngine;
using System.Collections;
using Bronk;

public class CubeLogic : MonoBehaviour
{
	private static bool _InitializedMaterials;
	private static Material _DirtMaterial;
	private static Material _StoneMaterial;
	private static Material _FoodMaterial;
	private static Material _DefaultMaterial;
	private static Material _SelectedMaterial;
	private static Material _SemiSelectedMaterial;
	public bool selected = false;
	private bool _semiSelected = false;
	private Color originalColor;
	private CubeData _data;

	public int Index { get; set; }

	public static void InitializeMaterials (Material defaultMaterial)
	{
		if (_InitializedMaterials == false) {
			_InitializedMaterials = true;

			_DefaultMaterial = defaultMaterial;
			_DirtMaterial = new Material (defaultMaterial);
			_DirtMaterial.color = new Color (0.545f, 0.271f, 0.075f);
			_StoneMaterial = new Material (defaultMaterial);
			_StoneMaterial.color = new Color (0.827f, 0.827f, 0.827f);
			_FoodMaterial = new Material (defaultMaterial);
			_FoodMaterial.color = new Color (0.827f, 0.0f, 0.2f);
			_SelectedMaterial = new Material (defaultMaterial);
			_SelectedMaterial.color = Color.green;
			_SemiSelectedMaterial = new Material (defaultMaterial);
			_SemiSelectedMaterial.color = Color.yellow;
		}
	}

	void OnBecameVisible ()
	{
		this.enabled = true;
		UpdateMaterial ();
	}

	void OnBecameInvisible ()
	{
		this.enabled = false;
	}

	void UpdateMaterial ()
	{
		gameObject.renderer.enabled = true;
		if (selected && _data.IsGround() == false) {
			gameObject.renderer.sharedMaterial = _SelectedMaterial;
		} else if (_semiSelected && _data.IsGround() == false) {
			gameObject.renderer.sharedMaterial = _SemiSelectedMaterial;
		} else {
			switch (_data.Type) {
			case GameWorld.BlockType.DirtGround:
				gameObject.renderer.enabled = false;
				break;
			case GameWorld.BlockType.Dirt:
				gameObject.renderer.sharedMaterial = _DirtMaterial;
				break;
			case GameWorld.BlockType.Stone:
				gameObject.renderer.sharedMaterial = _StoneMaterial;
				break;
			case GameWorld.BlockType.Food:
				gameObject.renderer.sharedMaterial = _FoodMaterial;
				break;
			default: 
				gameObject.renderer.sharedMaterial = _DefaultMaterial;
				break;
			}
		}
	}

	public void setSelected (bool flag)
	{
		_semiSelected = false;
		selected = flag;
		UpdateMaterial ();
	}

	public void setSemiSelected (bool semiSelected)
	{
		_semiSelected = semiSelected;
		UpdateMaterial ();
	}

	internal void setData (CubeData data)
	{
		_data = data;
	}
}
