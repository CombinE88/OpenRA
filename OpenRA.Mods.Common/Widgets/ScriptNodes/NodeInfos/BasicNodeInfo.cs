using System;
using System.Collections.Generic;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.NodeInfos
{
    public abstract class BasicNodeInfo
    {
        public static Dictionary<string, BuildNodeConstructorInfo> NodeConstructorInformation;

        public List<InConReference> InConnectionsReference;
        public List<OutConReference> OutConnectionsReference;

        public string Item;
        public string Method;

        public string NodeId;
        public string NodeName;
        public string NodeType;

        public int? OffsetPosX;
        public int? OffsetPosY;
        public string VariableReference;

        public BasicNodeInfo(
            string nodeType,
            string nodeId,
            string nodeName)
        {
            NodeType = nodeType;
            NodeName = nodeName;
            NodeId = nodeId;
        }
    }
}