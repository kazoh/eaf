using UnityEngine;
using System.Collections;

public class TestRoundMove : MonoBehaviour {

    public float r;
    public float w;
    public int Direct;

    private float t;

	// Use this for initialization
	void Start () {
        if (Direct < 0) Direct = -1;
        else Direct = 1;
	}
	
	// Update is called once per frame
	void Update () {

        t += Time.deltaTime;
        transform.position = new Vector3(GetX(), GetY(), transform.position.z);	
	}

    // theta = wt
    // sin(wt) = y / r
    // cos(wt) = x / r
    float GetX()
    {
        return r * Mathf.Cos(w*t) * Direct;
    }

    float GetY()
    {
        return r * Mathf.Sin(w*t);
    }
}
