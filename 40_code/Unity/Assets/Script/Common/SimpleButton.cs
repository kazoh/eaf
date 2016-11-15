using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SimpleButton : MonoBehaviour
{
    public GameObject NotifyTo;
    public string CallbackMethodName;

    /// <summary>
    /// Call the listener function.
    /// </summary>

    protected virtual void OnClick()
    {
        if (NotifyTo != null) NotifyTo.SendMessage(CallbackMethodName);
    }

}
