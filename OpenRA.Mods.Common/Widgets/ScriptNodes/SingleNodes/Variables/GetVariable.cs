using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.TriggerNodes;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.Variables
{
    public class GetVariableNode : NodeWidget
    {
        readonly DropDownButtonWidget availableVariables;
        public VatiableInfo SelectedVariable = new VatiableInfo {VarType = VariableType.Actor};

        public GetVariableNode(NodeEditorNodeScreenWidget screen, NodeInfo nodeInfo) : base(screen, nodeInfo)
        {
            AddChild(availableVariables = new DropDownButtonWidget(Screen.Snw.ModData));
            availableVariables.Text = "CHOOSE VARIABLE";
            Func<VatiableInfo, ScrollItemWidget, ScrollItemWidget> setupItem = (option, template) =>
            {
                var item = ScrollItemWidget.Setup(template, () => SelectedVariable == option, () =>
                {
                    var predefinedType = SelectedVariable.VarType;
                    SelectedVariable = option;
                    Update(predefinedType != option.VarType);
                });

                item.Get<LabelWidget>("LABEL").GetText = () => option.VariableName;
                return item;
            };

            availableVariables.OnClick = () =>
            {
                var variables = screen.VariableInfos;
                availableVariables.ShowDropDown("LABEL_DROPDOWN_TEMPLATE", 270, variables, setupItem);
            };
            availableVariables.Bounds =
                new Rectangle(FreeWidgetEntries.X, FreeWidgetEntries.Y + 75, FreeWidgetEntries.Width, 25);
        }

        public void Update(bool changedInputsOutputs)
        {
            availableVariables.Text = SelectedVariable.VariableName;

            if (!changedInputsOutputs)
                return;
            
            switch (SelectedVariable.VarType)
            {
                case VariableType.Actor:
                    OutConnections = new List<OutConnection>();
                    AddOutConnection(new OutConnection(ConnectionType.Actor, this)
                        {Actor = SelectedVariable.Actor});
                    break;
                case VariableType.Integer:
                    OutConnections = new List<OutConnection>();
                    AddOutConnection(new OutConnection(ConnectionType.Integer, this)
                        {Number = SelectedVariable.Number});
                    break;
                case VariableType.Location:
                    OutConnections = new List<OutConnection>();
                    AddOutConnection(new OutConnection(ConnectionType.Location, this)
                        {Location = SelectedVariable.Location});
                    break;
                case VariableType.Player:
                    OutConnections = new List<OutConnection>();
                    AddOutConnection(new OutConnection(ConnectionType.Player, this)
                        {Player = SelectedVariable.Player});
                    break;
                case VariableType.ActorInfo:
                    OutConnections = new List<OutConnection>();
                    AddOutConnection(new OutConnection(ConnectionType.ActorInfo, this)
                        {ActorInfo = SelectedVariable.ActorInfo});
                    break;
                case VariableType.ActorList:
                    OutConnections = new List<OutConnection>();
                    AddOutConnection(new OutConnection(ConnectionType.ActorList, this)
                        {ActorGroup = SelectedVariable.ActorGroup});
                    break;
                case VariableType.PlayerGroup:
                    OutConnections = new List<OutConnection>();
                    AddOutConnection(new OutConnection(ConnectionType.PlayerGroup, this)
                        {PlayerGroup = SelectedVariable.PlayerGroup});
                    break;
                case VariableType.CellArray:
                    OutConnections = new List<OutConnection>();
                    AddOutConnection(new OutConnection(ConnectionType.CellArray, this)
                        {CellArray = SelectedVariable.CellArray});
                    break;
                case VariableType.CellPath:
                    OutConnections = new List<OutConnection>();
                    AddOutConnection(new OutConnection(ConnectionType.CellPath, this)
                        {CellArray = SelectedVariable.CellArray});
                    break;
                case VariableType.Timer:
                    OutConnections = new List<OutConnection>();
                    AddOutConnection(new OutConnection(ConnectionType.TimerConnection, this));
                    break;
                case VariableType.Objective:
                    OutConnections = new List<OutConnection>();
                    AddOutConnection(new OutConnection(ConnectionType.Objective, this));
                    break;
            }
        }
    }

    public class GetVariableLogic : NodeLogic
    {
        VatiableInfo selectedVariable;

        public GetVariableLogic(NodeInfo nodeinfo, IngameNodeScriptSystem insc) : base(nodeinfo, insc)
        {
        }

        public override void Tick(Actor self)
        {
            OutConnections.First(c => c.ConTyp == ConnectionType.ActorInfo).ActorInfo = selectedVariable.ActorInfo;
            OutConnections.First(c => c.ConTyp == ConnectionType.Location).Location = selectedVariable.Location;
            OutConnections.First(c => c.ConTyp == ConnectionType.Player).Player = selectedVariable.Player;
            OutConnections.First(c => c.ConTyp == ConnectionType.PlayerGroup).PlayerGroup =
                selectedVariable.PlayerGroup;
            OutConnections.First(c => c.ConTyp == ConnectionType.Location).Location = selectedVariable.Location;
            OutConnections.First(c => c.ConTyp == ConnectionType.CellArray).CellArray = selectedVariable.CellArray;
            OutConnections.First(c => c.ConTyp == ConnectionType.Integer).Number = selectedVariable.Number;
            OutConnections.First(c => c.ConTyp == ConnectionType.ActorList).ActorGroup = selectedVariable.ActorGroup;
            OutConnections.First(c => c.ConTyp == ConnectionType.Objective).Number = selectedVariable.Number;
            OutConnections.First(c => c.ConTyp == ConnectionType.TimerConnection).Logic = selectedVariable.Timer;
        }
    }
}