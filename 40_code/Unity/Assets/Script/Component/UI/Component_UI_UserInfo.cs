using UnityEngine;
using System.Collections;
using System;

public class Component_UI_UserInfo : GameComponent
{

    public Action<GameEnum.Menu> ClickedEvent;

    public UISprite ChaIconSprite;
    public UISprite IconFrameSprite;
    public UILabel NameLabel;
    public UILabel GoldLabel;
    public UILabel KeyLabel;
    public UILabel CoinLabel;
    public UILabel CashLabel;
    public UILabel HpLabel;
    public UIProgressBar HpBar;

    public override void Init()
    {
        base.Init();

        Data_User _data = GameProcess.GetGameDataManager().UserData;
        _data.ChangedEvent += OnChange;
        GameProcess.GetGameDataManager().ChangedCurCharacterEvent += OnChangeCurCha;
        OnChange(_data);
        OnChangeCurCha();
    }

    void OnChange(Data_User _data)
    {
        GoldLabel.text = string.Format("{0:#,###,###,##0}", _data.Gold);
        KeyLabel.text = string.Format("{0}", _data.Key);
        CoinLabel.text = string.Format("{0:#,##0}", _data.Coin);
        CashLabel.text = string.Format("{0:##,##0}", _data.Cash);
    }

    void OnChangeCurCha()
    {
        Slot_Character slot = GameProcess.GetGameDataManager().GetCurCharacter();
        if (!slot.IsEmpty)
        {
            ChaIconSprite.spriteName = slot.Data.IconName;
            IconFrameSprite.spriteName = "grade_0" + slot.Data.Grade;
            NameLabel.text = string.Format("+{0} {1}", slot.Data.EnchantNum, slot.Data.ChaName);
            HpBar.value = (slot.Data.CurHp + 0f) / slot.Data.MaxHp;
            HpLabel.text = string.Format("{0:##,##0}/{1:##,##0}", slot.Data.CurHp, slot.Data.MaxHp);
        }
        else
        {
            ChaIconSprite.spriteName = "icon_item_6000";
            IconFrameSprite.spriteName = "grade_01";
            NameLabel.text = "";
            HpBar.value = 0f;
            HpLabel.text = string.Format("{0:##,##0}", 0);
        }
    }

    public void OnClick_Exchange()
    {
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        if (ClickedEvent != null) ClickedEvent(GameEnum.Menu.EXCHANGE);
    }

    public void OnClick_Recharge()
    {
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        if (ClickedEvent != null) ClickedEvent(GameEnum.Menu.RECHARGE);
    }

    public void OnClick_Cash()
    {
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        if (ClickedEvent != null) ClickedEvent(GameEnum.Menu.CASHSHOP);
    }

    public void OnClick_Character()
    {
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        if (ClickedEvent != null) ClickedEvent(GameEnum.Menu.CHARACTER);
    }
}
