using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IPunObservable
{

    [HideInInspector]
    public PhotonView view;
    private SpriteRenderer spriteRenderer;
    private Transform ladder;
    private Transform lastTile;
    //change
    public Vector2Int direction;
    public Vector2Int position;


    public void SetLadderLength(int length)
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            ladder.GetChild(i).gameObject.SetActive(i < length);

        }

        while (ladder.childCount < length)
        {
            lastTile = ladder.GetChild(ladder.childCount - 1);
            Instantiate(lastTile, lastTile.position + Vector3.down, Quaternion.identity, ladder) ;
        }
    }

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
        spriteRenderer = GetComponent<SpriteRenderer>();

        position = new Vector2Int((int)transform.position.x, (int)transform.position.y);

        //change maybe
        FindObjectOfType<BoardManager>().AddPlayer(this);

        //if (!view.IsMine) spriteRenderer.sprite = otherPlayerSprite;
    }
    private void Update()
    {
        if (view.IsMine)
        {
            if (Input.GetKey(KeyCode.LeftArrow))    {    direction = Vector2Int.left;   }
            if (Input.GetKey(KeyCode.RightArrow))   {    direction = Vector2Int.right;  }
            if (Input.GetKey(KeyCode.UpArrow))      {    direction = Vector2Int.up;     }
            if (Input.GetKey(KeyCode.DownArrow))    {    direction = Vector2Int.down;   }
          
        }

        if (direction == Vector2Int.left)   {    spriteRenderer.flipX = true;    }
        if (direction == Vector2Int.right)  {    spriteRenderer.flipX = false;   }


        //change
        transform.position = Vector3.Lerp(transform.position, (Vector2)position, Time.deltaTime * 3);
      
    }
   
   
}
