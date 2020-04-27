using System;
using System.Collections.Generic;
using System.Linq;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.NodeInfos.GroupInfos
{
    public class GroupPlayerGroupInfo : NodeInfo
    {
        public new static Dictionary<string, BuildNodeConstructorInfo> NodeConstructorInformation =
            new Dictionary<string, BuildNodeConstructorInfo>()
            {
                {
                    "GroupPlayerGroup", new BuildNodeConstructorInfo
                    {
                        Nesting = new[] {"Actor/Player Group"},
                        Name = "Group Player",

                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.PlayerGroup, "Group of grouped player's")
                        }
                    }
                },
            };

        public GroupPlayerGroupInfo(string nodeType, string nodeId, string nodeName) : base(nodeType, nodeId, nodeName)
        {
        }

        public override void WidgetInitialize(NodeWidget widget)
        {
            widget.IsIncorrectConnected = () => widget.InConnections.All(inCon => inCon.In == null);
        }

        public override void WidgetTick(NodeWidget widget)
        {
            var none = widget.InConnections.Where(c => c.In == null).ToArray();

            if (none.Length < 1)
            {
                var inCon = new InConnection(ConnectionType.Player, widget);
                widget.AddInConnection(inCon);
            }
            else if (none.Length > 1)
            {
                foreach (var con in none)
                    if (con != none.First())
                        widget.InConnections.Remove(con);
            }
        }

        List<PlayerReference> players = new List<PlayerReference>();


        public override void LogicDoAfterConnections(NodeLogic logic)
        {
            var changePlayers = new List<PlayerReference>();
            foreach (var info in logic.InConnections.Where(c =>
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

            logic.OutConnections.First(c => c.ConnectionTyp == ConnectionType.PlayerGroup).PlayerGroup =
                players.ToArray();
        }

        public override void LogicTick(Actor self, NodeLogic logic)
        {
            var changePlayers = new List<PlayerReference>();
            foreach (var info in logic.InConnections.Where(c =>
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

            logic.OutConnections.First(c => c.ConnectionTyp == ConnectionType.PlayerGroup).PlayerGroup =
                players.ToArray();
        }
    }
}