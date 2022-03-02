using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class Conn : MonoBehaviourPunCallbacks {

    [SerializeField] private GameObject initPanel, loginPanel;
    [SerializeField] private InputField nickname;
    [SerializeField] private GameObject player;

    public static Conn instance;

    void Awake() {
        if(instance == null) {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    void Start() {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void InitGame() {
        PhotonNetwork.ConnectUsingSettings();
        initPanel.SetActive(false);
        loginPanel.SetActive(true);
    }

    public void CreateRoomServer() {
        if(nickname.text != null && nickname.text != "") {
            PhotonNetwork.JoinOrCreateRoom("ServerTest", new RoomOptions(), TypedLobby.Default);
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Conectado!");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Lobby!!");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Conexão perdida!");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Não entrou me nenhuma sala!");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Entrei em uma sala!");
        print(PhotonNetwork.CurrentRoom.Name);
        print(PhotonNetwork.CurrentRoom.PlayerCount);
        //loginPanel.SetActive(false);
        //PhotonNetwork.Instantiate(player.name, new Vector3(0, Random.Range(1, 8, 0)), Quaternion.Euler(45, 45, 45), 0);
        if(PhotonNetwork.IsMasterClient) {
            PhotonNetwork.LoadLevel(1);
        }
    }
}
