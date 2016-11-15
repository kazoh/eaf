using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Kazoh.Table;

public class Data_UserMap
{
    public int DataId { get; private set; }
    public int Record { get; private set; }
    public bool IsNew { get; private set; }

    public Data_UserMap()
    {
        DataId = 0;
        Record = 0;
        IsNew = false;
    }

    public Data_UserMap(int _id, int _record, bool _new) : this()
    {
        DataId = _id;
        Record = _record;
        IsNew = _new;
    }

    public Data_UserMap(IDictionary dict) : this()
    {
        if (dict.Contains("ID")) DataId = System.Convert.ToInt32(dict["ID"]);
        if (dict.Contains("RECORD")) Record = System.Convert.ToInt32(dict["RECORD"]);
    }

    public Dictionary<string, object> GetHash()
    {
        Dictionary<string, object> hash = new Dictionary<string, object>();
        hash.Add("ID", DataId);
        hash.Add("RECORD", Record);

        return hash;
    }

    public void SetNew()
    {
        IsNew = false;
    }
}
