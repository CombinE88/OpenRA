using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenRA.Mods.Common.Traits;
using OpenRA.Mods.Common.Widgets.ScriptNodes.EditorNodeBrushes;
using OpenRA.Widgets;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.InfoNodes
{
    public class TextBoxSelectNode : NodeWidget
    {
        List<ButtonWidget> parralelButtons = new List<ButtonWidget>();

        public TextBoxSelectNode(NodeEditorNodeScreenWidget screen, NodeInfo nodeInfo) : base(screen, nodeInfo)
        {
            ButtonWidget addButton;
            AddChild(addButton = new ButtonWidget(screen.Snw.ModData));
            addButton.Bounds = new Rectangle(FreeWidgetEntries.X + WidgetEntries.Width - 10, FreeWidgetEntries.Y + 21, WidgetEntries.Width - 20, 20);
            addButton.Text = "+";
            addButton.OnClick = () =>
            {
                var outcon = new OutConnection(ConnectionType.String, this);
                var incon = new InConnection(ConnectionType.Exec, this);
                AddOutConnection(outcon);
                AddOutConConstructor(outcon);
                AddInConnection(incon);
            };
        }

        public override void AddOutConConstructor(OutConnection connection)
        {
            base.AddOutConConstructor(connection);

            var button = new ButtonWidget(Screen.Snw.ModData);
            button.Text = "-";

            AddChild(button);
            parralelButtons.Add(button);

            button.OnClick = () =>
            {
                for (int j = 0; j < parralelButtons.Count; j++)
                {
                    if (parralelButtons[j] == button)
                    {
                        RemoveChild(parralelButtons[j]);

                        parralelButtons.RemoveAt(j);
                        OutConnections.RemoveAt(j);
                        InConnections.RemoveAt(j + 2);

                        break;
                    }
                }
            };
        }
    }

    class TextBoxSelectLogic : NodeLogic
    {
        public TextBoxSelectLogic(NodeInfo nodeinfo, IngameNodeScriptSystem insc) : base(nodeinfo, insc)
        {
        }
    }
}