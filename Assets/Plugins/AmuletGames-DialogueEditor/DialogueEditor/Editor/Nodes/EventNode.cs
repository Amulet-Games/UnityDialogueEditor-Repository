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
        [Header("Ports")]
        public Port inputPort;
        public Port outputPort;

        [Header("Events")]
        public List<StringEventAddon> stringEventAddons = new List<StringEventAddon>();
        public List<ScriptableEventAddon> scriptableEventAddons = new List<ScriptableEventAddon>();

        [Header("USS.")]
        private string eventNode_Box_Container = "eventNode_Box_Container";
        private string eventNode_StringEvent_Text = "eventNode_StringEvent_Text";
        private string eventNode_StringEvent_Int = "eventNode_StringEvent_Int";
        private string eventNode_ScriptableEvent_Object = "eventNode_ScriptableEvent_Object";
        private string eventNode_Event_RemoveBtn = "eventNode_Event_RemoveBtn";

        public EventNode()
        {
            // GOAL: To be able to find in the node search bar.
        }

        public EventNode(Vector2 _position, DialogueEditorWindow _editorWindow, DialogueGraphView _graphView)
        {
            SetupRefs();

            SetupNodeDetails();

            SetupToolbarMenu();

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

            void SetupToolbarMenu()
            {
                ToolbarMenu dropdownMenu = new ToolbarMenu();
                dropdownMenu.text = "Add Event";

                dropdownMenu.menu.AppendAction("String ID", new Action<DropdownMenuAction>(x => AddStringEvent()));
                dropdownMenu.menu.AppendAction("Scriptable Object", new Action<DropdownMenuAction>(x => AddScriptableEvent()));

                titleContainer.Add(dropdownMenu);
            }

            void AddPorts()
            {
                inputPort = AddInputPort("Input", Port.Capacity.Multi, N_NodeTypeEnum.Event);
                outputPort = AddOutputPort("Output", Port.Capacity.Single, N_NodeTypeEnum.Event);
            }

            void FinishNodeSetup()
            {
                RefreshExpandedState();
                RefreshPorts();
            }
        }

        #region Toolbar Actions.
        public void AddStringEvent(StringEventAddon _savedStringEventAddon = null)
        {
            StringEventAddon newStringEventAddon;

            Box boxContainer;
            TextField textField;
            IntegerField intField;
            Button deleteBtn;

            // Create a new string event addon no matter if the event node is created from save or not.
            CreateStringEventAddon();

            // Container of all fields.
            CreateContainerForFields();

            // Create field elements.
            SetupTextField();

            SetupIntegerField();

            // Create delte event button.
            SetupButton_DeleteEvent();

            // Add them to container.
            AddFieldsToContainer();

            // Add this container to the main container.
            AddBoxToMainContainer();

            // Refresh this changes visually.
            RefreshExpandedState();

            void CreateStringEventAddon()
            {
                newStringEventAddon = new StringEventAddon();

                // If this string event is created from a saved event node.
                if (_savedStringEventAddon != null)
                {
                    newStringEventAddon.stringText = _savedStringEventAddon.stringText;
                    newStringEventAddon.intText = _savedStringEventAddon.intText;
                }

                stringEventAddons.Add(newStringEventAddon);
            }

            void CreateContainerForFields()
            {
                boxContainer = new Box();
                boxContainer.AddToClassList(eventNode_Box_Container);
            }

            void SetupTextField()
            {
                textField = new TextField();
                textField.AddToClassList(eventNode_StringEvent_Text);

                textField.RegisterValueChangedCallback(value =>
                {
                    newStringEventAddon.stringText = value.newValue;
                });
                textField.SetValueWithoutNotify(newStringEventAddon.stringText);
            }

            void SetupIntegerField()
            {
                intField = new IntegerField();
                intField.AddToClassList(eventNode_StringEvent_Int);

                intField.RegisterValueChangedCallback(value =>
                {
                    newStringEventAddon.intText = value.newValue;
                });
                intField.SetValueWithoutNotify(newStringEventAddon.intText);
            }

            void SetupButton_DeleteEvent()
            {
                deleteBtn = new Button()
                {
                    text = "X"
                };

                deleteBtn.clicked += () =>
                {
                    stringEventAddons.Remove(newStringEventAddon);
                    DeleteEventBoxContainer(boxContainer);
                };
                
                deleteBtn.AddToClassList(eventNode_Event_RemoveBtn);
            }

            void AddFieldsToContainer()
            {
                boxContainer.Add(textField);
                boxContainer.Add(intField);
                boxContainer.Add(deleteBtn);
            }

            void AddBoxToMainContainer()
            {
                mainContainer.Add(boxContainer);
            }
        }

        public void AddScriptableEvent(ScriptableEventAddon _savedScriptableEventAddon = null)
        {
            ScriptableEventAddon newScriptableEventAddon;

            Box boxContainer;
            ObjectField objectField;
            Button deleteBtn;

            CreateScriptableEventAddon();

            // Container of object field.
            CreateContainerForFields();

            // Create scriptable object field.
            SetupObjectField();

            // Create remove event button.
            SetupButton_DeleteEvent();

            AddFieldsToContainer();

            // Refresh this changes visually.
            RefreshExpandedState();

            void CreateScriptableEventAddon()
            {
                newScriptableEventAddon = new ScriptableEventAddon();

                // If this scriptable event is created from a saved event node.
                if (_savedScriptableEventAddon != null)
                {
                    newScriptableEventAddon.dialEventSO = _savedScriptableEventAddon.dialEventSO;
                }

                scriptableEventAddons.Add(newScriptableEventAddon);
            }

            void CreateContainerForFields()
            {
                boxContainer = new Box();
                boxContainer.AddToClassList(eventNode_Box_Container);
            }

            void SetupObjectField()
            {
                objectField = new ObjectField();
                objectField.objectType = typeof(DialEventSO);
                objectField.allowSceneObjects = false;
                objectField.value = null;
                objectField.AddToClassList(eventNode_ScriptableEvent_Object);

                objectField.RegisterValueChangedCallback(value =>
                {
                    newScriptableEventAddon.dialEventSO = value.newValue as DialEventSO;
                });
                objectField.SetValueWithoutNotify(newScriptableEventAddon.dialEventSO);
            }

            void SetupButton_DeleteEvent()
            {
                deleteBtn = new Button()
                {
                    text = "X"
                };

                deleteBtn.clicked += () =>
                {
                    scriptableEventAddons.Remove(newScriptableEventAddon);
                    DeleteEventBoxContainer(boxContainer);
                };

                deleteBtn.AddToClassList(eventNode_Event_RemoveBtn);
            }

            void AddFieldsToContainer()
            {
                // Add it to the container.
                boxContainer.Add(objectField);
                boxContainer.Add(deleteBtn);

                // Add this container to the main Container.
                mainContainer.Add(boxContainer);
            }
        }

        void DeleteEventBoxContainer(Box boxContainer)
        {
            // Remove the box container from the main container
            mainContainer.Remove(boxContainer);
            
            // Refresh this changes visually.
            RefreshExpandedState();
        }
        #endregion
    }
}