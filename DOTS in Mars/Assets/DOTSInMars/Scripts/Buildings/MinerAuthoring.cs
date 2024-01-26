using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using UnityEngine;

namespace DOTSInMars.Buildings
{
    public class MinerAuthoring : MonoBehaviour
    {

        class MinerBaker : Baker<MinerAuthoring>
        {
            public override void Bake(MinerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Building
                {
                    Type = BuildingType.Miner,
                    Recipe = 0
                });
            }
        }
    }
}
