using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenRA.Graphics;
using OpenRA.Mods.Common.Widgets.ScriptNodes.Library;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.InfoNodes;
using OpenRA.Widgets;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes
{
    public class NodeEditorBackgroundWidget : Widget
    {
        // Background
        public readonly string Background = "panel-black";

        public ScriptNodeWidget Snw;

        NodeEditorNodeScreenWidget screenWidget;
        DropDownButtonWidget createNodesList;
        DropDownButtonWidget createActorNodesList;
        DropDownButtonWidget triggerNodesList;
        DropDownButtonWidget groupNodesList;
        DropDownButtonWidget arithmeticNodesList;
        DropDownButtonWidget functionNodesList;
        DropDownButtonWidget uiNodesList;

        NodeType nodeType;

        ButtonWidget addNodeButton;

        // List<ButtonWidget> Buttons = new List<ButtonWidget>();

        [ObjectCreator.UseCtor]
        public NodeEditorBackgroundWidget(ScriptNodeWidget snw, WorldRenderer worldRenderer, World world)
        {
            Snw = snw;

            Children.Add(screenWidget = new NodeEditorNodeScreenWidget(Snw, this, worldRenderer, world));

            Bounds = new Rectangle(100, 100, Snw.RenderBounds.Width - 200, Snw.RenderBounds.Height - 200);

            AddNodesList();
            AddActorList();
            AddTriggerList();
            AddGroupList();
            AddArithmeticList();
            AddFunctionsList();
            AddUiList();

            createActorNodesList.Text = "- Actor Nodes -";
            triggerNodesList.Text = "- Trigger Nodes -";
            createNodesList.Text = "- Info Nodes -";
            groupNodesList.Text = "- Group Nodes -";
            arithmeticNodesList.Text = "- Arithmetic Nodes -";
            functionNodesList.Text = "- Function Nodes -";
            uiNodesList.Text = "- Ui Nodes -";

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

                Snw.World.WorldActor.Trait<EditorNodeLayer>().NodeInfo = nodeInfos;

                Visible = false;
            };
        }

        void AddNodesList()
        {
            List<NodeType> outputNodeTypes = new List<NodeType>
            {
                NodeType.MapInfoNode,
                NodeType.MapInfoActorInfoNode,
                NodeType.MapInfoActorReference
            };

            List<string> outputNodeStrings = new List<string>
            {
                "Info: Map Info",
                "Info: Actor Info",
                "Info: Actor"
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
                NodeType.ActorQueueMove,
                NodeType.ActorChangeOwner,
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
                NodeType.GroupActorGroup
            };

            List<string> groupNodeStrings = new List<string>
            {
                "Group: Player Group",
                "Group: Actor Info Group",
                "Group: Actor Group"
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
                NodeType.CreateEffect
            };

            List<string> nodeStrings = new List<string>
            {
                "Function: Reinforcements",
                "Function: Reinforce (Transport)",
                "Function: Create Effect"
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
            //  Group Nodes
            List<NodeType> nodeTypes = new List<NodeType>
            {
                NodeType.UiPlayNotification,
                NodeType.UiPlaySound,
                NodeType.UiRadarPing,
                NodeType.UiTextMessage,
                NodeType.UiAddMissionText
            };

            List<string> nodeStrings = new List<string>
            {
                "Ui: Play Notification",
                "Ui: Play Play Sound at",
                "Ui: Radar Ping",
                "Ui: Chat Text message",
                "Ui: Show Mission Text",
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

        public override void Tick()
        {
            Bounds = new Rectangle(5, 20, Snw.RenderBounds.Width - 20, Snw.RenderBounds.Height - 20);
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
            WidgetUtils.DrawPanel(Background, new Rectangle(RenderBounds.X - 3, RenderBounds.Y - 3, RenderBounds.Width + 6, RenderBounds.Height + 6));
        }
    }
}