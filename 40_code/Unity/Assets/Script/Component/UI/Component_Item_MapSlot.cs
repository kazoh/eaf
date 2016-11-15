using UnityEngine;
using System.Collections;
using System;

using Kazoh.Table;

public class Component_Item_MapSlot : GameComponent
{
    public Action<Component_Item_MapSlot> ClickedEvent;

    public UISprite IconSprite;
    public UISprite GradeSprite;
    public UISprite SelectSprite;
    public UILabel GradeLabel;
    public UISprite LockCoverSprite;
    public UISprite LockIconSprite;

    private bool isSelect;
    public bool IsSelect
    {
        get { return isSelect; }
        set
        {
            isSelect = value;
            if(isSelect)
            {
                SelectSprite.color = Color.cyan;
            }
            else
            {
                SelectSprite.color = Color.white;
            }
        }
    }

    private bool isLock;
    public bool IsLock
    {
        get { return isLock; }
        private set
        {
            isLock = value;
            if(isLock)
            {
                LockCoverSprite.alpha = 0.7f;
                LockIconSprite.alpha = 1;
            }
            else
            {
                LockCoverSprite.alpha = 0;
                LockIconSprite.alpha = 0;
            }
        }
    }

    public TableData_MapList Data { get; private set; }

    public void Init(TableData_MapList _data)
    {
        Data = _data;
        TableData_Map mapData = TableManager.GetGameData(_data.MapName) as TableData_Map;
        if(mapData == null)
        {
            IsLock = true;
            Hide();
        }
        else
        {
            gameObject.name = "map_" + mapData.Id;
            IsSelect = false;
            UpdateSlot();
            Show();
        }
    }

    public void UpdateSlot()
    {
        TableData_Map mapData = TableManager.GetGameData(Data.MapName) as TableData_Map;
        if(mapData != null)
        {
            IconSprite.spriteName = mapData.IconName;
            GradeLabel.text = GetGradeStr(mapData.Grade);
            IsLock = GameProcess.GetGameDataManager().IsLockMap(Data);
            if (GameProcess.GetGameDataManager().IsNewMap(mapData.Id)) GradeSprite.spriteName = "new_01";
            else GradeSprite.spriteName = "crown_0" + GameProcess.GetGameDataManager().GetMapRecord(mapData.Id);
        }
    }

    void OnClick()
    {
        if (LayoutManager.IsLock()) return;
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        if (ClickedEvent != null) ClickedEvent(this);
        TableData_Map mapData = TableManager.GetGameData(Data.MapName) as TableData_Map;
        if (mapData != null) GameProcess.GetGameDataManager().PlayMap(mapData.Id);
        if (GradeSprite.spriteName.Equals("new_01")) GradeSprite.spriteName = "";
    }

    string GetGradeStr(int _grade)
    {
        if (_grade == 1) return "[bbbbbb]D[-]";
        if (_grade == 2) return "[3AFB16]C[-]";
        if (_grade == 3) return "[1692FB]B[-]";
        if (_grade == 4) return "[C816FB]A[-]";
        if (_grade == 5) return "[FB162D]S[-]";
        if (_grade == 6) return "[FBA916]SS[-]";
        return "[bbbbbb]-[-]";
    }
}
