using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System;

using Kazoh.Table;

public class LayoutManager_UI_Game : LayoutManager {

    public UIPanel PopupPanel;
    public UIPanel MapPanel;

    public Component_UI_UserInfo UserInfoUI;
    public Component_UI_Menu MenuUI;
    public Component_UI_Shop ShopUI;
    public Component_UI_ItemInfo ItemInfoUI;
    public Component_UI_MapInfo MapInfoUI;
    public Component_UI_Character CharacterUI;
    public Component_UI_GameStart GameStartUI;
    public Component_UI_GameOver GameOverUI;
    public Component_UI_GameClear GameClearUI;
    public Component_UI_Inventory InventoryUI;
    public Component_UI_MapSelect MapSelectUI;
    public Component_UI_PopupShop PopupShopUI;
    public Component_UI_PopupRegion PopupRegionUI;
    public Component_UI_PopupEffect PopupEffectUI;
    public Component_UI_InputPopup InputPopupUI;
    public Component_UI_SettingPopup SettingPopupUI;
    public Component_UI_DailyReward DailyPopupUI;
    public Component_UI_PopupMenu MenuPopupUI;

    public UISprite BgSprite;

    public Component_UI_Loading PopupLoading { get; private set; }
    public Component_Popup_Common PopupCommon { get; private set; }
    public Component_UI_SettingPopup PopupSetup { get; private set; }
    public Component_Map CurMap { get; private set; }
    
    private int selectedRegionId = 1;

    private GameEnum.Menu curMenu;
    public GameEnum.Menu CurMenu
    {
        get { return curMenu; }
        private set
        {
            switch(value)
            {
                case GameEnum.Menu.CHARACTER:
                case GameEnum.Menu.INVENTORY:
                case GameEnum.Menu.MAP:
                case GameEnum.Menu.SHOP:
                    curMenu = value;
                    Debug.Log("CurMenu:" + curMenu.ToString());
                    break;
            }
        }
    }

    protected override IEnumerator Loading()
    {
        yield return base.Loading();

        /* 맵 프레임 배경 설정 */
#if UNITY_EDITOR
        SystemLanguage lang = GameProcess.Instance.Language;
#else
        SystemLanguage lang = Application.systemLanguage;
#endif
        switch (lang)
        {
            case SystemLanguage.Korean: BgSprite.spriteName = "title_ko"; break;
            case SystemLanguage.Japanese: BgSprite.spriteName = "title_ja"; break;
            default: BgSprite.spriteName = "title_en"; break;
        }

        /* 유저 정보 UI 초기화 */
        UserInfoUI.Init();
        UserInfoUI.ClickedEvent += OnClickMenu;

        /* 메뉴 UI 초기화 */
        MenuUI.Init();
        MenuUI.ClickedEvent += OnClickMenu;

        /* 상점 UI 초기화 */
        ShopUI.Init();
        ShopUI.ClickedEvent += BuyItem;

        /* 맵 정보 UI 초기화 */
        MapInfoUI.Init();
        MapInfoUI.ClickedEvent += OnClickStart;
        MapInfoUI.Hide();

        /* 게임 시작 UI 초기화 */
        GameStartUI.Init();
        GameStartUI.ClickedEvent += GameStart;
        //GameStartUI.Hide();

        /* 게임 오버 UI 초기화 */
        GameOverUI.Init();
        GameOverUI.ClickedEvent += OnClickGameEnd;
        GameOverUI.Hide();

        /* 게임 클리어 UI 초기화 */
        GameClearUI.Init();
        GameClearUI.ClickedEvent += OnClickGameEnd;
        GameClearUI.Hide();

        /* 아이템 정보 UI 초기화 */
        ItemInfoUI.Init();
        ItemInfoUI.SellItemEvent += OnSellItem;
        ItemInfoUI.UseItemEvent += OnUseItem;
        ItemInfoUI.EquipItemEvent += OnEquipItem;
        ItemInfoUI.EnchantItemEvent += OnEnchantItem;
        ItemInfoUI.Hide();

        /* 캐릭터 UI 초기화 */
        CharacterUI.Init();
        CharacterUI.AddCharacterEvent += OnAddCharacter;
        CharacterUI.SelectCharacterEvent += OnSelectCharacter;
        CharacterUI.DownGradeCharacterEvent += OnDownGradeCharacter;
        CharacterUI.EnchantCharacterEvent += OnEnchantCharacter;
        CharacterUI.DeleteCharacterEvent += OnDeleteCharacter;
        CharacterUI.DismountEvent += OnEquipItem;
        CharacterUI.AddSlotEvent += OnAddChaSlot;
        CharacterUI.Hide();

        /* 인벤토리 UI 초기화 */
        InventoryUI.Init();
        InventoryUI.ClickedEvent += OnClickSelectItem;
        InventoryUI.AddSlotEvent += OnAddItemSlot;
        InventoryUI.Hide();

        /* 퀘스트 선택 UI 초기화 */
        MapSelectUI.Init();
        MapSelectUI.ClickedEvent += OnClickMenu;
        MapSelectUI.ClickedChangeEvent += OnClickMenu;
        MapSelectUI.Show();

        /* 팝업샾 UI 초기화 */
        PopupShopUI.Init();
        PopupShopUI.ClickedEvent += BuyGoods;
        PopupShopUI.Hide();

        /* 지역 UI 초기화 */
        PopupRegionUI.Init();
        PopupRegionUI.ClickedEvent += OnSelectRegion;
        PopupRegionUI.Hide();

        /* 소환 UI 초기화 */
        PopupEffectUI.Init();
        PopupEffectUI.Hide();

        /* 입력 팝업 초기화 */
        InputPopupUI.Init();
        InputPopupUI.Hide();

        /* 세팅 팝업 초기화 */
        SettingPopupUI.Init();
        SettingPopupUI.Hide();

        /* 일일보상 팝업 초기화 */
        DailyPopupUI.Init();
        DailyPopupUI.Hide();

        /* 메뉴 팝업 초기화 */
        MenuPopupUI.Init();
        MenuPopupUI.ClickedEvent += OnClickMenu;
        MenuPopupUI.Hide();

        /* 배경음 적용 */
        GameProcess.PlaySound(SOUND_EFFECT.BGM, "bgm_01");

        OnClickMenu(GameEnum.Menu.MAP);
        OnFinishLoading();

        /* 일일보상 체크 */
        if (CheckDailyReward())
        {
            GameProcess.GetGameDataManager().Save();
            OnClickMenu(GameEnum.Menu.DAILY);
        }
    }

