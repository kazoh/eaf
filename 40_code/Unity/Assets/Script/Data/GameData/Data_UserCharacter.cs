using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using Kazoh.Table;

public class Data_UserCharacter
{
    public TableData_Character Data { get; private set; }
    public int EnchantNum { get; private set; }
    public int Grade { get; private set; }

    public int RndPow { get; private set; }
    public int RndDex { get; private set; }
    public int RndInt { get; private set; }
    public int RndCon { get; private set; }
    public int RndLuc { get; private set; }

    public int EnchantPow { get; private set; }
    public int EnchantDex { get; private set; }
    public int EnchantInt { get; private set; }
    public int EnchantCon { get; private set; }
    public int EnchantLuc { get; private set; }

    public int DGradePow { get; private set; }
    public int DGradeDex { get; private set; }
    public int DGradeInt { get; private set; }
    public int DGradeCon { get; private set; }
    public int DGradeLuc { get; private set; }

    public int OptionSAtk { get; private set; }
    public int OptionLAtk { get; private set; }
    public int OptionASpd { get; private set; }
    public int OptionSpd { get; private set; }
    public int OptionDef { get; private set; }
    public int OptionHp { get; private set; }

    public Dictionary<int, Slot_Item> EquipSlot;
    public List<int> OptionList;

    private int curHp;
    public int CurHp
    {
        get
        {
            if (curHp > MaxHp) curHp = MaxHp;
            return curHp;
        }
        set
        {
            if (value > MaxHp) curHp = MaxHp;
            else curHp = value;
            if (curHp < 0) curHp = 0;
        }
    }

    public int DataId { get { return Data == null ? 0 : Data.Id; } }

    public int Pow { get { return Data == null ? 0 : Data.Pow + RndPow + EnchantPow + DGradePow + GetEquipPow(); } }
    public int Dex { get { return Data == null ? 0 : Data.Dex + RndDex + EnchantDex + DGradeDex + GetEquipDex(); } }
    public int Int { get { return Data == null ? 0 : Data.Int + RndInt + EnchantInt + DGradeInt + GetEquipInt(); } }
    public int Con { get { return Data == null ? 0 : Data.Con + RndCon + EnchantCon + DGradeCon + GetEquipCon(); } }
    public int Luc { get { return Data == null ? 0 : Data.Luc + RndLuc + EnchantLuc + DGradeLuc + GetEquipLuc(); } }

    public int SAtk { get { return GetSAtk(); } }
    public int LAtk { get { return GetLAtk(); } }
    public int ASpd { get { return GetASpd(); } }
    public int Spd { get { return GetSpd(); } }
    public int Def { get { return GetDef(); } }
    public int MaxHp { get { return GetMaxHp(); } }
    public int Critical { get { return GetCritical(); } }
    public float CriticalPercent { get { return GetCriticalPercent(); } }

    public string ChaName { get { return Data == null ? "" : TableManager.GetString(Data.Str); } }
    public string ChaDesc { get { return Data == null ? "" : TableManager.GetString(Data.Desc); } }
    public string SpriteName { get { return Data == null ? "" : Data.SpriteName; } }
    public string IconName { get { return Data == null ? "" : Data.IconName; } }
    public bool CanEnchant { get { return EnchantNum < 5; } }
    public bool CanDownGrade { get { return Grade > 1; } }

    public string BulletName
    {
        get
        {
            if( EquipSlot.ContainsKey(1) && 
                EquipSlot[1] != null && 
                EquipSlot[1].Data != null && 
                EquipSlot[1].Data.Data != null && 
                !string.IsNullOrEmpty(EquipSlot[1].Data.Data.BulletName))
            {
                return EquipSlot[1].Data.Data.BulletName;
            }
            else if(Data != null && !string.IsNullOrEmpty(Data.BulletName))
            {
                return Data.BulletName;
            }

            return "pf_bullet_01";            
        }
    }

    public Data_UserCharacter()
    {
        EquipSlot = new Dictionary<int, Slot_Item>();
        for(int i=0; i < 4; ++i)
        {
            EquipSlot.Add(i, null);
        }
        OptionList = new List<int>();
    }

