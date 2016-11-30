using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using Kazoh.Table;

public class Component_UI_RankPopup : GameComponent
{
    public UILabel TitleLabel;
    public UILabel CancelBtnLabel;
    public UILabel ScoreLabel;
    public UIButton SendButton;
    public UILabel SendBtnLabel;
    public UIScrollView ScrollView;
    public UIGrid Grid;
    public GameObject pfRankItem;
    public Component_UI_InputName InputUI;

    private List<Component_Item_Rank> listRank;

    public override void Init()
    {
        base.Init();

        /* 스크롤 뷰 패널 뎁스 설정 */
        UIPanel panel = transform.parent.GetComponent<UIPanel>();
        if (panel != null)
        {
            ScrollView.panel.depth = panel.depth + 1;
        }

        /* 이니셜 입력 UI 숨김 */
        InputUI.Hide();

        /* UI 텍스트 설정 */
        TitleLabel.text = TableManager.GetString("STR_TITLE_RANK");
        CancelBtnLabel.text = TableManager.GetString("STR_UI_CLOSE");
        SendBtnLabel.text = TableManager.GetString("STR_UI_SYNC");
        SendButton.gameObject.SetActive(false);

        /* 리스트 설정 */
        listRank = new List<Component_Item_Rank>();
        for(int i=0; i < 10; ++i)
        {
            GameObject go = NGUITools.AddChild(Grid.gameObject, pfRankItem);
            Component_Item_Rank component = go.GetComponent<Component_Item_Rank>();
            if (component != null)
            {
                component.Init(i, null);
                listRank.Add(component);
            }
        }
        Grid.Reposition();
    }

    public void OnClick_Close()
    {
        if (gameObject.activeSelf)
        {
            GameProcess.PlaySound(SOUND_EFFECT.CLICK);
            Hide();
        }
    }

    public void OnClick_Sync()
    {
        if (isLock) return;
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);

        int id = GameProcess.GetGameDataManager().GetGUID();
        int chaId = GameProcess.GetGameDataManager().GetCurCharacter().ChaId;
        string name = GameProcess.GetGameDataManager().GetUserInitial();
        int score = GameProcess.GetGameDataManager().GetScore();

        string title = TableManager.GetString("STR_TITLE_NAME");
        string msg = TableManager.GetString("STR_MSG_NAME");
        string text1 = TableManager.GetString("STR_UI_SYNC");
        string text2 = TableManager.GetString("STR_UI_CANCEL");
        InputUI.Show(title, msg, text1, text2, name, delegate (string _newName)
        {
            try
            {
                GameProcess.GetGameDataManager().SetUserInitial(_newName);
                DBManager.UpdateRankData(id, _newName, chaId, score, delegate (bool _isFail, string _data)
                {
                    if (_isFail) GameProcess.ShowError(new GameException(GameException.ErrorCode.CanNotGetRankData));
                    else
                    {
                        title = TableManager.GetString("STR_TITLE_NOTICE");
                        msg = TableManager.GetString("STR_MSG_SYNCED");
                        text1 = TableManager.GetString("STR_UI_OK");
                        GameProcess.ShowPopup(NoticeType.OK, title, msg, text1, null);
                        
                        SendButton.gameObject.SetActive(false);
                        UpdateList(_data);
                    }
                });
            }
            catch (GameException e)
            {
                GameProcess.ShowError(e);
            }
            catch (Exception e)
            {
                GameProcess.ShowError(new GameException(GameException.ErrorCode.Unknown, e.Message));
            }
        }, null);

        isLock = false;
    }

    bool isLock;
    public override void Show()
    {
        try
        {
            if (isLock) return;
            isLock = true;

            // 갱신 버튼을 꺼 둔다.
            SendButton.gameObject.SetActive(false);

            DBManager.GetRankData(delegate (bool _isFail, string _data)
            {
                try
                {
                    if (_isFail) throw new GameException(GameException.ErrorCode.CanNotGetRankData);
                    UpdateList(_data);
                }
                catch (GameException e)
                {
                    GameProcess.ShowError(e);
                }
                catch (Exception e)
                {
                    GameProcess.ShowError(new GameException(GameException.ErrorCode.Unknown, e.Message));
                }
                finally
                {
                    isLock = false;
                }
            });
        }
        catch(GameException e)
        {
            GameProcess.ShowError(e);
        }
        catch(Exception e)
        {
            GameProcess.ShowError(new GameException(GameException.ErrorCode.Unknown, e.Message));
        }
    }

    void UpdateList(string _data)
    {
        IList list = null;
        if (!string.IsNullOrEmpty(_data)) list = MiniJSON.Json.Deserialize(_data) as IList;

        // 내 기록을 설정한다. 
        int id = GameProcess.GetGameDataManager().GetGUID();
        int chaId = GameProcess.GetGameDataManager().GetCurCharacter().ChaId;
        string name = GameProcess.GetGameDataManager().GetUserInitial();
        int score = GameProcess.GetGameDataManager().GetScore();
        Data_Rank myRank = new Data_Rank(id, chaId, name, score);

        ScoreLabel.text = string.Format("{0:###,##0}", myRank.Record);

        List<Data_Rank> rankList = new List<Data_Rank>();
        if (list != null)
        {
            foreach (IDictionary dict in list)
            {
                rankList.Add(new Data_Rank(dict));
            }
        }

        int preRankIdx = rankList.FindIndex(x => x.Id == myRank.Id);
        if (preRankIdx > -1)
        {
            if (rankList[preRankIdx].Record < myRank.Record) rankList[preRankIdx] = myRank;
        }
        else rankList.Add(myRank);

        rankList.Sort((x, y) => y.Record.CompareTo(x.Record));

        int preScore = int.MaxValue;
        int rank = 0;
        for (int i = 0; i < listRank.Count; ++i)
        {
            if (i < rankList.Count)
            {
                if(preScore > rankList[i].Record)
                {
                    preScore = rankList[i].Record;
                    rank++;
                }
                listRank[i].Init(rank, rankList[i]);
                if (rankList[i] == myRank) SendButton.gameObject.SetActive(true);
            }
            else listRank[i].Init(rank, null);
        }
        base.Show();
        Grid.Reposition();
        ScrollView.ResetPosition();
    }
}
