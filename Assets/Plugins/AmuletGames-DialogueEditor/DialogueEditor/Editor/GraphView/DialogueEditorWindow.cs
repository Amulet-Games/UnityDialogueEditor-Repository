using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace AG
{
    public class DialogueEditorWindow : EditorWindow
    {
        [Header("Config.")]
        public G_LanguageTypeEnum selectedLanguage = G_LanguageTypeEnum.English;                    /// Current selected language in the dialogue editor window.

        [Header("Refs.")]
        private DialContainerSO containerSO;                                                        /// Current open dialogue container in dialogue editor window.
        private DialogueGraphView graphView;                                                        /// Reference to GraphView Class.
        private GraphSerialization graphSerialization;                                              /// Reference to Save and Load Functions.

        public G_LanguageTypeEnum[] supportLanguageTypes;
        public int supportLanguageLength;

        [Header("UI Element Refs.")]
        private ToolbarMenu languageDropdown;
        private Label dialogueContainerLabel;                                                       /// Name of the current open dialogue container.

        [Header("USS.")]
        private string graphViewStyleSheet = "EditorWindowStyleSheet";
        private string dialogueContainerLabelSelector = "dialogueContainerLabelSelector";

        [Header("String Builder.")]
        private StringBuilder strBuilder;

        #region Callbacks.
        private void OnEnable()
        {
            SetupStringBuilder();

            SetupLanguageTypeArray();

            SetupEditorWindowStyle();

            ConstructGraphView();

            GenerateToolbar();

            SetupGraphSerialization();

            Load();
        }

        private void OnDisable()
        {
            DestructGraphView();
        }
        
        /// Callback attribute for opening an asset in Unity (e.g the callback is fired when double clicking an asset in the Project Browser).
        /// Read More https://docs.unity3d.com/ScriptReference/Callbacks.OnOpenAssetAttribute.html
        [OnOpenAsset(0)]
        public static bool ShowWindow(int instanceId, int line)
        {
            // GOAL: Opens up a editor window when double clicked a dialogue container asset.

            // Find unity object with this instanceId and load it in.
            Object item = EditorUtility.InstanceIDToObject(instanceId);
            if (item is DialContainerSO)
            {
                DialogueEditorWindow window = (DialogueEditorWindow)GetWindow(typeof(DialogueEditorWindow));

                window.titleContent = new GUIContent("Dialogue Editor");
                window.containerSO = item as DialContainerSO;
                window.minSize = new Vector2(400, 400);
                window.Load();
            }

            return false;
        }
        #endregion

        #region Editors.
        void ConstructGraphView()
        {
            // GOAL: Construct the editor window's graph

            graphView = new DialogueGraphView(this);
            graphView.StretchToParentSize();
            rootVisualElement.Add(graphView);
        }

        void DestructGraphView()
        {
            rootVisualElement.Remove(graphView);
        }

        void GenerateToolbar()
        {
            // GOAL: Create the toolbar you will see in the top left of the dialogue editor window.

            Toolbar toolbar = new Toolbar();

            // Save button
            CreateSaveButton();

            // Load button
            CreateLoadButton();

            // Language dropdown
            CreateLanguageDropdown();

            // Container name
            CreateContainerLabel();

            // Add completed toolbar to editor window
            rootVisualElement.Add(toolbar);

            void CreateSaveButton()
            {
                Button saveBtn = new Button()
                {
                    text = "Save",
                };
                saveBtn.clicked += Save;

                toolbar.Add(saveBtn);
            }

            void CreateLoadButton()
            {
                Button loadBtn = new Button()
                {
                    text = "Load",
                };
                loadBtn.clicked += Load;

                toolbar.Add(loadBtn);
            }

            void CreateLanguageDropdown()
            {
                languageDropdown = new ToolbarMenu();

                // Here we go through each language and make a button with that language.
                // When you click on the language in the dropdown menu we tell it to run the action's method.
                foreach (G_LanguageTypeEnum language in (G_LanguageTypeEnum[])Enum.GetValues(typeof(G_LanguageTypeEnum)))
                {
                    languageDropdown.menu.AppendAction(language.ToString(), new Action<DropdownMenuAction>(x => Language(language)));
                }

                toolbar.Add(languageDropdown);
            }

            void CreateContainerLabel()
            {
                dialogueContainerLabel = new Label("");
                toolbar.Add(dialogueContainerLabel);

                // USS
                AddSelector_DialogueContainerLabel();
            }
        }
        #endregion

        #region Load / Save.
        void SetupGraphSerialization()
        {
            graphSerialization = new GraphSerialization(graphView);
        }

        void Save()
        {
            // GOAL: Call the Save function in graph Serialization
            
            if (containerSO != null)
            {
                graphSerialization.Save(containerSO);
            }
        }

        void Load()
        {
            // GOAL: Load in dialogue container's data to edit window's info.

            if (containerSO != null)
            {
                Language(selectedLanguage);

                SetContainerLabelText();

                graphSerialization.Load(containerSO);
            }

            void SetContainerLabelText()
            {
                dialogueContainerLabel.text = Build_ContainerLabelName();
            }
        }
        #endregion

        #region Language.
        void SetupLanguageTypeArray()
        {
            supportLanguageTypes = (G_LanguageTypeEnum[])Enum.GetValues(typeof(G_LanguageTypeEnum));
            supportLanguageLength = supportLanguageTypes.Length;
        }

        void Language(G_LanguageTypeEnum language)
        {
            // GOAL: Change the current editor window language to desired one.

            languageDropdown.text = Build_LanguageDropdownText(language);
            selectedLanguage = language;
            graphView.ReloadLanguage();
        }
        #endregion

        #region Style Sheet.
        void SetupEditorWindowStyle()
        {
            rootVisualElement.styleSheets.Add(Resources.Load<StyleSheet>(graphViewStyleSheet));
        }

        void AddSelector_DialogueContainerLabel()
        {
            dialogueContainerLabel.AddToClassList(dialogueContainerLabelSelector);
        }
        #endregion

        #region String Builder.

        #region Build.
        string Build_ContainerLabelName()
        {
            strBuilder.Clear();
            strBuilder.Append("Name:   ").Append(containerSO.name);
            return strBuilder.ToString();
        }

        string Build_LanguageDropdownText(G_LanguageTypeEnum language)
        {
            strBuilder.Clear();
            strBuilder.Append("Language: ").Append(language.ToString());
            return strBuilder.ToString();
        }

        public string Build_ChoicePortName(int outputPortCount)
        {
            strBuilder.Clear();
            strBuilder.Append("Choice ").Append(outputPortCount);
            return strBuilder.ToString();
        }
        #endregion

        #region Setup.
        void SetupStringBuilder()
        {
            if (strBuilder == null)
                strBuilder = new StringBuilder();
        }
        #endregion

        #endregion
    }
}