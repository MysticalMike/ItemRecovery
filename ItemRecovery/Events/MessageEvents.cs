using ItemRecovery.Data;
using ItemRecovery.Objects;
using ItemRecovery.Util;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace ItemRecovery.Events
{
    public class MessageEvents
    {
        private IModHelper helper;
        private IManifest manifest;
        
        public MessageEvents(IModHelper helper, IManifest manifest)
        {
            this.helper = helper;
            this.manifest = manifest;
            helper.Events.Multiplayer.ModMessageReceived += OnModMessageReceived;
        }

        private void OnModMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            if (e.FromModID == manifest.UniqueID && e.Type == "MessageData")
            {
                MessageData message = e.ReadAs<MessageData>();

                switch (message.GetCommand())
                {
                    case "WriteToHost":
                    {
                        ModDataHelper.AddModData(message.GetModData());
                        ModEntry.GetManager().SetModData(ModDataHelper.GetHostModData());
                        ModEntry.GetManager().WriteToPlayers();
                        break;
                    }
                    case "WriteToLocal":
                    {
                        message.GetModData().PrintData();
                        
                        ModEntry.GetManager().SetModData(message.GetModData());
                        break;
                    }
                    default: ModEntry.Log($"Unknown Command: {message.GetCommand()}");
                        break;
                }
            }
        }
    }
}