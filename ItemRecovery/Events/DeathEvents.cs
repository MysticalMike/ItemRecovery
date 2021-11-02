using System.Collections.Generic;
using System.Linq;
using ItemRecovery.Util;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace ItemRecovery.Events
{
    public class DeathEvents
    {
        private static IModHelper helper;

        public static readonly Dictionary<long, int> cached_money = new Dictionary<long, int>();

        public DeathEvents(IModHelper _helper)
        {
            helper = _helper;
            
            helper.Events.Player.InventoryChanged += OnInventoryChanged;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            // helper.Events.GameLoop.UpdateTicked += UpdateTicked;
            helper.Events.Multiplayer.PeerConnected += PeerConnected;
        }

        private void PeerConnected(object sender, PeerConnectedEventArgs e)
        {
            if (!Context.IsMainPlayer)
                return;
            
            if (e.Peer.IsHost)
                ModEntry.GetManager().SetModData(ModDataHelper.GetHostModData());
            else
                ModEntry.GetManager().WriteToPlayer(e.Peer.PlayerID);
        }

        // TODO WIP Money Recovery System
        private void UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            Farmer player = Game1.player;
            
            if (Game1.killScreen)
            {
                if (!cached_money.ContainsKey(player.UniqueMultiplayerID))
                {
                    ModEntry.Log($"Before Money: ${player.Money}");
                    cached_money.Add(player.UniqueMultiplayerID, player.Money);
                }
            }
            else if (player.canMove && cached_money.ContainsKey(player.UniqueMultiplayerID))
            {
                if (Game1.activeClickableMenu is ItemListMenu)
                {
                    ModEntry.Log($"After Money: ${player.Money}");
                    player.Money = cached_money[player.UniqueMultiplayerID];
                    cached_money.Remove(player.UniqueMultiplayerID);
                }
            }
        }

        private static void OnInventoryChanged(object sender, InventoryChangedEventArgs e)
        {
            Farmer farmer = e.Player;
            
            if (farmer.itemsLostLastDeath.Count < 1)
                return;
            
            if (ListHelper.SortListsThenCompareItemNames(farmer.itemsLostLastDeath.ToList(), e.Removed.ToList()))
            {
                ModDataHelper.ResetPlayerDSLD(farmer.UniqueMultiplayerID);
            }
        }
        
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            if (!Context.IsMainPlayer)
                return;
            
            ModDataHelper.AdvanceAllPlayerDSLD();
        }
    }
}