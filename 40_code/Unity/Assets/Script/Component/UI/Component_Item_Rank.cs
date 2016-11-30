using UnityEngine;
using System.Collections;
using System;

using Kazoh.Table;

public class Component_Item_Rank : GameComponent
{
    public UISprite ChaSprite;
    public UISprite GradeSprite;
    public UISprite IconSprite;
    public UILabel RankLabel;
    public UILabel NameLabel;
    public UILabel ScoreLabel;

    public Data_Rank Data { get; private set; }

    public void Init(int _rank, Data_Rank _data)
    {
        if(_data == null)
        {
            Hide();
            return;
        }

        Data = _data;
        RankLabel.text = string.Format("{0:#0}위", _rank);
        if (Data.Id == GameProcess.GetGameDataManager().GetGUID()) RankLabel.color = Color.red;
        else if (_rank < 4) RankLabel.color = Color.blue;
        else RankLabel.color = Color.gray;
        NameLabel.text = Data.Name;
        ScoreLabel.text = string.Format("{0:###,##0}점", Data.Record);

        TableData_Character chaData = TableManager.GetGameData(Data.ChaId) as TableData_Character;
        if(chaData != null)
        {
            ChaSprite.spriteName = chaData.IconName;
            GradeSprite.spriteName = "grade_0" + chaData.Grade;
        }
        else
        {
            ChaSprite.spriteName = "icon_item_0000";
            GradeSprite.spriteName = "";
        }

        switch(_rank)
        {
            case 1: IconSprite.spriteName = "crown_03"; break;
            case 2: IconSprite.spriteName = "crown_02"; break;
            case 3: IconSprite.spriteName = "crown_01"; break;
            default: IconSprite.spriteName = ""; break;
        }

        Show();
    }
}