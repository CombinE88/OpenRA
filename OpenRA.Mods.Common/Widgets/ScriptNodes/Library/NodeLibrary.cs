using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using OpenRA.Mods.Common.Widgets.ScriptNodes.NodeInfos;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.TriggerNodes;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.Library
{
    public static class NodeLibrary
    {
        public static List<NodeWidget> LoadInNodes(NodeEditorNodeScreenWidget nensw, List<NodeInfo> nodeInfos)
        {
            // TODO Check for Duplications and cast exception

            var nodes = new List<NodeWidget>();
            foreach (var nodeInfo in nodeInfos)
            {
                Type choosenType;
                foreach (var type in Assembly.GetAssembly(typeof(NodeInfo)).GetTypes().Where(type =>
                    type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(NodeInfo))))
                {
                    var dictObject = type.GetField("NodeConstructorInformation").GetValue(null);
                    var dictionary = (Dictionary<string, BuildNodeConstructorInfo>) dictObject;

                    if (!dictionary.ContainsKey(nodeInfo.NodeType))
                        continue;

                    var createNode = new NodeWidget(nensw, nodeInfo);


                    createNode.DeleteButton.OnClick = () => { nensw.RemoveNode(createNode); };

                    nodes.Add(createNode);
                }
            }

            return nodes;
        }

        public static NodeWidget AddNode(string nodeType, NodeEditorNodeScreenWidget nensw, string nodeId = null,
            string nodeName = null)
        {
            NodeWidget nodeWidget = null;
            var dictionary = new Dictionary<string, BuildNodeConstructorInfo>();

            foreach (var type in Assembly.GetAssembly(typeof(NodeInfo)).GetTypes().Where(type =>
                type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(NodeInfo))))
            {
                var dictObject = type.GetField("NodeConstructorInformation").GetValue(null);
                dictionary = (Dictionary<string, BuildNodeConstructorInfo>) dictObject;

                if (!dictionary.ContainsKey(nodeType))
                    continue;

                if (dictionary[nodeType].WidgetType == typeof(TriggerNodeWorldLoaded) &&
                    nensw.Nodes.Any(n => n.GetType() == typeof(TriggerNodeWorldLoaded)))
                    continue;

                if (dictionary[nodeType].WidgetType == typeof(TriggerNodeTick) &&
                    nensw.Nodes.Any(n => n.GetType() == typeof(TriggerNodeTick)))
                    continue;

                var nodeInfo = (NodeInfo) type
                    .GetConstructor(new[] {typeof(string), typeof(string), typeof(string)})
                    .Invoke(new object[] {nodeType, nodeId, nodeName});

                nodeWidget = new NodeWidget(nensw, nodeInfo);

                break;
            }

            if (nodeWidget != null && dictionary.Any())
            {
                if (!dictionary.Any())
                    return nodeWidget;

                var connectionDictionary = dictionary[nodeType];
                if (connectionDictionary.InConnections != null && connectionDictionary.InConnections.Any())
                    foreach (var connection in connectionDictionary.InConnections)
                        nodeWidget.AddInConnection(new InConnection(connection.Item1, nodeWidget));

                if (connectionDictionary.OutConnections != null && connectionDictionary.OutConnections != null)
                    foreach (var connection in connectionDictionary.OutConnections)
                        nodeWidget.AddOutConnection(new OutConnection(connection.Item1, nodeWidget));
            }

            return nodeWidget;
        }

        public static List<NodeLogic> InitializeNodes(IngameNodeScriptSystem inss, List<NodeInfo> nodeInfos)
        {
            var nodeList = new List<NodeLogic>();

            foreach (var nodeInfo in nodeInfos)
            {
                foreach (var type in Assembly.GetAssembly(typeof(NodeInfo)).GetTypes().Where(type =>
                    type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(NodeInfo))))
                {
                    var dictObject = type.GetField("NodeConstructorInformation").GetValue(null);
                    var dictionary = (Dictionary<string, BuildNodeConstructorInfo>) dictObject;

                    if (!dictionary.ContainsKey(nodeInfo.NodeType) || dictionary[nodeInfo.NodeType].LogicClass == null)
                        continue;

                    var createNode = new NodeLogic(nodeInfo, inss);

                    nodeList.Add(createNode);
                }
            }

            return nodeList;
        }

        public static DropDownMenuWidget BuildWidgetMenu(ModData modData,
            NodeScriptContainerWidget nodeScriptContainerWidget, NodeEditorNodeScreenWidget nodeEditorNodeScreenWidget)
        {
            var dropDownMenuWidget = new DropDownMenuWidget
            {
                Bounds = new Rectangle(0, 0, 180, 75),
                Visible = false
            };

            foreach (var type in Assembly.GetAssembly(typeof(NodeInfo)).GetTypes().Where(type =>
                type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(NodeInfo))))
            {
                var dictObject = type.GetField("NodeConstructorInformation").GetValue(null);

                if (dictObject == null)
                    continue;

                var dictionary = (Dictionary<string, BuildNodeConstructorInfo>) dictObject;

                if (!dictionary.Any())
                    continue;

                foreach (var entry in dictionary)
                {
                    var createNodeWidget = new ButtonWidget(nodeScriptContainerWidget.ModData)
                    {
                        Bounds = new Rectangle(0, 0,
                            nodeScriptContainerWidget.FontRegular.Measure(entry.Value.Name).X + 25, 25),
                        Text = entry.Value.Name,
                        Align = TextAlign.Left,
                        OnClick = () =>
                        {
                            nodeEditorNodeScreenWidget.AddNode(entry.Key);
                            DropDownMenuWidget.Collapse(dropDownMenuWidget);
                            dropDownMenuWidget.Visible = false;
                        }
                    };

                    if (!entry.Value.Nesting.Any())
                    {
                        dropDownMenuWidget.AddDropDownMenu(createNodeWidget);
                        continue;
                    }

                    var label = entry.Value.Nesting.First();

                    var dropDownWidgetExpander =
                        new DropDownMenuExpandButton(nodeScriptContainerWidget.ModData,
                            new Rectangle(0, 0, 160, 25))
                        {
                            Text = label
                        };

                    var getGroundsChild = (DropDownMenuExpandButton) dropDownMenuWidget.Children.FirstOrDefault(c =>
                        c is DropDownMenuExpandButton &&
                        ((DropDownMenuExpandButton) c).Text == label);

                    if (getGroundsChild == null)
                    {
                        getGroundsChild = dropDownWidgetExpander;
                        dropDownMenuWidget.AddDropDownMenu(getGroundsChild);
                    }

                    if (entry.Value.Nesting.Length < 2)
                    {
                        getGroundsChild.AddDropDownMenu(createNodeWidget);
                        continue;
                    }

                    var i = 1;
                    while (i < entry.Value.Nesting.Length)
                    {
                        label = entry.Value.Nesting[i];

                        if (getGroundsChild.Children.FirstOrDefault(child => child is DropDownMenuWidget) != null)
                            dropDownWidgetExpander = (DropDownMenuExpandButton) getGroundsChild.Children
                                .FirstOrDefault(child => child is DropDownMenuWidget).Children.FirstOrDefault(c =>
                                    c is DropDownMenuExpandButton &&
                                    ((DropDownMenuExpandButton) c).Text == label);

                        if (dropDownWidgetExpander == null)
                        {
                            dropDownWidgetExpander =
                                new DropDownMenuExpandButton(nodeScriptContainerWidget.ModData,
                                    new Rectangle(0, 0, 160, 25))
                                {
                                    Text = label
                                };
                            getGroundsChild.AddDropDownMenu(dropDownWidgetExpander);
                        }

                        getGroundsChild = dropDownWidgetExpander;

                        i++;
                    }

                    dropDownWidgetExpander.AddDropDownMenu(createNodeWidget);
                }
            }

            return dropDownMenuWidget;
        }
    }
}