    public Data_UserCharacter(TableData_Character _data) : this()
    {
        Data = _data;
        Grade = _data.Grade;
        RndPow = UnityEngine.Random.Range(0, _data.RndPow);
        RndDex = UnityEngine.Random.Range(0, _data.RndDex);
        RndInt = UnityEngine.Random.Range(0, _data.RndInt);
        RndCon = UnityEngine.Random.Range(0, _data.RndCon);
        RndLuc = UnityEngine.Random.Range(0, _data.RndLuc);
        CurHp = MaxHp;

        if (_data.OptionList != null && _data.OptionList.Count > 0)
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

    public Data_UserCharacter(IDictionary dict) : this()
    {
        int id = 0;
        if (dict.Contains("ID")) id = System.Convert.ToInt32(dict["ID"]);
        if (id == 0) return;

        Data = TableManager.GetGameData(id) as TableData_Character;
        if (dict.Contains("ENCHANT")) EnchantNum = Convert.ToInt32(dict["ENCHANT"]);
        if (dict.Contains("GRADE")) Grade = Convert.ToInt32(dict["GRADE"]);
        if (dict.Contains("EPOW")) EnchantPow = Convert.ToInt32(dict["EPOW"]);
        if (dict.Contains("EDEX")) EnchantDex = Convert.ToInt32(dict["EDEX"]);
        if (dict.Contains("EINT")) EnchantInt = Convert.ToInt32(dict["EINT"]);
        if (dict.Contains("ECON")) EnchantCon = Convert.ToInt32(dict["ECON"]);
        if (dict.Contains("ELUC")) EnchantLuc = Convert.ToInt32(dict["ELUC"]);
        if (dict.Contains("DPOW")) DGradePow = Convert.ToInt32(dict["DPOW"]);
        if (dict.Contains("DDEX")) DGradeDex = Convert.ToInt32(dict["DDEX"]);
        if (dict.Contains("DINT")) DGradeInt = Convert.ToInt32(dict["DINT"]);
        if (dict.Contains("DCON")) DGradeCon = Convert.ToInt32(dict["DCON"]);
        if (dict.Contains("DLUC")) DGradeLuc = Convert.ToInt32(dict["DLUC"]);
        if (dict.Contains("RPOW")) RndPow = Convert.ToInt32(dict["RPOW"]);
        if (dict.Contains("RDEX")) RndDex = Convert.ToInt32(dict["RDEX"]);
        if (dict.Contains("RINT")) RndInt = Convert.ToInt32(dict["RINT"]);
        if (dict.Contains("RCON")) RndCon = Convert.ToInt32(dict["RCON"]);
        if (dict.Contains("RLUC")) RndLuc = Convert.ToInt32(dict["RLUC"]);
        if (dict.Contains("HP")) CurHp = Convert.ToInt32(dict["HP"]);

        IList list = dict["EQUIP_LIST"] as IList;
        if(list != null)
        {
            foreach(int slotIdx in list)
            {
                Slot_Item _item = GameProcess.GetGameDataManager().GetInventory(slotIdx);
                if(_item != null && _item.Data != null)
                {
                    EquipSlot[_item.EquipSlotId] = _item;
                    _item.Equip(true);
                } 
            }
        }

        list = dict["OPTION_LIST"] as IList;
        if (list != null)
        {
            foreach (int optionId in list)
            {
                TableData_Option option = TableManager.GetGameData(optionId) as TableData_Option;
                if(option != null)
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
        hash.Add("ENCHANT", EnchantNum);
        hash.Add("GRADE", Grade);
        hash.Add("EPOW", EnchantPow);
        hash.Add("EDEX", EnchantDex);
        hash.Add("EINT", EnchantInt);
        hash.Add("ECON", EnchantCon);
        hash.Add("ELUC", EnchantLuc);
        hash.Add("DPOW", DGradePow);
        hash.Add("DDEX", DGradeDex);
        hash.Add("DINT", DGradeInt);
        hash.Add("DCON", DGradeCon);
        hash.Add("DLUC", DGradeLuc);
        hash.Add("RPOW", RndPow);
        hash.Add("RDEX", RndDex);
        hash.Add("RINT", RndInt);
        hash.Add("RCON", RndCon);
        hash.Add("RLUC", RndLuc);
        hash.Add("HP", CurHp);

        ArrayList list = new ArrayList();
        List<Slot_Item> inven = GameProcess.GetGameDataManager().GetInventory();
        foreach (KeyValuePair<int,Slot_Item> equip in EquipSlot)
        {
            if(equip.Value != null && inven.Count > 0 && inven.IndexOf(equip.Value) > -1)
            {
                list.Add(inven.IndexOf(equip.Value));
            }
        }
        hash.Add("EQUIP_LIST", list);

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

    public string Enchant()
    {
        int _pow = UnityEngine.Random.Range(0, 100) % 2 == 0 ? 0 : 1;
        int _dex = UnityEngine.Random.Range(0, 100) % 2 == 0 ? 0 : 1;
        int _int = UnityEngine.Random.Range(0, 100) % 2 == 0 ? 0 : 1;
        int _con = UnityEngine.Random.Range(0, 100) % 2 == 0 ? 0 : 1;
        int _luc = UnityEngine.Random.Range(0, 100) % 2 == 0 ? 0 : 1;
        EnchantPow += _pow;
        EnchantDex += _dex;
        EnchantInt += _int;
        EnchantCon += _con;
        EnchantLuc += _luc;
        EnchantNum++;

        string str = string.Format("{0} +{1} {2} +{3} {4} +{5}\n{6} +{7} {8} +{9}", 
                                    TableManager.GetString("STR_UI_POW"), _pow, 
                                    TableManager.GetString("STR_UI_DEX"), _dex, 
                                    TableManager.GetString("STR_UI_INT"), _int, 
                                    TableManager.GetString("STR_UI_CON"), _con, 
                                    TableManager.GetString("STR_UI_LUC"), _luc);
        return str;
    }

    public string DownGrade()
    {
        string str = "";
        int dice = UnityEngine.Random.Range(0, 100) % 5;
        switch(dice)
        {
            case 0: DGradePow -= 5; str = string.Format("{0} -{1}", TableManager.GetString("STR_UI_POW"), 5); break;
            case 1: DGradeDex -= 5; str = string.Format("{0} -{1}", TableManager.GetString("STR_UI_DEX"), 5); break;
            case 2: DGradeInt -= 5; str = string.Format("{0} -{1}", TableManager.GetString("STR_UI_INT"), 5); break;
            case 3: DGradeCon -= 5; str = string.Format("{0} -{1}", TableManager.GetString("STR_UI_CON"), 5); break;
            default: DGradeLuc -= 5; str = string.Format("{0} -{1}", TableManager.GetString("STR_UI_LUC"), 5); break;
        }
        Grade--;

        return str;
    }

#if UNITY_EDITOR
    public void SetParamForTest(int satk, int latk, int aspd, int spd, int def, int hp)
    {
        OptionSAtk = satk;
        OptionLAtk = latk;
        OptionASpd = aspd;
        OptionSpd = spd;
        OptionDef = def;
        OptionHp = hp;
        CurHp = MaxHp;
    }
#endif

    int GetSAtk()
    {
        /* 장비 공격력 * ((Pow - 9) / 3 + 1) + 추가 공격력 */
        int value = 0;
        int equip = 0;
        int added = 0;
        if(Data != null)
        {
            added = OptionSAtk;
            for (int i = 0; i < EquipSlot.Count; ++i)
            {
                if (EquipSlot[i] != null && EquipSlot[i].IsEquipped)
                {
                    equip += EquipSlot[i].Data.SAtk;
                    added += EquipSlot[i].Data.OptionSAtk;
                }
            }

            equip = Mathf.Max(1, equip);
            value = equip * ((Pow - 9) / 3 + 1) + added;
        }
        return value;
    }

    int GetLAtk()
    {
        /* 장비 공격력 * ((Int - 9) / 3 + 1) + 추가 공격력 */
        int value = 0;
        int equip = 0;
        int added = 0;
        if (Data != null)
        {
            added = OptionLAtk;
            for (int i = 0; i < EquipSlot.Count; ++i)
            {
                if (EquipSlot[i] != null && EquipSlot[i].IsEquipped)
                {
                    equip += EquipSlot[i].Data.LAtk;
                    added += EquipSlot[i].Data.OptionLAtk;
                }
            }

            equip = Mathf.Max(1, equip);
            value = equip * ((Int - 9) / 3 + 1) + added;
        }
        return value;
    }

    int GetASpd()
    {
        /* 장비 공속 * ((100 - Dex) * 0.01) - 추가 공속 */
        int value = 0;
        int equip = GameProcess.GetGameConfig().DefaultASpd;
        int added = 0;
        if (Data != null)
        {
            added = OptionASpd;
            for (int i = 0; i < EquipSlot.Count; ++i)
            {
                if (EquipSlot[i] != null && EquipSlot[i].IsEquipped)
                {
                    if(EquipSlot[i].Data.Data.SlotIdx == 1) equip = EquipSlot[i].Data.ASpd;
                    added += EquipSlot[i].Data.OptionASpd;
                }
            }

            value = Mathf.FloorToInt(equip * Mathf.Max((100 - Dex) * 0.01f, 0.1f)) - added;
        }
        return value;
    }

    int GetSpd()
    {
        /* 기본 이속 + 장비 이속 + Dex가중치 + 추가 이속 */
        int value = 0;
        int equip = 0;
        int added = 0;
        if (Data != null)
        {
            added = OptionSpd;
            for (int i = 0; i < EquipSlot.Count; ++i)
            {
                if (EquipSlot[i] != null && EquipSlot[i].IsEquipped)
                {
                    equip += EquipSlot[i].Data.Spd;
                    added += EquipSlot[i].Data.OptionSpd;
                }
            }

            value = GameProcess.GetGameConfig().DefaultSpd + (Dex / 4) * GameProcess.GetGameConfig().SpdPerDex + equip + added;
        }
        return value;
    }

    int GetDef()
    {
        /* 장비 방어력 + 추가 방어력 */
        int value = 0;
        int equip = 0;
        int added = 0;
        if (Data != null)
        {
            added = OptionDef;
            for (int i = 0; i < EquipSlot.Count; ++i)
            {
                if (EquipSlot[i] != null && EquipSlot[i].IsEquipped)
                {
                    equip += EquipSlot[i].Data.Def;
                    added += EquipSlot[i].Data.OptionDef;
                }
            }

            value = equip + added;

        }
        return value;
    }

    int GetMaxHp()
    {
        /* 10 * Con + 추가 Hp */
        int value = 0;
        int equip = 0;
        int added = 0;
        if (Data != null)
        {
            added = OptionHp;
            for (int i = 0; i < EquipSlot.Count; ++i)
            {
                if (EquipSlot[i] != null && EquipSlot[i].IsEquipped)
                {
                    equip += EquipSlot[i].Data.Hp;
                    added += EquipSlot[i].Data.OptionHp;
                }
            }

            value = GameProcess.GetGameConfig().HpPerCon * Con + equip + added;
        }
        return value;
    }

    int GetCritical()
    {
        /* 기본 크리티컬 + Luc 가중치 + 추가 크리티컬 */
        int value = 0;
        int added = 0;
        if (Data != null)
        {
            value = GameProcess.GetGameConfig().DefaultCri + (Luc / 3) * 1 + added;
        }
        return Mathf.Min(GameProcess.GetGameConfig().MaxCritical, value);
    }

    float GetCriticalPercent()
    {
        int value = GameProcess.GetGameConfig().DefaultCriPercent;
        return value * 0.01f;
    }

    int GetEquipPow()
    {
        int value = 0;
        for(int i=0; i<EquipSlot.Count; ++i)
        {
            if(EquipSlot[i] != null && EquipSlot[i].IsEquipped) value += EquipSlot[i].Data.Pow;
        }

        return value;
    }

    int GetEquipDex()
    {
        int value = 0;
        for (int i = 0; i < EquipSlot.Count; ++i)
        {
            if (EquipSlot[i] != null && EquipSlot[i].IsEquipped) value += EquipSlot[i].Data.Dex;
        }

        return value;
    }

    int GetEquipInt()
    {
        int value = 0;
        for (int i = 0; i < EquipSlot.Count; ++i)
        {
            if (EquipSlot[i] != null && EquipSlot[i].IsEquipped) value += EquipSlot[i].Data.Int;
        }

        return value;
    }

    int GetEquipCon()
    {
        int value = 0;
        for (int i = 0; i < EquipSlot.Count; ++i)
        {
            if (EquipSlot[i] != null && EquipSlot[i].IsEquipped) value += EquipSlot[i].Data.Con;
        }

        return value;
    }

    int GetEquipLuc()
    {
        int value = 0;
        for (int i = 0; i < EquipSlot.Count; ++i)
        {
            if (EquipSlot[i] != null && EquipSlot[i].IsEquipped) value += EquipSlot[i].Data.Luc;
        }

        return value;
    }

    void SetOptionEffect(TableData_Option option)
    {
        if(option != null)
        {
            switch(option.Type)
            {
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
}
