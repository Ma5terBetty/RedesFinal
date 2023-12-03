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
    private Dictionary<string, Vector3> _posDic = new Dictionary<string, Vector3>();
    private Player _server;
    private int _playerCount;
    private bool _isGameStarted;
    private bool _isGameEnded;
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
            _playerCount++;
        }
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (IsServer)
        {
            if (_playersDic.ContainsKey(otherPlayer))
            {
                PhotonNetwork.Destroy(_playersDic[otherPlayer].gameObject);
                _playerCount--;
            }
        }
    }
    #endregion

    #region REQUESTS

    /// <summary>
    /// Pedido para instanciar el jugador y guardar su referencia.
    /// </summary>
    /// <param name="player">Cliente a instanciar</param>
    [PunRPC]
    private void RequestConnect(Player player)
    {
        CreatePlayer(player);
        print($"{player.NickName}");
    }

    /// <summary>
    /// Pedido de movimiento dentro del mundo
    /// </summary>
    /// <param name="client">Cliente a mover</param>
    /// <param name="movement">Dirección de movimiento</param>
    [PunRPC] // Pedido de Movimiento
    private void RequestMove(Player client, Vector3 movement)
    {
        _playersDic[client].transform.position += movement;
        UpdatePlayersPositions();
    }

    /// <summary>
    /// Pedido de rotación del personaje
    /// </summary>
    /// <param name="client">Cliente a rotar</param>
    /// <param name="degrees">Cantidad de grados a rotar</param>
    [PunRPC] // Pedido de Rotación
    private void RequestRotation(Player client, float degrees)
    {
        _playersDic[client].transform.Rotate(Vector3.up, degrees);
        UpdatePlayersRotation();
    }

    /// <summary>
    /// Pedido de ataque para instanciar una bola
    /// </summary>
    /// <param name="client">Cliente que solicita el ataque</param>
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
    /*[PunRPC]
    public void RequestNickNameUpdate()
    {
        foreach (var client in _playersDic)
        {
            //client.Value.UpdateName(client.Key.NickName);
        }
    }*/

    private void UpdatePlayersPositions()
    {
        foreach (Player player in _charactersDic.Values)
        {
            _posDic[_playersDic[player].PlayerRepresentation] = _playersDic[player].transform.position;
            _playersDic[player].photonView.RPC("Move", RpcTarget.All, _playersDic[player].PlayerRepresentation, _posDic[_playersDic[player].PlayerRepresentation]);
        }
    }

    private void UpdatePlayersRotation()
    {
        foreach (Player player in _charactersDic.Values)
        {
            _posDic[_playersDic[player].PlayerRepresentation] = _playersDic[player].transform.rotation.eulerAngles;
            _playersDic[player].photonView.RPC("Rotate", RpcTarget.All, _playersDic[player].PlayerRepresentation, _posDic[_playersDic[player].PlayerRepresentation]);
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
}
