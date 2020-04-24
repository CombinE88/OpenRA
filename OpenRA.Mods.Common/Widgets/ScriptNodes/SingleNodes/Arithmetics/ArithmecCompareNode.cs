using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenRA.Mods.Common.Traits;
using OpenRA.Mods.Common.Widgets.ScriptNodes.Library;
using OpenRA.Mods.Common.Widgets.ScriptNodes.NodeInfos;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.Arithmetics
{
    public class ArithmecCompareNode : NodeWidget
    {
        // TODO: Nicht sicher ob nötig

        public new static Dictionary<string, BuildNodeConstructorInfo> NodeConstructorInformation =
            new Dictionary<string, BuildNodeConstructorInfo>()
            {
                {
                    "CompareUniversal", new BuildNodeConstructorInfo
                    {
                        LogicClass = typeof(ArithmecCompareLogic),
                        Nesting = new[] {"Arithmetic's"},
                        Name = "Compare Universal",

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
        readonly DropDownButtonWidget methodSelection;
        string selectedItem;
        string selectedMethod;

        public ArithmecCompareNode(NodeEditorNodeScreenWidget screen, NodeInfo nodeInfo) : base(screen, nodeInfo)
        {
            Method = "Max";
            Item = "Health";

            var methodes = new List<string>
            {
                "Max",
                "Min"
            };

            selectedMethod = Method;
            methodSelection = new DropDownButtonWidget(Screen.NodeScriptContainerWidget.ModData);

            Func<string, ScrollItemWidget, ScrollItemWidget> setupItem2 = (option, template) =>
            {
                var item = ScrollItemWidget.Setup(template, () => selectedMethod == option, () =>
                {
                    selectedMethod = option;

                    methodSelection.Text = selectedMethod.ToString();
                    Method = selectedMethod;
                });

                item.Get<LabelWidget>("LABEL").GetText = () => option.ToString();

                return item;
            };

            methodSelection.OnClick = () =>
            {
                methodSelection.ShowDropDown("LABEL_DROPDOWN_TEMPLATE", 270, methodes, setupItem2);
            };

            methodSelection.Text = selectedMethod.ToString();

            AddChild(methodSelection);

            var items = new List<string>
            {
                "Health",
                "Damage",
                "Speed",
                "LocationX",
                "LocationY"
            };

            selectedItem = Item;
            itemSelection = new DropDownButtonWidget(Screen.NodeScriptContainerWidget.ModData);

            Func<string, ScrollItemWidget, ScrollItemWidget> setupItem = (option, template) =>
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

            methodSelection.Bounds =
                new Rectangle(FreeWidgetEntries.X, FreeWidgetEntries.Y + 77, FreeWidgetEntries.Width, 25);
            itemSelection.Bounds =
                new Rectangle(FreeWidgetEntries.X, FreeWidgetEntries.Y + 100, FreeWidgetEntries.Width, 25);
        }

        public override void AddOutConConstructor(OutConnection connection)
        {
            base.AddOutConConstructor(connection);

            if (NodeInfo.Method != null)
            {
                selectedMethod = NodeInfo.Method;
                methodSelection.Text = NodeInfo.Method;
            }

            if (NodeInfo.Item != null)
            {
                selectedItem = NodeInfo.Item;
                itemSelection.Text = NodeInfo.Item;
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

            if (Item == "Damage")
            {
                if (inc[0].In.ConnectionTyp == ConnectionType.Actor)
                    if (inc[0].In.Actor != null && inc[0].In.Actor.Trait<Health>() != null)
                        ints[0] = inc[0].In.Actor.Trait<Health>().MaxHP - inc[0].In.Actor.Trait<Health>().HP;

                if (inc[1].In.ConnectionTyp == ConnectionType.Actor)
                    if (inc[1].In.Actor != null && inc[1].In.Actor.Trait<Health>() != null)
                        ints[1] = inc[1].In.Actor.Trait<Health>().MaxHP - inc[1].In.Actor.Trait<Health>().HP;
            }

            if (Item == "Health")
            {
                if (inc[0].In.ConnectionTyp == ConnectionType.Actor)
                    if (inc[0].In.Actor != null && inc[0].In.Actor.Trait<Health>() != null)
                        ints[0] = inc[0].In.Actor.Trait<Health>().MaxHP;

                if (inc[1].In.ConnectionTyp == ConnectionType.Actor)
                    if (inc[1].In.Actor != null && inc[1].In.Actor.Trait<Health>() != null)
                        ints[1] = inc[1].In.Actor.Trait<Health>().MaxHP;
            }

            if (Item == "Speed")
            {
                if (inc[0].In.ConnectionTyp == ConnectionType.Actor)
                    if (inc[0].In.Actor != null && inc[0].In.Actor.Info.TraitInfo<MobileInfo>() != null)
                        ints[0] = inc[0].In.Actor.Info.TraitInfo<MobileInfo>().Speed;

                if (inc[1].In.ConnectionTyp == ConnectionType.Actor)
                    if (inc[1].In.Actor != null && inc[1].In.Actor.Info.TraitInfo<MobileInfo>() != null)
                        ints[1] = inc[1].In.Actor.Info.TraitInfo<MobileInfo>().Speed;
            }

            if (Item == "LocationX")
            {
                if (inc[0].In.ConnectionTyp == ConnectionType.Actor)
                    if (inc[0].In.Actor != null)
                        ints[0] = inc[0].In.Actor.Location.X;

                if (inc[1].In.ConnectionTyp == ConnectionType.Actor)
                    if (inc[1].In.Actor != null)
                        ints[1] = inc[1].In.Actor.Location.X;
            }

            if (Item == "LocationY")
            {
                if (inc[0].In.ConnectionTyp == ConnectionType.Actor)
                    if (inc[0].In.Actor != null)
                        ints[0] = inc[0].In.Actor.Location.Y;

                if (inc[1].In.ConnectionTyp == ConnectionType.Actor)
                    if (inc[1].In.Actor != null)
                        ints[1] = inc[1].In.Actor.Location.Y;
            }

            if (Method == "Max")
                OutConnections.First(c => c.ConnectionTyp == ConnectionType.Universal).Actor =
                    inc[ints.IndexOf(Math.Max(ints[0], ints[1]))].In.Actor;
        }
    }
}