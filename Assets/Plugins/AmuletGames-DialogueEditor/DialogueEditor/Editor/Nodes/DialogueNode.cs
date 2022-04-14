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
    public class DialogueNode : BaseNode
    {
        [Header("Refs.")]
        public string speakerName = "";
        public Sprite speakerSprite;
        public N_AvatarDirectionTypeEnum avatarDirectionType;

        public List<ChoiceData> choiceDataList = new List<ChoiceData>();

        [Header("Language Generic Refs.")]
        public List<LanguageGeneric<string>> String_LGs = new List<LanguageGeneric<string>>();
        public List<LanguageGeneric<AudioClip>> AudioClip_LGs = new List<LanguageGeneric<AudioClip>>();

        [Header("Ports")]
        public Port inputPort;

        [Header("UI Element Refs.")]
        private ObjectField audioClips_ObjectField;
        private ObjectField avatar_ObjectField;

        private Image avatar_ImageField;

        private TextField name_TextField;
        private TextField texts_TextField;

        private EnumField avatarDirectionType_EnumField;

        [Header("USS.")]
        private string dialNode_Label = "dialNode_Label";

        private string dialNode_Avatar_Preview_Image = "dialNode_Avatar_Preview_Image";

        private string dialNode_Name_Label_Selector = "dialNode_Name_Label_Selector";
        private string dialNode_Texts_Label_Selector = "dialNode_Texts_Label_Selector";

        private string dialNode_Name_TextField_Selector = "dialNode_Name_TextField_Selector";
        private string dialNode_Texts_TextField_Selector = "dialNode_Texts_TextField_Selector";

        public DialogueNode()
        {
            // GOAL: To be able to find in the node search bar.
        }

        public DialogueNode(Vector2 _position, DialogueEditorWindow _editorWindow, DialogueGraphView _graphView)
        {
            SetupRefs();

            SetupNodeDetails();

            SetupLanguageGenerics();

            Setup_Avatar_ObjectField_ImageField();

            Setup_AudioClips_ObjectField();

            SetupEnumField();

            SetupTextFields();

            SetupButton_AddChoice();

            AddPorts();

            FinishNodeSetup();

            void SetupRefs()
            {
                editorWindow = _editorWindow;
                graphView = _graphView;
            }

            void SetupNodeDetails()
            {
                title = "Dialogue";
                SetPosition(new Rect(_position, defaultNodeSize));
                nodeGuid = Guid.NewGuid().ToString();
            }

            void SetupLanguageGenerics()
            {
                for (int i = 0; i < editorWindow.supportLanguageLength; i++)
                {
                    String_LGs.Add(new LanguageGeneric<string>
                    {
                        languageType = editorWindow.supportLanguageTypes[i],
                        genericContent = ""
                    });

                    AudioClip_LGs.Add(new LanguageGeneric<AudioClip>
                    {
                        languageType = editorWindow.supportLanguageTypes[i],
                        genericContent = null
                    });
                }
            }

            void Setup_Avatar_ObjectField_ImageField()
            {
                // Object Field
                avatar_ObjectField = new ObjectField()
                {
                    objectType = typeof(Sprite),
                    allowSceneObjects = false,
                    value = speakerSprite
                };

                // Image Field
                avatar_ImageField = new Image();
                avatar_ImageField.AddToClassList(dialNode_Avatar_Preview_Image);

                avatar_ObjectField.RegisterValueChangedCallback((value) =>
                {
                    // Object Field
                    Sprite newSprite = value.newValue as Sprite;
                    speakerSprite = newSprite;

                    // Image Field
                    avatar_ImageField.image = newSprite != null ? newSprite.texture : null;
                });

                // Image Field
                mainContainer.Add(avatar_ImageField);

                // Object Field
                mainContainer.Add(avatar_ObjectField);
            }

            void Setup_AudioClips_ObjectField()
            {
                AudioClip curLanguageAudio = AudioClip_LGs.Find(AudioClip_LG => AudioClip_LG.languageType == editorWindow.selectedLanguage).genericContent;

                audioClips_ObjectField = new ObjectField()
                {
                    objectType = typeof(AudioClip),
                    allowSceneObjects = false,
                    value = curLanguageAudio
                };

                audioClips_ObjectField.RegisterValueChangedCallback((value) =>
                {
                    AudioClip_LGs.Find(AudioClip_LG => AudioClip_LG.languageType == editorWindow.selectedLanguage).genericContent = value.newValue as AudioClip;
                });

                audioClips_ObjectField.SetValueWithoutNotify(curLanguageAudio);

                mainContainer.Add(audioClips_ObjectField);
            }

            void SetupEnumField()
            {
                avatarDirectionType_EnumField = new EnumField()
                {
                    value = avatarDirectionType
                };

                avatarDirectionType_EnumField.Init(avatarDirectionType);

                avatarDirectionType_EnumField.RegisterValueChangedCallback((value) =>
                {
                    avatarDirectionType = (N_AvatarDirectionTypeEnum)value.newValue;
                });

                avatarDirectionType_EnumField.SetValueWithoutNotify(avatarDirectionType);

                mainContainer.Add(avatarDirectionType_EnumField);
            }

            void SetupTextFields()
            {
                Setup_Name_Label();

                Setup_Name_TextField();

                Setup_Texts_Label();

                Setup_Texts_TextField();

                void Setup_Name_Label()
                {
                    Label name_label = new Label("Name");
                    name_label.AddToClassList(dialNode_Name_Label_Selector);
                    name_label.AddToClassList(dialNode_Label);
                    mainContainer.Add(name_label);
                }

                void Setup_Name_TextField()
                {
                    name_TextField = new TextField("");
                    name_TextField.RegisterValueChangedCallback((value) =>
                    {
                        speakerName = value.newValue;
                    });

                    name_TextField.SetValueWithoutNotify(speakerName);

                    // USS
                    name_TextField.AddToClassList(dialNode_Name_TextField_Selector);

                    mainContainer.Add(name_TextField);
                }

                void Setup_Texts_Label()
                {
                    Label texts_label = new Label("Text Box");
                    texts_label.AddToClassList(dialNode_Texts_Label_Selector);
                    texts_label.AddToClassList(dialNode_Label);
                    mainContainer.Add(texts_label);
                }

                void Setup_Texts_TextField()
                {
                    texts_TextField = new TextField("");
                    texts_TextField.RegisterValueChangedCallback((value) =>
                    {
                        String_LGs.Find(String_LG => String_LG.languageType == editorWindow.selectedLanguage).genericContent = value.newValue;
                    });

                    texts_TextField.SetValueWithoutNotify(String_LGs.Find(String_LG => String_LG.languageType == editorWindow.selectedLanguage).genericContent);

                    // Allow to separte dialogue from single line into multiple lines.
                    texts_TextField.multiline = true;

                    // USS
                    texts_TextField.AddToClassList(dialNode_Texts_TextField_Selector);

                    mainContainer.Add(texts_TextField);
                }
            }

            void SetupButton_AddChoice()
            {
                Button addButton = new Button(() => AddChoicePort(this))
                {
                    text = "Add Choice"
                };

                titleButtonContainer.Add(addButton);
            }

            void AddPorts()
            {
                inputPort = AddInputPort("Input", Port.Capacity.Multi, N_NodeTypeEnum.Dialogue);
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
            audioClips_ObjectField.SetValueWithoutNotify
                (
                    AudioClip_LGs.Find(AudioClip_LG => AudioClip_LG.languageType == editorWindow.selectedLanguage).genericContent
                );

            if (speakerSprite != null)
            {
                avatar_ImageField.image = speakerSprite.texture;
            }

            avatar_ObjectField.SetValueWithoutNotify(speakerSprite);

            texts_TextField.SetValueWithoutNotify
                (
                    String_LGs.Find(String_LG => String_LG.languageType == editorWindow.selectedLanguage).genericContent
                );

            name_TextField.SetValueWithoutNotify(speakerName);

            avatarDirectionType_EnumField.SetValueWithoutNotify(avatarDirectionType);
        }

        public override void ReloadLanguage()
        {
            RefreshTexts();

            RefreshAudio();

            RefreshChoiceData_All();

            void RefreshTexts()
            {
                string curLanguageText = String_LGs.Find(String_LG => String_LG.languageType == editorWindow.selectedLanguage).genericContent;

                texts_TextField.RegisterValueChangedCallback((value) =>
                {
                    curLanguageText = value.newValue;
                });

                texts_TextField.SetValueWithoutNotify(curLanguageText);
            }

            void RefreshAudio()
            {
                AudioClip curLanguageAudio = AudioClip_LGs.Find(AudioClip_LG => AudioClip_LG.languageType == editorWindow.selectedLanguage).genericContent;

                audioClips_ObjectField.RegisterValueChangedCallback((value) =>
                {
                    curLanguageAudio = value.newValue as AudioClip;
                });

                audioClips_ObjectField.SetValueWithoutNotify(curLanguageAudio);
            }

            void RefreshChoiceData_All()
            {
                int choiceDataCount = choiceDataList.Count;
                for (int i = 0; i < choiceDataCount; i++)
                {
                    // Refresh choice data text field.
                    string curLanguageContent = choiceDataList[i].String_LGs.Find(String_LG => String_LG.languageType == editorWindow.selectedLanguage).genericContent;

                    choiceDataList[i].textField.RegisterValueChangedCallback((value) =>
                    {
                        curLanguageContent = value.newValue;
                    });

                    choiceDataList[i].textField.SetValueWithoutNotify(curLanguageContent);
                }
            }
        }
        #endregion

        public Port AddChoicePort(BaseNode _baseNode, ChoiceData _savedChoiceData = null)
        {
            ChoiceData choiceData;
            Port choicePort;

            int outputPortsCount;
            string choicePortName;

            CreateChoiceData();

            CreateChoicePort();

            SetupChoiceDataLanguageGenerics();

            SetupChoiceDataDetails();

            SetupChoiceDataTextField();

            SetupButton_DeleteChoice();

            FinishPortSetup();

            return choicePort;

            void CreateChoiceData()
            {
                choiceData = new ChoiceData();
                choiceData.dataGuid = Guid.NewGuid().ToString();
                choiceDataList.Add(choiceData);
            }

            void CreateChoicePort()
            {
                // Choice port's name needs to be the same as choice data's data guid.

                choicePort = GetPortInstance(Direction.Output, Port.Capacity.Single);
                choicePort.portName = "";
                choicePort.name = choiceData.dataGuid;
                choicePort.portColor = GetPortColorByNodeType(N_NodeTypeEnum.Dialogue);
                _baseNode.outputContainer.Add(choicePort);
            }

            void SetupChoiceDataLanguageGenerics()
            {
                outputPortsCount = _baseNode.outputContainer.Query("connector").ToList().Count;
                choicePortName = editorWindow.Build_ChoicePortName(outputPortsCount);

                for (int i = 0; i < editorWindow.supportLanguageLength; i++)
                {
                    choiceData.String_LGs.Add(new LanguageGeneric<string>()
                    {
                        languageType = editorWindow.supportLanguageTypes[i],
                        genericContent = choicePortName
                    });
                }
            }

            void SetupChoiceDataDetails()
            {
                if (_savedChoiceData != null)
                {
                    choiceData.outputGuid = _savedChoiceData.outputGuid;
                    choiceData.inputGuid = _savedChoiceData.inputGuid;
                    choiceData.dataGuid = _savedChoiceData.dataGuid;
                    choicePort.name = choiceData.dataGuid;                              // Choice port's name needs to be the same as choice data's data guid.

                    for (int i = 0; i < _savedChoiceData.String_LGs.Count; i++)
                    {
                        choiceData.String_LGs.Find
                        (
                            String_LG => String_LG.languageType == _savedChoiceData.String_LGs[i].languageType
                        )
                        .genericContent = _savedChoiceData.String_LGs[i].genericContent;
                    }
                }
            }

            void SetupChoiceDataTextField()
            {
                choiceData.textField = new TextField();
                choiceData.textField.RegisterValueChangedCallback((value) =>
                {
                    choiceData.String_LGs.Find(String_LG => String_LG.languageType == editorWindow.selectedLanguage).genericContent = value.newValue;
                });

                choiceData.textField.SetValueWithoutNotify
                (
                    choiceData.String_LGs.Find(String_LG => String_LG.languageType == editorWindow.selectedLanguage).genericContent
                );

                choicePort.contentContainer.Add(choiceData.textField);
            }

            void SetupButton_DeleteChoice()
            {
                Button deleteButton = new Button(() => DeletePort(_baseNode, choicePort))
                {
                    text = "X"
                };

                choicePort.contentContainer.Add(deleteButton);
            }

            void FinishPortSetup()
            {
                _baseNode.RefreshPorts();
                _baseNode.RefreshExpandedState();
            }
        }

        void DeletePort(BaseNode baseNode_DeleteFrom, Port port_ToDelete)
        {
            RemoveChoiceDataFromList();

            RemoveConnectingEdges();

            RemovePort();

            FinishPortRemoval();

            void RemoveChoiceDataFromList()
            {
                ChoiceData choiceData_ToDelete = choiceDataList.Find(port => port.dataGuid == port_ToDelete.name);
                choiceDataList.Remove(choiceData_ToDelete);
            }

            void RemoveConnectingEdges()
            {
                IEnumerable<Edge> connectingEdges = graphView.edges.ToList().Where(edge => edge.output == port_ToDelete);
                if (connectingEdges.Any())
                {
                    Edge edge = connectingEdges.First();
                    edge.input.Disconnect(edge);
                    edge.output.Disconnect(edge);
                    graphView.RemoveElement(edge);
                }
            }

            void RemovePort()
            {
                baseNode_DeleteFrom.outputContainer.Remove(port_ToDelete);
            }

            void FinishPortRemoval()
            {
                baseNode_DeleteFrom.RefreshPorts();
                baseNode_DeleteFrom.RefreshExpandedState();
            }
        }
    }
}