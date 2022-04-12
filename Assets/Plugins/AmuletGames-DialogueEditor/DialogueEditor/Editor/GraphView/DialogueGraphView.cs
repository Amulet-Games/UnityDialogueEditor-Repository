using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace AG
{
    public class DialogueGraphView : GraphView
    {
        [Header("Refs.")]
        private DialogueEditorWindow editorWindow;
        private NodeSearchWindow searchWindow;

        [Header("USS.")]
        private string styleSheetName = "GraphViewStyleSheet";

        public DialogueGraphView(DialogueEditorWindow _editorWindow)
        {
            editorWindow = _editorWindow;

            Setup();
        }

        #region Callbacks.
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            // INHERIT: Graph View Class
            // GOAL: This is where we tell the graph view which nodes can connect to each other.

            List<Port> compatiblePorts = new List<Port>();

            ports.ForEach((port) =>
            {
                Port portView = port;

                // First we tell that it cannot connect to itself.
                // Then we tell it cannot connect to a port on the same node.
                // Lastly we tell it a input node cannot connect to another input node and an output node cannot connect to output node.
                if (startPort != portView && startPort.node != portView.node && startPort.direction != port.direction)
                {
                    compatiblePorts.Add(port);
                }
            });

            // return all the acceptable ports.
            return compatiblePorts;
        }
        #endregion

        #region Setup
        void Setup()
        {
            SetupEditorWindowStyle();

            SetupGraphZoom();

            SetupGraphManipulator();

            SetupGridBackground();

            AddSearchWindow();
        }

        void SetupGraphZoom()
        {
            // Default Min Scale = 0.25f;
            // Default Max Scale = 1f;
            SetupZoom(ContentZoomer.DefaultMinScale, 1.15f);
        }

        void SetupGraphManipulator()
        {
            this.AddManipulator(new ContentDragger());          // The ability to drag nodes around.
            this.AddManipulator(new SelectionDragger());        // The ability to drag all selected nodes around.
            this.AddManipulator(new RectangleSelector());       // The ability to drag select a rectangle area.
            this.AddManipulator(new FreehandSelector());        // The ability to select a single node.
        }

        void SetupGridBackground()
        {
            // GOAL: Add a visible grid to the background.

            GridBackground grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();
        }
        #endregion

        #region Language Reload.
        public void ReloadLanguage()
        {
            // GOAL: Reload the current selected language, used when changing to a new language

            List<BaseNode> allBaseNodes = nodes.ToList().Where(node => node is BaseNode).Cast<BaseNode>().ToList();

            int dialogueNodesCount = allBaseNodes.Count;
            for (int i = 0; i < dialogueNodesCount; i++)
            {
                allBaseNodes[i].ReloadLanguage();
            }
        }
        #endregion

        #region Create Search Window.
        void AddSearchWindow()
        {
            searchWindow = ScriptableObject.CreateInstance<NodeSearchWindow>();
            searchWindow.Configure(editorWindow, this);
            nodeCreationRequest = context => SearchWindow.Open
            (
                new SearchWindowContext(context.screenMousePosition), searchWindow
            );
        }
        #endregion

        #region Create Nodes.
        public void CreateStartNode(Vector2 position)
        {
            AddElement(new StartNode(position, editorWindow, this));
        }

        public void CreateDialogueNode(Vector2 position)
        {
            AddElement(new DialogueNode(position, editorWindow, this));
        }

        public void CreateEventNode(Vector2 position)
        {
            AddElement(new EventNode(position, editorWindow, this));
        }

        public void CreateEndNode(Vector2 position)
        {
            AddElement(new EndNode(position, editorWindow, this));
        }
        #endregion

        #region Load Nodes.
        public void LoadStartNode(StartNodeData startNodeData)
        {
            StartNode startNode = new StartNode(startNodeData.position, editorWindow, this);
            startNode.nodeGuid = startNodeData.nodeGuid;

            AddElement(startNode);
        }

        public void LoadDialogueNode(DialogueNodeData dialogueNodeData)
        {
            DialogueNode dialogueNode = new DialogueNode(dialogueNodeData.position, editorWindow, this);

            LoadDialogueNodeData();

            LoadEachChoiceData();

            void LoadDialogueNodeData()
            {
                dialogueNode.nodeGuid = dialogueNodeData.nodeGuid;
                dialogueNode.speakerName = dialogueNodeData.speakerName;
                dialogueNode.speakerSprite = dialogueNodeData.speakerSprite;
                dialogueNode.avatarDirectionType = dialogueNodeData.avatarDirectionType;

                int String_LG_Count = dialogueNodeData.String_LGs.Count;
                for (int i = 0; i < String_LG_Count; i++)
                {
                    LanguageGeneric<string> _string_lg = dialogueNodeData.String_LGs[i];
                    dialogueNode.String_LGs.Find(String_LG => String_LG.languageType == _string_lg.languageType).genericContent = _string_lg.genericContent;
                }

                int AudioClip_LG_Count = dialogueNodeData.AudioClip_LGs.Count;
                for (int i = 0; i < AudioClip_LG_Count; i++)
                {
                    LanguageGeneric<AudioClip> _audioClip_lg = dialogueNodeData.AudioClip_LGs[i];
                    dialogueNode.AudioClip_LGs.Find(AudioClip_LG => AudioClip_LG.languageType == _audioClip_lg.languageType).genericContent = _audioClip_lg.genericContent;
                }

                dialogueNode.LoadValueIntoField();
                AddElement(dialogueNode);
            }

            void LoadEachChoiceData()
            {
                int choiceDataCount = dialogueNodeData.choiceDataList.Count;
                for (int i = 0; i < choiceDataCount; i++)
                {
                    dialogueNode.AddChoicePort(dialogueNode, dialogueNodeData.choiceDataList[i]);
                }
            }
        }

        public void LoadEventNode(EventNodeData eventNodeData)
        {
            EventNode eventNode = new EventNode(eventNodeData.position, editorWindow, this);
            eventNode.nodeGuid = eventNodeData.nodeGuid;
            eventNode.dialogueEvent = eventNodeData.dialogueEvent;

            eventNode.LoadValueIntoField();
            AddElement(eventNode);
        }

        public void LoadEndNode(EndNodeData endNodeData)
        {
            EndNode endNode = new EndNode(endNodeData.position, editorWindow, this);
            endNode.nodeGuid = endNodeData.nodeGuid;
            endNode.endNodeType = endNodeData.endNodeType;

            endNode.LoadValueIntoField();

            AddElement(endNode);
        }
        #endregion

        #region Style Sheet.
        void SetupEditorWindowStyle()
        {
            styleSheets.Add(Resources.Load<StyleSheet>(styleSheetName));
        }
        #endregion
    }
}