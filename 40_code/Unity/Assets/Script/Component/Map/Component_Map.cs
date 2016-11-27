using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Kazoh.Table;

public class Component_Map : GameComponent
{
    public Action<Component_Map> ReadyEvent;
    public Action<Component_Map> FinishedEvent;

    public UITexture BgTexture;
    public UIGrid CellGrid;
    public UIPanel ObjectPanel;
    public UIPanel BottomPanel;
    public UIPanel TopPanel;
    public int PlayTime = 10;
    public List<Component_Map_Cell> CellList;
    public List<Component_Map_Object> ObjList;
    //public List<Component_Map_Bullet> PlayerBulletList;

    public int ColNum { get; private set; }
    public Component_Map_Player Player { get; protected set; }
    public Component_Map_Timer Timer { get; protected set; }

    public TableData_Map Data { get; private set; }
    public Data_UserCharacter CharacterData { get; private set; }
    public Data_Reward RewardInfo { get; protected set; }
    public bool IsGameFinish { get; protected set; }
    public bool IsClear { get; protected set; }
    public int Crown { get; protected set; }

    public bool IsGameStart { get; protected set; }

    private List<IManagedDepth> listObjWithManagedDepth;

#if UNITY_EDITOR
    public bool TestMode;
    public int MapId;
    public int CharacterId;
    public int SAtk;
    public int LAtk;
    public int ASpd;
    public int Spd;
    public int Def;
    public int Hp;
#endif

    void Awake()
    {
#if UNITY_EDITOR
        if (TestMode)
        {
            try
            {
                GameProcess.LoadTable();
                TableData_Map _data = TableManager.GetGameData(MapId) as TableData_Map;
                TableData_Character _cha = TableManager.GetGameData(CharacterId) as TableData_Character;
                Data_UserCharacter _player = new Data_UserCharacter(_cha);
                _player.SetParamForTest(SAtk, LAtk, ASpd, Spd, Def, Hp);
                Init(_data, _player);
            }
            catch(GameException e)
            {
                GameProcess.ShowError(e);
            }
            catch(Exception e)
            {
                GameProcess.ShowError(new GameException(GameException.ErrorCode.Unknown, e.Message));
            }
        }
#endif
    }

    public virtual void Init(TableData_Map _data, Data_UserCharacter _cha)
    {
        base.Init();

        if (_data == null) throw new GameException(GameException.ErrorCode.NoGameData);
        if (_cha == null) throw new GameException(GameException.ErrorCode.NoGameData);
        Data = _data;
        CharacterData = _cha;
        RewardInfo = new Data_Reward();
        SetCellList();
        SetPlayer();
        SetObjList();
        SetObject();
        SetTimer();
        SetObjListWithManagedDepth();
        OnReady();
    }

    public Component_Map_Cell GetStartPoint()
    {
        return CellList.Find(x => x.Type == GameEnum.CellType.StartPoint);
    }

    public Component_Map_Cell GetCell(int idx)
    {
        if (idx < CellList.Count) return CellList[idx];
        return null;
    }

    public List<Component_Map_Cell> GetPath(Component_Map_Cell from, Component_Map_Cell to)
    {
        int r = to.Row - from.Row;
        int c = to.Col - from.Col;

        if (Mathf.Abs(r) > Mathf.Abs(c)) return GetCol(from, to);
        return GetRow(from, to);
    }

    List<Component_Map_Cell> GetRow(Component_Map_Cell from, Component_Map_Cell to)
    {
        List<Component_Map_Cell> list = new List<Component_Map_Cell>();
        int s = from.Idx;
        int e = from.Row * ColNum + to.Col;
        if (from.Col < to.Col)
        {
            while (s < e)
            {
                s++;
                if (CellList[s].IsBlock) break;
                list.Add(CellList[s]);
            }
        }
        else if (from.Col > to.Col)
        {
            while (s > e)
            {
                s--;
                if (CellList[s].IsBlock) break;
                list.Add(CellList[s]);
            }
        }
        return list;
    }

    List<Component_Map_Cell> GetCol(Component_Map_Cell from, Component_Map_Cell to)
    {
        List<Component_Map_Cell> list = new List<Component_Map_Cell>();
        int s = from.Idx;
        int e = to.Row * ColNum + to.Col;
        if(from.Row < to.Row)
        {
            while(s < e)
            {
                s+=ColNum;
                if (CellList[s].IsBlock) break;
                list.Add(CellList[s]);
            }
        } 
        else if(from.Row > to.Row)
        {
            while (s > e)
            {
                s-= ColNum;
                if (CellList[s].IsBlock) break;
                list.Add(CellList[s]);
            }
        }
        return list;
    }

    public virtual void GameStart()
    {
        IsGameStart = true;
        Timer.Pause(false);
    }

    protected virtual void OnReady()
    {
        if (ReadyEvent != null) ReadyEvent(this);
#if UNITY_EDITOR
        if (TestMode) GameStart();
#endif
    }

