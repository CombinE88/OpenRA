using System;
using System.Drawing;
using System.Linq;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes
{
    public class ActorInfoNodeWidget : SimpleNodeWidget
    {
        ScrollPanelWidget panel;
        ModData modData;

        public ActorInfoNodeWidget(NodeEditorNodeScreenWidget screen) : base(screen)
        {
            modData = screen.Snw.ModData;
            AddChild(panel = new ScrollPanelWidget(modData));
        }

        public override void Tick()
        {
            base.Tick();

            panel.Bounds = WidgetBackground;
        }

        public override void AddBasicNodePoints()
        {
            var outRecangle = new Rectangle(RenderBounds.X + GridPosX - 10, RenderBounds.Y + GridPosY + RenderBounds.Height / 2, 20, 20);
            var outConnection = new OutConnection(ConnectionType.ActorInfo, this);
            OutConnections.Add(new Tuple<Rectangle, OutConnection>(outRecangle, outConnection));
        }

        public override void Connection()
        {
            if (InConnections.First().Item2.conTyp == OutConnections.First().Item2.conTyp)
                OutConnections.First().Item2.Item = InConnections.First().Item2.Item;
        }

        public ScrollItemWidget AddScrollItem()
        {
            return new ScrollItemWidget(modData);
        }
    }
}