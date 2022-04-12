using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace AG
{
    public class EventNode : BaseNode
    {
        [Header("Refs.")]
        public DialEventSO dialogueEvent;

        [Header("UI Element Refs.")]
        private ObjectField dialogueEvent_ObjectField;

        public EventNode()
        {
            // GOAL: To be able to find in the node search bar.
        }

        public EventNode(Vector2 _position, DialogueEditorWindow _editorWindow, DialogueGraphView _graphView)
        {
            SetupRefs();

            SetupNodeDetails();

            SetupObjectField();

            AddPorts();

            FinishNodeSetup();

            void SetupRefs()
            {
                editorWindow = _editorWindow;
                graphView = _graphView;
            }

            void SetupNodeDetails()
            {
                title = "Event";
                SetPosition(new Rect(_position, defaultNodeSize));
                nodeGuid = Guid.NewGuid().ToString();
            }

            void SetupObjectField()
            {
                dialogueEvent_ObjectField = new ObjectField()
                {
                    objectType = typeof(DialEventSO),
                    allowSceneObjects = false,
                    value = dialogueEvent
                };

                dialogueEvent_ObjectField.RegisterValueChangedCallback((value) =>
                {
                    dialogueEvent = dialogueEvent_ObjectField.value as DialEventSO;
                });

                dialogueEvent_ObjectField.SetValueWithoutNotify(dialogueEvent);

                mainContainer.Add(dialogueEvent_ObjectField);
            }

            void AddPorts()
            {
                AddInputPort("Input", Port.Capacity.Multi, N_NodeTypeEnum.Event);
                AddOutputPort("Output", Port.Capacity.Single, N_NodeTypeEnum.Event);
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
            dialogueEvent_ObjectField.SetValueWithoutNotify(dialogueEvent);
        }
        #endregion
    }
}