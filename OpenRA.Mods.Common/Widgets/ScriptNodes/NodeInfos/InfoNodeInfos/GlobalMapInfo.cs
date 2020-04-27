using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenRA.Mods.Common.Traits;
using OpenRA.Mods.Common.Widgets.ScriptNodes.EditorNodeBrushes;
using OpenRA.Mods.Common.Widgets.ScriptNodes.NodeEditorTraits;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes;
using OpenRA.Widgets;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.NodeInfos.InfoNodeInfos
{
    public class GlobalMapInfo : NodeInfo
    {
        public new static Dictionary<string, BuildNodeConstructorInfo> NodeConstructorInformation =
            new Dictionary<string, BuildNodeConstructorInfo>()
            {
                {
                    "MapInfoNode", new BuildNodeConstructorInfo
                    {
                        Nesting = new[] {"Info Nodes"},
                        Name = "Global Info",
                    }
                }
            };

        ButtonWidget addButton;
        DropDownButtonWidget createInfoList;
        ConnectionType nodeType;
        List<ButtonWidget> parralelButtons = new List<ButtonWidget>();
        List<Widget> parralelWidgetList = new List<Widget>();

        public GlobalMapInfo(string nodeType, string nodeId, string nodeName) : base(nodeType, nodeId, nodeName)
        {
        }

        public override void WidgetInitialize(NodeWidget widget)
        {
            var typeType = new List<ConnectionType>
            {
                ConnectionType.Integer,
                ConnectionType.Location,
                ConnectionType.CellPath,
                ConnectionType.CellArray,
                ConnectionType.Player,
                ConnectionType.String,
                ConnectionType.Enabled
            };

            var typString = new List<string>
            {
                "Info: Number",
                "Info: Cell",
                "Info: Path",
                "Info: Footprint",
                "Info: Player",
                "Info: string",
                "Info: Enabled"
            };

            nodeType = typeType.First();

            widget.AddChild(addButton = new ButtonWidget(widget.Screen.NodeScriptContainerWidget.ModData));
            addButton.Bounds = new Rectangle(widget.FreeWidgetEntries.X + widget.WidgetEntries.Width - 10 - 25,
                widget.FreeWidgetEntries.Y + 21, 25, 20);
            addButton.Text = "+";

            addButton.OnClick = () =>
            {
                var connection = new OutConnection(nodeType, widget);
                widget.AddOutConnection(connection);
                widget.AddOutConConstructor(connection);
            };

            widget.AddChild(createInfoList = new DropDownButtonWidget(widget.Screen.NodeScriptContainerWidget.ModData));
            createInfoList.Bounds = new Rectangle(widget.FreeWidgetEntries.X, widget.FreeWidgetEntries.Y + 21,
                widget.WidgetEntries.Width - 10 - 25, 20);

            Func<ConnectionType, ScrollItemWidget, ScrollItemWidget> setupItemOutput = (option, template) =>
            {
                var item = ScrollItemWidget.Setup(template, () => nodeType == option, () =>
                {
                    nodeType = option;

                    createInfoList.Text = typString[typeType.IndexOf(nodeType)];
                });

                item.Get<LabelWidget>("LABEL").GetText = () => typString[typeType.IndexOf(option)];

                return item;
            };

            createInfoList.OnClick = () =>
            {
                var nodes = typeType;
                createInfoList.ShowDropDown("LABEL_DROPDOWN_TEMPLATE", 270, nodes, setupItemOutput);
            };

            createInfoList.Text = "Info: Number";
        }

        public override void WidgetAddOutConConstructor(OutConnection connection, NodeWidget widget)
        {
            base.WidgetAddOutConConstructor(connection, widget);

            if (connection.ConnectionTyp == ConnectionType.Integer)
            {
                var wid = new TextFieldWidget();
                wid.OnTextEdited = () =>
                {
                    int num;
                    int.TryParse(wid.Text, out num);
                    connection.Number = num;
                };
                widget.AddChild(wid);
                parralelWidgetList.Add(wid);

                if (connection.Number != null)
                    wid.Text = connection.Number.ToString();
            }
            else if (connection.ConnectionTyp == ConnectionType.String)
            {
                var wid = new TextFieldWidget();
                wid.OnTextEdited = () => { connection.String = wid.Text; };
                widget.AddChild(wid);
                parralelWidgetList.Add(wid);

                if (connection.String != null)
                    wid.Text = connection.String;
            }
            else if (connection.ConnectionTyp == ConnectionType.Location)
            {
                var wid = new ButtonWidget(widget.Screen.NodeScriptContainerWidget.ModData);
                wid.OnClick = () =>
                {
                    widget.Editor.SetBrush(new EditorNodeBrushBrush(CellPicking.Single, connection, widget.Editor,
                        widget.Screen.NodeScriptContainerWidget.WorldRenderer,
                        () => { wid.Text = "Cell: " + connection.Location; }));
                };
                widget.AddChild(wid);
                parralelWidgetList.Add(wid);

                if (connection.Location != null)
                    wid.Text = "Cell: " + connection.Location;
            }
            else if (connection.ConnectionTyp == ConnectionType.CellPath)
            {
                var wid = new ButtonWidget(widget.Screen.NodeScriptContainerWidget.ModData);
                wid.OnClick = () =>
                {
                    widget.Editor.SetBrush(new EditorNodeBrushBrush(CellPicking.Path, connection, widget.Editor,
                        widget.Screen.NodeScriptContainerWidget.WorldRenderer,
                        () => { wid.Text = "Path: " + connection.CellArray.Count; }));
                };
                widget.AddChild(wid);
                parralelWidgetList.Add(wid);

                if (connection.CellArray.Any())
                    wid.Text = "Path: " + connection.CellArray.Count;
            }
            else if (connection.ConnectionTyp == ConnectionType.CellArray)
            {
                var wid = new ButtonWidget(widget.Screen.NodeScriptContainerWidget.ModData);
                wid.OnClick = () =>
                {
                    widget.Editor.SetBrush(new EditorNodeBrushBrush(CellPicking.Array, connection, widget.Editor,
                        widget.Screen.NodeScriptContainerWidget.WorldRenderer,
                        () => { wid.Text = "Array: " + connection.CellArray.Count + " Cells"; }));
                };
                widget.AddChild(wid);
                parralelWidgetList.Add(wid);

                if (connection.CellArray.Any())
                    wid.Text = "Array: " + connection.CellArray.Count + " Cells";
            }
            else if (connection.ConnectionTyp == ConnectionType.Player)
            {
                var editorLayer = widget.Screen.NodeScriptContainerWidget.World.WorldActor.Trait<EditorActorLayer>();
                var selectedOwner = editorLayer.Players.Players.Values.First();
                var playerSelection = new DropDownButtonWidget(widget.Screen.NodeScriptContainerWidget.ModData);

                Func<PlayerReference, ScrollItemWidget, ScrollItemWidget> setupItem = (option, template) =>
                {
                    var item = ScrollItemWidget.Setup(template, () => selectedOwner == option, () =>
                    {
                        selectedOwner = option;

                        playerSelection.Text = selectedOwner.Name;
                        playerSelection.TextColor = selectedOwner.Color.RGB;
                        connection.Player = selectedOwner;
                    });

                    item.Get<LabelWidget>("LABEL").GetText = () => option.Name;
                    item.GetColor = () => option.Color.RGB;

                    return item;
                };

                playerSelection.OnClick = () =>
                {
                    var owners = editorLayer.Players.Players.Values.OrderBy(p => p.Name);
                    playerSelection.ShowDropDown("LABEL_DROPDOWN_TEMPLATE", 270, owners, setupItem);
                };

                playerSelection.Text = selectedOwner.Name;
                playerSelection.TextColor = selectedOwner.Color.RGB;

                widget.AddChild(playerSelection);
                parralelWidgetList.Add(playerSelection);

                if (connection.Player != null)
                {
                    playerSelection.Text = connection.Player.Name;
                    playerSelection.TextColor = connection.Player.Color.RGB;
                    selectedOwner = widget.Screen.World.WorldActor.Trait<EditorActorLayer>().Players.Players.Values
                        .First(p => p.Name == connection.Player.Name);
                }
            }
            else if (connection.ConnectionTyp == ConnectionType.Enabled)
            {
                var wid = new LabelWidget();
                wid.Text = "True";

                widget.AddChild(wid);
                parralelWidgetList.Add(wid);
            }
            else
            {
                parralelWidgetList.Add(null);
            }

            var button = new ButtonWidget(widget.Screen.NodeScriptContainerWidget.ModData);
            button.Text = "-";

            widget.AddChild(button);
            parralelButtons.Add(button);

            button.OnClick = () =>
            {
                for (var j = 0; j < parralelButtons.Count; j++)
                    if (parralelButtons[j] == button)
                    {
                        widget.RemoveChild(parralelButtons[j]);
                        widget.RemoveChild(parralelWidgetList[j]);

                        parralelButtons.RemoveAt(j);
                        parralelWidgetList.RemoveAt(j);
                        widget.OutConnections.RemoveAt(j);

                        break;
                    }
            };
        }

        public override void WidgetTick(NodeWidget widget)
        {
            for (var i = 0; i < parralelWidgetList.Count; i++)
            {
                var splitHeight = widget.RenderBounds.Height / (parralelWidgetList.Count + 1);
                if (parralelWidgetList[i] != null)
                    parralelWidgetList[i].Bounds = new Rectangle(widget.FreeWidgetEntries.X + 40,
                        widget.FreeWidgetEntries.Y + splitHeight * (i + 1), 150, 25);

                parralelButtons[i].Bounds = new Rectangle(widget.FreeWidgetEntries.X + 5,
                    widget.FreeWidgetEntries.Y + splitHeight * (i + 1), 30, 25);

                if (parralelWidgetList[i] != null && parralelWidgetList[i].HasKeyboardFocus &&
                    parralelWidgetList[i] is TextFieldWidget)
                {
                    var textField = parralelWidgetList[i] as TextFieldWidget;

                    if (textField.Text.Length * 9 > 150)
                        parralelWidgetList[i].Bounds = new Rectangle(widget.FreeWidgetEntries.X + 40,
                            widget.FreeWidgetEntries.Y + splitHeight * (i + 1), textField.Text.Length * 9, 25);
                }
            }
        }
    }
}