using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.NodeInfos.ConditionInfos
{
    public class CheckConditionInfo : NodeInfo
    {
        public new static Dictionary<string, BuildNodeConstructorInfo> NodeConstructorInformation =
            new Dictionary<string, BuildNodeConstructorInfo>()
            {
                {
                    "CheckCondition", new BuildNodeConstructorInfo
                    {
                        Nesting = new[] {"Conditions"},
                        Name = "Check Condition",

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Condition, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        }
                    }
                },
            };

        LabelWidget labal1;
        LabelWidget labal2;
        DropDownButtonWidget methodSelection;
        string selectedMethod;

        public CheckConditionInfo(string nodeType, string nodeId, string nodeName) : base(nodeType, nodeId, nodeName)
        {
        }

        public override void WidgetInitialize(NodeWidget widget)
        {
            Method = "True";

            var methodes = new List<string>
            {
                "True",
                "False"
            };

            selectedMethod = Method;
            methodSelection = new DropDownButtonWidget(widget.Screen.NodeScriptContainerWidget.ModData);

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

            methodSelection.Bounds =
                new Rectangle(widget.FreeWidgetEntries.X, widget.FreeWidgetEntries.Y + 77,
                    widget.FreeWidgetEntries.Width, 25);

            labal1 = new LabelWidget();
            labal2 = new LabelWidget();

            labal1.Bounds = new Rectangle(widget.FreeWidgetEntries.X + 150, widget.FreeWidgetEntries.Y + 45,
                widget.FreeWidgetEntries.Width - 150, 25);
            labal2.Bounds = new Rectangle(widget.FreeWidgetEntries.X + 150, widget.FreeWidgetEntries.Y + 125,
                widget.FreeWidgetEntries.Width - 150, 25);

            labal1.Text = "True";
            labal2.Text = "False";

            widget.AddChild(labal1);
            widget.AddChild(labal2);

            widget.AddChild(methodSelection);
        }

        public override void WidgetAddOutConConstructor(OutConnection connection, NodeWidget widget)
        {
            base.WidgetAddOutConConstructor(connection, widget);

            if (Method != null)
            {
                selectedMethod = Method;
                methodSelection.Text = Method;
            }
        }

        public override void LogicExecute(World world, NodeLogic logic)
        {
            var inCo = logic.GetLinkedConnectionFromInConnection(ConnectionType.Condition, 0);

            if (inCo == null)
                Debug.WriteLine(NodeId + "Condition not connected");

            if (inCo.Logic.CheckCondition(world) && Method == "True" ||
                !inCo.Logic.CheckCondition(world) && Method == "False")
                NodeLogic.ForwardExec(logic, 0);

            else if (!inCo.Logic.CheckCondition(world) && Method == "True" ||
                     inCo.Logic.CheckCondition(world) && Method == "False")
                NodeLogic.ForwardExec(logic, 1);
        }
    }

   public class ProvideCondition : NodeLogic
    {
        public ProvideCondition(NodeInfo nodeInfo, IngameNodeScriptSystem ingameNodeScriptSystem) : base(nodeInfo,
            ingameNodeScriptSystem)
        {
        }

        public override bool CheckCondition(World world)
        {
            if (NodeType == "CompareActor")
            {
                var actCon1 = GetLinkedConnectionFromInConnection(ConnectionType.Actor, 0);
                var actCon2 = GetLinkedConnectionFromInConnection(ConnectionType.Actor, 1);

                if (actCon1 == null)
                    Debug.WriteLine(NodeId + "Actor 1 not connected");

                if (actCon2 == null)
                    Debug.WriteLine(NodeId + "Actor 2 not connected");

                if (actCon1.Actor == null || actCon2.Actor == null)
                    return false;

                return actCon1.Actor.Equals(actCon2.Actor);
            }

            if (NodeType == "CompareNumber")
            {
                var actCon1 = GetLinkedConnectionFromInConnection(ConnectionType.Integer, 0);
                var actCon2 = GetLinkedConnectionFromInConnection(ConnectionType.Integer, 1);

                if (actCon1 == null)
                {
                    Debug.WriteLine(NodeId + "Number 1 not connected");
                    return false;
                }

                if (actCon2 == null)
                {
                    Debug.WriteLine(NodeId + "Number 2 not connected");
                    return false;
                }

                if (actCon1.Number == null || actCon2.Number == null)
                    return false;

                return actCon1.Number.Value.Equals(actCon2.Number.Value);
            }

            if (NodeType == "CompareActorInfo")
            {
                var actCon1 = GetLinkedConnectionFromInConnection(ConnectionType.ActorInfo, 0);
                var actCon2 = GetLinkedConnectionFromInConnection(ConnectionType.ActorInfo, 1);

                if (actCon1 == null)
                {
                    Debug.WriteLine(NodeId + "Actor Info 1 not connected");
                    return false;
                }

                if (actCon2 == null)
                {
                    Debug.WriteLine(NodeId + "Actor Info 2 not connected");
                    return false;
                }

                if (actCon1.ActorInfo == null || actCon2.ActorInfo == null)
                    return false;

                return actCon1.ActorInfo.Equals(actCon2.ActorInfo);
            }

            if (NodeType == "IsAlive")
            {
                var actCon1 = GetLinkedConnectionFromInConnection(ConnectionType.Actor, 0);

                if (actCon1 == null)
                {
                    Debug.WriteLine(NodeId + "Actor not connected");
                    return false;
                }

                if (actCon1.ActorInfo == null)
                    return false;

                return !actCon1.Actor.IsDead;
            }

            if (NodeType == "IsDead")
            {
                var actCon1 = GetLinkedConnectionFromInConnection(ConnectionType.Actor, 0);

                if (actCon1 == null)
                {
                    Debug.WriteLine(NodeId + "Actor not connected");
                    return false;
                }

                if (actCon1.ActorInfo == null)
                    return false;

                return actCon1.Actor.IsDead;
            }

            if (NodeType == "IsPlaying"
                || NodeType == "IsBot"
                || NodeType == "IsHumanPlayer"
                || NodeType == "IsNoncombatant"
                || NodeType == "HasWon"
                || NodeType == "HasLost")
            {
                var actCon1 = GetLinkedConnectionFromInConnection(ConnectionType.Player, 0);

                if (actCon1 == null)
                {
                    Debug.WriteLine(NodeId + "Player not connected");
                    return false;
                }

                if (actCon1.Player == null)
                    return false;

                var player = world.Players.FirstOrDefault(p => p.InternalName == actCon1.Player.Name);

                if (player == null)
                    return false;

                if (NodeType == "IsPlaying")
                    return player.WinState == WinState.Undefined && !player.Spectating;

                if (NodeType == "IsBot")
                    return player.IsBot;

                if (NodeType == "IsHumanPlayer")
                    return !player.IsBot && !player.NonCombatant && player.Playable;

                if (NodeType == "IsNoncombatant")
                    return player.NonCombatant;

                if (NodeType == "HasWon")
                    return player.WinState == WinState.Won;

                if (NodeType == "HasLost")
                    return player.WinState == WinState.Lost;
            }

            return false;
        }
    }
}