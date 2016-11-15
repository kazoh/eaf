using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using Kazoh.Table;

public class Data_UserItem
{
    public Action ChangedEvent;

    public TableData_Item Data { get; private set; }
    public int Num { get; private set; }
    public int EnchantNum { get; private set; }
    public bool IsEquipped { get; private set; }

    public List<int> OptionList;

    public int Pow { get; private set; }
    public int Dex { get; private set; }
    public int Int { get; private set; }
    public int Con { get; private set; }
    public int Luc { get; private set; }

    public int OptionSAtk { get; private set; }
    public int OptionLAtk { get; private set; }
    public int OptionASpd { get; private set; }
    public int OptionSpd { get; private set; }
    public int OptionDef { get; private set; }
    public int OptionHp { get; private set; }

    public int DataId { get { return Data == null ? 0 : Data.Id; }}
    public string ItemName { get { return Data == null ? "" : TableManager.GetString(Data.Str); } }
    public string ItemDesc { get { return Data == null ? "" : TableManager.GetString(Data.Desc); } }
    public string IconName { get { return Data == null ? "" : Data.IconName; } }
    public bool CanEnchant { get { return Data == null || EnchantNum == 99 ? false : Data.IsEnchant; } }
    public bool CanSell { get { return Data == null ? false : Data.IsSell; } }
    public bool CanUse { get { return Data == null ? false : Data.IsUse; } }
    public bool CanEquip { get { return Data == null ? false : Data.IsEquip; } }
    public bool CanStack { get { return Data == null ? false : Data.IsStack; } }

    public int SAtk { get { return Data == null ? 0 : Data.SAtk + (Data.SlotIdx == 1 ? EnchantNum : 0); } }
    public int LAtk { get { return Data == null ? 0 : Data.LAtk + (Data.SlotIdx == 1 ? EnchantNum : 0); } }
    public int ASpd { get { return Data == null ? 0 : Data.ASpd + (Data.SlotIdx == 0 ? EnchantNum : 0); } }
    public int Spd { get { return Data == null ? 0 : Data.Spd + (Data.SlotIdx == 3 ? EnchantNum : 0); } }
    public int Def { get { return Data == null ? 0 : Data.Def + (Data.SlotIdx == 2 || Data.SlotIdx == 3 ? EnchantNum : 0); } }
    public int Hp { get { return Data == null ? 0 : Data.Hp; } }

    public Data_UserItem()
    {
        Num = 0;
        EnchantNum = 0;
        OptionList = new List<int>();
    }

    public Data_UserItem(TableData_Item _data, int num) : this()
    {
        Data = _data;
        Num = num;

        if(_data.OptionList != null && _data.OptionList.Count > 0)
        {
            for (int i = 0; i < 1; ++i)
            {
                int dice = UnityEngine.Random.Range(0, _data.OptionList.Count);
                string optionName = _data.OptionList[dice];
                TableData_Option option = TableManager.GetGameData(optionName) as TableData_Option;
                OptionList.Add(option.Id);
                SetOptionEffect(option);
            }
        }
    }

    public Data_UserItem(IDictionary dict) : this()
    {
        int id = 0;
        if (dict.Contains("ID")) id = System.Convert.ToInt32(dict["ID"]);
        Data = TableManager.GetGameData(id) as TableData_Item;
        if (Data == null) return;

        if (dict.Contains("NUM")) Num = System.Convert.ToInt32(dict["NUM"]);
        if (dict.Contains("ENCHANT")) EnchantNum = System.Convert.ToInt32(dict["ENCHANT"]);

        IList list = dict["OPTION_LIST"] as IList;
        if (list != null)
        {
            foreach (int optionId in list)
            {
                TableData_Option option = TableManager.GetGameData(optionId) as TableData_Option;
                if (option != null)
                {
                    OptionList.Add(option.Id);
                    SetOptionEffect(option);
                }
            }
        }
    }

    public Dictionary<string, object> GetHash()
    {
        Dictionary<string, object> hash = new Dictionary<string, object>();
        hash.Add("ID", DataId);
        hash.Add("NUM", Num);
        hash.Add("ENCHANT", EnchantNum);

        ArrayList options = new ArrayList();
        foreach (int optionId in OptionList)
        {
            if (optionId > 0)
            {
                options.Add(optionId);
            }
        }
        hash.Add("OPTION_LIST", options);

        return hash;
    }

    public void Add(int num)
    {
        Num += num;
    }

    public void Equip(bool isEquip)
    {
        IsEquipped = isEquip;
        OnChanged();
    }

    public void Enchant(int value)
    {
        EnchantNum+=value;
        if (EnchantNum > 99) EnchantNum = 99;
        OnChanged();
    }

    void SetOptionEffect(TableData_Option option)
    {
        if (option != null)
        {
            switch (option.Type)
            {
                case TableEnum.OptionType.POW:
                    Pow += option.Value;
                    break;

                case TableEnum.OptionType.DEX:
                    Dex += option.Value;
                    break;

                case TableEnum.OptionType.INT:
                    Int += option.Value;
                    break;

                case TableEnum.OptionType.CON:
                    Con += option.Value;
                    break;

                case TableEnum.OptionType.LUC:
                    Luc += option.Value;
                    break;

                case TableEnum.OptionType.SATK:
                    OptionSAtk += option.Value;
                    break;

                case TableEnum.OptionType.LATK:
                    OptionLAtk += option.Value;
                    break;

                case TableEnum.OptionType.DEF:
                    OptionDef += option.Value;
                    break;

                case TableEnum.OptionType.ASPD:
                    OptionASpd += option.Value;
                    break;

                case TableEnum.OptionType.SPD:
                    OptionSpd += option.Value;
                    break;

                case TableEnum.OptionType.HP:
                    OptionHp += option.Value;
                    break;
            }
        }
    }

    void OnChanged()
    {
        if (ChangedEvent != null) ChangedEvent();
    }
}
