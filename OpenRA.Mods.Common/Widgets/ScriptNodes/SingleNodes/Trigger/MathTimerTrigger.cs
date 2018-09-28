using System.Drawing;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.ActorNodes
{
    public class MAthTimerTriggerWidget : SimpleNodeWidget
    {
        CheckboxWidget checkBox;
        TextFieldWidget textField;

        public MAthTimerTriggerWidget(NodeEditorNodeScreenWidget screen) : base(screen)
        {
            WidgetName = "Trigger: Timer";

            AddChild(checkBox = new CheckboxWidget(screen.Snw.ModData));
            checkBox.Text = "Repeated";
            AddChild(textField = new TextFieldWidget());
            textField.Type = TextFieldType.Integer;

            var inRecangle = new Rectangle(0, 0, 0, 0);
            InConnections.Add(new InConnection(ConnectionType.Boolean, this) { InWidgetPosition = inRecangle });
            OutConnections.Add(new OutConnection(ConnectionType.Boolean, this) { InWidgetPosition = inRecangle });
        }

        public override void Tick()
        {
            checkBox.Bounds = new Rectangle(FreeWidgetEntries.X, FreeWidgetEntries.Y + 0, FreeWidgetEntries.Width, 25);
            checkBox.Bounds = new Rectangle(FreeWidgetEntries.X, FreeWidgetEntries.Y + 25, FreeWidgetEntries.Width, 25);
            base.Tick();
        }

        public override void Draw()
        {
            base.Draw();

            Screen.Snw.FontRegular.DrawTextWithShadow("Timer: ", new float2(FreeWidgetEntries.X, FreeWidgetEntries.X + 50), Color.White, Color.Black, 1);

            var con = InConnections[0];
            Screen.Snw.FontRegular.DrawTextWithShadow(con.In != null && con.In != null ? "trigger set" : "-no trigger-",
                new float2(con.InWidgetPosition.Location.X + 25, con.InWidgetPosition.Location.Y),
                Color.White,
                Color.Black, 1);

            var ocon = OutConnections[0];
            var text = "Trigger";
            Screen.Snw.FontRegular.DrawTextWithShadow(text,
                new float2(ocon.InWidgetPosition.Location.X - 5 - Screen.Snw.FontRegular.Measure(text).X, ocon.InWidgetPosition.Location.Y),
                Color.White,
                Color.Black, 1);
        }
    }
}