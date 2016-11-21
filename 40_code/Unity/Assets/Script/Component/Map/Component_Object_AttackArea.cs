using UnityEngine;
using System.Collections;
using System;

public class Component_Object_AttackArea : Component_Map_Object, IAttackable
{
    //private UISprite areaSprite;

    //public override void Init()
    //{
    //    //areaSprite = GetComponent<UISprite>();
    //    //if (areaSprite != null)
    //    //{
    //    //    areaSprite.alpha = 0f;
    //    //}
    //    base.Init();
    //}

    void OnClick()
    {
        map.Player.OnClickObject(this);
    }

    public void Attacked(Vector3 _pos, int _atk, bool _critical, bool _knockback)
    {
        // 그냥 공격 가이드이므로 아무것도 안한다.
    }

    public bool IsDie()
    {
        // 그냥 공격 가이드이므로 아무것도 안한다.
        return false;
    }
}
