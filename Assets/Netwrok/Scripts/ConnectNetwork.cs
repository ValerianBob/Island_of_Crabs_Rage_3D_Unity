using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ConnectNetwork : MonoBehaviour
{
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;


    private void Start()
    {
        hostButton.onClick.AddListener(Host);
        clientButton.onClick.AddListener(Client);
    }

    private void Host()
    {
        NetworkManager.Singleton.StartHost();
    }

    private void Client()
    {
        NetworkManager.Singleton.StartClient();
    }
}
