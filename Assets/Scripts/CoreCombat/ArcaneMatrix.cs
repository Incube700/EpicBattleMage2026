using System.Collections.Generic;
using UnityEngine;

namespace EpicMageBattle.Combat
{
    public static class ArcaneMatrix
    {
        private static readonly Dictionary<Element, Dictionary<Element, float>> table = new()
        {
            [Element.Fire] = new() { [Element.Water] = 0.5f, [Element.Air] = 1.1f, [Element.Frost] = 1.2f },
            [Element.Water] = new() { [Element.Fire] = 1.4f, [Element.Lightning] = 1.2f, [Element.Earth] = 0.9f },
            [Element.Air] = new() { [Element.Fire] = 0.9f, [Element.Lightning] = 1.1f, [Element.Earth] = 1.2f },
            [Element.Earth] = new() { [Element.Air] = 0.8f, [Element.Lightning] = 0.7f },
            [Element.Lightning] = new() { [Element.Water] = 1.3f, [Element.Earth] = 0.6f },
            [Element.Frost] = new() { [Element.Fire] = 0.6f },
        };

        public static float Modify(float baseAmount, Element attack, Element target)
        {
            if (table.TryGetValue(attack, out var row) && row.TryGetValue(target, out var mod))
                return baseAmount * mod;
            return baseAmount;
        }
    }
}