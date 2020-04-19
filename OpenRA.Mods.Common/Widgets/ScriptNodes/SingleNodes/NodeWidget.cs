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
        public CompareMethod? Method = null;
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
            nodeInfo.Method = Method;
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

            connection.AddTooltip();
        }

        public void AddInConnection(InConnection connection)
        {
            connection.ConnectionId = "Con" + inConnectionCounter;
            inConnectionCounter++;

            SetEmptyInConnection(connection);

            InConnections.Add(connection);

            connection.AddTooltip();
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
}