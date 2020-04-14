using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenRA.Mods.Common.Traits;
using OpenRA.Primitives;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.Variables
{
    public class GetVariableNode : NodeWidget
    {
        DropDownButtonWidget AvailableVariables;
        VatiableInfo selectedVariable;

        public GetVariableNode(NodeEditorNodeScreenWidget screen, NodeInfo nodeInfo) : base(screen, nodeInfo)
        {
            AddChild(AvailableVariables = new DropDownButtonWidget(Screen.Snw.ModData));

            Func<VatiableInfo, ScrollItemWidget, ScrollItemWidget> setupItem = (option, template) =>
            {
                var item = ScrollItemWidget.Setup(template, () => selectedVariable == option, () =>
                {
                    selectedVariable = option;
                    Update();
                });

                item.Get<LabelWidget>("LABEL").GetText = () => selectedVariable.VariableName;
                return item;
            };

            AvailableVariables.OnClick = () =>
            {
                AvailableVariables.ShowDropDown("LABEL_DROPDOWN_TEMPLATE", 270, screen.VariableInfos, setupItem);
            };
            AvailableVariables.Bounds =
                new Rectangle(FreeWidgetEntries.X, FreeWidgetEntries.Y + 51, FreeWidgetEntries.Width, 25);
        }

        public void Update()
        {
            AvailableVariables.Text = selectedVariable.VariableName;

            switch (selectedVariable.VarType)
            {
                case VariableType.Actor:
                    OutConnections = new List<OutConnection>();
                    AddOutConnection(new OutConnection(ConnectionType.Actor, this)
                        {Actor = selectedVariable.Actor});
                    break;
                case VariableType.Integer:
                    OutConnections = new List<OutConnection>();
                    AddOutConnection(new OutConnection(ConnectionType.Integer, this)
                        {Number = selectedVariable.Number});
                    break;
                case VariableType.Location:
                    OutConnections = new List<OutConnection>();
                    AddOutConnection(new OutConnection(ConnectionType.Location, this)
                        {Location = selectedVariable.Location});
                    break;
                case VariableType.Player:
                    OutConnections = new List<OutConnection>();
                    AddOutConnection(new OutConnection(ConnectionType.Player, this)
                        {Player = selectedVariable.Player});
                    break;
                case VariableType.ActorInfo:
                    OutConnections = new List<OutConnection>();
                    AddOutConnection(new OutConnection(ConnectionType.ActorInfo, this)
                        {ActorInfo = selectedVariable.ActorInfo});
                    break;
                case VariableType.ActorList:
                    OutConnections = new List<OutConnection>();
                    AddOutConnection(new OutConnection(ConnectionType.ActorList, this)
                        {ActorGroup = selectedVariable.ActorGroup});
                    break;
            }
        }
    }
}