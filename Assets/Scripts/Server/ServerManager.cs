using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class ServerManager : MonoBehaviourPunCallbacks
{
    #region PRIVATE_PROPERTIES
    [SerializeField] private string _playerPrefabName = "PlayerPrefab";
    [SerializeField] private string _magicBallName = "MagicBall";
    [SerializeField] private Transform[] _spawnPoints;
    private Dictionary<Player, Character> _playersDic = new Dictionary<Player, Character>();
    private Dictionary<Character, Player> _charactersDic = new Dictionary<Character, Player>();
    private Player _server;
    private int _playerCount;
    private bool _isGameStarted;
    #endregion

    #region PUBLIC_PROPERTIES
    public Player GetServer => _server;
    public bool IsServer => PhotonNetwork.IsMasterClient;
    public bool IsGameStarted => _isGameStarted;

    public Dictionary<Player, Character> PlayersDic => _playersDic;
    public Dictionary<Character, Player> CharactersDic => _charactersDic;
    #endregion

    #region UNITY_EVENTS
    private void Awake()
    {
        _server = PhotonNetwork.MasterClient;
    }
    private void Start()
    {
        PrintPlayers();
        _isGameStarted = true;
    }
    #endregion

    #region PHOTON_RELATED
    public void RPC(string name, params object[] input)
    {
        photonView.RPC(name, _server, input);
    }
    private void CreatePlayer(Player client)
    {
        var obj = PhotonNetwork.Instantiate(_playerPrefabName, _spawnPoints[_playersDic.Count].position, Quaternion.identity);
        var character = obj.GetComponent<Character>();
        character.ChangeName(client);
        character.ServerManager = this;
        character.LocalPlayer = client;
        if (character != null)
        {
            _playersDic[client] = character;
            _charactersDic[character] = client;
        }
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (IsServer)
        {
            if (_playersDic.ContainsKey(otherPlayer))
            {
                PhotonNetwork.Destroy(_playersDic[otherPlayer].gameObject);
            }
        }
    }
    #endregion

    #region REQUESTS
    [PunRPC] // Hago un pedido de conexión
    private void RequestConnect(Player player)
    {
        CreatePlayer(player);
    }
    [PunRPC] // Pedido de Movimiento
    private void RequestMove(Player client, Vector3 dir)
    {
        _playersDic[client].Move(dir);
    }
    [PunRPC] // Pedido de Rotación
    private void RequestRotation(Player client, float degrees)
    {
        _playersDic[client].Rotate(degrees);
    }
    [PunRPC] // Pedido de Ataque
    private void RequestAttack(Player client)
    {
        var playerTransform = _playersDic[client].transform;
        Vector3 compensation = new Vector3(0, 1, 0);
        var magicBall = PhotonNetwork.Instantiate(_magicBallName, playerTransform.position + compensation, playerTransform.localRotation);
        var temp = magicBall.GetComponent<MagicBall>();
        temp._owner = client;
        temp._server = this;
    }

    private void RequestDamage(Player client, MagicBall ball)
    {
        if (ball.Owner != client)
        {
            _playersDic[client].GetDamage();
        }
    }
    #endregion

    #region OTHER_FUNCTIONS
    [PunRPC]
    public void RequestNickNameUpdate()
    {
        foreach (var client in _playersDic)
        {
            //client.Value.UpdateName(client.Key.NickName);
        }
    }

    //DEBUG
    void PrintPlayers()
    {
        foreach (var client in _playersDic)
        {
            print($"{client.Key.NickName}");
        }
    }

    void StartGame()
    { 
        _isGameStarted = true;
    }

    #endregion

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        
    }
}
