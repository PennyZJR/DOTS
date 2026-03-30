using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class UnitMoverAuthoring : MonoBehaviour
{
    public float moveSpeed;
    public float rotateSpeed;
    public class Baker:Baker<UnitMoverAuthoring>
    {
        public override void Bake(UnitMoverAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,new UnitMover()
            {
                moveSpeed = authoring.moveSpeed,
                rotationSpeed = authoring.rotateSpeed
            });
        }
    }
}
public struct UnitMover : IComponentData
{
    public float moveSpeed;
    public float rotationSpeed;
}
