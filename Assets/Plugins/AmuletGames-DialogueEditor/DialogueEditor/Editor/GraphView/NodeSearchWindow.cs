using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace AG
{
    public class NodeSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        [Header("Refs.")]
        private DialogueEditorWindow editorWindow;
        private DialogueGraphView graphView;

        private Texture2D fakeEntryPic;

        public void Configure(DialogueEditorWindow _editorWindow, DialogueGraphView _graphView)
        {
            editorWindow = _editorWindow;
            graphView = _graphView;

            // Icon image that we kinda don't use.
            // However use it to create space left of the text.
            // TODO: find a better way.
            fakeEntryPic = new Texture2D(1, 1);
            fakeEntryPic.SetPixel(0, 0, new Color(0, 0, 0, 0));
            fakeEntryPic.Apply();
        }

        #region Callbacks.

        #region Create Search Tree.
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<SearchTreeEntry> searchTrees = new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent("Dialogue Editor"), 0),
                new SearchTreeGroupEntry(new GUIContent("Nodes"), 1),

                AddNodeSearch("Start Node", new StartNode()),
                AddNodeSearch("Dialogue Node", new DialogueNode()),
                AddNodeSearch("Event Node", new EventNode()),
                AddNodeSearch("Branch Node", new BranchNode()),
                AddNodeSearch("End Node", new EndNode())
            };

            return searchTrees;
        }

        SearchTreeEntry AddNodeSearch(string _name, BaseNode _baseNode)
        {
            SearchTreeEntry searchTree = new SearchTreeEntry(new GUIContent(_name, fakeEntryPic))
            {
                level = 2,
                userData = _baseNode
            };

            return searchTree;
        }
        #endregion

        #region Select Search Tree
        public bool OnSelectEntry(SearchTreeEntry _searchTreeEntry, SearchWindowContext _context)
        {
            // Get mouse position on the screen.
            Vector2 mousePosition = editorWindow.rootVisualElement.ChangeCoordinatesTo
            (
                editorWindow.rootVisualElement.parent, _context.screenMousePosition - editorWindow.position.position
            );

            // Now we use mouse position to calculate where it is in the graph view
            Vector2 graphMousePosition = graphView.contentViewContainer.WorldToLocal(mousePosition);

            return CreateNodeByType(_searchTreeEntry, graphMousePosition);
        }
        
        bool CreateNodeByType(SearchTreeEntry _searchTreeEntry, Vector2 _pos)
        {
            switch (_searchTreeEntry.userData)
            {
                case StartNode node:
                    graphView.CreateStartNode(_pos);
                    return true;
                case DialogueNode node:
                    graphView.CreateDialogueNode(_pos);
                    return true;
                case EventNode node:
                    graphView.CreateEventNode(_pos);
                    return true;
                case BranchNode node:
                    graphView.CreateBranchNode(_pos);
                    return true;
                case EndNode node:
                    graphView.CreateEndNode(_pos);
                    return true;
            }

            return false;
        }
        #endregion

        #endregion
    }
}