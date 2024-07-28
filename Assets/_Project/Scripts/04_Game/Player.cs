using Fusion;

public class Player : NetworkBehaviour
{
    private NetworkCharacterController _cc;

    [Networked]
    public float Health { get; set; }

    private void Awake()
    {
        _cc = GetComponent<NetworkCharacterController>();
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            data.direction.Normalize();
            transform.Translate(5 * data.direction * Runner.DeltaTime);
        }
    }
}
