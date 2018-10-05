using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenRA.Widgets;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes
{
    public class NodeWidget : BasicNodeWidget
    {
        int inConnectionCounter = 1;
        int outConnectionCounter = 1;

        public NodeWidget(NodeEditorNodeScreenWidget screen, NodeInfo nodeInfo) : base(screen)
        {
            if (nodeInfo == null)
                throw new NotImplementedException();

            NodeInfo = nodeInfo;

            OffsetPosX = nodeInfo.OffsetPosX ?? screen.CenterCoordinates.X;
            OffsetPosY = nodeInfo.OffsetPosY ?? screen.CenterCoordinates.Y;

            Bounds = new Rectangle(GridPosX, GridPosY, 200 + SizeX, 150 + SizeY);

            if (nodeInfo.NodeID != null)
                NodeID = nodeInfo.NodeID;
            else
            {
                screen.NodeID += 1;
                NodeID = "ND" + (screen.NodeID < 10 ? "0" + screen.NodeID : Screen.NodeID.ToString());
            }

            NodeType = nodeInfo.NodeType;
            NodeName = nodeInfo.NodeName ?? NodeType.ToString();
            NodeIDTextfield.Text = NodeName;
        }

        public NodeInfo BuildNodeInfo()
        {
            var nodeInfo = new NodeInfo(NodeType, NodeID, NodeName);

            nodeInfo.InConnections = ReferenceInConnections();
            nodeInfo.OutConnections = ReferenceOutConnections();
            nodeInfo.OffsetPosX = OffsetPosX;
            nodeInfo.OffsetPosY = OffsetPosY;

            return nodeInfo;
        }

        public void AddOutConnection(OutConnection connection)
        {
            connection.ConnectionId = "Con" + outConnectionCounter.ToString();
            outConnectionCounter++;

            SetEmptyOutConnection(connection);

            OutConnections.Add(connection);
        }

        public void AddInConnection(InConnection connection)
        {
            connection.ConnectionId = "Con" + inConnectionCounter.ToString();
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

            foreach (var connection in OutConnections)
            {
                AddOutConConstructor(connection);
            }

            int count = 0;
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

            int count = 0;
            foreach (var node in InConnections)
            {
                int c;
                int.TryParse(node.ConnectionId.Replace("Con", ""), out c);
                count = Math.Max(c, count);
            }
        }

        List<OutConReference> ReferenceOutConnections()
        {
            List<OutConReference> outConRef = new List<OutConReference>();

            foreach (var outCon in OutConnections)
            {
                var outRef = new OutConReference();
                outRef.String = outCon.String ?? null;
                outRef.Number = outCon.Number ?? null;
                outRef.Location = outCon.Location ?? null;
                outRef.Strings = outCon.Strings ?? null;
                outRef.Player = outCon.Player ?? null;
                outRef.ActorInfo = outCon.ActorInfo ?? null;
                outRef.CellArray = outCon.CellArray ?? null;
                outRef.ConnectionId = outCon.ConnectionId;
                outRef.ConTyp = outCon.ConTyp;

                outConRef.Add(outRef);
            }

            return outConRef;
        }

        List<InConReference> ReferenceInConnections()
        {
            List<InConReference> outConRef = new List<InConReference>();

            foreach (var inCon in InConnections)
            {
                var inRef = new InConReference();
                inRef.ConnectionId = inCon.ConnectionId;
                inRef.ConTyp = inCon.ConTyp;
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
            List<OutConnection> readyOutCons = new List<OutConnection>();

            foreach (var conRef in nodeInfo.OutConnections)
            {
                var connection = new OutConnection(conRef.ConTyp, this);

                connection.String = conRef.String ?? null;
                connection.Number = conRef.Number ?? null;
                connection.Location = conRef.Location ?? null;
                connection.Strings = conRef.Strings ?? null;
                connection.Player = conRef.Player ?? null;
                connection.ActorInfo = conRef.ActorInfo ?? null;
                connection.CellArray = conRef.CellArray ?? null;
                connection.ConnectionId = conRef.ConnectionId;

                readyOutCons.Add(connection);
            }

            return readyOutCons;
        }

        List<InConnection> BuildInConnections(NodeInfo nodeInfo)
        {
            List<InConnection> readyOutCons = new List<InConnection>();

            foreach (var conRef in nodeInfo.InConnections)
            {
                var connection = new InConnection(conRef.ConTyp, this);

                connection.ConnectionId = conRef.ConnectionId ?? null;
                var referenceNode = Screen.Nodes.FirstOrDefault(n => n.NodeID == conRef.WidgetReferenceId);
                var referenceConnection = referenceNode != null ? referenceNode.OutConnections.FirstOrDefault(c => c.ConnectionId == conRef.WidgetNodeReference) : null;
                connection.In = referenceConnection ?? null;
                // connection.In = Screen.Nodes.First(bsw => bsw.NodeID == conRef.WidgetReferenceId).OutConnections.First(oc => oc.ConnectionId == conRef.WidgetNodeReference);

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
        public readonly string NodeId;
        public readonly NodeType NodeType;
        public readonly string NodeName;
        public readonly NodeInfo NodeInfo;

        public List<InConnection> InConnections = new List<InConnection>();
        public List<OutConnection> OutConnections = new List<OutConnection>();

        public IngameNodeScriptSystem Insc;

        public NodeLogic(NodeInfo nodeinfo, IngameNodeScriptSystem insc)
        {
            Insc = insc;
            NodeId = nodeinfo.NodeID;
            NodeType = nodeinfo.NodeType;
            NodeInfo = nodeinfo;
            NodeName = nodeinfo.NodeName;
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
            List<OutConnection> readyOutCons = new List<OutConnection>();

            foreach (var conRef in nodeInfo.OutConnections)
            {
                var connection = new OutConnection(conRef.ConTyp);

                connection.String = conRef.String ?? null;
                connection.Number = conRef.Number ?? null;
                connection.Location = conRef.Location ?? null;
                connection.Strings = conRef.Strings ?? null;
                connection.Player = conRef.Player ?? null;
                connection.ActorInfo = conRef.ActorInfo ?? null;
                connection.CellArray = conRef.CellArray ?? null;
                connection.ConnectionId = conRef.ConnectionId;
                connection.Logic = this;

                readyOutCons.Add(connection);
            }

            return readyOutCons;
        }

        List<InConnection> BuildInConnections(NodeInfo nodeInfo)
        {
            List<InConnection> readyOutCons = new List<InConnection>();

            foreach (var conRef in nodeInfo.InConnections)
            {
                var connection = new InConnection(conRef.ConTyp);

                connection.ConnectionId = conRef.ConnectionId ?? null;
                var referenceNode = Insc.NodeLogics.FirstOrDefault(n => n.NodeId == conRef.WidgetReferenceId);
                var referenceConnection = referenceNode != null ? referenceNode.OutConnections.FirstOrDefault(c => c.ConnectionId == conRef.WidgetNodeReference) : null;
                connection.In = referenceConnection ?? null;
                if (referenceConnection != null)
                    connection.In.Logic = referenceConnection.Logic ?? null;
                // connection.In = Screen.Nodes.First(bsw => bsw.NodeID == conRef.WidgetReferenceId).OutConnections.First(oc => oc.ConnectionId == conRef.WidgetNodeReference);

                readyOutCons.Add(connection);
            }

            return readyOutCons;
        }

        public virtual void Execute(World world)
        {
        }

        public virtual void DoAfterConnections()
        {
        }

        public virtual void Tick(Actor self)
        {
        }
    }
}