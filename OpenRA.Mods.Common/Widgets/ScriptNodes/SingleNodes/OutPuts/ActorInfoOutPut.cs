using System;
using System.Drawing;
using System.Linq;
using OpenRA.Graphics;
using OpenRA.Mods.Common.Traits;
using OpenRA.Mods.Common.Traits.Render;
using OpenRA.Widgets;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.OutPuts
{
    public class ActorInfoOutPutWidget : SimpleNodeWidget
    {
        private DropDownButtonWidget playerSelection;
        ActorInfo selectedOwner;
        OutConnection outconnection;
        IReadOnlyDictionary<string, ActorInfo> ruleActors;
        TextFieldWidget textfield;
        ScrollItemWidget actorTemplate;
        World world;
        ActorInfo actor;
        WorldRenderer worldRenderer;

        public ActorInfoOutPutWidget(NodeEditorNodeScreenWidget screen) : base(screen)
        {
            WidgetName = "Output: ActorInfo";
            world = screen.Snw.World;
            worldRenderer = screen.Snw.WorldRenderer;

            AddChild(playerSelection = new DropDownButtonWidget(screen.Snw.ModData));
            AddChild(textfield = new TextFieldWidget());
            AddChild(actorTemplate = new ScrollItemWidget(screen.Snw.ModData));

            var inRecangle = new Rectangle(0, 0, 0, 0);
            outconnection = new OutConnection(ConnectionType.ActorInfo, this);
            OutConnections.Add(outconnection);
            outconnection.InWidgetPosition = inRecangle;

            ruleActors = screen.Snw.World.Map.Rules.Actors;
            selectedOwner = ruleActors.First().Value;

            Func<ActorInfo, ScrollItemWidget, ScrollItemWidget> setupItem = (option, template) =>
            {
                var item = ScrollItemWidget.Setup(template, () => selectedOwner == option, () =>
                {
                    selectedOwner = option;

                    playerSelection.Text = selectedOwner.TraitInfo<TooltipInfo>().Name;
                    playerSelection.TextColor = Color.White;
                });

                item.Get<LabelWidget>("LABEL").GetText = () => option.TraitInfo<TooltipInfo>().Name;


                return item;
            };

            playerSelection.OnClick = () =>
            {
                var actors = ruleActors.Values.Where(a => a.TraitInfoOrDefault<TooltipInfo>() != null);
                if (textfield.Text != "")
                    actors = actors.Where(a => a.TraitInfo<TooltipInfo>().Name.ToLowerInvariant().Contains(textfield.Text.ToLowerInvariant()));

                actors = actors.OrderBy(a => a.TraitInfo<TooltipInfo>().Name);
                playerSelection.ShowDropDown("LABEL_DROPDOWN_TEMPLATE", 270, actors, setupItem);

                actor = ruleActors.Values.FirstOrDefault(a => a.Name == selectedOwner.Name);
            };
            playerSelection.Text = selectedOwner.TraitInfo<TooltipInfo>().Name;

            playerSelection.Bounds = new Rectangle(FreeWidgetEntries.X, FreeWidgetEntries.Y + 25, FreeWidgetEntries.Width, 25);
            textfield.Bounds = new Rectangle(FreeWidgetEntries.X, FreeWidgetEntries.Y, FreeWidgetEntries.Width, 25);
        }

        public override void Tick()
        {
            outconnection.ActorInfo = selectedOwner;
            playerSelection.Bounds = new Rectangle(FreeWidgetEntries.X, FreeWidgetEntries.Y + 35, FreeWidgetEntries.Width, 25);
            textfield.Bounds = new Rectangle(FreeWidgetEntries.X, FreeWidgetEntries.Y, FreeWidgetEntries.Width, 25);
            base.Tick();
        }

        public override void Draw()
        {
            base.Draw();

            if (actor != null && actor.TraitInfoOrDefault<RenderSpritesInfo>() != null && actor.TraitInfoOrDefault<RenderSpritesInfo>().Image != null)
            {
                var palette = worldRenderer.Palette("terrain");
                var animation = new Animation(world, actor.TraitInfoOrDefault<RenderSpritesInfo>().Image);

                animation.PlayFetchIndex("idle", () => 0);
                WidgetUtils.DrawSHPCentered(animation.Image, new float2(RenderBounds.X + FreeWidgetEntries.X, RenderBounds.Y + FreeWidgetEntries.Y + 50), palette);
            }
        }
    }
}