using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace AG
{
    public class BaseNode : Node
    {
        [Header("Config.")]
        protected Vector2 defaultNodeSize = new Vector2(200, 250);

        [Header("Status.")]
        [ReadOnlyInspector] public string nodeGuid;

        [Header("Refs.")]
        protected DialogueGraphView graphView;
        protected DialogueEditorWindow editorWindow;

        public BaseNode()
        {
            SetupBaseNodeStyle();
        }

        #region Virtual.
        public virtual void LoadValueIntoField()
        {
        }

        public virtual void ReloadLanguage()
        {
        }
        #endregion

        #region Ports.
        protected void AddOutputPort(string name, Port.Capacity capacity, N_NodeTypeEnum nodeType)
        {
            Port outputPort = GetPortInstance(Direction.Output, capacity);
            outputPort.portName = name;
            outputPort.portColor = GetPortColorByNodeType(nodeType);
            outputContainer.Add(outputPort);
        }

        protected void AddInputPort(string name, Port.Capacity capacity, N_NodeTypeEnum nodeType)
        {
            Port inputPort = GetPortInstance(Direction.Input, capacity);
            inputPort.portName = name;
            inputPort.portColor = GetPortColorByNodeType(nodeType);
            inputContainer.Add(inputPort);
        }

        protected Port GetPortInstance(Direction nodeDirection, Port.Capacity capacity)
        {
            return InstantiatePort(Orientation.Horizontal, nodeDirection, capacity, typeof(float));
        }

        protected Color GetPortColorByNodeType(N_NodeTypeEnum portColor)
        {
            switch (portColor)
            {
                case N_NodeTypeEnum.Start:
                case N_NodeTypeEnum.End:
                    return new Color(0.29f, 1, 0.34f);
                case N_NodeTypeEnum.Dialogue:
                    return new Color(1, 0.29f, 0.29f);
                case N_NodeTypeEnum.Event:
                    return new Color(0.89f, 0.29f, 1);
                default:
                    return new Color(0f, 0, 0f);
            }
        }
        #endregion

        #region Style Sheet.
        void SetupBaseNodeStyle()
        {
            styleSheets.Add(Resources.Load<StyleSheet>("NodeStyleSheet"));
        }
        #endregion
    }
}