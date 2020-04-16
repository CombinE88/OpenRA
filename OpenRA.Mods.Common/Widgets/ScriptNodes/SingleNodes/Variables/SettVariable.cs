using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.TriggerNodes;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.Variables
{
    public class SetVariableNode : NodeWidget
    {
        readonly DropDownButtonWidget availableVariables;

        public SetVariableNode(NodeEditorNodeScreenWidget screen, NodeInfo nodeInfo) : base(screen, nodeInfo)
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
                    InConnections = new List<InConnection>();
                    AddInConnection(new InConnection(ConnectionType.Actor, this));
                    break;
                case VariableType.Integer:
                    InConnections = new List<InConnection>();
                    AddInConnection(new InConnection(ConnectionType.Integer, this));
                    break;
                case VariableType.Location:
                    InConnections = new List<InConnection>();
                    AddInConnection(new InConnection(ConnectionType.Location, this));
                    break;
                case VariableType.Player:
                    InConnections = new List<InConnection>();
                    AddInConnection(new InConnection(ConnectionType.Player, this));
                    break;
                case VariableType.ActorInfo:
                    InConnections = new List<InConnection>();
                    AddInConnection(new InConnection(ConnectionType.ActorInfo, this));
                    break;
                case VariableType.ActorList:
                    InConnections = new List<InConnection>();
                    AddInConnection(new InConnection(ConnectionType.ActorList, this));
                    break;
                case VariableType.PlayerGroup:
                    InConnections = new List<InConnection>();
                    AddInConnection(new InConnection(ConnectionType.PlayerGroup, this));
                    break;
                case VariableType.CellArray:
                    InConnections = new List<InConnection>();
                    AddInConnection(new InConnection(ConnectionType.CellArray, this));
                    break;
                case VariableType.CellPath:
                    InConnections = new List<InConnection>();
                    AddInConnection(new InConnection(ConnectionType.CellPath, this));
                    break;
                case VariableType.Timer:
                    InConnections = new List<InConnection>();
                    AddInConnection(new InConnection(ConnectionType.TimerConnection, this));
                    break;
                case VariableType.Objective:
                    InConnections = new List<InConnection>();
                    AddInConnection(new InConnection(ConnectionType.Objective, this));
                    break;
            }

            AddInConnection(new InConnection(ConnectionType.Exec, this));
        }
    }

    public class SetVariableLogic : NodeLogic
    {
        readonly VariableInfo selectedSharedVariable;

        public SetVariableLogic(NodeInfo nodeInfo, IngameNodeScriptSystem IngameNodeScriptSystem) : base(nodeInfo,
            IngameNodeScriptSystem)
        {
            selectedSharedVariable =
                IngameNodeScriptSystem.VariableInfos.First(v => v.VariableName == nodeInfo.VariableReference);
        }

        public override void Execute(World world)
        {
            switch (selectedSharedVariable.VarType)
            {
                case VariableType.Actor:
                    selectedSharedVariable.Actor =
                        InConnections.First(c => c.ConnectionTyp == ConnectionType.Actor).In.Actor;
                    break;
                case VariableType.ActorInfo:
                    selectedSharedVariable.ActorInfo =
                        InConnections.First(c => c.ConnectionTyp == ConnectionType.ActorInfo).In.ActorInfo;
                    break;
                case VariableType.Player:
                    selectedSharedVariable.Player =
                        InConnections.First(c => c.ConnectionTyp == ConnectionType.Player).In.Player;
                    break;
                case VariableType.PlayerGroup:
                    selectedSharedVariable.PlayerGroup =
                        InConnections.First(c => c.ConnectionTyp == ConnectionType.PlayerGroup).In.PlayerGroup;
                    break;
                case VariableType.Location:
                    selectedSharedVariable.Location =
                        InConnections.First(c => c.ConnectionTyp == ConnectionType.Location).In.Location;
                    break;
                case VariableType.CellArray:
                    selectedSharedVariable.CellArray =
                        InConnections.First(c => c.ConnectionTyp == ConnectionType.CellArray).In.CellArray;
                    break;
                case VariableType.CellPath:
                    selectedSharedVariable.CellArray =
                        InConnections.First(c => c.ConnectionTyp == ConnectionType.CellArray).In.CellArray;
                    break;
                case VariableType.Integer:
                    selectedSharedVariable.Number =
                        InConnections.First(c => c.ConnectionTyp == ConnectionType.Integer).In.Number;
                    break;
                case VariableType.ActorList:
                    selectedSharedVariable.ActorGroup =
                        InConnections.First(c => c.ConnectionTyp == ConnectionType.ActorList).In.ActorGroup;
                    break;
                case VariableType.Timer:
                    selectedSharedVariable.Timer =
                        (TriggerLogicCreateTimer) InConnections
                            .First(c => c.ConnectionTyp == ConnectionType.TimerConnection).In.Logic;
                    break;
                case VariableType.Objective:
                    selectedSharedVariable.Number =
                        InConnections.First(c => c.ConnectionTyp == ConnectionType.Objective).In.Number;
                    break;
            }

            foreach (var getterLogic in IngameNodeScriptSystem.NodeLogics.Where(v =>
                v is GetVariableLogic && (v as GetVariableLogic).SelectedSharedVariable == selectedSharedVariable))
                ((GetVariableLogic) getterLogic).Refresh();
        }
    }
}