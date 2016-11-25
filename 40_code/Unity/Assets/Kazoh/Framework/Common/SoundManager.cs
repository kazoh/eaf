using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum SOUND{ ON, OFF }
public enum SOUND_EFFECT
{
	BGM,
	FIRE1,
    FIRE2,
    FIRE3,
    HIT1,
    HIT2,
    HIT3,
    CLICK,
	ERROR,
	MISSION_CLEAR,
	GET,
    COIN,
	WARNING,
	SYS,
    MOVE1,
    MOVE2,
    MOVE3,
    WIN,
    LOSE,
    SUCCESS,
    FAIL,
    GAMEOVER,
    GAMECLEAR,
    SUMMON,
    ENCHANT,
    DOWN_GRADE,
    CHA_ENCHANT,
    NOTICE,
    SPAWN,
    HEAL,
    OPEN,
    EQUIP,
}

public class SoundManager : MonoBehaviour {
	
	public int EffectSoundPoolCount;

    public AudioClip Fire1AudioClip;
    public AudioClip Fire2AudioClip;
    public AudioClip Fire3AudioClip;
    public AudioClip Hit1AudioClip;
    public AudioClip Hit2AudioClip;
    public AudioClip Hit3AudioClip;
    public AudioClip ClickAudioClip;
	public AudioClip ErrorAudioClip;
	public AudioClip MissionAudioClip;
	public AudioClip ItemAudioClip;
    public AudioClip CoinAudioClip;
    public AudioClip WarningAudioClip;
    public AudioClip SysAudioClip;
    public AudioClip Move1AudioClip;
    public AudioClip Move2AudioClip;
    public AudioClip Move3AudioClip;
    public AudioClip WinAudioClip;
    public AudioClip LoseAudioClip;
    public AudioClip SuccessAudioClip;
    public AudioClip FailAudioClip;
    public AudioClip SummonAudioClip;
    public AudioClip EnchantAudioClip;
    public AudioClip DownGradeAudioClip;
    public AudioClip ChaEnchantAudioClip;
    public AudioClip GameClearAudioClip;
    public AudioClip GameOverAudioClip;
    public AudioClip NoticeAudioClip;
    public AudioClip SpawnAudioClip;
    public AudioClip HealAudioClip;
    public AudioClip OpenAudioClip;
    public AudioClip EquipAudioClip;

    private bool isMute = false;
	public bool IsMute
	{
		get{ return isMute; }
		private set
		{
			if( isMute == value ) return;
			isMute = value;
			if( bgmAudioSource != null ){
				if( isMute ) bgmAudioSource.volume = 0;
			}
		}
	}
	public bool MuteButton = false;

	private AudioClip bgmAudioClip;
	private AudioSource bgmAudioSource;
	public AudioSource[] effectAudioSource;
    public Dictionary<SOUND_EFFECT, AudioClip> dicAudioClip;

	private bool hasInit = false;
	private float bgmVolume = 0.8f;
	private float effectVolume = 1.0f;
	public float EffectVolum { get{ return effectVolume; } }

    void Awake()
    {
        Init();
    }

	void Init()
	{
		AddAudioSource();
        SetAudioClipDic();
		hasInit = true;
		
		Debug.Log("Sound Manager is Inited.");
	}
	
	// Update is called once per frame
	void Update () {	
		if( bgmAudioSource == null ) return;
		if( !bgmAudioSource.isPlaying ) {
			bgmAudioSource.Play();
		}
        if (bgmAudioSource.volume < bgmVolume - 0.01f || bgmAudioSource.volume > bgmVolume + 0.01f)
            bgmAudioSource.volume = Mathf.Clamp(bgmVolume, 0.0f, 1f);
	}

	void AddAudioSource() {
		bgmAudioSource = gameObject.AddComponent<AudioSource>();
		bgmAudioSource.playOnAwake = false;
		effectAudioSource = null;
		effectAudioSource = new AudioSource[EffectSoundPoolCount];
		for(int i=0; i<EffectSoundPoolCount; i++)
		{
			effectAudioSource[i] = gameObject.AddComponent<AudioSource>();
			effectAudioSource[i].playOnAwake = false;
		}

		SetVolume();         
	}

