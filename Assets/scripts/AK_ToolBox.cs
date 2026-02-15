using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
using NUnit.Framework.Constraints;

namespace AKTool
{
    public class AK_ToolBox
    {
        /// <summary>
        /// DataRow 為資料個數 陣列返回會去除第一行內容
        /// </summary>
        /// <param name="textAssetCSV"></param>
        /// <param name="DataRow"></param>
        static public string[] GetReadCSV(TextAsset textAssetCSV)
        {
            string[] data = textAssetCSV.text.Split(new string[] { ",", "\n" }, System.StringSplitOptions.None);
            Array.Resize(ref data, data.Length - 1);
            return data;
        }

        static public string[] GetCertainColumn(string[] data, int dataRowNum, int certainColNum, bool isExcludeHeader = true)
        {
            if (data == null || dataRowNum <= 0 || certainColNum < 0 || certainColNum >= dataRowNum)
            {
                Debug.LogError("GetCertainColumn: 參數錯誤");
                return Array.Empty<string>();
            }

            List<string> result = new List<string>();

            int startOffset = isExcludeHeader ? dataRowNum : 0;

            for (int i = startOffset + certainColNum; i < data.Length; i += dataRowNum)
            {
                if (i >= data.Length) break;
                result.Add(data[i]);
            }

            return result.ToArray();
        }


        static public string[] GetCertainRow(string[] data, int dataRowNum, int certainRowNum)
        {
            List<string> CalStrList = new List<string>();

            if (data == null || dataRowNum <= 0 || certainRowNum < 0)
            {
                Debug.LogError("GetCertainRow: 參數錯誤");
                return Array.Empty<string>();
            }

            int startPointer = dataRowNum * certainRowNum;

            if (startPointer + dataRowNum > data.Length)
            {
                Debug.LogError("GetCertainRow: 超出資料範圍");
                return Array.Empty<string>();
            }

            List<string> result = new List<string>();

            for (int i = startPointer; i < startPointer + dataRowNum; i++)
            {
                result.Add(data[i]);
            }

            return result.ToArray();
        }

        static public void LoadLangData(TextAsset textAsset, ref string[] langData)
        {
            if (textAsset == null) { Debug.Log("textAsset沒有資料"); }

            string[] str = GetReadCSV(textAsset);
            int langIndex = ((int)SaveSystem.SF.SelectingLanguage);
            langData = GetCertainColumn(str, AllGameManager.SystemLanguageNumber, langIndex);

            for (int i = 0; i < langData.Length; i++)
            {
                langData[i] = SpecialStringProcess(langData[i]);
            }
        }

        static public string SpecialStringProcess(string str)
        {
            string l = "";
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == '$')
                {
                    l += "\n";
                }
                else if (str[i] == '^')
                {
                    l += ",";
                }
                else
                {
                    l += str[i];
                }
            }
            return l;
        }
    }
}