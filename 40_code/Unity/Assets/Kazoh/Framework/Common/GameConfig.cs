using UnityEngine;
using System.Collections;
using System;
using MiniJSON;

public class GameConfig
{
    public int Ver { get; private set; }                // 게임 설정 데이터 버전.

    public int MaxCash { get; private set; }            // 최대 보석 갯수.
    public int MaxCoin { get; private set; }            // 최대 코인 갯수.
    public int MaxGold { get; private set; }            // 최대 골드.

    public int DefaultGold { get; private set; }        // 최초 보유하는 골드.
    public int DefaultCoin { get; private set; }        // 최초 보유하는 코인.

    public int CostAddChaSlot { get; private set; }     // 캐릭터 슬롯 확장 비용, 코인.
    public int CostAddItemSlot { get; private set; }    // 가방 확장 비용, 코인.
    public int CostEnchantItem { get; private set; }    // 아이템 강화 비용, 골드.
    public int CostEnchantCha { get; private set; }     // 캐릭터 각성 비용, 골드.
    public int CostDownGradeCha { get; private set; }   // 캐릭터 등급하락 비용, 코인.

    public int DefaultChaSlotNum { get; private set; }  // 기본 캐릭터 슬롯 개수.
    public int DefaultItemSlotNum { get; private set; } // 기본 아이템 슬롯 개수.
    public int MaxChaSlotNum { get; private set; }      // 최대 캐릭터 슬롯 개수.
    public int MaxItemSlotNum { get; private set; }     // 최대 아이템 슬롯 개수.
    public int AddedChaSlotNum { get; private set; }    // 확장 시 추가되는 캐릭터 슬롯 개수.
    public int AddedItemSlotNum { get; private set; }   // 확장 시 추가되는 아이템 슬롯 개수.

    public int DefaultASpd { get; private set; }        // 기본 근거리 공격 속도.
    public int DefaultSpd { get; private set; }         // 기본 이동 속도.
    public int SpdPerDex { get; private set; }          // 덱스당 이동 속도.
    public int DefaultCri { get; private set; }         // 기본 크리티컬.
    public int CriPerLuc { get; private set; }          // 운 당 크리티컬.
    public int MaxCritical { get; private set; }        // 최대 크리티컬.
    public int DefaultCriPercent { get; private set; }  // 최대 크리티컬.
    public int HpPerCon { get; private set; }           // 콘 당 최대Hp.

    public int[] VipGradeArray;                         // Vip 등급 달성을 위한 구매액 배열.

    public GameConfig()
    {
        Ver = 0;

        MaxCash = 99999;
        MaxCoin = 9999;
        MaxGold = 999999999;

        DefaultGold = 0;
        DefaultCoin = 0;

        CostAddChaSlot = 25;
        CostAddItemSlot = 50;
        CostEnchantItem = 50000;
        CostEnchantCha = 100000;
        CostDownGradeCha = 25;

        DefaultChaSlotNum = 4;
        DefaultItemSlotNum = 10;
        MaxChaSlotNum = 30;
        MaxItemSlotNum = 100;
        AddedChaSlotNum = 1;
        AddedItemSlotNum = 5;

        DefaultASpd = 700;
        DefaultSpd = 10;
        SpdPerDex = 10;
        DefaultCri = 5;
        CriPerLuc = 1;
        MaxCritical = 25;
        HpPerCon = 10;
        DefaultCriPercent = 120;

        VipGradeArray = new int[5];
        VipGradeArray[0] = 1000;
        VipGradeArray[1] = 5000;
        VipGradeArray[2] = 10000;
        VipGradeArray[3] = 50000;
        VipGradeArray[4] = 100000;
    }

