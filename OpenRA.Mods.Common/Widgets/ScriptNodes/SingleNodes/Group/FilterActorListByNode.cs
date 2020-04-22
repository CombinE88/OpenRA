using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using OpenRA.Mods.Common.Traits;
using OpenRA.Mods.Common.Widgets.ScriptNodes.Library;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.Group
{
    public class FilterActorListByNode : NodeWidget
    {
        public new static Dictionary<string, BuildNodeConstructorInfo> NodeConstructorInformation =
            new Dictionary<string, BuildNodeConstructorInfo>()
            {
                {
                    "FilterActorGroup", new BuildNodeConstructorInfo
                    {
                        LogicClass = typeof(FilterActorListByLogic),
                        Nesting = new[] {"Actor/Player Group"},
                        Name = "Filter Actors in Group",

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Player, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.ActorList, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.ActorList, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        }
                    }
                },
            };

        readonly DropDownButtonWidget itemSelection;
        readonly DropDownButtonWidget methodeSelection;
        CompareItem selectedItem;
        CompareMethod selectedMethod;

        public FilterActorListByNode(NodeEditorNodeScreenWidget screen, NodeInfo nodeInfo) : base(screen, nodeInfo)
        {
            Method = CompareMethod.Contains;
            Item = CompareItem.Owner;

            var methodes = new List<CompareMethod>
            {
                CompareMethod.Contains,
                CompareMethod.ContainsNot
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
            var actIn = GetLinkedConnectionFromInConnection(ConnectionType.ActorList, 0);
            var actOut = OutConnections.First(c => c.ConnectionTyp == ConnectionType.ActorList);

            if (actIn == null)
            {
                Debug.WriteLine(NodeId + "Actor List not connected");
                return;
            }

            if (Item == CompareItem.Owner)
            {
                var ply = GetLinkedConnectionFromInConnection(ConnectionType.ActorList, 0);

                if (ply == null)
                {
                    Debug.WriteLine(NodeId + "Player not connected");
                    return;
                }

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
                var strings = GetLinkedConnectionFromInConnection(ConnectionType.ActorInfoArray, 0);
                if (strings == null)
                {
                    Debug.WriteLine(NodeId + "Actor Types not connected");
                    return;
                }

                if (Methode == CompareMethod.Contains)
                    actOut.ActorGroup = actIn.ActorGroup.Where(c => strings.ActorInfos.Contains(c.Info)).ToArray();
                else
                    actOut.ActorGroup = actIn.ActorGroup.Where(c => !strings.ActorInfos.Contains(c.Info)).ToArray();
            }
            else if (Item == CompareItem.IsIdle)
            {
                if (Methode == CompareMethod.Contains)
                    actOut.ActorGroup = actIn.ActorGroup.Where(c => c.IsIdle).ToArray();
                else
                    actOut.ActorGroup = actIn.ActorGroup.Where(c => !c.IsIdle).ToArray();
            }

            ForwardExec(this);
        }
    }
}