    void OnClickStart(int mapId)
    {
#if UNITY_EDITOR
        Debug.Log("게임 시작!! 맵 : " + mapId);
#endif
        GameProcess.ShowLoading();
        try
        {
            DBManager.GetServerState(delegate (int _state)
            {
                try
                {
                    switch (_state)
                    {
                        case -1: throw new GameException(GameException.ErrorCode.UnknownServerError);
                        case 0: throw new GameException(GameException.ErrorCode.ServerShutDown);
                        case 2: throw new GameException(GameException.ErrorCode.CloseTest);
                        default:
                            StartCoroutine(LoadingMap(mapId));
                            break;
                    }
                }
                catch (GameException e)
                {
                    Debug.LogError(e.Msg);
                    GameProcess.ShowError(e);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                    GameProcess.ShowError(new GameException(GameException.ErrorCode.Unknown, e.Message));
                }
            });
        }
        catch (GameException e)
        {
            Debug.LogError(e.Msg);
            GameProcess.ShowError(e);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            GameProcess.ShowError(new GameException(GameException.ErrorCode.Unknown, e.Message));
        }
    }

    IEnumerator LoadingMap(int _id)
    {
        yield return null;

        try
        {
            if (CurMap != null) DestroyMap();

            if (GameProcess.GetGameDataManager().GetCurCharacter().Data.CurHp == 0)
                throw new GameException(GameException.ErrorCode.HpIsZero);

            TableData_Map map = TableManager.GetGameData(_id) as TableData_Map;
            if (map.Grade < GameProcess.GetGameDataManager().GetCurCharacter().Data.Grade)
                throw new GameException(GameException.ErrorCode.OverGrade);
            if (GameProcess.GetGameDataManager().GetEmptyItemSlotNum() == 0) // < map.NeedSlotNum)
                throw new GameException(GameException.ErrorCode.NotEnoughItemSlot);

            CurMap = CreateMap(map);
            CurMap.ReadyEvent += OnReadyGame;
            CurMap.FinishedEvent += OnGameFinish;
            CurMap.Init(map, GameProcess.GetGameDataManager().GetCurCharacter().Data);
        }
        catch (GameException e)
        {
            GameProcess.ShowError(e);
            OnFinishLoading();
            OnClickGameEnd();
        }
        catch (Exception e)
        {
            GameProcess.ShowError(new GameException(GameException.ErrorCode.Unknown, e.Message));
            OnFinishLoading();
            OnClickGameEnd();
        }
    }

    Component_Map CreateMap(TableData_Map _data)
    {
        GameObject map = GameProcess.GetResourceManager().GetPrefabs(_data.PrefName);
        if (map == null) throw new GameException(GameException.ErrorCode.NoMapPrefabs);

        GameObject go = NGUITools.AddChild(MapPanel.gameObject, map);

        return go.GetComponent<Component_Map>();
    }

    void DestroyMap()
    {
        if (CurMap != null)
        {
            Destroy(CurMap.gameObject);
            CurMap = null;
        }
    }

    void OnReadyGame(Component_Map map)
    {
        try
        {
            //GameProcess.GetGameDataManager().UseKey(map.Data.Price);
            StartCoroutine(GameStartUIShow(map));
        }
        catch (GameException e)
        {
            GameProcess.ShowError(e);
            OnFinishLoading();
            OnClickGameEnd();
        }
        catch (Exception e)
        {
            GameProcess.ShowError(new GameException(GameException.ErrorCode.Unknown, e.Message));
            OnFinishLoading();
            OnClickGameEnd();
        }
    }

    IEnumerator GameStartUIShow(Component_Map map)
    {
        yield return new WaitForSeconds(1);

        MapInfoUI.Hide();
        OnFinishLoading();
        GameStartUI.Show(map.Data);
    }

    void GameStart()
    {
        if (CurMap != null) CurMap.GameStart();
    }

    bool isLock = false;
    void OnGameFinish(Component_Map map)
    {
        try
        {
            GameProcess.Instance.MuteBGM(true);
            if (map.IsClear)
            {
                GameProcess.GetGameDataManager().SaveMapRecord(map.Data.Id, map.Crown);
                GameProcess.GetGameDataManager().ExcuteReward(map.RewardInfo);
                GameProcess.GetGameDataManager().Save();
                GameClearUI.Show(map.Crown, map.RewardInfo);
            }
            else GameOverUI.Show();
        }
        catch (GameException e)
        {
            GameProcess.ShowError(e);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            GameProcess.ShowError(new GameException(GameException.ErrorCode.Unknown, e.Message));
        }
    }

    void OnClickGameEnd()
    {
        LayoutManager.UnLock();
        CheckMission();
        OnClickMenu(GameEnum.Menu.MAP);
        GameProcess.Instance.MuteBGM(false);
        DestroyMap();
    }

