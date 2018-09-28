using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.InfoNodes
{
    public class InfoStringsWidget : SimpleNodeWidget
    {
        List<TextFieldWidget> textFields = new List<TextFieldWidget>();

        public InfoStringsWidget(NodeEditorNodeScreenWidget screen) : base(screen)
        {
            var textF = new TextFieldWidget { Text = "" };
            textFields.Add(textF);
            AddChild(textF);
        }

        public override void Tick()
        {
            var splitHi = (RenderBounds.Height + 20) / (OutConnections.Count + 1);
            for (int i = 0; i < textFields.Count; i++)
            {
                textFields[i].Bounds = new Rectangle(FreeWidgetEntries.X, splitHi * (i + 1), FreeWidgetEntries.Width, 25);
            }

            var emptyFields = textFields.Where(f => f.Text == "").ToArray();
            if (emptyFields.Length > 1)
                foreach (var field in emptyFields)
                {
                    if (field != emptyFields.First())
                    {
                        textFields.Remove(field);
                        RemoveChild(field);
                    }
                }

            if (emptyFields.Length < 1)
            {
                var textF = new TextFieldWidget { Text = "" };
                textFields.Add(textF);
                AddChild(textF);
            }

            if (textFields.Count > OutConnections.Count)
                OutConnections.Add(new OutConnection(ConnectionType.String, this));
            if (OutConnections.Any() && textFields.Count < OutConnections.Count)
                OutConnections.Remove(OutConnections.Last());

            base.Tick();
        }
    }
}