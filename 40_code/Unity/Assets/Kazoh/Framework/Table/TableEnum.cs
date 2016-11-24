using UnityEngine;
using System.Collections;


public class TableEnum
{
    public enum ItemType
    {
        None = 0,
        Equip,
        Potion,
        Box,
        Key,
        Quest,
        Meterial,
    }

    public enum EffectType
    {
        None = 0,
        ADD_HP,
        ADD_CHARACTER_SLOT,
        ADD_ITEM_SLOT,
        ADD_ITEM,
        SUMMON,
    }

    public enum MoneyType
    {
        None = 0,
        Gold,
        Coin,
        Key,
        Cash,
    }

    public enum SaleCatecory
    {
        None = 0,
        Equip,
        Potion,
        Box,
        Ect,
    }

    public enum UnlockType
    {
        None = 0,
        GCrownMapId,
        SCrownMapId,
        BCrownMapId,
        GCrownNum,
        SCrownNum,
        BCrownNum,
    }

    public enum MapType
    {
        None = 0,
        Move,
        Talk,
        SlowTalk,
        Monster,
        Reward,
        QuestTalk,
        QuestMob,
    }

    public enum GoodsType
    {
        None = 0,
        Cash,
        CoinToKey,
        CashToGold,
        CashToCoin,
        Summon,
    }

    public enum NpcType
    {
        None = 0,
        Npc,
        Monster,
        Object,
    }

    public enum OptionType
    {
        NONE = 0,
        POW,
        DEX,
        INT,
        CON,
        LUC,
        SATK,
        LATK,
        DEF,
        ASPD,
        SPD,
        HP,
    }

    public enum MissionType
    {
        None = 0,
        Gold,
        Coin,
        Item,
    }

    public enum MapClearGrade
    {
        None = 0,
        Bronze,
        Silver,
        Gold,
    }

    #region // old
    public enum SchoolType
    {
        None = 0,
        Private,
        Public,
        National,
        Commercial,
        Technical,
        Art,
        Science,
        Specialized,
        Agricultural,
        Language,
        International,
        Athlete,
    }

    public enum ConfigType
    {
        None,
        GameSetting,
        Balance,
        AI,
    }

    public enum OptionCategory
    {
        None = 0,
        Human,
        Structure,
    }

    public enum JoinCondType
    {
        None,
        Power,
        Wisdom,
        Charm,
        Gold,
        Member,
        ExceptMember,
        Strategy,
        Month,
        Cafeteria,
    }

    public enum MemberType
    {
        Power = 0,
        Wisdom,
        Charm,
    }

    public enum SexType
    {
        Both = 0,
        Male,
        Female,
        None,
    }

    public enum ModeType
    {
        NONE = 0,
        EASY,
        NORMAL,
        DIFFICULTY,
    }

    public enum CheetType
    {
        NONE = 0,
        ADD_GOLD,
        EDIT_PARAM,
    }

    #endregion // old
}
