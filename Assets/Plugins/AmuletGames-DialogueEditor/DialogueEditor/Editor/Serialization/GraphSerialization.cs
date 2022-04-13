using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace AG
{
    public class GraphSerialization
    {
        [Header("Refs.")]
        private DialogueGraphView graphView;

        private List<Edge> edges;
        private List<BaseNode> nodes;

        public GraphSerialization(DialogueGraphView _graphView)
        {
            graphView = _graphView;
        }

        public void Save(DialContainerSO _dialogueContainerSO)
        {
            RefreshNodeEdgeList();

            SaveEdges(_dialogueContainerSO);
            SaveNodes(_dialogueContainerSO);

            EditorUtility.SetDirty(_dialogueContainerSO);
            AssetDatabase.SaveAssets();
        }

        public void Load(DialContainerSO _dialogueContainerSO)
        {
            RefreshNodeEdgeList();

            ClearGraph();

            LoadNodesFromSavables(_dialogueContainerSO);

            RefreshNodeList();

            LoadEdgesFromSavables(_dialogueContainerSO);

            void ClearGraph()
            {
                int edgesCount = edges.Count;
                for (int i = 0; i < edgesCount; i++)
                {
                    graphView.RemoveElement(edges[i]);
                }

                int nodesCount = nodes.Count;
                for (int i = 0; i < nodesCount; i++)
                {
                    graphView.RemoveElement(nodes[i]);
                }
            }
        }

        #region Refresh.
        void RefreshNodeEdgeList()
        {
            RefreshEdgeList();
            RefreshNodeList();
        }

        void RefreshNodeList()
        {
            nodes = graphView.nodes.ToList().Where(node => node is BaseNode).Cast<BaseNode>().ToList();
        }

        void RefreshEdgeList()
        {
            edges = graphView.edges.ToList();
        }
        #endregion

        #region Save Edges.
        void SaveEdges(DialContainerSO _dialogueContainerSO)
        {
            ClearSavableRecords();

            AddSavablesToRecords();

            void ClearSavableRecords()
            {
                _dialogueContainerSO.nodeEdgeDataSavables.Clear();
            }

            void AddSavablesToRecords()
            {
                Edge[] connectedEdges = edges.Where(edge => edge.input.node != null).ToArray();

                for (int i = 0; i < connectedEdges.Length; i++)
                {
                    BaseNode outputNode = (BaseNode)connectedEdges[i].output.node;
                    BaseNode inputNode = (BaseNode)connectedEdges[i].input.node;

                    _dialogueContainerSO.nodeEdgeDataSavables.Add(new NodeEdgeData
                    {
                        outputGuid = outputNode.nodeGuid,
                        inputGuid = inputNode.nodeGuid
                    });
                }
            }
        }
        #endregion

        #region Save Nodes.
        void SaveNodes(DialContainerSO _dialogueContainerSO)
        {
            ClearSavableRecords();

            AddSavablesToRecords();

            void ClearSavableRecords()
            {
                _dialogueContainerSO.startNodeDataSavables.Clear();
                _dialogueContainerSO.dialogueNodeDataSavables.Clear();
                _dialogueContainerSO.eventNodeDataSavables.Clear();
                _dialogueContainerSO.endNodeDataSavables.Clear();
            }

            void AddSavablesToRecords()
            {
                int baseNodesCount = nodes.Count;
                for (int i = 0; i < baseNodesCount; i++)
                {
                    switch (nodes[i])
                    {
                        case StartNode node:
                            _dialogueContainerSO.startNodeDataSavables.Add(SaveStartNodeData(node));
                            break;
                        case DialogueNode node:
                            _dialogueContainerSO.dialogueNodeDataSavables.Add(SaveDialogueNodeData(node));
                            break;
                        case EventNode node:
                            _dialogueContainerSO.eventNodeDataSavables.Add(SaveEventNodeData(node));
                            break;
                        case EndNode node:
                            _dialogueContainerSO.endNodeDataSavables.Add(SaveEndNodeData(node));
                            break;
                    }
                }
            }
        }

        StartNodeData SaveStartNodeData(StartNode _startNode)
        {
            StartNodeData startNodeData = new StartNodeData()
            {
                nodeGuid = _startNode.nodeGuid,
                position = _startNode.GetPosition().position
            };

            return startNodeData;
        }

        DialogueNodeData SaveDialogueNodeData(DialogueNode _dialogueNode)
        {
            DialogueNodeData dialogueNodeData;

            SaveDialogueNodeData();

            SaveEachChoiceData();

            return dialogueNodeData;

            void SaveDialogueNodeData()
            {
                dialogueNodeData = new DialogueNodeData();
                dialogueNodeData.nodeGuid = _dialogueNode.nodeGuid;
                dialogueNodeData.position = _dialogueNode.GetPosition().position;

                dialogueNodeData.speakerName = _dialogueNode.speakerName;
                dialogueNodeData.speakerSprite = _dialogueNode.speakerSprite;
                dialogueNodeData.avatarDirectionType = _dialogueNode.avatarDirectionType;
                dialogueNodeData.String_LGs = _dialogueNode.String_LGs;
                dialogueNodeData.AudioClip_LGs = _dialogueNode.AudioClip_LGs;

                dialogueNodeData.choiceDataList = new List<ChoiceData>(_dialogueNode.choiceDataList);
            }

            void SaveEachChoiceData()
            {
                int choiceDataCount = dialogueNodeData.choiceDataList.Count;
                int edgesCount = edges.Count;

                for (int i = 0; i < choiceDataCount; i++)
                {
                    ChoiceData _choiceData = dialogueNodeData.choiceDataList[i];

                    _choiceData.outputGuid = string.Empty;
                    _choiceData.inputGuid = string.Empty;

                    for (int j = 0; j < edgesCount; j++)
                    {
                        if (edges[j].output.name == _choiceData.dataGuid)
                        {
                            _choiceData.outputGuid = (edges[j].output.node as BaseNode).nodeGuid;
                            _choiceData.inputGuid = (edges[j].input.node as BaseNode).nodeGuid;
                        }
                    }
                }
            }
        }

        EventNodeData SaveEventNodeData(EventNode _eventNode)
        {
            EventNodeData eventNodeData = new EventNodeData()
            {
                nodeGuid = _eventNode.nodeGuid,
                position = _eventNode.GetPosition().position,

                stringEventAddons = _eventNode.stringEventAddons,
                scriptableEventAddons = _eventNode.scriptableEventAddons
            };

            return eventNodeData;
        }

        EndNodeData SaveEndNodeData(EndNode _endNode)
        {
            EndNodeData endNodeData = new EndNodeData()
            {
                nodeGuid = _endNode.nodeGuid,
                position = _endNode.GetPosition().position,
                endNodeType = _endNode.endNodeType
            };

            return endNodeData;
        }
        #endregion

        #region Load Nodes.
        void LoadNodesFromSavables(DialContainerSO _containerSO)
        {
            LoadStartNode();

            LoadDialogueNode();

            LoadEventNode();

            LoadEndNode();

            void LoadStartNode()
            {
                int startNodeSavableCount = _containerSO.startNodeDataSavables.Count;
                for (int i = 0; i < startNodeSavableCount; i++)
                {
                    graphView.LoadStartNode(_containerSO.startNodeDataSavables[i]);
                }
            }

            void LoadDialogueNode()
            {
                int dialogueNodeSavableCount = _containerSO.dialogueNodeDataSavables.Count;
                for (int i = 0; i < dialogueNodeSavableCount; i++)
                {
                    graphView.LoadDialogueNode(_containerSO.dialogueNodeDataSavables[i]);
                }
            }

            void LoadEventNode()
            {
                int eventNodeSavableCount = _containerSO.eventNodeDataSavables.Count;
                for (int i = 0; i < eventNodeSavableCount; i++)
                {
                    graphView.LoadEventNode(_containerSO.eventNodeDataSavables[i]);
                }
            }

            void LoadEndNode()
            {
                int endNodeSavableCount = _containerSO.endNodeDataSavables.Count;
                for (int i = 0; i < endNodeSavableCount; i++)
                {
                    graphView.LoadEndNode(_containerSO.endNodeDataSavables[i]);
                }
            }
        }
        #endregion

        #region Load Edges.
        void LoadEdgesFromSavables(DialContainerSO _containerSO)
        {
            // GOAL: Link all nodes together.

            int nodesCount = nodes.Count;
            for (int i = 0; i < nodesCount; i++)
            {
                // If node IS NOT Dialogue Nodes. 
                if ((nodes[i] is DialogueNode) == false)
                {
                    List<NodeEdgeData> edges = _containerSO.nodeEdgeDataSavables.Where(edge => edge.outputGuid == nodes[i].nodeGuid).ToList();

                    for (int j = 0; j < edges.Count; j++)
                    {
                        BaseNode _1stInputNode = nodes.First(node => node.nodeGuid == edges[j].inputGuid);
                        LinkNodesToTogether(nodes[i].outputContainer[j].Q<Port>(), (Port)_1stInputNode.inputContainer[0]);
                    }
                }
                else
                {
                    // If node IS Dialogue Nodes.
                    DialogueNode dialogueNode = (DialogueNode)nodes[i];

                    for (int j = 0; j < dialogueNode.choiceDataList.Count; j++)
                    {
                        ChoiceData _choiceData = dialogueNode.choiceDataList[j];

                        // Check if port has a connection
                        if (_choiceData.inputGuid != string.Empty)
                        {
                            BaseNode _1stInputNode = nodes.First(node => node.nodeGuid == _choiceData.inputGuid);

                            Port _referedPort = null;

                            // Check all ports in nodes outputContainer.
                            for (int z = 0; z < dialogueNode.outputContainer.childCount; z++)
                            {
                                Debug.Log("dialogueNode.outputContainer.childCount = " + dialogueNode.outputContainer.childCount);
                                Debug.Log("dialogueNode.outputContainer.name" + z + " " + dialogueNode.outputContainer[z].Q<Port>().name);
                                Debug.Log("_choiceData.dataGuid" + z + " " + _choiceData.dataGuid);

                                // Find port with its port name has the same ID as the saved choice Data Guid ID.
                                if (dialogueNode.outputContainer[z].Q<Port>().name == _choiceData.dataGuid)
                                {
                                    _referedPort = dialogueNode.outputContainer[z].Q<Port>();
                                }
                            }

                            LinkNodesToTogether(_referedPort, (Port)_1stInputNode.inputContainer[0]);
                        }
                    }
                }
            }
        }

        void LinkNodesToTogether(Port _outputPort, Port _inputPort)
        {
            Edge edge = new Edge()
            {
                output = _outputPort,
                input = _inputPort
            };

            edge.output.Connect(edge);
            edge.input.Connect(edge);

            graphView.Add(edge);
        }
        #endregion
    }
}