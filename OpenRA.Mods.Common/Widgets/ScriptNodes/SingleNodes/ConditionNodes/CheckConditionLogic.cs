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
        readonly LabelWidget labal1;
        readonly LabelWidget labal2;
        readonly DropDownButtonWidget methodeSelection;
        CompareMethod selectedMethod;

        public CheckConditionNode(NodeEditorNodeScreenWidget screen, NodeInfo nodeInfo) : base(screen, nodeInfo)
        {
            Method = CompareMethod.True;

            var methodes = new List<CompareMethod>
            {
                CompareMethod.True,
                CompareMethod.False
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

            methodeSelection.Bounds =
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

            AddChild(methodeSelection);
        }

        public override void AddOutConConstructor(OutConnection connection)
        {
            base.AddOutConConstructor(connection);

            if (NodeInfo.Method != null)
            {
                selectedMethod = NodeInfo.Method.Value;
                methodeSelection.Text = NodeInfo.Method.Value.ToString();
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

            if (inCo.Logic.CheckCondition(world) && Methode == CompareMethod.True ||
                !inCo.Logic.CheckCondition(world) && Methode == CompareMethod.False)
                ForwardExec(this, 0);

            else if (!inCo.Logic.CheckCondition(world) && Methode == CompareMethod.True ||
                     inCo.Logic.CheckCondition(world) && Methode == CompareMethod.False)
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
            if (NodeType == NodeType.CompareActor)
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

            if (NodeType == NodeType.CompareNumber)
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

            if (NodeType == NodeType.CompareActorInfo)
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

            if (NodeType == NodeType.IsAlive)
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

            if (NodeType == NodeType.IsDead)
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

            if (NodeType == NodeType.IsPlaying
                || NodeType == NodeType.IsBot
                || NodeType == NodeType.IsHumanPlayer
                || NodeType == NodeType.IsNoncombatant
                || NodeType == NodeType.HasWon
                || NodeType == NodeType.HasLost)
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

                if (NodeType == NodeType.IsPlaying)
                    return player.WinState == WinState.Undefined && !player.Spectating;

                if (NodeType == NodeType.IsBot)
                    return player.IsBot;

                if (NodeType == NodeType.IsHumanPlayer)
                    return !player.IsBot && !player.NonCombatant && player.Playable;

                if (NodeType == NodeType.IsNoncombatant)
                    return player.NonCombatant;

                if (NodeType == NodeType.HasWon)
                    return player.WinState == WinState.Won;

                if (NodeType == NodeType.HasLost)
                    return player.WinState == WinState.Lost;
            }

            return false;
        }
    }
}