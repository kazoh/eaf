using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using Kazoh.Table;

public class Component_UI_MapSelect : GameComponent
{
    public Action<GameEnum.Menu> ClickedEvent;
    public Action<GameEnum.Menu> ClickedChangeEvent;

    public UILabel TitleLabel;
    public UILabel ChangeLabel;
    public UIScrollView ScrollView;
    public UIGrid Grid;
    public GameObject PrefMapSlot;

    public Component_Item_MapSlot SelectedSlot { get; private set; }

    private List<Component_Item_MapSlot> slotList;
    private List<TableData_MapList> mapList;
    private int curRegionId;

    readonly private int ITEM_NUM = 50;

    public override void Init()
    {
        base.Init();

        /* 스크롤 뷰 패널 뎁스 설정 */
        UIPanel panel = transform.parent.GetComponent<UIPanel>();
        if (panel != null) ScrollView.panel.depth = panel.depth + 1;

        TitleLabel.text = TableManager.GetString("STR_UI_SELETE_MAP");
        ChangeLabel.text = TableManager.GetString("STR_UI_CHANGE_REGION");

        GameDataManager gdm = GameProcess.GetGameDataManager();
        gdm.ChangeUserMapListEvent += OnChange;

        mapList = TableManager.GetMapList();

        /* 맵 아이템 설정 */
        slotList = new List<Component_Item_MapSlot>();
        for(int i=0; i < ITEM_NUM; ++i)
        {
            GameObject go = NGUITools.AddChild(Grid.gameObject, PrefMapSlot);
            Component_Item_MapSlot component = go.GetComponent<Component_Item_MapSlot>();
            if (component != null)
            {
                component.Init();
                component.ClickedEvent += OnSelectSlot;
                slotList.Add(component);
            }
        }
        Show(1);
        //Grid.repositionNow = true;
    }

    public void Show(int _regionId)
    {
        if (_regionId == curRegionId) Show();
        else
        {
            curRegionId = _regionId;
            if (SelectedSlot != null)
            {
                SelectedSlot.IsSelect = false;
                SelectedSlot = null;
            }

            List<TableData_MapList> list = new List<TableData_MapList>();
            for(int i=0; i < mapList.Count; ++i)
            {
                TableData_Map data = TableManager.GetGameData(mapList[i].MapName) as TableData_Map;
                if (data != null && data.RegionId == curRegionId) list.Add(mapList[i]);
            }
            for(int i=0; i < slotList.Count; ++i)
            {
                if(i < list.Count)
                {
                    slotList[i].Init(list[i]);
                }
                else
                {
                    slotList[i].Hide();
                }
            }

            Show();
            Grid.repositionNow = true;
            ScrollView.ResetPosition();

            if (slotList[0].gameObject.activeSelf) OnSelectSlot(slotList[0]);
        }
    }

    void OnChange()
    {
        for(int i=0; i < slotList.Count; ++i)
        {
            if(slotList[i].Data != null) slotList[i].UpdateSlot();
        }
    }

    void OnSelectSlot(Component_Item_MapSlot slot)
    {
        if(SelectedSlot != slot)
        {
            if (SelectedSlot != null) SelectedSlot.IsSelect = false;
            SelectedSlot = slot;
            SelectedSlot.IsSelect = true;
            if (ClickedEvent != null) ClickedEvent(GameEnum.Menu.MAP);
        }
    }

    public void OnClick_Change()
    {
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        if (ClickedChangeEvent != null) ClickedChangeEvent(GameEnum.Menu.REGION);
    }
}
