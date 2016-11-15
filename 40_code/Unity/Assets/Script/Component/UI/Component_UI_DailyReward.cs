using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using Kazoh.Table;

public class Component_UI_DailyReward : GameComponent
{
    public UILabel TitleLabel;
    public UILabel CancelBtnLabel;
    public UIGrid RewardGrid;
    public GameObject pfDailyRewardItem;

    private List<Component_Item_DailyReward> list;

    public override void Init()
    {
        base.Init();
        SetUI();
    }

    public void OnClick_Close()
    {
        if (gameObject.activeSelf)
        {
            GameProcess.PlaySound(SOUND_EFFECT.CLICK);
            Hide();
        }
    }

    void SetUI()
    {
        TitleLabel.text = TableManager.GetString("STR_TITLE_DAILY");
        CancelBtnLabel.text = TableManager.GetString("STR_UI_CLOSE");

        /* 리스트 획득 */
        List<TableData_DailyReward> _list = TableManager.GetDailyList();
        int count = GameProcess.GetGameDataManager().UserData.DailyCount;

        /* 리스트 설정 */
        list = new List<Component_Item_DailyReward>();
        foreach (TableData_DailyReward item in _list)
        {
            if (item.Type == TableEnum.MissionType.None) continue;
            GameObject go = NGUITools.AddChild(RewardGrid.gameObject, pfDailyRewardItem);
            Component_Item_DailyReward component = go.GetComponent<Component_Item_DailyReward>();
            if (component != null)
            {
                component.Init(item,count);
                list.Add(component);
            }
        }
        RewardGrid.Reposition();
    }

    public override void Show()
    {
        base.Show();
        int count = GameProcess.GetGameDataManager().UserData.DailyCount;
        Component_Item_DailyReward item = list.Find(x => x.Data.Id == count);
        if (item != null && !item.IsRewarded) item.ShowMark(); 
    }
}
