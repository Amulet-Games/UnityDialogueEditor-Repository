using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace AG
{
    public class StartNode : BaseNode
    {
        [Header("Ports")]
        public Port outputPort;

        public StartNode()
        {
            // GOAL: To be able to find in the node search bar.
        }

        public StartNode(Vector2 _position, DialogueEditorWindow _editorWindow, DialogueGraphView _graphView)
        {
            SetupRefs();

            SetupNodeDetails();

            AddPorts();

            FinishNodeSetup();

            void SetupRefs()
            {
                editorWindow = _editorWindow;
                graphView = _graphView;
            }

            void SetupNodeDetails()
            {
                title = "Start";
                SetPosition(new Rect(_position, defaultNodeSize));
                nodeGuid = Guid.NewGuid().ToString();
            }

            void AddPorts()
            {
                outputPort = AddOutputPort("Output", Port.Capacity.Single, N_NodeTypeEnum.Start);
            }

            void FinishNodeSetup()
            {
                RefreshExpandedState();
                RefreshPorts();
            }
        }
    }
}