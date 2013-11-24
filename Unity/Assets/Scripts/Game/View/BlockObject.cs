using UnityEngine;
using System.Collections;
using Bronk;

public class BlockObject : MonoBehaviour, IInteractable
{
	public int Index;

	public void Interact ()
	{
		Game.World.ViewComponent.OnBlockInteract (Index);
	}

	public void SemiSelect(bool semiSelected)
	{

	}
}
