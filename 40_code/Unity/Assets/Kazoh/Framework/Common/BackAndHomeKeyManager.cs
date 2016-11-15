using UnityEngine;
using System.Collections;

public class BackAndHomeKeyManager : MonoBehaviour {

    public string FuncName;
	
	// Update is called once per frame
	void Update () {               
        if (Input.GetKeyDown(KeyCode.Escape)) SendMessage(FuncName);    	
	}
}
