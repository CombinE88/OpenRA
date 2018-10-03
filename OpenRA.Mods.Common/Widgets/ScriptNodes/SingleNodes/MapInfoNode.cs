using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenRA.Mods.Common.Traits;
using OpenRA.Mods.Common.Widgets.ScriptNodes.EditorNodeBrushes;
using OpenRA.Widgets;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes
{
    public class MapInfoNode : NodeWidget
    {
        ConnectionType nodeType;
        DropDownButtonWidget createInfoList;
        ButtonWidget addButton;
        List<Widget> parralelWidgetList = new List<Widget>();

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
                ConnectionType.String
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
                "Info: string"
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

            if (nodeType == ConnectionType.Integer)
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
            }
            else if (nodeType == ConnectionType.String)
            {
                var wid = new TextFieldWidget();
                wid.OnTextEdited = () => { connection.String = wid.Text; };
                AddChild(wid);
                parralelWidgetList.Add(wid);
            }
            else if (nodeType == ConnectionType.Location)
            {
                var wid = new ButtonWidget(Screen.Snw.ModData);
                wid.OnClick = () =>
                {
                    Editor.SetBrush(new EditorCellPickerBrush(CellPicking.Single, connection, Editor, Screen.Snw.WorldRenderer,
                        () => { wid.Text = "Cell: " + connection.Location.ToString(); }));
                };
                AddChild(wid);
                parralelWidgetList.Add(wid);
            }
            else if (nodeType == ConnectionType.CellPath)
            {
                var wid = new ButtonWidget(Screen.Snw.ModData);
                wid.OnClick = () =>
                {
                    Editor.SetBrush(new EditorCellPickerBrush(CellPicking.Path, connection, Editor, Screen.Snw.WorldRenderer,
                        () => { wid.Text = "Path: " + connection.CellArray.First().ToString(); }));
                };
                AddChild(wid);
                parralelWidgetList.Add(wid);
            }
            else if (nodeType == ConnectionType.CellArray)
            {
                var wid = new ButtonWidget(Screen.Snw.ModData);
                wid.OnClick = () =>
                {
                    Editor.SetBrush(new EditorCellPickerBrush(CellPicking.Array, connection, Editor, Screen.Snw.WorldRenderer,
                        () => { wid.Text = "Array: " + connection.CellArray.First().ToString(); }));
                };
                AddChild(wid);
                parralelWidgetList.Add(wid);
            }
            else if (nodeType == ConnectionType.LocationRange)
            {
                var wid = new ButtonWidget(Screen.Snw.ModData);
                wid.OnClick = () =>
                {
                    Editor.SetBrush(new EditorCellPickerBrush(CellPicking.Range, connection, Editor, Screen.Snw.WorldRenderer,
                        () => { wid.Text = "Cell: " + connection.Location.ToString() + " | " + connection.Number; }));
                };
                AddChild(wid);
                parralelWidgetList.Add(wid);
            }
            else if (nodeType == ConnectionType.Player)
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
            }
            else if (nodeType == ConnectionType.ActorInfo)
            {
                var ruleActors = Screen.Snw.World.Map.Rules.Actors;
                var selectedOwner = ruleActors.First().Value;
                var playerSelection = new DropDownButtonWidget(Screen.Snw.ModData);

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
                connection.ActorInfo = selectedOwner;

                AddChild(playerSelection);
                parralelWidgetList.Add(playerSelection);
            }
            else
                parralelWidgetList.Add(null);

            for (int i = 0; i < OutConnections.Count; i++)
            {
                var splitHeight = (RenderBounds.Height + 20) / (OutConnections.Count + 1);
                if (parralelWidgetList[i] != null)
                    parralelWidgetList[i].Bounds = new Rectangle(FreeWidgetEntries.X + 20, FreeWidgetEntries.Y + splitHeight * (i + 1), 170, 25);
            }
        }
    }
}