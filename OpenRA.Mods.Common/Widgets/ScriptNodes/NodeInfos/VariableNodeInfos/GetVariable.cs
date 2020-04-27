using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenRA.Mods.Common.Widgets.ScriptNodes.NodeInfos.TriggerNodeInfos;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.NodeInfos.VariableNodeInfos
{
    public class GetVariableInfo : NodeInfo
    {
        public new static Dictionary<string, BuildNodeConstructorInfo> NodeConstructorInformation =
            new Dictionary<string, BuildNodeConstructorInfo>()
            {
                {
                    "GetVariable", new BuildNodeConstructorInfo
                    {
                        Nesting = new[] {"Variable Nodes"},
                        Name = "Get Variable",

                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Actor, "")
                        }
                    }
                },
            };

        DropDownButtonWidget availableVariables;

        public GetVariableInfo(string nodeType, string nodeId, string nodeName) : base(nodeType, nodeId, nodeName)
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
                    var predefinedType =
                        widget.VariableReference == null || widget.VariableReference.VarType != option.VarType;
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
                    widget.OutConnections = new List<OutConnection>();
                    widget.AddOutConnection(new OutConnection(ConnectionType.Actor, widget)
                        {Actor = widget.VariableReference.Actor});
                    break;
                case VariableType.Integer:
                    widget.OutConnections = new List<OutConnection>();
                    widget.AddOutConnection(new OutConnection(ConnectionType.Integer, widget)
                        {Number = widget.VariableReference.Number});
                    break;
                case VariableType.Location:
                    widget.OutConnections = new List<OutConnection>();
                    widget.AddOutConnection(new OutConnection(ConnectionType.Location, widget)
                        {Location = widget.VariableReference.Location});
                    break;
                case VariableType.Player:
                    widget.OutConnections = new List<OutConnection>();
                    widget.AddOutConnection(new OutConnection(ConnectionType.Player, widget)
                        {Player = widget.VariableReference.Player});
                    break;
                case VariableType.ActorInfo:
                    widget.OutConnections = new List<OutConnection>();
                    widget.AddOutConnection(new OutConnection(ConnectionType.ActorInfo, widget)
                        {ActorInfo = widget.VariableReference.ActorInfo});
                    break;
                case VariableType.ActorList:
                    widget.OutConnections = new List<OutConnection>();
                    widget.AddOutConnection(new OutConnection(ConnectionType.ActorList, widget)
                        {ActorGroup = widget.VariableReference.ActorGroup});
                    break;
                case VariableType.PlayerGroup:
                    widget.OutConnections = new List<OutConnection>();
                    widget.AddOutConnection(new OutConnection(ConnectionType.PlayerGroup, widget)
                        {PlayerGroup = widget.VariableReference.PlayerGroup});
                    break;
                case VariableType.CellArray:
                    widget.OutConnections = new List<OutConnection>();
                    widget.AddOutConnection(new OutConnection(ConnectionType.CellArray, widget)
                        {CellArray = widget.VariableReference.CellArray});
                    break;
                case VariableType.CellPath:
                    widget.OutConnections = new List<OutConnection>();
                    widget.AddOutConnection(new OutConnection(ConnectionType.CellPath, widget)
                        {CellArray = widget.VariableReference.CellArray});
                    break;
                case VariableType.Timer:
                    widget.OutConnections = new List<OutConnection>();
                    widget.AddOutConnection(new OutConnection(ConnectionType.TimerConnection, widget));
                    break;
                case VariableType.Objective:
                    widget.OutConnections = new List<OutConnection>();
                    widget.AddOutConnection(new OutConnection(ConnectionType.Objective, widget));
                    break;
            }
        }

        public void Refresh(NodeLogic logic)
        {
            var selectedSharedVariable =
                logic.IngameNodeScriptSystem.VariableInfos.First(v => v.VariableName == VariableReference);
            switch (selectedSharedVariable.VarType)
            {
                case VariableType.Actor:
                    logic.OutConnections.First(c => c.ConnectionTyp == ConnectionType.Actor).Actor =
                        selectedSharedVariable.Actor;
                    break;
                case VariableType.ActorInfo:
                    logic.OutConnections.First(c => c.ConnectionTyp == ConnectionType.ActorInfo).ActorInfo =
                        selectedSharedVariable.ActorInfo;
                    break;
                case VariableType.Player:
                    logic.OutConnections.First(c => c.ConnectionTyp == ConnectionType.Player).Player =
                        selectedSharedVariable.Player;
                    break;
                case VariableType.PlayerGroup:
                    logic.OutConnections.First(c => c.ConnectionTyp == ConnectionType.PlayerGroup).PlayerGroup =
                        selectedSharedVariable.PlayerGroup;
                    break;
                case VariableType.Location:
                    logic.OutConnections.First(c => c.ConnectionTyp == ConnectionType.Location).Location =
                        selectedSharedVariable.Location;
                    break;
                case VariableType.CellArray:
                    logic.OutConnections.First(c => c.ConnectionTyp == ConnectionType.CellArray).CellArray =
                        selectedSharedVariable.CellArray;
                    break;
                case VariableType.CellPath:
                    logic.OutConnections.First(c => c.ConnectionTyp == ConnectionType.CellArray).CellArray =
                        selectedSharedVariable.CellArray;
                    break;
                case VariableType.Integer:
                    logic.OutConnections.First(c => c.ConnectionTyp == ConnectionType.Integer).Number =
                        selectedSharedVariable.Number;
                    break;
                case VariableType.ActorList:
                    logic.OutConnections.First(c => c.ConnectionTyp == ConnectionType.ActorList).ActorGroup =
                        selectedSharedVariable.ActorGroup;
                    break;
                case VariableType.Timer:
                    selectedSharedVariable.Timer =
                        (CreateTimerInfo) logic.OutConnections
                            .First(c => c.ConnectionTyp == ConnectionType.TimerConnection).Logic.NodeInfo;
                    break;
                case VariableType.Objective:
                    logic.OutConnections.First(c => c.ConnectionTyp == ConnectionType.TimerConnection).Number =
                        selectedSharedVariable.Number;
                    break;
            }
        }
    }
}