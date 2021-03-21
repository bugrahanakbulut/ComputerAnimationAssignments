using System.Collections.Generic;

public interface ICollisionSimulator
{
    void ResetSim();
    
    CollisionData DetectCollision(List<MovementData> movementRange);

    CollisionData DetectCollision(MovementData prev, MovementData next);
}
