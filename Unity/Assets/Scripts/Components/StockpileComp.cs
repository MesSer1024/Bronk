using UnityEngine;
using System.Collections;
using Bronk;
using System.Collections.Generic;

public class StockpileComp : MonoBehaviour, IMessageListener {
    public int GoldCount { get; private set; } //TODO: Probably do this in a much better way with underlying data-class but this seems easiest for now

    private List<CarryObject> _items;

	// Use this for initialization
	void Start () {
	
	}

    void Awake() {
        Game.World.StockpileComponent = this;
        _items = new List<CarryObject>();
        GoldCount = 0;
    }

    public void init() {
        //Game.World.Blocks.getBlockIDByPosition(Game.World.StartArea.center), digjob.EndTime, gold)

        var r = Game.World.StartArea.center;
        this.transform.position = new Vector3(r.x, 0.1f, r.y);
        MessageManager.AddListener(this);
    }

	
	// Update is called once per frame
	void Update () {
	
	}

    public void onMessage(IMessage message) {
        if (message is ItemDeliveredMessage) {
            var msg = message as ItemDeliveredMessage;
            _items.Add(msg.Item);

            if (msg.Item is GoldObject) {
                GoldCount++;
            }
        }
    }

    public bool isItemInStockpile(CarryObject item) {
        return _items.Contains(item);
    }
}
