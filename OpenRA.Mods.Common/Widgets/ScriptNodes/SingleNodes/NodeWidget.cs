using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

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

            PosX = Screen.RenderBounds.Width / 2;
            PosY = Screen.RenderBounds.Height / 2;
            OffsetPosX = nodeInfo.OffsetPosX ?? 0;
            OffsetPosY = nodeInfo.OffsetPosY ?? 0;

            GridPosX = PosX - screen.CenterCoordinates.X + OffsetPosX;
            GridPosY = PosY - screen.CenterCoordinates.Y + OffsetPosY;

            Bounds = new Rectangle(GridPosX + OffsetPosX, GridPosY + OffsetPosY, 200 + SizeX, 150 + SizeY);

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

        public virtual void AddOutConnection(OutConnection connection)
        {
            connection.ConnectionID = "Con" + outConnectionCounter.ToString();
            outConnectionCounter++;
            OutConnections.Add(connection);
        }

        public virtual void AddInConnection(InConnection connection)
        {
            connection.ConnectionID = "Con" + inConnectionCounter.ToString();
            inConnectionCounter++;
            InConnections.Add(connection);
        }

        public void AddOutConnectionReferences()
        {
            SetOuts(BuildOutConnections(NodeInfo));

            int count = 0;
            foreach (var node in OutConnections)
            {
                int c;
                int.TryParse(node.ConnectionID, out c);
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
                int.TryParse(node.ConnectionID, out c);
                count = Math.Max(c, count);
            }

            inConnectionCounter = count + 1;
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
                outRef.ConnecitonName = outCon.ConnectionID;
                outRef.conTyp = outCon.ConTyp;

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
                inRef.ConnecitonName = inCon.ConnectionID;
                inRef.conTyp = inCon.ConTyp;
                inRef.WidgetNodeReference = inCon.In.ConnectionID;
                inRef.WidgetReferenceID = inCon.Widget.NodeID;
                inRef.conTyp = inCon.ConTyp;

                outConRef.Add(inRef);
            }

            return outConRef;
        }

        List<OutConnection> BuildOutConnections(NodeInfo nodeInfo)
        {
            List<OutConnection> readyOutCons = new List<OutConnection>();

            foreach (var conRef in nodeInfo.OutConnections)
            {
                var connection = new OutConnection(conRef.conTyp, this);

                connection.String = conRef.String ?? null;
                connection.Number = conRef.Number ?? null;
                connection.Location = conRef.Location ?? null;
                connection.Strings = conRef.Strings ?? null;
                connection.Player = conRef.Player ?? null;
                connection.ActorInfo = conRef.ActorInfo ?? null;
                connection.CellArray = conRef.CellArray ?? null;
                connection.ConnectionID = conRef.ConnecitonName;

                readyOutCons.Add(connection);

                AddOutConConstructor(connection);
            }

            return readyOutCons;
        }

        List<InConnection> BuildInConnections(NodeInfo nodeInfo)
        {
            List<InConnection> readyOutCons = new List<InConnection>();

            foreach (var conRef in nodeInfo.InConnections)
            {
                var connection = new InConnection(conRef.conTyp, this);

                connection.ConnectionID = conRef.ConnecitonName ?? null;
                connection.In = Screen.Nodes.First(bsw => bsw.NodeID == conRef.WidgetReferenceID).OutConnections.First(oc => oc.ConnectionID == conRef.WidgetNodeReference);

                readyOutCons.Add(connection);
            }

            return readyOutCons;
        }

        public virtual void AddOutConConstructor(OutConnection outConnection)
        {
        }
    }
}