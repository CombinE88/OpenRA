using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenRA.Mods.Common.Widgets.ScriptNodes.Library;
using OpenRA.Mods.Common.Widgets.ScriptNodes.NodeInfos;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.Arithmetics
{
    public class ArithmeticMathNode : NodeWidget
    {
        public new static Dictionary<string, BuildNodeConstructorInfo> NodeConstructorInformation =
            new Dictionary<string, BuildNodeConstructorInfo>()
            {
                {
                    "ArithmeticsMath", new BuildNodeConstructorInfo
                    {
                        LogicClass = typeof(ArithmeticMathNodeLogic),
                        Nesting = new[] {"Arithmetic's"},
                        Name = "Math",

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Integer, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Integer, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Integer, "")
                        }
                    }
                },
            };
        
        readonly DropDownButtonWidget methodSelection;
        string selectedMethod;

        public ArithmeticMathNode(NodeEditorNodeScreenWidget screen, NodeInfo nodeInfo) : base(screen, nodeInfo)
        {
            Method = "Add";

            var methodes = new List<string>
            {
                "Add",
                "Subtract",
                "Multiply",
                "Divide"
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

            AddChild(methodSelection);

            methodSelection.Bounds =
                new Rectangle(FreeWidgetEntries.X, FreeWidgetEntries.Y + 25, FreeWidgetEntries.Width, 25);
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

            if (Method == "Add")
                outcon.Number = incon1.Number.Value + incon2.Number.Value;
            else if (Method == "Subtract")
                outcon.Number = incon1.Number.Value - incon2.Number.Value >= 0
                    ? incon1.Number.Value - incon2.Number.Value
                    : 0;
            else if (Method == "Multiply")
                outcon.Number = incon1.Number.Value * incon2.Number.Value;
            else if (Method == "Divide")
                outcon.Number = incon1.Number.Value / (incon2.Number.Value > 0 ? incon2.Number.Value : 1);
        }
    }
}