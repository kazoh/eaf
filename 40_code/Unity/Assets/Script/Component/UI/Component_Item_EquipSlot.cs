using UnityEngine;
using System.Collections;
using System;

public class Component_Item_EquipSlot : GameComponent
{
    public Action<Component_Item_EquipSlot> ClickedEvent;

    public UISprite IconSprite;
    public UISprite GradeSprite;

    public Slot_Item Data { get; private set; }

    public void Init(int idx)
    {
        base.Init();
        gameObject.name = "equip_" + idx;
        UpdateSlot();
    }

    public void UpdateSlot()
    {
        if(Data == null || Data.IsEmpty)
        {
            IconSprite.spriteName = "icon_item_0000";
            GradeSprite.spriteName = "";
        }
        else
        {
            Data_UserItem item = Data.Data;
            IconSprite.spriteName = item.IconName;
            GradeSprite.spriteName = "grade_0" + item.Data.Grade;
        }
    }

    public void SetData(Slot_Item _data)
    {
        Data = _data;
        UpdateSlot();
    }

    void OnClick()
    {
        if (Data == null || Data.IsEmpty) return;
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        if (ClickedEvent != null) ClickedEvent(this);
    }
}
