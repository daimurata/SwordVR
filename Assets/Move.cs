using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    float MoveSpeed = 0.1f;

	void Update ()
    {
        Vector3 movepos = transform.position;

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            movepos.x -= MoveSpeed;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            movepos.x += MoveSpeed;
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            movepos.y += MoveSpeed;
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            movepos.y -= MoveSpeed;
        }

        transform.position = movepos;
    }
}