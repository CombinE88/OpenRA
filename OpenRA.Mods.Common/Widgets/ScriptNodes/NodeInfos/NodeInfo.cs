using System.Linq;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.NodeInfos
{
    public class NodeInfo : BasicNodeInfo
    {
        public NodeInfo(string nodeType, string nodeId, string nodeName) : base(nodeType, nodeId, nodeName)
        {
        }

        public virtual void WidgetInitialize(NodeWidget widget)
        {
        }

        public virtual void WidgetAddOutConConstructor(OutConnection connection, NodeWidget widget)
        {
        }
        public virtual void WidgetTick(NodeWidget widget)
        {
        }

        public virtual bool WidgetIsIncorrectConnected(NodeWidget widget)
        {
            return widget.InConnections.Any(inCon => inCon.In == null && inCon.ConnectionTyp != ConnectionType.Integer);
        }

        public virtual void WidgetDraw(NodeWidget widget)
        {
        }


        public virtual void LogicExecute(World world, NodeLogic logic)
        {
        }

        public virtual bool LogicCheckCondition(World world, NodeLogic logic)
        {
            return true;
        }

        public virtual void LogicDoAfterConnections(NodeLogic logic)
        {
        }

        public virtual void LogicTick(Actor self, NodeLogic logic)
        {
        }

        public virtual void LogicExecuteTick(Actor self, NodeLogic logic)
        {
        }
    }
}