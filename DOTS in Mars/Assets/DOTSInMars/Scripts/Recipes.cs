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
            // Miner
            new() { Duration = 2.0f, Inputs = new ResourceWithAmount[] {}, Output = ResourceType.CopperOre },
            // Refinery
            new() { Duration = 2.0f, Inputs = new ResourceWithAmount[] { new(ResourceType.CopperOre, 3) }, Output = ResourceType.CopperBar },
            // Manufacturer
            new() { Duration = 3.0f, Inputs = new ResourceWithAmount[] { new(ResourceType.CopperBar, 3), new(ResourceType.CopperBar, 3) }, Output = ResourceType.Giggels },
            // Deposit
            new() { Duration = 0.1f, Inputs = new ResourceWithAmount[]
            {
                new(ResourceType.Giggels, 1), new(ResourceType.Giggels, 1),
                new(ResourceType.Giggels, 1), new(ResourceType.Giggels, 1),
                new(ResourceType.Giggels, 1), new(ResourceType.Giggels, 1),
                new(ResourceType.Giggels, 1), new(ResourceType.Giggels, 1),
            }, Output = ResourceType.Score },
        };

        public static Recipe Get(int index)
        {
            return recipes[index];
        }
    }
}