    void CheckMission()
    {
        TableData_Mission _mission = GameProcess.GetMissionManager().GetCompletedMission();
        if (_mission == null || _mission.Type == TableEnum.MissionType.None) return;
        Data_Reward _reward = new Data_Reward();
        string msg = string.Empty;
        switch(_mission.Type)
        {
            case TableEnum.MissionType.Gold:
                _reward.SetReward(_mission.Reward, 0);
                GameProcess.GetGameDataManager().ExcuteReward(_reward);
                msg = string.Format(TableManager.GetString("STR_MSG_REWARD_GOLD"), _mission.Reward);
                break;
            case TableEnum.MissionType.Coin:
                _reward.SetReward(0,_mission.Reward);
                GameProcess.GetGameDataManager().ExcuteReward(_reward);
                msg = string.Format(TableManager.GetString("STR_MSG_REWARD_COIN"), _mission.Reward);
                break;
            case TableEnum.MissionType.Item:
                TableData_Item _item = TableManager.GetGameData(_mission.Reward) as TableData_Item;
                GameProcess.GetGameDataManager().AddItem(_item,1);
                msg = string.Format(TableManager.GetString("STR_MSG_REWARD_ITEM"), TableManager.GetString(_item.Str),1);
                break;
        }
        GameProcess.GetGameDataManager().SetClearMission(_mission.Id);
        GameProcess.GetGameDataManager().Save();
        GameProcess.ShowPopup(NoticeType.OK, TableManager.GetString("STR_TITLE_MISSION"), msg, TableManager.GetString("STR_UI_OK"), delegate() 
        {
            // 숲 속 마을을 클리어 하면 리뷰를 요청한다.
            if (_mission.MapId == 50029) OnClickMenu(GameEnum.Menu.REVIEW);
        });
    }

    bool CheckDailyReward()
    {
        TableData_DailyReward _daily = GameProcess.GetGameDataManager().GetDailyReward();
        if (_daily == null || _daily.Type == TableEnum.MissionType.None) return false;
        Data_Reward _reward = new Data_Reward();
        switch (_daily.Type)
        {
            case TableEnum.MissionType.Gold:
                _reward.SetReward(_daily.Reward, 0);
                GameProcess.GetGameDataManager().ExcuteReward(_reward);
                break;
            case TableEnum.MissionType.Coin:
                _reward.SetReward(0, _daily.Reward);
                GameProcess.GetGameDataManager().ExcuteReward(_reward);
                break;
            case TableEnum.MissionType.Item:
                TableData_Item _item = TableManager.GetGameData(_daily.Reward) as TableData_Item;
                GameProcess.GetGameDataManager().AddItem(_item, 1);
                break;
        }

        GameProcess.GetGameDataManager().Save();
        return true;
    }

    void OnClickMenu(GameEnum.Menu type)
    {
        if (LayoutManager.IsLock()) return;

#if UNITY_EDITOR
        Debug.Log("메뉴 클릭 : " + type.ToString());
#endif

        CurMenu = type;
        if (CurMenu != GameEnum.Menu.MAP) GameProcess.PlaySound(SOUND_EFFECT.BGM, "bgm_01");

        string title = "";
        string msg = "";
        string text1 = "";
        string text2 = "";
        switch (type)
        {
            case GameEnum.Menu.INVENTORY:
                InventoryUI.Show();
                ItemInfoUI.UpdateUI(InventoryUI.SelectedSlot);
                ItemInfoUI.Show();
                CharacterUI.Hide();
                ShopUI.Hide();
                MapInfoUI.Hide();
                MapSelectUI.Hide();
                break;

            case GameEnum.Menu.SHOP:
                InventoryUI.Show();
                ShopUI.Show();
                CharacterUI.Hide();
                ItemInfoUI.Hide();
                MapInfoUI.Hide();
                MapSelectUI.Hide();
                break;

            case GameEnum.Menu.MAP:
                if(MapSelectUI.SelectedSlot != null) MapInfoUI.Show(MapSelectUI.SelectedSlot.Data);
                MapSelectUI.Show(selectedRegionId);
                CharacterUI.Hide();
                InventoryUI.Hide();
                ShopUI.Hide();
                ItemInfoUI.Hide();
                break;

            case GameEnum.Menu.REGION:
                PopupRegionUI.Show(selectedRegionId);
                break;

            case GameEnum.Menu.CHARACTER:
                InventoryUI.Show();
                CharacterUI.Show();
                ItemInfoUI.Hide();
                ShopUI.Hide();
                MapInfoUI.Hide();
                MapSelectUI.Hide();
                break;

            case GameEnum.Menu.CASHSHOP:
            case GameEnum.Menu.EXCHANGE:
            case GameEnum.Menu.RECHARGE:
            case GameEnum.Menu.SUMMON:
                PopupShopUI.Show(type);
                break;

            case GameEnum.Menu.SETUP:
                SettingPopupUI.Show();
                break;

            case GameEnum.Menu.DAILY:
                DailyPopupUI.Show();
                break;

            case GameEnum.Menu.ECT:
                MenuPopupUI.Show();
                break;

            case GameEnum.Menu.RANK:
                GameProcess.ShowError(new GameException(GameException.ErrorCode.NoService));
                break;

            case GameEnum.Menu.LIKE:
                title = TableManager.GetString("STR_TITLE_LIKE");
                msg = TableManager.GetString("STR_MSG_LIKE");
                text1 = TableManager.GetString("STR_UI_YES");
                text2 = TableManager.GetString("STR_UI_NO");
                GameProcess.ShowPopup(NoticeType.YES_NO, title, msg, text1, text2, delegate ()
                {
                    Application.OpenURL("https://www.facebook.com/10RPG");
                }, null);
                break;

            case GameEnum.Menu.REVIEW:
                title = TableManager.GetString("STR_TITLE_REVIEW");
                msg = TableManager.GetString("STR_MSG_REVIEW");
                text1 = TableManager.GetString("STR_UI_YES");
                text2 = TableManager.GetString("STR_UI_NO");
                GameProcess.ShowPopup(NoticeType.YES_NO, title, msg, text1, text2, delegate ()
                {
                    Application.OpenURL("https://docs.google.com/forms/d/e/1FAIpQLSflYLIUobxG1F5VLb3RnorH0XupA8glZ_1778adAgbGqieQoQ/viewform?c=0&w=1");
                }, null);
                break;

            case GameEnum.Menu.BUG:
                title = TableManager.GetString("STR_TITLE_BUG");
                msg = TableManager.GetString("STR_MSG_BUG");
                text1 = TableManager.GetString("STR_UI_YES");
                text2 = TableManager.GetString("STR_UI_NO");
                GameProcess.ShowPopup(NoticeType.YES_NO, title, msg, text1, text2, delegate ()
                {
                    Application.OpenURL("https://docs.google.com/forms/d/e/1FAIpQLSc6C69W5lCPe7iZetfcqWqnGD4cRArbs9LrYZE8SfM2Qx-BNg/viewform?c=0&w=1");
                }, null);
                break;

            case GameEnum.Menu.EXIT:
                Quit();
                break;

        }
    }

