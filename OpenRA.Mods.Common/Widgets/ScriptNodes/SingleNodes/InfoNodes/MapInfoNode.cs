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
            //  Output Nodes
            List<ConnectionType> typeType = new List<ConnectionType>
            {
                ConnectionType.Integer,
                ConnectionType.ActorInfo,
                ConnectionType.Location,
                ConnectionType.CellPath,
                ConnectionType.CellArray,
                ConnectionType.LocationRange,
                ConnectionType.Player,
                ConnectionType.String,
                ConnectionType.Boolean
            };

            List<string> typString = new List<string>
            {
                "Info: Number",
                "Info: Actor Info",
                "Info: Cell",
                "Info: Path",
                "Info: Footprint",
                "Info: Location Range",
                "Info: Player",
                "Info: string",
                "Info: Enabled"
            };

            nodeType = typeType.First();

            AddChild(addButton = new ButtonWidget(screen.Snw.ModData));
            addButton.Bounds = new Rectangle(FreeWidgetEntries.X + WidgetEntries.Width - 10 - 25, FreeWidgetEntries.Y + 21, 25, 20);
            addButton.Text = "+";

            addButton.OnClick = () =>
            {
                var connection = new OutConnection(nodeType, this);
                AddOutConnection(connection);
                AddOutConConstructor(connection);
            };

            AddChild(createInfoList = new DropDownButtonWidget(screen.Snw.ModData));
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
                var wid = new ButtonWidget(Screen.Snw.ModData);
                wid.OnClick = () =>
                {
                    Editor.SetBrush(new EditorCellPickerBrush(CellPicking.Single, connection, Editor, Screen.Snw.WorldRenderer,
                        () => { wid.Text = "Cell: " + connection.Location.ToString(); }));
                };
                AddChild(wid);
                parralelWidgetList.Add(wid);

                if (connection.Location != null)
                    wid.Text = "Cell: " + connection.Location.ToString();
            }
            else if (connection.ConTyp == ConnectionType.CellPath)
            {
                var wid = new ButtonWidget(Screen.Snw.ModData);
                wid.OnClick = () =>
                {
                    Editor.SetBrush(new EditorCellPickerBrush(CellPicking.Path, connection, Editor, Screen.Snw.WorldRenderer,
                        () => { wid.Text = "Path: " + connection.CellArray.Count().ToString(); }));
                };
                AddChild(wid);
                parralelWidgetList.Add(wid);

                if (connection.CellArray.Any())
                    wid.Text = "Path: " + connection.CellArray.Count().ToString();
            }
            else if (connection.ConTyp == ConnectionType.CellArray)
            {
                var wid = new ButtonWidget(Screen.Snw.ModData);
                wid.OnClick = () =>
                {
                    Editor.SetBrush(new EditorCellPickerBrush(CellPicking.Array, connection, Editor, Screen.Snw.WorldRenderer,
                        () => { wid.Text = "Array: " + connection.CellArray.First().ToString(); }));
                };
                AddChild(wid);
                parralelWidgetList.Add(wid);

                if (connection.CellArray.Any())
                    wid.Text = "Array: " + connection.CellArray.Count().ToString();
            }
            else if (connection.ConTyp == ConnectionType.LocationRange)
            {
                var wid = new ButtonWidget(Screen.Snw.ModData);
                wid.OnClick = () =>
                {
                    Editor.SetBrush(new EditorCellPickerBrush(CellPicking.Range, connection, Editor, Screen.Snw.WorldRenderer,
                        () => { wid.Text = "Cell: " + connection.Location.ToString() + " | " + connection.Number; }));
                };
                AddChild(wid);
                parralelWidgetList.Add(wid);

                if (connection.Location != null && connection.Number != null)
                    wid.Text = "Cell: " + connection.Location.ToString() + " | " + connection.Number;
            }
            else if (connection.ConTyp == ConnectionType.Player)
            {
                var editorLayer = Screen.Snw.World.WorldActor.Trait<EditorActorLayer>();
                var selectedOwner = editorLayer.Players.Players.Values.First();
                var playerSelection = new DropDownButtonWidget(Screen.Snw.ModData);

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
                }
            }
            else if (connection.ConTyp == ConnectionType.ActorInfo)
            {
                var ruleActors = Screen.Snw.World.Map.Rules.Actors;
                var selectedOwner = ruleActors.First().Value;
                var playerSelection = new DropDownButtonWidget(Screen.Snw.ModData);
                var coninf = connection.ActorInfo ?? null;

                Func<ActorInfo, ScrollItemWidget, ScrollItemWidget> setupItem = (option, template) =>
                {
                    var item = ScrollItemWidget.Setup(template, () => selectedOwner == option, () =>
                    {
                        selectedOwner = option;

                        playerSelection.Text = selectedOwner.TraitInfo<TooltipInfo>().Name;
                        playerSelection.TextColor = Color.White;

                        connection.ActorInfo = selectedOwner;
                    });

                    item.Get<LabelWidget>("LABEL").GetText = () => option.TraitInfo<TooltipInfo>().Name;


                    return item;
                };

                playerSelection.OnClick = () =>
                {
                    var actors = ruleActors.Values.Where(a => a.TraitInfoOrDefault<TooltipInfo>() != null);
                    actors = actors.OrderBy(a => a.TraitInfo<TooltipInfo>().Name);
                    playerSelection.ShowDropDown("LABEL_DROPDOWN_TEMPLATE", 270, actors, setupItem);
                };

                playerSelection.Text = selectedOwner.TraitInfo<TooltipInfo>().Name;
                playerSelection.Bounds = new Rectangle(FreeWidgetEntries.X, FreeWidgetEntries.Y + 25, FreeWidgetEntries.Width, 25);

                AddChild(playerSelection);
                parralelWidgetList.Add(playerSelection);

                if (coninf != null)
                {
                    selectedOwner = coninf;
                    playerSelection.Text = coninf.TraitInfo<TooltipInfo>().Name;
                }
                else
                {
                    connection.ActorInfo = selectedOwner;
                }
            }
            else if (connection.ConTyp == ConnectionType.Boolean)
            {
                LabelWidget wid = new LabelWidget();
                wid.Text = "True";

                AddChild(wid);
                parralelWidgetList.Add(wid);
            }
            else
                parralelWidgetList.Add(null);

            var button = new ButtonWidget(Screen.Snw.ModData);
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

                for (int i = 0; i < parralelWidgetList.Count; i++)
                {
                    var splitHeight = (RenderBounds.Height + 20) / (parralelWidgetList.Count + 1);
                    if (parralelWidgetList[i] != null)
                        parralelWidgetList[i].Bounds = new Rectangle(FreeWidgetEntries.X + 40, FreeWidgetEntries.Y + splitHeight * (i + 1), 150, 25);

                    parralelButtons[i].Bounds = new Rectangle(FreeWidgetEntries.X + 5, FreeWidgetEntries.Y + splitHeight * (i + 1), 30, 25);
                }
            };

            for (int i = 0; i < parralelWidgetList.Count; i++)
            {
                var splitHeight = (RenderBounds.Height + 20) / (parralelWidgetList.Count + 1);
                if (parralelWidgetList[i] != null)
                    parralelWidgetList[i].Bounds = new Rectangle(FreeWidgetEntries.X + 40, FreeWidgetEntries.Y + splitHeight * (i + 1), 150, 25);

                parralelButtons[i].Bounds = new Rectangle(FreeWidgetEntries.X + 5, FreeWidgetEntries.Y + splitHeight * (i + 1), 30, 25);
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