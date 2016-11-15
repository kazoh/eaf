using UnityEngine;
using System.Collections;

public class Component_UI_Loading : GameComponent {

    public GameObject AnimRoot;
    public UILabel TitleLabel;
    public UILabel TipLabel;

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
        if (TipLabel != null) TipLabel.text = "";
        if (TitleLabel != null)
        {
#if UNITY_EDITOR
            switch (GameProcess.Instance.Language)
            {
                case SystemLanguage.Korean: TitleLabel.text = "데이터 로딩 중...."; break;
                case SystemLanguage.Japanese: TitleLabel.text = "データの読み込み中...."; break;
                case SystemLanguage.English: TitleLabel.text = "Loading...."; break;
                default: TitleLabel.text = "Loading...."; break;
            }
#else               
            switch(Application.systemLanguage)
            { 
                case SystemLanguage.Korean: TitleLabel.text = "데이터 로딩 중...."; break;
                case SystemLanguage.Japanese: TitleLabel.text = "データの読み込み中...."; break;
                case SystemLanguage.English: TitleLabel.text = "Loading...."; break;
                default: TitleLabel.text = "Loading...."; break;                
            }
#endif
        }

        base.Show();
        if (anim != null && !anim.isPlaying) anim.Play();
    }

    public override void Hide()
    {
        if (anim != null && anim.isPlaying) anim.Stop();
        base.Hide();
    }

    public void ShowTip(string tip)
    {
        TipLabel.text = tip;
    }
}
