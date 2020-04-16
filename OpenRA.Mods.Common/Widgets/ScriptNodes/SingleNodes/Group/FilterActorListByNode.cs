using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenRA.Mods.Common.Traits;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.Group
{
    public class FilterActorListByNode : NodeWidget
    {
        readonly DropDownButtonWidget itemSelection;
        readonly DropDownButtonWidget methodeSelection;
        CompareItem selectedItem;
        CompareMethod selectedMethod;

        public FilterActorListByNode(NodeEditorNodeScreenWidget screen, NodeInfo nodeInfo) : base(screen, nodeInfo)
        {
            Methode = CompareMethod.Contains;
            Item = CompareItem.Owner;

            var methodes = new List<CompareMethod>
            {
                CompareMethod.Contains,
                CompareMethod.ContainsNot
            };

            selectedMethod = Methode.Value;
            methodeSelection = new DropDownButtonWidget(Screen.NodeScriptContainerWidget.ModData);

            Func<CompareMethod, ScrollItemWidget, ScrollItemWidget> setupItem2 = (option, template) =>
            {
                var item = ScrollItemWidget.Setup(template, () => selectedMethod == option, () =>
                {
                    selectedMethod = option;

                    methodeSelection.Text = selectedMethod.ToString();
                    Methode = selectedMethod;
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
                CompareItem.Owner,
                CompareItem.Building,
                CompareItem.Aircraft,
                CompareItem.Unit,
                CompareItem.ActorTypes,
                CompareItem.IsIdle
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

    public class FilterActorListByLogic : NodeLogic
    {
        public FilterActorListByLogic(NodeInfo nodeInfo, IngameNodeScriptSystem ingameNodeScriptSystem) : base(nodeInfo,
            ingameNodeScriptSystem)
        {
        }

        public override void Execute(World world)
        {
            var actIn = InConnections.First(c => c.ConnectionTyp == ConnectionType.ActorList).In;
            var actOut = OutConnections.First(c => c.ConnectionTyp == ConnectionType.ActorList);

            if (actIn == null)
                throw new YamlException(NodeId + "Actor List not connected");

            if (Item == CompareItem.Owner)
            {
                var ply = InConnections.First(c => c.ConnectionTyp == ConnectionType.ActorList).In;

                if (ply == null)
                    throw new YamlException(NodeId + "Player not connected");

                if (Methode == CompareMethod.Contains)
                    actOut.ActorGroup = actIn.ActorGroup
                        .Where(c => c.Owner == world.Players.First(p => p.InternalName == ply.Player.Name)).ToArray();
                else
                    actOut.ActorGroup = actIn.ActorGroup
                        .Where(c => c.Owner == world.Players.First(p => p.InternalName != ply.Player.Name)).ToArray();
            }
            else if (Item == CompareItem.Aircraft)
            {
                if (Methode == CompareMethod.Contains)
                    actOut.ActorGroup = actIn.ActorGroup.Where(c => c.Trait<Aircraft>() != null).ToArray();
                else
                    actOut.ActorGroup = actIn.ActorGroup.Where(c => c.Trait<Aircraft>() == null).ToArray();
            }
            else if (Item == CompareItem.Building)
            {
                if (Methode == CompareMethod.Contains)
                    actOut.ActorGroup = actIn.ActorGroup.Where(c => c.Trait<Building>() != null).ToArray();
                else
                    actOut.ActorGroup = actIn.ActorGroup.Where(c => c.Trait<Building>() == null).ToArray();
            }
            else if (Item == CompareItem.Unit)
            {
                if (Methode == CompareMethod.Contains)
                    actOut.ActorGroup = actIn.ActorGroup.Where(c => c.Trait<Mobile>() != null).ToArray();
                else
                    actOut.ActorGroup = actIn.ActorGroup.Where(c => c.Trait<Mobile>() == null).ToArray();
            }
            else if (Item == CompareItem.ActorTypes)
            {
                var strngs = InConnections.First(c => c.ConnectionTyp == ConnectionType.ActorInfoArray).In;
                if (strngs == null)
                    throw new YamlException(NodeId + "Actor Types not connected");

                if (Methode == CompareMethod.Contains)
                    actOut.ActorGroup = actIn.ActorGroup.Where(c => strngs.ActorInfos.Contains(c.Info)).ToArray();
                else
                    actOut.ActorGroup = actIn.ActorGroup.Where(c => !strngs.ActorInfos.Contains(c.Info)).ToArray();
            }
            else if (Item == CompareItem.IsIdle)
            {
                if (Methode == CompareMethod.Contains)
                    actOut.ActorGroup = actIn.ActorGroup.Where(c => c.IsIdle).ToArray();
                else
                    actOut.ActorGroup = actIn.ActorGroup.Where(c => !c.IsIdle).ToArray();
            }

            if (actOut.ActorGroup.Any())
            {
                var oCon = OutConnections.FirstOrDefault(o => o.ConnectionTyp == ConnectionType.Exec);
                if (oCon != null)
                    foreach (var node in IngameNodeScriptSystem.NodeLogics.Where(n =>
                        n.InConnections.FirstOrDefault(c => c.ConnectionTyp == ConnectionType.Exec) != null))
                    {
                        var inCon = node.InConnections.FirstOrDefault(c =>
                            c.ConnectionTyp == ConnectionType.Exec && c.In == oCon);
                        if (inCon != null)
                            inCon.Execute = true;
                    }
            }
        }
    }
}