using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.Arithmetics
{
    public class GetCountNode : NodeWidget
    {
        CompareMethode selectedMethode;
        DropDownButtonWidget methodeSelection;

        public GetCountNode(NodeEditorNodeScreenWidget screen, NodeInfo nodeInfo) : base(screen, nodeInfo)
        {
            Methode = CompareMethode.All;

            List<CompareMethode> methodes = new List<CompareMethode>
            {
                CompareMethode.All,
                CompareMethode.PleyerPlaying,
                CompareMethode.AliveActors
            };

            selectedMethode = Methode.Value;
            methodeSelection = new DropDownButtonWidget(Screen.Snw.ModData);

            Func<CompareMethode, ScrollItemWidget, ScrollItemWidget> setupItem2 = (option, template) =>
            {
                var item = ScrollItemWidget.Setup(template, () => selectedMethode == option, () =>
                {
                    selectedMethode = option;

                    methodeSelection.Text = selectedMethode.ToString();
                    Methode = selectedMethode;
                });

                item.Get<LabelWidget>("LABEL").GetText = () => option.ToString();

                return item;
            };

            methodeSelection.OnClick = () => { methodeSelection.ShowDropDown("LABEL_DROPDOWN_TEMPLATE", 270, methodes, setupItem2); };

            methodeSelection.Text = selectedMethode.ToString();

            AddChild(methodeSelection);

            methodeSelection.Bounds = new Rectangle(FreeWidgetEntries.X, FreeWidgetEntries.Y + 25, FreeWidgetEntries.Width, 25);
        }

        public override void AddOutConConstructor(OutConnection connection)
        {
            base.AddOutConConstructor(connection);

            if (NodeInfo.Methode != null)
            {
                selectedMethode = NodeInfo.Methode.Value;
                methodeSelection.Text = NodeInfo.Methode.Value.ToString();
            }
        }
    }

    public class GetCountNodeLogic : NodeLogic
    {
        public GetCountNodeLogic(NodeInfo nodeinfo, IngameNodeScriptSystem insc) : base(nodeinfo, insc)
        {
        }

        public override void Tick(Actor self)
        {
            var outcon = OutConnections.First(c => c.ConTyp == ConnectionType.Integer);
            var incon = InConnections.First(c => c.ConTyp == ConnectionType.Universal);
            var integ = 0;

            if (incon.In == null)
                return;

            if (incon.In.ConTyp == ConnectionType.Integer && incon.In.Number != null)
                outcon.Number = incon.In.Number.Value;
            else if (incon.In.ConTyp == ConnectionType.ActorList)
            {
                foreach (var actor in incon.In.ActorGroup)
                {
                        if (Methode == CompareMethode.AliveActors && !actor.IsDead && actor.IsInWorld)
                            integ++;
                        else if (Methode == CompareMethode.All)
                            integ++;
                }

                outcon.Number = integ;
            }
            else if (incon.In.ConTyp == ConnectionType.PlayerGroup)
            {
                foreach (var player in incon.In.PlayerGroup)
                {
                    var play = self.World.Players.First(p => p.PlayerReference == player);
                    if (!play.NonCombatant && play.Playable)
                    {
                        if (Methode == CompareMethode.PleyerPlaying && play.WinState == WinState.Undefined)
                            integ++;
                        else if (Methode == CompareMethode.All)
                            integ++;
                    }
                }

                outcon.Number = integ;
            }
            else if (incon.In.ConTyp == ConnectionType.ActorInfoArray)
                outcon.Number = incon.In.ActorInfos.Length;
            else if (incon.In.ConTyp == ConnectionType.CellPath)
                outcon.Number = incon.In.CellArray.Count;
            else if (incon.In.ConTyp == ConnectionType.CellArray)
                outcon.Number = incon.In.CellArray.Count;
            else if (incon.In.ConTyp == ConnectionType.LocationRange && incon.In.Number != null)
                outcon.Number = incon.In.Number.Value;
            else
                outcon.Number = 0;
        }
    }
}