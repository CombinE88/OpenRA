using System.Drawing;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.ActorNodes
{
    public class ActorTriggerOnIldeWidget : SimpleNodeWidget
    {
        CheckboxWidget checkBox;

        public ActorTriggerOnIldeWidget(NodeEditorNodeScreenWidget screen) : base(screen)
        {
            WidgetName = "Trigger: Actor idle";

            AddChild(checkBox = new CheckboxWidget(screen.Snw.ModData));
            checkBox.Text = "Repeated";

            var inRecangle = new Rectangle(0, 0, 0, 0);
            InConnections.Add(new InConnection(ConnectionType.Actor, this) { InWidgetPosition = inRecangle });
            OutConnections.Add(new OutConnection(ConnectionType.Boolean, this) { InWidgetPosition = inRecangle });
        }

        public override void Tick()
        {
            checkBox.Bounds = new Rectangle(FreeWidgetEntries.X, FreeWidgetEntries.Y + 0, FreeWidgetEntries.Width, 25);
            base.Tick();
        }

        public override void Draw()
        {
            base.Draw();

            var con = InConnections[0];
            Screen.Snw.FontRegular.DrawTextWithShadow(con.In != null && con.In.Actor != null ? con.In.Actor.Info.Name : "-no actor-",
                new float2(con.InWidgetPosition.Location.X + 25, con.InWidgetPosition.Location.Y),
                Color.White,
                Color.Black, 1);

            var ocon = OutConnections[0];
            var text = "Trigger: actor becomes idle";
            Screen.Snw.FontRegular.DrawTextWithShadow(text,
                new float2(ocon.InWidgetPosition.Location.X - 5 - Screen.Snw.FontRegular.Measure(text).X, ocon.InWidgetPosition.Location.Y),
                Color.White,
                Color.Black, 1);
        }
    }
}