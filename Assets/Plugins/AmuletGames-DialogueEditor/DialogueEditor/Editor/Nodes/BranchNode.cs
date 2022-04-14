using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace AG
{
    public class BranchNode : BaseNode
    {
        [Header("Ports")]
        public Port inputPort;
        public Port trueOutputPort;
        public Port falseOutputPort;

        [Header("Addons")]
        public List<StringConditionAddon> stringConditionAddons = new List<StringConditionAddon>();

        [Header("USS.")]
        private string branchNode_BoxContainer = "branchNode_BoxContainer";
        private string branchNode_StringCondition_Text = "branchNode_StringCondition_Text";
        private string branchNode_StringCondition_Int = "branchNode_StringCondition_Int";
        private string branchNode_Condition_RemoveBtn = "branchNode_Condition_RemoveBtn";

        public BranchNode()
        {
            // GOAL: To be able to find in the node search bar.
        }

        public BranchNode(Vector2 _position, DialogueEditorWindow _editorWindow, DialogueGraphView _graphView)
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
                title = "Branch";
                SetPosition(new Rect(_position, defaultNodeSize));
                nodeGuid = Guid.NewGuid().ToString();
            }

            void SetupToolbarMenu()
            {
                ToolbarMenu dropdownMenu = new ToolbarMenu();
                dropdownMenu.text = "Add Condition";

                dropdownMenu.menu.AppendAction("String Condition", new Action<DropdownMenuAction>(x => AddStringCondition()));

                titleContainer.Add(dropdownMenu);
            }

            void AddPorts()
            {
                inputPort = AddInputPort("Input", Port.Capacity.Multi, N_NodeTypeEnum.Branch);
                trueOutputPort = AddOutputPort("True", Port.Capacity.Single, N_NodeTypeEnum.Branch);
                falseOutputPort = AddOutputPort("False", Port.Capacity.Single, N_NodeTypeEnum.Branch);
            }

            void FinishNodeSetup()
            {
                RefreshExpandedState();
                RefreshPorts();
            }
        }

        #region Toolbar Actions.
        public void AddStringCondition(StringConditionAddon savedStringConditionAddon = null)
        {
            StringConditionAddon newStringConditionAddon;

            Box boxContainer;
            TextField textField;
            IntegerField intField;
            Button deleteBtn;

            // Create a new string condition addon no matter if the event node is created from save or not.
            CreateStringConditionAddon();
            
            CreateContainerForFields();

            // Create field elements.
            SetupTextField();

            // Container of all fields.
            SetupIntegerField();

            // Create delte event button.
            SetupButton_DeleteCondition();

            // Add them to container.
            AddFieldsToContainer();

            // Add this container to the main container.
            AddBoxToMainContainer();

            // Refresh this changes visually.
            RefreshExpandedState();

            void CreateStringConditionAddon()
            {
                newStringConditionAddon = new StringConditionAddon();

                // If this string condition is created from a saved branch node.
                if (savedStringConditionAddon != null)
                {
                    newStringConditionAddon.stringText = savedStringConditionAddon.stringText;
                    newStringConditionAddon.intText = savedStringConditionAddon.intText;
                }

                stringConditionAddons.Add(newStringConditionAddon);
            }

            void CreateContainerForFields()
            {
                boxContainer = new Box();
                boxContainer.AddToClassList(branchNode_BoxContainer);
            }

            void SetupTextField()
            {
                textField = new TextField();
                textField.AddToClassList(branchNode_StringCondition_Text);

                textField.RegisterValueChangedCallback(value =>
                {
                    newStringConditionAddon.stringText = value.newValue;
                });
                textField.SetValueWithoutNotify(newStringConditionAddon.stringText);
            }

            void SetupIntegerField()
            {
                intField = new IntegerField();
                intField.AddToClassList(branchNode_StringCondition_Int);

                intField.RegisterValueChangedCallback(value =>
                {
                    newStringConditionAddon.intText = value.newValue;
                });
                intField.SetValueWithoutNotify(newStringConditionAddon.intText);
            }

            void SetupButton_DeleteCondition()
            {
                deleteBtn = new Button()
                {
                    text = "X"
                };

                deleteBtn.clicked += () =>
                {
                    stringConditionAddons.Remove(newStringConditionAddon);
                    DeleteConditionBoxContainer(boxContainer);
                };

                deleteBtn.AddToClassList(branchNode_Condition_RemoveBtn);
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

        void DeleteConditionBoxContainer(Box boxContainer)
        {
            // Remove the box container from the main container.
            mainContainer.Remove(boxContainer);

            // Refresh this changes visually.
            RefreshExpandedState();
        }
        #endregion
    }
}