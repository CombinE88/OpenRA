using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.Arithmetic
{
    public class SelectWidget : SimpleNodeWidget
    {
        DropDownButtonWidget selectyByList;
        string selectType;

        public SelectWidget(NodeEditorNodeScreenWidget screen) : base(screen)
        {
            WidgetName = "Arithmetic: Select";

            InConnections.Add(new InConnection(ConnectionType.ActorList, this));
            OutConnections.Add(new OutConnection(ConnectionType.Actor, this));

            //  Trigger Nodes
            List<string> triggerNodeTypes = new List<string>
            {
                "First",
                "Last",
                "Random",
                "Closest to Location",
                "Closest to Actor"
            };

            AddChild(selectyByList = new DropDownButtonWidget(screen.Snw.ModData));
            selectyByList.Bounds = new Rectangle(5, 5 + 26 + 26 + 26, 190, 25);

            Func<string, ScrollItemWidget, ScrollItemWidget> setupItem = (option, template) =>
            {
                var item = ScrollItemWidget.Setup(template, () => selectType == option, () =>
                {
                    selectType = option;
                    selectyByList.Text = selectType;

                    var inCons = InConnections.First();
                    InConnections = new List<InConnection>();
                    InConnections.Add(inCons);

                    if (selectType == "Closest to Location")
                    {
                        InConnections.Add(new InConnection(ConnectionType.Location, this));
                    }
                    else if (selectType == "Closest to Actor")
                    {
                        InConnections.Add(new InConnection(ConnectionType.Actor, this));
                    }
                });
                item.Get<LabelWidget>("LABEL").GetText = () => option;

                return item;
            };

            selectyByList.Text = "First";

            selectyByList.OnClick = () =>
            {
                var nodes = triggerNodeTypes;
                selectyByList.ShowDropDown("LABEL_DROPDOWN_TEMPLATE", 270, nodes, setupItem);
            };
        }

        public override void Tick()
        {
            selectyByList.Bounds = new Rectangle(FreeWidgetEntries.X, FreeWidgetEntries.Y + 25, FreeWidgetEntries.Width, 25);

            base.Tick();
        }

        public override void Draw()
        {
            base.Draw();

            string text = "Select by: ";
            Screen.Snw.FontRegular.DrawTextWithShadow(text, new float2(RenderBounds.X + FreeWidgetEntries.X + 2, RenderBounds.Y + FreeWidgetEntries.Y + 0),
                Color.White, Color.Black, 1);
        }
    }
}