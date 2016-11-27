using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Component_Map_Player : GameComponent, IAttackable, IManagedDepth
{
    public enum PlayerState
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

    public Action DieEvent;

    public int Range = 32;
    public bool HasShadow = true;
    public UISprite ChaSprite;
    public UISprite HpBar;
    public List<UILabel> DmgLabelList;
    
    public Data_UserCharacter CharacterData { get; private set; }
    public Vector3 Pos { get { return transform.localPosition; } }
    public float Speed { get; private set; }

    public int SpriteDepth { get { return ChaSprite.depth; } }

    private Component_Map map;
    private Queue<Component_Map_Cell> pathQueue;
    private Component_Map_Cell curPath;
    private Component_Map_Cell playerCell;
    private TweenPosition tp;
    private string actorName;
    private GameEnum.Direction curDir;
    private string spriteFormat;
    private int spriteNum;
    private float animSpeed;
    private GameObject pfBullet;
    private UISprite shadowSprite;

    private IAttackable attackTarget;
    private float delay = float.MinValue;

    private PlayerState state;
    public PlayerState State
    {
        get { return state; }
        private set
        {
            if (state == value) return;
#if DEBUG_MODE
            Debug.Log(string.Format("플레이어 상태 변경 : {0} -> {1}", state, value));
#endif
            state = value;
            switch(state)
            {
                case PlayerState.None:
                    break;

                case PlayerState.Spawn:
                    Spawn();
                    break;

                case PlayerState.Wait:
                    Wait();
                    break;

                case PlayerState.Move:
                    Move();
                    break;

                case PlayerState.Attack:
                    LookAt((attackTarget as Component_Map_Object).Pos);
                    break;

                case PlayerState.Die:
                    Die();
                    break;

                case PlayerState.End:
                    End();
                    break;
            }
        }
    }

    public void Init(Component_Map _map)
    {
        try
        {
            map = _map;
            pathQueue = new Queue<Component_Map_Cell>();
            tp = GetComponent<TweenPosition>();
            if (tp == null)
            {
                tp = gameObject.AddComponent<TweenPosition>();
                tp.enabled = false;
            }
            SetData(map.CharacterData);
            foreach (UILabel lb in DmgLabelList) { lb.alpha = 0f; }
            Transform shadowTransform = transform.FindChild("shadow");
            if (shadowTransform != null)
            {
                if (HasShadow)
                {
                    shadowSprite = shadowTransform.GetComponent<UISprite>();
                    if(shadowSprite != null) shadowSprite.alpha = 1f;
                }
                else
                {
                    shadowSprite = shadowTransform.GetComponent<UISprite>();
                    if (shadowSprite != null) shadowSprite.alpha = 0f;
                }
            }
            pfBullet = Resources.Load("Prefabs/" + CharacterData.BulletName) as GameObject;
            State = PlayerState.Spawn;
        }
        catch(Exception e)
        {
            throw e;
        }
    }

    public void SetPath(Component_Map_Cell _cell)
    {
        if (State == PlayerState.None || State == PlayerState.Die || State == PlayerState.End) return;

        List<Component_Map_Cell> paths = map.GetPath(playerCell, _cell);
        pathQueue.Clear();
        for(int i=0; i < paths.Count; ++i)
        {
            pathQueue.Enqueue(paths[i]);
        }
        
        State = PlayerState.Move;
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
        if (map.IsGameFinish) State = PlayerState.End;
        else if (CharacterData.CurHp == 0) State = PlayerState.Die;
        switch (State)
        {
            case PlayerState.None:
                break;

            case PlayerState.Wait:
                if (pathQueue.Count > 0) State = PlayerState.Move;
                break;

            case PlayerState.Move:
                if (curPath == null) State = PlayerState.Wait;
                break;

            case PlayerState.Attack:
                if (attackTarget.IsDie()) State = PlayerState.Wait;
                else Attack();
                break;

            case PlayerState.Die:
                break;

            case PlayerState.End:
                break;
        }
    }

    void SetData(Data_UserCharacter _data)
    {
        if (_data == null) throw new GameException(GameException.ErrorCode.NoGameData);
        CharacterData = _data;
        actorName = CharacterData.Data.SpriteName;
        Speed = 32f / CharacterData.Spd;
        animSpeed = 0.33f * Speed;
        float audioLength = GameProcess.GetEffectLength(SOUND_EFFECT.MOVE1);
        if (audioLength > animSpeed) animSpeed = audioLength;
        spriteFormat = actorName + "_{0:00}";
        ChaSprite.spriteName = string.Format(spriteFormat, 2);
    }

    void UpdateUI()
    {
        HpBar.fillAmount = (CharacterData.CurHp + 0f) / CharacterData.MaxHp;
    }

    void Spawn()
    {
        Component_Map_Cell sp = map.GetStartPoint();
        if (sp != null)
        {
            playerCell = sp;
            transform.localPosition = sp.Pos;
            UpdateUI();
            State = PlayerState.Wait;
        }
        else
        {
#if UNITY_EDITOR
            Debug.LogError("스타트 포인트가 설정되지 않음");
#endif
            State = PlayerState.Die;
        }
    }

    void Wait()
    {
    }

    void Move()
    {
        SetNextPath();
        if(curPath != null) StartCoroutine(PlayMoveAnim());
    }

    void Attack()
    {
        if (delay > Time.time) return;

        bool isCritical = IsCraticalAttack();

        delay = CharacterData.ASpd * 0.001f + Time.time;
        if (CheckDistant())
        {
            GameProcess.PlaySound(SOUND_EFFECT.FIRE1);
            int atk = isCritical ? Mathf.CeilToInt(CharacterData.SAtk * CharacterData.CriticalPercent) : CharacterData.SAtk;
            attackTarget.Attacked(Pos,atk,isCritical,true);
        }
        else
        {
            GameObject go = NGUITools.AddChild(transform.parent.gameObject, pfBullet);
            if(go != null)
            {
                Component_Map_Bullet bullet = go.GetComponent<Component_Map_Bullet>();
                if (bullet != null)
                {
                    int atk = isCritical ? Mathf.CeilToInt(CharacterData.LAtk * CharacterData.CriticalPercent) : CharacterData.LAtk;
                    bullet.Init(map, Component_Map_Bullet.TargetType.Enemy, ChaSprite.depth);
                    bullet.Shoot(curDir, Pos, atk, isCritical);
                }
                else
                {
#if UNITY_EDITOR
                    Debug.Log("탄환 없음");
#endif
                }
            }
            else
            {
#if UNITY_EDITOR
                Debug.Log("탄환 프리펩 없음");
#endif
            }
        }
    }

    void Die()
    {
        ChaSprite.spriteName = "die_01";
        StartCoroutine(OnDie());
    }

    IEnumerator OnDie()
    {
        yield return new WaitForSeconds(0.5f);
        if (DieEvent != null) DieEvent();
    }

    public void Falling()
    {
        State = PlayerState.End;
        StartCoroutine(OnFalling());
    }

    IEnumerator OnFalling()
    {
        ChaSprite.spriteName = "falling_01";
        yield return new WaitForSeconds(0.1f);
        ChaSprite.spriteName = "falling_02";
        yield return new WaitForSeconds(0.1f);
        ChaSprite.spriteName = "falling_03";
        yield return new WaitForSeconds(0.4f);
        if (DieEvent != null) DieEvent();
    }

    void End()
    {
    }

    GameEnum.Direction GetDirection(Component_Map_Cell _path)
    {
        if (playerCell == _path) return GameEnum.Direction.none;
        if(Mathf.Pow(playerCell.Row - _path.Row,2) > Mathf.Pow(playerCell.Col - _path.Col, 2))
        {
            if (playerCell.Row < _path.Row) return GameEnum.Direction.down;
            return GameEnum.Direction.up;
        }
        else
        {
            if (playerCell.Col < _path.Col) return GameEnum.Direction.right;
            return GameEnum.Direction.left;
        }
    }

    GameEnum.Direction GetDirection(Vector3 _pos)
    {        
        if (Mathf.Pow(Pos.x - _pos.x, 2) > Mathf.Pow(Pos.y - _pos.y, 2))
        {
            if (Pos.x < _pos.x) return GameEnum.Direction.right;
            return GameEnum.Direction.left;
        }
        else
        {
            if (Pos.y < _pos.y) return GameEnum.Direction.up;
            return GameEnum.Direction.down;
        }
    }

    IEnumerator PlayMoveAnim()
    {
        float animTime = float.MinValue;
        while (State == PlayerState.Move)
        {
            yield return null;
            if(animTime < Time.time)
            {
                animTime = Time.time + animSpeed;
                GameProcess.PlaySound(SOUND_EFFECT.MOVE1);
                ChaSprite.spriteName = string.Format(spriteFormat, (spriteNum % 3 + (int)curDir));
                spriteNum++;
            }
        }
    }

    void SetMove(GameEnum.Direction dir)
    {

#if DEBUG_MODE
        Debug.Log(string.Format("이동: {0},{1}->{2},{3}", playerCell.Row, playerCell.Col, curPath.Row, curPath.Col));
#endif

        if (playerCell == curPath)
        {
            SetNextPath();
            return;
        }

        playerCell = curPath;

        tp = TweenPosition.Begin(gameObject, Speed, curPath.Pos);
        tp.eventReceiver = gameObject;
        tp.callWhenFinished = "SetNextPath";
        if (curDir != dir)
        {
            curDir = dir;
            spriteNum = 0;
        }
    }

    void SetNextPath()
    {
        if (State != PlayerState.Move) return;
        if (pathQueue.Count > 0)
        {
            curPath = pathQueue.Dequeue();
            GameEnum.Direction dir = GetDirection(curPath);
            SetMove(dir);
        }
        else
        {
            curPath = null;
        }
    }

    bool CheckDistant()
    {
        float dist = Mathf.Pow((attackTarget as Component_Map_Object).Pos.y - Pos.y, 2) + Mathf.Pow((attackTarget as Component_Map_Object).Pos.x - Pos.x, 2);
        if (dist > Mathf.Pow(Range, 2)) return false;

        return true;
    }

    public void OnClickObject(Component_Map_Object obj)
    {
        if (State != PlayerState.Wait) return;

        if(obj is ITalkable)
        {
            LookAt(obj.Pos);
            (obj as ITalkable).Talk(this);
        }
        else if (obj is IAttackable)
        {
            attackTarget = obj as IAttackable;
            State = PlayerState.Attack;
        }
        else 
        {
            LookAt(obj.Pos);
        }
    }
    
    void LookAt(Vector3 _pos)
    {
        GameEnum.Direction dir = GetDirection(_pos);

        curDir = dir;
        ChaSprite.spriteName = string.Format(spriteFormat, (2 + (int)dir));
    }

    public virtual void Attacked(Vector3 _pos, int _atk, bool _critical, bool _knockback)
    {
        if (State == PlayerState.Die || State == PlayerState.End) return;

        LookAt(_pos);

        _atk = _critical ? Mathf.CeilToInt(_atk * 1.2f) : _atk;
        int dmg = Mathf.Max(1, _atk - CharacterData.Def);
#if UNITY_EDITOR
        if(map.TestMode)
        {
            CharacterData.CurHp -= dmg;
        }
        else
        {
            GameProcess.GetGameDataManager().AddHp(GameProcess.GetGameDataManager().CurCharacterSlot, -dmg);
        }
#else
        GameProcess.GetGameDataManager().AddHp(GameProcess.GetGameDataManager().CurCharacterSlot,-dmg);
#endif

        GameProcess.PlaySound(SOUND_EFFECT.HIT1);
        ShowDmg(dmg, _critical);
    }

    void ShowDmg(int dmg, bool _critical)
    {
        for (int i = 0; i < DmgLabelList.Count; ++i)
        {
            if (DmgLabelList[i].GetComponent<Animation>().isPlaying) continue;

            if (_critical) DmgLabelList[i].text = string.Format("[ff0000]CRITICAL\n-{0}[-]", dmg);
            else DmgLabelList[i].text = string.Format("[ff0000]-{0}[-]", dmg);
            DmgLabelList[i].GetComponent<Animation>().Play();
            break;
        }
    }

    bool IsCraticalAttack()
    {
        int dice = UnityEngine.Random.Range(0, 100);
        if (dice < CharacterData.Critical) return true;
        return false;
    }

    public virtual bool IsDie()
    {
        return State == PlayerState.Die;
    }

    public void SetDepth(int _depth)
    {
        if(ChaSprite.depth != _depth)
        {
            ChaSprite.depth = _depth;
            //if (shadowSprite != null) shadowSprite.depth = _depth - 1;
            if (HpBar != null) HpBar.depth = _depth + 1;
        }
    }

    public float GetPosY()
    {
        return Pos.y;
    }
}
