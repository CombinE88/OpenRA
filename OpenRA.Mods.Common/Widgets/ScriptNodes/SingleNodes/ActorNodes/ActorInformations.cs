using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.ActorNodes
{
    public class ActorInformationsWidget : SimpleNodeWidget
    {
        DropDownButtonWidget selectyByList;
        string selectType;

        public ActorInformationsWidget(NodeEditorNodeScreenWidget screen) : base(screen)
        {
            WidgetName = "Info: Actor info";

            InConnections.Add(new InConnection(ConnectionType.Actor, this));

            //  Trigger Nodes
            List<string> triggerNodeTypes = new List<string>
            {
                "Owner",
                "Location",
                "Target Types",
                "Health",
                "Facing",
                "Cost",
                "Name",
                "Internal Name",
                "Ammo Count",
                "Maximum Ammo Count",
                "Cruise Altitude"

            };

            AddChild(selectyByList = new DropDownButtonWidget(screen.Snw.ModData));
            selectyByList.Bounds = new Rectangle(5, 5 + 26 + 26 + 26, 190, 25);

            Func<string, ScrollItemWidget, ScrollItemWidget> setupItem = (option, template) =>
            {
                var item = ScrollItemWidget.Setup(template, () => selectType == option, () =>
                {
                    selectType = option;
                    selectyByList.Text = selectType;

                    OutConnections = new List<OutConnection>();

                    if (selectType == "Owner")
                    {
                        OutConnections.Add(new OutConnection(ConnectionType.Player, this));
                    }
                    else if (selectType == "Location")
                    {
                        OutConnections.Add(new OutConnection(ConnectionType.Location, this));
                    }
                    else if (selectType == "Target Types")
                    {
                        OutConnections.Add(new OutConnection(ConnectionType.Strings, this));
                    }
                    else if (selectType == "Health"
                             || selectType == "Facing"
                             || selectType == "Cost"
                             || selectType == "Ammo Count"
                             || selectType == "Maximum Ammo Count"
                             || selectType == "Cruise Altitude")
                    {
                        OutConnections.Add(new OutConnection(ConnectionType.Integer, this));
                    }
                    else if (selectType == "Name")
                    {
                        OutConnections.Add(new OutConnection(ConnectionType.String, this));
                    }
                    else if (selectType == "Internal Name")
                    {
                        OutConnections.Add(new OutConnection(ConnectionType.String, this));
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