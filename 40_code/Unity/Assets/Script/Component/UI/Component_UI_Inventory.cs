using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using Kazoh.Table;

public class Component_UI_Inventory : GameComponent
{
    public Action<Slot_Item> ClickedEvent;
    public Action AddSlotEvent;

    public UILabel TitleLabel;
    public UILabel AddLabel;
    public UILabel SortLabel;
    public UIScrollView ScrollView;
    public UIGrid Grid;
    public GameObject PrefItemSlot;

    public Slot_Item SelectedSlot { get; private set; }

    private List<Component_Item_ItemSlot> slotList;

    public override void Init()
    {
        base.Init();

        /* 스크롤 뷰 패널 뎁스 설정 */
        UIPanel panel = transform.parent.GetComponent<UIPanel>();
        if (panel != null) ScrollView.panel.depth = panel.depth + 1;

        TitleLabel.text = TableManager.GetString("STR_UI_INVENTORY");
        AddLabel.text = TableManager.GetString("STR_UI_ADD_SLOT");
        SortLabel.text = TableManager.GetString("STR_UI_SORT");

        GameProcess.GetGameDataManager().AddedInventorySizeEvent += OnChange;

        /* 아이템 슬롯 설정 */
        slotList = new List<Component_Item_ItemSlot>();
        //OnChangeInventory();
        OnChange();
    }

    void OnChange()
    {
        List<Slot_Item> list = GameProcess.GetGameDataManager().GetInventory(true);
        if (slotList.Count < list.Count) SetItemSlot(list);
        //if (list.Count > slotList.Count) OnChangeInventory();
        //else
        //{
        //    for (int i = 0; i < list.Count; ++i)
        //    {              
        //        slotList[i].Init(list[i]);
        //    }
        //}
    }

    void OnSelectSlot(Component_Item_ItemSlot slot)
    {
        SelectedSlot = slot.Data;
        if (ClickedEvent != null) ClickedEvent(SelectedSlot);
    }

    bool isLock;
    public void OnClick_AddSlot()
    {
        if (isLock) return;
        isLock = true;

        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        if (AddSlotEvent != null) AddSlotEvent();

        isLock = false;
    }

    public void OnClick_Sort()
    {
        if (isLock) return;
        isLock = true;

        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        List<Slot_Item> list = GameProcess.GetGameDataManager().GetInventory(true);
        SetItemSlot(list);

        isLock = false;
    }

    void SetItemSlot(List<Slot_Item> list)
    {
        int num = list.Count - slotList.Count;
        for(int i=0; i < num; ++i)
        {
            GameObject go = NGUITools.AddChild(Grid.gameObject, PrefItemSlot);
            Component_Item_ItemSlot component = go.GetComponent<Component_Item_ItemSlot>();
            if (component != null)
            {
                component.ClickedEvent += OnSelectSlot;
                slotList.Add(component);
            }
        }
        for (int i = 0; i < list.Count; ++i)
        {
            Component_Item_ItemSlot component = slotList[i];
            component.Init(list[i]);
        }
        Grid.repositionNow = true;
    }
}
