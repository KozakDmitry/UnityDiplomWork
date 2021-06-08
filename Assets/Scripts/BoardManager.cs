using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardManager : MonoBehaviour, IOnEventCallback
{
    private bool[,] cells;
    [SerializeField]
    private PanelManager panelManager;
    public List<PlayerController> players = new List<PlayerController>();

    private double lastTickTime;
    private RaiseEventOptions options;
    private SendOptions sendOptions;
    private Vector2Int[] directions;


    [SerializeField]
    private TileBase stonecellTile;
    [SerializeField]
    private TileBase grassCellTile;
    [SerializeField]
    private TileBase dirtCellTile;

    [SerializeField]
    private Tilemap tilemap;
    [SerializeField]
    private TileBase rockTile;
    public void OnEvent(EventData photonEvent)
    {
        switch (photonEvent.Code)
        {
            case 13:
                directions = (Vector2Int[]) photonEvent.CustomData;
                PerformTick(directions);
                break;
            case 15:
                var data = (SyncData)photonEvent.CustomData;
                StartCoroutine( OnSyncDataReceived(data));
                break;
        }
    }



    public void SetCell(Vector2Int pos, bool set)
    {
        cells[pos.x, pos.y] = set;
        TileBase cell;
        if (!set)
        {
            cell = null;

        }
        else if (pos.y < 10)
        {
            cell = stonecellTile;
        }
        else if (pos.y<20)
        {
            cell = dirtCellTile;
        }
        else
        {
            cell = grassCellTile;
        }
        tilemap.SetTile((Vector3Int)pos,cell); 
    }

    //return sync data to players
    private IEnumerator OnSyncDataReceived(SyncData data)
    {
        PlayerController[] sortedPlayers;
        do
        {
            yield return null;
            sortedPlayers = players
             .Where(p => !p.isDead)
             .Where(p => !p.photonView.IsMine)
             .OrderBy(p => p.photonView.Owner.ActorNumber)
             .ToArray();
        } while (sortedPlayers.Length != data.positions.Length);
        for(int i = 0; i < sortedPlayers.Length; i++)
        {
            sortedPlayers[i].gamePosition = data.positions[i];
            sortedPlayers[i].score = data.scores[i];
            sortedPlayers[i].transform.position = (Vector2)sortedPlayers[i].gamePosition;
        }

        for(int x = 0; x < cells.GetLength(0); x++)
        { 
            for(int y = 0; y < cells.GetLength(1); y++)
            {
                bool cellActive = data.mapData.Get(x + y * cells.GetLength(0));
                if (!cellActive) SetCell(new Vector2Int(x, y), false);
            }
        }
    }

    void Start()
    {
        cells = new bool[Constants.fieldHeigth, Constants.fieldWidth];
        for(int x = 0; x < cells.GetLength(0); x++)
        {
            for(int y = 0; y < cells.GetLength(1); y++)
            {
                SetCell(new Vector2Int(x, y), true);
            }
        }

        for (int x = 0; x < cells.GetLength(0); x++)
        {
            tilemap.SetTile(new Vector3Int(x, -1, 0), rockTile);
            tilemap.SetTile(new Vector3Int(x, cells.GetLength(1), 0), rockTile);
        }
        for (int y = 0; y < cells.GetLength(1); y++)
        {
            tilemap.SetTile(new Vector3Int(-1, y, 0), rockTile);
            tilemap.SetTile(new Vector3Int(cells.GetLength(0), y, 0), rockTile);
        }
    }

    public void AddPlayer(PlayerController pl)
    {
        players.Add(pl);
        SetCell(pl.gamePosition, false);
    }


    private void Update()
    {
        //CheckTurnTime
        if(PhotonNetwork.Time >lastTickTime + 1  && 
            PhotonNetwork.IsMasterClient && 
            PhotonNetwork.CurrentRoom.PlayerCount>=2)
        {

            
            directions = players
                .Where(p=>!p.isDead)
                .OrderBy(p => p.photonView.Owner.ActorNumber)
                .Select(p => p.direction)
                .ToArray();

            //Determine players to check and send signal
            options = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
            sendOptions = new SendOptions { Reliability = true };
            PhotonNetwork.RaiseEvent(13, directions, options ,sendOptions);


            PerformTick(directions);
        }
    }

    //Making a move from everyone
    private void PerformTick(Vector2Int[] directions)
    {
        if(players.Count != directions.Length) { return; }
 
        PlayerController[] sortedPlayers = 
            players
            .Where(p => !p.isDead)
            .OrderBy(p => p.photonView.Owner.ActorNumber)
            .ToArray();
        int i = 0;
        foreach (var player in sortedPlayers)
        {
            player.direction = directions[i++];
            MinePlayer(player);
        }
        foreach (var player in sortedPlayers)
        {
            MovePlayer(player);
        }
        foreach (var player in players.Where(p => !p.isDead))
        {
            Vector2Int testPosition = player.gamePosition;
            while (testPosition.y > 0 && !cells[testPosition.x, testPosition.y - 1])
            {
                testPosition.y--;
            }
            player.gamePosition = testPosition;
        }
        panelManager.setTexts(players);
        lastTickTime = PhotonNetwork.Time;
    }

    //Syncronise data, making a array of data
    public void SendSyncData(Player player)
    {
        SyncData data = new SyncData();

        data.positions = new Vector2Int[players.Count];
        data.scores = new int[players.Count];


        PlayerController[] sortedPlayers = players
            .Where(p => !p.isDead)
            .OrderBy(p => p.photonView.Owner.ActorNumber)
            .ToArray();
        for(int i =0; i < sortedPlayers.Length; i++)
        {
            data.positions[i] = sortedPlayers[i].gamePosition;
            data.scores[i] = sortedPlayers[i].score;
        }

        //change
        data.mapData = new BitArray(Constants.fieldWidth*Constants.fieldWidth);
        for(int x = 0; x < cells.GetLength(0); x++)
        {
            for(int y = 0; y < cells.GetLength(1); y++)
            {
                data.mapData.Set(x + y * cells.GetLength(0), cells[x, y]);
            }
        }
        options = new RaiseEventOptions { TargetActors = new[] {player.ActorNumber } };
        sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(13, directions, options, sendOptions);

        PhotonNetwork.RaiseEvent(15, data, options, sendOptions);
    }


    //Dig and check who was killed
    private void MinePlayer(PlayerController player)
    {
        if (player.direction == Vector2Int.zero) return;
        //Digging
        Vector2Int targerPosition = player.gamePosition + player.direction;
        if (targerPosition.x < 0) { return; }
        if (targerPosition.y < 0) { return; }
        if (targerPosition.x >= cells.GetLength(0)) { return; }
        if (targerPosition.y >= cells.GetLength(1)) { return; }

        if(cells[targerPosition.x, targerPosition.y])
        {
            SetCell(targerPosition, false);
            player.score++;
        }
        


        //CheckKill
        Vector2Int testPosition = targerPosition;
        PlayerController minePlayer = players.First(p => p.photonView.IsMine);
        if (minePlayer != player) {
            while (testPosition.y < cells.GetLength(1) && !cells[testPosition.x, testPosition.y])
            {
                if (testPosition == minePlayer.gamePosition)
                {
                    PhotonNetwork.LeaveRoom();
                    break;
                   
                }
                testPosition.y++;
            }
        }
    }

    //Moving player
    private void MovePlayer(PlayerController player)
    {
        //Moving
        player.gamePosition += player.direction;
        if (player.gamePosition.x < 0) { player.gamePosition.x = 0; }
        if (player.gamePosition.y < 0) { player.gamePosition.y = 0; }
        if (player.gamePosition.x >= cells.GetLength(0)) { player.gamePosition.x = cells.GetLength(0) - 1; }
        if (player.gamePosition.y >= cells.GetLength(1)) { player.gamePosition.y = cells.GetLength(1) - 1; }


        int ladderLength = 0;
        Vector2Int testPosition = player.gamePosition;
        while (testPosition.y > 0 && !cells[testPosition.x, testPosition.y - 1])
        {
            ladderLength++;
            testPosition.y--;
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
