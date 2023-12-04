using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using Photon.Voice.PUN;

public class ServerManager : MonoBehaviourPunCallbacks
{
    #region PRIVATE_PROPERTIES

    [SerializeField] private string _playerPrefabName = "PlayerPrefab";
    [SerializeField] private string _magicBallName = "MagicBall";
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private UIManager _uiManager;

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
    public bool IsGameEnded => _isGameEnded;
    #endregion

    #region UNITY_EVENTS

    private void Awake()
    {
        _server = PhotonNetwork.MasterClient;
    }

    private void Start()
    {
        _isGameStarted = false;
        _isGameEnded = false;
    }
    #endregion

    #region PHOTON_RELATED

    /// <summary>
    /// Llamada simplificada a los RPC's del servidor.
    /// </summary>
    /// <param name="name">Nombre de la función a reproducir</param>
    /// <param name="input">Parámetros a utilizar</param>
    public void RPC(string name, params object[] input)
    {
        photonView.RPC(name, _server, input);
    }

    /// <summary>
    /// Crea y guarda la referencia del jugador.
    /// </summary>
    /// <param name="client">Cliente a guardar</param>
    private void CreatePlayer(Player client)
    {
        var obj = PhotonNetwork.Instantiate(_playerPrefabName, _spawnPoints[_playersDic.Count].position, Quaternion.identity);
        var character = obj.GetComponent<Character>();
        character.ChangeName(client);
        character._Server = this;
        character.LocalPlayer = client;
        character.photonView.RPC("StartVoice", RpcTarget.AllBuffered);
        if (character != null)
        {
            _playersDic[client] = character;
            _charactersDic[character] = client;
            _playerCount++;
        }

        if (_playerCount == 2)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                StartCounter(); 
            }
        }
    }

    /// <summary>
    /// De desconectó un jugador de la Sala.
    /// </summary>
    /// <param name="otherPlayer">Cliente desconectado de la sala</param>
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (IsServer)
        {
            if (_playersDic.ContainsKey(otherPlayer))
            {
                PhotonNetwork.Destroy(_playersDic[otherPlayer].gameObject);
                _playerCount--;
                CheckForVictory();
            }
        }
    }
    #endregion

    #region REQUESTS

    [PunRPC]
    /// <summary>
    /// Pedido para instanciar el jugador y guardar su referencia.
    /// </summary>
    /// <param name="player">Cliente a instanciar</param>
    private void RequestConnect(Player player)
    {
        CreatePlayer(player);
        print($"{player.NickName}");
    }

    [PunRPC]
    /// <summary>
    /// Pedido de movimiento dentro del mundo
    /// </summary>
    /// <param name="client">Cliente a mover</param>
    /// <param name="movement">Dirección de movimiento</param>
    private void RequestMove(Player client, Vector3 movement)
    {
        if (!_isGameStarted) return;

        _playersDic[client]._anim.SetBool("Attack", false);
        _playersDic[client]._anim.SetBool("Move", true);
        _playersDic[client].transform.position += movement;
        UpdatePlayersPositions();
    }

    [PunRPC]
    /// <summary>
    /// Pedido de rotación del personaje
    /// </summary>
    /// <param name="client">Cliente a rotar</param>
    /// <param name="degrees">Cantidad de grados a rotar</param>
    private void RequestRotation(Player client, float degrees)
    {
        if (!_isGameStarted) return;

        _playersDic[client].transform.Rotate(Vector3.up, degrees);
        UpdatePlayersRotation();
    }

    [PunRPC]
    /// <summary>
    /// Pedido de ataque para instanciar una bola
    /// </summary>
    /// <param name="client">Cliente que solicita el ataque</param>
    private void RequestAttack(Player client)
    {
        if (!_isGameStarted) return;

        _playersDic[client]._anim.SetBool("Move", false);
        _playersDic[client]._anim.SetBool("Attack", true);
        
        var playerTransform = _playersDic[client].transform;
        Vector3 compensation = new Vector3(0, 1, 0);
        var magicBall = PhotonNetwork.Instantiate(_magicBallName, playerTransform.position + compensation, playerTransform.localRotation);
        var temp = magicBall.GetComponent<MagicBall>();
        temp._server = this;
        temp.photonView.RPC("SetOwner", RpcTarget.All, client);
    }

    [PunRPC]
    /// <summary>
    /// Pedido de daño para el jugador
    /// </summary>
    /// <param name="client">Cliente que solicita el daño</param>
    private void RequestDamage(Character client)
    {
        if (!_isGameStarted) return;

        _playersDic[_charactersDic[client]].Health -= 25f;
        if (_playersDic[_charactersDic[client]].Health <= 0)
        {
            _uiManager.photonView.RPC("UpdateHealth", _charactersDic[client], _playersDic[_charactersDic[client]].Health);
            DestroyPlayer(client);
        }
        else
        {
            _playersDic[_charactersDic[client]].photonView.RPC("GetDamage", _charactersDic[client], _playersDic[_charactersDic[client]].Health);
            _uiManager.photonView.RPC("UpdateHealth", _charactersDic[client], _playersDic[_charactersDic[client]].Health);
        }
    }

    [PunRPC]
    /// <summary>
    /// Pedido de curación para el jugador
    /// </summary>
    /// <param name="client">Cliente que solicita la salud</param>
    private void Healing(Player client)
    {
        if (!_isGameStarted) return;

        _playersDic[client].Health += 10f;
        if (_playersDic[client].Health >= 100)
        {
            _playersDic[client].Health = 100;
        }
        else
        {
            _playersDic[client].photonView.RPC("GetDamage", client, _playersDic[client].Health);
            _uiManager.photonView.RPC("UpdateHealth", client, _playersDic[client].Health);
        }
    }

    [PunRPC]
    private void DestroyPlayer(Character client)
    {
        PhotonNetwork.Destroy(_playersDic[_charactersDic[client]].gameObject);
        _playerCount--;
        CheckForVictory();
    }

    [PunRPC]
    private void DestroyBall(Player client, GameObject ball)
    {
        PhotonNetwork.Destroy(ball);
    }
    #endregion

    #region OTHER_FUNCTIONS
    /// <summary>
    /// Refresca la posición de los jugadores en la escena
    /// </summary>
    private void UpdatePlayersPositions()
    {
        foreach (Player player in _charactersDic.Values)
        {
            _posDic[_playersDic[player].PlayerRepresentation] = _playersDic[player].transform.position;
            _playersDic[player].photonView.RPC("Move", RpcTarget.All, _playersDic[player].PlayerRepresentation, _posDic[_playersDic[player].PlayerRepresentation]);
        }
    }
    /// <summary>
    /// Refresca la rotación de los jugadores en escena
    /// </summary>
    private void UpdatePlayersRotation()
    {
        foreach (Player player in _charactersDic.Values)
        {
            _posDic[_playersDic[player].PlayerRepresentation] = _playersDic[player].transform.rotation.eulerAngles;
            _playersDic[player].photonView.RPC("Rotate", RpcTarget.All, _playersDic[player].PlayerRepresentation, _posDic[_playersDic[player].PlayerRepresentation]);
        }
    }

    void StartGame()
    {
        _isGameStarted = true;
    }

    void CheckForVictory()
    {
        if (_playerCount == 1 && _isGameStarted)
        { 
            _isGameEnded = true;
            _uiManager.photonView.RPC("WinnerScreen", RpcTarget.All, _playersDic.ElementAt(0).Key.NickName);
        }
    }

    void StartCounter()
    {
        StartCoroutine(BeginPlayCounter());
    }

    IEnumerator BeginPlayCounter()
    {
        _uiManager.photonView.RPC("ChangeMessage", RpcTarget.All, "3");
        yield return new WaitForSeconds(1f);
        _uiManager.photonView.RPC("ChangeMessage", RpcTarget.All, "2");
        yield return new WaitForSeconds(1f);
        _uiManager.photonView.RPC("ChangeMessage", RpcTarget.All, "1");
        yield return new WaitForSeconds(1f);
        _uiManager.photonView.RPC("ChangeMessage", RpcTarget.All, "Fight!");
        yield return new WaitForSeconds(1f);
        _uiManager.photonView.RPC("DeactivateMainTitle", RpcTarget.All);
        _isGameStarted = true;
    }
    #endregion
}
