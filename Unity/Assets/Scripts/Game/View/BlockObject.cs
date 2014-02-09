using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Bronk;

public class BlockObject : MonoBehaviour, IInteractable
{
	public static Material _SelectMaterial;
	public static Material _SemiSelectMaterial;
	public Material DefaultMaterial;
	public GameWorld.BlockType BlockType;
	public bool Discovered;
	private Renderer _renderer;
	private bool _semiSelected;
	private bool _selected;
	private List<BlockDecorators.DecoratorObject> _Decorators;
	public int BlockID;

	static BlockObject ()
	{
		_SelectMaterial = Resources.Load<Material> ("Terrain/select_mat");
		_SemiSelectMaterial = Resources.Load<Material> ("Terrain/semiselect_mat");
	}

	void GenerateDecorators ()
	{
		int tileSubdivisions = Random.Range (1, 4);
		float tileSize = 1f / (float)tileSubdivisions; 
		int decorators = Random.Range (0, Mathf.Min (tileSubdivisions * tileSubdivisions, 3));
		List<int> indices = new List<int> ();
		for (int i = 0; i < decorators; i++) {
			int index = Random.Range (0, tileSubdivisions * tileSubdivisions);
			while (indices.Contains (index)) {
				index++;
				if (index >= tileSubdivisions * tileSubdivisions)
					index = 0;
			}
			indices.Add (index);
		}
		for (int i = 0; i < indices.Count; i++) {
			int index = indices [i];
			int tileX = index % tileSubdivisions;
			int tileZ = index / tileSubdivisions;
			Vector3 tilePos = new Vector3 (BlockID % GameWorld.SIZE_X - 0.5f + tileX * tileSize + tileSize * 0.5f, 0.001f, BlockID / GameWorld.SIZE_Z - 0.5f + tileZ * tileSize + tileSize * 0.5f);
			if (BlockType != GameWorld.BlockType.DirtGround)
				tilePos += Vector3.up;
			_Decorators.Add (BlockDecorators.GetDecorator (BlockType, tilePos, tileSize));
		}
	}

	void OnBecameVisible ()
	{
		if (Discovered) {
			GenerateDecorators ();
		}
	}

	void OnBecameInvisible ()
	{
		if (Discovered) {
			for (int i = 0; i < _Decorators.Count; i++) {
				var deco = _Decorators [i];
				BlockDecorators.FreeDecorator (ref deco);
			}
			_Decorators.Clear ();
		}
	}

	void Awake ()
	{
		_renderer = GetComponentInChildren<Renderer> ();
		DefaultMaterial = _renderer.sharedMaterial;
		_Decorators = new List<BlockDecorators.DecoratorObject> (9);
	}

	public void Interact ()
	{
		Game.World.ViewComponent.OnBlockInteract (BlockID);
	}

	public void SemiSelect (bool semiSelected)
	{
		_semiSelected = semiSelected;
		UpdateMaterial ();
	}

	public void UpdateMaterial ()
	{
		if (_selected)
			_renderer.sharedMaterial = _SelectMaterial;
		else if (_semiSelected)
			_renderer.sharedMaterial = _SemiSelectMaterial;
		else
			_renderer.sharedMaterial = DefaultMaterial;
	}

	public void SetSelected (bool selected)
	{
		_selected = selected;
		UpdateMaterial ();
	}
}
