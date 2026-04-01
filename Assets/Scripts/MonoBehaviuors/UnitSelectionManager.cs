using System;
using Authoring;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace MonoBehaviuors
{
    public class UnitSelectionManager:SingletonMono<UnitSelectionManager>
    {
        public event EventHandler OnSelectionAreaStart;
        public event EventHandler OnSelectionAreaEnd;
        
        private Vector2 selectionStartMousePosition;
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                selectionStartMousePosition=Input.mousePosition;
                OnSelectionAreaStart?.Invoke(this,EventArgs.Empty);
            }

            if (Input.GetMouseButtonUp(0))
            {
                Vector2 selectionEndPosition=Input.mousePosition;
                EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
                //取消之前选中的Unit
                EntityQuery entityQuery =
                    new EntityQueryBuilder(Allocator.Temp).WithAll<Selected>().Build(entityManager);
                NativeArray<Entity>entities=entityQuery.ToEntityArray(Allocator.Temp);
                NativeArray<Selected> selectedArray = entityQuery.ToComponentDataArray<Selected>(Allocator.Temp);
                for (int i = 0; i < entities.Length; ++i)
                {
                    entityManager.SetComponentEnabled<Selected>(entities[i],false);
                    var selected = selectedArray[i];
                    selected.OnDeSelected = true;
                    entityManager.SetComponentData(entities[i],selected);
                }
                
                //重新选择
                
                Rect selectionAreaRect=GetSelectionAreaRect();
                float selectionAreaSize=selectionAreaRect.width*selectionAreaRect.height;
                float multipleSelectionSizeMin = 40f;
                bool isMultileSelection = selectionAreaSize > multipleSelectionSizeMin;
                if (isMultileSelection)//多选
                {
                    //WithPresent确保能找到Selected组件，不管是启用还是禁用，不能放在WithAll里，否则禁用的Selected组件会被过滤掉
                     entityQuery =
                        new EntityQueryBuilder(Allocator.Temp).WithAll<LocalTransform,Unit>().WithPresent<Selected>().Build(entityManager);
                    NativeArray<LocalTransform>localTransformArray=entityQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);
                    entities=entityQuery.ToEntityArray(Allocator.Temp);
                    for (int i = 0; i < localTransformArray.Length; ++i)
                    {
                        LocalTransform localTransform = localTransformArray[i];
                        Vector2 unitScreenPosition=Camera.main.WorldToScreenPoint(localTransform.Position);
                        if (selectionAreaRect.Contains(unitScreenPosition))
                        {
                            //在选择范围内的单位，启用Selected组件
                            entityManager.SetComponentEnabled<Selected>(entities[i],true);
                            Selected selected=entityManager.GetComponentData<Selected>(entities[i]);
                            selected.OnSelected=true;
                            entityManager.SetComponentData(entities[i],selected);
                        }
                    }
                }
                else//单选
                {
                    entityQuery=entityManager.CreateEntityQuery(typeof(PhysicsWorldSingleton));
                    var physicsWorlSingleton=entityQuery.GetSingleton<PhysicsWorldSingleton>();
                    var collisionWorld=physicsWorlSingleton.CollisionWorld;
                    var cameraRay=Camera.main.ScreenPointToRay(Input.mousePosition);
                    var raycastInput = new RaycastInput()
                    {
                        Start = cameraRay.GetPoint(0),
                        End = cameraRay.GetPoint(9999f),
                        Filter = new CollisionFilter()
                        {
                            BelongsTo = ~0u,
                            CollidesWith = 1u << LayerMask.NameToLayer("Unit"),
                            GroupIndex = 0
                        }
                    };
                    if (collisionWorld.CastRay(raycastInput, out var hit))
                    {
                        if (entityManager.HasComponent<Unit>(hit.Entity))
                        {
                            entityManager.SetComponentEnabled<Selected>(hit.Entity,true);
                            Selected selected=entityManager.GetComponentData<Selected>(hit.Entity);
                            selected.OnSelected=true;
                            entityManager.SetComponentData(hit.Entity,selected);
                        }
                    }
                }
                OnSelectionAreaEnd?.Invoke(this,EventArgs.Empty);
            }
            if (Input.GetMouseButtonDown(1))
            {
                Vector3 mousePosition=MouseWorldPosition.Instance.GetPosition();

                EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
                EntityQuery entityQuery =
                    new EntityQueryBuilder(Allocator.Temp).WithAll<UnitMover,Selected>().Build(entityManager);
                NativeArray<UnitMover>unitMoverArray=entityQuery.ToComponentDataArray<UnitMover>(Allocator.Temp);
                var movePositionArray=GenerateMvePositionArray(mousePosition,unitMoverArray.Length);
                for (int i = 0; i < unitMoverArray.Length; ++i)
                {
                    UnitMover unitMover = unitMoverArray[i];
                    unitMover.targetPosition = movePositionArray[i];
                    unitMoverArray[i] = unitMover;
                }
                entityQuery.CopyFromComponentDataArray(unitMoverArray);
            }
        }

        public Rect GetSelectionAreaRect()
        {
            Vector2 selectionEndPosition=Input.mousePosition;
            Vector2 lowerLeftCorner =
                new Vector2(
                    Mathf.Min(selectionStartMousePosition.x, selectionEndPosition.x),Mathf.Min(
                        selectionStartMousePosition.y, selectionEndPosition.y));
            Vector2 upperRightCorner =
                new Vector2(
                    Mathf.Max(selectionStartMousePosition.x, selectionEndPosition.x),Mathf.Max(
                        selectionStartMousePosition.y, selectionEndPosition.y));
            return new Rect(lowerLeftCorner.x,lowerLeftCorner.y,upperRightCorner.x-lowerLeftCorner.x,upperRightCorner.y-lowerLeftCorner.y);
        }
        private NativeArray<float3>GenerateMvePositionArray(float3 targetPosition,int positionCount)
        {
            var positionArray = new NativeArray<float3>(positionCount, Allocator.Temp);
            if (positionCount == 0)
            {
                return positionArray;
            }
            positionArray[0]=targetPosition;
            if(positionCount==1)
                return positionArray;
            float ringSize = 2.2f;
            int ring = 0;
            int positionIndex = 1;
            while (positionIndex < positionCount)
            {
                int ringPositionCount = 3 + ring * 2;
                for (int i = 0; i < ringPositionCount; i++)
                {
                    float angle=i*(math.PI2/ringPositionCount);
                    float3 ringVector=math.rotate(quaternion.RotateY(angle), new float3(ringSize*(ring+1), 0, 0));
                    float3 ringPosition = targetPosition + ringVector;
                    positionArray[positionIndex++] = ringPosition;
                    if (positionIndex >= positionCount)
                    {
                        break;
                    }
                }

                ring++;
            }
            return positionArray;
        }
    }
}