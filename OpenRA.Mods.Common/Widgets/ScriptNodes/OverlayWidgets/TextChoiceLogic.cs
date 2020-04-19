using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenRA.Graphics;
using OpenRA.Mods.Common.Widgets.ScriptNodes.Library;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.UiNodes;
using OpenRA.Widgets;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.OverlayWidgets
{
    public class TextChoiceLogic : ChromeLogic
    {
        readonly IngameNodeScriptSystem ingameNodeScriptSystem;
        readonly BackgroundWidget brw;
        readonly List<ButtonWidget> buttons = new List<ButtonWidget>();
        TextBoxSelectLogic currentTextBos;
        readonly LabelWidget label;
        readonly ModData modData;

        bool show;
        readonly World world;

        [ObjectCreator.UseCtorAttribute]
        public TextChoiceLogic(Widget widget, World world, WorldRenderer worldRenderer, ModData modData)
        {
            ingameNodeScriptSystem = world.WorldActor.TraitOrDefault<IngameNodeScriptSystem>();
            this.modData = modData;
            this.world = world;

            brw = widget.GetOrNull<BackgroundWidget>("CHOICEBACKGROUND");
            label = brw.GetOrNull<LabelWidget>("TEXT");

            brw.Visible = false;
        }

        public override void Tick()
        {
            if (!show)
            {
                brw.Visible = false;
                foreach (var nodeLogic in ingameNodeScriptSystem.NodeLogics.Where(
                    l => l.NodeType == NodeType.TextChoice))
                {
                    var textNode = (TextBoxSelectLogic) nodeLogic;
                    if (!textNode.Listen)
                        continue;

                    if (buttons.Any())
                        foreach (var button in buttons)
                        {
                            brw.RemoveChild(button);
                            button.Removed();
                        }

                    label.Text = textNode.Text;
                    label.VAlign = TextVAlign.Top;

                    currentTextBos = textNode;
                    show = true;

                    break;
                }
            }
            else
            {
                brw.Visible = true;

                if (!buttons.Any())
                    foreach (var option in currentTextBos.Options)
                    {
                        var newButton = new ButtonWidget(modData);
                        newButton.Text = option.Item2;
                        brw.AddChild(newButton);
                        buttons.Add(newButton);

                        newButton.OnClick = () =>
                        {
                            brw.Visible = false;
                            world.SetPauseState(false);
                            currentTextBos.ExecuteBranch(option.Item1);

                            show = false;
                        };
                    }

                for (var i = 0; i < buttons.Count; i++)
                    buttons[i].Bounds = new Rectangle(brw.RenderBounds.Width / (buttons.Count + 1) * (i + 1) - 50,
                        brw.RenderBounds.Height - 20 - 25, 100, 25);
            }
        }
    }
}