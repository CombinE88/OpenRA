using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenRA.Mods.Common.Traits;
using OpenRA.Mods.Common.Widgets.ScriptNodes.EditorNodeBrushes;
using OpenRA.Widgets;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.InfoNodes
{
    public class MapInfoNode : NodeWidget
    {
        ConnectionType nodeType;
        DropDownButtonWidget createInfoList;
        ButtonWidget addButton;
        List<Widget> parralelWidgetList = new List<Widget>();
        List<ButtonWidget> parralelButtons = new List<ButtonWidget>();

        public MapInfoNode(NodeEditorNodeScreenWidget screen, NodeInfo nodeInfo) : base(screen, nodeInfo)
        {
            List<ConnectionType> typeType = new List<ConnectionType>
            {
                ConnectionType.Integer,
                ConnectionType.Location,
                ConnectionType.CellPath,
                ConnectionType.CellArray,
                ConnectionType.LocationRange,
                ConnectionType.Player,
                ConnectionType.String,
                ConnectionType.Repeatable
            };

            List<string> typString = new List<string>
            {
                "Info: Number",
                "Info: Cell",
                "Info: Path",
                "Info: Footprint",
                "Info: Location Range",
                "Info: Player",
                "Info: string",
                "Info: Enabled"
            };

            nodeType = typeType.First();

            AddChild(addButton = new ButtonWidget(screen.ScriptNodeWidget.ModData));
            addButton.Bounds = new Rectangle(FreeWidgetEntries.X + WidgetEntries.Width - 10 - 25, FreeWidgetEntries.Y + 21, 25, 20);
            addButton.Text = "+";

            addButton.OnClick = () =>
            {
                var connection = new OutConnection(nodeType, this);
                AddOutConnection(connection);
                AddOutConConstructor(connection);
            };

            AddChild(createInfoList = new DropDownButtonWidget(screen.ScriptNodeWidget.ModData));
            createInfoList.Bounds = new Rectangle(FreeWidgetEntries.X, FreeWidgetEntries.Y + 21, WidgetEntries.Width - 10 - 25, 20);

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

        public override void AddOutConConstructor(OutConnection connection)
        {
            base.AddOutConConstructor(connection);

            if (connection.ConTyp == ConnectionType.Integer)
            {
                var wid = new TextFieldWidget();
                wid.OnTextEdited = () =>
                {
                    int num;
                    int.TryParse(wid.Text, out num);
                    connection.Number = num;
                };
                AddChild(wid);
                parralelWidgetList.Add(wid);

                if (connection.Number != null)
                    wid.Text = connection.Number.ToString();
            }
            else if (connection.ConTyp == ConnectionType.String)
            {
                var wid = new TextFieldWidget();
                wid.OnTextEdited = () => { connection.String = wid.Text; };
                AddChild(wid);
                parralelWidgetList.Add(wid);

                if (connection.String != null)
                    wid.Text = connection.String;
            }
            else if (connection.ConTyp == ConnectionType.Location)
            {
                var wid = new ButtonWidget(Screen.ScriptNodeWidget.ModData);
                wid.OnClick = () =>
                {
                    Editor.SetBrush(new EditorNodeBrushBrush(CellPicking.Single, connection, Editor, Screen.ScriptNodeWidget.WorldRenderer,
                        () => { wid.Text = "Cell: " + connection.Location; }));
                };
                AddChild(wid);
                parralelWidgetList.Add(wid);

                if (connection.Location != null)
                    wid.Text = "Cell: " + connection.Location;
            }
            else if (connection.ConTyp == ConnectionType.CellPath)
            {
                var wid = new ButtonWidget(Screen.ScriptNodeWidget.ModData);
                wid.OnClick = () =>
                {
                    Editor.SetBrush(new EditorNodeBrushBrush(CellPicking.Path, connection, Editor, Screen.ScriptNodeWidget.WorldRenderer,
                        () => { wid.Text = "Path: " + connection.CellArray.Count; }));
                };
                AddChild(wid);
                parralelWidgetList.Add(wid);

                if (connection.CellArray.Any())
                    wid.Text = "Path: " + connection.CellArray.Count;
            }
            else if (connection.ConTyp == ConnectionType.CellArray)
            {
                var wid = new ButtonWidget(Screen.ScriptNodeWidget.ModData);
                wid.OnClick = () =>
                {
                    Editor.SetBrush(new EditorNodeBrushBrush(CellPicking.Array, connection, Editor, Screen.ScriptNodeWidget.WorldRenderer,
                        () => { wid.Text = "Array: " + connection.CellArray.Count + " Cells"; }));
                };
                AddChild(wid);
                parralelWidgetList.Add(wid);

                if (connection.CellArray.Any())
                    wid.Text = "Array: " + connection.CellArray.Count + " Cells";
            }
            else if (connection.ConTyp == ConnectionType.LocationRange)
            {
                var wid = new ButtonWidget(Screen.ScriptNodeWidget.ModData);
                wid.OnClick = () =>
                {
                    Editor.SetBrush(new EditorNodeBrushBrush(CellPicking.Range, connection, Editor, Screen.ScriptNodeWidget.WorldRenderer,
                        () => { wid.Text = "Cell: " + connection.Location + " | " + connection.Number; }));
                };
                AddChild(wid);
                parralelWidgetList.Add(wid);

                if (connection.Location != null && connection.Number != null)
                    wid.Text = "Cell: " + connection.Location + " | " + connection.Number;
            }
            else if (connection.ConTyp == ConnectionType.Player)
            {
                var editorLayer = Screen.ScriptNodeWidget.World.WorldActor.Trait<EditorActorLayer>();
                var selectedOwner = editorLayer.Players.Players.Values.First();
                var playerSelection = new DropDownButtonWidget(Screen.ScriptNodeWidget.ModData);

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

                AddChild(playerSelection);
                parralelWidgetList.Add(playerSelection);

                if (connection.Player != null)
                {
                    playerSelection.Text = connection.Player.Name;
                    playerSelection.TextColor = connection.Player.Color.RGB;
                    selectedOwner = Screen.World.WorldActor.Trait<EditorActorLayer>().Players.Players.Values.First(p => p.Name == connection.Player.Name);
                }
            }
            else if (connection.ConTyp == ConnectionType.Repeatable)
            {
                LabelWidget wid = new LabelWidget();
                wid.Text = "True";

                AddChild(wid);
                parralelWidgetList.Add(wid);
            }
            else
                parralelWidgetList.Add(null);

            var button = new ButtonWidget(Screen.ScriptNodeWidget.ModData);
            button.Text = "-";

            AddChild(button);
            parralelButtons.Add(button);

            button.OnClick = () =>
            {
                for (int j = 0; j < parralelButtons.Count; j++)
                {
                    if (parralelButtons[j] == button)
                    {
                        RemoveChild(parralelButtons[j]);
                        RemoveChild(parralelWidgetList[j]);

                        parralelButtons.RemoveAt(j);
                        parralelWidgetList.RemoveAt(j);
                        OutConnections.RemoveAt(j);

                        break;
                    }
                }
            };
        }

        public override void Tick()
        {
            base.Tick();

            for (int i = 0; i < parralelWidgetList.Count; i++)
            {
                var splitHeight = RenderBounds.Height / (parralelWidgetList.Count + 1);
                if (parralelWidgetList[i] != null)
                    parralelWidgetList[i].Bounds = new Rectangle(FreeWidgetEntries.X + 40, FreeWidgetEntries.Y + splitHeight * (i + 1), 150, 25);

                parralelButtons[i].Bounds = new Rectangle(FreeWidgetEntries.X + 5, FreeWidgetEntries.Y + splitHeight * (i + 1), 30, 25);

                if (parralelWidgetList[i] != null && parralelWidgetList[i].HasKeyboardFocus && parralelWidgetList[i] is TextFieldWidget)
                {
                    TextFieldWidget textField = parralelWidgetList[i] as TextFieldWidget;

                    if (textField.Text.Length * 9 > 150)
                        parralelWidgetList[i].Bounds = new Rectangle(FreeWidgetEntries.X + 40, FreeWidgetEntries.Y + splitHeight * (i + 1), textField.Text.Length * 9, 25);
                }
            }
        }
    }

    class MapInfoLogicNode : NodeLogic
    {
        public MapInfoLogicNode(NodeInfo nodeinfo, IngameNodeScriptSystem insc) : base(nodeinfo, insc)
        {
        }
    }
}