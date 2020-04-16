using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Mime;
using OpenRA.Graphics;
using OpenRA.Mods.Common.Widgets.ScriptNodes.Library;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.Variables;
using OpenRA.Widgets;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes
{
    public class NodeEditorBackgroundWidget : Widget
    {
        // Background
        public readonly string Background = "panel-black";

        public ScriptNodeWidget Snw;

        ScrollPanelWidget scrollPanel;
        NodeEditorNodeScreenWidget screenWidget;

        NodeType nodeType;

        public DropDownMenuWidget DropDownMenuWidget;

        ButtonWidget addNodeButton;

        [ObjectCreator.UseCtor]
        public NodeEditorBackgroundWidget(ScriptNodeWidget snw, WorldRenderer worldRenderer, World world)
        {
            Snw = snw;

            Bounds = new Rectangle(5, 40, Game.Renderer.Resolution.Width - 265, Game.Renderer.Resolution.Height - 45);

            screenWidget = new NodeEditorNodeScreenWidget(Snw, this, worldRenderer, world)
            {
                Bounds = new Rectangle(Bounds.X + 5, Bounds.Y + 5,
                    Bounds.Width - 160,
                    Bounds.Height - 10),
                WidgetScreenCenterCoordinates = new int2((Bounds.Width / 2), (Bounds.Height / 2))
            };

            Children.Add(screenWidget);

            // Add Variable Buttons

            AddChild(scrollPanel = new ScrollPanelWidget(snw.ModData));
            scrollPanel.Layout = new ListLayout(scrollPanel);
            scrollPanel.Bounds = new Rectangle(Bounds.Width - 145, 40, 140, Bounds.Height - 60);

            var addButton = new ButtonWidget(snw.ModData)
            {
                Bounds = new Rectangle(Bounds.Width - 145, 5, 140, 25),
                Text = "Add Variable",
                OnClick = () => { AddNewVariable(VariableType.Actor); }
            };

            AddChild(addButton);

            // End

            var closeButton = new ButtonWidget(snw.ModData);
            AddChild(closeButton);
            closeButton.Bounds = new Rectangle(5, Bounds.Height - 5 - 25, 190, 25);
            closeButton.Text = "Close";
            closeButton.OnClick = () =>
            {
                List<NodeInfo> nodeInfos = new List<NodeInfo>();
                foreach (var node in screenWidget.Nodes)
                {
                    nodeInfos.Add(node.BuildNodeInfo());
                }

                Snw.World.WorldActor.Trait<EditorNodeLayer>().NodeInfos = nodeInfos;
                Snw.World.WorldActor.Trait<EditorNodeLayer>().VariableInfos = screenWidget.VariableInfos;
                Snw.Toggle();
            };

            CreateLeftClickDropDownMenu();
        }

        public void AddNewVariable(VariableType variableType, string variableName = "var")
        {
            var variableItem = new VariableInfo
            {
                VarType = variableType
            };

            var variableTemplate = new ScrollItemWidget(Snw.ModData)
            {
                Bounds = new Rectangle(0, 0, 110, 55)
            };

            var textFieldWidget = new TextFieldWidget
            {
                Bounds = new Rectangle(2, 2, 105, 25),
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
                    n is GetVariableNode && n.NodeType == NodeType.GetVariable &&
                    (n as GetVariableNode).VariableReference == variableItem))
                {
                    ((GetVariableNode) node).Update(false);
                }

                foreach (var node in screenWidget.Nodes.Where(n =>
                    n is SetVariableNode && n.NodeType == NodeType.SetVariable &&
                    (n as SetVariableNode).VariableReference == variableItem))
                {
                    ((SetVariableNode) node).Update(false);
                }
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

            List<VariableType> variableTypes = new List<VariableType>
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
                VariableType.Objective
            };

            List<string> variableString = new List<string>
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
                "Objective"
            };

            var dropDownText = new DropDownButtonWidget(Snw.ModData);
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
                        n is GetVariableNode && n.NodeType == NodeType.GetVariable &&
                        (n as GetVariableNode).VariableReference == variableItem))
                    {
                        ((GetVariableNode) node).Update(true);
                    }

                    foreach (var node in screenWidget.Nodes.Where(n =>
                        n is SetVariableNode && n.NodeType == NodeType.SetVariable &&
                        (n as SetVariableNode).VariableReference == variableItem))
                    {
                        ((SetVariableNode) node).Update(true);
                    }
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
                    n is GetVariableNode && n.NodeType == NodeType.GetVariable &&
                    (n as GetVariableNode).VariableReference == variableItem))
                {
                    dropDownText.Text = "SELECT VARIABLE";
                    ((GetVariableNode) node).VariableReference = null;
                }

                foreach (var node in screenWidget.Nodes.Where(n =>
                    n is SetVariableNode && n.NodeType == NodeType.SetVariable &&
                    (n as SetVariableNode).VariableReference == variableItem))
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
            WidgetUtils.DrawPanel(Background, RenderBounds);
        }

        void CreateLeftClickDropDownMenu()
        {
            DropDownMenuWidget = new DropDownMenuWidget()
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

            DropDownMenuWidget.AddDropDownMenu(GetSubMenue("Info Nodes", infoNodes));

            var variableNodes = new Dictionary<NodeType, string>
            {
                {NodeType.GetVariable, "Variable: Get"},
                {NodeType.SetVariable, "Variable: Set"}
            };

            DropDownMenuWidget.AddDropDownMenu(GetSubMenue("Variable Nodes", variableNodes));

            var actorNodes = new Dictionary<NodeType, string>
            {
                {NodeType.ActorGetInformations, "Informations of Actor"},
                {NodeType.ActorKill, "Kill"},
                {NodeType.ActorRemove, "Remove"},
                {NodeType.ActorChangeOwner, "Change Owner"},
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

            var actorSubMenu = GetSubMenue("Actor Activity", actorNodes);
            actorSubMenu.AddDropDownMenu(GetSubMenue("Queue Activities", actorQueueNodes));

            DropDownMenuWidget.AddDropDownMenu(actorSubMenu);

            var triggerNodes = new Dictionary<NodeType, string>
            {
                {NodeType.TriggerWorldLoaded, "World Loaded"},
                {NodeType.TriggerCreateTimer, "Create Timer"},
                {NodeType.TriggerTick, "On Tick"},
                {NodeType.TriggerOnEnteredFootprint, "On Entered Footprint"},
                {NodeType.TriggerOnEnteredRange, "On Entered Range"},
                {NodeType.TriggerOnIdle, "On Actor Idle"},
                {NodeType.TriggerOnKilled, "On Actor Killed"},
                {NodeType.TriggerOnAllKilled, "On All Actors Killed"},
            };

            DropDownMenuWidget.AddDropDownMenu(GetSubMenue("Trigger", triggerNodes));

            var timerNodes = new Dictionary<NodeType, string>
            {
                {NodeType.TimerStop, "Stop Timer"},
                {NodeType.TimerStart, "Start Timer"},
                {NodeType.TimerReset, "Reset Timer"}
            };

            DropDownMenuWidget.AddDropDownMenu(GetSubMenue("Timer", timerNodes));

            var groupNodes = new Dictionary<NodeType, string>
            {
                {NodeType.GroupPlayerGroup, "Player Group"},
                {NodeType.GroupActorInfoGroup, "Actor Info Group"},
                {NodeType.GroupActorGroup, "Actor Group"},
                {NodeType.FinActorsInCircle, "Find Actors in Circle"},
                {NodeType.FindActorsOnFootprint, "Find Actors on Footprint"},
                {NodeType.FilterActorGroup, "Filter Actors in Group"}
            };

            DropDownMenuWidget.AddDropDownMenu(GetSubMenue("Actor/Player Group", groupNodes));

            var arithmeticNodes = new Dictionary<NodeType, string>
            {
                {NodeType.ArithmeticsAnd, "And Trigger"},
                {NodeType.ArithmeticsOr, "Or Trigger"},
                {NodeType.ArithmeticsMath, "Math"},
                {NodeType.Count, "Get Count"},
                {NodeType.CompareActors, "Compare: Actors"}
            };

            DropDownMenuWidget.AddDropDownMenu(GetSubMenue("Arithmetic's", arithmeticNodes));

            var functionNodes = new Dictionary<NodeType, string>
            {
                {NodeType.ActorCreateActor, "Create Actor"},
                {NodeType.Reinforcements, "Reinforcements"},
                {NodeType.ReinforcementsWithTransport, "Reinforcements (Transport)"},
                {NodeType.CreateEffect, "Create Effect"},
                {NodeType.TimedExecution, "Timed Execution"},
                {NodeType.DoMultiple, "Repeating: Action"}
            };

            DropDownMenuWidget.AddDropDownMenu(GetSubMenue("Functions", functionNodes));

            var uiNodes = new Dictionary<NodeType, string>
            {
                {NodeType.UiPlayNotification, "Play Notification"},
                {NodeType.UiPlaySound, "Play Play Sound Location"},
                {NodeType.UiRadarPing, "Radar Ping"},
                {NodeType.UiTextMessage, "Chat Text message"},
                {NodeType.TextChoice, "Text Choice"},
                {NodeType.SetCameraPosition, "Set Camera Location"},
                {NodeType.CameraRide, "Camera Ride"},
                {NodeType.GlobalLightning, "Change Global Lightning"},
            };

            var uiObjectiveNodes = new Dictionary<NodeType, string>
            {
                {NodeType.UiNewObjective, "Add Objective"},
                {NodeType.UiCompleteObjective, "Complete Objective"},
                {NodeType.UiFailObjective, "Fail Objective"},
                {NodeType.UiAddMissionText, "Show Mission Text"},
            };

            var uiSubMenu =
                new DropDownMenuExpandButton(Snw.ModData, new Rectangle(0, 0, 160, 25))
                {
                    Text = "User Interface"
                };

            uiSubMenu.AddDropDownMenu(GetSubMenue("General UI", uiNodes));
            uiSubMenu.AddDropDownMenu(GetSubMenue("Objectives", uiObjectiveNodes));

            DropDownMenuWidget.AddDropDownMenu(uiSubMenu);

            var actorConditionNodes = new Dictionary<NodeType, string>
            {
                {NodeType.CompareActor, "Same actor"},
                {NodeType.CompareNumber, "Same number"},
                {NodeType.CompareActorInfo, "Same actortype"},
                {NodeType.IsDead, "Actor is dead"},
                {NodeType.IsAlive, "Actor is alive"},
            };

            var playerConditionNodes = new Dictionary<NodeType, string>
            {
                {NodeType.IsPlaying, "Player is Playing"},
                {NodeType.HasLost, "Player has Lost"},
                {NodeType.HasWon, "Player has Won"},
                {NodeType.IsHumanPlayer, "Player is Human"},
                {NodeType.IsBot, "Player is Bot"},
                {NodeType.IsNoncombatant, "Player is Noncombatant"},
            };

            var conditionsSubMenu =
                new DropDownMenuExpandButton(Snw.ModData, new Rectangle(0, 0, 160, 25))
                {
                    Text = "Conditions"
                };

            var checkConditionButton = new ButtonWidget(Snw.ModData)
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
            conditionsSubMenu.AddDropDownMenu(GetSubMenue("Actor Conditions", actorConditionNodes));
            conditionsSubMenu.AddDropDownMenu(GetSubMenue("Player Conditions", playerConditionNodes));

            DropDownMenuWidget.AddDropDownMenu(conditionsSubMenu);


            AddChild(DropDownMenuWidget);
        }

        DropDownMenuExpandButton GetSubMenue(string label, Dictionary<NodeType, string> types)
        {
            var actorNodes = new DropDownMenuExpandButton(Snw.ModData, new Rectangle(0, 0, 160, 25))
            {
                Text = label
            };

            foreach (var type in types)
            {
                var newWidget = new ButtonWidget(Snw.ModData)
                {
                    Bounds = new Rectangle(0, 0,
                        types.Max(t => Snw.FontRegular.Measure(t.Value).X) + 25, 25),
                    Text = type.Value,
                    Align = TextAlign.Left,
                    OnClick = () =>
                    {
                        screenWidget.AddNode(type.Key);
                        DropDownMenuWidget.Collapse(DropDownMenuWidget);
                        DropDownMenuWidget.Visible = false;
                    }
                };
                actorNodes.AddDropDownMenu(newWidget);
            }

            return actorNodes;
        }
    }
}