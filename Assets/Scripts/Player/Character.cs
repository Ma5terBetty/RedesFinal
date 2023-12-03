using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Collections.Generic;

public class Character : MonoBehaviourPun
{
    [SerializeField] private TextMeshPro _playerName;
    [SerializeField] private float _speed = 5;
    [SerializeField] private float _turningSpeed = 50;
    [SerializeField] private float _health;
    private Rigidbody _rigidbody;

    public ServerManager ServerManager;
    public Player LocalPlayer;
    public string PlayerRepresentation;

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

    public void GetDamage()
    {
        _health -= 25;
        Debug.Log("He recibido daño");
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
            ServerManager.RPC("RequestDamage", ball);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var ball = other.gameObject.GetComponent<MagicBall>();

        if (ball != null)
        {
            print("Me toco una bola");
            ServerManager.RPC("RequestDamage", PhotonNetwork.LocalPlayer, ball);
            PhotonNetwork.Destroy(ball.gameObject);
        }
    }
}
