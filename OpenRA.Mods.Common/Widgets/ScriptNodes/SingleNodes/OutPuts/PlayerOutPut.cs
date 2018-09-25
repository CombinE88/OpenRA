using System;
using System.Drawing;
using System.Linq;
using OpenRA.Mods.Common.Traits;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.OutPuts
{
    public class PlayerOutPutWidget : SimpleNodeWidget
    {
        private DropDownButtonWidget playerSelection;
        PlayerReference selectedOwner;
        OutConnection outconnection;
        string text;

        public PlayerOutPutWidget(NodeEditorNodeScreenWidget screen) : base(screen)
        {
            AddChild(playerSelection = new DropDownButtonWidget(screen.Snw.ModData));

            var inRecangle = new Rectangle(0, 0, 0, 0);
            outconnection = new OutConnection(ConnectionType.Player, this);
            OutConnections.Add(new Tuple<Rectangle, OutConnection>(inRecangle, outconnection));

            var editorLayer = screen.Snw.World.WorldActor.Trait<EditorActorLayer>();

            selectedOwner = editorLayer.Players.Players.Values.First();
            Func<PlayerReference, ScrollItemWidget, ScrollItemWidget> setupItem = (option, template) =>
            {
                var item = ScrollItemWidget.Setup(template, () => selectedOwner == option, () =>
                {
                    selectedOwner = option;

                    playerSelection.Text = selectedOwner.Name;
                    playerSelection.TextColor = selectedOwner.Color.RGB;
                });

                item.Get<LabelWidget>("LABEL").GetText = () => option.Name;
                item.GetColor = () => option.Color.RGB;

                return item;
            };

            playerSelection.OnClick = () =>
            {
                var owners = editorLayer.Players.Players.Values.OrderBy(p => p.Name);
                playerSelection.ShowDropDown("LABEL_DROPDOWN_TEMPLATE", 270, owners, setupItem);
                };

            playerSelection.Text = selectedOwner.Name;
            playerSelection.TextColor = selectedOwner.Color.RGB;
        }

        public override void Tick()
        {
            playerSelection.Bounds = new Rectangle(FreeWidgetEntries.X, FreeWidgetEntries.Y, FreeWidgetEntries.Width, 25);
            outconnection.Item.Player = selectedOwner;
            base.Tick();
        }

        public override void Draw()
        {
            if (outconnection.Item.Player != null)
            {
                text = outconnection.Item.Player.Name;
                var meassureText = screen.Snw.FontRegular.Measure(text);
                screen.Snw.FontRegular.DrawTextWithShadow(text, new float2(outconnection.Out.Item1.X - meassureText.X - 5, outconnection.Out.Item1.X),
                    Color.White, Color.Black, 1);
            }

            base.Draw();
        }
    }
}