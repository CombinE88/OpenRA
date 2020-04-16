using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.Arithmetics
{
    public class GetCountNode : NodeWidget
    {
        readonly DropDownButtonWidget methodeSelection;
        CompareMethod selectedMethod;

        public GetCountNode(NodeEditorNodeScreenWidget screen, NodeInfo nodeInfo) : base(screen, nodeInfo)
        {
            Methode = CompareMethod.All;

            var methodes = new List<CompareMethod>
            {
                CompareMethod.All,
                CompareMethod.PlayerIsPlaying,
                CompareMethod.AliveActors
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

            methodeSelection.Bounds =
                new Rectangle(FreeWidgetEntries.X, FreeWidgetEntries.Y + 25, FreeWidgetEntries.Width, 25);
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

    public class GetCountNodeLogic : NodeLogic
    {
        public GetCountNodeLogic(NodeInfo nodeInfo, IngameNodeScriptSystem ingameNodeScriptSystem) : base(nodeInfo,
            ingameNodeScriptSystem)
        {
        }

        public override void Tick(Actor self)
        {
            var outcon = OutConnections.First(c => c.ConnectionTyp == ConnectionType.Integer);
            var incon = InConnections.First(c => c.ConnectionTyp == ConnectionType.Universal);
            var integ = 0;

            if (incon.In == null)
                return;

            if (incon.In.ConnectionTyp == ConnectionType.Integer && incon.In.Number != null)
            {
                outcon.Number = incon.In.Number.Value;
            }
            else if (incon.In.ConnectionTyp == ConnectionType.ActorList)
            {
                foreach (var actor in incon.In.ActorGroup)
                    if (Methode == CompareMethod.AliveActors && !actor.IsDead && actor.IsInWorld)
                        integ++;
                    else if (Methode == CompareMethod.All)
                        integ++;

                outcon.Number = integ;
            }
            else if (incon.In.ConnectionTyp == ConnectionType.PlayerGroup)
            {
                foreach (var player in incon.In.PlayerGroup)
                {
                    var play = self.World.Players.First(p => p.PlayerReference == player);
                    if (!play.NonCombatant && play.Playable)
                    {
                        if (Methode == CompareMethod.PlayerIsPlaying && play.WinState == WinState.Undefined)
                            integ++;
                        else if (Methode == CompareMethod.All)
                            integ++;
                    }
                }

                outcon.Number = integ;
            }
            else if (incon.In.ConnectionTyp == ConnectionType.ActorInfoArray)
            {
                outcon.Number = incon.In.ActorInfos.Length;
            }
            else if (incon.In.ConnectionTyp == ConnectionType.CellPath)
            {
                outcon.Number = incon.In.CellArray.Count;
            }
            else if (incon.In.ConnectionTyp == ConnectionType.CellArray)
            {
                outcon.Number = incon.In.CellArray.Count;
            }
            else if (incon.In.ConnectionTyp == ConnectionType.LocationRange && incon.In.Number != null)
            {
                outcon.Number = incon.In.Number.Value;
            }
            else
            {
                outcon.Number = 0;
            }
        }
    }
}