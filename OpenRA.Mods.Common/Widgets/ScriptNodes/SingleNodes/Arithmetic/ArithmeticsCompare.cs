using System;
using System.Collections.Generic;
using System.Drawing;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.Arithmetic
{
    public class ArithmeticsCompare : SimpleNodeWidget
    {
        DropDownButtonWidget selectyByList;
        DropDownButtonWidget selectyByValue;
        string selectMethode;
        string selectValue;

        public ArithmeticsCompare(NodeEditorNodeScreenWidget screen) : base(screen)
        {
            WidgetName = "Arithmetic: Compare";

            OutConnections.Add(new OutConnection(ConnectionType.Actor, this));
            InConnections.Add(new InConnection(ConnectionType.ActorList, this));

            // Compare Methodes
            List<string> compareMethode = new List<string>
            {
                "Highest",
                "Lowest"
            };

            // Compare Values
            List<string> compareValues = new List<string>
            {
                "Health",
                "Cost",
                "Percent Damaged"
            };

            AddChild(selectyByList = new DropDownButtonWidget(screen.Snw.ModData));
            selectyByList.Bounds = new Rectangle(5, 5 + 26 + 26 + 26, 190, 25);

            Func<string, ScrollItemWidget, ScrollItemWidget> methodeItem = (option, template) =>
            {
                var item = ScrollItemWidget.Setup(template, () => selectMethode == option, () =>
                {
                    selectMethode = option;
                    selectyByList.Text = selectMethode;
                });
                item.Get<LabelWidget>("LABEL").GetText = () => option;

                return item;
            };

            selectyByList.Text = "Owner";

            selectyByList.OnClick = () =>
            {
                var nodes = compareMethode;
                selectyByList.ShowDropDown("LABEL_DROPDOWN_TEMPLATE", 270, nodes, methodeItem);
            };

            AddChild(selectyByValue = new DropDownButtonWidget(screen.Snw.ModData));
            selectyByValue.Bounds = new Rectangle(5, 5 + 26 + 26 + 26, 190, 25);

            Func<string, ScrollItemWidget, ScrollItemWidget> valueItem = (option, template) =>
            {
                var item = ScrollItemWidget.Setup(template, () => selectValue == option, () =>
                {
                    selectValue = option;
                    selectyByValue.Text = selectValue;
                });
                item.Get<LabelWidget>("LABEL").GetText = () => option;

                return item;
            };

            selectyByValue.Text = "Owner";

            selectyByValue.OnClick = () =>
            {
                var nodes = compareValues;
                selectyByValue.ShowDropDown("LABEL_DROPDOWN_TEMPLATE", 270, nodes, valueItem);
            };
        }
    }
}