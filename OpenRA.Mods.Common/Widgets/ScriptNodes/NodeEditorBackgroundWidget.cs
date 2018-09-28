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

        readonly NodeEditorNodeScreenWidget screenWidget;
        readonly DropDownButtonWidget createNodesList;
        readonly DropDownButtonWidget createActorNodesList;
        readonly DropDownButtonWidget triggerNodesList;

        NodeType nodeType;

        ButtonWidget addNodeButton;

        // List<ButtonWidget> Buttons = new List<ButtonWidget>();

        [ObjectCreator.UseCtor]
        public NodeEditorBackgroundWidget(ScriptNodeWidget snw)
        {
            Snw = snw;

            Children.Add(screenWidget = new NodeEditorNodeScreenWidget(Snw, this));

            Bounds = new Rectangle(100, 100, Snw.RenderBounds.Width - 200, Snw.RenderBounds.Height - 200);

            //  Output Nodes
            List<NodeType> outputNodeTypes = new List<NodeType>
            {
                NodeType.PlayerOutput,
                NodeType.LocationOutput,
                NodeType.PathNode,
                NodeType.ActorOutPut,
                NodeType.CellArrayOutput,
                NodeType.CellRange
            };

            List<string> outputNodeStrings = new List<string>
            {
                "Player Output",
                "Location Output",
                "Path Output",
                "Actor Info Output",
                "Cell Array Output",
                "Cell and Range"
            };

            nodeType = outputNodeTypes.First();

            AddChild(createNodesList = new DropDownButtonWidget(snw.ModData));
            createNodesList.Bounds = new Rectangle(5, 5 + 26, 190, 25);


            Func<NodeType, ScrollItemWidget, ScrollItemWidget> setupItemOutput = (option, template) =>
            {
                var item = ScrollItemWidget.Setup(template, () => nodeType == option, () =>
                {
                    nodeType = option;

                    createNodesList.Text = outputNodeStrings[outputNodeTypes.IndexOf(nodeType)];
                    createActorNodesList.Text = "- none -";
                    triggerNodesList.Text = "- none -";
                });

                item.Get<LabelWidget>("LABEL").GetText = () => outputNodeStrings[outputNodeTypes.IndexOf(option)];

                return item;
            };

            createNodesList.OnClick = () =>
            {
                var nodes = outputNodeTypes;
                createNodesList.ShowDropDown("LABEL_DROPDOWN_TEMPLATE", 270, nodes, setupItemOutput);
            };

            //  Actor Nodes
            List<NodeType> actorNodeTypes = new List<NodeType>
            {
                NodeType.CreateActor,
                NodeType.RemoveActor,
                NodeType.KillActor,
                NodeType.MoveActor,
                NodeType.ActorFollowPath
            };

            List<string> actorNodeStrings = new List<string>
            {
                "Actor: Create",
                "Actor: Remove",
                "Actor: Kill",
                "Actor: Move",
                "Actor: Follow path"
            };

            AddChild(createActorNodesList = new DropDownButtonWidget(snw.ModData));
            createActorNodesList.Bounds = new Rectangle(5, 5 + 26 + 26, 190, 25);


            Func<NodeType, ScrollItemWidget, ScrollItemWidget> setupItemActor = (option, template) =>
            {
                var item = ScrollItemWidget.Setup(template, () => nodeType == option, () =>
                {
                    nodeType = option;

                    createActorNodesList.Text = actorNodeStrings[actorNodeTypes.IndexOf(nodeType)];
                    createNodesList.Text = "- none -";
                    triggerNodesList.Text = "- none -";
                });

                item.Get<LabelWidget>("LABEL").GetText = () => actorNodeStrings[actorNodeTypes.IndexOf(option)];

                return item;
            };

            createActorNodesList.OnClick = () =>
            {
                var nodes = actorNodeTypes;
                createActorNodesList.ShowDropDown("LABEL_DROPDOWN_TEMPLATE", 270, nodes, setupItemActor);
            };

            //  Trigger Nodes
            List<NodeType> triggerNodeTypes = new List<NodeType>
            {
                NodeType.ActorKilledTrigger,
                NodeType.ActorIdleTrigger,
                NodeType.MathTimerTrigger
            };

            List<string> triggerNodeStrings = new List<string>
            {
                "Trigger: Actor killed",
                "Trigger: Actor idle",
                "Trigger: Timer"
            };

            AddChild(triggerNodesList = new DropDownButtonWidget(snw.ModData));
            triggerNodesList.Bounds = new Rectangle(5, 5 + 26 + 26 + 26, 190, 25);

            Func<NodeType, ScrollItemWidget, ScrollItemWidget> setupItemTrigger = (option, template) =>
            {
                var item = ScrollItemWidget.Setup(template, () => nodeType == option, () =>
                {
                    nodeType = option;

                    triggerNodesList.Text = triggerNodeStrings[triggerNodeTypes.IndexOf(nodeType)];
                    createNodesList.Text = "- none -";
                    createActorNodesList.Text = "- none -";
                });

                item.Get<LabelWidget>("LABEL").GetText = () => triggerNodeStrings[triggerNodeTypes.IndexOf(option)];

                return item;
            };

            triggerNodesList.OnClick = () =>
            {
                var nodes = triggerNodeTypes;
                triggerNodesList.ShowDropDown("LABEL_DROPDOWN_TEMPLATE", 270, nodes, setupItemTrigger);
            };

            AddChild(addNodeButton = new ButtonWidget(snw.ModData));
            addNodeButton.Bounds = new Rectangle(5, 400, 190, 25);
            addNodeButton.Text = "Add Node";
            addNodeButton.OnClick = () => { screenWidget.AddNode(nodeType); };
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