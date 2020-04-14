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
        public VatiableInfo SelectedVariable = new VatiableInfo {VarType = VariableType.Actor};

        public SetVariableNode(NodeEditorNodeScreenWidget screen, NodeInfo nodeInfo) : base(screen, nodeInfo)
        {
            AddChild(availableVariables = new DropDownButtonWidget(Screen.Snw.ModData));
            availableVariables.Text = "CHOOSE VARIABLE";
            Func<VatiableInfo, ScrollItemWidget, ScrollItemWidget> setupItem = (option, template) =>
            {
                var item = ScrollItemWidget.Setup(template, () => SelectedVariable == option, () =>
                {
                    var predefinedType = SelectedVariable.VarType;
                    SelectedVariable = option;
                    Update(predefinedType != SelectedVariable.VarType);
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
        VatiableInfo selectedVariable;

        public SetVariableLogic(NodeInfo nodeinfo, IngameNodeScriptSystem insc) : base(nodeinfo, insc)
        {
        }

        public override void Execute(World world)
        {
            selectedVariable.ActorInfo = InConnections.First(c => c.ConTyp == ConnectionType.ActorInfo).In.ActorInfo;
            selectedVariable.Location = InConnections.First(c => c.ConTyp == ConnectionType.Location).In.Location;
            selectedVariable.Player = InConnections.First(c => c.ConTyp == ConnectionType.Player).In.Player;
            selectedVariable.PlayerGroup =
                InConnections.First(c => c.ConTyp == ConnectionType.PlayerGroup).In.PlayerGroup;
            selectedVariable.CellArray = InConnections.First(c => c.ConTyp == ConnectionType.CellArray).In.CellArray;
            selectedVariable.Number = InConnections.First(c => c.ConTyp == ConnectionType.Integer).In.Number;
            selectedVariable.ActorGroup = InConnections.First(c => c.ConTyp == ConnectionType.ActorList).In.ActorGroup;
            selectedVariable.Number = InConnections.First(c => c.ConTyp == ConnectionType.Objective).In.Number;
            selectedVariable.Timer =
                (TriggerLogicCreateTimer) InConnections.First(c => c.ConTyp == ConnectionType.TimerConnection).In.Logic;
        }
    }
}