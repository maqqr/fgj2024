using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using UnityEngine;

namespace DOTSInMars
{
    public class CentralComplexAuthoring : MonoBehaviour
    {


        class CentralComplexBaker : Baker<CentralComplexAuthoring>
        {
            public override void Bake(CentralComplexAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new CentralComplex());
            }
        }
    }
}
