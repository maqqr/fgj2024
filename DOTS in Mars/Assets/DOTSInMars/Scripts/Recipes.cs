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
            new() { Duration = 1.0f, Inputs = new ResourceWithAmount[] {}, Output = ResourceType.Bronze },
            new() { Duration = 2.0f, Inputs = new ResourceWithAmount[] { new(ResourceType.Bronze, 3) }, Output = ResourceType.Iron },
            new() { Duration = 3.0f, Inputs = new ResourceWithAmount[] { new(ResourceType.Iron, 3) }, Output = ResourceType.Giggels },
        };

        public static Recipe Get(int index)
        {
            return recipes[index];
        }
    }
}