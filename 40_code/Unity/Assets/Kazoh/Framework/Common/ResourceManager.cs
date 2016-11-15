using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ResourceManager : MonoBehaviour {

    private Dictionary<string, Texture> dicTexture;

    public void Init()
    {
        dicTexture = new Dictionary<string, Texture>();
    }

    public Texture GetMapImg(string name)
    {
        if (dicTexture.ContainsKey(name)) return dicTexture[name];
        Texture tex = Resources.Load("Texture/Map/" + name) as Texture;
        if (tex != null)
        {
            dicTexture.Add(name, tex);
        }

        return tex;
    }

    public GameObject GetPrefabs(string name)
    {
        GameObject go = Resources.Load("Map/" + name) as GameObject;
        if (go != null)
        {
        }

        return go;
    }

    public void UnloadResource(Object _resource)
    {
        Resources.UnloadAsset(_resource);
    }
}
