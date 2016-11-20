using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using Kazoh.Table;

public class Component_UI_PopupRegion : GameComponent
{
    public Action<GameComponent> ClickedEvent;

    public UILabel TitleLabel;
    public UILabel BtnLabel;
    public UIScrollView ScrollView;
    public UIGrid Grid;

    public GameObject PrefRegionItem;

    private List<Component_Item_Region> listRegionObject;

    public override void Init()
    {
        base.Init();

        /* 스크롤 뷰 패널 뎁스 설정 */
        UIPanel panel = transform.parent.GetComponent<UIPanel>();
        if (panel != null) ScrollView.panel.depth = panel.depth + 1;

        /* 텍스트 설정 */
        TitleLabel.text = TableManager.GetString("STR_TITLE_REGION");
        BtnLabel.text = TableManager.GetString("STR_UI_CLOSE");

        /* 리스트 획득 */
        List<TableData_Region> list = TableManager.GetRegionList();
        listRegionObject = new List<Component_Item_Region>(gameObject.GetComponentsInChildren<Component_Item_Region>());

        /* 리스트 설정 */     
        foreach(Component_Item_Region component in listRegionObject)
        {
            TableData_Region data = list.Find(x => x.Name == component.RegionName);
            component.Init(data);
            if (data != null)
            {
                component.ClickedEvent += OnSelect;
            }
        }
    }

    public void Show(int _curRegionId)
    {
        for(int i=0; i < listRegionObject.Count; ++i)
        {
            if (listRegionObject[i].Data != null && listRegionObject[i].Data.Id == _curRegionId) listRegionObject[i].ShowMarker();
            else listRegionObject[i].HideMarker();
        }
        
        base.Show();
    }

    void OnSelect(GameComponent item)
    {
        if (ClickedEvent != null) ClickedEvent(item);
    }

    public void OnClick_Close()
    {
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        Hide();
    }
}
