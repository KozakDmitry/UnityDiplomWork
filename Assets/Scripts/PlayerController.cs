using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour, IPunObservable
{

    [HideInInspector]
    public PhotonView photonView;
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private Transform ladder;
    [SerializeField]
    private Sprite deathSprite;
    [SerializeField]
    private TextMeshPro nickName;
    [SerializeField]
    private Color colorOfNickname;
    private Transform lastTile;
    //change
    public Vector2Int direction;
    public Vector2Int gamePosition;
    public bool isDead;
    public int score = 0;

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
    public void Death()
    {
        isDead = true;
        spriteRenderer.sprite = deathSprite;
        SetLadderLength(0);
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
        photonView = GetComponent<PhotonView>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        gamePosition = new Vector2Int((int)transform.position.x, (int)transform.position.y);
        nickName.SetText(photonView.Owner.NickName);
        nickName.color = colorOfNickname;
        //change maybe
        FindObjectOfType<BoardManager>().AddPlayer(this);

        //if (!view.IsMine) spriteRenderer.sprite = otherPlayerSprite;
    }
    private void Update()
    {
        if (photonView.IsMine && !isDead)
        {
            if (Input.GetKey(KeyCode.LeftArrow))    {    direction = Vector2Int.left;   }
            if (Input.GetKey(KeyCode.RightArrow))   {    direction = Vector2Int.right;  }
            if (Input.GetKey(KeyCode.UpArrow))      {    direction = Vector2Int.up;     }
            if (Input.GetKey(KeyCode.DownArrow))    {    direction = Vector2Int.down;   }
          
        }

        if (direction == Vector2Int.left)   {    spriteRenderer.flipX = true;    }
        if (direction == Vector2Int.right)  {    spriteRenderer.flipX = false;   }


        //change
        transform.position = Vector3.Lerp(transform.position, (Vector2)gamePosition, Time.deltaTime * 3);
      
    }
   
   
}
