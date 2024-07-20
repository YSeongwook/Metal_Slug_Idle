using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class DataLoader<T> where T : class
{
    // JSON 파일에서 데이터를 로드합니다.
    public static List<T> LoadDataFromJson(string fileName)
    {
        string filePath = Path.Combine(Application.persistentDataPath, fileName);

        // 파일이 존재하지 않으면 StreamingAssets에서 복사합니다.
        if (!File.Exists(filePath))
        {
            CopyFromStreamingAssets(fileName);
        }

        // 파일이 존재하면 데이터를 읽고, 존재하지 않으면 빈 리스트를 반환합니다.
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            DataWrapper<T> dataWrapper = JsonUtility.FromJson<DataWrapper<T>>(json);
            return dataWrapper.data;
        }
        else
        {
            DebugLogger.LogError("File not found: " + filePath);
            return new List<T>();
        }
    }

    // StreamingAssets에서 persistentDataPath로 파일을 복사하는 메서드
    private static void CopyFromStreamingAssets(string fileName)
    {
        string sourcePath = Path.Combine(Application.streamingAssetsPath, fileName);
        string destinationPath = Path.Combine(Application.persistentDataPath, fileName);

        #if UNITY_EDITOR
        // Unity 에디터에서 파일 복사
        File.Copy(sourcePath, destinationPath, true);
        #elif UNITY_ANDROID
        // Android에서 파일 복사
        CopyFileFromStreamingAssetsToPersistentDataPath(sourcePath, destinationPath);
        #else
        // 다른 플랫폼에서 파일 복사 (필요 시 추가)
        File.Copy(sourcePath, destinationPath, true);
        #endif

        DebugLogger.Log("File copied from StreamingAssets to persistentDataPath: " + fileName);
    }

    // Android에서 StreamingAssets에서 persistentDataPath로 파일을 복사하는 메서드
    private static void CopyFileFromStreamingAssetsToPersistentDataPath(string sourcePath, string destinationPath)
    {
        using (WWW www = new WWW(sourcePath))
        {
            while (!www.isDone) { }

            if (string.IsNullOrEmpty(www.error))
            {
                File.WriteAllBytes(destinationPath, www.bytes);
                DebugLogger.Log("File successfully copied from StreamingAssets to persistentDataPath.");
            }
            else
            {
                DebugLogger.LogError("Error copying file from StreamingAssets: " + www.error);
            }
        }
    }

    [System.Serializable]
    private class DataWrapper<TData>
    {
        public List<TData> data; // 제네릭 데이터를 담는 리스트
    }
}
