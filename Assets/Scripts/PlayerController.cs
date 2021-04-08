using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private void Update()
    {
        if (this.gameObject.activeSelf)
        {
            return;
        }

        int horizontal = 0, vertical = 0;

        horizontal = (int)(Input.GetAxisRaw("Horizontal"));
        vertical = (int)(Input.GetAxisRaw("Vertical"));

        if (horizontal != 0)
        {
            vertical = 0;
        }
        
        if(horizontal!=0 || vertical != 0)
        {
            TryToMove(horizontal, vertical);
        }
    }





    private void TryToMove(int horizontal, int vertical)
    {

    }


    private void OnTriggerEnter2D(Collider2D other)
    {

    }
}