    void SetClearReward()
    {
        List<TableData_Drop> _list = new List<TableData_Drop>();
        for (int i = 0; i < Data.DropList.Count; ++i)
        {
            _list.Add(TableManager.GetGameData(Data.DropList[i]) as TableData_Drop);
        }
        if (Crown > 1 && !string.IsNullOrEmpty(Data.AddDrop_SCrown))
        {
            _list.Add(TableManager.GetGameData(Data.AddDrop_SCrown) as TableData_Drop);
        }
        if (Crown > 2 && !string.IsNullOrEmpty(Data.AddDrop_GCrown))
        {
            _list.Add(TableManager.GetGameData(Data.AddDrop_GCrown) as TableData_Drop);
        }

        AddReward(Data.Reward, 0, Data_Reward.DoDrop(_list));
    }

    void AddReward(int _gold, int _coin, Data_Reward _reward)
    {
        RewardInfo.SetReward(_gold, _coin, _reward);
    }

    void SetCellList()
    {
        ColNum = CellGrid.maxPerLine;
        CellList = new List<Component_Map_Cell>();
        int idx = 0;
        foreach(Transform c in CellGrid.GetChildList())
        {
            Component_Map_Cell cell = c.GetComponent<Component_Map_Cell>();
            if (cell == null) continue;
            cell.Init(idx, ColNum == 0 ? 1 : ColNum);
            cell.SelectEvent += OnSelectCell;
            CellList.Add(cell);
            idx++;
        }
    }

    void SetPlayer()
    {
        Player = ObjectPanel.GetComponentInChildren<Component_Map_Player>();
        if (Player == null) throw new GameException(GameException.ErrorCode.NoPlayer);
        Player.Init(this);
        Player.DieEvent += OnGameOver;
    }

    void SetObjList()
    {
        ObjList = new List<Component_Map_Object>();
        ObjList.AddRange(ObjectPanel.GetComponentsInChildren<Component_Map_Object>());
        ObjList.AddRange(BottomPanel.GetComponentsInChildren<Component_Map_Object>());
        foreach (Component_Map_Object c in ObjList)
        {
            c.Init(this);
        }
    }

    /// <summary>
    /// 공용으로 쓰이는 오브젝트의 이벤트를 설정한다.
    /// </summary>
    void SetObject()
    {
        foreach (Component_Map_Object c in ObjList)
        {
            if (c is Component_Object_Box)
            {
                (c as Component_Object_Box).OpenEvent += OnReward;
            }
            else if (c is Component_Object_RewardArea)
            {
                (c as Component_Object_RewardArea).EnterAreaEvent += OnReward;
            }
            else if (c is Component_Map_Monster)
            {
                Component_Map_Monster mob = c as Component_Map_Monster;
                mob.DropEvent += OnReward;
            }
        }
    }

    void SetObjListWithManagedDepth()
    {
        listObjWithManagedDepth = new List<IManagedDepth>();
        if (Player is IManagedDepth) listObjWithManagedDepth.Add(Player as IManagedDepth);
        Component_Map_Object[] objArray = ObjectPanel.GetComponentsInChildren<Component_Map_Object>();
        if (objArray == null) return;
        for(int i=0; i < objArray.Length; ++i)
        {
            if(objArray[i] is IManagedDepth) listObjWithManagedDepth.Add(objArray[i] as IManagedDepth);
        }

        if (listObjWithManagedDepth.Count > 1) StartCoroutine(ManageObjectDepth());
    }

    IEnumerator ManageObjectDepth()
    {
        List<IManagedDepth> list = new List<IManagedDepth>();
        list.AddRange(listObjWithManagedDepth);
        while (true)
        {
            list.Sort((x, y) => y.GetPosY().CompareTo(x.GetPosY()));
            for (int i = 0; i < list.Count; ++i)
            {
                list[i].SetDepth(i * 5 + 5);
            }
            yield return new WaitForSeconds(0.2f);
        }
    }

    protected virtual void SetTimer()
    {
        Timer = TopPanel.GetComponentInChildren<Component_Map_Timer>();
        if (Timer == null) throw new GameException(GameException.ErrorCode.NoTimer);
        Timer.Init(this, PlayTime);
        Timer.TimeOverEvent += OnGameOver;
    }

    protected virtual int GetCrown()
    {
        float t = Timer.TotTime - Timer.CurTime;
        if (t >= Data.Time_GCrown) return 3;
        if (t >= Data.Time_SCrown) return 2;
        return 1;
    }

    #region // Callback

    protected void OnGameClear()
    {
        if (IsGameFinish) return;
        IsGameFinish = true;
        IsClear = true;
        Timer.Stop();
        Crown = GetCrown();
        SetClearReward();
        if (FinishedEvent != null) FinishedEvent(this);
    }

    protected void OnGameOver()
    {
        if (IsGameFinish) return;
        Timer.Stop();
        IsGameFinish = true;
        IsClear = false;
        Crown = 0;
        if (FinishedEvent != null) FinishedEvent(this);
    }

    protected virtual void OnSelectCell(Component_Map_Cell _cell)
    {
        if (!IsGameStart) return;
        Player.SetPath(_cell);
    }

    /// <summary>
    /// 보상을 해야 할 때 사용함.
    /// </summary>
    /// <param name="_gold">골드</param>
    /// <param name="_coin">코인</param>
    /// <param name="_list">드롭리스트</param>
    protected virtual void OnReward(int _gold, int _coin, Data_Reward _reward)
    {
        AddReward(_gold, _coin, _reward);
    }
    #endregion // Callback

}
