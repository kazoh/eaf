using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using Kazoh.Table;

public class Component_UI_MapInfo : GameComponent
{
    public Action<int> ClickedEvent;

    public UITexture MapImg;
    public UILabel GradeLabel;
    public UILabel TitleLabel;
    public UILabel DescLabel;
    public UISprite UnlockIconSprite;
    public UILabel BtnLabel;

    private bool isLock;
    private int mapId;

    public override void Init()
    {
        base.Init();
    }

    public void Show(TableData_MapList _data)
    {
        if (_data == null)
        {
            Hide();
            return;
        }
        TableData_Map map = TableManager.GetGameData(_data.MapName) as TableData_Map;
        if(map == null)
        {
            Hide();
            return;
        }

        mapId = map.Id;
        MapImg.mainTexture = GameProcess.GetResourceManager().GetMapImg(map.ImageName);

        GradeLabel.text = string.Format("{0} : {1}", TableManager.GetString("STR_UI_GRADE_LIMITE"),GetGradeStr(map.Grade));
        TitleLabel.text = TableManager.GetString(map.Str);
        BtnLabel.text = TableManager.GetString("STR_UI_START");
        isLock = GameProcess.GetGameDataManager().IsLockMap(_data);
        if (isLock)
        {
            UnlockIconSprite.alpha = 1f;
            for (int i = 0; i < _data.UnlockCondList.Count; ++i)
            {
                if (_data.UnlockCondList[i].Cond == TableEnum.UnlockType.None) break;
                switch (_data.UnlockCondList[i].Cond)
                {
                    case TableEnum.UnlockType.BCrownMapId:
                    case TableEnum.UnlockType.SCrownMapId:
                    case TableEnum.UnlockType.GCrownMapId:
                        DescLabel.text = MakeUnlockCondDesc(_data.UnlockCondList[i]);
                        break;
                    case TableEnum.UnlockType.BCrownNum:
                    case TableEnum.UnlockType.SCrownNum:
                    case TableEnum.UnlockType.GCrownNum:
                        DescLabel.text = MakeUnlockCondDesc2(_data.UnlockCondList[i]);
                        break;
                    default:
                        DescLabel.text = "";
                        break;
                }
            }
        }
        else
        {
            DescLabel.text = TableManager.GetString(map.Desc);
            UnlockIconSprite.alpha = 0f;
        }

        if (!string.IsNullOrEmpty(map.Bgm))
        {
            GameProcess.StopBGM();
            GameProcess.PlaySound(SOUND_EFFECT.BGM, map.Bgm);
        }
        base.Show();
    }

    string MakeUnlockCondDesc(TableData_MapList.UnlockCond cond)
    {
        TableData_Map condMap = TableManager.GetGameData(cond.Value) as TableData_Map;
        if (condMap == null) return string.Empty;
        string format = TableManager.GetString("STR_UI_OPEN_COND");
        switch (cond.Cond)
        {
            case TableEnum.UnlockType.BCrownMapId:
                return string.Format(format, TableManager.GetString(condMap.Str), TableManager.GetString("STR_CROWN_1"));
            case TableEnum.UnlockType.SCrownMapId:
                return string.Format(format, TableManager.GetString(condMap.Str), TableManager.GetString("STR_CROWN_2"));
            case TableEnum.UnlockType.GCrownMapId:
                return string.Format(format, TableManager.GetString(condMap.Str), TableManager.GetString("STR_CROWN_3"));
        }
        return string.Empty;
    }

    string MakeUnlockCondDesc2(TableData_MapList.UnlockCond cond)
    {
        string format = "{0} {1}/{2}";
        switch (cond.Cond)
        {
            case TableEnum.UnlockType.BCrownNum:
                return string.Format(format, TableManager.GetString("STR_CROWN_1"), GameProcess.GetGameDataManager().GetMapNum(1), cond.Value);
            case TableEnum.UnlockType.SCrownNum:
                return string.Format(format, TableManager.GetString("STR_CROWN_2"), GameProcess.GetGameDataManager().GetMapNum(2), cond.Value);
            case TableEnum.UnlockType.GCrownNum:
                return string.Format(format, TableManager.GetString("STR_CROWN_3"), GameProcess.GetGameDataManager().GetMapNum(3), cond.Value);
        }
        return string.Empty;
    }

    public void OnClick_Start()
    {
        if (isLock) return;
        LayoutManager.Lock();
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        if (ClickedEvent != null) ClickedEvent(mapId);
    }

    string GetGradeStr(int _grade)
    {
        if (_grade == 1) return "[bbbbbb]D[-]";
        if (_grade == 2) return "[3AFB16]C[-]";
        if (_grade == 3) return "[1692FB]B[-]";
        if (_grade == 4) return "[C816FB]A[-]";
        if (_grade == 5) return "[FB162D]S[-]";
        if (_grade == 6) return "[FBA916]SS[-]";
        return TableManager.GetString("STR_UI_NO_LIMITE");
    }

}
