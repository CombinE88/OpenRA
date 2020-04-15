using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenRA.Mods.Common.Traits;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.Arithmetics
{
    public class ArithmecCompareNode : NodeWidget
    {
        CompareMethode selectedMethode;
        DropDownButtonWidget methodeSelection;
        CompareItem selectedItem;
        DropDownButtonWidget itemSelection;

        public ArithmecCompareNode(NodeEditorNodeScreenWidget screen, NodeInfo nodeInfo) : base(screen, nodeInfo)
        {
            Methode = CompareMethode.Max;
            Item = CompareItem.Health;

            List<CompareMethode> methodes = new List<CompareMethode>
            {
                CompareMethode.Max,
                CompareMethode.Min
            };

            selectedMethode = Methode.Value;
            methodeSelection = new DropDownButtonWidget(Screen.ScriptNodeWidget.ModData);

            Func<CompareMethode, ScrollItemWidget, ScrollItemWidget> setupItem2 = (option, template) =>
            {
                var item = ScrollItemWidget.Setup(template, () => selectedMethode == option, () =>
                {
                    selectedMethode = option;

                    methodeSelection.Text = selectedMethode.ToString();
                    Methode = selectedMethode;
                });

                item.Get<LabelWidget>("LABEL").GetText = () => option.ToString();

                return item;
            };

            methodeSelection.OnClick = () => { methodeSelection.ShowDropDown("LABEL_DROPDOWN_TEMPLATE", 270, methodes, setupItem2); };

            methodeSelection.Text = selectedMethode.ToString();

            AddChild(methodeSelection);

            List<CompareItem> items = new List<CompareItem>
            {
                CompareItem.Health,
                CompareItem.Damage,
                CompareItem.Speed,
                CompareItem.LocationX,
                CompareItem.LocationY
            };

            selectedItem = Item.Value;
            itemSelection = new DropDownButtonWidget(Screen.ScriptNodeWidget.ModData);

            Func<CompareItem, ScrollItemWidget, ScrollItemWidget> setupItem = (option, template) =>
            {
                var item = ScrollItemWidget.Setup(template, () => selectedItem == option, () =>
                {
                    selectedItem = option;

                    itemSelection.Text = selectedItem.ToString();
                    Item = selectedItem;
                });

                item.Get<LabelWidget>("LABEL").GetText = () => option.ToString();

                return item;
            };

            itemSelection.OnClick = () => { itemSelection.ShowDropDown("LABEL_DROPDOWN_TEMPLATE", 270, items, setupItem); };

            itemSelection.Text = selectedItem.ToString();

            AddChild(itemSelection);

            methodeSelection.Bounds = new Rectangle(FreeWidgetEntries.X, FreeWidgetEntries.Y + 77, FreeWidgetEntries.Width, 25);
            itemSelection.Bounds = new Rectangle(FreeWidgetEntries.X, FreeWidgetEntries.Y + 100, FreeWidgetEntries.Width, 25);
        }

        public override void AddOutConConstructor(OutConnection connection)
        {
            base.AddOutConConstructor(connection);

            if (NodeInfo.Methode != null)
            {
                selectedMethode = NodeInfo.Methode.Value;
                methodeSelection.Text = NodeInfo.Methode.Value.ToString();
            }

            if (NodeInfo.Item != null)
            {
                selectedItem = NodeInfo.Item.Value;
                itemSelection.Text = NodeInfo.Item.Value.ToString();
            }
        }
    }

    public class ArithmecCompareLogic : NodeLogic
    {
        public ArithmecCompareLogic(NodeInfo nodeinfo, IngameNodeScriptSystem insc) : base(nodeinfo, insc)
        {
        }

        public override void Tick(Actor self)
        {
            List<InConnection> inc = new List<InConnection>
            {
                InConnections.First(c => c.ConTyp == ConnectionType.Actor),
                InConnections.Last(c => c.ConTyp == ConnectionType.Actor)
            };

            if (inc[0].In == null || inc[1].In == null)
                return;

            List<int> ints = new List<int>
            {
                0,
                0
            };

            if (Item == CompareItem.Damage)
            {
                if (inc[0].In.ConTyp == ConnectionType.Actor)
                    if (inc[0].In.Actor != null && inc[0].In.Actor.Trait<Health>() != null)
                        ints[0] = inc[0].In.Actor.Trait<Health>().MaxHP - inc[0].In.Actor.Trait<Health>().HP;

                if (inc[1].In.ConTyp == ConnectionType.Actor)
                    if (inc[1].In.Actor != null && inc[1].In.Actor.Trait<Health>() != null)
                        ints[1] = inc[1].In.Actor.Trait<Health>().MaxHP - inc[1].In.Actor.Trait<Health>().HP;
            }

            if (Item == CompareItem.Health)
            {
                if (inc[0].In.ConTyp == ConnectionType.Actor)
                    if (inc[0].In.Actor != null && inc[0].In.Actor.Trait<Health>() != null)
                        ints[0] = inc[0].In.Actor.Trait<Health>().MaxHP;

                if (inc[1].In.ConTyp == ConnectionType.Actor)
                    if (inc[1].In.Actor != null && inc[1].In.Actor.Trait<Health>() != null)
                        ints[1] = inc[1].In.Actor.Trait<Health>().MaxHP;
            }

            if (Item == CompareItem.Speed)
            {
                if (inc[0].In.ConTyp == ConnectionType.Actor)
                    if (inc[0].In.Actor != null && inc[0].In.Actor.Info.TraitInfo<MobileInfo>() != null)
                        ints[0] = inc[0].In.Actor.Info.TraitInfo<MobileInfo>().Speed;

                if (inc[1].In.ConTyp == ConnectionType.Actor)
                    if (inc[1].In.Actor != null && inc[1].In.Actor.Info.TraitInfo<MobileInfo>() != null)
                        ints[1] = inc[1].In.Actor.Info.TraitInfo<MobileInfo>().Speed;
            }

            if (Item == CompareItem.LocationX)
            {
                if (inc[0].In.ConTyp == ConnectionType.Actor)
                    if (inc[0].In.Actor != null)
                        ints[0] = inc[0].In.Actor.Location.X;

                if (inc[1].In.ConTyp == ConnectionType.Actor)
                    if (inc[1].In.Actor != null)
                        ints[1] = inc[1].In.Actor.Location.X;
            }

            if (Item == CompareItem.LocationY)
            {
                if (inc[0].In.ConTyp == ConnectionType.Actor)
                    if (inc[0].In.Actor != null)
                        ints[0] = inc[0].In.Actor.Location.Y;

                if (inc[1].In.ConTyp == ConnectionType.Actor)
                    if (inc[1].In.Actor != null)
                        ints[1] = inc[1].In.Actor.Location.Y;
            }

            if (Methode == CompareMethode.Max)
            {
                OutConnections.First(c => c.ConTyp == ConnectionType.Universal).Actor = inc[ints.IndexOf(Math.Max(ints[0], ints[1]))].In.Actor;
            }
        }
    }
}