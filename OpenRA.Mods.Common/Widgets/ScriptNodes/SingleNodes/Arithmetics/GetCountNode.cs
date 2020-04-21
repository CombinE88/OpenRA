using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenRA.Mods.Common.Widgets.ScriptNodes.Library;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.Arithmetics
{
    public class GetCountNode : NodeWidget
    {
        public static Dictionary<NodeType, BuildNodeConstructorInfo> NodeBuilder =
            new Dictionary<NodeType, BuildNodeConstructorInfo>()
            {
                {
                    NodeType.Count, new BuildNodeConstructorInfo
                    {
                        LogicClass = typeof(GetCountNodeLogic),

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Universal, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Integer, "")
                        }
                    }
                },
            };
        
        readonly DropDownButtonWidget methodeSelection;
        CompareMethod selectedMethod;

        public GetCountNode(NodeEditorNodeScreenWidget screen, NodeInfo nodeInfo) : base(screen, nodeInfo)
        {
            Method = CompareMethod.All;

            var methodes = new List<CompareMethod>
            {
                CompareMethod.All,
                CompareMethod.PlayerIsPlaying,
                CompareMethod.AliveActors
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
            var outcon = GetLinkedConnectionFromInConnection(ConnectionType.Integer, 0);
            var incon = GetLinkedConnectionFromInConnection(ConnectionType.Universal, 0);
            var integ = 0;

            if (incon == null)
                return;

            if (incon.ConnectionTyp == ConnectionType.Integer && incon.Number != null)
            {
                outcon.Number = incon.Number.Value;
            }
            else if (incon.ConnectionTyp == ConnectionType.ActorList)
            {
                foreach (var actor in incon.ActorGroup)
                    if (Methode == CompareMethod.AliveActors && !actor.IsDead && actor.IsInWorld)
                        integ++;
                    else if (Methode == CompareMethod.All)
                        integ++;

                outcon.Number = integ;
            }
            else if (incon.ConnectionTyp == ConnectionType.PlayerGroup)
            {
                foreach (var player in incon.PlayerGroup)
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
            else if (incon.ConnectionTyp == ConnectionType.ActorInfoArray)
            {
                outcon.Number = incon.ActorInfos.Length;
            }
            else if (incon.ConnectionTyp == ConnectionType.CellPath)
            {
                outcon.Number = incon.CellArray.Count;
            }
            else if (incon.ConnectionTyp == ConnectionType.CellArray)
            {
                outcon.Number = incon.CellArray.Count;
            }
            else if (incon.ConnectionTyp == ConnectionType.LocationRange && incon.Number != null)
            {
                outcon.Number = incon.Number.Value;
            }
            else
            {
                outcon.Number = 0;
            }
        }
    }
}