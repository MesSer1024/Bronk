using UnityEngine;
using System.Collections;
using Bronk;

public class BlockObject : MonoBehaviour, IInteractable
{
	public static Material _SelectMaterial;
	public static Material _SemiSelectMaterial;
	private Material _originalMaterial;
	private Renderer _renderer;
	private bool _semiSelected;
	private bool _selected;

	static BlockObject ()
	{
		_SelectMaterial = Resources.Load<Material> ("Terrain/select_mat");
		_SemiSelectMaterial = Resources.Load<Material> ("Terrain/semiselect_mat");
	}

	public int Index;

	void Awake ()
	{
		_renderer = GetComponentInChildren<Renderer> ();
		_originalMaterial = _renderer.sharedMaterial;
	}

	public void Interact ()
	{
		Game.World.ViewComponent.OnBlockInteract (Index);
	}

	public void SemiSelect (bool semiSelected)
	{
		_semiSelected = semiSelected;
		UpdateMaterial ();
	}

	void UpdateMaterial ()
	{
		if (_selected)
			_renderer.sharedMaterial = _SelectMaterial;
		else if (_semiSelected)
			renderer.sharedMaterial = _SemiSelectMaterial;
		else
			_renderer.sharedMaterial = _originalMaterial;
	}

	public void SetSelected (bool selected)
	{
		_selected = selected;
		UpdateMaterial ();
	}
}
