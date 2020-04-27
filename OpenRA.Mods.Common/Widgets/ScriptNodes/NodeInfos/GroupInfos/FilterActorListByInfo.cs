using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using OpenRA.Mods.Common.Traits;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.NodeInfos.GroupInfos
{
    public class FilterActorListByInfo : NodeInfo
    {
        public new static Dictionary<string, BuildNodeConstructorInfo> NodeConstructorInformation =
            new Dictionary<string, BuildNodeConstructorInfo>()
            {
                {
                    "FilterActorGroup", new BuildNodeConstructorInfo
                    {
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

        DropDownButtonWidget itemSelection;
        DropDownButtonWidget methodSelection;
        string selectedItem;
        string selectedMethod;

        public FilterActorListByInfo(string nodeType, string nodeId, string nodeName) : base(nodeType, nodeId, nodeName)
        {
        }

        public override void WidgetInitialize(NodeWidget widget)
        {
            Method = "Contains";
            Item = "Owner";

            var methodes = new List<string>
            {
                "Contains",
                "ContainsNot"
            };

            selectedMethod = Method;
            methodSelection = new DropDownButtonWidget(widget.Screen.NodeScriptContainerWidget.ModData);

            Func<string, ScrollItemWidget, ScrollItemWidget> setupItem2 = (option, template) =>
            {
                var item = ScrollItemWidget.Setup(template, () => selectedMethod == option, () =>
                {
                    selectedMethod = option;

                    methodSelection.Text = selectedMethod;
                    Method = selectedMethod;
                });

                item.Get<LabelWidget>("LABEL").GetText = () => option;

                return item;
            };

            methodSelection.OnClick = () =>
            {
                methodSelection.ShowDropDown("LABEL_DROPDOWN_TEMPLATE", 270, methodes, setupItem2);
            };

            methodSelection.Text = selectedMethod.ToString();

            widget.AddChild(methodSelection);

            var items = new List<string>
            {
                "Owner",
                "Building",
                "Aircraft",
                "Unit",
                "ActorTypes",
                "IsIdle"
            };

            selectedItem = Item;
            itemSelection = new DropDownButtonWidget(widget.Screen.NodeScriptContainerWidget.ModData);

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

            widget.AddChild(itemSelection);

            methodSelection.Bounds =
                new Rectangle(widget.FreeWidgetEntries.X, widget.FreeWidgetEntries.Y + 77,
                    widget.FreeWidgetEntries.Width, 25);
            itemSelection.Bounds =
                new Rectangle(widget.FreeWidgetEntries.X, widget.FreeWidgetEntries.Y + 100,
                    widget.FreeWidgetEntries.Width, 25);
        }

        public override void WidgetAddOutConConstructor(OutConnection connection, NodeWidget widget)
        {
            base.WidgetAddOutConConstructor(connection, widget);

            if (Method != null)
            {
                selectedMethod = Method;
                methodSelection.Text = Method;
            }

            if (Item != null)
            {
                selectedItem = Item;
                itemSelection.Text = Item;
            }
        }

        public override void LogicExecute(World world, NodeLogic logic)
        {
            var actIn = logic.GetLinkedConnectionFromInConnection(ConnectionType.ActorList, 0);
            var actOut = logic.OutConnections.First(c => c.ConnectionTyp == ConnectionType.ActorList);

            if (actIn == null)
            {
                Debug.WriteLine(NodeId + "Actor List not connected");
                return;
            }

            if (Item == "Owner")
            {
                var ply = logic.GetLinkedConnectionFromInConnection(ConnectionType.ActorList, 0);

                if (ply == null)
                {
                    Debug.WriteLine(NodeId + "Player not connected");
                    return;
                }

                if (Method == "Contains")
                    actOut.ActorGroup = actIn.ActorGroup
                        .Where(c => c.Owner == world.Players.First(p => p.InternalName == ply.Player.Name)).ToArray();
                else
                    actOut.ActorGroup = actIn.ActorGroup
                        .Where(c => c.Owner == world.Players.First(p => p.InternalName != ply.Player.Name)).ToArray();
            }
            else if (Item == "Aircraft")
            {
                if (Method == "Contains")
                    actOut.ActorGroup = actIn.ActorGroup.Where(c => c.Trait<Aircraft>() != null).ToArray();
                else
                    actOut.ActorGroup = actIn.ActorGroup.Where(c => c.Trait<Aircraft>() == null).ToArray();
            }
            else if (Item == "Building")
            {
                if (Method == "Contains")
                    actOut.ActorGroup = actIn.ActorGroup.Where(c => c.Trait<Building>() != null).ToArray();
                else
                    actOut.ActorGroup = actIn.ActorGroup.Where(c => c.Trait<Building>() == null).ToArray();
            }
            else if (Item == "Unit")
            {
                if (Method == "Contains")
                    actOut.ActorGroup = actIn.ActorGroup.Where(c => c.Trait<Mobile>() != null).ToArray();
                else
                    actOut.ActorGroup = actIn.ActorGroup.Where(c => c.Trait<Mobile>() == null).ToArray();
            }
            else if (Item == "ActorTypes")
            {
                var strings = logic.GetLinkedConnectionFromInConnection(ConnectionType.ActorInfoArray, 0);
                if (strings == null)
                {
                    Debug.WriteLine(NodeId + "Actor Types not connected");
                    return;
                }

                if (Method == "Contains")
                    actOut.ActorGroup = actIn.ActorGroup.Where(c => strings.ActorInfos.Contains(c.Info)).ToArray();
                else
                    actOut.ActorGroup = actIn.ActorGroup.Where(c => !strings.ActorInfos.Contains(c.Info)).ToArray();
            }
            else if (Item == "IsIdle")
            {
                if (Method == "Contains")
                    actOut.ActorGroup = actIn.ActorGroup.Where(c => c.IsIdle).ToArray();
                else
                    actOut.ActorGroup = actIn.ActorGroup.Where(c => !c.IsIdle).ToArray();
            }

            NodeLogic.ForwardExec(logic);
        }
    }
}