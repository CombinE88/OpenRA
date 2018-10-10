using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenRA.Widgets;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.Arithmetics
{
    public class ArithmeticMathNode : NodeWidget
    {
        CompareMethode selectedMethode;
        DropDownButtonWidget methodeSelection;

        public ArithmeticMathNode(NodeEditorNodeScreenWidget screen, NodeInfo nodeInfo) : base(screen, nodeInfo)
        {
            Methode = CompareMethode.Add;

            List<CompareMethode> methodes = new List<CompareMethode>
            {
                CompareMethode.Add,
                CompareMethode.Substract,
                CompareMethode.Multiply,
                CompareMethode.Devide
            };

            selectedMethode = Methode;
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


    public class ArithmeticMathNodeLogic : NodeLogic
    {
        public ArithmeticMathNodeLogic(NodeInfo nodeinfo, IngameNodeScriptSystem insc) : base(nodeinfo, insc)
        {
        }

        public override void DoAfterConnections()
        {
            OutConnections.First(c => c.ConTyp == ConnectionType.Integer).Number = 0;
        }

        public override void Tick(Actor self)
        {
            var incon1 = InConnections.First(c => c.ConTyp == ConnectionType.Integer).In;
            var incon2 = InConnections.Last(c => c.ConTyp == ConnectionType.Integer).In;
            var outcon = OutConnections.First(c => c.ConTyp == ConnectionType.Integer);


            if (incon1 == null || incon2 == null)
                return;

            if (incon1.Number == null || incon2.Number == null)
                return;

            if (Methode == CompareMethode.Add)
                outcon.Number = incon1.Number.Value + incon2.Number.Value;
            else if (Methode == CompareMethode.Substract)
                outcon.Number = incon1.Number.Value - incon2.Number.Value >= 0 ? incon1.Number.Value - incon2.Number.Value : 0;
            else if (Methode == CompareMethode.Multiply)
                outcon.Number = incon1.Number.Value * incon2.Number.Value;
            else if (Methode == CompareMethode.Devide)
                outcon.Number = incon1.Number.Value / (incon2.Number.Value > 0 ? incon2.Number.Value : 1);
        }
    }
}