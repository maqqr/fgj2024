using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using UnityEngine;

namespace DOTSInMars.Buildings
{
    public class BuildingCatalogAuthoring : MonoBehaviour
    {
        [SerializeField] private GameObject _miner;

        public GameObject Miner => _miner;

        class BuildingCatalogBaker : Baker<BuildingCatalogAuthoring>
        {
            public override void Bake(BuildingCatalogAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new BuildingCatalog
                {
                    Miner = GetEntity(authoring.Miner, TransformUsageFlags.None)
                });
            }
        }
    }

    public struct BuildingCatalog : IComponentData
    {
        public Entity Miner;
    }
}
