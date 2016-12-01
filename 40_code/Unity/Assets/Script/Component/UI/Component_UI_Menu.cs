using UnityEngine;
using System.Collections;
using System;

using Kazoh.Table;

public class Component_UI_Menu : GameComponent {

    public Action<GameEnum.Menu> ClickedEvent;
    
    public UISprite NewChaSprite;
    public UISprite NewMapSprite;
    public UISprite NewItemSprite;

    public UILabel ChaLabel;
    public UILabel MapLabel;
    public UILabel InvenLabel;
    public UILabel ShopLabel;
    public UILabel SummonLabel;
    public UILabel EctLabel;

    public override void Init()
    {
        base.Init();
        GameProcess.GetGameDataManager().NewChaEvent += OnNewCha;
        GameProcess.GetGameDataManager().NewMapEvent += OnNewMap;
        GameProcess.GetGameDataManager().NewItemEvent += OnNewItem;
        SetConstantUI();
        OnNewCha();
        OnNewMap();
        OnNewItem();
    }

    void SetConstantUI()
    {
        ChaLabel.text = TableManager.GetString("STR_MENU_01");
        MapLabel.text = TableManager.GetString("STR_MENU_02");
        InvenLabel.text = TableManager.GetString("STR_MENU_03");
        ShopLabel.text = TableManager.GetString("STR_MENU_04");
        SummonLabel.text = TableManager.GetString("STR_MENU_15");
        EctLabel.text = TableManager.GetString("STR_MENU_06");
    }

    void OnClicked(GameEnum.Menu menu)
    {
        if (ClickedEvent != null)
        {
            ClickedEvent(menu);
        }
    }

    public void OnClick_Cha()
    {
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        OnClicked(GameEnum.Menu.CHARACTER);
        GameProcess.GetGameDataManager().ConformCharacter();
        OnNewCha();
    }

    public void OnClick_Inven()
    {
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        OnClicked(GameEnum.Menu.INVENTORY);
        OnNewItem();
    }

    public void OnClick_Map()
    {
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        OnClicked(GameEnum.Menu.MAP);
    }

    public void OnClick_Shop()
    {
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        OnClicked(GameEnum.Menu.SHOP);
    }

    public void OnClick_Recharge()
    {
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        OnClicked(GameEnum.Menu.RECHARGE);
    }

    public void OnClick_Exchange()
    {
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        OnClicked(GameEnum.Menu.EXCHANGE);
    }

    public void OnClick_CashShop()
    {
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        OnClicked(GameEnum.Menu.CASHSHOP);
    }

    public void OnClick_Setup()
    {
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        OnClicked(GameEnum.Menu.SETUP);
    }

    public void OnClick_Rank()
    {
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        OnClicked(GameEnum.Menu.RANK);
    }

    public void OnClick_Daily()
    {
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        OnClicked(GameEnum.Menu.DAILY);
    }

    public void OnClick_Review()
    {
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        OnClicked(GameEnum.Menu.REVIEW);
    }

    public void OnClick_Bug()
    {
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        OnClicked(GameEnum.Menu.BUG);
    }

    public void OnClick_Exit()
    {
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        OnClicked(GameEnum.Menu.EXIT);
    }

    public void OnClick_Ect()
    {
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        OnClicked(GameEnum.Menu.ECT);
    }

    public void OnClick_Like()
    {
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        OnClicked(GameEnum.Menu.LIKE);
    }

    public void OnClick_Summon()
    {
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        OnClicked(GameEnum.Menu.SUMMON);
    }

    void OnNewMap()
    {
        if (GameProcess.GetGameDataManager().HasNewMap()) NewMapSprite.gameObject.SetActive(true);
        else NewMapSprite.gameObject.SetActive(false);
    }

    void OnNewCha()
    {
        if (GameProcess.GetGameDataManager().HasNewCharacter()) NewChaSprite.gameObject.SetActive(true);
        else NewChaSprite.gameObject.SetActive(false);
    }

    void OnNewItem()
    {
        if (GameProcess.GetGameDataManager().HasNewItem()) NewItemSprite.gameObject.SetActive(true);
        else NewItemSprite.gameObject.SetActive(false);
    }
}
