using System;
using System.Collections.Generic;
using System.Linq;
using OpenRA.Mods.Common.Traits;
using OpenRA.Mods.Common.Widgets.ScriptNodes.Library;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes
{
    public class NodeLogic
    {
        public readonly CompareItem? Item;

        public readonly CompareMethod? Methode;
        public readonly string NodeId;
        public readonly NodeInfo NodeInfo;
        public readonly string NodeName;
        public readonly NodeType NodeType;

        public List<InConnection> InConnections = new List<InConnection>();

        public IngameNodeScriptSystem IngameNodeScriptSystem;
        public List<OutConnection> OutConnections = new List<OutConnection>();

        public NodeLogic(NodeInfo nodeInfo, IngameNodeScriptSystem ingameNodeScriptSystem)
        {
            IngameNodeScriptSystem = ingameNodeScriptSystem;
            NodeId = nodeInfo.NodeId;
            NodeType = nodeInfo.NodeType;
            NodeInfo = nodeInfo;
            NodeName = nodeInfo.NodeName;
            Methode = nodeInfo.Method;
            Item = nodeInfo.Item;
        }

        public void AddOutConnectionReferences()
        {
            SetOuts(BuildOutConnections(NodeInfo));
        }

        public void AddInConnectionReferences()
        {
            SetIns(BuildInConnections(NodeInfo));
        }

        public void SetOuts(List<OutConnection> o)
        {
            OutConnections = o;
        }

        public void SetIns(List<InConnection> i)
        {
            InConnections = i;
        }

        List<OutConnection> BuildOutConnections(NodeInfo nodeInfo)
        {
            var readyOutCons = new List<OutConnection>();

            foreach (var conRef in nodeInfo.OutConnectionsReference)
            {
                var connection = new OutConnection(conRef.ConTyp);

                connection.String = conRef.String ?? null;
                connection.Number = conRef.Number ?? null;
                connection.Location = conRef.Location ?? null;
                connection.Strings = conRef.Strings ?? null;
                connection.Player = conRef.Player ?? null;
                connection.ActorInfo = conRef.ActorInfo ?? null;
                connection.CellArray = conRef.CellArray ?? null;
                if (conRef.ActorId != null)
                {
                    var actor = IngameNodeScriptSystem.World.WorldActor.Trait<SpawnMapActors>().Actors
                        .FirstOrDefault(a => a.Key == conRef.ActorId).Value;
                    if (actor != null)
                        connection.Actor = actor;
                }

                var actorList = IngameNodeScriptSystem.World.WorldActor.Trait<SpawnMapActors>().Actors;
                var act = new List<Actor>();

                if (conRef.ActorIds != null && conRef.ActorIds.Any())
                {
                    foreach (var prev in conRef.ActorIds)
                    {
                        Actor ret;

                        if (!actorList.TryGetValue(prev, out ret))
                            continue;

                        if (ret.Disposed)
                            continue;

                        act.Add(ret);
                    }

                    connection.ActorGroup = act.ToArray();
                }

                connection.ConnectionId = conRef.ConnectionId;
                connection.Logic = this;

                readyOutCons.Add(connection);
            }

            return readyOutCons;
        }

        List<InConnection> BuildInConnections(NodeInfo nodeInfo)
        {
            var readyOutCons = new List<InConnection>();

            foreach (var conRef in nodeInfo.InConnectionsReference)
            {
                var connection = new InConnection(conRef.ConTyp);

                connection.ConnectionId = conRef.ConnectionId ?? null;
                var referenceNode =
                    IngameNodeScriptSystem.NodeLogics.FirstOrDefault(n => n.NodeId == conRef.WidgetReferenceId);
                var referenceConnection = referenceNode != null
                    ? referenceNode.OutConnections.FirstOrDefault(c => c.ConnectionId == conRef.WidgetNodeReference)
                    : null;
                connection.In = referenceConnection ?? null;
                if (referenceConnection != null)
                    connection.In.Logic = referenceConnection.Logic ?? null;

                readyOutCons.Add(connection);
            }

            return readyOutCons;
        }

        public virtual void Execute(World world)
        {
        }

        public virtual bool CheckCondition(World world)
        {
            return true;
        }

        public virtual void DoAfterConnections()
        {
        }

        public virtual void Tick(Actor self)
        {
        }

        public virtual void ExecuteTick(Actor self)
        {
            foreach (var conn in InConnections.Where(c => c.ConnectionTyp == ConnectionType.Exec))
                if (conn.Execute)
                {
                    Execute(self.World);
                    conn.Execute = false;
                    break;
                }
        }

        protected static void ForwardExec(NodeLogic self, int connectionNumber = 0)
        {
            var connections = self.OutConnections.Where(o => o.ConnectionTyp == ConnectionType.Exec).ToArray();

            if (!connections.Any())
                return;

            var oCon = connections[connectionNumber];

            if (oCon == null)
                return;

            foreach (var node in self.IngameNodeScriptSystem.NodeLogics.Where(n =>
                n.InConnections.FirstOrDefault(c => c.ConnectionTyp == ConnectionType.Exec) != null))
            {
                var inCon = node.InConnections.FirstOrDefault(c =>
                    c.ConnectionTyp == ConnectionType.Exec && c.In == oCon);
                if (inCon != null)
                    inCon.Execute = true;
            }
        }

        protected static OutConnection GetLinkedConnectionFromInConnection(NodeLogic logic,
            List<InConnection> connections, ConnectionType connectionType,
            int position)
        {
            var inConnections = connections.Where(c => c.ConnectionTyp == connectionType).ToArray();
            if (!inConnections.Any() || inConnections.Length < position)
                throw new Exception(
                    logic.NodeName + " has no " + connectionType + " connection on position " + position);

            var connection = inConnections[position].In;
            
            if(connection.Out == null)
                throw new Exception(
                    logic.NodeName + ": Connection " + connection.ConnectionId + " is not connected ");


            return connection;
        }
    }
}