using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOTSInMars.Narrator
{
    public enum NarrationType
    {
        IdleChatter = 0,
        BuildingPlacementIdling = 4,
        MinerPlacementStart = 8,
        MinerPlacementIdling = 9,
        MinerDone = 10,
        RefineryPlacementStart = 5,
        RefineryPlacementIdling = 6,
        RefineryDone = 7,
        ManufacturerPlacementStart = 1,
        ManufacturerPlacementIdling = 2,
        ManufacturerDone = 3,
        ConveyorPlacementStart = 11,
        ConveyorPlacementIdling = 12,
        ConveyorDone = 14,
        DepositedValuableItems = 13,
        InsanityByRepeatedEffort = 15,
        InsanityByRepeatedEffortSecond = 16,
    }
}
