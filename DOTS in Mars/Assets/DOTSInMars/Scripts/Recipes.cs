using System.Collections;
using System.Collections.Generic;
using DOTSInMars.Resources;
using UnityEngine;

namespace DOTSInMars
{
    public struct ResourceWithAmount
    {
        public ResourceWithAmount(ResourceType resourceType, int amount)
        {
            ResourceType = resourceType;
            Amount = amount;
        }

        public ResourceType ResourceType;
        public int Amount;
    }

    public class Recipe
    {
        public float Duration = 1.0f; // How often an item is produced
        public ResourceWithAmount[] Inputs;
        public ResourceType Output;
    }

    public static class Recipes
    {
        private static readonly List<Recipe> recipes = new()
        {
            new() { Duration = 0.5f, Inputs = new ResourceWithAmount[] {}, Output = ResourceType.CopperOre },
            new() { Duration = 2.0f, Inputs = new ResourceWithAmount[] { new(ResourceType.CopperOre, 3) }, Output = ResourceType.CopperBar },
            new() { Duration = 3.0f, Inputs = new ResourceWithAmount[] { new(ResourceType.CopperBar, 3) }, Output = ResourceType.Giggels },
        };

        public static Recipe Get(int index)
        {
            return recipes[index];
        }
    }
}