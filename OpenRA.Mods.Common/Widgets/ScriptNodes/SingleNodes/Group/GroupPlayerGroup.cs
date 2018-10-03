using System.Collections.Generic;
using System.Linq;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.InfoNodes;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.Group
{
    public class GroupPlayerGroup : NodeWidget
    {
        public GroupPlayerGroup(NodeEditorNodeScreenWidget screen, NodeInfo nodeInfo) : base(screen, nodeInfo)
        {
            InConTexts.Add("Player");
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
                foreach (var con in none)
                {
                    if (con != none.First())
                        InConnections.Remove(con);
                }

            base.Tick();
        }
    }

    public class GroupPlayerLogic : NodeLogic
    {
        public GroupPlayerLogic(NodeInfo nodeinfo, IngameNodeScriptSystem insc) : base(nodeinfo, insc)
        {
        }

        public override void DoAfterConnections()
        {
            List<PlayerReference> players = new List<PlayerReference>();
            foreach (var info in InConnections.Where(c => c.ConTyp == ConnectionType.Player))
            {
                if (info.In != null && info.In.Player != null)
                    players.Add(info.In.Player);
            }

            OutConnections.First(c => c.ConTyp == ConnectionType.PlayerGroup).PlayerGroup = players.ToArray();
        }
    }
}