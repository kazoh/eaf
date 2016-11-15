using UnityEngine;
using System.Collections;
using System;

using Kazoh.Table;

public class Component_Item_SaleTab : GameComponent {

    public Action<Component_Item_SaleTab> ClickedEvent;

    public UISprite TabSprite;
    public UILabel NameLabel;

    public Color SelectedColor;
    public Color DefaultColor;

    private bool isSelected;
    public bool IsSelected
    {
        get { return isSelected; }
        set
        {
            isSelected = value;
            if(isSelected)
            {
                TabSprite.color = SelectedColor;
            }
            else
            {
                TabSprite.color = DefaultColor;
            }
        }
    }

    public TableEnum.SaleCatecory Type { get; private set; }

    public void Init(int _type)
    {
        Type = (TableEnum.SaleCatecory)_type;
        NameLabel.text = TableManager.GetString("STR_UI_CATEGORY_" + Type.ToString());
        IsSelected = false;
    }

    void OnClick()
    {
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        if (ClickedEvent != null) ClickedEvent(this);
    }
}
