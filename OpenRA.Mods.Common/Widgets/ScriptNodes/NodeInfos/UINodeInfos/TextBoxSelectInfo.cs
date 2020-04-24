using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.NodeInfos.UINodeInfos
{
    public class TextBoxSelectInfo : NodeInfo
    {
        public new static Dictionary<string, BuildNodeConstructorInfo> NodeConstructorInformation =
            new Dictionary<string, BuildNodeConstructorInfo>()
            {
                {
                    "TextChoice", new BuildNodeConstructorInfo
                    {
                        Nesting = new[] {"User Interface", "General UI"},
                        Name = "Text Choice",

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.String, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        }
                    }
                },
            };

        readonly List<ButtonWidget> parralelButtons = new List<ButtonWidget>();

        public TextBoxSelectInfo(string nodeType, string nodeId, string nodeName) : base(nodeType, nodeId, nodeName)
        {
        }

        public override void WidgetInitialize(NodeWidget widget)
        {
            widget.IsIncorrectConnected = () =>
                widget.InConnections.Any(inCon => inCon.In != null) && widget.InConnections.FirstOrDefault(inCon =>
                    inCon.In != null && inCon.ConnectionTyp == ConnectionType.String) != null;

            ButtonWidget addButton;
            widget.AddChild(addButton = new ButtonWidget(widget.Screen.NodeScriptContainerWidget.ModData));
            addButton.Bounds = new Rectangle(widget.FreeWidgetEntries.X + 10, widget.FreeWidgetEntries.Y + 21,
                widget.WidgetEntries.Width - 20, 20);
            addButton.Text = "Add Choice";
            addButton.OnClick = () =>
            {
                var outcon = new OutConnection(ConnectionType.Exec, widget);
                var incon = new InConnection(ConnectionType.String, widget);
                widget.AddOutConnection(outcon);
                widget.AddOutConConstructor(outcon);
                widget.AddInConnection(incon);
            };
        }

        public override void WidgetAddOutConConstructor(OutConnection connection, NodeWidget widget)
        {
            base.WidgetAddOutConConstructor(connection, widget);

            var button = new ButtonWidget(widget.Screen.NodeScriptContainerWidget.ModData);
            button.Text = "Remove";

            widget.AddChild(button);
            parralelButtons.Add(button);

            button.OnClick = () =>
            {
                for (var j = 0; j < parralelButtons.Count; j++)
                    if (parralelButtons[j] == button)
                    {
                        widget.RemoveChild(parralelButtons[j]);

                        parralelButtons.RemoveAt(j);
                        widget.OutConnections.RemoveAt(j);
                        widget.InConnections.RemoveAt(j + 2);

                        break;
                    }
            };
        }

        public override void WidgetTick(NodeWidget widget)
        {
            base.WidgetTick(widget);

            for (var i = 0; i < parralelButtons.Count; i++)
            {
                var splitHeight = widget.RenderBounds.Height / (parralelButtons.Count + 3);
                parralelButtons[i].Bounds = new Rectangle(widget.FreeWidgetEntries.X + 20,
                    widget.FreeWidgetEntries.Y + splitHeight * (i + 3), widget.WidgetEntries.Width - 40, 20);
            }

            var nsplitHeight = (widget.RenderBounds.Height + 20) / (widget.OutConnections.Count + 3);
            for (var i = 0; i < widget.OutConnections.Count; i++)
            {
                var rect = new Rectangle(widget.RenderBounds.X + widget.RenderBounds.Width - 5,
                    widget.RenderBounds.Y + nsplitHeight * (i + 3), 20, 20);
                widget.OutConnections[i].InWidgetPosition = rect;
            }
        }

        List<InConnection> inCons = new List<InConnection>();
        public bool Listen;
        public List<Tuple<InConnection, string>> Options = new List<Tuple<InConnection, string>>();
        public string Text;
        NodeLogic logic;

        public override void LogicExecute(World world, NodeLogic logic)
        {
            this.logic = logic;
            inCons = logic.InConnections.Where(i => i.ConnectionTyp == ConnectionType.String).ToList();
            inCons.Remove(inCons.First());

            Text = logic.GetLinkedConnectionFromInConnection(ConnectionType.String, 0).String;

            foreach (var inCon in inCons) Options.Add(new Tuple<InConnection, string>(inCon, inCon.In.String));

            world.SetPauseState(true);
            Listen = true;
        }

        public void ExecuteBranch(InConnection choice)
        {
            if (choice == null)
                return;

            var oCon = logic.OutConnections[inCons.IndexOf(choice)];
            if (oCon != null)
                foreach (var node in logic.IngameNodeScriptSystem.NodeLogics.Where(n =>
                    n.InConnections.FirstOrDefault(c => c.ConnectionTyp == ConnectionType.Exec) != null))
                {
                    var inCon = node.InConnections.FirstOrDefault(c =>
                        c.ConnectionTyp == ConnectionType.Exec && c.In == oCon);
                    if (inCon != null)
                        inCon.Execute = true;
                }

            Listen = false;
        }
    }
}