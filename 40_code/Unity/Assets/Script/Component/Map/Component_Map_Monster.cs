using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using Kazoh.Table;

public class Component_Map_Monster : Component_Map_Object, IAttackable
{
    public enum MonsterState
    {
        None,
        Spawn,
        Wait,
        Move,
        Attack,
        Attacked,
        Die,
        End,
    }

    public Action<Component_Map_Monster> DieEvent;
    public Action<int, int, Data_Reward> DropEvent;

    public int NpcId;
    public int Range = 32;
    public bool HasShadow;
    public Component_Map_Cell SpawnCell;
    public UISprite NpcSprite;
    public UISprite HpBar;
    public List<UILabel> DmgLabelList;
    public List<string> DialogList;
    public GameObject PfCoinEffect;
    public GameObject PfItemEffect;

    public TableData_Npc Data { get; protected set; }
    public float Speed { get; private set; }

    private Component_Map_Cell curCell;
    private Component_Map_Cell preCell;
    private GameEnum.Direction curDir;

    private float delay = float.MinValue;
    private float attackDelay = float.MinValue;
    private int hp;
    private int spriteNum;

    private MonsterState state;
    public MonsterState State
    {
        get { return state; }
        private set
        {
            if (state == value) return;
#if DEBUG_MODE
            Debug.Log(string.Format("몬스터 상태 변경 : {0} -> {1}", state, value));
#endif
            state = value;
            switch (state)
            {
                case MonsterState.None:
                    break;

                case MonsterState.Spawn:
                    Spawn();
                    break;

                case MonsterState.Wait:
                    Wait();
                    break;

                case MonsterState.Move:
                    break;

                case MonsterState.Attack:
                    break;

                case MonsterState.Attacked:
                    break;

                case MonsterState.Die:
                    Die();
                    break;

                case MonsterState.End:
                    End();
                    break;
            }
        }
    }

    private string spriteFormat;

    public override void Init()
    {
        base.Init();
        Data = TableManager.GetGameData(NpcId) as TableData_Npc;
        if (Data == null) throw new GameException(GameException.ErrorCode.NoGameData);
        Name = TableManager.GetString(Data.Str);
        hp = Data.Hp;
        Speed = 32f / Data.Spd;
        DialogList = new List<string>();
        foreach(string dialog in Data.DialogList)
        {
            if(!string.IsNullOrEmpty(dialog)) DialogList.Add(TableManager.GetString(dialog));
        }
        spriteFormat = Data.SpriteName + "_{0:00}";
        foreach (UILabel lb in DmgLabelList) { lb.alpha = 0f; }
        Transform shadowTransform = transform.FindChild("shadow");
        if (shadowTransform != null)
        {
            if (HasShadow) shadowTransform.GetComponent<UISprite>().alpha = 1f;
            else shadowTransform.GetComponent<UISprite>().alpha = 0f;
        }

        State = MonsterState.Spawn;
    }

    void LateUpdate()
    {
        if (map.IsGameStart)
        {
            ExcuteFSM();
            UpdateUI();
        }
    }

    void ExcuteFSM()
    {
        if (map.IsGameFinish) State = MonsterState.End;
        else if (hp == 0) State = MonsterState.Die;

        switch (State)
        {
            case MonsterState.None:
                break;

            case MonsterState.Spawn:
                State = MonsterState.Wait;
                break;

            case MonsterState.Wait:
                if (CheckDistant())
                {
                    curCell = preCell;
                    State = MonsterState.Attack;
                }
                else State = MonsterState.Move;                
                break;

            case MonsterState.Move:
                if (HasArrived())
                {
                    transform.localPosition = curCell.Pos;
                    preCell = curCell;
                    State = MonsterState.Wait;
                }                
                else if (CheckDistant())
                {
                    curCell = preCell;
                    transform.localPosition = curCell.Pos;
                    State = MonsterState.Attack;
                }
                else Move();
                break;

            case MonsterState.Attack:
                if (CheckDistant()) Attack();
                else State = MonsterState.Wait;
                break;

            case MonsterState.Die:
                break;

            case MonsterState.End:
                break;
        }
    }

    void UpdateUI()
    {
        HpBar.fillAmount = (hp+0f) / Data.Hp;
    }

    void Spawn()
    {
        transform.localPosition = SpawnCell.Pos;
        LookAt(GameEnum.Direction.down);
        curCell = SpawnCell;
        preCell = SpawnCell;
    }

    void Wait()
    {
        curDir = GetDirection(map.Player.Pos);
        if (curDir != GameEnum.Direction.none)
        {
            curCell = GetNextCell(curDir);
        }
        Debug.Log("Path " + curCell.Idx + "/ Dir "+curDir.ToString());
    }

