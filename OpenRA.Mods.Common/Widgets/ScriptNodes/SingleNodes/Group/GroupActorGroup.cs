using System;
using System.Collections.Generic;
using System.Linq;
using OpenRA.Mods.Common.Widgets.ScriptNodes.Library;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.Group
{
    public class GroupActorGroup : NodeWidget
    {
        public new static Dictionary<string, BuildNodeConstructorInfo> NodeConstructorInformation =
            new Dictionary<string, BuildNodeConstructorInfo>()
            {
                {
                    "GroupActorGroup", new BuildNodeConstructorInfo
                    {
                        LogicClass = typeof(GroupActorLogic),
                        Nesting = new[] {"Actor/Player Group"},
                        Name = "Group Actors",

                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.ActorList, "Group of grouped actor's")
                        }
                    }
                },
            };

        public GroupActorGroup(NodeEditorNodeScreenWidget screen, NodeInfo nodeInfo) : base(screen, nodeInfo)
        {
            IsIncorrectConnected = () => InConnections.Any(inCon => inCon.In != null);
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
            {
                foreach (var con in none)
                    if (con != none.First())
                        InConnections.Remove(con);
            }

            base.Tick();
        }
    }

    public class GroupActorInfoGroup : NodeWidget
    {
        public new static Dictionary<string, BuildNodeConstructorInfo> NodeConstructorInformation =
            new Dictionary<string, BuildNodeConstructorInfo>()
            {
                {
                    "GroupActorInfoGroup", new BuildNodeConstructorInfo
                    {
                        LogicClass = typeof(GroupActorInfoLogic),
                        Nesting = new[] {"Actor/Player Group"},
                        Name = "Group Actor Types",

                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.ActorInfoArray, "")
                        }
                    }
                },
            };

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
            {
                foreach (var con in none)
                    if (con != none.First())
                        InConnections.Remove(con);
            }

            base.Tick();
        }
    }

    public class GroupActorLogic : NodeLogic
    {
        List<Actor> actors = new List<Actor>();

        public GroupActorLogic(NodeInfo nodeInfo, IngameNodeScriptSystem ingameNodeScriptSystem) : base(nodeInfo,
            ingameNodeScriptSystem)
        {
        }

        public override void DoAfterConnections()
        {
            var changeActors = new List<Actor>();
            foreach (var info in InConnections.Where(c =>
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

            OutConnections.First(c => c.ConnectionTyp == ConnectionType.ActorList).ActorGroup = actors.ToArray();
        }

        public override void Tick(Actor self)
        {
            var changeActors = new List<Actor>();
            foreach (var info in InConnections.Where(c =>
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

            OutConnections.First(c => c.ConnectionTyp == ConnectionType.ActorList).ActorGroup = actors.ToArray();
        }
    }

    public class GroupActorInfoLogic : NodeLogic
    {
        List<ActorInfo> actors = new List<ActorInfo>();

        public GroupActorInfoLogic(NodeInfo nodeInfo, IngameNodeScriptSystem ingameNodeScriptSystem) : base(nodeInfo,
            ingameNodeScriptSystem)
        {
        }

        public override void DoAfterConnections()
        {
            var changeActors = new List<ActorInfo>();
            var incon = InConnections.Where(c =>
            {
                if (c.ConnectionTyp != ConnectionType.ActorInfo)
                    return false;

                if (c.In == null)
                    return false;

                if (c.In.ActorInfo == null)
                    return false;

                return true;
            });

            foreach (var info in incon) changeActors.Add(info.In.ActorInfo);

            actors = changeActors;

            OutConnections.First(c => c.ConnectionTyp == ConnectionType.ActorInfoArray).ActorInfos = actors.ToArray();
        }

        public override void Tick(Actor self)
        {
            var incon = InConnections.Where(c =>
            {
                if (c.ConnectionTyp != ConnectionType.ActorInfo)
                    return false;

                if (c.In == null)
                    return false;

                if (c.In.ActorInfo == null)
                    return false;

                return true;
            });

            var changeActors = incon.Select(info => info.In.ActorInfo).ToList();

            actors = changeActors;

            OutConnections.First(c => c.ConnectionTyp == ConnectionType.ActorInfoArray).ActorInfos = actors.ToArray();
        }
    }
}