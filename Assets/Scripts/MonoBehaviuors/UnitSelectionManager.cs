using System;
using Unity.Collections;
using Unity.Entities;
using Unity.VisualScripting;
using UnityEngine;

namespace MonoBehaviuors
{
    public class UnitSelectionManager:SingletonMono<UnitSelectionManager>
    {

        private void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
                Vector3 mousePosition=MouseWorldPosition.Instance.GetPosition();

                EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
                EntityQuery entityQuery =
                    new EntityQueryBuilder(Allocator.Temp).WithAll<UnitMover>().Build(entityManager);
                NativeArray<UnitMover>unitMoverArray=entityQuery.ToComponentDataArray<UnitMover>(Allocator.Temp);
                for (int i = 0; i < unitMoverArray.Length; ++i)
                {
                    UnitMover unitMover = unitMoverArray[i];
                    unitMover.targetPosition = mousePosition;
                    unitMoverArray[i] = unitMover;
                }
                entityQuery.CopyFromComponentDataArray(unitMoverArray);
            }
        }
    }
}