using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Kazoh.Table;

public class Data_Rank
{
    public int Id { get; private set; }
    public int ChaId { get; private set; }
    public string Name { get; private set; }
    public int Record { get; private set; }

    public Data_Rank()
    {
        Id = 0;
        Record = 0;
        ChaId = 0;
        Name = "";
    }

    public Data_Rank(int _id, int _chaId, string _name, int _record) : this()
    {
        Id = _id;
        Record = _record;
        ChaId = _chaId;
        Name = _name;
    }

    public Data_Rank(IDictionary dict) : this()
    {
        if (dict.Contains("ID")) Id = System.Convert.ToInt32(dict["ID"]);
        if (dict.Contains("SCORE")) Record = System.Convert.ToInt32(dict["SCORE"]);
        if (dict.Contains("CHA_ID")) ChaId = System.Convert.ToInt32(dict["CHA_ID"]);
        if (dict.Contains("NAME")) Name = System.Convert.ToString(dict["NAME"]);
    }
}