    void OnSelectRegion(GameComponent other)
    {
        GameProcess.ShowLoading();
        PopupRegionUI.Hide();
        StartCoroutine(SetRegion(other));
    }

    IEnumerator SetRegion(GameComponent other)
    {
        yield return null;
        if (other is Component_Item_Region)
        {
            Component_Item_Region region = other as Component_Item_Region;
            if (region.Data != null) selectedRegionId = region.Data.Id;
        }
        OnClickMenu(GameEnum.Menu.MAP);
        GameProcess.HideLoading();
    }

    void OnClickSelectItem(Slot_Item _item)
    {
        if (LayoutManager.IsLock()) return;

        switch (CurMenu)
        {
            case GameEnum.Menu.INVENTORY:
                ItemInfoUI.UpdateUI(InventoryUI.SelectedSlot);
                ItemInfoUI.Show();
                break;

            case GameEnum.Menu.SHOP:
                if(_item.CanSell) OnSellItem(_item);
                break;

            case GameEnum.Menu.CHARACTER:
                if (CharacterUI.Data != null && !CharacterUI.Data.IsEmpty)
                {
                    if (_item.CanEquip) OnEquipItem(_item);
                    else if (_item.CanUse) OnUseItem(_item);
                }
                else OnClickMenu(GameEnum.Menu.INVENTORY);
                break;

            default:
                OnClickMenu(GameEnum.Menu.INVENTORY);
                break;
        }
    }

    void BuyItem(GameComponent compo)
    {
        TableData_SaleList _data = null;
        TableData_Item _item = null; 
        if (compo is Component_UI_Shop)
        {
            _data = (compo as Component_UI_Shop).SelectedItem.Data;
            _item = TableManager.GetGameData(_data.ItemName) as TableData_Item;
        }
        if (_data == null) GameProcess.ShowError(new GameException(GameException.ErrorCode.NoGameData));
        else if(_item == null) GameProcess.ShowError(new GameException(GameException.ErrorCode.NoGameData));
        else
        {
            string title = TableManager.GetString("STR_TITLE_BUY");
            string msg = "";
            if (_data.Type == TableEnum.MoneyType.Coin)
            {
                int price = _data.Amount * _item.CoinPrice;
                string itemName = TableManager.GetString(_item.Str);
                msg = string.Format(TableManager.GetString("STR_MSG_BUY_COIN"),price,itemName, _data.Amount);
            }
            else
            {
                int price = _data.Amount * _item.Price;
                string itemName = TableManager.GetString(_item.Str);
                msg = string.Format(TableManager.GetString("STR_MSG_BUY_GOLD"), price, itemName, _data.Amount);
            }
            string text1 = TableManager.GetString("STR_UI_YES");
            string text2 = TableManager.GetString("STR_UI_NO");

            GameProcess.ShowPopup(NoticeType.YES_NO, title, msg, text1, text2, delegate ()
            {
                try
                {
                    GameProcess.GetGameDataManager().BuyItem(_data);
                    GameProcess.GetGameDataManager().Save();
                }
                catch (GameException e)
                {
                    GameProcess.ShowError(e);
                }
                catch (Exception e)
                {
                    GameProcess.ShowError(new GameException(GameException.ErrorCode.Unknown, e.Message));
                }
            }, null);
        }
    }

