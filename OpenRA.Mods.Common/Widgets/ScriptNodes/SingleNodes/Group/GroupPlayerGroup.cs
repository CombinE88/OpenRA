using System;
using System.Collections.Generic;
using System.Linq;
using OpenRA.Mods.Common.Widgets.ScriptNodes.Library;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.Group
{
    public class GroupPlayerGroup : NodeWidget
    {
        public new static Dictionary<string, BuildNodeConstructorInfo> NodeConstructorInformation =
            new Dictionary<string, BuildNodeConstructorInfo>()
            {
                {
                    "GroupPlayerGroup", new BuildNodeConstructorInfo
                    {
                        LogicClass = typeof(GroupPlayerLogic),

                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.PlayerGroup, "Group of grouped player's")
                        }
                    }
                },
            };
        
        public GroupPlayerGroup(NodeEditorNodeScreenWidget screen, NodeInfo nodeInfo) : base(screen, nodeInfo)
        {
            IsIncorrectConnected = () => InConnections.Any(inCon => inCon.In != null);
        }

        public override void Tick()
        {
            var none = InConnections.Where(c => c.In == null).ToArray();

            if (none.Length < 1)
            {
                var inCon = new InConnection(ConnectionType.Player, this);
                AddInConnection(inCon);
            }
            else if (none.Length > 1)
            {
                foreach (var con in none)
                    if (con != none.First())
                        InConnections.Remove(con);
            }

            base.Tick();
        }
    }

    public class GroupPlayerLogic : NodeLogic
    {
        List<PlayerReference> players = new List<PlayerReference>();

        public GroupPlayerLogic(NodeInfo nodeInfo, IngameNodeScriptSystem ingameNodeScriptSystem) : base(nodeInfo,
            ingameNodeScriptSystem)
        {
        }

        public override void DoAfterConnections()
        {
            var changePlayers = new List<PlayerReference>();
            foreach (var info in InConnections.Where(c =>
            {
                if (c.ConnectionTyp != ConnectionType.Player)
                    return false;

                if (c.In == null)
                    return false;

                if (c.In.Player == null)
                    return false;

                return true;
            }))
                changePlayers.Add(info.In.Player);

            players = changePlayers;

            OutConnections.First(c => c.ConnectionTyp == ConnectionType.PlayerGroup).PlayerGroup = players.ToArray();
        }

        public override void Tick(Actor self)
        {
            var changePlayers = new List<PlayerReference>();
            foreach (var info in InConnections.Where(c =>
            {
                if (c.ConnectionTyp != ConnectionType.Player)
                    return false;

                if (c.In == null)
                    return false;

                if (c.In.Player == null)
                    return false;

                return true;
            }))
                changePlayers.Add(info.In.Player);

            players = changePlayers;

            OutConnections.First(c => c.ConnectionTyp == ConnectionType.PlayerGroup).PlayerGroup = players.ToArray();
        }
    }
}