using System;
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
        readonly TextFieldWidget textField;

        DropDownButtonWidget playerSelection;
        ActorInfo selectedActorInfo;

        ActorPreviewWidget preview;

        public MapInfoActorInfoNode(NodeEditorNodeScreenWidget screen, NodeInfo nodeInfo) : base(screen, nodeInfo)
        {
            textField = new TextFieldWidget();
            AddChild(textField);

            var ruleActors = Screen.Snw.World.Map.Rules.Actors.Values
                .Where(a =>
                {
                    if (a.Name.Contains('^'))
                        return false;

                    if (!a.HasTraitInfo<IRenderActorPreviewInfo>())
                        return false;

                    var editorData = a.TraitInfoOrDefault<MapEditorDataInfo>();

                    if (editorData == null || editorData.Categories == null)
                        return false;

                    if (!a.TraitInfos<TooltipInfo>().Any())
                        return false;

                    var tooltip = a.TraitInfos<TooltipInfo>().First();

                    if (string.IsNullOrEmpty(tooltip.Name))
                        return false;

                    return true;
                })
                .OrderBy(a => a.TraitInfos<TooltipInfo>().First().Name);

            var choosenActors = ruleActors.ToList();

            selectedActorInfo = ruleActors.First();

            AddChild(playerSelection = new DropDownButtonWidget(Screen.Snw.ModData));
            AddChild(preview = new ActorPreviewWidget(screen.WorldRenderer));

            preview.Bounds = new Rectangle(FreeWidgetEntries.X, FreeWidgetEntries.Y + 77, FreeWidgetEntries.Width, 90);

            Func<ActorInfo, ScrollItemWidget, ScrollItemWidget> setupItem = (option, template) =>
            {
                var item = ScrollItemWidget.Setup(template, () => selectedActorInfo == option, () =>
                {
                    selectedActorInfo = option;

                    playerSelection.Text = selectedActorInfo.TraitInfos<TooltipInfo>().First().Name + "(" + selectedActorInfo.Name + ")";
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

                item.Get<LabelWidget>("LABEL").GetText = () => option.TraitInfos<TooltipInfo>().First().Name;
                return item;
            };

            playerSelection.OnClick = () => { playerSelection.ShowDropDown("LABEL_DROPDOWN_TEMPLATE", 270, choosenActors, setupItem); };

            textField.OnTextEdited = () =>
            {
                if (textField.Text != "")
                    choosenActors = ruleActors.Where(a =>
                        a.TraitInfos<TooltipInfo>().First().Name.ToLowerInvariant().Contains(textField.Text.ToLowerInvariant()) ||
                        a.Name.ToLowerInvariant().Contains(textField.Text.ToLowerInvariant())).ToList();
                else
                    choosenActors = ruleActors.ToList();
            };

            playerSelection.Text = selectedActorInfo.TraitInfos<TooltipInfo>().First().Name + "(" + selectedActorInfo.Name + ")";
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
                playerSelection.Text = connection.ActorInfo.TraitInfos<TooltipInfo>().First().Name + "(" + selectedActorInfo.Name + ")";
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