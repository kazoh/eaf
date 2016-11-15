using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class HttpManager : MonoBehaviour {

    public struct RequestStruct
    {
        public string Url;
        public Action<WWW> Callback;
        public bool IsLoader;

        public RequestStruct(string url, Action<WWW> callback, bool isLoader = false)
        {
            Url = url;
            Callback = callback;
            IsLoader = isLoader;
        }
    }

    private Queue<RequestStruct> requests;

    public void Init()
    {
        requests = new Queue<RequestStruct>();
    }

    public void CallRequest(string _url, bool _loader, Action<WWW> _callback)
    {
        requests.Enqueue(new RequestStruct(_url, _callback, _loader));
    }
	
	// Update is called once per frame
	void Update () {	
        if(requests != null && requests.Count > 0)
        {
            StartCoroutine(Request(requests.Dequeue()));
        }
	}

    IEnumerator Request(RequestStruct _request)
    {
        if(_request.IsLoader) GameProcess.ShowServerLoading();

        WWW www = new WWW(_request.Url);
        yield return www;

        if (www.error == null)
        {
            if (_request.Callback != null) _request.Callback(www);
        }
        else
        {
            Debug.LogError(www.error);
            GameProcess.ShowError(new GameException(GameException.ErrorCode.UnknownServerError));
        }

        if(_request.IsLoader) GameProcess.HideServerLoading();
    }
}
