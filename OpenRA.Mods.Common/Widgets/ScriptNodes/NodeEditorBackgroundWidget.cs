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
                    return node != null && node.NodeType == NodeType.GetVariable &&
                           node.VariableReference == variableItem;
                }))
                    ((GetVariableNode) node).Update(false);

                foreach (var node in screenWidget.Nodes.Where(n =>
                {
                    var node = n as SetVariableNode;
                    return node != null && node.NodeType == NodeType.SetVariable &&
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
                        return node != null && node.NodeType == NodeType.GetVariable &&
                               node.VariableReference == variableItem;
                    }))
                        ((GetVariableNode) node).Update(true);

                    foreach (var node in screenWidget.Nodes.Where(n =>
                    {
                        var node = n as SetVariableNode;
                        return node != null && node.NodeType == NodeType.SetVariable &&
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
                    return node != null && node.NodeType == NodeType.GetVariable &&
                           node.VariableReference == variableItem;
                }))
                {
                    dropDownText.Text = "SELECT VARIABLE";
                    ((GetVariableNode) node).VariableReference = null;
                }

                foreach (var node in screenWidget.Nodes.Where(n =>
                {
                    var node = n as SetVariableNode;
                    return node != null && node.NodeType == NodeType.SetVariable &&
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
            DropDownMenuWidget = new DropDownMenuWidget
            {
                Bounds = new Rectangle(0, 0, 180, 75),
                Visible = false
            };

            var infoNodes = new Dictionary<NodeType, string>
            {
                {NodeType.MapInfoNode, "Global Info"},
                {NodeType.MapInfoActorInfo, "Actor Type Info"},
                {NodeType.MapInfoActorReference, "Actor on Map"}
            };

            DropDownMenuWidget.AddDropDownMenu(GetSubMenu("Info Nodes", infoNodes));

            var variableNodes = new Dictionary<NodeType, string>
            {
                {NodeType.GetVariable, "Variable: Get"},
                {NodeType.SetVariable, "Variable: Set"}
            };

            DropDownMenuWidget.AddDropDownMenu(GetSubMenu("Variable Nodes", variableNodes));

            var actorNodes = new Dictionary<NodeType, string>
            {
                {NodeType.ActorGetInformations, "Informations of Actor"},
                {NodeType.ActorKill, "Kill"},
                {NodeType.ActorRemove, "Remove"},
                {NodeType.ActorChangeOwner, "Change Owner"}
            };
            var actorQueueNodes = new Dictionary<NodeType, string>
            {
                {NodeType.ActorQueueMove, "Queue Move"},
                {NodeType.ActorQueueAttack, "Queue Attack"},
                {NodeType.ActorQueueHunt, "Queue Hunt"},
                {NodeType.ActorQueueAttackMoveActivity, "Queue AttackMoveActivity"},
                {NodeType.ActorQueueSell, "Queue Sell"},
                {NodeType.ActorQueueFindResources, "QueueFindResources"}
            };

            var actorSubMenu = GetSubMenu("Actor Activity", actorNodes);
            actorSubMenu.AddDropDownMenu(GetSubMenu("Queue Activities", actorQueueNodes));

            DropDownMenuWidget.AddDropDownMenu(actorSubMenu);

            var triggerNodes = new Dictionary<NodeType, string>
            {
                {NodeType.TriggerWorldLoaded, "World Loaded"},
                {NodeType.TriggerTick, "On Tick"},
                {NodeType.TriggerOnEnteredFootprint, "On Entered Footprint"},
                {NodeType.TriggerOnEnteredRange, "On Entered Range"},
                {NodeType.TriggerOnIdle, "On Actor Idle"},
                {NodeType.TriggerOnKilled, "On Actor Killed"},
                {NodeType.TriggerOnAllKilled, "On All Actors Killed"}
            };

            DropDownMenuWidget.AddDropDownMenu(GetSubMenu("Trigger", triggerNodes));

            var timerNodes = new Dictionary<NodeType, string>
            {
                {NodeType.TriggerCreateTimer, "Create Timer"},
                {NodeType.TimerStop, "Stop Timer"},
                {NodeType.TimerStart, "Start Timer"},
                {NodeType.TimerReset, "Reset Timer"}
            };

            DropDownMenuWidget.AddDropDownMenu(GetSubMenu("Timer", timerNodes));

            var groupNodes = new Dictionary<NodeType, string>
            {
                {NodeType.GroupPlayerGroup, "Player Group"},
                {NodeType.GroupActorInfoGroup, "Actor Info Group"},
                {NodeType.GroupActorGroup, "Actor Group"},
                {NodeType.FinActorsInCircle, "Find Actors in Circle"},
                {NodeType.FindActorsOnFootprint, "Find Actors on Footprint"},
                {NodeType.FilterActorGroup, "Filter Actors in Group"}
            };

            DropDownMenuWidget.AddDropDownMenu(GetSubMenu("Actor/Player Group", groupNodes));

            var arithmeticNodes = new Dictionary<NodeType, string>
            {
                {NodeType.ArithmeticsAnd, "And Trigger"},
                {NodeType.ArithmeticsOr, "Or Trigger"},
                {NodeType.ArithmeticsMath, "Math"},
                {NodeType.Count, "Get Count"},
                {NodeType.CompareActors, "Compare: Actors"}
            };

            DropDownMenuWidget.AddDropDownMenu(GetSubMenu("Arithmetic's", arithmeticNodes));

            var functionNodes = new Dictionary<NodeType, string>
            {
                {NodeType.ActorCreateActor, "Create Actor"},
                {NodeType.Reinforcements, "Reinforcements"},
                {NodeType.ReinforcementsWithTransport, "Reinforcements (Transport)"},
                {NodeType.CreateEffect, "Create Effect"},
                {NodeType.TimedExecution, "Timed Execution"},
                {NodeType.DoMultiple, "Repeating: Action"}
            };

            DropDownMenuWidget.AddDropDownMenu(GetSubMenu("Functions", functionNodes));

            var uiNodes = new Dictionary<NodeType, string>
            {
                {NodeType.UiPlayNotification, "Play Notification"},
                {NodeType.UiPlaySound, "Play Play Sound Location"},
                {NodeType.UiRadarPing, "Radar Ping"},
                {NodeType.UiTextMessage, "Chat Text message"},
                {NodeType.TextChoice, "Text Choice"},
                {NodeType.SetCameraPosition, "Set Camera Location"},
                {NodeType.CameraRide, "Camera Ride"},
                {NodeType.GlobalLightning, "Change Global Lightning"}
            };

            var uiObjectiveNodes = new Dictionary<NodeType, string>
            {
                {NodeType.UiNewObjective, "Add Objective"},
                {NodeType.UiCompleteObjective, "Complete Objective"},
                {NodeType.UiFailObjective, "Fail Objective"},
                {NodeType.UiAddMissionText, "Show Mission Text"}
            };

            var uiSubMenu =
                new DropDownMenuExpandButton(nodeScriptContainerWidget.ModData, new Rectangle(0, 0, 160, 25))
                {
                    Text = "User Interface"
                };

            uiSubMenu.AddDropDownMenu(GetSubMenu("General UI", uiNodes));
            uiSubMenu.AddDropDownMenu(GetSubMenu("Objectives", uiObjectiveNodes));

            DropDownMenuWidget.AddDropDownMenu(uiSubMenu);

            var actorConditionNodes = new Dictionary<NodeType, string>
            {
                {NodeType.CompareActor, "Same actor"},
                {NodeType.CompareNumber, "Same number"},
                {NodeType.CompareActorInfo, "Same Actor Type"},
                {NodeType.IsDead, "Actor is dead"},
                {NodeType.IsAlive, "Actor is alive"}
            };

            var playerConditionNodes = new Dictionary<NodeType, string>
            {
                {NodeType.IsPlaying, "Player is Playing"},
                {NodeType.HasLost, "Player has Lost"},
                {NodeType.HasWon, "Player has Won"},
                {NodeType.IsHumanPlayer, "Player is Human"},
                {NodeType.IsBot, "Player is Bot"},
                {NodeType.IsNoncombatant, "Player is Noncombatant"}
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
                    screenWidget.AddNode(NodeType.CheckCondition);
                    DropDownMenuWidget.Collapse(DropDownMenuWidget);
                    DropDownMenuWidget.Visible = false;
                }
            };

            conditionsSubMenu.AddDropDownMenu(checkConditionButton);
            conditionsSubMenu.AddDropDownMenu(GetSubMenu("Actor Conditions", actorConditionNodes));
            conditionsSubMenu.AddDropDownMenu(GetSubMenu("Player Conditions", playerConditionNodes));

            DropDownMenuWidget.AddDropDownMenu(conditionsSubMenu);


            AddChild(DropDownMenuWidget);
        }

        DropDownMenuExpandButton GetSubMenu(string label, Dictionary<NodeType, string> types)
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