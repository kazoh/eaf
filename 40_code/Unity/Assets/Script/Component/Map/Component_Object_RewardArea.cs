using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Component_Object_RewardArea : Component_Map_Object
{
    public Action<int, int, Data_Reward> EnterAreaEvent;

    public int Width;
    public int Height;
    public float RewardTime;
    public int RewardGold;
    public GameObject PfCoinEffect;

    private bool isEnter;
    private float enterTime;
    private UISprite areaSprite;

    public override void Init()
    {
        areaSprite = gameObject.GetComponentInChildren<UISprite>();
        if(areaSprite != null)
        {
#if UNITY_EDITOR
            areaSprite.width = Width;
            areaSprite.height = Height;
            areaSprite.gameObject.SetActive(true);
#else
            areaSprite.gameObject.SetActive(false);
#endif
        }

        StartCoroutine(Trace());
        base.Init();
    }

    IEnumerator Trace()
    {
        while(true)
        {
            yield return null;

            if (EnterArea())
            {
                if (isEnter) enterTime += Time.deltaTime;
                else isEnter = true;
            }
            else
            {
                isEnter = false;
                enterTime = 0;
            }

            if(enterTime > RewardTime && EnterAreaEvent != null)
            {
                enterTime = 0;
                EnterAreaEvent(RewardGold,0,null);
                ShowEffect("coin_01", PfCoinEffect);
            }
        }
    }

    bool EnterArea()
    {
        if (map.Player.Pos.y > transform.localPosition.y + Height * 0.5f) return false;
        if (map.Player.Pos.y < transform.localPosition.y - Height * 0.5f) return false;
        if (map.Player.Pos.x > transform.localPosition.x + Width * 0.5f) return false;
        if (map.Player.Pos.x < transform.localPosition.x - Width * 0.5f) return false;

        return true;
    }

    void OnDestroy()
    {
        StopAllCoroutines();
    }

    void ShowEffect(string _sprite, GameObject _pf)
    {
        if (map.IsGameFinish) return;
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
}
