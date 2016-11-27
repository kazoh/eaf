using UnityEngine;
using System.Collections;

public class Component_UI_Loading : GameComponent {

    public GameObject AnimRoot;
    public UILabel TitleLabel;
    public UILabel TipLabel;
    public UISprite LoaderSprite;
    public string LoaderSpriteName;

    private string spriteFormat;
    private bool isLoading;
    private string tip;

    public override void Init()
    {
        base.Init();

        /* 로딩 아이콘 초기화 */
        spriteFormat = LoaderSpriteName + "_{0:00}";
        TipLabel.text = "";
    }

    public override void Show()
    {
        if (TipLabel != null && !string.IsNullOrEmpty(tip)) TipLabel.text = tip;
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
        if (!isLoading)
        {
            isLoading = true;
            StartCoroutine(PlayAnim());
        }
    }

    public override void Hide()
    {
        if (isLoading)
        {
            isLoading = false;
            StopCoroutine(PlayAnim());
        }
        base.Hide();
    }

    public void SetTip(string _tip)
    {
        tip = _tip;
    }

    IEnumerator PlayAnim()
    {
        int num = 0;
        while(true)
        {
            LoaderSprite.spriteName = string.Format(spriteFormat, num % 3 + 1);
            num++;
            yield return new WaitForSeconds(0.1f);
        }
    }
}
