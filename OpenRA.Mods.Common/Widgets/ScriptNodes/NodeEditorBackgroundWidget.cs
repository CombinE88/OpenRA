using System.Collections.Generic;
using System.Drawing;
using OpenRA.Widgets;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes
{
    public class NodeEditorBackgroundWidget : Widget
    {
        // Background
        public readonly string Background = "panel-black";

        public ScriptNodeWidget Snw;
        NodeEditorNodeScreenWidget screenWidget;
        List<ButtonWidget> Buttons = new List<ButtonWidget>();

        [ObjectCreator.UseCtor]
        public NodeEditorBackgroundWidget(ScriptNodeWidget snw)
        {
            Snw = snw;

            Children.Add(screenWidget = new NodeEditorNodeScreenWidget(Snw, this));

            Bounds = new Rectangle(100, 100, Snw.RenderBounds.Width - 200, Snw.RenderBounds.Height - 200);

            for (int i = 0; i < 6; i++)
            {
                var button = new ButtonWidget(snw.ModData);
                Buttons.Add(button);
                button.Bounds = new Rectangle(5, 5 + i * 26, 115, 25);
                AddChild(button);
            }

            Buttons[0].OnClick = () => { screenWidget.AddNode(NodeType.ActorOutPut); };
            Buttons[0].Text = "Add Actor Output";
            Buttons[1].OnClick = () => { screenWidget.AddNode(NodeType.PathNode); };
            Buttons[1].Text = "Add Path Output";
            Buttons[2].OnClick = () => { screenWidget.AddNode(NodeType.PlayerOutput); };
            Buttons[2].Text = "Add Player Output";
            Buttons[3].OnClick = () => { screenWidget.AddNode(NodeType.LocationOutput); };
            Buttons[3].Text = "Add Location Output";
            Buttons[4].OnClick = () => { screenWidget.AddNode(NodeType.CelLArrayOutput); };
            Buttons[4].Text = "Add Cell Array Output";
            Buttons[5].OnClick = () => { screenWidget.AddNode(NodeType.ActorInfluence); };
            Buttons[5].Text = "Add CreateActor Influence";
        }

        public override void Tick()
        {
            Bounds = new Rectangle(100, 100, Snw.RenderBounds.Width - 200, Snw.RenderBounds.Height - 200);
        }

        public override bool HandleMouseInput(MouseInput mi)
        {
            if (!EventBounds.Contains(mi.Location))
            {
                return false;
            }

            return true;
        }

        public override void Draw()
        {
            WidgetUtils.DrawPanel(Background, new Rectangle(RenderBounds.X - 3, RenderBounds.Y - 3, RenderBounds.Width + 6, RenderBounds.Height + 6));
            // WidgetUtils.FillRectWithColor(new Rectangle(RenderBounds.X - 3, RenderBounds.Y - 3, RenderBounds.Width + 6, RenderBounds.Height + 6), Color.Black);
            // WidgetUtils.FillRectWithColor(new Rectangle(RenderBounds.X, RenderBounds.Y, RenderBounds.Width, RenderBounds.Height), Color.DarkGray);
        }
    }
}