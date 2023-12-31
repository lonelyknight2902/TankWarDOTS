using System.Collections;
using System.Collections.Generic;
using Components;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class GameMapAuthoring : MonoBehaviour
    {
        public int width;
        public int height;
        public int wall;
        public GameObject cellPrefab;
        public GameObject wallPrefab;
        // Start is called before the first frame update
        public class GameMapBaker : Baker<GameMapAuthoring>
        {
            public override void Bake(GameMapAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new GameMapComponent
                {
                    Height = authoring.height,
                    Width = authoring.width,
                    Wall = authoring.wall,
                    CellPrefab = GetEntity(authoring.cellPrefab, TransformUsageFlags.Dynamic),
                    WallPrefab = GetEntity(authoring.wallPrefab, TransformUsageFlags.Dynamic)
                });
            }
        }
    }
}
