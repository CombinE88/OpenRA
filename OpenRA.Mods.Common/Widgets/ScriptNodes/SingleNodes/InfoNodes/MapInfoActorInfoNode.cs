using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenRA.Mods.Common.Traits;
using OpenRA.Mods.Common.Widgets.ScriptNodes.Library;
using OpenRA.Primitives;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.InfoNodes
{
    public class MapInfoActorInfoNode : NodeWidget
    {
        public new static Dictionary<string, BuildNodeConstructorInfo> NodeConstructorInformation =
            new Dictionary<string, BuildNodeConstructorInfo>()
            {
                {
                    "MapInfoActorInfo", new BuildNodeConstructorInfo
                    {
                        LogicClass = typeof(MapInfoLogicNode),
                        Nesting = new[] {"Info Nodes"},
                        Name = "Actor Type",

                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.ActorInfo,
                                "Output: yaml name of choosen Actortype")
                        }
                    }
                }
            };

        readonly TextFieldWidget textField;

        readonly DropDownButtonWidget playerSelection;

        readonly ActorPreviewWidget preview;
        ActorInfo selectedActorInfo;

        public MapInfoActorInfoNode(NodeEditorNodeScreenWidget screen, NodeInfo nodeInfo) : base(screen, nodeInfo)
        {
            textField = new TextFieldWidget();
            AddChild(textField);

            var ruleActors = Screen.NodeScriptContainerWidget.World.Map.Rules.Actors.Values
                .Where(a =>
                {
                    if (a.Name.Contains('^'))
                        return false;

                    if (!a.HasTraitInfo<IRenderActorPreviewInfo>())
                        return false;

                    var editorData = a.TraitInfoOrDefault<EditorTilesetFilterInfo>();

                    if (editorData == null || editorData.Categories == null)
                        return false;

                    if (a.TraitInfoOrDefault<TooltipInfo>() == null)
                        return false;

                    return true;
                })
                .OrderBy(a => a.TraitInfo<TooltipInfo>().Name);

            var choosenActors = ruleActors.ToList();

            selectedActorInfo = ruleActors.First();

            AddChild(playerSelection = new DropDownButtonWidget(Screen.NodeScriptContainerWidget.ModData));
            AddChild(preview = new ActorPreviewWidget(screen.WorldRenderer));

            preview.Bounds = new Rectangle(FreeWidgetEntries.X, FreeWidgetEntries.Y + 77, FreeWidgetEntries.Width, 90);

            Func<ActorInfo, ScrollItemWidget, ScrollItemWidget> setupItem = (option, template) =>
            {
                var item = ScrollItemWidget.Setup(template, () => selectedActorInfo == option, () =>
                {
                    selectedActorInfo = option;

                    playerSelection.Text = selectedActorInfo.TraitInfo<TooltipInfo>().Name + "(" +
                                           selectedActorInfo.Name + ")";
                    playerSelection.TextColor = Color.White;

                    OutConnections.First().ActorInfo = selectedActorInfo;

                    var td2 = new TypeDictionary();
                    td2.Add(new OwnerInit("Neutral"));
                    td2.Add(new FactionInit(screen.World.WorldActor.Trait<EditorActorLayer>().Players.Players.Values
                        .First().Faction));
                    foreach (var api in selectedActorInfo.TraitInfos<IActorPreviewInitInfo>())
                    foreach (var o in api.ActorPreviewInits(selectedActorInfo, ActorPreviewType.MapEditorSidebar))
                        td2.Add(o);
                    preview.SetPreview(selectedActorInfo, td2);
                });

                item.Get<LabelWidget>("LABEL").GetText = () => option.TraitInfo<TooltipInfo>().Name;
                return item;
            };

            playerSelection.OnClick = () =>
            {
                playerSelection.ShowDropDown("LABEL_DROPDOWN_TEMPLATE", 270, choosenActors, setupItem);
            };

            textField.OnTextEdited = () =>
            {
                if (textField.Text != "")
                    choosenActors = ruleActors.Where(a =>
                        a.TraitInfo<TooltipInfo>().Name.ToLowerInvariant()
                            .Contains(textField.Text.ToLowerInvariant()) ||
                        a.Name.ToLowerInvariant().Contains(textField.Text.ToLowerInvariant())).ToList();
                else
                    choosenActors = ruleActors.ToList();
            };

            playerSelection.Text = selectedActorInfo.TraitInfo<TooltipInfo>().Name + "(" + selectedActorInfo.Name + ")";
            playerSelection.Bounds =
                new Rectangle(FreeWidgetEntries.X, FreeWidgetEntries.Y + 51, FreeWidgetEntries.Width, 25);
            textField.Bounds =
                new Rectangle(FreeWidgetEntries.X, FreeWidgetEntries.Y + 25, FreeWidgetEntries.Width, 25);

            var td = new TypeDictionary();
            td.Add(new OwnerInit("Neutral"));
            td.Add(new FactionInit(screen.World.WorldActor.Trait<EditorActorLayer>().Players.Players.Values.First()
                .Faction));
            foreach (var api in selectedActorInfo.TraitInfos<IActorPreviewInitInfo>())
            foreach (var o in api.ActorPreviewInits(selectedActorInfo, ActorPreviewType.MapEditorSidebar))
                td.Add(o);
            preview.SetPreview(selectedActorInfo, td);
        }

        public override void AddOutConConstructor(OutConnection connection)
        {
            base.AddOutConConstructor(connection);

            if (connection.ActorInfo != null)
            {
                playerSelection.Text = connection.ActorInfo.TraitInfo<TooltipInfo>().Name + "(" +
                                       selectedActorInfo.Name + ")";
                selectedActorInfo = connection.ActorInfo;

                var td = new TypeDictionary();
                td.Add(new OwnerInit("Neutral"));
                td.Add(new FactionInit(Screen.World.WorldActor.Trait<EditorActorLayer>().Players.Players.Values.First()
                    .Faction));
                foreach (var api in selectedActorInfo.TraitInfos<IActorPreviewInitInfo>())
                foreach (var o in api.ActorPreviewInits(selectedActorInfo, ActorPreviewType.MapEditorSidebar))
                    td.Add(o);
                preview.SetPreview(selectedActorInfo, td);
            }
        }
    }
}