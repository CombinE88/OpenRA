using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenRA.Graphics;
using OpenRA.Mods.Common.Graphics;
using OpenRA.Mods.Common.Traits;
using OpenRA.Mods.Common.Traits.Render;
using OpenRA.Primitives;
using OpenRA.Scripting;
using OpenRA.Widgets;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.InfoNodes
{
    public class MapInfoActorInfoNode : NodeWidget
    {
        DropDownButtonWidget playerSelection;
        ActorInfo selectedActorInfo;

        PaletteReference palette;
        Animation animation;
        ActorPreviewWidget preview;
        TextFieldWidget textField;
        List<ActorInfo> choosenActors;

        public MapInfoActorInfoNode(NodeEditorNodeScreenWidget screen, NodeInfo nodeInfo) : base(screen, nodeInfo)
        {
            textField = new TextFieldWidget();
            AddChild(textField);

            var ruleActors = Screen.Snw.World.Map.Rules.Actors.Values
                .Where(a =>
                {
                    // Partial templates are not allowed
                    if (a.Name.Contains('^'))
                        return false;

                    // Actor must have a preview associated with it
                    if (!a.HasTraitInfo<IRenderActorPreviewInfo>())
                        return false;

                    var editorData = a.TraitInfoOrDefault<EditorTilesetFilterInfo>();

                    // Actor must be included in at least one category
                    if (editorData == null || editorData.Categories == null)
                        return false;

                    if (a.TraitInfoOrDefault<TooltipInfo>() == null)
                        return false;

                    return true;
                })
                .OrderBy(a => a.TraitInfo<TooltipInfo>().Name);

            choosenActors = ruleActors.ToList();

            selectedActorInfo = ruleActors.First();

            AddChild(playerSelection = new DropDownButtonWidget(Screen.Snw.ModData));
            AddChild(preview = new ActorPreviewWidget(screen.WorldRenderer));

            preview.Bounds = new Rectangle(FreeWidgetEntries.X, FreeWidgetEntries.Y + 77, FreeWidgetEntries.Width, 90);

            Func<ActorInfo, ScrollItemWidget, ScrollItemWidget> setupItem = (option, template) =>
            {
                var item = ScrollItemWidget.Setup(template, () => selectedActorInfo == option, () =>
                {
                    selectedActorInfo = option;


                    playerSelection.Text = selectedActorInfo.TraitInfo<TooltipInfo>().Name;
                    playerSelection.TextColor = Color.White;

                    OutConnections.First().ActorInfo = selectedActorInfo;

                    var td2 = new TypeDictionary();
                    td2.Add(new OwnerInit("Neutral"));
                    td2.Add(new FactionInit(screen.World.WorldActor.Trait<EditorActorLayer>().Players.Players.Values.First().Faction));
                    foreach (var api in selectedActorInfo.TraitInfos<IActorPreviewInitInfo>())
                    foreach (var o in api.ActorPreviewInits(selectedActorInfo, ActorPreviewType.MapEditorSidebar))
                        td2.Add(o);
                    preview.SetPreview(selectedActorInfo, td2);
                });

                item.Get<LabelWidget>("LABEL").GetText = () => option.TraitInfo<TooltipInfo>().Name;
                return item;
            };

            playerSelection.OnClick = () => { playerSelection.ShowDropDown("LABEL_DROPDOWN_TEMPLATE", 270, choosenActors, setupItem); };

            textField.OnTextEdited = () =>
            {
                if (textField.Text != "")
                    choosenActors = ruleActors.Where(a => a.TraitInfo<TooltipInfo>().Name.ToLowerInvariant().Contains(textField.Text)).ToList();
                else
                    choosenActors = ruleActors.ToList();
            };

            playerSelection.Text = selectedActorInfo.TraitInfo<TooltipInfo>().Name;
            playerSelection.Bounds = new Rectangle(FreeWidgetEntries.X, FreeWidgetEntries.Y + 51, FreeWidgetEntries.Width, 25);
            textField.Bounds = new Rectangle(FreeWidgetEntries.X, FreeWidgetEntries.Y + 25, FreeWidgetEntries.Width, 25);


            var td = new TypeDictionary();
            td.Add(new OwnerInit("Neutral"));
            td.Add(new FactionInit(screen.World.WorldActor.Trait<EditorActorLayer>().Players.Players.Values.First().Faction));
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
                playerSelection.Text = connection.ActorInfo.TraitInfo<TooltipInfo>().Name;
                selectedActorInfo = connection.ActorInfo;

                var td = new TypeDictionary();
                td.Add(new OwnerInit("Neutral"));
                td.Add(new FactionInit(Screen.World.WorldActor.Trait<EditorActorLayer>().Players.Players.Values.First().Faction));
                foreach (var api in selectedActorInfo.TraitInfos<IActorPreviewInitInfo>())
                foreach (var o in api.ActorPreviewInits(selectedActorInfo, ActorPreviewType.MapEditorSidebar))
                    td.Add(o);
                preview.SetPreview(selectedActorInfo, td);
            }
        }
    }
}