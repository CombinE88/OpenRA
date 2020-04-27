using System;
using System.Collections.Generic;
using System.Linq;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.NodeInfos.GroupInfos
{
    public class GroupActorGroupInfo : NodeInfo
    {
        public new static Dictionary<string, BuildNodeConstructorInfo> NodeConstructorInformation =
            new Dictionary<string, BuildNodeConstructorInfo>()
            {
                {
                    "GroupActorGroup", new BuildNodeConstructorInfo
                    {
                        Nesting = new[] {"Actor/Player Group"},
                        Name = "Group Actors",

                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.ActorList, "Group of grouped actor's")
                        }
                    }
                },
            };

        public GroupActorGroupInfo(string nodeType, string nodeId, string nodeName) : base(nodeType, nodeId, nodeName)
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
                var inCon = new InConnection(ConnectionType.Actor, widget);
                widget.AddInConnection(inCon);
            }
            else if (none.Length > 1)
            {
                foreach (var con in none)
                    if (con != none.First())
                        widget.InConnections.Remove(con);
            }
        }

        List<Actor> actors = new List<Actor>();

        public override void LogicDoAfterConnections(NodeLogic logic)
        {
            var changeActors = new List<Actor>();
            foreach (var info in logic.InConnections.Where(c =>
            {
                if (c.ConnectionTyp != ConnectionType.Actor)
                    return false;

                if (c.In == null)
                    return false;

                if (c.In.Actor == null)
                    return false;

                if (actors.Contains(c.In.Actor))
                    return false;

                return true;
            }))
                actors.Add(info.In.Actor);

            actors = changeActors;

            logic.OutConnections.First(c => c.ConnectionTyp == ConnectionType.ActorList).ActorGroup = actors.ToArray();
        }

        public override void LogicTick(Actor self, NodeLogic logic)
        {
            var changeActors = new List<Actor>();
            foreach (var info in logic.InConnections.Where(c =>
            {
                if (c.ConnectionTyp != ConnectionType.Actor)
                    return false;

                if (c.In == null)
                    return false;

                if (c.In.Actor == null)
                    return false;

                if (actors.Contains(c.In.Actor))
                    return false;

                return true;
            }))
                actors.Add(info.In.Actor);

            actors = changeActors;

            logic.OutConnections.First(c => c.ConnectionTyp == ConnectionType.ActorList).ActorGroup = actors.ToArray();
        }
    }
}