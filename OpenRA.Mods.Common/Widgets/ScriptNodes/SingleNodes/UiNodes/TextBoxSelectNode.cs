using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.UiNodes
{
    public class TextBoxSelectNode : NodeWidget
    {
        public new static Dictionary<string, BuildNodeConstructorInfo> NodeConstructorInformation =
            new Dictionary<string, BuildNodeConstructorInfo>()
            {
                {
                    "TextChoice", new BuildNodeConstructorInfo
                    {
                        LogicClass = typeof(TextBoxSelectLogic),
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

        public TextBoxSelectNode(NodeEditorNodeScreenWidget screen, NodeInfo nodeInfo) : base(screen, nodeInfo)
        {
            IsIncorrectConnected = () =>
                InConnections.Any(inCon => inCon.In != null) && InConnections.FirstOrDefault(inCon =>
                    inCon.In != null && inCon.ConnectionTyp == ConnectionType.String) != null;

            ButtonWidget addButton;
            AddChild(addButton = new ButtonWidget(screen.NodeScriptContainerWidget.ModData));
            addButton.Bounds = new Rectangle(FreeWidgetEntries.X + 10, FreeWidgetEntries.Y + 21,
                WidgetEntries.Width - 20, 20);
            addButton.Text = "Add Choice";
            addButton.OnClick = () =>
            {
                var outcon = new OutConnection(ConnectionType.Exec, this);
                var incon = new InConnection(ConnectionType.String, this);
                AddOutConnection(outcon);
                AddOutConConstructor(outcon);
                AddInConnection(incon);
            };
        }

        public override void AddOutConConstructor(OutConnection connection)
        {
            base.AddOutConConstructor(connection);

            var button = new ButtonWidget(Screen.NodeScriptContainerWidget.ModData);
            button.Text = "Remove";

            AddChild(button);
            parralelButtons.Add(button);

            button.OnClick = () =>
            {
                for (var j = 0; j < parralelButtons.Count; j++)
                    if (parralelButtons[j] == button)
                    {
                        RemoveChild(parralelButtons[j]);

                        parralelButtons.RemoveAt(j);
                        OutConnections.RemoveAt(j);
                        InConnections.RemoveAt(j + 2);

                        break;
                    }
            };
        }

        public override void Tick()
        {
            base.Tick();

            for (var i = 0; i < parralelButtons.Count; i++)
            {
                var splitHeight = RenderBounds.Height / (parralelButtons.Count + 3);
                parralelButtons[i].Bounds = new Rectangle(FreeWidgetEntries.X + 20,
                    FreeWidgetEntries.Y + splitHeight * (i + 3), WidgetEntries.Width - 40, 20);
            }

            var nsplitHeight = (RenderBounds.Height + 20) / (OutConnections.Count + 3);
            for (var i = 0; i < OutConnections.Count; i++)
            {
                var rect = new Rectangle(RenderBounds.X + RenderBounds.Width - 5,
                    RenderBounds.Y + nsplitHeight * (i + 3), 20, 20);
                OutConnections[i].InWidgetPosition = rect;
            }
        }
    }

    internal class TextBoxSelectLogic : NodeLogic
    {
        List<InConnection> inCons = new List<InConnection>();
        public bool Listen;
        public List<Tuple<InConnection, string>> Options = new List<Tuple<InConnection, string>>();
        public string Text;

        public TextBoxSelectLogic(NodeInfo nodeInfo, IngameNodeScriptSystem ingameNodeScriptSystem) : base(nodeInfo,
            ingameNodeScriptSystem)
        {
        }

        public override void Execute(World world)
        {
            inCons = InConnections.Where(i => i.ConnectionTyp == ConnectionType.String).ToList();
            inCons.Remove(inCons.First());

            Text = GetLinkedConnectionFromInConnection(ConnectionType.String, 0).String;

            foreach (var inCon in inCons) Options.Add(new Tuple<InConnection, string>(inCon, inCon.In.String));

            world.SetPauseState(true);
            Listen = true;
        }

        public void ExecuteBranch(InConnection choice)
        {
            if (choice == null)
                return;

            var oCon = OutConnections[inCons.IndexOf(choice)];
            if (oCon != null)
                foreach (var node in IngameNodeScriptSystem.NodeLogics.Where(n =>
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