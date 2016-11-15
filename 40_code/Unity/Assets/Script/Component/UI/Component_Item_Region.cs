using UnityEngine;
using System.Collections;
using System;

using Kazoh.Table;

public class Component_Item_Region : GameComponent
{

    public Action<Component_Item_Region> ClickedEvent;

    public string RegionName;
    public UILabel NameLabel;
    public UISprite GuidSprite;
    public UISprite MarkerSprite;

    public TableData_Region Data { get; private set; }

    public void Init(TableData_Region _data)
    {
        if(_data == null)
        {
            Hide();
            return;
        }

        Data = _data;
        
        GuidSprite.alpha = 0f;
        NameLabel.text = TableManager.GetString(_data.Str);
        Show();
    }

    public void ShowMarker()
    {
        try
        {
            string chaName = GameProcess.GetGameDataManager().GetCurCharacter().Data.SpriteName;
            MarkerSprite.spriteName = chaName + "_02";
            MarkerSprite.alpha = 1f;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            HideMarker();
        }
    }

    public void HideMarker()
    {
        MarkerSprite.alpha = 0f;
    }

    void OnClick()
    {
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        if (ClickedEvent != null) ClickedEvent(this);
    }
}