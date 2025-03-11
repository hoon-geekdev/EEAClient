
using System.IO;
using Newtonsoft.Json;
using System.Runtime.Serialization.Formatters.Binary;
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WEBGL
using UnityEngine;
#else
using Serilog;
#endif

public class DataTableGenerate {
    static public void Generate(string className, string jsonPath, string binOutputPath) {
        switch(className) 
        {
            case "StageTable": JsonToBin<TableData.StageTable>(className, jsonPath, binOutputPath); break;
			case "StageWaveTable": JsonToBin<TableData.StageWaveTable>(className, jsonPath, binOutputPath); break;
			case "ObjectTable": JsonToBin<TableData.ObjectTable>(className, jsonPath, binOutputPath); break;
			case "ItemTable": JsonToBin<TableData.ItemTable>(className, jsonPath, binOutputPath); break;
			case "AbilityTable": JsonToBin<TableData.AbilityTable>(className, jsonPath, binOutputPath); break;
			case "AbilityLevelTable": JsonToBin<TableData.AbilityLevelTable>(className, jsonPath, binOutputPath); break;
			case "LevelTable": JsonToBin<TableData.LevelTable>(className, jsonPath, binOutputPath); break;
			case "TextTable": JsonToBin<TableData.TextTable>(className, jsonPath, binOutputPath); break;
			
        }
    }

    static public TableData.BaseTable[] BinToClass(byte[] data, string className)
    {
        try
        {
            switch(className)
            {
                case "StageTable": return BinToObject<TableData.StageTable>(data);
			case "StageWaveTable": return BinToObject<TableData.StageWaveTable>(data);
			case "ObjectTable": return BinToObject<TableData.ObjectTable>(data);
			case "ItemTable": return BinToObject<TableData.ItemTable>(data);
			case "AbilityTable": return BinToObject<TableData.AbilityTable>(data);
			case "AbilityLevelTable": return BinToObject<TableData.AbilityLevelTable>(data);
			case "LevelTable": return BinToObject<TableData.LevelTable>(data);
			case "TextTable": return BinToObject<TableData.TextTable>(data);
			
            }
        }
        catch (System.Exception ex)
        {
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WEBGL
            Debug.LogError($"Failed to convert bin to object: {ex.Message}");
#endif
        }

        return null;
    }

    static public TableData.BaseTable[]? JsonToObject(string className, string jsonText)
    {
        try
        {
            switch(className)
            {
                case "StageTable": return JsonConvert.DeserializeObject<TableData.StageTable[]>(jsonText);
			case "StageWaveTable": return JsonConvert.DeserializeObject<TableData.StageWaveTable[]>(jsonText);
			case "ObjectTable": return JsonConvert.DeserializeObject<TableData.ObjectTable[]>(jsonText);
			case "ItemTable": return JsonConvert.DeserializeObject<TableData.ItemTable[]>(jsonText);
			case "AbilityTable": return JsonConvert.DeserializeObject<TableData.AbilityTable[]>(jsonText);
			case "AbilityLevelTable": return JsonConvert.DeserializeObject<TableData.AbilityLevelTable[]>(jsonText);
			case "LevelTable": return JsonConvert.DeserializeObject<TableData.LevelTable[]>(jsonText);
			case "TextTable": return JsonConvert.DeserializeObject<TableData.TextTable[]>(jsonText);
			
            }
        }
        catch (System.Exception ex)
        {
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WEBGL
#else
            Log.Error($"JsonToObject: {className} {ex.Message}");
#endif
        }

        return null;
    }

    static private void JsonToBin<T>(string className, string jsonFilePath, string outputPath)
    {
        string jsonText = File.ReadAllText(jsonFilePath);
        T[] table = JsonConvert.DeserializeObject<T[]>(jsonText);
#pragma warning disable SYSLIB0011
        BinaryFormatter formatter = new BinaryFormatter();
#pragma warning restore SYSLIB0011

        using (MemoryStream stream = new MemoryStream())
        {
            formatter.Serialize(stream, table);
            byte[] binaryData = stream.ToArray();
            File.WriteAllBytes(outputPath, binaryData);
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WEBGL
            Debug.Log($"{className}.bin is generated");
#endif
        }
    }

    static private T[] BinToObject<T>(byte[] data)
    {
#pragma warning disable SYSLIB0011
        BinaryFormatter formatter = new BinaryFormatter();
#pragma warning restore SYSLIB0011

        using (MemoryStream stream = new MemoryStream(data))
        {
            T[] table = (T[])formatter.Deserialize(stream);
            return table;
        }
    }
}