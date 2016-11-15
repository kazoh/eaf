using UnityEngine;
using System.Collections;

public class TestResult : MonoBehaviour
{
    public Transform Goal;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void LateUpdate ()
    {

        if (Input.GetMouseButtonDown(0))
        {
            switch (GetResult())
            {
                case 0: print("Perfect!!!"); break;
                case 1: print("Good!!!"); break;
                case 2: print("Cool!!!"); break;
                default: print("Bad!!!"); break;
            }
        }

    }

    int GetResult()
    {
        float size = Goal.localScale.x;
        Vector2 v1 = new Vector2(transform.position.x, transform.position.y);
        Vector2 v2 = new Vector2(Goal.position.x, Goal.position.y);
        float dist = Vector2.Distance(v1, v2);

        if (dist < size * 0.1f) return 0;
        if (dist < size * 0.5f) return 1;
        if (dist < size) return 2;
        return 3;
    }
}
