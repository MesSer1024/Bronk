using UnityEngine;
using Bronk;
using System.Collections.Generic;
using System;

public class Hud : MonoBehaviour, IMessageListener {
    public WorldGameObject World;
    public Rect MiningToolPos;
    public int ButtonSpacing = 5;
    private int _firstItemIndex;

    private enum Tools {
        None,
        MiningTool,
    }

    private Tools _activeTool;

	void Start () {
        switchTool(Tools.None);
        MessageManager.AddListener(this);
	}
	
	void Update () {
	    
	}

    void OnGUI () {
        switch (_activeTool) {
            case Tools.None:
                if (GUI.Button(MiningToolPos, Translate.text("ID_HUD_MININGTOOL"))) {
                    switchTool(Tools.MiningTool);
                }
                break;
            case Tools.MiningTool: {
                    var cancel = new Rect(MiningToolPos.x + MiningToolPos.width + ButtonSpacing, MiningToolPos.y, MiningToolPos.width, MiningToolPos.height);

                    if (_firstItemIndex < 0) {
                        if (GUI.Button(cancel, Translate.text("ID_ACTION_CANCEL"))) {
                            switchTool(Tools.None);
                        }
                    } else {
                        if (GUI.Button(MiningToolPos, Translate.text("ID_ACTION_OK"))) {

                        } else if (GUI.Button(cancel, Translate.text("ID_ACTION_CANCEL"))) {
                            switchTool(Tools.None);
                        }
                    }
                }
                break;
            default:
                GUI.Button(MiningToolPos, Translate.text("Default"));
                break;
        }
    }

    private void switchTool(Tools tool) {
        _activeTool = tool;
        _firstItemIndex = -1;
    }

    public void onMessage(IMessage message) {
        if (message is CubeClickedMessage) {
            var msg = message as CubeClickedMessage;
            CubeLogic cube = msg.getCube();
            Logger.Info(String.Format("onMessage CubeClickedMessage cubeIndex= {0}, storedIndex={1}", cube.Index, _firstItemIndex));

            if (_firstItemIndex < 0) {
                //first click
                _firstItemIndex = cube.Index;
            } else {
                //mark all items in this range...
                //var cubes = World.getCubesBetween(_firstItemIndex, cube.Index);
            }
        }
    }
}
