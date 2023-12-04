using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Unity.VisualScripting;

public class MagicBall : MonoBehaviourPun
{
    [SerializeField] private float _speed;
    public ServerManager _server;
    public Player _owner;

    public Player Owner => _owner;
    public float lifeSpawn = 5f;
    public float counter = 0;

    public string duenio;

    void Update()
    {
        counter += Time.deltaTime;
        if (counter >= lifeSpawn)
        {
            PhotonNetwork.Destroy(this.gameObject);
        }

        transform.position += transform.forward * Time.deltaTime * _speed;
    }

    [PunRPC]
    void SetOwner(Player player)
    { 
        _owner = player;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (other.GetComponent<Character>() == null )
        {
            PhotonNetwork.Destroy(this.gameObject);
        }
    }
}
