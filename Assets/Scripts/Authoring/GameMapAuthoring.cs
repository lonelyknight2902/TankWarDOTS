using System.Collections;
using System.Collections.Generic;
using Components;
using Unity.Entities;
using UnityEngine;

public class GameMapAuthoring : MonoBehaviour
{
    public int width;

    public int height;

    public GameObject cellPrefab;
    // Start is called before the first frame update
    public class GameMapBaker : Baker<GameMapAuthoring>
    {
        public override void Bake(GameMapAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new GameMapComponent
            {
                height = authoring.height,
                width = authoring.width,
                cell = GetEntity(authoring.cellPrefab, TransformUsageFlags.Dynamic),
            });
        }
    }
}
