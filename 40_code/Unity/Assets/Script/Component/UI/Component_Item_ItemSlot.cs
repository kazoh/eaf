using UnityEngine;
using System.Collections;
using System;

public class Component_Item_ItemSlot : GameComponent
{
    public Action<Component_Item_ItemSlot> ClickedEvent;

    public UISprite IconSprite;
    public UISprite GradeSprite;
    public UILabel EquipLabel;
    public UILabel NumLabel;
    public UISprite NewIconSprite;

    public Slot_Item Data { get; private set; }

    public void Init(Slot_Item _data)
    {
        if (Data != null) Data.ChangedEvent -= UpdateSlot;
        Data = _data;
        Data.ChangedEvent += UpdateSlot;
        UpdateSlot();
    }

    public void UpdateSlot()
    {
        if(Data == null || Data.IsEmpty)
        {
            IconSprite.spriteName = "icon_item_0000";
            GradeSprite.spriteName = "";
            NewIconSprite.spriteName = "";
            EquipLabel.text = "";
            NumLabel.text = "";
        }
        else
        {
            Data_UserItem item = Data.Data;
            IconSprite.spriteName = item.IconName;
            GradeSprite.spriteName = "grade_0" + item.Data.Grade;
            EquipLabel.text = Data.IsEquipped ? "E" : "";
            NumLabel.text = item.CanStack ? "" + item.Num : "";
            if(Data.IsNew) NewIconSprite.spriteName = "new_01";
            else NewIconSprite.spriteName = "";
        }
    }

    void OnClick()
    {
        if (Data == null || Data.IsEmpty) return;
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        if (ClickedEvent != null) ClickedEvent(this);
        if (Data.IsNew) Data.SetNew();
        GameProcess.GetGameDataManager().CheckNewItem();
    }
}
