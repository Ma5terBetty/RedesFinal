using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviourPun
{
    public static UIManager Instance;

    [SerializeField] private ServerManager _server;
    [SerializeField] private Slider _slider;
    [SerializeField] private TextMeshProUGUI _mainText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        _slider.maxValue = 100;
        _slider.value = 100;
    }

    public void ChangeMessage(string message)
    { 
        _mainText.text = message;
    }

    [PunRPC]
    private void UpdateHealth(float healthPoints)
    { 
        _slider.value = healthPoints;
    }
}
