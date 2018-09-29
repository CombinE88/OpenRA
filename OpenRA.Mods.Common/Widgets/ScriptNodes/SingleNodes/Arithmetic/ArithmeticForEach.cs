using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.Arithmetic
{
    public class ArithmeticForEachWidget : SimpleNodeWidget
    {
        public ArithmeticForEachWidget(NodeEditorNodeScreenWidget screen) : base(screen)
        {
            WidgetName = "Arithmetic: For Each";

            InConnections.Add(new InConnection(ConnectionType.ActorList, this));
            InConnections.Add(new InConnection(ConnectionType.Boolean, this));
            OutConnections.Add(new OutConnection(ConnectionType.Actor, this));
            OutConnections.Add(new OutConnection(ConnectionType.Boolean, this));
        }
    }
}