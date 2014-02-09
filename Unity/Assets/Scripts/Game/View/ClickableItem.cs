using UnityEngine;
using System.Collections;
using Bronk;

public class ClickableItem : MonoBehaviour, IInteractable
{
    public ICarryObject CarryObject;
  
    public void Interact()
    {
        MessageManager.QueueMessage(new ItemClickedMessage("gold", CarryObject.ItemId, CarryObject.BlockId, CarryObject));
    }

    public void SemiSelect(bool semiselected)
    {
    }
}