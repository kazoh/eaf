using UnityEngine;
using System.Collections;
using System;

public class Component_UI_Menu : GameComponent {

    public Action<GameEnum.Menu> ClickedEvent;

    public UIScrollView ScrollView;
    public UISprite NewChaSprite;
    public UISprite NewMapSprite;
    public UISprite NewItemSprite;

    public override void Init()
    {
        base.Init();
        GameProcess.GetGameDataManager().NewChaEvent += OnNewCha;
        GameProcess.GetGameDataManager().NewMapEvent += OnNewMap;
        GameProcess.GetGameDataManager().NewItemEvent += OnNewItem;
        OnNewCha();
        OnNewMap();
        OnNewItem();
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

    public void OnClick_Achievement()
    {
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        OnClicked(GameEnum.Menu.ACHIEVEMENT);
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

    public void OnClick_Left()
    {
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        ScrollView.Scroll(0.7f);
    }

    public void OnCLick_Right()
    {
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        ScrollView.Scroll(-0.7f);
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