    void BuyGoods(GameComponent other)
    {
        TableData_GoodsList _data = null;
        if (other is Component_Item_Goods)
        {
            _data = (other as Component_Item_Goods).Data;
        }
        else if(other is Component_Item_Summon)
        {
            _data = (other as Component_Item_Summon).Data;
        }
        if (_data != null)
        {
            string title = TableManager.GetString("STR_TITLE_BUY");
            string msg = "";
            switch (_data.Type)
            {
                case TableEnum.GoodsType.CashToGold:
                case TableEnum.GoodsType.CashToCoin:
                    msg = string.Format(TableManager.GetString("STR_MSG_BYCASH"), _data.Price);
                    break;

                case TableEnum.GoodsType.CoinToKey:
                case TableEnum.GoodsType.Summon:
                    msg = string.Format(TableManager.GetString("STR_MSG_BYCOIN"), _data.Price);
                    break;

                case TableEnum.GoodsType.Cash:
                default:
                    msg = string.Format(TableManager.GetString("STR_MSG_BUY"), TableManager.GetString(_data.Str));
                    break;
            }
            string text1 = TableManager.GetString("STR_UI_YES");
            string text2 = TableManager.GetString("STR_UI_NO");

            GameProcess.ShowPopup(NoticeType.YES_NO, title, msg, text1, text2, delegate()
            {
                try
                {
                    int membership = GameProcess.GetGameDataManager().GetMembership();
                    if (membership < _data.Membership)
                    {
                        switch(_data.Membership)
                        {
                            case 1: throw new GameException(GameException.ErrorCode.OnlySilverMembership);
                            case 2: throw new GameException(GameException.ErrorCode.OnlyGoldMembership);
                            case 3: throw new GameException(GameException.ErrorCode.OnlyDiaMembership);
                            case 4: throw new GameException(GameException.ErrorCode.OnlyPlatinumMembership);
                            default: throw new GameException(GameException.ErrorCode.CanNotUseMembershipService);
                        }
                    }

                    if (_data.PurchaseLimit > 0 && GameProcess.GetGameDataManager().GetPurchaseCount(_data.ProductId) >= _data.PurchaseLimit)
                    {
                        throw new GameException(GameException.ErrorCode.OverPurchaseLimit);
                    }

                    switch (_data.Type)
                    {
                        case TableEnum.GoodsType.CashToGold:
                            GameProcess.GetGameDataManager().ExchangeGold(_data, delegate()
                            {
                                title = TableManager.GetString("STR_TITLE_BUY_SUCCESS");
                                msg = string.Format(TableManager.GetString("STR_MSG_BUY_SUCCESS"), TableManager.GetString(_data.Str)); 
                                text1 = TableManager.GetString("STR_UI_OK");
                                GameProcess.ShowPopup(NoticeType.OK, title, msg, text1, null);
                            });
                            break;

                        case TableEnum.GoodsType.CashToCoin:
                            GameProcess.GetGameDataManager().ExchangeCoin(_data, delegate()
                            {
                                title = TableManager.GetString("STR_TITLE_BUY_SUCCESS");
                                msg = string.Format(TableManager.GetString("STR_MSG_BUY_SUCCESS"), TableManager.GetString(_data.Str));
                                text1 = TableManager.GetString("STR_UI_OK");
                                GameProcess.ShowPopup(NoticeType.OK, title, msg, text1, null);
                            });
                            break;

                        case TableEnum.GoodsType.Cash:
                            GameProcess.GetGameDataManager().BuyCash(_data, delegate()
                            {
                                title = TableManager.GetString("STR_TITLE_BUY_SUCCESS");
                                msg = string.Format(TableManager.GetString("STR_MSG_BUY_SUCCESS"), TableManager.GetString(_data.Str));
                                text1 = TableManager.GetString("STR_UI_OK");
                                GameProcess.ShowPopup(NoticeType.OK, title, msg, text1, null);
                            });
                            break;

                        case TableEnum.GoodsType.CoinToKey:
                            GameProcess.GetGameDataManager().Recharge(_data);
                            break;

                        case TableEnum.GoodsType.Summon:
                            TableData_Character[] chaArray = GameProcess.GetGameDataManager().SummonCharater(_data, CharacterUI.Data);
                            title = TableManager.GetString("STR_TITLE_SUMMON");
                            string grade = chaArray[0].GradeStr;
                            if(chaArray.Length > 1)
                                msg = string.Format(TableManager.GetString("STR_MSG_SUMMON_02"), grade, TableManager.GetString(chaArray[0].Str), chaArray.Length - 1);
                            else msg = string.Format(TableManager.GetString("STR_MSG_SUMMON"), grade, TableManager.GetString(chaArray[0].Str));

                            string[] spriteNames = new string[chaArray.Length];
                            for(int i=0; i < chaArray.Length; ++i)
                            {
                                spriteNames[i] = chaArray[i].SpriteName;
                            }
                            PopupEffectUI.Show(Component_UI_PopupEffect.EffectType.Summon, spriteNames, title, "", msg);
                            break;
                    }
                    GameProcess.GetGameDataManager().Save();
                }
                catch (GameException e)
                {
                    GameProcess.ShowError(e);
                }
                catch (Exception e)
                {
                    GameProcess.ShowError(new GameException(GameException.ErrorCode.Unknown, e.Message));
                }

            },null);
        }
    }

    void OnAddCharacter(Slot_Character _slot)
    {
        if (_slot.IsEmpty) OnClickMenu(GameEnum.Menu.SUMMON);
    }

    void OnSelectCharacter(Slot_Character _slot)
    {
        try
        {
            GameProcess.GetGameDataManager().SelectCharacter(_slot);
            GameProcess.GetGameDataManager().Save();
            string title = TableManager.GetString("STR_UI_SELECT");
            string msg = string.Format(TableManager.GetString("STR_MSG_CHA_CHANGE"), _slot.Data.EnchantNum, _slot.Data.ChaName);
            string text1 = TableManager.GetString("STR_UI_OK");
            GameProcess.ShowPopup(NoticeType.OK, title, msg, text1, null);
        }
        catch (GameException e)
        {
            GameProcess.ShowError(e);
        }
        catch (Exception e)
        {
            GameProcess.ShowError(new GameException(GameException.ErrorCode.Unknown, e.Message));
        }
    }

