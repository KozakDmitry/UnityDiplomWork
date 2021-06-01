using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject PlayerPrefab;
    private Vector3 position;
    // Start is called before the first frame update
    void Start()
    {
        position = new Vector3(UnityEngine.Random.Range(1, 15), UnityEngine.Random.Range(1, 5));
        PhotonNetwork.Instantiate(PlayerPrefab.name,position,Quaternion.identity);
        PhotonPeer.RegisterType(typeof(Vector2Int), 200,SerializeVector2Int , DeserializeVector2Int);
    }


    // Update is called once per frame
    void Update()
    {
       
    }

    public override void OnLeftRoom()
    {
       
        SceneManager.LoadScene(0);


    }
    public void Leave()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.LogFormat("Player {0} entered Room", newPlayer.NickName);

    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.LogFormat("Player {0} left Room", otherPlayer.NickName);
    }

    public static object DeserializeVector2Int(byte[] data)
    {
        Vector2Int result = new Vector2Int();

        result.x = BitConverter.ToInt32(data,0);
        result.y = BitConverter.ToInt32(data, 1);
        return result;
    }
    public static byte[] SerializeVector2Int(object obj)
    {
        byte[] result = new byte[8];
        Vector2Int vector = (Vector2Int)obj;
        BitConverter.GetBytes(vector.x).CopyTo(result,0);
        BitConverter.GetBytes(vector.y).CopyTo(result, 1);
        return result;
    }
}
