using System;
using System.Collections.Generic;
using System.Drawing;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes
{
    public class NodeWidget : BasicNodeWidget
    {
        public NodeWidget(NodeEditorNodeScreenWidget screen, NodeInfo nodeInfo) : base(screen)
        {
            if (nodeInfo == null)
                throw new NotImplementedException();

            PosX = nodeInfo.PosX ?? screen.CorrectCenterCoordinates.X;
            PosY = nodeInfo.PosY ?? screen.CorrectCenterCoordinates.X;
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

            InConnections = nodeInfo.InConnections ?? new List<InConnection>();
            OutConnections = nodeInfo.OutConnections ?? new List<OutConnection>();
        }
    }
}