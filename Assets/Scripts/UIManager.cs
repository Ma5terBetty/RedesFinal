using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviourPun
{
    public static UIManager Instance;

    [SerializeField] private ServerManager _server;
    [SerializeField] private TextMeshProUGUI _mainText;
    [SerializeField] private TextMeshProUGUI _roomName;
    [SerializeField] private TextMeshProUGUI _winnerText;
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private GameObject _winnerScreen;
    [SerializeField] private Image _titleBackground;
    [SerializeField] private Slider _slider;

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
        _roomName.text = $"Room: {PhotonNetwork.CurrentRoom.Name}";
        _pauseMenu.SetActive(false);
        _winnerScreen.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        { 
            PauseMenu();
        }
    }

    [PunRPC]
    public void ChangeMessage(string message)
    { 
        _mainText.text = message;
    }

    [PunRPC]
    private void UpdateHealth(float healthPoints)
    { 
        _slider.value = healthPoints;
    }

    [PunRPC]
    private void UpdateTextStatus(string message)
    { 
        
    }

    [PunRPC]
    private void ActivateMainTitle()
    { 
        _titleBackground.gameObject.SetActive(true);
        _mainText.gameObject.SetActive(true);
    }

    [PunRPC]
    private void DeactivateMainTitle()
    {
        _titleBackground.gameObject.SetActive(false);
        _mainText.gameObject.SetActive(false);
    }

    [PunRPC]
    private void WinnerScreen(string winner)
    {
        _winnerScreen.gameObject.SetActive(true);
        _winnerText.text = $"The Wizard {winner} has won!";
    }

    public void PauseMenu()
    {
        _pauseMenu.SetActive(!_pauseMenu.activeSelf);
    }

    public void LoadMainMenu()
    { 
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel("MainMenu");
    }
}
