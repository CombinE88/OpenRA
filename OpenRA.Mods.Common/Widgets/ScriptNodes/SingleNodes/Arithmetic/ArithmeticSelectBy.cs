using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.Arithmetic
{
    public class SelectByWidget : SimpleNodeWidget
    {
        DropDownButtonWidget selectyByList;
        string selectType;

        public SelectByWidget(NodeEditorNodeScreenWidget screen) : base(screen)
        {
            WidgetName = "Arithmetic: Select by";

            InConnections.Add(new InConnection(ConnectionType.ActorList, this));
            OutConnections.Add(new OutConnection(ConnectionType.ActorList, this));

            //  Trigger Nodes
            List<string> triggerNodeTypes = new List<string>
            {
                "Owner",
                "Location",
                "Types",
                "TargetTypes"
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

                    if (selectType == "Owner")
                    {
                        InConnections.Add(new InConnection(ConnectionType.Player, this));
                    }

                    if (selectType == "Location")
                    {
                        InConnections.Add(new InConnection(ConnectionType.Location, this));
                        InConnections.Add(new InConnection(ConnectionType.Integer, this));
                    }

                    if (selectType == "Types")
                    {
                        InConnections.Add(new InConnection(ConnectionType.Strings, this));
                    }

                    if (selectType == "TargetTypes")
                    {
                        InConnections.Add(new InConnection(ConnectionType.Strings, this));
                    }
                });
                item.Get<LabelWidget>("LABEL").GetText = () => option;

                return item;
            };

            selectyByList.Text = "Owner";

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