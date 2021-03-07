using BLUE.PoolingSystem;
using UnityEngine;

public class BallActivationInfo : PoolObjectActivationInfo
{
    public Vector3 Position { get; }

    public BallActivationInfo(Vector3 position)
    {
        Position = position;
    }
}

public class BallPoolObject : PoolGameObject<BallActivationInfo>
{
    protected override void ActivateCustomActions(BallActivationInfo activationInfo)
    {
        transform.position = activationInfo.Position;

        gameObject.SetActive(true);
    }

    protected override void DeactivationCustomActions()
    {
        gameObject.SetActive(false);
    }
}
