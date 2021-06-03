﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{

    public Text LogText;
    public InputField nickNameInput;
    private string nickName;
    void Start()
    {
        nickName = PlayerPrefs.GetString("NickName", "Player " + Random.Range(1000, 9999));

        PhotonNetwork.NickName = nickName;
        nickNameInput.text = nickName;
        
        Log("Player name is set to " + PhotonNetwork.NickName);


        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = "1";
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Log("Connected to Master");
    }
    public void CreateRoom()
    {
        PhotonNetwork.NickName = nickNameInput.text;
        PlayerPrefs.SetString("NickName", nickNameInput.text);
        PhotonNetwork.CreateRoom(null, new Photon.Realtime.RoomOptions { MaxPlayers = 3 });
        Log("Created the Room");
    }

    public void JoinRoom()
    {
        PhotonNetwork.NickName = nickNameInput.text;
        PlayerPrefs.SetString("NickName", nickNameInput.text);
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedRoom()
    {
        Log("Joined the Room");
        PhotonNetwork.LoadLevel("Game");
    }
    private void Log(string message)
    {
        Debug.Log(message);
        LogText.text += "\n";
        LogText.text += message;
    }


}