    public void SetGameConfig(string _data)
    {
        if (string.IsNullOrEmpty(_data)) throw new GameException(GameException.ErrorCode.FailToGetGameConfig);
        IDictionary dict = Json.Deserialize(_data) as IDictionary;

        /* 버전 체크 */
        Ver = GameProcess.Instance.VersionCode;
        int dataVer = Convert.ToInt32(dict["VER"]);
        if (Ver < dataVer) throw new GameException(GameException.ErrorCode.InvalidConfigVer);

        /* 세팅 데이터 설정 */
        if (dict.Contains("DEFAULT_GOLD")) DefaultGold = Convert.ToInt32(dict["DEFAULT_GOLD"]);
        else throw new GameException(GameException.ErrorCode.FailToGetGameConfig);
        if (dict.Contains("DEFAULT_COIN")) DefaultCoin = Convert.ToInt32(dict["DEFAULT_COIN"]);
        else throw new GameException(GameException.ErrorCode.FailToGetGameConfig);
        if (dict.Contains("COST_ADD_CHA_SLOT")) CostAddChaSlot = Convert.ToInt32(dict["COST_ADD_CHA_SLOT"]);
        else throw new GameException(GameException.ErrorCode.FailToGetGameConfig);
        if (dict.Contains("COST_ADD_ITEM_SLOT")) CostAddItemSlot = Convert.ToInt32(dict["COST_ADD_ITEM_SLOT"]);
        else throw new GameException(GameException.ErrorCode.FailToGetGameConfig);
        if (dict.Contains("COST_ENCHANT_ITEM")) CostEnchantItem = Convert.ToInt32(dict["COST_ENCHANT_ITEM"]);
        else throw new GameException(GameException.ErrorCode.FailToGetGameConfig);
        if (dict.Contains("COST_ENCHANT_CHA")) CostEnchantCha = Convert.ToInt32(dict["COST_ENCHANT_CHA"]);
        else throw new GameException(GameException.ErrorCode.FailToGetGameConfig);
        if (dict.Contains("COST_DEGRADE_CHA")) CostDownGradeCha = Convert.ToInt32(dict["COST_DEGRADE_CHA"]);
        else throw new GameException(GameException.ErrorCode.FailToGetGameConfig);
        if (dict.Contains("DEFAULT_CHA_SLOT")) DefaultChaSlotNum = Convert.ToInt32(dict["DEFAULT_CHA_SLOT"]);
        else throw new GameException(GameException.ErrorCode.FailToGetGameConfig);
        if (dict.Contains("DEFAULT_ITEM_SLOT")) DefaultItemSlotNum = Convert.ToInt32(dict["DEFAULT_ITEM_SLOT"]);
        else throw new GameException(GameException.ErrorCode.FailToGetGameConfig);
        if (dict.Contains("DEFAULT_ASPD")) DefaultASpd = Convert.ToInt32(dict["DEFAULT_ASPD"]);
        else throw new GameException(GameException.ErrorCode.FailToGetGameConfig);
        if (dict.Contains("DEFAULT_SPD")) DefaultSpd = Convert.ToInt32(dict["DEFAULT_SPD"]);
        else throw new GameException(GameException.ErrorCode.FailToGetGameConfig);
        if (dict.Contains("SPD_PER_DEX")) SpdPerDex = Convert.ToInt32(dict["SPD_PER_DEX"]);
        else throw new GameException(GameException.ErrorCode.FailToGetGameConfig);
        if (dict.Contains("DEFAULT_CRI")) DefaultCri = Convert.ToInt32(dict["DEFAULT_CRI"]);
        else throw new GameException(GameException.ErrorCode.FailToGetGameConfig);
        if (dict.Contains("CRI_PER_LUC")) CriPerLuc = Convert.ToInt32(dict["CRI_PER_LUC"]);
        else throw new GameException(GameException.ErrorCode.FailToGetGameConfig);
        if (dict.Contains("MAX_CRITICAL")) MaxCritical = Convert.ToInt32(dict["MAX_CRITICAL"]);
        else throw new GameException(GameException.ErrorCode.FailToGetGameConfig);
        if (dict.Contains("HP_PER_CON")) HpPerCon = Convert.ToInt32(dict["HP_PER_CON"]);
        else throw new GameException(GameException.ErrorCode.FailToGetGameConfig);
        if (dict.Contains("VIP_0")) VipGradeArray[0] = Convert.ToInt32(dict["VIP_0"]);
        else throw new GameException(GameException.ErrorCode.FailToGetGameConfig);
        if (dict.Contains("VIP_1")) VipGradeArray[1] = Convert.ToInt32(dict["VIP_1"]);
        else throw new GameException(GameException.ErrorCode.FailToGetGameConfig);
        if (dict.Contains("VIP_2")) VipGradeArray[2] = Convert.ToInt32(dict["VIP_2"]);
        else throw new GameException(GameException.ErrorCode.FailToGetGameConfig);
        if (dict.Contains("VIP_3")) VipGradeArray[3] = Convert.ToInt32(dict["VIP_3"]);
        else throw new GameException(GameException.ErrorCode.FailToGetGameConfig);
        if (dict.Contains("VIP_4")) VipGradeArray[4] = Convert.ToInt32(dict["VIP_4"]);
        else throw new GameException(GameException.ErrorCode.FailToGetGameConfig);

#if UNITY_EDITOR
        Debug.Log("게임 Config 설정 완료!!!");
#endif

    }
}
