using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TableData_Npc : TableData
{
    readonly public TableEnum.NpcType Type;
    readonly public int SAtk;
    readonly public int LAtk;
    readonly public int Def;
    readonly public int ASpd;
    readonly public int Spd;
    readonly public int Hp;
    readonly public string SpriteName;
    readonly public string IconName;

    public List<string> DropList;
    public List<string> DialogList;

    public TableData_Npc() : base()
    {
        DropList = new List<string>();
        DialogList = new List<string>();
    }

    public TableData_Npc(IDictionary dict)
        : base(dict)
    {
        if (dict.Contains("TYPE")) Type = (TableEnum.NpcType)Enum.Parse(typeof(TableEnum.NpcType), Convert.ToString(dict["TYPE"]), true);
        if (dict.Contains("SATK")) SAtk = Convert.ToInt32(dict["SATK"]);
        if (dict.Contains("LATK")) LAtk = Convert.ToInt32(dict["LATK"]);
        if (dict.Contains("DEF")) Def = Convert.ToInt32(dict["DEF"]);
        if (dict.Contains("ASPD")) ASpd = Convert.ToInt32(dict["ASPD"]);
        if (dict.Contains("SPD")) Spd = Convert.ToInt32(dict["SPD"]);
        if (dict.Contains("HP")) Hp = Convert.ToInt32(dict["HP"]);
        if (dict.Contains("MODEL")) SpriteName = Convert.ToString(dict["MODEL"]);
        if (dict.Contains("ICON")) IconName = Convert.ToString(dict["ICON"]);

        DropList = new List<string>();
        IList list = dict["DROP_ARRAY"] as IList;
        foreach (IDictionary dict2 in list)
        {
            if (dict2.Contains("DROP"))
            {
                string dropName = Convert.ToString(dict2["DROP"]);
                if (!string.IsNullOrEmpty(dropName)) DropList.Add(dropName);
            }
        }

        DialogList = new List<string>();
        list = dict["DIALOG_ARRAY"] as IList;
        foreach (IDictionary dict2 in list)
        {
            if (dict2.Contains("DIALOG"))
            {
                string story = Convert.ToString(dict2["DIALOG"]);
                if (!string.IsNullOrEmpty(story)) DialogList.Add(story);
            }
        }
    }
}
