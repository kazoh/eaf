using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using Kazoh.Table;

public class Component_Map_Bullet : Component_Map_Object
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

    public enum TargetType
    {
        Enemy,
        Player,
        All,
    }

    public UISprite BulletSprite;
    public string SpriteFormat;
    public int Range;
    public int Speed;
    public bool IsSpawnEffect;
    public bool IsDespawnEffect;

    private TargetType targetType;
    private IAttackable target;
    private GameEnum.Direction curDir;

    private int atk;
    private bool isCritiacl;
    private int num;
    private float delay = float.MinValue;
    private List<Component_Map_Cell> listWall;

    private MonsterState state;
    public MonsterState State
    {
        get { return state; }
        private set
        {
            if (state == value) return;
#if UNITY_EDITOR
            //Debug.Log(string.Format("불릿 상태 변경 : {0} -> {1}", state, value));
#endif
            state = value;
            switch (state)
            {
                case MonsterState.None:
                    break;

                case MonsterState.Wait:
                    Wait();
                    break;

                case MonsterState.Spawn:
                    Spawn();
                    break;

                case MonsterState.Move:
                    StartCoroutine(PlayAnim_Move());
                    break;

                case MonsterState.Attack:
                    Attack();
                    break;

                case MonsterState.End:
                    End();
                    break;
            }
        }
    }

    public override void Init()
    {
        base.Init();
        listWall = map.CellList.FindAll(x => x.IsWall);
        num = 0;
        delay = float.MinValue;
        State = MonsterState.Wait;
    }

    public void Init(Component_Map _map, TargetType _target, int _depth)
    {
        map = _map;
        targetType = _target;
        BulletSprite.depth = _depth;
        Init();
    }

    void LateUpdate()
    {
        if (map.IsGameStart)
        {
            ExcuteFSM();
        }
    }

    void ExcuteFSM()
    {
        switch (State)
        {
            case MonsterState.None:
                break;

            case MonsterState.Wait:     
                break;

            case MonsterState.Spawn:
                break;

            case MonsterState.Move:
                if (CheckEnd() || CheckCollisionWithWall()) State = MonsterState.End;
                else if (HasArrived())
                {
                    State = MonsterState.Attack;
                }
                else Move();
                break;

            case MonsterState.Attack:
                State = MonsterState.End;
                break;

            case MonsterState.End:
                State = MonsterState.Wait;
                break;
        }
    }

    void Wait()
    {
    }

    void Spawn()
    {
        BulletSprite.alpha = 1f;
        StartCoroutine(PlayAnim_Spawn());
    }

    void Move()
    {
        switch (curDir)
        {
            case GameEnum.Direction.up:
                transform.localPosition += Vector3.up * Time.deltaTime * Speed;
                break;

            case GameEnum.Direction.right:
                transform.localPosition += Vector3.right * Time.deltaTime * Speed;
                break;

            case GameEnum.Direction.down:
                transform.localPosition += Vector3.down * Time.deltaTime * Speed;
                break;

            case GameEnum.Direction.left:
                transform.localPosition += Vector3.left * Time.deltaTime * Speed;
                break;
        }
    }

    void Attack()
    {
        target.Attacked(Pos,atk,isCritiacl,false);
    }

    void End()
    {
        StartCoroutine(PlayAnim_Destroy());
    }

    bool CheckDistant(Vector3 _pos)
    {
        float dist = Mathf.Pow(_pos.y - Pos.y, 2) + Mathf.Pow(_pos.x - Pos.x, 2);
        if (dist > Mathf.Pow(Range, 2)) return false;

        return true;
    }

    bool HasArrived()
    {
        if(targetType == TargetType.Enemy || targetType == TargetType.All)
        {
            List<Component_Map_Object> list = map.ObjList;
            for (int i = 0; i < list.Count; ++i)
            {
                if (list[i] is IAttackable && CheckDistant(list[i].Pos))
                {
                    target = list[i] as IAttackable;
                    return true;
                }
            }
        }
        if (targetType == TargetType.Player || targetType == TargetType.All)
        {
            if (map.Player is IAttackable && CheckDistant(map.Player.Pos))
            {
                target = map.Player as IAttackable;
                return true;
            }
        }

        return false;
    }

    bool CheckEnd()
    {
        if (Pos.x > 200 || Pos.x < -200 || Pos.y > 200 || Pos.y < -200) return true;
        return false;
    }

    bool CheckCollisionWithWall()
    {
        if (listWall == null) return false;
        for(int i=0; i<listWall.Count; ++i)
        {
            float dist_x = listWall[i].transform.localPosition.x - transform.localPosition.x;
            float dist_y = listWall[i].transform.localPosition.y - transform.localPosition.y;
            if (Mathf.Pow(dist_x, 2) + Mathf.Pow(dist_y, 2) < Mathf.Pow(Range, 2)) return true;
        }
        return false;
    }

    void LookAt(GameEnum.Direction dir)
    {
        curDir = dir;
    }

    IEnumerator PlayAnim_Spawn()
    {
        if (IsSpawnEffect)
        {
            for (int i = 17; i < 20; ++i)
            {
                BulletSprite.spriteName = string.Format("{0}_{1:00}", SpriteFormat, i);
                yield return null;
            }
        }

        State = MonsterState.Move;
    }

    IEnumerator PlayAnim_Move()
    {
        while(State == MonsterState.Move)
        {
            if (delay < Time.time)
            {
                delay = Time.time + 0.1f;
                BulletSprite.spriteName = string.Format("{0}_{1:00}", SpriteFormat, (int)curDir + (num % 3));
                num++;
            }
            
            yield return null;
        }
    }

    IEnumerator PlayAnim_Destroy()
    {
        if(IsDespawnEffect)
        {
            for (int i = 13; i < 17; ++i)
            {
                BulletSprite.spriteName = string.Format("{0}_{1:00}", SpriteFormat, i);
                yield return null;
            }
        }

        Destroy(gameObject);
    }

    public void Shoot(GameEnum.Direction dir, Vector3 pos, int _atk, bool _critical)
    {
        LookAt(dir);

        transform.localPosition = pos;
        if (dir == GameEnum.Direction.right) transform.localPosition += Vector3.right * 32;
        else if (dir == GameEnum.Direction.left) transform.localPosition += Vector3.left * 32;
        else if (dir == GameEnum.Direction.down) transform.localPosition += Vector3.down * 16;
        else if (dir == GameEnum.Direction.up) transform.localPosition += Vector3.up * 32;

        atk = _atk;
        isCritiacl = _critical;

        GameProcess.PlaySound(SOUND_EFFECT.FIRE2);
        State = MonsterState.Spawn;
    }
}
