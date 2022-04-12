using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace AG
{
    public class EndNode : BaseNode
    {
        [Header("Config.")]
        public N_EndNodeTypeEnum endNodeType = N_EndNodeTypeEnum.End;

        [Header("UI Element Refs.")]
        private EnumField endNodeType_EnumField;

        public EndNode()
        {
            // GOAL: To be able to find in the node search bar.
        }

        public EndNode(Vector2 _position, DialogueEditorWindow _editorWindow, DialogueGraphView _graphView)
        {
            SetupRefs();

            SetupNodeDetails();

            SetupEnumField();

            AddPorts();

            FinishNodeSetup();

            void SetupRefs()
            {
                editorWindow = _editorWindow;
                graphView = _graphView;
            }

            void SetupNodeDetails()
            {
                title = "End";
                SetPosition(new Rect(_position, defaultNodeSize));
                nodeGuid = Guid.NewGuid().ToString();
            }

            void SetupEnumField()
            {
                endNodeType_EnumField = new EnumField()
                {
                    value = endNodeType
                };

                endNodeType_EnumField.Init(endNodeType);

                endNodeType_EnumField.RegisterValueChangedCallback((value) =>
                {
                    endNodeType = (N_EndNodeTypeEnum)value.newValue;
                });

                endNodeType_EnumField.SetValueWithoutNotify(endNodeType);

                mainContainer.Add(endNodeType_EnumField);
            }

            void AddPorts()
            {
                AddInputPort("Input", Port.Capacity.Multi, N_NodeTypeEnum.End);
            }

            void FinishNodeSetup()
            {
                RefreshExpandedState();
                RefreshPorts();
            }
        }

        #region Override.
        public override void LoadValueIntoField()
        {
            endNodeType_EnumField.SetValueWithoutNotify(endNodeType);
        }
        #endregion
    }
}