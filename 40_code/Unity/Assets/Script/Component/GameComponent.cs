using UnityEngine;
using System.Collections;

public class GameComponent : MonoBehaviour {

    public virtual void Init()
    {

    }

    public virtual void Show()
    {
        if (!gameObject.activeSelf) gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        if (gameObject.activeSelf) gameObject.SetActive(false);
    }
}
