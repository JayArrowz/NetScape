using NetScape.Modules.FiveZeroEight.LoginProtocol.WorldList;

namespace NetScape.Modules.FiveZeroEight.LoginProtocol.Handlers
{
    public class WorldListMessage
    {
        public int SessionId { get; }
        public Country[] Countries { get; }
        public World[] Worlds { get; }
        public int[] Players { get; }
        public WorldListMessage(int sessionId, Country[] countries, World[] worlds, int[] players)
        {
            SessionId = sessionId;
            Countries = countries;
            Worlds = worlds;
            Players = players;
        }
    }
}
