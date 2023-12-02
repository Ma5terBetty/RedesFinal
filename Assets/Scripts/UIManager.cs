using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class UIManager : MonoBehaviourPun
{
    public static UIManager Instance;

    [SerializeField] TextMeshProUGUI _mainText;

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
        
    }

    public void ChangeMessage(string message)
    { 
        _mainText.text = message;
    }
}
