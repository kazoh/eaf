using UnityEngine;
using System.Collections;

public class Module_SimpleDepth : MonoBehaviour {

    public Component_Map_Player Target;
    public UISprite Sprite;

    void LateUpdate()
    {
        if (Target == null || Sprite == null) return;
        if (transform.localPosition.y < Target.transform.localPosition.y)
        {
            SetDepth(Target.SpriteDepth + 1);
        }
        else
        {
            SetDepth(Target.SpriteDepth - 1);
        }
    }

    void SetDepth(int _depth)
    {
        if (_depth != Sprite.depth) Sprite.depth = _depth;
    }
}
