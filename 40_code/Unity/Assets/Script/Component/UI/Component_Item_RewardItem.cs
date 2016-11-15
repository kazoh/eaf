using UnityEngine;
using System.Collections;
using System;

public class Component_Item_RewardItem : GameComponent
{
    public UISprite IconSprite;
    public UISprite GradeSprite;
    public UILabel NumLabel;

    public void Show(TableData_Item _data, int num)
    {
        IconSprite.spriteName = _data.IconName;
        GradeSprite.spriteName = "grade_0" + _data.Grade;
        NumLabel.text = _data.IsStack ? "" + num : "";
        base.Show();
    }
}