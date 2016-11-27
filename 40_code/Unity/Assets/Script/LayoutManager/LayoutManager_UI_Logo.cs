using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class LayoutManager_UI_Logo : LayoutManager {

	public Animation AnimLogo;
    public UILabel LabelStart;
    public UILabel LabelVersion;
    public UISprite SpriteTitle;
    public UISprite SpriteLogo;
    
	private string startText;
	private bool isReady;
    private string verText;

    public override void Init()
    {
        SpriteLogo.alpha = 0f;
        StartCoroutine(Loading());
    }

    protected override IEnumerator Loading()
    {
        yield return base.Loading();

        LabelStart.text = string.Empty;

#if UNITY_EDITOR
        SystemLanguage lang = GameProcess.Instance.Language;
#else
        SystemLanguage lang = Application.systemLanguage;
#endif
        switch (lang)
        {
            case SystemLanguage.Korean:
                SpriteTitle.spriteName = "title_ko";
                SpriteLogo.spriteName = "logo_ko";
                startText = "화 면 을  터 치 해  주 세 요 .";
                break;

            case SystemLanguage.Japanese:
                SpriteTitle.spriteName = "title_ja";
                SpriteLogo.spriteName = "logo_ja";
                startText = "画 面 を タ ッ チ し て く だ さ い 。";
                break;

            default:
                SpriteTitle.spriteName = "title_en";
                SpriteLogo.spriteName = "logo_en";
                startText = "T o u c h  t o  s t a r t .";
                break;
        }
        verText = "Ver " + GameProcess.Instance.ShortVersion;
        LabelVersion.text = "";

        AnimLogo.Play();
        while (AnimLogo.isPlaying)
        {
            yield return null;
        }

        GameProcess.PlaySound(SOUND_EFFECT.BGM, "bgm_01");
        LabelVersion.text = verText;
        StartCoroutine(AnimStartText());
        isReady = true;
    }

    bool isLock = false;
	public void OnClick_Start()
	{
		if(!isReady || isLock) return;
		isLock = true;

        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        StartCoroutine(GameStarting());
	}

    IEnumerator GameStarting()
    {
        GameProcess.ShowLoading();

        yield return null;

        /* 테이블 데이터 로딩 */
        try
        {
            GameProcess.LoadTable();
            DBManager.GetServerState(delegate (int _state)
            {
                try
                {
                    switch(_state)
                    {
                        case -1: throw new GameException(GameException.ErrorCode.UnknownServerError);
                        case 0: throw new GameException(GameException.ErrorCode.ServerShutDown);
                        case 2: throw new GameException(GameException.ErrorCode.CloseTest);
                        default:
                            DBManager.GetConfig(delegate (bool _isFail, string _data)
                            {
                                try
                                {
                                    if (_isFail) throw new GameException(GameException.ErrorCode.FailToGetGameConfig);
                                    GameProcess.SetGameConfig(_data);
                                    LoadScene("LogInScene");
                                }
                                catch (GameException e)
                                {
                                    Debug.LogError(e.Msg);
                                    GameProcess.ShowError(e);
                                }
                                catch (Exception e)
                                {
                                    Debug.LogError(e.Message);
                                    GameProcess.ShowError(new GameException(GameException.ErrorCode.Unknown, e.Message));
                                }
                            });
                            break;
                    }
                }
                catch (GameException e)
                {
                    Debug.LogError(e.Msg);
                    GameProcess.ShowError(e);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                    GameProcess.ShowError(new GameException(GameException.ErrorCode.Unknown, e.Message));
                }
            });
        }
        catch (GameException e)
        {
            Debug.LogError(e.Msg);
            GameProcess.ShowError(e,"오류","확인");
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            Quit();
        }
    }

    IEnumerator AnimStartText()
    {
        while(true)
        {
            LabelStart.text = startText;
            yield return new WaitForSeconds(1f);
            LabelStart.text = string.Empty;
            yield return new WaitForSeconds(0.5f);
        }
    }

    protected override void Quit()
    {
        Application.Quit();
    }
}
