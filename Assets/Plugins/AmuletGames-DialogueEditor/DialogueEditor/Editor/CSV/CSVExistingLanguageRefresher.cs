using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AG
{
    public class CSVExistingLanguageRefresher
    {
        public void RefreshExistingLanguageTo_LGs()
        {
            // GOAL: Instant creates a list of new language generic to suit any new added language for the editor.
            //       Since this method will overwrite all the current language generic lists,
            //       if language generic's language is not supported in the editor anymore it will be deleted as well.

            List<DialContainerSO> dialContainers = CSVHelper.FindAllObjectsFromResources<DialContainerSO>();
            
            foreach (DialContainerSO containerSO in dialContainers)
            {
                foreach (DialogueNodeData nodeData in containerSO.dialogueNodeDataSavables)
                {
                    nodeData.String_LGs = CreateNewListOf_T_LGs(nodeData.String_LGs);
                    nodeData.AudioClip_LGs = CreateNewListOf_T_LGs(nodeData.AudioClip_LGs);

                    foreach (ChoiceData choiceData in nodeData.choiceDataList)
                    {
                        choiceData.String_LGs = CreateNewListOf_T_LGs(choiceData.String_LGs);
                    }
                }
            }
        }

        List<LanguageGeneric<T>> CreateNewListOf_T_LGs<T>(List<LanguageGeneric<T>> overwrite_LGs)
        {
            List<LanguageGeneric<T>> new_T_LGs = new List<LanguageGeneric<T>>();

            G_LanguageTypeEnum[] languageTypes = (G_LanguageTypeEnum[])Enum.GetValues(typeof(G_LanguageTypeEnum));
            for (int i = 0; i < languageTypes.Length; i++)
            {
                new_T_LGs.Add(new LanguageGeneric<T>
                {
                    languageType = languageTypes[i]
                });
            }

            int overwrite_LGs_Count = overwrite_LGs.Count;
            for (int i = 0; i < overwrite_LGs_Count; i++)
            {
                LanguageGeneric<T> _languageMatched_T_LG = new_T_LGs.Find(T_LG => T_LG.languageType == overwrite_LGs[i].languageType);
                if (_languageMatched_T_LG != null)
                {
                    _languageMatched_T_LG.genericContent = overwrite_LGs[i].genericContent;
                }
            }

            return new_T_LGs;
        }
    }
}