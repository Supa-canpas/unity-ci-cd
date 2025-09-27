using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TestNetworkUI : MonoBehaviour
{
    [SerializeField] private Button serverButton;
    [SerializeField] private Button clientButton;
    
    [SerializeField] NetworkManager networkManager;

    private void Start()
    {
        serverButton.onClick.AddListener(() => networkManager.StartServer());
        clientButton.onClick.AddListener(() => networkManager.StartClient());
    }

    private void OnClickClient()
    {
        networkManager.StartClient();
    }

    private void OnClickHost()
    {
        networkManager.StartHost();
    }
}
