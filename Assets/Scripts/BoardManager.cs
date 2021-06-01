using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardManager : MonoBehaviour, IOnEventCallback
{
    [SerializeField]
    private GameObject cellPrefab;
    private GameObject[,] cells;

    private List<PlayerController> players = new List<PlayerController>();

    private double lastTickTime;
    private RaiseEventOptions options;
    private SendOptions sendOptions;
    private Vector2Int[] directions;


    public void OnEvent(EventData photonEvent)
    {
        switch (photonEvent.Code)
        {
            case 13:
                directions = (Vector2Int[]) photonEvent.CustomData;
                PerformTick(directions);
                break;
        }
    }

    void Start()
    {
        cells = new GameObject[20, 10];
        for(int x = 0; x < cells.GetLength(0); x++)
        {
            for(int y = 0; y < cells.GetLength(1); y++)
            {
                cells[x, y] = Instantiate(cellPrefab, new Vector3(x, y), Quaternion.identity, transform);
            }
        }

    }

    public void AddPlayer(PlayerController pl)
    {
        players.Add(pl);
        cells[pl.position.x, pl.position.y].SetActive(false);
    }


    private void Update()
    {
        if(PhotonNetwork.Time >lastTickTime + 1  && PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount>=2)
        {

            directions = players
                .OrderBy(p => p.view.Owner.ActorNumber)
                .Select(p => p.direction)
                .ToArray();

            options = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
            sendOptions = new SendOptions { Reliability = true };
            PhotonNetwork.RaiseEvent(13, directions, options ,sendOptions);


            PerformTick(directions);
        }
    }

    private void PerformTick(Vector2Int[] directions)
    {
        if(players.Count != directions.Length) { return; }
        int i = 0;
        PlayerController[] sortedPlayers = players.OrderBy(p => p.view.Owner.ActorNumber).ToArray();
        foreach (var player in sortedPlayers)
        {
            player.direction = directions[i++];
            MinePlayer(player);
        }
        foreach (var player in sortedPlayers)
        {
            MovePlayer(player);
        }
        lastTickTime = PhotonNetwork.Time;
    }
    private void MinePlayer(PlayerController player)
    {

        Vector2Int targerPosition = player.direction + player.position;
        if (targerPosition.x < 0) { return; }
        if (targerPosition.y < 0) { return; }
        if (targerPosition.x >= cells.GetLength(0)) { return; }
        if (targerPosition.y >= cells.GetLength(1)) { return; }

        cells[player.position.x, player.position.y].SetActive(false);


        Vector2Int position = targerPosition;
        PlayerController minePlayer = players.First(p => p.view.IsMine);
        if (minePlayer != player) {
            while (position.y < cells.GetLength(1) && !cells[position.x, position.y].activeSelf)
            {
                if (position == minePlayer.position)
                {
                    PhotonNetwork.LeaveRoom();
                    break;
                   
                }
                position.y++;
            }
        }
    }
    private void MovePlayer(PlayerController player)
    {
        player.position += player.direction;
        if (player.position.x < 0) { player.position.x = 0; }
        if (player.position.y < 0) { player.position.y = 0; }
        if (player.position.x >= cells.GetLength(0)) { player.position.x = cells.GetLength(0) - 1; }
        if (player.position.y >= cells.GetLength(1)) { player.position.y = cells.GetLength(1) - 1; }


        int ladderLength = 0;
        Vector2Int position = player.position;
        while (position.y > 0 && cells[position.x, position.y - 1].activeSelf)
        {
            ladderLength++;
            position.y--;
        }

        player.SetLadderLength(ladderLength);
    }
    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }
    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }
}
