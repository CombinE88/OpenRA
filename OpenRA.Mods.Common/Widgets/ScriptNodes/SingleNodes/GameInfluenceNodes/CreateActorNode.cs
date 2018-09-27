using System.Drawing;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.GameInfluenceNodes
{
    public class CreateActorNodeWidget : SimpleNodeWidget
    {
        public CreateActorNodeWidget(NodeEditorNodeScreenWidget screen) : base(screen)
        {
            WidgetName = "Influence: CreateActor";
            var inRecangle = new Rectangle(0, 0, 0, 0);
            InConnections.Add(new InConnection(ConnectionType.ActorInfo, this) { InWidgetPosition = inRecangle });
            InConnections.Add(new InConnection(ConnectionType.Player, this) { InWidgetPosition = inRecangle });
            InConnections.Add(new InConnection(ConnectionType.Location, this) { InWidgetPosition = inRecangle });
            InConnections.Add(new InConnection(ConnectionType.Integer, this) { InWidgetPosition = inRecangle });
            InConnections.Add(new InConnection(ConnectionType.Boolean, this) { InWidgetPosition = inRecangle });
        }

        public override void Draw()
        {
            base.Draw();

            var con = InConnections[0];
            Screen.Snw.FontRegular.DrawTextWithShadow(con.In != null && con.In.ActorInfo != null ? con.In.ActorInfo.Name : "-no actorInfo-",
                new float2(con.InWidgetPosition.Location.X + 25, con.InWidgetPosition.Location.Y + 25),
                Color.White,
                Color.Black, 1);

            con = InConnections[1];
            Screen.Snw.FontRegular.DrawTextWithShadow(con.In != null && con.In.Player != null ? con.In.Player.Name : "-no player-",
                new float2(con.InWidgetPosition.Location.X + 25, con.InWidgetPosition.Location.Y + 25),
                Color.White,
                Color.Black, 1);

            con = InConnections[2];
            Screen.Snw.FontRegular.DrawTextWithShadow(con.In != null && con.In.Location != CPos.Zero ? con.In.Location.ToString() : "-no location-",
                new float2(con.InWidgetPosition.Location.X + 25, con.InWidgetPosition.Location.Y + 25),
                Color.White,
                Color.Black, 1);

            con = InConnections[3];
            Screen.Snw.FontRegular.DrawTextWithShadow(con.In != null ? con.In.Number.ToString() : "-no facing-",
                new float2(con.InWidgetPosition.Location.X + 25, con.InWidgetPosition.Location.Y + 25),
                Color.White,
                Color.Black, 1);

            con = InConnections[4];
            Screen.Snw.FontRegular.DrawTextWithShadow(con.In != null ? con.In.Boolean.ToString() : "-no bool-",
                new float2(con.InWidgetPosition.Location.X + 25, con.InWidgetPosition.Location.Y + 25),
                Color.White,
                Color.Black, 1);
        }
    }
}