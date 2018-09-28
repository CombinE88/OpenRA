using System.Drawing;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.ActorNodes
{
    public class MoveActorWidget : SimpleNodeWidget
    {
        public MoveActorWidget(NodeEditorNodeScreenWidget screen) : base(screen)
        {
            WidgetName = "Actor: Move";

            var inRecangle = new Rectangle(0, 0, 0, 0);
            InConnections.Add(new InConnection(ConnectionType.Actor, this) { InWidgetPosition = inRecangle });
            InConnections.Add(new InConnection(ConnectionType.Location, this) { InWidgetPosition = inRecangle });
            InConnections.Add(new InConnection(ConnectionType.Boolean, this) { InWidgetPosition = inRecangle });
            OutConnections.Add(new OutConnection(ConnectionType.Boolean, this) { InWidgetPosition = inRecangle });
        }

        public override void Draw()
        {
            base.Draw();

            var con = InConnections[0];
            Screen.Snw.FontRegular.DrawTextWithShadow(con.In != null && con.In.Actor != null ? con.In.Actor.Info.Name : "-no actor-",
                new float2(con.InWidgetPosition.Location.X + 25, con.InWidgetPosition.Location.Y),
                Color.White,
                Color.Black, 1);

            con = InConnections[1];
            Screen.Snw.FontRegular.DrawTextWithShadow(con.In != null && con.In != null ? "trigger set" : "-no trigger-",
                new float2(con.InWidgetPosition.Location.X + 25, con.InWidgetPosition.Location.Y),
                Color.White,
                Color.Black, 1);

            var ocon = OutConnections[0];
            var text = "Trigger: location reached";
            Screen.Snw.FontRegular.DrawTextWithShadow(text,
                new float2(ocon.InWidgetPosition.Location.X - 5 - Screen.Snw.FontRegular.Measure(text).X, ocon.InWidgetPosition.Location.Y),
                Color.White,
                Color.Black, 1);
        }
    }
}