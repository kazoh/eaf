using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using Kazoh.Table;

public class Component_UI_PopupEffect : GameComponent
{
    public enum EffectType
    {
        Enchant,
        ChaEnchant,
        DownGrade,
        Summon,
    }

    public UILabel TitleLabel;
    public UIButton ClostBtn;
    public UILabel BtnLabel;
    public UISprite[] ChaSpriteArray;
    public UISprite EffectSprite;
    public UILabel ResultLabel;
    public UILabel MsgLabel;

    private UISprite chaSprite;
    private Animation anim;

    public override void Init()
    {
        base.Init();

        /* 텍스트 설정 */
        BtnLabel.text = TableManager.GetString("STR_UI_CLOSE");

        /* 캐릭터 스프라이트 설정 */
        chaSprite = ChaSpriteArray[2];

        /* 애니메이션 설정 */
        anim = gameObject.GetComponent<Animation>();
    }

    public void Show(EffectType _type, string _spriteName,string _title, string _result, string _msg, bool _success = false)
    {
        TitleLabel.text = _title;
        chaSprite.spriteName = _spriteName + "_02";
        ResultLabel.text = _result;
        MsgLabel.text = _msg;

        EffectSprite.alpha = 0f;
        ResultLabel.alpha = 0f;
        MsgLabel.alpha = 0f;

        for (int i = 0; i < ChaSpriteArray.Length; ++i)
        {
            ChaSpriteArray[i].alpha = 0f;
        }

        ClostBtn.gameObject.SetActive(false);

        Show();
        switch(_type)
        {
            case EffectType.Enchant:
                StartCoroutine(PlayEnchantEffect(_success));
                break;

            case EffectType.ChaEnchant:
                StartCoroutine(PlayEffect(SOUND_EFFECT.CHA_ENCHANT, "cha_enchant", 30, 0.05f));
                break;

            case EffectType.DownGrade:
                StartCoroutine(PlayEffect(SOUND_EFFECT.DOWN_GRADE, "down_grade", 25, 0.05f));
                break;

            default:
                Hide();
                break;
        }
    }

    public void Show(EffectType _type, string[] _spriteNames, string _title, string _result, string _msg, bool _success = false)
    {
        TitleLabel.text = _title;
        ResultLabel.text = _result;
        MsgLabel.text = _msg;

        for (int i = 0; i < ChaSpriteArray.Length; ++i)
        {
            if(i < _spriteNames.Length) ChaSpriteArray[i].spriteName = _spriteNames[i] + "_02";
            ChaSpriteArray[i].alpha = 0f;
        }

        EffectSprite.alpha = 0f;
        ResultLabel.alpha = 0f;
        MsgLabel.alpha = 0f;
        ClostBtn.gameObject.SetActive(false);

        Show();
        switch (_type)
        {
            case EffectType.Summon:
                if(_spriteNames.Length > 1) StartCoroutine(PlayMultySummonEffect(SOUND_EFFECT.SUMMON, "summon", 15, 0.1f));
                else StartCoroutine(PlayEffect(SOUND_EFFECT.SUMMON, "summon", 15, 0.1f));
                break;
        }
    }

    IEnumerator PlayEffect(SOUND_EFFECT _sound, string _sprite, int _spriteNum, float _delay)
    {
        GameProcess.PlaySound(_sound);
        EffectSprite.spriteName = _sprite + "_01";
        EffectSprite.alpha = 1f;
        for(int i=2; i<_spriteNum+1; ++i)
        {
            yield return new WaitForSeconds(_delay);
            EffectSprite.spriteName = string.Format(_sprite+"_{0:00}", i);
        }
        yield return new WaitForSeconds(0.1f);
        EffectSprite.alpha = 0f;
        GameProcess.PlaySound(SOUND_EFFECT.SUCCESS);
        chaSprite.width = 64;
        chaSprite.height = 64;
        chaSprite.alpha = 1f;
        ResultLabel.alpha = 1f;
        MsgLabel.alpha = 1f;
        ClostBtn.gameObject.SetActive(true);
    }

    IEnumerator PlayEnchantEffect(bool _success)
    {
        EffectSprite.spriteName = "enchant_01";
        EffectSprite.alpha = 1f;
        yield return new WaitForSeconds(0.1f);
        for (int i = 1; i < 9; ++i)
        {
            if(i%3 == 1) GameProcess.PlaySound(SOUND_EFFECT.ENCHANT);
            EffectSprite.spriteName = string.Format("enchant_{0:00}", i%3+1);
            if (i % 3 == 2) yield return new WaitForSeconds(0.3f);
            else yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(0.3f);
        if (_success) GameProcess.PlaySound(SOUND_EFFECT.SUCCESS);
        else GameProcess.PlaySound(SOUND_EFFECT.FAIL);
        ResultLabel.alpha = 1f;
        MsgLabel.alpha = 1f;
        ClostBtn.gameObject.SetActive(true);
    }

    IEnumerator PlayMultySummonEffect(SOUND_EFFECT _sound, string _sprite, int _spriteNum, float _delay)
    {
        GameProcess.PlaySound(_sound);
        EffectSprite.spriteName = _sprite + "_01";
        EffectSprite.alpha = 1f;
        for (int i = 2; i < _spriteNum + 1; ++i)
        {
            yield return new WaitForSeconds(_delay);
            EffectSprite.spriteName = string.Format(_sprite + "_{0:00}", i);
        }
        yield return new WaitForSeconds(0.1f);
        EffectSprite.alpha = 0f;

        chaSprite.width = 32;
        chaSprite.height = 32;

        for (int i = 0; i < ChaSpriteArray.Length; ++i)
        {
            GameProcess.PlaySound(SOUND_EFFECT.SPAWN);
            ChaSpriteArray[i].alpha = 1f;
            yield return new WaitForSeconds(GameProcess.GetEffectLength(SOUND_EFFECT.SPAWN));
        }

        GameProcess.PlaySound(SOUND_EFFECT.SUCCESS);
        ResultLabel.alpha = 1f;
        MsgLabel.alpha = 1f;
        ClostBtn.gameObject.SetActive(true);
    }

    public void OnClick_Close()
    {
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        Hide();
    }
}
