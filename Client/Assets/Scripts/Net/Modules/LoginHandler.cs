﻿using Shinobytes.Ravenfall.RavenNet.Models;
using System;

namespace Shinobytes.Ravenfall.RavenNet.Modules
{
    public class CharacterHandler : IModule
    {
        public string Name => "Login";

        public Player[] Players { get; private set; } = new Player[0];

        internal void SetCharacterList(Player[] players)
        {
            Players = players;
        }
    }
}
