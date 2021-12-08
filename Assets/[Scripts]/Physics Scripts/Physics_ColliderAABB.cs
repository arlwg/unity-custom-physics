using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Physics_ColliderAABB : Physics_ColliderBase
{
    /// <summary>
    /// The dimensions.
    /// </summary>
    public Vector3 dimensions = new Vector3(1, 1, 1);


    public Vector3 GetMin()
    {
        return transform.position - this.GetHalfSize();
    }
    public Vector3 GetMax()
    {
        return transform.position + this.GetHalfSize();
    }
    public Vector3 GetSize()
    {
        return Vector3.Scale(dimensions, transform.lossyScale);
    }

    public Vector3 GetHalfSize()
    {
        return Vector3.Scale(dimensions, transform.lossyScale) * 0.5f;
    }
    /// <summary>
    /// The get collision shape.
    /// </summary>
    /// <returns>
    /// The <see cref="CollisionShape"/>.
    /// </returns>
    public override CollisionShape GetCollisionShape()
    {
        return CollisionShape.AABB;
    }
}
