using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AG
{
    public class CSVToolbar
    {
        [MenuItem("Custom Tools/Dialogue/Save To CSV")]
        public static void SaveToCSV()
        {
            SaveCSV saveCsv = new SaveCSV();
            saveCsv.Save();

            EditorApplication.Beep();
            Debug.Log("<color=green> Save CSV File Successfully! </color>");
        }

        [MenuItem("Custom Tools/Dialogue/Load From CSV")]
        public static void LoadFromCSV()
        {
            LoadCSV loadCSV = new LoadCSV();
            loadCSV.Load();

            EditorApplication.Beep();
            Debug.Log("<color=green> Load CSV File Successfully! </color>");
        }

        [MenuItem("Custom Tools/Dialogue/Refresh Existing Languages")]
        public static void RefreshExistingLanguage()
        {
            CSVExistingLanguageRefresher refresher = new CSVExistingLanguageRefresher();
            refresher.RefreshExistingLanguageTo_LGs();

            EditorApplication.Beep();
            Debug.Log("<color=green> Refreshed Langagues Successfully! </color>");
        }
    }
}