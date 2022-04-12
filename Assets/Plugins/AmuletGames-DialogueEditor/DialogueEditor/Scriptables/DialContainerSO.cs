using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace AG
{
    [CreateAssetMenu(menuName = "Dialogue/New Dialogue Container")]
    public class DialContainerSO : ScriptableObject
    {
        public List<NodeLinkData> nodeLinkDataSavables = new List<NodeLinkData>();

        public List<StartNodeData> startNodeDataSavables = new List<StartNodeData>();

        public List<DialogueNodeData> dialogueNodeDataSavables = new List<DialogueNodeData>();

        public List<EventNodeData> eventNodeDataSavables = new List<EventNodeData>();

        public List<EndNodeData> endNodeDataSavables = new List<EndNodeData>();

        public List<BaseNodeData> AllNodeDataSavables
        {
            get
            {
                List<BaseNodeData> allNodeDataSavables = new List<BaseNodeData>();

                allNodeDataSavables.AddRange(startNodeDataSavables);
                allNodeDataSavables.AddRange(dialogueNodeDataSavables);
                allNodeDataSavables.AddRange(eventNodeDataSavables);
                allNodeDataSavables.AddRange(endNodeDataSavables);

                return allNodeDataSavables;
            }
        }
    }

    [Serializable]
    public class LanguageGeneric<T>
    {
        public G_LanguageTypeEnum languageType;
        public T genericContent;
    }

    [Serializable]
    public class ChoiceData
    {
        public string outputGuid;               // this should be the same as its base node guid.
        public string inputGuid;                // input node is the target node that this choice is connecting to.
        public string dataGuid;                 // the guid that represent this data.
        public TextField textField;
        public List<LanguageGeneric<string>> String_LGs = new List<LanguageGeneric<string>>();      // Port's name in different languages.
    }

    [Serializable]
    public class NodeLinkData
    {
        public string outputGuid;       // output node is the base node where this link originate from.
        public string inputGuid;        // input node is the target node where this link connects to.
    }

    [Serializable]
    public class BaseNodeData
    {
        public string nodeGuid;
        public Vector2 position;
    }

    [Serializable]
    public class StartNodeData : BaseNodeData
    {
    }

    [Serializable]
    public class DialogueNodeData : BaseNodeData
    {
        public string speakerName;
        public Sprite speakerSprite;
        public N_AvatarDirectionTypeEnum avatarDirectionType;
        public List<LanguageGeneric<string>> String_LGs;
        public List<LanguageGeneric<AudioClip>> AudioClip_LGs;

        public List<ChoiceData> choiceDataList;
    }

    [Serializable]
    public class EventNodeData : BaseNodeData
    {
        public DialEventSO dialogueEvent;
    }

    [Serializable]
    public class EndNodeData : BaseNodeData
    {
        public N_EndNodeTypeEnum endNodeType;
    }
}