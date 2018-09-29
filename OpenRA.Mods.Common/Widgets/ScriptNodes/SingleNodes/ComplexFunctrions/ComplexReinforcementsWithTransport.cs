using System.Drawing;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.ComplexFunctrions
{
    public class ComplexReinforcementsWithTransportWidget : SimpleNodeWidget
    {
        public ComplexReinforcementsWithTransportWidget(NodeEditorNodeScreenWidget screen) : base(screen)
        {
            WidgetName = "Scripts: Reinforcements (Transport)";

            InConnections.Add(new InConnection(ConnectionType.Strings, this));
            InConnections.Add(new InConnection(ConnectionType.Player, this));
            InConnections.Add(new InConnection(ConnectionType.Location, this));
            InConnections.Add(new InConnection(ConnectionType.Location, this));
            InConnections.Add(new InConnection(ConnectionType.ActorInfo, this));
            InConnections.Add(new InConnection(ConnectionType.Boolean, this));
            OutConnections.Add(new OutConnection(ConnectionType.Boolean, this));
            OutConnections.Add(new OutConnection(ConnectionType.ActorList, this));
        }

        public override void Draw()
        {
            base.Draw();

            var conPos = InConnections[0].InWidgetPosition;
            string text = "List Actortypes";
            Screen.Snw.FontRegular.DrawTextWithShadow(text, new float2(conPos.X + 22, conPos.Y + 2),
                Color.White, Color.Black, 1);

            conPos = InConnections[1].InWidgetPosition;
            text = "Player Owner";
            Screen.Snw.FontRegular.DrawTextWithShadow(text, new float2(conPos.X + 22, conPos.Y + 2),
                Color.White, Color.Black, 1);

            conPos = InConnections[2].InWidgetPosition;
            text = "Location Start";
            Screen.Snw.FontRegular.DrawTextWithShadow(text, new float2(conPos.X + 22, conPos.Y + 2),
                Color.White, Color.Black, 1);

            conPos = InConnections[3].InWidgetPosition;
            text = "Location Target";
            Screen.Snw.FontRegular.DrawTextWithShadow(text, new float2(conPos.X + 22, conPos.Y + 2),
                Color.White, Color.Black, 1);

            conPos = InConnections[4].InWidgetPosition;
            text = "Actortype Transport";
            Screen.Snw.FontRegular.DrawTextWithShadow(text, new float2(conPos.X + 22, conPos.Y + 2),
                Color.White, Color.Black, 1);

            conPos = InConnections[5].InWidgetPosition;
            text = "Trigger";
            Screen.Snw.FontRegular.DrawTextWithShadow(text, new float2(conPos.X + 22, conPos.Y + 2),
                Color.White, Color.Black, 1);

            conPos = OutConnections[0].InWidgetPosition;
            text = "Trigger: Finished";
            Screen.Snw.FontRegular.DrawTextWithShadow(text, new float2(conPos.X - 2 - Screen.Snw.FontRegular.Measure(text).X, conPos.Y + 2),
                Color.White, Color.Black, 1);

            conPos = OutConnections[1].InWidgetPosition;
            text = "Actor List: Actors";
            Screen.Snw.FontRegular.DrawTextWithShadow(text, new float2(conPos.X - 2 - Screen.Snw.FontRegular.Measure(text).X, conPos.Y + 2),
                Color.White, Color.Black, 1);
        }
    }
}