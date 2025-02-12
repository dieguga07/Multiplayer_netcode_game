using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class DetectorController : MonoBehaviour
{
    [SerializeField] private GameObject detector;
    private static Transform transformButton1;
    private static Transform transformButton2;
    private static Transform transformButton3;
    void Start()
    {
        // Instanciamos los detectores en diferentes posiciones
        SpawnDetector(new Vector3(1, 1, 0), "Spawn detector 1");
        SpawnDetector(new Vector3(10, 5, 0), "Spawn detector 2");
        SpawnDetector(new Vector3(5, 5, 0), "Spawn detector 3");
    }

    // Método para crear un detector en una posición específica
    private void SpawnDetector(Vector3 position, string logMessage)
    {
        Debug.Log(logMessage);
        GameObject detectorInstance = Instantiate(detector, position, Quaternion.identity);

        // Si el objeto debe ser sincronizado en red, usa NetworkObject y Spawn
        NetworkObject networkObject = detectorInstance.GetComponent<NetworkObject>();
        if (networkObject != null && NetworkManager.Singleton.IsServer)
        {
            networkObject.Spawn();
        }
    }

}
