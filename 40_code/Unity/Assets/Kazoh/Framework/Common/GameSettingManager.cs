using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Kazoh.Table;
using MiniJSON;

public class GameSettingManager {

    public float BgmVolume { get; private set; }
    public float EffectVolume { get; private set; }

    public GameSettingManager()
    {
        if (PlayerPrefs.HasKey("BGM_VOLUME")) BgmVolume = PlayerPrefs.GetFloat("BGM_VOLUME");
        else BgmVolume = 1;
        if (PlayerPrefs.HasKey("EFFECT_VOLUME")) EffectVolume = PlayerPrefs.GetFloat("EFFECT_VOLUME");
        else EffectVolume = 1;

#if UNITY_EDITOR
        Debug.Log("[GameSettingManager] GameSettingManager 생성 완료!!!");
#endif

    }

    public void SetGameSetting(float bgmVol, float effectVol)
    {
        BgmVolume = bgmVol;
        EffectVolume = effectVol;

        Save();
    }

    void Save()
    {
        PlayerPrefs.SetFloat("BGM_VOLUME", BgmVolume);
        PlayerPrefs.SetFloat("EFFECT_VOLUME", EffectVolume);

#if UNITY_EDITOR || DEBUG_MODE
        Debug.Log(string.Format("[GameSettingManager] 게임 설정 저장: 배경음:{0} 효과음:{1}",BgmVolume,EffectVolume));
#endif
    }
}
