using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

class SelectedAuthoring : MonoBehaviour
{
    public GameObject visualEntity;
    public float showScale;
}

class SelectedAuthoringBaker : Baker<SelectedAuthoring>
{
    public override void Bake(SelectedAuthoring authoring)
    {
        Entity entity=GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity,new Selected()
        {
            visualEntitiy = GetEntity(authoring.visualEntity, TransformUsageFlags.Dynamic),
            showScale = authoring.showScale
        });
        SetComponentEnabled<Selected>(entity,false);
    }
}

public struct Selected:IComponentData,IEnableableComponent
{
    public Entity visualEntitiy;
    public float showScale;

    public bool OnSelected;
    public bool OnDeSelected;
}
