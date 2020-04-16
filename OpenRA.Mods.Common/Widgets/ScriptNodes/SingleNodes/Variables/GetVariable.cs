using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.Variables
{
    public class GetVariableNode : NodeWidget
    {
        readonly DropDownButtonWidget availableVariables;

        public GetVariableNode(NodeEditorNodeScreenWidget screen, NodeInfo nodeInfo) : base(screen, nodeInfo)
        {
            AddChild(availableVariables = new DropDownButtonWidget(Screen.NodeScriptContainerWidget.ModData));
            Func<VariableInfo, ScrollItemWidget, ScrollItemWidget> setupItem = (option, template) =>
            {
                var item = ScrollItemWidget.Setup(template, () => VariableReference == option, () =>
                {
                    var predefinedType = VariableReference == null || VariableReference.VarType != option.VarType;
                    VariableReference = option;
                    Update(predefinedType);
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

            if (nodeInfo.VariableReference == null)
                return;

            var foundVariable = screen.VariableInfos.FirstOrDefault(v => v.VariableName == nodeInfo.VariableReference);

            if (foundVariable == null)
                return;

            availableVariables.Text = foundVariable.VariableName;
            VariableReference = foundVariable;
            Update(true);
        }

        public void Update(bool changedInputsOutputs)
        {
            availableVariables.Text = VariableReference.VariableName;

            if (!changedInputsOutputs)
                return;

            switch (VariableReference.VarType)
            {
                case VariableType.Actor:
                    OutConnections = new List<OutConnection>();
                    AddOutConnection(new OutConnection(ConnectionType.Actor, this)
                        {Actor = VariableReference.Actor});
                    break;
                case VariableType.Integer:
                    OutConnections = new List<OutConnection>();
                    AddOutConnection(new OutConnection(ConnectionType.Integer, this)
                        {Number = VariableReference.Number});
                    break;
                case VariableType.Location:
                    OutConnections = new List<OutConnection>();
                    AddOutConnection(new OutConnection(ConnectionType.Location, this)
                        {Location = VariableReference.Location});
                    break;
                case VariableType.Player:
                    OutConnections = new List<OutConnection>();
                    AddOutConnection(new OutConnection(ConnectionType.Player, this)
                        {Player = VariableReference.Player});
                    break;
                case VariableType.ActorInfo:
                    OutConnections = new List<OutConnection>();
                    AddOutConnection(new OutConnection(ConnectionType.ActorInfo, this)
                        {ActorInfo = VariableReference.ActorInfo});
                    break;
                case VariableType.ActorList:
                    OutConnections = new List<OutConnection>();
                    AddOutConnection(new OutConnection(ConnectionType.ActorList, this)
                        {ActorGroup = VariableReference.ActorGroup});
                    break;
                case VariableType.PlayerGroup:
                    OutConnections = new List<OutConnection>();
                    AddOutConnection(new OutConnection(ConnectionType.PlayerGroup, this)
                        {PlayerGroup = VariableReference.PlayerGroup});
                    break;
                case VariableType.CellArray:
                    OutConnections = new List<OutConnection>();
                    AddOutConnection(new OutConnection(ConnectionType.CellArray, this)
                        {CellArray = VariableReference.CellArray});
                    break;
                case VariableType.CellPath:
                    OutConnections = new List<OutConnection>();
                    AddOutConnection(new OutConnection(ConnectionType.CellPath, this)
                        {CellArray = VariableReference.CellArray});
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
        public VariableInfo SelectedSharedVariable;

        public GetVariableLogic(NodeInfo nodeInfo, IngameNodeScriptSystem ingameNodeScriptSystem) : base(nodeInfo,
            ingameNodeScriptSystem)
        {
            SelectedSharedVariable =
                ingameNodeScriptSystem.VariableInfos.First(v => v.VariableName == nodeInfo.VariableReference);
        }

        public void Refresh()
        {
            switch (SelectedSharedVariable.VarType)
            {
                case VariableType.Actor:
                    OutConnections.First(c => c.ConnectionTyp == ConnectionType.Actor).Actor =
                        SelectedSharedVariable.Actor;
                    break;
                case VariableType.ActorInfo:
                    OutConnections.First(c => c.ConnectionTyp == ConnectionType.ActorInfo).ActorInfo =
                        SelectedSharedVariable.ActorInfo;
                    break;
                case VariableType.Player:
                    OutConnections.First(c => c.ConnectionTyp == ConnectionType.Player).Player =
                        SelectedSharedVariable.Player;
                    break;
                case VariableType.PlayerGroup:
                    OutConnections.First(c => c.ConnectionTyp == ConnectionType.PlayerGroup).PlayerGroup =
                        SelectedSharedVariable.PlayerGroup;
                    break;
                case VariableType.Location:
                    OutConnections.First(c => c.ConnectionTyp == ConnectionType.Location).Location =
                        SelectedSharedVariable.Location;
                    break;
                case VariableType.CellArray:
                    OutConnections.First(c => c.ConnectionTyp == ConnectionType.CellArray).CellArray =
                        SelectedSharedVariable.CellArray;
                    break;
                case VariableType.CellPath:
                    OutConnections.First(c => c.ConnectionTyp == ConnectionType.CellArray).CellArray =
                        SelectedSharedVariable.CellArray;
                    break;
                case VariableType.Integer:
                    OutConnections.First(c => c.ConnectionTyp == ConnectionType.Integer).Number =
                        SelectedSharedVariable.Number;
                    break;
                case VariableType.ActorList:
                    OutConnections.First(c => c.ConnectionTyp == ConnectionType.ActorList).ActorGroup =
                        SelectedSharedVariable.ActorGroup;
                    break;
                case VariableType.Timer:
                    OutConnections.First(c => c.ConnectionTyp == ConnectionType.Objective).Number =
                        SelectedSharedVariable.Number;
                    break;
                case VariableType.Objective:
                    OutConnections.First(c => c.ConnectionTyp == ConnectionType.TimerConnection).Logic =
                        SelectedSharedVariable.Timer;
                    break;
                case VariableType.LocationRange:
                    OutConnections.First(c => c.ConnectionTyp == ConnectionType.LocationRange).Number =
                        SelectedSharedVariable.Number;
                    OutConnections.First(c => c.ConnectionTyp == ConnectionType.LocationRange).Location =
                        SelectedSharedVariable.Location;
                    break;
            }
        }
    }
}