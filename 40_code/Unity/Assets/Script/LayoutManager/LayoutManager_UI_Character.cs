using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class LayoutManager_UI_Character : LayoutManager {

    public Component_UI_CharacterSelect CharacterSelectUI;

    protected override IEnumerator Loading()
    {
        yield return base.Loading();

        /* 배경음 적용 */
        GameProcess.PlaySound(SOUND_EFFECT.BGM, "bgm_01");

        /* 유저 정보 UI 초기화 */
        CharacterSelectUI.Init();
        CharacterSelectUI.ClickedEvent += OnSelect;

        OnFinishLoading();
    }

    void OnSelect(int _id)
    {
        Debug.Log("선택된 캐릭터 : " + _id);
        try
        {
            GameProcess.GetGameDataManager().AddCharacter(_id);
            GameProcess.GetGameDataManager().Save();
            LoadScene("GameScene");
        }
        catch(GameException e)
        {
            Debug.LogError(e.Message);
            GameProcess.ShowError(e);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    protected override void Quit()
    {
        base.Quit();
    }
}