    void OnDownGradeCharacter(Slot_Character _slot)
    {
        if (_slot.IsEmpty)
        {
            GameProcess.ShowError(new GameException(GameException.ErrorCode.CharacterSlotIsEmpty));
            return;
        }
        if (_slot.Data.Grade < 2)
        {
            GameProcess.ShowError(new GameException(GameException.ErrorCode.CanNotDGradeCharacter));
            return;
        }

        int price = GameProcess.GetGameConfig().CostDownGradeCha;
        string title = TableManager.GetString("STR_UI_DOWN");
        string msg = string.Format(TableManager.GetString("STR_MSG_CHA_DOWN_1"), price, _slot.Data.EnchantNum, _slot.Data.ChaName);
        string text1 = TableManager.GetString("STR_UI_YES");
        string text2 = TableManager.GetString("STR_UI_NO");
        GameProcess.ShowPopup(NoticeType.YES_NO, title, msg, text1, text2, delegate ()
        {
            try
            {
                string result = GameProcess.GetGameDataManager().DownGradeCharacter(_slot);
                GameProcess.GetGameDataManager().Save();

                msg = string.Format(TableManager.GetString("STR_MSG_CHA_DOWN_2"), _slot.Data.ChaName, result);
                PopupEffectUI.Show(Component_UI_PopupEffect.EffectType.DownGrade,_slot.Data.Data.SpriteName, title, "", msg);
            }
            catch (GameException e)
            {
                GameProcess.ShowError(e);
            }
            catch (Exception e)
            {
                GameProcess.ShowError(new GameException(GameException.ErrorCode.Unknown, e.Message));
            }

        }, null);
    }

    void OnEnchantCharacter(Slot_Character _slot)
    {
        if (_slot.IsEmpty)
        {
            GameProcess.ShowError(new GameException(GameException.ErrorCode.CharacterSlotIsEmpty));
            return;
        }
        if (_slot.Data.EnchantNum > 4)
        {
            GameProcess.ShowError(new GameException(GameException.ErrorCode.CanNotEnchantCharacter));
            return;
        }

        int price = GameProcess.GetGameConfig().CostEnchantCha;
        string title = TableManager.GetString("STR_UI_CHA_ENCHANT");
        string msg = string.Format(TableManager.GetString("STR_MSG_CHA_ENCHANT_1"), price, _slot.Data.EnchantNum, _slot.Data.ChaName);
        string text1 = TableManager.GetString("STR_UI_YES");
        string text2 = TableManager.GetString("STR_UI_NO");
        GameProcess.ShowPopup(NoticeType.YES_NO, title, msg, text1, text2, delegate ()
        {
            try
            {
                string result = GameProcess.GetGameDataManager().EnchantCharacter(_slot);
                GameProcess.GetGameDataManager().Save();

                msg = string.Format(TableManager.GetString("STR_MSG_CHA_ENCHANT_2"), _slot.Data.ChaName, result);
                PopupEffectUI.Show(Component_UI_PopupEffect.EffectType.ChaEnchant, _slot.Data.Data.SpriteName, title, "", msg);
            }
            catch (GameException e)
            {
                GameProcess.ShowError(e);
            }
            catch (Exception e)
            {
                GameProcess.ShowError(new GameException(GameException.ErrorCode.Unknown, e.Message));
            }

        }, null);
    }

    void OnDeleteCharacter(Slot_Character _slot)
    {
        if (_slot.IsEmpty)
        {
            GameProcess.ShowError(new GameException(GameException.ErrorCode.CharacterSlotIsEmpty));
            return;
        }
        if (_slot.IsDefaultCha)
        {
            GameProcess.ShowError(new GameException(GameException.ErrorCode.CanNotDeleteDefaultCharacter));
            return;
        }
        if (GameProcess.GetGameDataManager().GetCharacterNum() < 2)
        {
            GameProcess.ShowError(new GameException(GameException.ErrorCode.CanNotDeleteCharacter));
            return;
        }
        if (_slot == GameProcess.GetGameDataManager().GetCurCharacter())
        {
            GameProcess.ShowError(new GameException(GameException.ErrorCode.CanNotDeleteCharacter));
            return;
        }

        string title = TableManager.GetString("STR_UI_DELETE");
        string msg = string.Format(TableManager.GetString("STR_MSG_DELETE_1"), _slot.Data.EnchantNum, _slot.Data.ChaName);
        string text1 = TableManager.GetString("STR_UI_YES");
        string text2 = TableManager.GetString("STR_UI_NO");
        GameProcess.ShowPopup(NoticeType.YES_NO, title, msg, text1, text2, delegate ()
        {
            try
            {
                GameProcess.GetGameDataManager().DeleteCharacter(_slot);
                GameProcess.GetGameDataManager().Save();
                msg = TableManager.GetString("STR_MSG_DELETE_2");
                text1 = TableManager.GetString("STR_UI_OK");
                GameProcess.ShowPopup(NoticeType.OK, title, msg, text1, null);
            }
            catch (GameException e)
            {
                GameProcess.ShowError(e);
            }
            catch (Exception e)
            {
                GameProcess.ShowError(new GameException(GameException.ErrorCode.Unknown, e.Message));
            }

        }, null);
    }

    void OnAddChaSlot()
    {
        string title = TableManager.GetString("STR_UI_ADD_CHA_SLOT");
        string msg = string.Format(TableManager.GetString("STR_MSG_ADD_CHA_SLOT"), GameProcess.GetGameConfig().CostAddChaSlot);
        string text1 = TableManager.GetString("STR_UI_YES");
        string text2 = TableManager.GetString("STR_UI_NO");
        GameProcess.ShowPopup(NoticeType.YES_NO, title, msg, text1, text2, delegate ()
        {
            try
            {
                GameProcess.GetGameDataManager().AddCharacterSlot(false);
                GameProcess.GetGameDataManager().Save();
                msg = TableManager.GetString("STR_MSG_ADDED_CHA_SLOT");
                text1 = TableManager.GetString("STR_UI_OK");
                GameProcess.ShowPopup(NoticeType.OK, title, msg, text1, null);
            }
            catch (GameException e)
            {
                GameProcess.ShowError(e);
            }
            catch (Exception e)
            {
                GameProcess.ShowError(new GameException(GameException.ErrorCode.Unknown, e.Message));
            }

        }, null);
    }

