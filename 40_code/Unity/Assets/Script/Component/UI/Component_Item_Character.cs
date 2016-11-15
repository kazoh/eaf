using UnityEngine;
using System.Collections;
using System;

using Kazoh.Table;

public class Component_Item_Character : GameComponent {

    public Action<Component_Item_Character> ClickedEvent;
    
    public UISprite ChaSprite;
    public UISprite FrameSprite;
    public UILabel NameLabel;

    public TableData_Character Data { get; private set; }
    private bool isSelected;
    public bool IsSelected
    {
        get { return isSelected; }
        set
        {
            isSelected = value;
            if (isSelected)
            {
                ChaSprite.color = Color.white;
                FrameSprite.alpha = 1f;
                if (ClickedEvent != null) ClickedEvent(this);
            }
            else
            {
                ChaSprite.color = Color.gray;
                FrameSprite.alpha = 0f;
            }
        }
    }

    public void Init(TableData_Character _data)
    {
        gameObject.name = "cha_" + _data.Id;
        Data = _data;
        SetConstantUI();
    }

    void SetConstantUI()
    {
        ChaSprite.spriteName = string.Format("{0}_{1:00}", Data.SpriteName, 2);
        NameLabel.text = TableManager.GetString(Data.Str);
    }

    void OnClick()
    {
        IsSelected = true;
    }
}
