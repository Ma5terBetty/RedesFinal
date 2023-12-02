using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class Character : MonoBehaviourPun
{
    [SerializeField] private TextMeshPro _playerName;
    [SerializeField] private float _speed = 5;
    [SerializeField] private float _turningSpeed = 50;
    [SerializeField] private float _health;
    private Rigidbody _rigidbody;

    public ServerManager ServerManager;
    public Player LocalPlayer;

    private void Start()
    {
        _health = 100;
    }
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void Move(Vector3 direction)
    {
        direction *= _speed;
        _rigidbody.position += direction;
    }

    public void Rotate(float rotation)
    {
        transform.Rotate(0, rotation * _turningSpeed * Time.deltaTime, 0);
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
