using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.Arithmetics
{
    public class ArithmeticMathNode : NodeWidget
    {
        readonly DropDownButtonWidget methodeSelection;
        CompareMethod selectedMethod;

        public ArithmeticMathNode(NodeEditorNodeScreenWidget screen, NodeInfo nodeInfo) : base(screen, nodeInfo)
        {
            Method = CompareMethod.Add;

            var methodes = new List<CompareMethod>
            {
                CompareMethod.Add,
                CompareMethod.Subtract,
                CompareMethod.Multiply,
                CompareMethod.Divide
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

    public class ArithmeticMathNodeLogic : NodeLogic
    {
        public ArithmeticMathNodeLogic(NodeInfo nodeInfo, IngameNodeScriptSystem ingameNodeScriptSystem) : base(
            nodeInfo, ingameNodeScriptSystem)
        {
        }

        public override void DoAfterConnections()
        {
            OutConnections.First(c => c.ConnectionTyp == ConnectionType.Integer).Number = 0;
        }

        public override void Tick(Actor self)
        {
            var incon1 = GetLinkedConnectionFromInConnection(ConnectionType.Integer, 0);
            var incon2 = GetLinkedConnectionFromInConnection(ConnectionType.Integer, 1);
            var outcon = OutConnections.First(c => c.ConnectionTyp == ConnectionType.Integer);

            if (incon1 == null || incon2 == null)
                return;

            if (incon1.Number == null || incon2.Number == null)
                return;

            if (Methode == CompareMethod.Add)
                outcon.Number = incon1.Number.Value + incon2.Number.Value;
            else if (Methode == CompareMethod.Subtract)
                outcon.Number = incon1.Number.Value - incon2.Number.Value >= 0
                    ? incon1.Number.Value - incon2.Number.Value
                    : 0;
            else if (Methode == CompareMethod.Multiply)
                outcon.Number = incon1.Number.Value * incon2.Number.Value;
            else if (Methode == CompareMethod.Divide)
                outcon.Number = incon1.Number.Value / (incon2.Number.Value > 0 ? incon2.Number.Value : 1);
        }
    }
}