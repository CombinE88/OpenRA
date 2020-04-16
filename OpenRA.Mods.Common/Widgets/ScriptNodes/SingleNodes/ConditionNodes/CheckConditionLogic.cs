using System;
using System.Collections.Generic;
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
            Methode = CompareMethod.True;

            var methodes = new List<CompareMethod>
            {
                CompareMethod.True,
                CompareMethod.False
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
            var inCo = InConnections.First(c => c.ConnectionTyp == ConnectionType.Condition);

            if (inCo.In == null)
                throw new YamlException(NodeId + "Condition not connected");

            if (inCo.In.Logic.CheckCondition(world) && Methode == CompareMethod.True ||
                !inCo.In.Logic.CheckCondition(world) && Methode == CompareMethod.False)
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
            else if (!inCo.In.Logic.CheckCondition(world) && Methode == CompareMethod.True ||
                     inCo.In.Logic.CheckCondition(world) && Methode == CompareMethod.False)
            {
                var oCon = OutConnections.LastOrDefault(o => o.ConnectionTyp == ConnectionType.Exec);
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
                var actCon1 = InConnections.First(c => c.ConnectionTyp == ConnectionType.Actor);
                var actCon2 = InConnections.Last(c => c.ConnectionTyp == ConnectionType.Actor);

                if (actCon1.In == null)
                    throw new YamlException(NodeId + "Actor 1 not connected");

                if (actCon2.In == null)
                    throw new YamlException(NodeId + "Actor 2 not connected");

                if (actCon1.In.Actor == null || actCon2.In.Actor == null)
                    return false;

                return actCon1.In.Actor.Equals(actCon2.In.Actor);
            }

            if (NodeType == NodeType.CompareNumber)
            {
                var actCon1 = InConnections.First(c => c.ConnectionTyp == ConnectionType.Integer);
                var actCon2 = InConnections.Last(c => c.ConnectionTyp == ConnectionType.Integer);

                if (actCon1.In == null)
                    throw new YamlException(NodeId + "Number 1 not connected");

                if (actCon2.In == null)
                    throw new YamlException(NodeId + "Number 2 not connected");

                if (actCon1.In.Number == null || actCon2.In.Number == null)
                    return false;

                return actCon1.In.Number.Value.Equals(actCon2.In.Number.Value);
            }

            if (NodeType == NodeType.CompareActorInfo)
            {
                var actCon1 = InConnections.First(c => c.ConnectionTyp == ConnectionType.ActorInfo);
                var actCon2 = InConnections.Last(c => c.ConnectionTyp == ConnectionType.ActorInfo);

                if (actCon1.In == null)
                    throw new YamlException(NodeId + "Actor Info 1 not connected");

                if (actCon2.In == null)
                    throw new YamlException(NodeId + "Actor Info 2 not connected");

                if (actCon1.In.ActorInfo == null || actCon2.In.ActorInfo == null)
                    return false;

                return actCon1.In.ActorInfo.Equals(actCon2.In.ActorInfo);
            }

            if (NodeType == NodeType.IsAlive)
            {
                var actCon1 = InConnections.First(c => c.ConnectionTyp == ConnectionType.Actor);

                if (actCon1.In == null)
                    throw new YamlException(NodeId + "Actor not connected");

                if (actCon1.In.ActorInfo == null)
                    return false;

                return !actCon1.In.Actor.IsDead;
            }

            if (NodeType == NodeType.IsDead)
            {
                var actCon1 = InConnections.First(c => c.ConnectionTyp == ConnectionType.Actor);

                if (actCon1.In == null)
                    throw new YamlException(NodeId + "Actor not connected");

                if (actCon1.In.ActorInfo == null)
                    return false;

                return actCon1.In.Actor.IsDead;
            }

            if (NodeType == NodeType.IsPlaying
                || NodeType == NodeType.IsBot
                || NodeType == NodeType.IsHumanPlayer
                || NodeType == NodeType.IsNoncombatant
                || NodeType == NodeType.HasWon
                || NodeType == NodeType.HasLost)
            {
                var actCon1 = InConnections.First(c => c.ConnectionTyp == ConnectionType.Player);

                if (actCon1.In == null)
                    throw new YamlException(NodeId + "Player not connected");

                if (actCon1.In.Player == null)
                    return false;

                var player = world.Players.FirstOrDefault(p => p.InternalName == actCon1.In.Player.Name);

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