    void OnSellItem(Slot_Item _slot)
    {
        if (_slot.IsEmpty)
        {
            GameProcess.ShowError(new GameException(GameException.ErrorCode.ItemSlotIsEmpty));
            return;
        }
        if (!_slot.CanSell)
        {
            GameProcess.ShowError(new GameException(GameException.ErrorCode.CanNotSellItem));
            return;
        }
        if(_slot.IsEquipped)
        {
            GameProcess.ShowError(new GameException(GameException.ErrorCode.CanNotSellEquippedItem));
            return;
        }

        string title = TableManager.GetString("STR_UI_SELL");
        string text1 = TableManager.GetString("STR_UI_SELL");
        string text2 = TableManager.GetString("STR_UI_CANCEL");
        if(_slot.Data.CanStack && _slot.Data.Num > 1)
        {
            string msg = TableManager.GetString("STR_MSG_INPUT_NUM");
            InputPopupUI.Show(title, msg, text1, text2, _slot.Data.Num, delegate (int num)
            {
                try
                {
                    GameProcess.GetGameDataManager().SellItem(_slot, num);
                    GameProcess.GetGameDataManager().Save();

                    msg = TableManager.GetString("STR_MSG_SELL_2");
                    text1 = TableManager.GetString("STR_UI_OK");
                    GameProcess.ShowPopup(NoticeType.OK, title, msg, text1, null);
                    OnClickMenu(CurMenu);
                }
                catch (GameException e)
                {
                    GameProcess.ShowError(e);
                }
                catch (Exception e)
                {
                    GameProcess.ShowError(new GameException(GameException.ErrorCode.Unknown, e.Message));
                }
            }, null);

        }
        else
        {
            string msg = string.Format(TableManager.GetString("STR_MSG_SELL_1"), _slot.Data.EnchantNum, _slot.Data.ItemName);
            GameProcess.ShowPopup(NoticeType.YES_NO, title, msg, text1, text2, delegate ()
            {
                try
                {
                    GameProcess.GetGameDataManager().SellItem(_slot, 1);
                    GameProcess.GetGameDataManager().Save();

                    msg = TableManager.GetString("STR_MSG_SELL_3");
                    text1 = TableManager.GetString("STR_UI_OK");
                    GameProcess.ShowPopup(NoticeType.OK, title, msg, text1, null);
                    OnClickMenu(CurMenu);
                }
                catch (GameException e)
                {
                    GameProcess.ShowError(e);
                }
                catch (Exception e)
                {
                    GameProcess.ShowError(new GameException(GameException.ErrorCode.Unknown, e.Message));
                }

            }, null);
        }
    }

    void OnUseItem(Slot_Item _slot)
    {
        if (_slot.IsEmpty)
        {
            GameProcess.ShowError(new GameException(GameException.ErrorCode.ItemSlotIsEmpty));
            return;
        }
        if (!_slot.CanUse)
        {
            GameProcess.ShowError(new GameException(GameException.ErrorCode.CanNotUseItem));
            return;
        }

        string title = TableManager.GetString("STR_UI_USE");
        string msg = string.Format(TableManager.GetString("STR_MSG_USE_1"), _slot.Data.ItemName);
        string text1 = TableManager.GetString("STR_UI_USE");
        string text2 = TableManager.GetString("STR_UI_CANCEL");
        GameProcess.ShowPopup(NoticeType.YES_NO, title, msg, text1, text2, delegate ()
        {
            try
            {
                Slot_Character target = null;
                if (CurMenu == GameEnum.Menu.CHARACTER) target = CharacterUI.Data;
                else target = GameProcess.GetGameDataManager().GetCurCharacter();
                if (_slot.Data.Data.EffectType == TableEnum.EffectType.SUMMON)
                {
                    TableData_Character cha = TableManager.GetGameData(_slot.Data.Data.EffectValue) as TableData_Character;
                    string[] spriteNames = { cha.SpriteName };
                    title = TableManager.GetString("STR_TITLE_SUMMON");
                    string grade = cha.GradeStr;
                    msg = string.Format(TableManager.GetString("STR_MSG_SUMMON"), grade, TableManager.GetString(cha.Str));
                    GameProcess.GetGameDataManager().UseItem(_slot, target);
                    PopupEffectUI.Show(Component_UI_PopupEffect.EffectType.Summon, spriteNames, title, "", msg);
                }
                else GameProcess.GetGameDataManager().UseItem(_slot, target);
                GameProcess.GetGameDataManager().Save();

                OnClickMenu(CurMenu);
            }
            catch (GameException e)
            {
                GameProcess.ShowError(e);
            }
            catch (Exception e)
            {
                GameProcess.ShowError(new GameException(GameException.ErrorCode.Unknown, e.Message));
            }

        }, null);
    }

