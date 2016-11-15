using UnityEngine;
using System.Collections;
using System;

using Kazoh.Table;

public class Component_UI_GameStart : GameComponent {
    
    public Action ClickedEvent;

    public UILabel TitleLabel;
    public UILabel DescLabel;
    public UILabel StartLabel;

    private Animation anim;

    public override void Init()
    {
        base.Init();
        anim = GetComponent<Animation>();
        StartLabel.text = TableManager.GetString("STR_TITLE_GAMESTART");
    }

    public void Show(TableData_Map _data)
    {
        TitleLabel.text = TableManager.GetString(_data.Str);
        DescLabel.text = TableManager.GetString(_data.Desc);
        base.Show();
        if (anim != null) anim.Play("anim_ui_start_01");
    }

    public override void Hide()
    {
        if (anim != null) anim.Play("anim_ui_start_02");
    }

    public void OnClick_Start()
    {
        if (!anim.isPlaying)
        {
            Hide();
            GameProcess.PlaySound(SOUND_EFFECT.CLICK);
            StartCoroutine(CheckAnimPlay());
        }
    }

    IEnumerator CheckAnimPlay()
    {
        while(anim.isPlaying)
        {
            yield return null;
        }

        if (ClickedEvent != null) ClickedEvent();
    }
}
