using System;
using System.Collections.Generic;
using ED.SC;
using Unity.Netcode;
using UnityEngine;

public class AvatarController : NetworkBehaviour {
    [SerializeField] private float movementSpeed = 3.0f;
    [SerializeField] private GameObject doorPrefab;
    private bool doorOpen = false;
    private readonly List<Color> m_Colors = new List<Color>() { Color.blue , Color.green, Color.red};
    private SpriteRenderer m_Renderer;
    private static NetworkVariable<Vector3> m_Position = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<Color> m_Color = new NetworkVariable<Color>(Color.black, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public static NetworkVariable<int> btnCounter = new NetworkVariable<int>(0, 
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    
    
    // Start is called before the first frame update
    void Start()
    {
        SmartConsole.Log($"ClientId {OwnerClientId}");
        if (OwnerClientId > 2)
        {
            NetworkManager.Singleton.DisconnectClient(OwnerClientId);
            Destroy(gameObject);
            return;
        }
        transform.position += Vector3.right * 2 * OwnerClientId;
        m_Renderer = GetComponent<SpriteRenderer>();
        if (IsOwner) m_Color.Value = m_Colors[(int) OwnerClientId];

        if (IsOwner) m_Color.Value = m_Colors[(int) OwnerClientId];
    }

    // Update is called once per frame
    void Update() {
        if (m_Renderer.color != m_Color.Value) m_Renderer.color = m_Color.Value;

        if (!IsOwner) return;
        
        if (Input.GetKey(KeyCode.LeftArrow)) LerpPosition(Vector3.left, movementSpeed);
        if (Input.GetKey(KeyCode.RightArrow)) LerpPosition(Vector3.right, movementSpeed);
        if (Input.GetKey(KeyCode.UpArrow)) LerpPosition(Vector3.up, movementSpeed);
        if (Input.GetKey(KeyCode.DownArrow)) LerpPosition(Vector3.down, movementSpeed);

        if (btnCounter.Value == 3) {
            SpawnDoorServerRpc();
        }
    }
    
    [ServerRpc]
    void IncrementBtnCounterServerRpc(bool increment)
    {
        if (increment)
            btnCounter.Value += 1;
        else
            btnCounter.Value -= 1;

        Debug.Log($"btnCounter actualizado: {btnCounter.Value} por {gameObject.name}");
    }

    void LerpPosition(Vector3 offset, float speed) {
        Vector3 positionNow = transform.position;
        transform.position = Vector3.Lerp(positionNow, positionNow +  offset, Time.deltaTime * speed);
    }

    [ServerRpc(RequireOwnership = false)]
    void SpawnDoorServerRpc()
    {
        if (!doorOpen)
        {
            SmartConsole.Log("spawn door");
            GameObject door = Instantiate(doorPrefab, m_Position.Value, Quaternion.identity);
            m_Position.Value += Vector3.right + Vector3.up;
            door.GetComponent<NetworkObject>().Spawn();
            doorOpen = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        IncrementBtnCounterServerRpc(true);
        Debug.Log($"btnCounter incrementado: {btnCounter} por {gameObject.name}");
        
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        IncrementBtnCounterServerRpc(false);
        Debug.Log($"btnCounter decrementado: {btnCounter} por {gameObject.name}");
        
    }
}
