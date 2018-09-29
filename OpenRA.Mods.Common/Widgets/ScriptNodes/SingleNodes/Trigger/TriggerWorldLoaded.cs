namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.Trigger
{
    public class TriggerWorldLoadedWidget : SimpleNodeWidget
    {
        CheckboxWidget checkBox;
        TextFieldWidget textField;

        public TriggerWorldLoadedWidget(NodeEditorNodeScreenWidget screen) : base(screen)
        {
            WidgetName = "Trigger: World Loaded";

            OutConnections.Add(new OutConnection(ConnectionType.Boolean, this));
        }
    }
}