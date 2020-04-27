using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenRA.Mods.Common.Widgets.ScriptNodes.NodeInfos.TriggerNodeInfos;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.NodeInfos.VariableNodeInfos
{
    public class SetVariableInfo : NodeInfo
    {
        public new static Dictionary<string, BuildNodeConstructorInfo> NodeConstructorInformation =
            new Dictionary<string, BuildNodeConstructorInfo>()
            {
                {
                    "SetVariable", new BuildNodeConstructorInfo
                    {
                        Nesting = new[] {"Variable Nodes"},
                        Name = "Set Variable",

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Actor, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        }
                    }
                },
            };

        DropDownButtonWidget availableVariables;

        public SetVariableInfo(string nodeType, string nodeId, string nodeName) : base(nodeType, nodeId, nodeName)
        {
        }

        public override void WidgetInitialize(NodeWidget widget)
        {
            widget.AddChild(availableVariables =
                new DropDownButtonWidget(widget.Screen.NodeScriptContainerWidget.ModData));
            Func<VariableInfo, ScrollItemWidget, ScrollItemWidget> setupItem = (option, template) =>
            {
                var item = ScrollItemWidget.Setup(template, () => widget.VariableReference == option, () =>
                {
                    var predefinedType = widget.VariableReference == null ||
                                         widget.VariableReference.VarType != option.VarType;
                    widget.VariableReference = option;
                    Update(predefinedType, widget);
                });

                item.Get<LabelWidget>("LABEL").GetText = () => option.VariableName;
                return item;
            };

            availableVariables.OnClick = () =>
            {
                var variables = widget.Screen.VariableInfos;
                availableVariables.ShowDropDown("LABEL_DROPDOWN_TEMPLATE", 270, variables, setupItem);
            };
            availableVariables.Bounds =
                new Rectangle(widget.FreeWidgetEntries.X, widget.FreeWidgetEntries.Y + 75,
                    widget.FreeWidgetEntries.Width, 25);

            if (VariableReference == null)
                return;

            var foundVariable = widget.Screen.VariableInfos.FirstOrDefault(v => v.VariableName == VariableReference);

            if (foundVariable == null)
                return;

            availableVariables.Text = foundVariable.VariableName;
            widget.VariableReference = foundVariable;
            Update(true, widget);
        }

        public void Update(bool changedInputsOutputs, NodeWidget widget)
        {
            availableVariables.Text = widget.VariableReference.VariableName;

            if (!changedInputsOutputs)
                return;

            switch (widget.VariableReference.VarType)
            {
                case VariableType.Actor:
                    widget.InConnections = new List<InConnection>();
                    widget.AddInConnection(new InConnection(ConnectionType.Actor, widget));
                    break;
                case VariableType.Integer:
                    widget.InConnections = new List<InConnection>();
                    widget.AddInConnection(new InConnection(ConnectionType.Integer, widget));
                    break;
                case VariableType.Location:
                    widget.InConnections = new List<InConnection>();
                    widget.AddInConnection(new InConnection(ConnectionType.Location, widget));
                    break;
                case VariableType.Player:
                    widget.InConnections = new List<InConnection>();
                    widget.AddInConnection(new InConnection(ConnectionType.Player, widget));
                    break;
                case VariableType.ActorInfo:
                    widget.InConnections = new List<InConnection>();
                    widget.AddInConnection(new InConnection(ConnectionType.ActorInfo, widget));
                    break;
                case VariableType.ActorList:
                    widget.InConnections = new List<InConnection>();
                    widget.AddInConnection(new InConnection(ConnectionType.ActorList, widget));
                    break;
                case VariableType.PlayerGroup:
                    widget.InConnections = new List<InConnection>();
                    widget.AddInConnection(new InConnection(ConnectionType.PlayerGroup, widget));
                    break;
                case VariableType.CellArray:
                    widget.InConnections = new List<InConnection>();
                    widget.AddInConnection(new InConnection(ConnectionType.CellArray, widget));
                    break;
                case VariableType.CellPath:
                    widget.InConnections = new List<InConnection>();
                    widget.AddInConnection(new InConnection(ConnectionType.CellPath, widget));
                    break;
                case VariableType.Timer:
                    widget.InConnections = new List<InConnection>();
                    widget.AddInConnection(new InConnection(ConnectionType.TimerConnection, widget));
                    break;
                case VariableType.Objective:
                    widget.InConnections = new List<InConnection>();
                    widget.AddInConnection(new InConnection(ConnectionType.Objective, widget));
                    break;
            }

            widget.AddInConnection(new InConnection(ConnectionType.Exec, widget));
        }


        public override void LogicExecute(World world, NodeLogic logic)
        {
            var selectedSharedVariable =
                logic.IngameNodeScriptSystem.VariableInfos.First(v => v.VariableName == VariableReference);

            switch (selectedSharedVariable.VarType)
            {
                case VariableType.Actor:
                    selectedSharedVariable.Actor =
                        logic.InConnections.First(c => c.ConnectionTyp == ConnectionType.Actor).In.Actor;
                    break;
                case VariableType.ActorInfo:
                    selectedSharedVariable.ActorInfo =
                        logic.InConnections.First(c => c.ConnectionTyp == ConnectionType.ActorInfo).In.ActorInfo;
                    break;
                case VariableType.Player:
                    selectedSharedVariable.Player =
                        logic.InConnections.First(c => c.ConnectionTyp == ConnectionType.Player).In.Player;
                    break;
                case VariableType.PlayerGroup:
                    selectedSharedVariable.PlayerGroup =
                        logic.InConnections.First(c => c.ConnectionTyp == ConnectionType.PlayerGroup).In.PlayerGroup;
                    break;
                case VariableType.Location:
                    selectedSharedVariable.Location =
                        logic.InConnections.First(c => c.ConnectionTyp == ConnectionType.Location).In.Location;
                    break;
                case VariableType.CellArray:
                    selectedSharedVariable.CellArray =
                        logic.InConnections.First(c => c.ConnectionTyp == ConnectionType.CellArray).In.CellArray;
                    break;
                case VariableType.CellPath:
                    selectedSharedVariable.CellArray =
                        logic.InConnections.First(c => c.ConnectionTyp == ConnectionType.CellArray).In.CellArray;
                    break;
                case VariableType.Integer:
                    selectedSharedVariable.Number =
                        logic.InConnections.First(c => c.ConnectionTyp == ConnectionType.Integer).In.Number;
                    break;
                case VariableType.ActorList:
                    selectedSharedVariable.ActorGroup =
                        logic.InConnections.First(c => c.ConnectionTyp == ConnectionType.ActorList).In.ActorGroup;
                    break;
                case VariableType.Timer:
                    selectedSharedVariable.Timer =
                        (CreateTimerInfo) logic.InConnections
                            .First(c => c.ConnectionTyp == ConnectionType.TimerConnection).In.Logic.NodeInfo;
                    break;
                case VariableType.Objective:
                    selectedSharedVariable.Number =
                        logic.InConnections.First(c => c.ConnectionTyp == ConnectionType.Objective).In.Number;
                    break;
            }

            foreach (var getterLogic in logic.IngameNodeScriptSystem.NodeLogics.Where(v =>
                v.NodeInfo is GetVariableInfo &&
                v.NodeInfo.VariableReference ==
                selectedSharedVariable
                    .VariableName))
                ((GetVariableInfo) getterLogic.NodeInfo).Refresh(getterLogic);

            NodeLogic.ForwardExec(logic);
        }
    }
}