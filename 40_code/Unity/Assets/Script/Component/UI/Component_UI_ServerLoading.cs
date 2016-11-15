using UnityEngine;
using System.Collections;

public class Component_UI_ServerLoading : GameComponent {

    public GameObject AnimRoot;

    public GameObject PrefLoadingIcon;
    private Animation anim;

    public override void Init()
    {
        base.Init();

        /* 로딩 아이콘 초기화 */
        GameObject go = NGUITools.AddChild(AnimRoot, PrefLoadingIcon);
        anim = go.GetComponent<Animation>();
    }

    public override void Show()
    {
        base.Show();
        if (anim != null && !anim.isPlaying) anim.Play();
    }

    public override void Hide()
    {
        if (anim != null && anim.isPlaying) anim.Stop();
        base.Hide();
    }
}
