using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    public float moveSpeed = 5f;

    // 네트워크로 동기화할 위치 변수
    private NetworkVariable<Vector2> networkPosition = new NetworkVariable<Vector2>();

    private void Update()
    {
        if (IsOwner)
        {
            Debug.Log("IsOwner");
            HandleMovement();
        }
        else
        {
            // 소유자가 아닌 경우, 네트워크 위치로 보간
            transform.position = networkPosition.Value;
        }
    }

    public override void OnNetworkSpawn()
    {
        Debug.Log(nameof(OnNetworkSpawn));
    }

    private void HandleMovement()
    {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");

        Vector2 movement = new Vector2(moveHorizontal, moveVertical).normalized;
        Vector2 newPosition = (Vector2)transform.position + movement * moveSpeed * Time.deltaTime;

        // 서버에 새 위치 전송
        UpdatePositionServerRpc(newPosition);
    }

    [ServerRpc]
    private void UpdatePositionServerRpc(Vector2 newPosition)
    {
        Debug.Log(newPosition + NetworkObjectId.ToString());
        // 서버에서 위치 업데이트 및 네트워크 동기화
        transform.position = newPosition;
        networkPosition.Value = newPosition;
    }

}
