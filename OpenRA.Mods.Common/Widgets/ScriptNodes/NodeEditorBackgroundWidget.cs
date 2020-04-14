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
        DropDownButtonWidget createNodesList;
        DropDownButtonWidget createActorNodesList;
        DropDownButtonWidget triggerNodesList;
        DropDownButtonWidget groupNodesList;
        DropDownButtonWidget arithmeticNodesList;
        DropDownButtonWidget functionNodesList;
        DropDownButtonWidget uiNodesList;
        DropDownButtonWidget conditionNodesList;

        NodeType nodeType;

        ButtonWidget addNodeButton;

        [ObjectCreator.UseCtor]
        public NodeEditorBackgroundWidget(ScriptNodeWidget snw, WorldRenderer worldRenderer, World world)
        {
            Snw = snw;

            Children.Add(screenWidget = new NodeEditorNodeScreenWidget(Snw, this, worldRenderer, world));

            Bounds = new Rectangle(5, 40, Game.Renderer.Resolution.Width - 265, Game.Renderer.Resolution.Height - 45);

            AddNodesList();
            AddActorList();
            AddTriggerList();
            AddGroupList();
            AddArithmeticList();
            AddFunctionsList();
            AddUiList();
            AddConditionList();

            createActorNodesList.Text = "- Actor Nodes -";
            triggerNodesList.Text = "- Trigger Nodes -";
            createNodesList.Text = "- Info Nodes -";
            groupNodesList.Text = "- Group Nodes -";
            arithmeticNodesList.Text = "- Arithmetic Nodes -";
            functionNodesList.Text = "- Function Nodes -";
            uiNodesList.Text = "- Ui Nodes -";
            conditionNodesList.Text = "- Condition Nodes -";

            // Add Variable Buttons


            AddChild(scrollPanel = new ScrollPanelWidget(snw.ModData));
            scrollPanel.Layout = new ListLayout(scrollPanel);
            scrollPanel.Bounds = new Rectangle(Bounds.Width - 145, 40, 140, Bounds.Height - 60);

            var addButton = new ButtonWidget(snw.ModData)
            {
                Bounds = new Rectangle(Bounds.Width - 145, 5, 140, 25),
                Text = "Add Variable",
                OnClick = () =>
                {
                    var variableItem = new VatiableInfo
                    {
                        VarType = VariableType.Actor
                    };

                    var variableTemplate = new ScrollItemWidget(snw.ModData)
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
                            counter++;
                            text += counter;
                            textFieldWidget.Text = text;
                        }

                        variableItem.VariableName = textFieldWidget.Text;

                        foreach (var node in screenWidget.Nodes.Where(n =>
                            n is GetVariableNode && n.NodeType == NodeType.GetVariable &&
                            (n as GetVariableNode).SelectedVariable == variableItem))
                        {
                            ((GetVariableNode) node).Update(false);
                        }

                        foreach (var node in screenWidget.Nodes.Where(n =>
                            n is SetVariableNode && n.NodeType == NodeType.SetVariable &&
                            (n as SetVariableNode).SelectedVariable == variableItem))
                        {
                            ((SetVariableNode) node).Update(false);
                        }
                    };

                    variableTemplate.AddChild(textFieldWidget);

                    var i = 1;
                    var name = "var" + i;
                    variableItem.VariableName = name;
                    textFieldWidget.Text = name;
                    while (screenWidget.VariableInfos.Any(v => v.VariableName == name && v != variableItem))
                    {
                        i++;
                        name = "var" + i;
                        variableItem.VariableName = name;
                        textFieldWidget.Text = name;
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
                    dropDownText.Text = "Actor";

                    var type = VariableType.Actor;

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
                                (n as GetVariableNode).SelectedVariable == variableItem))
                            {
                                ((GetVariableNode) node).Update(true);
                            }

                            foreach (var node in screenWidget.Nodes.Where(n =>
                                n is SetVariableNode && n.NodeType == NodeType.SetVariable &&
                                (n as SetVariableNode).SelectedVariable == variableItem))
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
                            (n as GetVariableNode).SelectedVariable == variableItem))
                        {
                            dropDownText.Text = "SELECT VARIABLE";
                            ((GetVariableNode) node).SelectedVariable = null;
                        }

                        foreach (var node in screenWidget.Nodes.Where(n =>
                            n is SetVariableNode && n.NodeType == NodeType.SetVariable &&
                            (n as SetVariableNode).SelectedVariable == variableItem))
                        {
                            dropDownText.Text = "SELECT VARIABLE";
                            ((SetVariableNode) node).SelectedVariable = null;
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
            };

            AddChild(addButton);

            // End

            AddChild(addNodeButton = new ButtonWidget(snw.ModData));
            addNodeButton.Bounds = new Rectangle(5, 400, 190, 25);
            addNodeButton.Text = "Add Node";
            addNodeButton.OnClick = () => { screenWidget.AddNode(nodeType); };

            var closeButton = new ButtonWidget(snw.ModData);
            AddChild(closeButton);
            closeButton.Bounds = new Rectangle(5, 600, 190, 25);
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
        }

        void AddNodesList()
        {
            List<NodeType> outputNodeTypes = new List<NodeType>
            {
                NodeType.MapInfoNode,
                NodeType.MapInfoActorInfoNode,
                NodeType.MapInfoActorReference,
                NodeType.GetVariable,
                NodeType.SetVariable
            };

            List<string> outputNodeStrings = new List<string>
            {
                "Info: Map Info",
                "Info: Actor Info",
                "Info: Actor",
                "Variable: Get",
                "Variable: Set"
            };

            nodeType = outputNodeTypes.First();
            AddChild(createNodesList = new DropDownButtonWidget(Snw.ModData));
            createNodesList.Bounds = new Rectangle(5, 5 + 26, 190, 25);

            Func<NodeType, ScrollItemWidget, ScrollItemWidget> setupItemOutput = (option, template) =>
            {
                var item = ScrollItemWidget.Setup(template, () => nodeType == option, () =>
                {
                    nodeType = option;

                    createNodesList.Text = outputNodeStrings[outputNodeTypes.IndexOf(nodeType)];
                    createActorNodesList.Text = "- Actor Nodes -";
                    triggerNodesList.Text = "- Trigger Nodes -";
                    groupNodesList.Text = "- Group Nodes -";
                    arithmeticNodesList.Text = "- Arithmetic Nodes -";
                    functionNodesList.Text = "- Function Nodes -";
                    uiNodesList.Text = "- Ui Nodes -";
                    conditionNodesList.Text = "- Condition Nodes -";
                });

                item.Get<LabelWidget>("LABEL").GetText = () => outputNodeStrings[outputNodeTypes.IndexOf(option)];

                return item;
            };

            createNodesList.OnClick = () =>
            {
                var nodes = outputNodeTypes;
                createNodesList.ShowDropDown("LABEL_DROPDOWN_TEMPLATE", 270, nodes, setupItemOutput);
            };
        }

        void AddActorList()
        {
            List<NodeType> actorNodeTypes = new List<NodeType>
            {
                NodeType.ActorCreateActor,
                NodeType.ActorGetInformations,
                NodeType.ActorKill,
                NodeType.ActorRemove,
                NodeType.ActorChangeOwner,
                NodeType.ActorQueueMove,
                NodeType.ActorQueueAttack,
                NodeType.ActorQueueHunt,
                NodeType.ActorQueueAttackMoveActivity,
                NodeType.ActorQueueSell,
                NodeType.ActorQueueFindResources
            };

            List<string> actorNodeStrings = new List<string>
            {
                "Actor: Create Actor",
                "Actor: Informations of Actor",
                "Actor: Kill",
                "Actor: Remove",
                "Actor: Change Owner",
                "Activity: Queue Move",
                "Activity: Queue Attack",
                "Activity: Queue Hunt",
                "Activity: Queue AttackMoveActivity",
                "Activity: Queue Sell",
                "Activity: QueueFindResources"
            };

            AddChild(createActorNodesList = new DropDownButtonWidget(Snw.ModData));
            createActorNodesList.Bounds = new Rectangle(5, 5 + 26 + 26, 190, 25);

            Func<NodeType, ScrollItemWidget, ScrollItemWidget> setupItemActor = (option, template) =>
            {
                var item = ScrollItemWidget.Setup(template, () => nodeType == option, () =>
                {
                    nodeType = option;

                    createActorNodesList.Text = actorNodeStrings[actorNodeTypes.IndexOf(nodeType)];
                    createNodesList.Text = "- Info Nodes -";
                    triggerNodesList.Text = "- Trigger Nodes -";
                    groupNodesList.Text = "- Group Nodes -";
                    arithmeticNodesList.Text = "- Arithmetic Nodes -";
                    functionNodesList.Text = "- Function Nodes -";
                    uiNodesList.Text = "- Ui Nodes -";
                    conditionNodesList.Text = "- Condition Nodes -";
                });

                item.Get<LabelWidget>("LABEL").GetText = () => actorNodeStrings[actorNodeTypes.IndexOf(option)];

                return item;
            };

            createActorNodesList.OnClick = () =>
            {
                var nodes = actorNodeTypes;
                createActorNodesList.ShowDropDown("LABEL_DROPDOWN_TEMPLATE", 270, nodes, setupItemActor);
            };
        }

        void AddTriggerList()
        {
            List<NodeType> triggerNodeTypes = new List<NodeType>
            {
                NodeType.TriggerWorldLoaded,
                NodeType.TriggerCreateTimer,
                NodeType.TriggerTick,
                NodeType.TriggerOnEnteredFootprint,
                NodeType.TriggerOnEnteredRange,
                NodeType.TriggerOnIdle,
                NodeType.TriggerOnKilled,
                NodeType.TriggerOnAllKilled,
                NodeType.TimerStop,
                NodeType.TimerStart,
                NodeType.TimerReset
            };

            List<string> triggerNodeStrings = new List<string>
            {
                "Trigger: World Loaded",
                "Trigger: Create Timer",
                "Trigger: On Tick",
                "Trigger: On Entered Footprint",
                "Trigger: On Entered Range",
                "Trigger: On Actor Idle",
                "Trigger: On Actor Killed",
                "Trigger: On All Actors Killed",
                "Timer: Stop Timer",
                "Timer: Start Timer",
                "Timer: Reset Timer"
            };

            AddChild(triggerNodesList = new DropDownButtonWidget(Snw.ModData));
            triggerNodesList.Bounds = new Rectangle(5, 5 + 26 + 26 + 26, 190, 25);

            Func<NodeType, ScrollItemWidget, ScrollItemWidget> setupItemTrigger = (option, template) =>
            {
                var item = ScrollItemWidget.Setup(template, () => nodeType == option, () =>
                {
                    nodeType = option;

                    triggerNodesList.Text = triggerNodeStrings[triggerNodeTypes.IndexOf(nodeType)];
                    createNodesList.Text = "- Info Nodes -";
                    createActorNodesList.Text = "- Actor Nodes -";
                    groupNodesList.Text = "- Group Nodes -";
                    arithmeticNodesList.Text = "- Arithmetic Nodes -";
                    functionNodesList.Text = "- Function Nodes -";
                    uiNodesList.Text = "- Ui Nodes -";
                    conditionNodesList.Text = "- Condition Nodes -";
                });

                item.Get<LabelWidget>("LABEL").GetText = () => triggerNodeStrings[triggerNodeTypes.IndexOf(option)];

                return item;
            };

            triggerNodesList.OnClick = () =>
            {
                var nodes = triggerNodeTypes;
                triggerNodesList.ShowDropDown("LABEL_DROPDOWN_TEMPLATE", 270, nodes, setupItemTrigger);
            };
        }

        void AddGroupList()
        {
            List<NodeType> groupNodeTypes = new List<NodeType>
            {
                NodeType.GroupPlayerGroup,
                NodeType.GroupActorInfoGroup,
                NodeType.GroupActorGroup,
                NodeType.FinActorsInCircle,
                NodeType.FindActorsOnFootprint,
                NodeType.FilterActorGroup
            };

            List<string> groupNodeStrings = new List<string>
            {
                "Group: Player Group",
                "Group: Actor Info Group",
                "Group: Actor Group",
                "Group: Find Actors in Circle",
                "Group: Find Actors on Footprint",
                "Group: Filter Actors in Group"
            };

            AddChild(groupNodesList = new DropDownButtonWidget(Snw.ModData));
            groupNodesList.Bounds = new Rectangle(5, 5 + 26 + 26 + 26 + 26, 190, 25);

            Func<NodeType, ScrollItemWidget, ScrollItemWidget> setupItemGroup = (option, template) =>
            {
                var item = ScrollItemWidget.Setup(template, () => nodeType == option, () =>
                {
                    nodeType = option;

                    groupNodesList.Text = groupNodeStrings[groupNodeTypes.IndexOf(nodeType)];
                    createActorNodesList.Text = "- Actor Nodes -";
                    triggerNodesList.Text = "- Trigger Nodes -";
                    createNodesList.Text = "- Info Nodes -";
                    arithmeticNodesList.Text = "- Arithmetic Nodes -";
                    functionNodesList.Text = "- Function Nodes -";
                    uiNodesList.Text = "- Ui Nodes -";
                    conditionNodesList.Text = "- Condition Nodes -";
                });

                item.Get<LabelWidget>("LABEL").GetText = () => groupNodeStrings[groupNodeTypes.IndexOf(option)];

                return item;
            };

            groupNodesList.OnClick = () =>
            {
                var nodes = groupNodeTypes;
                groupNodesList.ShowDropDown("LABEL_DROPDOWN_TEMPLATE", 270, nodes, setupItemGroup);
            };
        }

        void AddArithmeticList()
        {
            List<NodeType> nodeTypes = new List<NodeType>
            {
                NodeType.ArithmeticsAnd,
                NodeType.ArithmeticsOr,
                NodeType.ArithmeticsMath,
                NodeType.CountNode,
                NodeType.CompareActors,
                NodeType.DoMultiple
            };

            List<string> nodeStrings = new List<string>
            {
                "Arithmetics: And Trigger",
                "Arithmetics: Or Trigger",
                "Arithmetics: Math",
                "Arithmetics: Get Count",
                "Compare: Actors",
                "Repeating: Action"
            };

            AddChild(arithmeticNodesList = new DropDownButtonWidget(Snw.ModData));
            arithmeticNodesList.Bounds = new Rectangle(5, 5 + 26 + 26 + 26 + 26 + 26, 190, 25);

            Func<NodeType, ScrollItemWidget, ScrollItemWidget> setupItemGroup = (option, template) =>
            {
                var item = ScrollItemWidget.Setup(template, () => nodeType == option, () =>
                {
                    nodeType = option;

                    arithmeticNodesList.Text = nodeStrings[nodeTypes.IndexOf(nodeType)];
                    createActorNodesList.Text = "- Actor Nodes -";
                    triggerNodesList.Text = "- Trigger Nodes -";
                    createNodesList.Text = "- Info Nodes -";
                    groupNodesList.Text = "- Group Nodes -";
                    functionNodesList.Text = "- Function Nodes -";
                    uiNodesList.Text = "- Ui Nodes -";
                    conditionNodesList.Text = "- Condition Nodes -";
                });

                item.Get<LabelWidget>("LABEL").GetText = () => nodeStrings[nodeTypes.IndexOf(option)];

                return item;
            };

            arithmeticNodesList.OnClick = () =>
            {
                var nodes = nodeTypes;
                arithmeticNodesList.ShowDropDown("LABEL_DROPDOWN_TEMPLATE", 270, nodes, setupItemGroup);
            };
        }

        void AddFunctionsList()
        {
            List<NodeType> nodeTypes = new List<NodeType>
            {
                NodeType.Reinforcements,
                NodeType.ReinforcementsWithTransport,
                NodeType.CreateEffect,
                NodeType.TimedExecution
            };

            List<string> nodeStrings = new List<string>
            {
                "Function: Reinforcements",
                "Function: Reinforce (Transport)",
                "Function: Create Effect",
                "Function: Timed Execution"
            };

            AddChild(functionNodesList = new DropDownButtonWidget(Snw.ModData));
            functionNodesList.Bounds = new Rectangle(5, 5 + 26 + 26 + 26 + 26 + 26 + 26, 190, 25);

            Func<NodeType, ScrollItemWidget, ScrollItemWidget> setupItemGroup = (option, template) =>
            {
                var item = ScrollItemWidget.Setup(template, () => nodeType == option, () =>
                {
                    nodeType = option;

                    functionNodesList.Text = nodeStrings[nodeTypes.IndexOf(nodeType)];
                    createActorNodesList.Text = "- Actor Nodes -";
                    triggerNodesList.Text = "- Trigger Nodes -";
                    createNodesList.Text = "- Info Nodes -";
                    groupNodesList.Text = "- Group Nodes -";
                    arithmeticNodesList.Text = "- Arithmetic Nodes -";
                    conditionNodesList.Text = "- Condition Nodes -";
                });

                item.Get<LabelWidget>("LABEL").GetText = () => nodeStrings[nodeTypes.IndexOf(option)];

                return item;
            };

            functionNodesList.OnClick = () =>
            {
                var nodes = nodeTypes;
                functionNodesList.ShowDropDown("LABEL_DROPDOWN_TEMPLATE", 270, nodes, setupItemGroup);
            };
        }

        void AddUiList()
        {
            List<NodeType> nodeTypes = new List<NodeType>
            {
                NodeType.UiPlayNotification,
                NodeType.UiPlaySound,
                NodeType.UiRadarPing,
                NodeType.UiTextMessage,
                NodeType.UiAddMissionText,
                NodeType.UiNewObjective,
                NodeType.UiCompleteObjective,
                NodeType.UiFailObjective,
                NodeType.TextChoice,
                NodeType.SetCameraPosition,
                NodeType.CameraRide,
                NodeType.GlobalLightning
            };

            List<string> nodeStrings = new List<string>
            {
                "Ui: Play Notification",
                "Ui: Play Play Sound at",
                "Ui: Radar Ping",
                "Ui: Chat Text message",
                "Ui: Show Mission Text",
                "Ui: Add Objective",
                "Ui: Complete Objective",
                "Ui: Fail Objective",
                "Ui: Text Choice",
                "Player: Set Camera Location",
                "Player: Camera Ride",
                "Global Lightning: Change RGBA"
            };

            AddChild(uiNodesList = new DropDownButtonWidget(Snw.ModData));
            uiNodesList.Bounds = new Rectangle(5, 5 + 26 + 26 + 26 + 26 + 26 + 26 + 26, 190, 25);

            Func<NodeType, ScrollItemWidget, ScrollItemWidget> setupItemGroup = (option, template) =>
            {
                var item = ScrollItemWidget.Setup(template, () => nodeType == option, () =>
                {
                    nodeType = option;

                    uiNodesList.Text = nodeStrings[nodeTypes.IndexOf(nodeType)];
                    createActorNodesList.Text = "- Actor Nodes -";
                    triggerNodesList.Text = "- Trigger Nodes -";
                    createNodesList.Text = "- Info Nodes -";
                    groupNodesList.Text = "- Group Nodes -";
                    arithmeticNodesList.Text = "- Arithmetic Nodes -";
                    functionNodesList.Text = "- Function Nodes -";
                    conditionNodesList.Text = "- Condition Nodes -";
                });

                item.Get<LabelWidget>("LABEL").GetText = () => nodeStrings[nodeTypes.IndexOf(option)];

                return item;
            };

            uiNodesList.OnClick = () =>
            {
                var nodes = nodeTypes;
                uiNodesList.ShowDropDown("LABEL_DROPDOWN_TEMPLATE", 270, nodes, setupItemGroup);
            };
        }

        void AddConditionList()
        {
            List<NodeType> nodeTypes = new List<NodeType>
            {
                NodeType.CheckCondition,
                NodeType.CompareActor,
                NodeType.CompareNumber,
                NodeType.CompareActorInfo,
                NodeType.IsDead,
                NodeType.IsAlive,
                NodeType.IsPlaying,
                NodeType.HasLost,
                NodeType.HasWon,
                NodeType.IsHumanPlayer,
                NodeType.IsBot,
                NodeType.IsNoncombatant
            };

            List<string> nodeStrings = new List<string>
            {
                "Check Condition Node",
                "Con.: Same actor",
                "Con.: Same number",
                "Con.: Same actortype",
                "Con.: is dead",
                "Con.: is alive",
                "Player: is Playing",
                "Player: has Lost",
                "Player: has Won",
                "Player: is Human",
                "Player: is Bot",
                "Player: is Noncombatant"
            };

            AddChild(conditionNodesList = new DropDownButtonWidget(Snw.ModData));
            conditionNodesList.Bounds = new Rectangle(5, 5 + 26 + 26 + 26 + 26 + 26 + 26 + 26 + 26, 190, 25);

            Func<NodeType, ScrollItemWidget, ScrollItemWidget> setupItemGroup = (option, template) =>
            {
                var item = ScrollItemWidget.Setup(template, () => nodeType == option, () =>
                {
                    nodeType = option;

                    conditionNodesList.Text = nodeStrings[nodeTypes.IndexOf(nodeType)];
                    createActorNodesList.Text = "- Actor Nodes -";
                    triggerNodesList.Text = "- Trigger Nodes -";
                    createNodesList.Text = "- Info Nodes -";
                    groupNodesList.Text = "- Group Nodes -";
                    arithmeticNodesList.Text = "- Arithmetic Nodes -";
                    functionNodesList.Text = "- Function Nodes -";
                    uiNodesList.Text = "- Ui Nodes -";
                });

                item.Get<LabelWidget>("LABEL").GetText = () => nodeStrings[nodeTypes.IndexOf(option)];

                return item;
            };

            conditionNodesList.OnClick = () =>
            {
                var nodes = nodeTypes;
                conditionNodesList.ShowDropDown("LABEL_DROPDOWN_TEMPLATE", 270, nodes, setupItemGroup);
            };
        }

        public override bool HandleMouseInput(MouseInput mi)
        {
            if (!EventBounds.Contains(mi.Location))
            {
                return false;
            }

            return true;
        }

        public override void Draw()
        {
            WidgetUtils.DrawPanel(Background,
                new Rectangle(RenderBounds.X - 3, RenderBounds.Y - 3, RenderBounds.Width + 6, RenderBounds.Height + 6));
        }
    }
}