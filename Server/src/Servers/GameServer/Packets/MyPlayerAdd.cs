﻿using Shinobytes.Ravenfall.RavenNet.Models;
using System.Collections.Generic;
using System.Linq;

namespace RavenfallServer.Packets
{
    public class MyPlayerAdd
    {
        public const short OpCode = 16;
        public int PlayerId { get; set; }
        public string Name { get; set; }
        public int CombatLevel { get; set; }
        public Vector3 Position { get; set; }
        public int[] EffectiveLevel { get; set; }
        public decimal[] Experience { get; set; }
        public Appearance Appearance { get; set; }
        internal static MyPlayerAdd Create(Player player, int combatLevel, IEnumerable<PlayerStat> stats)
        {
            return new MyPlayerAdd
            {
                PlayerId = player.Id,
                Name = player.Name,
                Position = player.Position,
                CombatLevel = combatLevel,
                Experience = stats.Select(x => x.Experience).ToArray(),
                EffectiveLevel = stats.Select(x => x.EffectiveLevel).ToArray(),
                Appearance = player.Appearance
            };
        }
    }
}
