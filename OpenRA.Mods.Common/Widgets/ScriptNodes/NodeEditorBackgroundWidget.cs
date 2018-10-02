using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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

        NodeType nodeType;

        ButtonWidget addNodeButton;
        // List<ButtonWidget> Buttons = new List<ButtonWidget>();

        [ObjectCreator.UseCtor]
        public NodeEditorBackgroundWidget(ScriptNodeWidget snw)
        {
            Snw = snw;

            Children.Add(screenWidget = new NodeEditorNodeScreenWidget(Snw, this));

            Bounds = new Rectangle(100, 100, Snw.RenderBounds.Width - 200, Snw.RenderBounds.Height - 200);

            AddNodesList();
            AddActorList();
            AddTriggerList();
            AddGroupList();
            AddArithmeticList();
            AddFunctionsList();

            createActorNodesList.Text = "- Actor Nodes -";
            triggerNodesList.Text = "- Trigger Nodes -";
            createNodesList.Text = "- Info Nodes -";
            groupNodesList.Text = "- Group Nodes -";
            arithmeticNodesList.Text = "- Arithmetic Nodes -";
            functionNodesList.Text = "- Function Nodes -";

            AddChild(addNodeButton = new ButtonWidget(snw.ModData));
            addNodeButton.Bounds = new Rectangle(5, 400, 190, 25);
            addNodeButton.Text = "Add Node";
            addNodeButton.OnClick = () => { screenWidget.AddNode(nodeType); };

            var closeButton = new ButtonWidget(snw.ModData);
            AddChild(closeButton);
            closeButton.Bounds = new Rectangle(5, 600, 190, 25);
            closeButton.Text = "Close";
            closeButton.OnClick = () => { Visible = false; };

        }

        void AddNodesList()
        {
            //  Output Nodes
            List<NodeType> outputNodeTypes = new List<NodeType>
            {
                NodeType.MapInfoNode
            };

            List<string> outputNodeStrings = new List<string>
            {
                "Info: Map Info"
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
            //  Actor Nodes
            List<NodeType> actorNodeTypes = new List<NodeType>
            {
                NodeType.ActorCreateNode,
                NodeType.ActorRemoveNode,
                NodeType.ActorKillNode,
                NodeType.ActorFollowPathNode,
                NodeType.ActorGetInfoNode
            };

            List<string> actorNodeStrings = new List<string>
            {
                "Actor: Create",
                "Actor: Remove",
                "Actor: Kill",
                "Actor: Move",
                "Actor: Follow path",
                "Actor: Information"
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
            // Trigger Nodes
            List<NodeType> triggerNodeTypes = new List<NodeType>
            {
                NodeType.TriggerWorldLoadedNode,
                NodeType.TriggerActorKilledNode,
                NodeType.TriggerActorOnIdleNode,
                NodeType.TriggerTimerNode
            };

            List<string> triggerNodeStrings = new List<string>
            {
                "Trigger: World Loaded",
                "Trigger: Actor killed",
                "Trigger: Actor idle",
                "Trigger: Timer"
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
            //  Group Nodes
            List<NodeType> groupNodeTypes = new List<NodeType>
            {
                NodeType.GroupCreateGroupNode,
                NodeType.GroupFindActorsInCircleNode,
                NodeType.GroupFindActorsOnCellsNode
            };

            List<string> groupNodeStrings = new List<string>
            {
                "Group: Create group",
                "Group: Find Actors in circle",
                "Group: Find Actors on cells"
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
            //  Group Nodes
            List<NodeType> nodeTypes = new List<NodeType>
            {
                NodeType.ArithmeticsSelectByNode,
                NodeType.ArithmeticsSelectNode,
                NodeType.ArithmeticsCompareNode,
                NodeType.ArithmeticsForEachNode
            };

            List<string> nodeStrings = new List<string>
            {
                "Arithmetic: Select by",
                "Arithmetic: Select",
                "Arithmetic: Compare",
                "Arithmetic: For Each"
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
            //  Group Nodes
            List<NodeType> nodeTypes = new List<NodeType>
            {
                NodeType.FunctionReinforcmentsNode,
                NodeType.FunctionReinforceWithTransPort
            };

            List<string> nodeStrings = new List<string>
            {
                "Function: Reinforcements",
                "Function: Reinforcements (Transport)"
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
            // WidgetUtils.FillRectWithColor(new Rectangle(RenderBounds.X - 3, RenderBounds.Y - 3, RenderBounds.Width + 6, RenderBounds.Height + 6), Color.Black);
            // WidgetUtils.FillRectWithColor(new Rectangle(RenderBounds.X, RenderBounds.Y, RenderBounds.Width, RenderBounds.Height), Color.DarkGray);
        }
    }
}