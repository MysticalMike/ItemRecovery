using System.Collections.Generic;
using ItemRecovery.Data;
using ItemRecovery.Objects;
using ItemRecovery.Util;
using StardewModdingAPI;

namespace ItemRecovery
{
    public class ModDataManager
    {
        private IModHelper helper;
        private IManifest manifest;
        private ModData mod_data = new ModData(new Dictionary<long, int>());

        public ModDataManager(IModHelper helper, IManifest manifest)
        {
            this.helper = helper;
            this.manifest = manifest;
        }

        public ModData GetModData()
        {
            return mod_data;
        }

        public void SetModData(ModData mod_data)
        {
            this.mod_data = mod_data;
        }

        public void WriteToHost()
        {
            bool host = Context.IsMainPlayer;
            
            if (host)
            {
                ModDataHelper.SaveHostModData(mod_data);
                WriteToPlayers();
                return;
            }
            
            MessageData message = new MessageData("WriteToHost", mod_data);
            helper.Multiplayer.SendMessage(message,"MessageData", new[] { manifest.UniqueID }, new[] {PlayerHelper.GetHost().UniqueMultiplayerID});
        }

        public void WriteToPlayers()
        {
            MessageData message = new MessageData("WriteToLocal", mod_data);
            helper.Multiplayer.SendMessage(message,"MessageData", new[] { manifest.UniqueID });
        }

        public void WriteToPlayer(long id)
        {
            MessageData message = new MessageData("WriteToLocal", mod_data);
            helper.Multiplayer.SendMessage(message,"MessageData", new[] { manifest.UniqueID }, new[] { id });
        }
    }
}