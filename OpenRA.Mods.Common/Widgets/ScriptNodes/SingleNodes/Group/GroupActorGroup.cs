using System.Collections.Generic;
using System.Linq;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.InfoNodes;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.Group
{
    public class GroupActorGroup : NodeWidget
    {
        public GroupActorGroup(NodeEditorNodeScreenWidget screen, NodeInfo nodeInfo) : base(screen, nodeInfo)
        {
        }

        public override void Tick()
        {
            var none = InConnections.Where(c => c.In == null).ToArray();

            if (none.Length < 1)
            {
                var inCon = new InConnection(ConnectionType.Actor, this);
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

    public class GroupActorInfoGroup : NodeWidget
    {
        public GroupActorInfoGroup(NodeEditorNodeScreenWidget screen, NodeInfo nodeInfo) : base(screen, nodeInfo)
        {
        }

        public override void Tick()
        {
            var none = InConnections.Where(c => c.In == null).ToArray();

            if (none.Length < 1)
            {
                var inCon = new InConnection(ConnectionType.ActorInfo, this);
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

    public class GroupActorLogic : NodeLogic
    {
        List<Actor> actors = new List<Actor>();

        public GroupActorLogic(NodeInfo nodeinfo, IngameNodeScriptSystem insc) : base(nodeinfo, insc)
        {
        }

        public override void DoAfterConnections()
        {
            var changeActors = new List<Actor>();
            foreach (var info in InConnections.Where(c =>
            {
                if (c.ConTyp != ConnectionType.Actor)
                    return false;

                if (c.In == null)
                    return false;

                if (c.In.Actor == null)
                    return false;

                if (actors.Contains(c.In.Actor))
                    return false;

                return true;
            }))
            {
                actors.Add(info.In.Actor);
            }

            actors = changeActors;

            OutConnections.First(c => c.ConTyp == ConnectionType.ActorList).ActorGroup = actors.ToArray();
        }

        public override void Tick(Actor self)
        {
            var changeActors = new List<Actor>();
            foreach (var info in InConnections.Where(c =>
            {
                if (c.ConTyp != ConnectionType.Actor)
                    return false;

                if (c.In == null)
                    return false;

                if (c.In.Actor == null)
                    return false;

                if (actors.Contains(c.In.Actor))
                    return false;

                return true;
            }))
            {
                actors.Add(info.In.Actor);
            }

            actors = changeActors;

            OutConnections.First(c => c.ConTyp == ConnectionType.ActorList).ActorGroup = actors.ToArray();
        }
    }

    public class GroupActorInfoLogic : NodeLogic
    {
        List<ActorInfo> actors = new List<ActorInfo>();

        public GroupActorInfoLogic(NodeInfo nodeinfo, IngameNodeScriptSystem insc) : base(nodeinfo, insc)
        {
        }

        public override void DoAfterConnections()
        {
            var changeActors = new List<ActorInfo>();
            var incon = InConnections.Where(c =>
            {
                if (c.ConTyp != ConnectionType.ActorInfo)
                    return false;

                if (c.In == null)
                    return false;

                if (c.In.ActorInfo == null)
                    return false;

                return true;
            });

            foreach (var info in incon)
            {
                changeActors.Add(info.In.ActorInfo);
            }

            actors = changeActors;

            OutConnections.First(c => c.ConTyp == ConnectionType.ActorInfos).ActorInfos = actors.ToArray();
        }

        public override void Tick(Actor self)
        {
            var changeActors = new List<ActorInfo>();

            var incon = InConnections.Where(c =>
            {
                if (c.ConTyp != ConnectionType.ActorInfo)
                    return false;

                if (c.In == null)
                    return false;

                if (c.In.ActorInfo == null)
                    return false;

                return true;
            });

            foreach (var info in incon)
            {
                changeActors.Add(info.In.ActorInfo);
            }

            actors = changeActors;

            OutConnections.First(c => c.ConTyp == ConnectionType.ActorInfos).ActorInfos = actors.ToArray();
        }
    }
}