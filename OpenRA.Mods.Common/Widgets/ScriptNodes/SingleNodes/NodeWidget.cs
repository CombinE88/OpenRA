using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenRA.Mods.Common.Traits;
using OpenRA.Mods.Common.Widgets.ScriptNodes.Library;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes
{
    public class NodeWidget : BasicNodeWidget
    {
        int inConnectionCounter = 1;
        public CompareItem? Item = null;
        public CompareMethod? Methode = null;
        int outConnectionCounter = 1;
        public VariableInfo VariableReference = null;

        public NodeWidget(NodeEditorNodeScreenWidget screen, NodeInfo nodeInfo) : base(screen)
        {
            if (nodeInfo == null)
                throw new NotImplementedException();

            NodeInfo = nodeInfo;

            OffsetPosX = nodeInfo.OffsetPosX ?? screen.MouseOffsetCoordinates.X;
            OffsetPosY = nodeInfo.OffsetPosY ?? screen.MouseOffsetCoordinates.Y;

            Bounds = new Rectangle(GridPosX, GridPosY, 200 + SizeX, 150 + SizeY);

            if (nodeInfo.NodeId != null)
            {
                NodeID = nodeInfo.NodeId;
            }
            else
            {
                screen.RunningNodeId += 1;
                NodeID = "ND" + (screen.RunningNodeId < 10
                             ? "0" + screen.RunningNodeId
                             : Screen.RunningNodeId.ToString());
            }

            NodeType = nodeInfo.NodeType;
            NodeName = nodeInfo.NodeName ?? NodeType.ToString();
            NodeIDTextfield.Text = NodeName;
        }

        public NodeInfo BuildNodeInfo()
        {
            var nodeInfo = new NodeInfo(NodeType, NodeID, NodeName);

            nodeInfo.InConnectionsReference = ReferenceInConnections();
            nodeInfo.OutConnectionsReference = ReferenceOutConnections();
            nodeInfo.OffsetPosX = OffsetPosX;
            nodeInfo.OffsetPosY = OffsetPosY;
            nodeInfo.Method = Methode;
            nodeInfo.Item = Item;
            nodeInfo.VariableReference = VariableReference != null ? VariableReference.VariableName : null;

            return nodeInfo;
        }

        public void AddOutConnection(OutConnection connection)
        {
            connection.ConnectionId = "Con" + outConnectionCounter;
            outConnectionCounter++;

            SetEmptyOutConnection(connection);

            OutConnections.Add(connection);
        }

        public void AddInConnection(InConnection connection)
        {
            connection.ConnectionId = "Con" + inConnectionCounter;
            inConnectionCounter++;

            SetEmptyInConnection(connection);

            InConnections.Add(connection);
        }

        void SetEmptyOutConnection(OutConnection connection)
        {
            if (connection.ConnectionId == null)
            {
                connection.ConnectionId = inConnectionCounter.ToString();
                inConnectionCounter++;
            }

            if (connection.Widget == null)
                connection.Widget = this;
        }

        void SetEmptyInConnection(InConnection connection)
        {
            if (connection.ConnectionId == null)
            {
                connection.ConnectionId = inConnectionCounter.ToString();
                inConnectionCounter++;
            }

            if (connection.Widget == null)
                connection.Widget = this;
        }

        public void AddOutConnectionReferences()
        {
            SetOuts(BuildOutConnections(NodeInfo));

            foreach (var connection in OutConnections) AddOutConConstructor(connection);

            var count = 0;
            foreach (var node in OutConnections)
            {
                int c;
                int.TryParse(node.ConnectionId.Replace("Con", ""), out c);
                count = Math.Max(c, count);
            }

            outConnectionCounter = count + 1;
        }

        public void AddInConnectionReferences()
        {
            SetIns(BuildInConnections(NodeInfo));

            var count = 0;
            foreach (var node in InConnections)
            {
                int c;
                int.TryParse(node.ConnectionId.Replace("Con", ""), out c);
                count = Math.Max(c, count);
            }

            inConnectionCounter = count + 1;
        }

        List<OutConReference> ReferenceOutConnections()
        {
            var outConRef = new List<OutConReference>();

            foreach (var outCon in OutConnections)
            {
                var outRef = new OutConReference();
                outRef.String = outCon.String ?? null;
                outRef.Number = outCon.Number ?? null;
                outRef.Location = outCon.Location ?? null;
                outRef.Strings = outCon.Strings != null ? new List<string>(outCon.Strings).ToArray() : null;
                outRef.Player = outCon.Player ?? null;
                outRef.ActorInfo = outCon.ActorInfo ?? null;
                outRef.CellArray = outCon.CellArray != null ? new List<CPos>(outCon.CellArray) : null;
                outRef.ActorPreview = outCon.ActorPrev ?? null;
                outRef.ActorPreviews = outCon.ActorPreviews != null
                    ? new List<EditorActorPreview>(outCon.ActorPreviews).ToArray()
                    : null;
                outRef.ConnectionId = outCon.ConnectionId;
                outRef.ConTyp = outCon.ConnectionTyp;

                outConRef.Add(outRef);
            }

            return outConRef;
        }

        List<InConReference> ReferenceInConnections()
        {
            var outConRef = new List<InConReference>();

            foreach (var inCon in InConnections)
            {
                var inRef = new InConReference();
                inRef.ConnectionId = inCon.ConnectionId;
                inRef.ConTyp = inCon.ConnectionTyp;
                if (inCon.In != null)
                {
                    inRef.WidgetNodeReference = inCon.In.ConnectionId ?? null;
                    inRef.WidgetReferenceId = inCon.In.Widget.NodeID;
                }

                outConRef.Add(inRef);
            }

            return outConRef;
        }

        List<OutConnection> BuildOutConnections(NodeInfo nodeInfo)
        {
            var readyOutCons = new List<OutConnection>();

            foreach (var conRef in nodeInfo.OutConnectionsReference)
            {
                var connection = new OutConnection(conRef.ConTyp, this);

                connection.String = conRef.String ?? null;
                connection.Number = conRef.Number ?? null;
                connection.Location = conRef.Location ?? null;
                connection.Strings = conRef.Strings ?? null;
                connection.Player = conRef.Player ?? null;
                connection.ActorInfo = conRef.ActorInfo ?? null;
                connection.CellArray = conRef.CellArray ?? null;
                connection.ActorPreviews = conRef.ActorPreviews ?? null;
                connection.ConnectionId = conRef.ConnectionId;

                readyOutCons.Add(connection);
            }

            return readyOutCons;
        }

        List<InConnection> BuildInConnections(NodeInfo nodeInfo)
        {
            var readyOutCons = new List<InConnection>();

            foreach (var conRef in nodeInfo.InConnectionsReference)
            {
                var connection = new InConnection(conRef.ConTyp, this);

                connection.ConnectionId = conRef.ConnectionId ?? null;
                var referenceNode = Screen.Nodes.FirstOrDefault(n => n.NodeID == conRef.WidgetReferenceId);
                var referenceConnection = referenceNode != null
                    ? referenceNode.OutConnections.FirstOrDefault(c => c.ConnectionId == conRef.WidgetNodeReference)
                    : null;
                connection.In = referenceConnection ?? null;

                readyOutCons.Add(connection);
            }

            return readyOutCons;
        }

        public virtual void AddOutConConstructor(OutConnection outConnection)
        {
        }
    }

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
    }
}