using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Collections.Generic;
using Unity.VisualScripting;
using Photon.Voice.PUN;
using Photon.Voice.Unity;

public class Character : MonoBehaviourPun
{
    [SerializeField] private TextMeshPro _playerName;
    [SerializeField] private float _health;
    private Rigidbody _rigidbody;

    public ServerManager _Server;
    public Player LocalPlayer;
    public string PlayerRepresentation;

    public float Health => _health;

    private void Start()
    {
        LocalPlayer = PhotonNetwork.LocalPlayer;
        _health = 100;
    }
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }


    [PunRPC]
    public void Move(string name, Vector3 position)
    {
        if (PlayerRepresentation == name)
        {
            _rigidbody.position = position;
        }
    }

    public void NewPosition(Vector3 position)
    {
        _rigidbody.position = position;
    }

    [PunRPC]
    public void Rotate(string name, Vector3 rotation)
    {
        if (PlayerRepresentation == name)
        {
            transform.eulerAngles = rotation;
        }
    }

    [PunRPC]
    public void GetDamage(float health)
    {
        _health = health;
    }

    [PunRPC]
    public void GetHealing(float health)
    {
        _health = health;
    }

    public void ChangeName(Player client)
    {
        photonView.RPC("UpdateName", RpcTarget.AllBuffered, client.NickName);
    }

    [PunRPC]
    public void UpdateName(string name)
    {
        _playerName.text = name;
        PlayerRepresentation = name;
    }

    private void OnCollisionEnter(Collision collision)
    {
        var ball = collision.gameObject.GetComponent<MagicBall>();

        if (ball != null)
        {
            print("Me toco una bola");
            _Server.RPC("RequestDamage", ball);
        }
    }

    [PunRPC]
    private void StartVoice()
    {
        var audioView = photonView.AddComponent<PhotonVoiceView>();
        var speaker = photonView.AddComponent<Speaker>();

        audioView.UsePrimaryRecorder = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        var ball = other.gameObject.GetComponent<MagicBall>();
        if (ball != null)
        {
            
        }
    }
}