    void OnEquipItem(Slot_Item _slot)
    {
        if (_slot.IsEmpty)
        {
            GameProcess.ShowError(new GameException(GameException.ErrorCode.ItemSlotIsEmpty));
            return;
        }
        if (!_slot.CanEquip)
        {
            GameProcess.ShowError(new GameException(GameException.ErrorCode.CanNotEquipItem));
            return;
        }

        if (_slot.IsEquipped)
        {
            string title = TableManager.GetString("STR_UI_DISMOUNT");
            string msg = string.Format(TableManager.GetString("STR_MSG_DISMOUNT_1"), _slot.Data.ItemName);
            string text1 = TableManager.GetString("STR_UI_DISMOUNT");
            string text2 = TableManager.GetString("STR_UI_CANCEL");
            GameProcess.ShowPopup(NoticeType.YES_NO, title, msg, text1, text2, delegate ()
            {
                try
                {
                    GameProcess.PlaySound(SOUND_EFFECT.EQUIP);
                    GameProcess.GetGameDataManager().DismountItem(_slot);
                    GameProcess.GetGameDataManager().Save();
                    OnClickMenu(CurMenu);
                }
                catch (GameException e)
                {
                    GameProcess.ShowError(e);
                }
                catch (Exception e)
                {
                    GameProcess.ShowError(new GameException(GameException.ErrorCode.Unknown, e.Message));
                }

            }, null);
        }
        else
        {
            string title = TableManager.GetString("STR_UI_MOUNT");
            string msg = string.Format(TableManager.GetString("STR_MSG_MOUNT_1"), _slot.Data.ItemName);
            string text1 = TableManager.GetString("STR_UI_MOUNT");
            string text2 = TableManager.GetString("STR_UI_CANCEL");
            GameProcess.ShowPopup(NoticeType.YES_NO, title, msg, text1, text2, delegate ()
            {
                Slot_Character target = null;
                try
                {
                    if (CurMenu == GameEnum.Menu.CHARACTER) target = CharacterUI.Data;
                    else target = GameProcess.GetGameDataManager().GetCurCharacter();
                    GameProcess.PlaySound(SOUND_EFFECT.EQUIP);
                    GameProcess.GetGameDataManager().EquipItem(_slot, target);
                    GameProcess.GetGameDataManager().Save();
                    OnClickMenu(CurMenu);
                }
                catch (GameException e)
                {
                    GameProcess.ShowError(e);
                }
                catch (Exception e)
                {
                    GameProcess.ShowError(new GameException(GameException.ErrorCode.Unknown, e.Message));
                }

            }, null);
        }
    }

    void OnEnchantItem(Slot_Item _slot)
    {
        if (_slot.IsEmpty)
        {
            GameProcess.ShowError(new GameException(GameException.ErrorCode.ItemSlotIsEmpty));
            return;
        }
        if (!_slot.CanEnchant)
        {
            GameProcess.ShowError(new GameException(GameException.ErrorCode.CanNotEnchantItem));
            return;
        }
        if (_slot.IsEquipped)
        {
            GameProcess.ShowError(new GameException(GameException.ErrorCode.CanNotEnchantEquippedItem));
            return;
        }

        int price = GameProcess.GetGameConfig().CostEnchantItem;
        string title = TableManager.GetString("STR_UI_ENCHANT");
        string msg = string.Format(TableManager.GetString("STR_MSG_ENCHANT_1"), price, _slot.Data.EnchantNum, _slot.Data.ItemName);
        if (_slot.Data.EnchantNum > 3) msg +="\n\n" + TableManager.GetString("STR_MSG_ENCHANT_2");
        string text1 = TableManager.GetString("STR_UI_ENCHANT");
        string text2 = TableManager.GetString("STR_UI_CANCEL");
        GameProcess.ShowPopup(NoticeType.YES_NO, title, msg, text1, text2, delegate ()
        {
            try
            {
                GameProcess.GetGameDataManager().EnchantItem(_slot);
                GameProcess.GetGameDataManager().Save();
                string result = string.Empty;
                bool success = false;
                if(_slot.IsEmpty)
                {
                    result = TableManager.GetString("STR_TITLE_ENCHANT_FAIL");
                    msg = TableManager.GetString("STR_MSG_ENCHANT_FAIL");
                }
                else
                {
                    result = TableManager.GetString("STR_TITLE_ENCHANT_SUCCESS");
                    msg = TableManager.GetString("STR_MSG_ENCHANT_SUCCESS");
                    success = true;
                }
                
                PopupEffectUI.Show(Component_UI_PopupEffect.EffectType.Enchant,"", title, result, msg, success);
                OnClickMenu(CurMenu);
            }
            catch (GameException e)
            {
                GameProcess.ShowError(e);
            }
            catch (Exception e)
            {
                GameProcess.ShowError(new GameException(GameException.ErrorCode.Unknown, e.Message));
            }

        }, null);

    }

    void OnAddItemSlot()
    {
        string title = TableManager.GetString("STR_UI_ADD_CHA_SLOT");
        string msg = string.Format(TableManager.GetString("STR_MSG_ADD_ITEM_SLOT"), GameProcess.GetGameConfig().CostAddItemSlot);
        string text1 = TableManager.GetString("STR_UI_YES");
        string text2 = TableManager.GetString("STR_UI_NO");
        GameProcess.ShowPopup(NoticeType.YES_NO, title, msg, text1, text2, delegate ()
        {
            try
            {
                GameProcess.GetGameDataManager().AddItemSlot(false);
                GameProcess.GetGameDataManager().Save();
                msg = TableManager.GetString("STR_MSG_ADDED_ITEM_SLOT");
                text1 = TableManager.GetString("STR_UI_OK");
                GameProcess.ShowPopup(NoticeType.OK, title, msg, text1, null);
            }
            catch (GameException e)
            {
                GameProcess.ShowError(e);
            }
            catch (Exception e)
            {
                GameProcess.ShowError(new GameException(GameException.ErrorCode.Unknown, e.Message));
            }

        }, null);
    }

    protected override void Quit()
    {
        base.Quit();
    }
}
