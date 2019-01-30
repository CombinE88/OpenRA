using System;
using System.Collections.Generic;
using System.Drawing;
using OpenRA.Mods.Common.Widgets.ScriptNodes;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.ConditionNodes
{
    public class SetConditionOnExec : NodeWidget
    {
        CompareMethode selectedMethode;
        DropDownButtonWidget methodeSelection;
        LabelWidget labal1;
        LabelWidget labal2;

        public SetConditionOnExec(NodeEditorNodeScreenWidget screen, NodeInfo nodeInfo) : base(screen, nodeInfo)
        {
            Methode = CompareMethode.True;

            List<CompareMethode> methodes = new List<CompareMethode>
            {
                CompareMethode.True,
                CompareMethode.False
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

            methodeSelection.Bounds = new Rectangle(FreeWidgetEntries.X, FreeWidgetEntries.Y + 77, FreeWidgetEntries.Width, 25);

            labal1 = new LabelWidget();
            labal2 = new LabelWidget();

            labal1.Bounds = new Rectangle(FreeWidgetEntries.X + 150, FreeWidgetEntries.Y + 45, FreeWidgetEntries.Width - 150, 25);
            labal2.Bounds = new Rectangle(FreeWidgetEntries.X + 150, FreeWidgetEntries.Y + 125, FreeWidgetEntries.Width - 150, 25);

            labal1.Text = "True";
            labal2.Text = "False";

            AddChild(labal1);
            AddChild(labal2);

            AddChild(methodeSelection);
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

    public class SetConditionOnExecLogic : NodeLogic
    {
        public bool condition;

        public SetConditionOnExecLogic(NodeInfo nodeinfo, IngameNodeScriptSystem insc) : base(nodeinfo, insc)
        {
        }

        public override void Execute(World world)
        {
            if (Methode == CompareMethode.True)
                condition = true;
            else if (Methode == CompareMethode.False)
                condition = false;
        }

        public override void DoAfterConnections()
        {
            if (Methode == CompareMethode.True)
                condition = false;
            else if (Methode == CompareMethode.False)
                condition = true;
        }

        public override bool CheckCondition(World world)
        {
            return condition;
        }
    }
}