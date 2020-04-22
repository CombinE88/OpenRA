using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenRA.Graphics;
using OpenRA.Mods.Common.Traits;
using OpenRA.Mods.Common.Widgets.ScriptNodes.Library;
using OpenRA.Mods.Common.Widgets.ScriptNodes.OverlayWidgets;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.Variables;
using OpenRA.Widgets;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes
{
    public class NodeEditorBackgroundWidget : Widget
    {
        // Background
        readonly string background = "panel-black";

        readonly NodeScriptContainerWidget nodeScriptContainerWidget;
        readonly NodeEditorNodeScreenWidget screenWidget;
        readonly ScrollPanelWidget scrollPanel;
        public DropDownMenuWidget DropDownMenuWidget;
        public NodeEditorTooltipWidget ToolTip;

        [ObjectCreator.UseCtorAttribute]
        public NodeEditorBackgroundWidget(NodeScriptContainerWidget nodeScriptContainerWidget,
            WorldRenderer worldRenderer,
            World world)
        {
            this.nodeScriptContainerWidget = nodeScriptContainerWidget;

            Bounds = new Rectangle(5, 40, Game.Renderer.Resolution.Width - 265, Game.Renderer.Resolution.Height - 45);

            screenWidget = new NodeEditorNodeScreenWidget(this.nodeScriptContainerWidget, this, worldRenderer, world)
            {
                Bounds = new Rectangle(Bounds.X + 5, Bounds.Y + 5,
                    Bounds.Width - 160,
                    Bounds.Height - 10),
                WidgetScreenCenterCoordinates = new int2(Bounds.Width / 2, Bounds.Height / 2)
            };

            Children.Add(screenWidget);

            // Add Variable Buttons
            AddChild(scrollPanel = new ScrollPanelWidget(nodeScriptContainerWidget.ModData));
            scrollPanel.Layout = new ListLayout(scrollPanel);
            scrollPanel.Bounds = new Rectangle(Bounds.Width - 145, 40, 140, Bounds.Height - 60);

            var addButton = new ButtonWidget(nodeScriptContainerWidget.ModData)
            {
                Bounds = new Rectangle(Bounds.Width - 145, 5, 140, 25),
                Text = "Add Variable",
                OnClick = () => { AddNewVariable(VariableType.Actor); }
            };

            AddChild(addButton);

            // End

            var closeButton = new ButtonWidget(nodeScriptContainerWidget.ModData);
            AddChild(closeButton);
            closeButton.Bounds = new Rectangle(5, Bounds.Height - 5 - 25, 190, 25);
            closeButton.Text = "Close";
            closeButton.OnClick = () =>
            {
                screenWidget.YieldKeyboardFocus();
                var nodeInfos = screenWidget.Nodes.Select(node => node.BuildNodeInfo()).ToList();

                this.nodeScriptContainerWidget.World.WorldActor.Trait<EditorNodeLayer>().NodeInfos = nodeInfos;
                this.nodeScriptContainerWidget.World.WorldActor.Trait<EditorNodeLayer>().VariableInfos =
                    screenWidget.VariableInfos;
                this.nodeScriptContainerWidget.Toggle();
            };

            CreateLeftClickDropDownMenu();

            ToolTip = new NodeEditorTooltipWidget(nodeScriptContainerWidget.FontRegular);
            AddChild(ToolTip);
        }

        public void AddNewVariable(VariableType variableType, string variableName = "var")
        {
            var variableItem = new VariableInfo
            {
                VarType = variableType
            };

            var variableTemplate = new ScrollItemWidget(nodeScriptContainerWidget.ModData)
            {
                Bounds = new Rectangle(0, 0, 110, 55)
            };

            var textFieldWidget = new TextFieldWidget
            {
                Bounds = new Rectangle(2, 2, 105, 25)
            };

            textFieldWidget.OnTextEdited = () =>
            {
                if (textFieldWidget.Text == variableItem.VariableName)
                    return;

                var counter = 1;
                var text = textFieldWidget.Text;
                while (screenWidget.VariableInfos.Any(v =>
                    v.VariableName == textFieldWidget.Text && v != variableItem))
                {
                    text += counter;
                    textFieldWidget.Text = text;
                    counter++;
                }

                variableItem.VariableName = textFieldWidget.Text;

                foreach (var node in screenWidget.Nodes.Where(n =>
                {
                    var node = n as GetVariableNode;
                    return node != null && node.NodeType == "GetVariable" &&
                           node.VariableReference == variableItem;
                }))
                    ((GetVariableNode) node).Update(false);

                foreach (var node in screenWidget.Nodes.Where(n =>
                {
                    var node = n as SetVariableNode;
                    return node != null && node.NodeType == "SetVariable" &&
                           node.VariableReference == variableItem;
                }))
                    ((SetVariableNode) node).Update(false);
            };

            variableTemplate.AddChild(textFieldWidget);

            var i = 1;
            var name = variableName;
            variableItem.VariableName = name;
            textFieldWidget.Text = name;
            while (screenWidget.VariableInfos.Any(v => v.VariableName == name && v != variableItem))
            {
                name = variableName + i;
                variableItem.VariableName = name;
                textFieldWidget.Text = name;
                i++;
            }

            var variableTypes = new List<VariableType>
            {
                VariableType.Actor,
                VariableType.ActorInfo,
                VariableType.Player,
                VariableType.PlayerGroup,
                VariableType.Location,
                VariableType.CellArray,
                VariableType.CellPath,
                VariableType.Integer,
                VariableType.ActorList,
                VariableType.Timer,
                VariableType.Objective,
                VariableType.LocationRange
            };

            var variableString = new List<string>
            {
                "Actor",
                "Actor Info",
                "Player",
                "Player Group",
                "Cell",
                "Cells",
                "Cell Path",
                "Integer",
                "Actor List",
                "Timer",
                "Objective",
                "Location + Range"
            };

            var dropDownText = new DropDownButtonWidget(nodeScriptContainerWidget.ModData);
            dropDownText.Text = variableString[variableTypes.IndexOf(variableType)];

            var type = variableType;

            Func<VariableType, ScrollItemWidget, ScrollItemWidget> setupItemGroup = (option, template) =>
            {
                var item = ScrollItemWidget.Setup(template, () => type == option, () =>
                {
                    type = option;
                    dropDownText.Text = variableString[variableTypes.IndexOf(type)];

                    var oldType = variableItem.VarType;
                    variableItem.VarType = option;

                    if (variableItem.VarType == oldType)
                        return;

                    foreach (var node in screenWidget.Nodes.Where(n =>
                    {
                        var node = n as GetVariableNode;
                        return node != null && node.NodeType == "GetVariable" &&
                               node.VariableReference == variableItem;
                    }))
                        ((GetVariableNode) node).Update(true);

                    foreach (var node in screenWidget.Nodes.Where(n =>
                    {
                        var node = n as SetVariableNode;
                        return node != null && node.NodeType == "SetVariable" &&
                               node.VariableReference == variableItem;
                    }))
                        ((SetVariableNode) node).Update(true);
                });

                item.Get<LabelWidget>("LABEL").GetText = () => variableString[variableTypes.IndexOf(option)];

                return item;
            };

            dropDownText.OnClick = () =>
            {
                var nodes = variableTypes;
                dropDownText.ShowDropDown("LABEL_DROPDOWN_TEMPLATE", 270, nodes, setupItemGroup);
            };

            variableTemplate.OnDoubleClick = () =>
            {
                foreach (var node in screenWidget.Nodes.Where(n =>
                {
                    var node = n as GetVariableNode;
                    return node != null && node.NodeType == "GetVariabl" &&
                           node.VariableReference == variableItem;
                }))
                {
                    dropDownText.Text = "SELECT VARIABLE";
                    ((GetVariableNode) node).VariableReference = null;
                }

                foreach (var node in screenWidget.Nodes.Where(n =>
                {
                    var node = n as SetVariableNode;
                    return node != null && node.NodeType == "SetVariable" &&
                           node.VariableReference == variableItem;
                }))
                {
                    dropDownText.Text = "SELECT VARIABLE";
                    ((SetVariableNode) node).VariableReference = null;
                }

                scrollPanel.RemoveChild(variableTemplate);
                screenWidget.RemoveVariableInfo(variableItem);
            };

            screenWidget.AddVariableInfo(variableItem);
            variableTemplate.AddChild(dropDownText);
            dropDownText.Bounds = new Rectangle(2, 29, 105, 25);

            variableTemplate.IsVisible = () => true;
            scrollPanel.AddChild(variableTemplate);
        }

        public override void Draw()
        {
            WidgetUtils.DrawPanel(background, RenderBounds);
        }

        //// TODO Move to static class and make Static
        void CreateLeftClickDropDownMenu()
        {
            DropDownMenuWidget = NodeLibrary.BuildWidgetMenu(nodeScriptContainerWidget.ModData,
                nodeScriptContainerWidget, screenWidget);
            /*
            DropDownMenuWidget = new DropDownMenuWidget
            {
                Bounds = new Rectangle(0, 0, 180, 75),
                Visible = false
            };

            var infoNodes = new Dictionary<string, string>
            {
                     {"MapInfoNode", "Global Info"},
                     {"MapInfoActorInfo", "Actor Type Info"},
                     {"MapInfoActorReference", "Actor on Map"}
            };

            DropDownMenuWidget.AddDropDownMenu(GetSubMenu("Info Nodes", infoNodes));

            var variableNodes = new Dictionary<string, string>
            {
                     {"GetVariable", "Variable: Get"},
                     {"SetVariable", "Variable: Set"}
            };

            DropDownMenuWidget.AddDropDownMenu(GetSubMenu("Variable Nodes", variableNodes));

            var actorNodes = new Dictionary<string, string>
            {
                     {"ActorGetInformations", "Informations of Actor"},
                     {"ActorKill", "Kill"},
                     {"ActorRemove", "Remove"},
                     {"ActorChangeOwner", "Change Owner"}
            };
            var actorQueueNodes = new Dictionary<string, string>
            {
                     {"ActorQueueMove", "Queue Move"},
                     {"ActorQueueAttack", "Queue Attack"},
                     {"ActorQueueHunt", "Queue Hunt"},
                     {"ActorQueueAttackMoveActivity", "Queue AttackMoveActivity"},
                     {"ActorQueueSell", "Queue Sell"},
                     {"ActorQueueFindResources", "QueueFindResources"}
            };

            var actorSubMenu = GetSubMenu("Actor Activity", actorNodes);
            actorSubMenu.AddDropDownMenu(GetSubMenu("Queue Activities", actorQueueNodes));

            DropDownMenuWidget.AddDropDownMenu(actorSubMenu);

            var triggerNodes = new Dictionary<string, string>
            {
                     {"TriggerWorldLoaded", "World Loaded"},
                     {"TriggerTick", "On Tick"},
                     {"TriggerOnEnteredFootprint", "On Entered Footprint"},
                     {"TriggerOnEnteredRange", "On Entered Range"},
                     {"TriggerOnIdle", "On Actor Idle"},
                     {"TriggerOnKilled", "On Actor Killed"},
                     {"TriggerOnAllKilled", "On All Actors Killed"}
            };

            DropDownMenuWidget.AddDropDownMenu(GetSubMenu("Trigger", triggerNodes));

            var timerNodes = new Dictionary<string, string>
            {
                     {"TriggerCreateTimer", "Create Timer"},
                     {"TimerStop", "Stop Timer"},
                     {"TimerStart", "Start Timer"},
                     {"TimerReset", "Reset Timer"}
            };

            DropDownMenuWidget.AddDropDownMenu(GetSubMenu("Timer", timerNodes));

            var groupNodes = new Dictionary<string, string>
            {
                     {"GroupPlayerGroup", "Player Group"},
                     {"GroupActorInfoGroup", "Actor Info Group"},
                     {"GroupActorGroup", "Actor Group"},
                     {"FinActorsInCircle", "Find Actors in Circle"},
                     {"FindActorsOnFootprint", "Find Actors on Footprint"},
                     {"FilterActorGroup", "Filter Actors in Group"}
            };

            DropDownMenuWidget.AddDropDownMenu(GetSubMenu("Actor/Player Group", groupNodes));

            var arithmeticNodes = new Dictionary<string, string>
            {
                     {"ArithmeticsAnd", "And Trigger"},
                     {"ArithmeticsOr", "Or Trigger"},
                     {"ArithmeticsMath", "Math"},
                     {"Count", "Get Count"},
                     {"CompareActors", "Compare: Actors"}
            };

            DropDownMenuWidget.AddDropDownMenu(GetSubMenu("Arithmetic's", arithmeticNodes));

            var functionNodes = new Dictionary<string, string>
            {
                     {"ActorCreateActor", "Create Actor"},
                     {"Reinforcements", "Reinforcements"},
                     {"ReinforcementsWithTransport", "Reinforcements (Transport)"},
                     {"CreateEffect", "Create Effect"},
                     {"TimedExecution", "Timed Execution"},
                     {"DoMultiple", "Repeating: Action"}
            };

            DropDownMenuWidget.AddDropDownMenu(GetSubMenu("Functions", functionNodes));

            var uiNodes = new Dictionary<string, string>
            {
                     {"UiPlayNotification", "Play Notification"},
                     {"UiPlaySound", "Play Play Sound Location"},
                     {"UiRadarPing", "Radar Ping"},
                     {"UiTextMessage", "Chat Text message"},
                     {"TextChoice", "Text Choice"},
                     {"SetCameraPosition", "Set Camera Location"},
                     {"CameraRide", "Camera Ride"},
                     {"GlobalLightning", "Change Global Lightning"}
            };

            var uiObjectiveNodes = new Dictionary<string, string>
            {
                     {"UiNewObjective", "Add Objective"},
                     {"UiCompleteObjective", "Complete Objective"},
                     {"UiFailObjective", "Fail Objective"},
                     {"UiAddMissionText", "Show Mission Text"}
            };

            var uiSubMenu =
                new DropDownMenuExpandButton(nodeScriptContainerWidget.ModData, new Rectangle(0, 0, 160, 25))
                {
                    Text = "User Interface"
                };

            uiSubMenu.AddDropDownMenu(GetSubMenu("General UI", uiNodes));
            uiSubMenu.AddDropDownMenu(GetSubMenu("Objectives", uiObjectiveNodes));

            DropDownMenuWidget.AddDropDownMenu(uiSubMenu);

            var actorConditionNodes = new Dictionary<string, string>
            {
                     {"CompareActor", "Same actor"},
                     {"CompareNumber", "Same number"},
                     {"CompareActorInfo", "Same Actor Type"},
                     {"IsDead", "Actor is dead"},
                     {"IsAlive", "Actor is alive"}
            };

            var playerConditionNodes = new Dictionary<string, string>
            {
                     {"IsPlaying", "Player is Playing"},
                     {"HasLost", "Player has Lost"},
                     {"HasWon", "Player has Won"},
                     {"IsHumanPlayer", "Player is Human"},
                     {"IsBot", "Player is Bot"},
                     {"IsNoncombatant", "Player is Noncombatant"}
            };

            var conditionsSubMenu =
                new DropDownMenuExpandButton(nodeScriptContainerWidget.ModData, new Rectangle(0, 0, 160, 25))
                {
                    Text = "Conditions"
                };

            var checkConditionButton = new ButtonWidget(nodeScriptContainerWidget.ModData)
            {
                Bounds = new Rectangle(0, 0, 130, 25),
                Text = "Check Condition",
                OnClick = () =>
                {
                    screenWidget.AddNode("CheckCondition");
                    DropDownMenuWidget.Collapse(DropDownMenuWidget);
                    DropDownMenuWidget.Visible = false;
                }
            };

            conditionsSubMenu.AddDropDownMenu(checkConditionButton);
            conditionsSubMenu.AddDropDownMenu(GetSubMenu("ctor ConditionsA", actorConditionNodes));
            conditionsSubMenu.AddDropDownMenu(GetSubMenu("Player Conditions", playerConditionNodes));

            DropDownMenuWidget.AddDropDownMenu(conditionsSubMenu);

            */

            AddChild(DropDownMenuWidget);
        }

        DropDownMenuExpandButton GetSubMenu(string label, Dictionary<string, string> types)
        {
            var actorNodes =
                new DropDownMenuExpandButton(nodeScriptContainerWidget.ModData, new Rectangle(0, 0, 160, 25))
                {
                    Text = label
                };

            foreach (var newWidget in types.Select(type => new ButtonWidget(nodeScriptContainerWidget.ModData)
            {
                Bounds = new Rectangle(0, 0,
                    types.Max(t => nodeScriptContainerWidget.FontRegular.Measure(t.Value).X) + 25, 25),
                Text = type.Value,
                Align = TextAlign.Left,
                OnClick = () =>
                {
                    screenWidget.AddNode(type.Key);
                    DropDownMenuWidget.Collapse(DropDownMenuWidget);
                    DropDownMenuWidget.Visible = false;
                }
            }))
                actorNodes.AddDropDownMenu(newWidget);

            return actorNodes;
        }
    }
}