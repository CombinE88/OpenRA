using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenRA.Mods.Common.Traits;
using OpenRA.Mods.Common.Widgets.ScriptNodes.Library;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.Arithmetics
{
    public class ArithmecCompareNode : NodeWidget
    {
        
        public new static Dictionary<string, BuildNodeConstructorInfo> NodeConstructorInformation =
            new Dictionary<string, BuildNodeConstructorInfo>()
            {
                {
                    "CompareActors", new BuildNodeConstructorInfo
                    {
                        LogicClass = typeof(ArithmecCompareLogic),

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Universal, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Universal, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Universal, "")
                        }
                    }
                }
            };
        
        readonly DropDownButtonWidget itemSelection;
        readonly DropDownButtonWidget methodeSelection;
        CompareItem selectedItem;
        CompareMethod selectedMethod;

        public ArithmecCompareNode(NodeEditorNodeScreenWidget screen, NodeInfo nodeInfo) : base(screen, nodeInfo)
        {
            Method = CompareMethod.Max;
            Item = CompareItem.Health;

            var methodes = new List<CompareMethod>
            {
                CompareMethod.Max,
                CompareMethod.Min
            };

            selectedMethod = Method.Value;
            methodeSelection = new DropDownButtonWidget(Screen.NodeScriptContainerWidget.ModData);

            Func<CompareMethod, ScrollItemWidget, ScrollItemWidget> setupItem2 = (option, template) =>
            {
                var item = ScrollItemWidget.Setup(template, () => selectedMethod == option, () =>
                {
                    selectedMethod = option;

                    methodeSelection.Text = selectedMethod.ToString();
                    Method = selectedMethod;
                });

                item.Get<LabelWidget>("LABEL").GetText = () => option.ToString();

                return item;
            };

            methodeSelection.OnClick = () =>
            {
                methodeSelection.ShowDropDown("LABEL_DROPDOWN_TEMPLATE", 270, methodes, setupItem2);
            };

            methodeSelection.Text = selectedMethod.ToString();

            AddChild(methodeSelection);

            var items = new List<CompareItem>
            {
                CompareItem.Health,
                CompareItem.Damage,
                CompareItem.Speed,
                CompareItem.LocationX,
                CompareItem.LocationY
            };

            selectedItem = Item.Value;
            itemSelection = new DropDownButtonWidget(Screen.NodeScriptContainerWidget.ModData);

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

            itemSelection.OnClick = () =>
            {
                itemSelection.ShowDropDown("LABEL_DROPDOWN_TEMPLATE", 270, items, setupItem);
            };

            itemSelection.Text = selectedItem.ToString();

            AddChild(itemSelection);

            methodeSelection.Bounds =
                new Rectangle(FreeWidgetEntries.X, FreeWidgetEntries.Y + 77, FreeWidgetEntries.Width, 25);
            itemSelection.Bounds =
                new Rectangle(FreeWidgetEntries.X, FreeWidgetEntries.Y + 100, FreeWidgetEntries.Width, 25);
        }

        public override void AddOutConConstructor(OutConnection connection)
        {
            base.AddOutConConstructor(connection);

            if (NodeInfo.Method != null)
            {
                selectedMethod = NodeInfo.Method.Value;
                methodeSelection.Text = NodeInfo.Method.Value.ToString();
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
        public ArithmecCompareLogic(NodeInfo nodeInfo, IngameNodeScriptSystem ingameNodeScriptSystem) : base(nodeInfo,
            ingameNodeScriptSystem)
        {
        }

        public override void Tick(Actor self)
        {
            var inc = new List<InConnection>
            {
                InConnections.First(c => c.ConnectionTyp == ConnectionType.Actor),
                InConnections.Last(c => c.ConnectionTyp == ConnectionType.Actor)
            };

            if (inc[0].In == null || inc[1].In == null)
                return;

            var ints = new List<int>
            {
                0,
                0
            };

            if (Item == CompareItem.Damage)
            {
                if (inc[0].In.ConnectionTyp == ConnectionType.Actor)
                    if (inc[0].In.Actor != null && inc[0].In.Actor.Trait<Health>() != null)
                        ints[0] = inc[0].In.Actor.Trait<Health>().MaxHP - inc[0].In.Actor.Trait<Health>().HP;

                if (inc[1].In.ConnectionTyp == ConnectionType.Actor)
                    if (inc[1].In.Actor != null && inc[1].In.Actor.Trait<Health>() != null)
                        ints[1] = inc[1].In.Actor.Trait<Health>().MaxHP - inc[1].In.Actor.Trait<Health>().HP;
            }

            if (Item == CompareItem.Health)
            {
                if (inc[0].In.ConnectionTyp == ConnectionType.Actor)
                    if (inc[0].In.Actor != null && inc[0].In.Actor.Trait<Health>() != null)
                        ints[0] = inc[0].In.Actor.Trait<Health>().MaxHP;

                if (inc[1].In.ConnectionTyp == ConnectionType.Actor)
                    if (inc[1].In.Actor != null && inc[1].In.Actor.Trait<Health>() != null)
                        ints[1] = inc[1].In.Actor.Trait<Health>().MaxHP;
            }

            if (Item == CompareItem.Speed)
            {
                if (inc[0].In.ConnectionTyp == ConnectionType.Actor)
                    if (inc[0].In.Actor != null && inc[0].In.Actor.Info.TraitInfo<MobileInfo>() != null)
                        ints[0] = inc[0].In.Actor.Info.TraitInfo<MobileInfo>().Speed;

                if (inc[1].In.ConnectionTyp == ConnectionType.Actor)
                    if (inc[1].In.Actor != null && inc[1].In.Actor.Info.TraitInfo<MobileInfo>() != null)
                        ints[1] = inc[1].In.Actor.Info.TraitInfo<MobileInfo>().Speed;
            }

            if (Item == CompareItem.LocationX)
            {
                if (inc[0].In.ConnectionTyp == ConnectionType.Actor)
                    if (inc[0].In.Actor != null)
                        ints[0] = inc[0].In.Actor.Location.X;

                if (inc[1].In.ConnectionTyp == ConnectionType.Actor)
                    if (inc[1].In.Actor != null)
                        ints[1] = inc[1].In.Actor.Location.X;
            }

            if (Item == CompareItem.LocationY)
            {
                if (inc[0].In.ConnectionTyp == ConnectionType.Actor)
                    if (inc[0].In.Actor != null)
                        ints[0] = inc[0].In.Actor.Location.Y;

                if (inc[1].In.ConnectionTyp == ConnectionType.Actor)
                    if (inc[1].In.Actor != null)
                        ints[1] = inc[1].In.Actor.Location.Y;
            }

            if (Methode == CompareMethod.Max)
                OutConnections.First(c => c.ConnectionTyp == ConnectionType.Universal).Actor =
                    inc[ints.IndexOf(Math.Max(ints[0], ints[1]))].In.Actor;
        }
    }
}