    void Move()
    {
        switch(curDir)
        {
            case GameEnum.Direction.left:
                transform.localPosition += Vector3.left * Time.deltaTime * Data.Spd;
                break;

            case GameEnum.Direction.right:
                transform.localPosition += Vector3.right * Time.deltaTime * Data.Spd;
                break;

            case GameEnum.Direction.up:
                transform.localPosition += Vector3.up * Time.deltaTime * Data.Spd;
                break;

            case GameEnum.Direction.down:
                transform.localPosition += Vector3.down * Time.deltaTime * Data.Spd;
                break;
        }

        if(delay < Time.time)
        {
            delay = 0.33f * Speed;
            float audioLength = GameProcess.GetEffectLength(SOUND_EFFECT.MOVE1);
            if (audioLength > delay) delay = audioLength;
            delay = Time.time + delay;
            GameProcess.PlaySound(SOUND_EFFECT.MOVE1);
            NpcSprite.spriteName = string.Format(spriteFormat, (spriteNum % 3 + (int)curDir));
            spriteNum++;
        }

        if(transform.localPosition.y < map.Player.transform.localPosition.y)
        {
            NpcSprite.depth = map.Player.SpriteDepth + 1;
        }
        else
        {
            NpcSprite.depth = map.Player.SpriteDepth - 1;
        }
    }

    void Attack()
    {
        if ((map.Player as IAttackable).IsDie()) return;
        LookAt(map.Player.Pos);
        if (attackDelay < Time.time)
        {
            attackDelay = Data.ASpd * 0.001f + Time.time;
            GameProcess.PlaySound(SOUND_EFFECT.LOSE);
            map.Player.Attacked(Pos,Data.SAtk,false,false);
            Debug.Log("공격");
        }
    }

    void Die()
    {
        NpcSprite.spriteName = "die_02";
        StartCoroutine(OnDie());
    }

    IEnumerator OnDie()
    {
        yield return new WaitForSeconds(0.5f);
        Data_Reward reward = Data_Reward.DoDrop(Data.DropList);
        if (DropEvent != null)
        {
            DropEvent(0, 0, reward);
        }
        if (reward.ItemRewardList.Count > 0)
        {
            TableData_Item item = TableManager.GetGameData(reward.ItemRewardList[0].ItemId) as TableData_Item;
            if (item != null)
            {
                ShowEffect(item.IconName, PfItemEffect);
            }
        }
        else if (reward.Gold > 0)
        {
            ShowEffect("coin_01", PfCoinEffect);
        }
        else if (reward.Coin > 0)
        {
            ShowEffect("coin_02", PfCoinEffect);
        }
        if (DieEvent != null) DieEvent(this);
        yield return new WaitForSeconds(0.5f);
        Hide();
    }

    void End()
    {

    }

    bool CheckDistant()
    {
        float dist = Mathf.Pow(map.Player.Pos.y - Pos.y, 2) + Mathf.Pow(map.Player.Pos.x - Pos.x, 2);
        if (dist > Mathf.Pow(Range, 2)) return false;

        return true;
    }

    bool HasArrived()
    {
        switch (curDir)
        {
            case GameEnum.Direction.left:
                if (curCell.Pos.x < Pos.x) return false;
                break;

            case GameEnum.Direction.right:
                if (curCell.Pos.x > Pos.x) return false;
                break;

            case GameEnum.Direction.up:
                if (curCell.Pos.y > Pos.y) return false;
                break;

            case GameEnum.Direction.down:
                if (curCell.Pos.y < Pos.y) return false;
                break;
        }

        return true;
    }

    public virtual void Attacked(Vector3 _pos, int _atk, bool _critical, bool _knockback)
    {
        if(State == MonsterState.Wait || State == MonsterState.Move || State == MonsterState.Attack)
        {
            GameEnum.Direction dir = GetDirection(_pos);
            LookAt(dir);

            if (_knockback && _critical)
            {
                int idx = curCell.Idx;
                switch (dir)
                {
                    case GameEnum.Direction.left: idx++; break;
                    case GameEnum.Direction.right: idx--; break;
                    case GameEnum.Direction.up: idx += map.ColNum; break;
                    case GameEnum.Direction.down: idx -= map.ColNum; break;
                }
                Component_Map_Cell cell = map.GetCell(idx);
                if (!cell.IsBlock)
                {
                    if (dir == GameEnum.Direction.left) transform.localPosition += Vector3.right * 32;
                    else if (dir == GameEnum.Direction.right) transform.localPosition += Vector3.left * 32;
                    else if (dir == GameEnum.Direction.up) transform.localPosition += Vector3.down * 32;
                    else if (dir == GameEnum.Direction.down) transform.localPosition += Vector3.up * 32;
                    curCell = cell;
                }
            }

            //_atk = _critical ? Mathf.CeilToInt(_atk * 1.2f) : _atk;
            int dmg = Mathf.Max(1, _atk - Data.Def);
            hp = Mathf.Max(0, hp - dmg);

            GameProcess.PlaySound(SOUND_EFFECT.HIT1);
            ShowDmg(dmg, _critical);
        }
    }

