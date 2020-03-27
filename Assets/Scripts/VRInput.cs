using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR;

public class VRInput : BaseInput
{
    public Camera eventCamera = null;

    public SteamVR_Action_Boolean ButtonBAction = null;
    public GameObject controller = null;

    public override bool GetMouseButton(int button)
    {
        return ButtonBAction.state;
    }

    public override bool GetMouseButtonDown(int button)
    {
        return ButtonBAction.stateDown;
    }

    public override bool GetMouseButtonUp(int button)
    {
        return ButtonBAction.stateUp;
    }

    protected override void Awake()
    {
        GetComponent<BaseInputModule>().inputOverride = this;
    }

    public override Vector2 mousePosition
    {
        get
        {
            return new Vector2(eventCamera.pixelWidth / 2, eventCamera.pixelHeight / 2);
        }
    }

}
