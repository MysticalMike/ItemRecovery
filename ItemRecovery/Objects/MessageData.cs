using ItemRecovery.Data;
using Newtonsoft.Json;

namespace ItemRecovery.Objects
{
    public class MessageData
    {
        [JsonProperty]
        private string command = "none";
        [JsonProperty]
        private ModData data;
        
        public MessageData(string command, ModData data)
        {
            this.command = command;
            this.data = data;
        }

        public string GetCommand()
        {
            return command;
        }

        public ModData GetModData()
        {
            return data;
        }
    }
}