using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IPunObservable
{
    private PhotonView view;
    private SpriteRenderer spriteRenderer;

    private bool isRed;
    private Vector2Int direction;


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(direction);
        }
        else
        {
            direction = (Vector2Int) stream.ReceiveNext();
        }
    }

    private void Start()
    {
        view = GetComponent<PhotonView>();
    }
    private void Update()
    {
        if (view.IsMine)
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                direction = Vector2Int.left;
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                direction = Vector2Int.right;
            }
            if (Input.GetKey(KeyCode.UpArrow))
            {
                direction = Vector2Int.up;
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                direction = Vector2Int.down;
            }
          
        }

        if (direction == Vector2Int.left)
        {
            spriteRenderer.flipX = true;
        }
        if(direction == Vector2Int.right)
        {
            spriteRenderer.flipX = false;
        }

      
    }
   
   
}