    public virtual bool IsDie()
    {
        return State == MonsterState.Die;
    }

    void ShowDmg(int dmg, bool _critical)
    {
        for(int i=0; i < DmgLabelList.Count; ++i)
        {
            if (DmgLabelList[i].GetComponent<Animation>().isPlaying) continue;

            if(_critical) DmgLabelList[i].text = string.Format("[ff0000]CRITICAL\n-{0}[-]", dmg);
            else DmgLabelList[i].text = string.Format("[ff0000]-{0}[-]", dmg);
            DmgLabelList[i].GetComponent<Animation>().Play();
            break;
        }
    }

    GameEnum.Direction GetDirection(Vector3 _pos)
    {
        GameEnum.Direction dir = GameEnum.Direction.none;
        if (_pos.x > Pos.x) dir = GameEnum.Direction.right;
        else if (_pos.x < Pos.x) dir = GameEnum.Direction.left;
        else if (_pos.y > Pos.y) dir = GameEnum.Direction.up;
        else if (_pos.y < Pos.y) dir = GameEnum.Direction.down;

        return dir;
    }

    void LookAt(Vector3 _pos)
    {
        GameEnum.Direction dir = GameEnum.Direction.down;
        if (_pos.x > Pos.x) dir = GameEnum.Direction.right;
        else if (_pos.x < Pos.x) dir = GameEnum.Direction.left;
        else if (_pos.y > Pos.y) dir = GameEnum.Direction.up;

        LookAt(dir);
    }

    void LookAt(GameEnum.Direction dir)
    {
        NpcSprite.spriteName = string.Format(spriteFormat, (2 + (int)dir));
    }

    Component_Map_Cell GetNextCell(GameEnum.Direction dir)
    {
        int idx = curCell.Idx;
        switch(dir)
        {
            case GameEnum.Direction.left: idx--; break;
            case GameEnum.Direction.right: idx++; break;
            case GameEnum.Direction.up: idx -= map.ColNum; break;
            case GameEnum.Direction.down: idx += map.ColNum; break;
        }

        Component_Map_Cell cell = map.GetCell(idx);
        if(cell.IsBlock)
        {
            switch (dir)
            {
                case GameEnum.Direction.left:
                case GameEnum.Direction.right:
                    if (map.Player.Pos.y > transform.localPosition.y) curDir = GameEnum.Direction.up;
                    else curDir = GameEnum.Direction.down;
                    break;

                case GameEnum.Direction.up:
                case GameEnum.Direction.down:
                    if (map.Player.Pos.x > transform.localPosition.x) curDir = GameEnum.Direction.right;
                    else curDir = GameEnum.Direction.left;
                    break;
            }

            idx = curCell.Idx;
            switch (curDir)
            {
                case GameEnum.Direction.left: idx--; break;
                case GameEnum.Direction.right: idx++; break;
                case GameEnum.Direction.up: idx -= map.ColNum; break;
                case GameEnum.Direction.down: idx += map.ColNum; break;
            }

            cell = map.GetCell(idx);
            if (cell.IsBlock) cell = curCell;
        }

        return cell;
    }

    bool isLock;
    void OnClick()
    {
        if (isLock) return;
        isLock = true;
        map.Player.OnClickObject(this);        
        isLock = false;
    }

    void ShowEffect(string _sprite, GameObject _pf)
    {
        GameObject go = NGUITools.AddChild(transform.parent.gameObject, _pf);
        Component_Effect_Coin effect = go.GetComponent<Component_Effect_Coin>();
        if (effect != null)
        {
            go.transform.position = map.Player.transform.position;
            effect.Init(_sprite);
            effect.Play();
        }
        else Destroy(go);
    }

    //void Reward()
    //{
    //    List<TableData_Drop> list = new List<TableData_Drop>();
    //    for (int i = 0; i < Data.DropList.Count; ++i)
    //    {
    //        TableData_Drop drop = TableManager.GetGameData(Data.DropList[i]) as TableData_Drop;
    //        if (drop != null) list.Add(drop);
    //    }

    //    map.AddReward(0, 0, list);
    //}
}
