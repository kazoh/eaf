using UnityEngine;
using System.Collections;
using System;

public class Component_Effect_Coin : Component_Map_Object
{
    public UISprite Sprite;
    private Animation anim;

    public void Init(string _spriteName)
    {
        Init();
        if (Sprite != null)
        {
            Sprite.spriteName = _spriteName;
            Sprite.alpha = 0f;
        }
        anim = GetComponent<Animation>();
    }

    public void Play()
    {
        if (anim == null || anim.isPlaying) return;
        anim.Play();
        GameProcess.PlaySound(SOUND_EFFECT.COIN);
        StartCoroutine(Despawn());
    }

    IEnumerator Despawn()
    {
        while(anim.isPlaying)
        {
            yield return null;
        }

        Destroy(gameObject);
    }
}
