using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenRA.Mods.Common.Widgets.ScriptNodes.EditorNodeBrushes;
using OpenRA.Mods.Common.Widgets.ScriptNodes.NodeEditorTraits;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.NodeInfos.InfoNodeInfos
{
    public class ActorsOnMapInfo : NodeInfo
    {
        public new static Dictionary<string, BuildNodeConstructorInfo> NodeConstructorInformation =
            new Dictionary<string, BuildNodeConstructorInfo>()
            {
                {
                    "MapInfoActorReference", new BuildNodeConstructorInfo
                    {
                        LogicClass = typeof(NodeLogic),
                        Nesting = new[] {"Info Nodes"},
                        Name = "Actor on Map",

                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Actor,
                                "First actor of all selected actors on the map"),
                            new Tuple<ConnectionType, string>(ConnectionType.ActorList, "List of all selected actors")
                        }
                    }
                },
            };

        ButtonWidget button;

        public ActorsOnMapInfo(string nodeType, string nodeId, string nodeName) : base(nodeType, nodeId, nodeName)
        {
        }

        public override void WidgetInitialize(NodeWidget widget)
        {
            button = new ButtonWidget(widget.Screen.NodeScriptContainerWidget.ModData);
            widget.AddChild(button);
            button.Bounds = new Rectangle(widget.FreeWidgetEntries.X, widget.FreeWidgetEntries.Y + 34,
                widget.FreeWidgetEntries.Width, 25);
            button.Text = "Add Actor";
            button.OnClick = () =>
            {
                widget.Editor.SetBrush(new EditorNodeBrushBrush(
                    CellPicking.Actor,
                    widget.OutConnections.First(c => c.ConnectionTyp == ConnectionType.ActorList),
                    widget.Editor,
                    widget.Screen.NodeScriptContainerWidget.WorldRenderer,
                    () =>
                    {
                        button.Text =
                            widget.OutConnections.First(c => c.ConnectionTyp == ConnectionType.ActorList)
                                .ActorPreviews != null
                                ? "Group: " + widget.OutConnections
                                      .First(c => c.ConnectionTyp == ConnectionType.ActorList)
                                      .ActorPreviews.Count()
                                : "None";
                    }));
            };
        }

        public override void WidgetAddOutConConstructor(OutConnection connection, NodeWidget widget)
        {
            base.WidgetAddOutConConstructor(connection, widget);

            if (connection.ActorPreviews != null && connection.ActorPreviews.Any())
                button.Text = "Group: " + connection.ActorPreviews.Length;
        }

        public override void WidgetTick(NodeWidget widget)
        {
            base.WidgetTick(widget);

            var first = widget.OutConnections.FirstOrDefault(c => c.ConnectionTyp == ConnectionType.ActorList);
            var second = widget.OutConnections.FirstOrDefault(c => c.ConnectionTyp == ConnectionType.Actor);

            if (first != null && second != null)
                second.ActorPrev = first.ActorPreviews != null && first.ActorPreviews.Any()
                    ? first.ActorPreviews.First()
                    : null;
        }
    }
}