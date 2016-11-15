using UnityEngine;
using System.Collections;
using System;

using Kazoh.Table;

public class Component_UI_GameOver : GameComponent
{
    public Action ClickedEvent;

    public UILabel GameOverLabel;
    public UILabel RetryLabel;

    private Animation anim;

    public override void Init()
    {
        anim = GetComponent<Animation>();
        GameOverLabel.text = TableManager.GetString("STR_TITLE_GAMEOVER");
        RetryLabel.text = TableManager.GetString("STR_UI_RETRY");
        base.Init();
    }

    public override void Show()
    {
        base.Show();
        if (anim != null)
        {
            GameProcess.PlaySound(SOUND_EFFECT.GAMEOVER);
            anim.Play();
        }
    }

    void OnClick()
    {
        if (anim.isPlaying) return;
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        Hide();
        if (ClickedEvent != null) ClickedEvent();
    }
}