    void SetAudioClipDic()
    {
        dicAudioClip = new Dictionary<SOUND_EFFECT, AudioClip>();
        dicAudioClip.Add(SOUND_EFFECT.FIRE1, Fire1AudioClip);
        dicAudioClip.Add(SOUND_EFFECT.FIRE2, Fire2AudioClip);
        dicAudioClip.Add(SOUND_EFFECT.FIRE3, Fire3AudioClip);
        dicAudioClip.Add(SOUND_EFFECT.HIT1, Hit1AudioClip);
        dicAudioClip.Add(SOUND_EFFECT.HIT2, Hit2AudioClip);
        dicAudioClip.Add(SOUND_EFFECT.HIT3, Hit3AudioClip);
        dicAudioClip.Add(SOUND_EFFECT.CLICK, ClickAudioClip);
        dicAudioClip.Add(SOUND_EFFECT.ERROR, ErrorAudioClip);
        dicAudioClip.Add(SOUND_EFFECT.MISSION_CLEAR, MissionAudioClip);
        dicAudioClip.Add(SOUND_EFFECT.GET, ItemAudioClip);
        dicAudioClip.Add(SOUND_EFFECT.COIN, CoinAudioClip);
        dicAudioClip.Add(SOUND_EFFECT.WARNING, WarningAudioClip);
        dicAudioClip.Add(SOUND_EFFECT.SYS, SysAudioClip);
        dicAudioClip.Add(SOUND_EFFECT.MOVE1, Move1AudioClip);
        dicAudioClip.Add(SOUND_EFFECT.MOVE2, Move2AudioClip);
        dicAudioClip.Add(SOUND_EFFECT.MOVE3, Move3AudioClip);
        dicAudioClip.Add(SOUND_EFFECT.WIN, WinAudioClip);
        dicAudioClip.Add(SOUND_EFFECT.LOSE, LoseAudioClip);
        dicAudioClip.Add(SOUND_EFFECT.SUCCESS, SuccessAudioClip);
        dicAudioClip.Add(SOUND_EFFECT.FAIL, FailAudioClip);
        dicAudioClip.Add(SOUND_EFFECT.GAMEOVER, GameOverAudioClip);
        dicAudioClip.Add(SOUND_EFFECT.GAMECLEAR, GameClearAudioClip);
        dicAudioClip.Add(SOUND_EFFECT.SUMMON, SummonAudioClip);
        dicAudioClip.Add(SOUND_EFFECT.ENCHANT, EnchantAudioClip);
        dicAudioClip.Add(SOUND_EFFECT.DOWN_GRADE, DownGradeAudioClip);
        dicAudioClip.Add(SOUND_EFFECT.CHA_ENCHANT, ChaEnchantAudioClip);
        dicAudioClip.Add(SOUND_EFFECT.NOTICE, NoticeAudioClip);
        dicAudioClip.Add(SOUND_EFFECT.SPAWN, SpawnAudioClip);
        dicAudioClip.Add(SOUND_EFFECT.HEAL, HealAudioClip);
        dicAudioClip.Add(SOUND_EFFECT.OPEN, OpenAudioClip);
        dicAudioClip.Add(SOUND_EFFECT.EQUIP, EquipAudioClip);
    }

	AudioSource GetAudioSource()
	{
		for(int i=0; i<effectAudioSource.Length; i++)
		{
			if( !effectAudioSource[i].isPlaying ) {
				return effectAudioSource[i];
			}
		}

		return null;
	}

	public void SetVolume()
	{
		if(PlayerPrefs.HasKey("BGM_VOLUME")) bgmVolume = PlayerPrefs.GetFloat("BGM_VOLUME");
		if(PlayerPrefs.HasKey("EFFECT_VOLUME"))	effectVolume = PlayerPrefs.GetFloat("EFFECT_VOLUME");
		bgmAudioSource.volume = bgmVolume;
	}

    public void SetVolumeBGM(float value)
    {
        bgmVolume = value;
    }

    public void SetVolumeEffect(float value)
    {
        effectVolume = value;
    }

    public void MuteBGM(bool _mute)
    {
        bgmAudioSource.mute = _mute;
    }

    public void StopBGM()
    {
        if (bgmAudioSource != null && bgmAudioSource.isPlaying)
        {
            bgmAudioSource.Stop();
            bgmAudioClip = null;
#if UNITY_EDITOR || DEBUG_MODE
            Debug.Log("BGM 정지");
#endif
        }
        else
        {
#if UNITY_EDITOR || DEBUG_MODE
            Debug.Log("재생중인 BGM이 없음");
#endif
        }
    }

	public void PlaySound(SOUND_EFFECT type)
	{
		PlaySound(type,"");
	}                                                                    

	public void PlaySound(SOUND_EFFECT type,string name)
	{
		if( !hasInit ) Init ();
		AudioSource source = null;

        switch (type)
        {
            case SOUND_EFFECT.BGM:
                if (bgmAudioClip == null || !bgmAudioClip.name.Equals(name))
                {
                    AudioClip ac = Resources.Load("Sound/" + name) as AudioClip;
                    if (ac != null) bgmAudioClip = ac;

                    bgmAudioSource.Stop();
                    bgmAudioSource.clip = bgmAudioClip;
                    bgmAudioSource.volume = bgmVolume;
                    if (bgmAudioSource.mute) bgmAudioSource.mute = false;
                    bgmAudioSource.Play();
                }
                break;

            default:
                source = GetAudioSource();
                if (source != null && dicAudioClip.ContainsKey(type))
                {
                    source.clip = dicAudioClip[type];
                    source.volume = effectVolume;
                    source.Play();
                }
                break;
        }
	}

	public void SoundOnOff(SOUND onOff)
	{
		switch(onOff){
		case SOUND.ON: 
			if( !MuteButton ) IsMute = false;
			break;
		case SOUND.OFF:
			IsMute = true;
			break;
		}
	}

	public float BgmLength()
	{
		return ( bgmAudioClip != null ) ? bgmAudioClip.length : 0.0f;
	}

    public float EffectLength(SOUND_EFFECT type)
    {
        if (dicAudioClip.ContainsKey(type)) return dicAudioClip[type].length;
        return 0f;
    }
}
