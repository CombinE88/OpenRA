using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using OpenRA.Mods.Common.Widgets.ScriptNodes.Library;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.ConditionNodes
{
    public class CheckConditionNode : NodeWidget
    {
        public new static Dictionary<string, BuildNodeConstructorInfo> NodeConstructorInformation =
            new Dictionary<string, BuildNodeConstructorInfo>()
            {
                {
                    "CheckCondition", new BuildNodeConstructorInfo
                    {
                        LogicClass = typeof(CheckConditionLogic),
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

        readonly LabelWidget labal1;
        readonly LabelWidget labal2;
        readonly DropDownButtonWidget methodSelection;
        string selectedMethod;

        public CheckConditionNode(NodeEditorNodeScreenWidget screen, NodeInfo nodeInfo) : base(screen, nodeInfo)
        {
            Method = "True";

            var methodes = new List<string>
            {
                "True",
                "False"
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

            methodSelection.Bounds =
                new Rectangle(FreeWidgetEntries.X, FreeWidgetEntries.Y + 77, FreeWidgetEntries.Width, 25);

            labal1 = new LabelWidget();
            labal2 = new LabelWidget();

            labal1.Bounds = new Rectangle(FreeWidgetEntries.X + 150, FreeWidgetEntries.Y + 45,
                FreeWidgetEntries.Width - 150, 25);
            labal2.Bounds = new Rectangle(FreeWidgetEntries.X + 150, FreeWidgetEntries.Y + 125,
                FreeWidgetEntries.Width - 150, 25);

            labal1.Text = "True";
            labal2.Text = "False";

            AddChild(labal1);
            AddChild(labal2);

            AddChild(methodSelection);
        }

        public override void AddOutConConstructor(OutConnection connection)
        {
            base.AddOutConConstructor(connection);

            if (NodeInfo.Method != null)
            {
                selectedMethod = NodeInfo.Method;
                methodSelection.Text = NodeInfo.Method;
            }
        }
    }

    public class CheckConditionLogic : NodeLogic
    {
        public CheckConditionLogic(NodeInfo nodeInfo, IngameNodeScriptSystem ingameNodeScriptSystem) : base(nodeInfo,
            ingameNodeScriptSystem)
        {
        }

        public override void Execute(World world)
        {
            var inCo = GetLinkedConnectionFromInConnection(ConnectionType.Condition, 0);

            if (inCo == null)
                Debug.WriteLine(NodeId + "Condition not connected");

            if (inCo.Logic.CheckCondition(world) && Method == "True" ||
                !inCo.Logic.CheckCondition(world) && Method == "False")
                ForwardExec(this, 0);

            else if (!inCo.Logic.CheckCondition(world) && Method == "True" ||
                     inCo.Logic.CheckCondition(world) && Method == "False")
                ForwardExec(this, 1);
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