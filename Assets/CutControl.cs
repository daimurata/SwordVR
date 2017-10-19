using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutControl : MonoBehaviour {

    public int Count=2;

	void Start () {
		
	}
	
	
	void Update () {
        if (transform.position.y <= 0)
        {
            Debug.Log("消えます");
            Destroy(gameObject);
        }
    }
    
    public void CheckCutCount()
    {
    }
    

}
