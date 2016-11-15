using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TableData_Item : TableData
{
    readonly public int Grade;
    readonly public TableEnum.ItemType Type;
    readonly public int SlotIdx;
    readonly public int SAtk;
    readonly public int LAtk;
    readonly public int Def;
    readonly public int Spd;
    readonly public int ASpd;
    readonly public int Hp;
    readonly public TableEnum.EffectType EffectType;
    readonly public int EffectValue;
    readonly public bool IsStack;
    readonly public int Price;
    readonly public int SalePrice;
    readonly public int CoinPrice;
    readonly public string KeyItemName;
    readonly public int KeyAmount;
    readonly public string IconName;
    readonly public bool IsUse;
    readonly public bool IsEquip;
    readonly public bool IsEnchant;
    readonly public bool IsSell;
    readonly public string BulletName;

    public List<string> DropList;
    public List<string> OptionList;
    
    public TableData_Item() : base()
	{
        CoinPrice = int.MaxValue;
        DropList = new List<string>();
        OptionList = new List<string>();
	}

    public TableData_Item(IDictionary dict)
        : base(dict)
    {
        if (dict.Contains("GRADE")) Grade = Convert.ToInt32(dict["GRADE"]);
        if (dict.Contains("TYPE")) Type = (TableEnum.ItemType)Enum.Parse(typeof(TableEnum.ItemType), Convert.ToString(dict["TYPE"]), true);
        if (dict.Contains("SLOT_IDX")) SlotIdx = Convert.ToInt32(dict["SLOT_IDX"]);
        if (dict.Contains("SATK")) SAtk = Convert.ToInt32(dict["SATK"]);
        if (dict.Contains("LATK")) LAtk = Convert.ToInt32(dict["LATK"]);
        if (dict.Contains("DEF")) Def = Convert.ToInt32(dict["DEF"]);
        if (dict.Contains("SPD")) Spd = Convert.ToInt32(dict["SPD"]);
        if (dict.Contains("ASPD")) ASpd = Convert.ToInt32(dict["ASPD"]);
        if (dict.Contains("HP")) Hp = Convert.ToInt32(dict["HP"]);
        if (dict.Contains("EFFECT")) EffectType = (TableEnum.EffectType)Enum.Parse(typeof(TableEnum.EffectType), Convert.ToString(dict["EFFECT"]), true);
        if (dict.Contains("EFFECT_VALUE")) EffectValue = Convert.ToInt32(dict["EFFECT_VALUE"]);
        if (dict.Contains("IS_STACK")) IsStack = Convert.ToBoolean(dict["IS_STACK"]);
        if (dict.Contains("PRICE")) Price = Convert.ToInt32(dict["PRICE"]);
        SalePrice = Mathf.FloorToInt(Price * 0.1f);
        if (dict.Contains("COIN_PRICE")) CoinPrice = Convert.ToInt32(dict["COIN_PRICE"]);
        if (CoinPrice == 0) CoinPrice = int.MaxValue;
        if (dict.Contains("KEY_AMOUNT")) KeyAmount = Convert.ToInt32(dict["KEY_AMOUNT"]);
        if (dict.Contains("KEY")) KeyItemName = Convert.ToString(dict["KEY"]);
        if (dict.Contains("ICON")) IconName = Convert.ToString(dict["ICON"]);
        if (dict.Contains("BULLET")) BulletName = Convert.ToString(dict["BULLET"]);

        IsUse = Type == TableEnum.ItemType.Potion || Type == TableEnum.ItemType.Box;
        IsEquip = Type == TableEnum.ItemType.Equip;
        IsEnchant = Type == TableEnum.ItemType.Equip;
        IsSell = Type != TableEnum.ItemType.Quest;

        DropList = new List<string>();
        IList list = dict["DROP_ARRAY"] as IList;
        foreach (IDictionary dict2 in list)
        {
            if (dict2.Contains("DROP"))
            {
                string dropName = Convert.ToString(dict2["DROP"]);
                if(!string.IsNullOrEmpty(dropName)) DropList.Add(dropName);
            }
        }

        OptionList = new List<string>();
        list = dict["OPTION_ARRAY"] as IList;
        foreach (IDictionary dict2 in list)
        {
            if (dict2.Contains("OPTION"))
            {
                string optionName = Convert.ToString(dict2["OPTION"]);
                if (!string.IsNullOrEmpty(optionName)) OptionList.Add(optionName);
            }
        }
    }
}
