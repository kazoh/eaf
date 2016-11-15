using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Kazoh.Table;

public class Manager_Mission
{
    private List<TableData_Mission> missionList;
    private Stack<TableData_Mission> missionStack;

    public void Init()
    {
        missionStack = new Stack<TableData_Mission>();

        Debug.Log("[Manager_Mission] 미션 매니저 초기화!!!");
    }

    public void Load()
    {
        missionList = TableManager.GetAllMission();
        List<int> completedMissionList = GameProcess.GetGameDataManager().GetCompletedMissionList();
        foreach (int id in completedMissionList)
        {
            TableData_Mission _mission = missionList.Find(x => x.Id == id);
            if (_mission != null) missionList.Remove(_mission);
        }
    }

    public void CompleteMission(int _mapId, TableEnum.MapClearGrade _grade)
    {
        TableData_Mission _mission = missionList.Find(x => x.MapId == _mapId && x.MapClearGrade <= _grade);
        if (_mission != null) missionStack.Push(_mission);
    }

    public TableData_Mission GetCompletedMission()
    {
        if (missionStack == null || missionStack.Count == 0) return null;
        TableData_Mission _mission = missionStack.Pop();
        missionList.Remove(_mission);
        return _mission;
    }
}