using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class DataLoader<T> where T : class
{
    // JSON 파일에서 데이터를 로드합니다.
    public static List<T> LoadDataFromJson(string fileName)
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);

        // 파일이 존재하면 데이터를 읽고, 존재하지 않으면 빈 리스트를 반환합니다.
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            DataWrapper<T> dataWrapper = JsonUtility.FromJson<DataWrapper<T>>(json);
            return dataWrapper.data;
        }
        else
        {
            Debug.LogError("File not found: " + filePath);
            return new List<T>();
        }
    }

    [System.Serializable]
    private class DataWrapper<TData>
    {
        public List<TData> data; // 제네릭 데이터를 담는 리스트
    }
}