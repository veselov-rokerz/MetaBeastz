using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PhotonLobby : MonoBehaviourPunCallbacks
{
    public static PhotonLobby lobby;

    public GameObject playButton;
    public GameObject cancelButton;

    private void Awake()
    {
        lobby = this; // Creates the Singleton, lives within the Game Scene
    }


    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings(); // Connects to master photon server.
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPlayButtonClicked()
    {
        playButton.SetActive(false);
        cancelButton.SetActive(true);
        PhotonNetwork.JoinRandomRoom();
    }
    public void OnCancelButtonClicked()
    {
        cancelButton.SetActive(false);
        playButton.SetActive(true);        
        PhotonNetwork.LeaveRoom();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Player connected to master server");
        playButton.SetActive(true);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Player connected to room");
        //SceneManager.LoadScene("GamePlay");
        PhotonNetwork.LoadLevel("GamePlay");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        //base.OnJoinRandomFailed(returnCode, message);
        Debug.Log("Error["+ returnCode + "] =>"+" Player connected to master server due to : " + message);
        CreateRoom();
    }

    void CreateRoom()
    {
        int randomRoomName = Random.Range(0, 10000);
        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = 2 };
        PhotonNetwork.CreateRoom("Room" + randomRoomName, roomOps);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        //base.OnCreateRoomFailed(returnCode, message);
        Debug.Log("Error[" + returnCode + "] =>" + " An attempt to create room failed due to : " + message);
        CreateRoom();
    }
}
