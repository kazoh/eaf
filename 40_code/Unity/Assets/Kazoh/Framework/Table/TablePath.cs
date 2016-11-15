using UnityEngine;
using System.Collections;

/// <summary>
/// 테이블 데이터의 경로와 키 정보를 관리하는 클래스.
/// </summary>
public class TablePath {

    public enum DataTable
    {
        Table_Data_Character,
        Table_Data_Npc,
        Table_Data_Item,
        Table_Data_Option,
        Table_Data_Map,
        Table_Data_Drop,
        Table_Data_Mission,

        //테이블 추가가 필요하면 아래에 추가한다.
    }

    public enum StringTable
    {
        Table_STR_String,
    }

    public enum ListTable
    {
        Table_List_Sale,
        Table_List_Map,
        Table_List_Goods,
        Table_List_DailyReward,
        Table_List_Region,

        //리스트 추가가 필요하면 아래에 추가한다.
    }

    public enum ConfigTable
    {
        Table_Setting_Balance,
        Table_Setting_AI,
    }

    const string dirPath = "Data/";

    public static string GetPath(DataTable table)
    {
        return dirPath + table.ToString();
    }

    public static string GetPath(ListTable table)
    {
        return dirPath + table.ToString();
    }

    public static string GetPath(StringTable table)
    {
        return dirPath + table.ToString();
    }

    public static string GetPath(ConfigTable table)
    {
        return dirPath + table.ToString();
    }
}
