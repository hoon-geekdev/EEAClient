using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using Newtonsoft.Json;
using TableData;
using System.Linq;
using System.Net.Http;
using EEA.Define;
using EEA.Manager;

public class ExcelParser : EditorWindow
{
    private string secretId = "IKIDIUR9f9WeYFhbFengKirkNlunQoxqTr2y";      // Tencent Cloud에서 제공한 SecretId
    private string secretKey = "kETSBesqY1ECuMZXFivsb5JI1UhfOr53";    // Tencent Cloud에서 제공한 SecretKey
    private string region = "ap-seoul";          // 사용 중인 지역
    private string _bucket = "EEA-1311333533"; // COS 버킷 이름

    private string _excelFolderPath = "";
    private string _jsonOutputFolderPath = "";
    private string _binOutputFolderPath = "";
    private string _converterFilePath = "";
    private string _serverRootPath = "";
    private string _envPrefix = "Dev";

    private bool _selectAll = false;

    private List<bool> _checkbox = new List<bool>();
    private List<string> _fileNames = new List<string>();
    private Vector2 scrollPosition;

    private List<string> _envPrefixes;
    private string _newEnvPrefix;
    private string _serverIp;
    private Dictionary<string, string> _serverIps = new Dictionary<string, string>()
    {
        { "Local", "localhost" },
        {"Dev", "175.106.99.169" }
    };

    [MenuItem("DataTable/ConvertExcel")]
    private static void Init()
    {
        ExcelParser window = GetWindow<ExcelParser>();
        string projectRootPath = Path.GetDirectoryName(Application.dataPath);

        window.titleContent = new GUIContent("ExcelParser");
        window._envPrefix = EditorPrefs.GetString(EditorPrefPathKey.EnvPrefixKey, "Dev");
        window._envPrefixes = EditorPrefs.GetString(EditorPrefPathKey.EnvPrefixesKey, "Dev,QA").Split(',').ToList();

        window._jsonOutputFolderPath = $"{projectRootPath}\\ServerData\\DataTable\\{window._envPrefix}\\json";
        window._binOutputFolderPath = $"{projectRootPath}\\ServerData\\DataTable\\{window._envPrefix}\\bin";
        window._excelFolderPath = EditorPrefs.GetString(EditorPrefPathKey.ExcelFolderPathkey, "");
        window._converterFilePath = $"{projectRootPath}\\Tools\\ExcelParser\\ExcelParser.exe";
        window._serverRootPath = EditorPrefs.GetString(EditorPrefPathKey.ServerRootPath, "C:\\");

        if (window._excelFolderPath != null)
            window.CheckExcelFiles();

        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("Add or Delete Environment", EditorStyles.boldLabel);
        _newEnvPrefix = EditorGUILayout.TextField("Environment Prefix", _newEnvPrefix);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Environment"))
        {
            if (!string.IsNullOrEmpty(_newEnvPrefix) && _envPrefixes != null && !_envPrefixes.Contains(_newEnvPrefix))
            {
                _envPrefixes.Add(_newEnvPrefix);
                SaveEnvPrefixes();
                _newEnvPrefix = string.Empty;
            }
        }

        if (GUILayout.Button("Delete"))
        {
            if (EditorUtility.DisplayDialog("Delete Environment", $"Are you sure you want to delete the environment '{_newEnvPrefix}'?", "Yes", "No"))
            {
                _envPrefixes.Remove(_newEnvPrefix);
                SaveEnvPrefixes();
                if (_envPrefix == _newEnvPrefix)
                {
                    _envPrefix = _envPrefixes.FirstOrDefault() ?? "Dev";
                    EditorPrefs.SetString(EditorPrefPathKey.EnvPrefixKey, _envPrefix);
                    UpdatePaths();
                }

                _newEnvPrefix = string.Empty;
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        GUILayout.Label("Available Environments", EditorStyles.boldLabel, GUILayout.Width(200));
        GUILayout.BeginHorizontal();
        if (_envPrefixes != null)
        {
            foreach (var env in _envPrefixes)
            {
                if (GUILayout.Button($"Set {env} Env", GUILayout.Width(200)))
                {
                    _envPrefix = env;
                    EditorPrefs.SetString(EditorPrefPathKey.EnvPrefixKey, _envPrefix);
                    UpdatePaths();

                    // 폴더 경로에서 마지막 디렉토리를 EnvPrefix로 변경
                    if (string.IsNullOrEmpty(_excelFolderPath) == false)
                    {
                        string lastFolderName = Path.GetFileName(_excelFolderPath.TrimEnd(Path.DirectorySeparatorChar));
                        string path = _excelFolderPath.Replace(lastFolderName, _envPrefix);
                        UnityEngine.Debug.Log(path);
                        if (Directory.Exists(path))
                        {
                            _excelFolderPath = path;
                            EditorPrefs.SetString(EditorPrefPathKey.ExcelFolderPathkey, _excelFolderPath);
                            CheckExcelFiles();
                        }
                    }
                }
            }
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(10);

        GUILayout.Label("Available Server", EditorStyles.boldLabel, GUILayout.Width(200));
        GUILayout.BeginHorizontal();
        if (_serverIps != null)
        {
            foreach (var server in _serverIps)
            {
                if (GUILayout.Button($"Set {server.Key} Server", GUILayout.Width(200)))
                {
                    _serverIp = $"http://{server.Value}:8080/api/admin/table/upload";
                }
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Select Excel Folder", GUILayout.Width(200)))
        {
            string path = EditorUtility.OpenFolderPanel("Select Folder", "", "");

            if (string.IsNullOrEmpty(path))
                return;

            _excelFolderPath = path;
            EditorPrefs.SetString(EditorPrefPathKey.ExcelFolderPathkey, _excelFolderPath);
            CheckExcelFiles();
        }

        GUILayout.TextField(_excelFolderPath);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Select Server Root Path", GUILayout.Width(200)))
        {
            string selectedPath = EditorUtility.OpenFolderPanel("Select Destination Folder", _serverRootPath, "");
            if (!string.IsNullOrEmpty(selectedPath))
            {
                _serverRootPath = selectedPath;
                EditorPrefs.SetString(EditorPrefPathKey.ServerRootPath, _serverRootPath);
            }
        }
        _serverRootPath = EditorGUILayout.TextField(_serverRootPath);
        EditorGUILayout.EndHorizontal();

        GUILayout.Label($"Current env: {_envPrefix}", EditorStyles.boldLabel);
        GUILayout.Label($"Admin Server IP: {_serverIp}", EditorStyles.boldLabel);

        bool state = EditorGUILayout.ToggleLeft("Select All", _selectAll);
        if (state != _selectAll)
        {
            _selectAll = state;
            for (int i = 0; i < _checkbox.Count; ++i)
            {
                _checkbox[i] = _selectAll;
            }
        }

        if (_fileNames != null && _checkbox != null)
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            for (int i = 0; i < _fileNames.Count; i++)
            {
                _checkbox[i] = EditorGUILayout.ToggleLeft(_fileNames[i], _checkbox[i]);
            }

            GUILayout.EndScrollView();
        }

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Make class & json files"))
        {
            ExecuteConverter();
            AssetDatabase.Refresh();
            EditorUtility.RequestScriptReload();
        }

        if (GUILayout.Button("Make Json to Bin"))
        {
            JsonToBin();
            GenerateHash();
        }

        // if (GUILayout.Button("Check Validate & Patch list"))
        // {
        //     TableDataValidateCheck.Init();
        // }

        if (GUILayout.Button("Make DataTableGenerater"))
        {
            GenerateClass(_fileNames.ToArray());
            EditorUtility.RequestScriptReload();
        }

        // if (GUILayout.Button("Upload CDN"))
        // {
        //     UploadCDN();
        // }

        if (GUILayout.Button("Update Server Tables"))
        {
            UpdateServerTables();
        }

        if (GUILayout.Button("Copy to server"))
        {
            string fromPath = $"{Application.dataPath}/Scripts/DataTable";
            string toPath = $"{_serverRootPath}/DataTable";
            CopyCSFiles(fromPath, toPath);
        }

        GUILayout.EndHorizontal();
    }

    private async void UpdateServerTables()
    {
        //"http://175.106.99.169:8080/api/admin/table/upload";
        string url = _serverIp;

        using (HttpClient client = new HttpClient())
        {
            using (var content = new MultipartFormDataContent())
            {
                string uploadedFiles = "";
                for (int i = 0; i < _fileNames.Count; ++i)
                {
                    if (_checkbox[i] == false)
                        continue;

                    string fileName = $"{RemoveNumberAndConvertToUpper(_fileNames[i])}Table.json";
                    string filePath = Path.Combine(_jsonOutputFolderPath, fileName);

                    // 파일 경로와 존재 여부 확인
                    if (!File.Exists(filePath))
                    {
                        UnityEngine.Debug.LogError($"File not found: {filePath}");
                        continue;
                    }

                    content.Add(new StreamContent(File.OpenRead(filePath)), "files", fileName);
                    uploadedFiles += $"{fileName}\n";
                }

                try
                {
                    HttpResponseMessage response = await client.PutAsync(url, content);

                    if (response.IsSuccessStatusCode)
                    {
                        EditorUtility.DisplayDialog("Update Complete", $"The following files have been successfully updated to server:\n\n{uploadedFiles}", "OK");
                        UnityEngine.Debug.Log("Files update successfully.");
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Update Failed", $"Exception during update: {response}", "OK");
                    }
                }
                catch (Exception ex)
                {
                    EditorUtility.DisplayDialog("Update Failed", $"Exception during update: {ex.Message}", "OK");
                }
            }
        }
    }

    void CopyCSFiles(string sourcePath, string destPath)
    {
        if (!Directory.Exists(sourcePath))
        {
            UnityEngine.Debug.LogError("Source folder does not exist!");
            return;
        }

        if (!Directory.Exists(destPath))
        {
            Directory.CreateDirectory(destPath);
        }

        string[] csFiles = Directory.GetFiles(sourcePath, "*.cs", SearchOption.TopDirectoryOnly);

        foreach (var file in csFiles)
        {
            string fileName = Path.GetFileName(file);
            string destFile = Path.Combine(destPath, fileName);
            File.Copy(file, destFile, true);
            UnityEngine.Debug.Log($"Copied {fileName} to {destPath}");
        }

        UnityEngine.Debug.Log("File copy complete.");
    }

    private void UploadCDN()
    {
        // 파일 업로드
        //try
        //{
        //    QCloudCredentialProvider credentialProvider = new DefaultQCloudCredentialProvider(secretId, secretKey, 600);
        //    CosXmlServer cosXml = new CosXmlServer(new CosXmlConfig.Builder()
        //        .SetRegion(region)
        //        .Build(), credentialProvider);

        //    string uploadedFiles = "";
        //    for (int i = 0; i < _fileNames.Count; ++i)
        //    {
        //        if (_checkbox[i] == false)
        //            continue;

        //        string fileName = $"{RemoveNumberAndConvertToUpper(_fileNames[i])}Table.bin";
        //        string filePath = Path.Combine(_binOutputFolderPath, fileName);
        //        // 파일 경로와 존재 여부
        //        if (!File.Exists(filePath))
        //        {
        //            UnityEngine.Debug.LogError($"File not found: {filePath}");
        //            return;
        //        }

        //        PutObject(cosXml, _bucket, $"table/{_envPrefix}/{fileName}", filePath);
        //        uploadedFiles += $"{fileName}\n";
        //    }

        //    // setting.json 파일 업로드
        //    PutObject(cosXml, _bucket, $"table/{_envPrefix}/Setting.json", Path.Combine(_binOutputFolderPath, "Setting.json"));
        //    uploadedFiles += "Setting.json";
        //    EditorUtility.DisplayDialog("Upload Complete", $"The following files have been successfully uploaded to CDN:\n\n{uploadedFiles}", "OK");
        //}
        //catch (Exception ex)
        //{
        //    EditorUtility.DisplayDialog("Upload Failed", $"Exception during upload: {ex.Message}", "OK");
        //}
    }

    //private void PutObject(CosXmlServer cosXml, string bucket, string objKey, string srcPath)
    //{
    //    try
    //    {
    //        PutObjectRequest request = new PutObjectRequest(bucket, objKey, srcPath);
    //        cosXml.PutObject(request, delegate (CosResult cosResult)
    //        {
    //            PutObjectResult result = cosResult as PutObjectResult;
    //        },
    //        delegate (CosClientException clientEx, CosServerException serverEx)
    //        {
    //            if (clientEx != null)
    //            {
    //                UnityEngine.Debug.LogError($"Client error: {clientEx.Message}");
    //            }
    //            if (serverEx != null)
    //            {
    //                UnityEngine.Debug.LogError($"Server error: {serverEx.GetInfo()}");
    //            }
    //        });
    //    }
    //    catch (Exception ex)
    //    {
    //        UnityEngine.Debug.LogError($"Exception during upload: {ex.Message}");
    //    }
    //}

    private void UpdatePaths()
    {
        string projectRootPath = Path.GetDirectoryName(Application.dataPath);
        _jsonOutputFolderPath = $"{projectRootPath}\\ServerData\\DataTable\\{_envPrefix}\\json";
        _binOutputFolderPath = $"{projectRootPath}\\ServerData\\DataTable\\{_envPrefix}\\bin";
    }

    private void SaveEnvPrefixes()
    {
        EditorPrefs.SetString(EditorPrefPathKey.EnvPrefixesKey, string.Join(",", _envPrefixes));
    }

    private void CheckExcelFiles()
    {
        string[] allFiles = Directory.GetFiles(_excelFolderPath);
        List<bool> tmpCheckbox = new List<bool>();
        List<string> tmpFileNames = new List<string>();

        //_fileNames.Clear();
        //_checkbox.Clear();

        for (int i = 0; i < allFiles.Length; i++)
        {
            string extension = Path.GetExtension(allFiles[i]).ToLower();
            if (allFiles[i].Contains("~$") == false && extension == ".xlsx")
            {
                string fileName = Path.GetFileNameWithoutExtension(allFiles[i]);
                tmpFileNames.Add(fileName);

                int idx = _fileNames.FindIndex((x) => x == fileName);
                if (idx >= 0)
                    tmpCheckbox.Add(_checkbox[idx]);
                else
                    tmpCheckbox.Add(false);
            }
        }

        _fileNames = tmpFileNames;
        _checkbox = tmpCheckbox;
    }

    private List<string> ExecuteConverter()
    {
        if (string.IsNullOrEmpty(_converterFilePath) || string.IsNullOrEmpty(_jsonOutputFolderPath))
        {
            UnityEngine.Debug.LogError("Please select all folders");
            return null;
        }

        string files = "";
        List<string> fileList = new List<string>();
        for (int i = 0; i < _checkbox.Count; i++)
        {
            if (_checkbox[i])
            {
                if (files.Length > 0)
                    files += ",";

                string fileName = _fileNames[i];

                files += $"{fileName}";
                fileList.Add(fileName);
            }
        }

        if (!Directory.Exists(_jsonOutputFolderPath))
            Directory.CreateDirectory(_jsonOutputFolderPath);

        string classPath = $"{Application.dataPath}/Scripts/DataTable";
        string args = $"\"{_excelFolderPath}\" \"{classPath}\" \"{_jsonOutputFolderPath}\" \"{files}\"";
        Process program = new Process();
        program.StartInfo.FileName = _converterFilePath;
        program.StartInfo.Arguments = args;
        program.Exited += ProcessExitedEventHandler;

        program.Start();
        program.WaitForExit();

        return fileList;
    }

    private void GenerateClass(string[] fileNames)
    {
        string classFormat = @"
using System.IO;
using Newtonsoft.Json;
using System.Runtime.Serialization.Formatters.Binary;
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WEBGL
using UnityEngine;
#else
using Serilog;
#endif

public class DataTableGenerate {{
    static public void Generate(string className, string jsonPath, string binOutputPath) {{
        switch(className) 
        {{
            {0}
        }}
    }}

    static public TableData.BaseTable[] BinToClass(byte[] data, string className)
    {{
        try
        {{
            switch(className)
            {{
                {1}
            }}
        }}
        catch (System.Exception ex)
        {{
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WEBGL
            Debug.LogError($""Failed to convert bin to object: {{ex.Message}}"");
#endif
        }}

        return null;
    }}

    static public TableData.BaseTable[]? JsonToObject(string className, string jsonText)
    {{
        try
        {{
            switch(className)
            {{
                {2}
            }}
        }}
        catch (System.Exception ex)
        {{
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WEBGL
#else
            Log.Error($""JsonToObject: {{className}} {{ex.Message}}"");
#endif
        }}

        return null;
    }}

    static private void JsonToBin<T>(string className, string jsonFilePath, string outputPath)
    {{
        string jsonText = File.ReadAllText(jsonFilePath);
        T[] table = JsonConvert.DeserializeObject<T[]>(jsonText);
#pragma warning disable SYSLIB0011
        BinaryFormatter formatter = new BinaryFormatter();
#pragma warning restore SYSLIB0011

        using (MemoryStream stream = new MemoryStream())
        {{
            formatter.Serialize(stream, table);
            byte[] binaryData = stream.ToArray();
            File.WriteAllBytes(outputPath, binaryData);
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WEBGL
            Debug.Log($""{{className}}.bin is generated"");
#endif
        }}
    }}

    static private T[] BinToObject<T>(byte[] data)
    {{
#pragma warning disable SYSLIB0011
        BinaryFormatter formatter = new BinaryFormatter();
#pragma warning restore SYSLIB0011

        using (MemoryStream stream = new MemoryStream(data))
        {{
            T[] table = (T[])formatter.Deserialize(stream);
            return table;
        }}
    }}
}}";

        string generateFormat = @"case {0}: JsonToBin<TableData.{1}>(className, jsonPath, binOutputPath); break;" + "\r\n\t\t\t";
        string binToObjectFormat = @"case {0}: return BinToObject<TableData.{1}>(data);" + "\r\n\t\t\t";
        string jsonToObjectFormat = @"case {0}: return JsonConvert.DeserializeObject<TableData.{1}[]>(jsonText);" + "\r\n\t\t\t";
        string generateItems = "";
        string binToObjectItems = "";
        string jsonToObjectItems = "";

        for (int i = 0; i < _fileNames.Count; ++i)
        {
            string className = RemoveNumberAndConvertToUpper(_fileNames[i]) + "Table";
            string formattedItem = string.Format(generateFormat, $"\"{className}\"", $"{className}");
            generateItems += formattedItem;

            formattedItem = string.Format(binToObjectFormat, $"\"{className}\"", $"{className}");
            binToObjectItems += formattedItem;

            formattedItem = string.Format(jsonToObjectFormat, $"\"{className}\"", $"{className}");
            jsonToObjectItems += formattedItem;
        }

        string result = string.Format(classFormat, generateItems, binToObjectItems, jsonToObjectItems);
        File.WriteAllText($"{Application.dataPath}\\Scripts\\DataTable\\DataTableGenerate.cs", result);
    }

    private void JsonToBin()
    {
        for (int i = 0; i < _checkbox.Count; i++)
        {
            if (_checkbox[i])
            {
                string fileName = RemoveNumberAndConvertToUpper(_fileNames[i]);
                string jsonPath = $"{_jsonOutputFolderPath}/{fileName}Table.json";
                string binPath = $"{_binOutputFolderPath}/{fileName}Table.bin";

                string className = fileName + "Table";
                DataTableGenerate.Generate(className, jsonPath, binPath);
            }
        }
        //DataTableGenerate.Generate();
    }

    private void GenerateHash()
    {
        string outputPath = $"{_binOutputFolderPath}/Setting.json";
        string jsonStr = "";

        List<string> selectedFiles = new List<string>();
        for (int i = 0; i < _checkbox.Count; ++i)
        {
            if (_checkbox[i] == true)
                selectedFiles.Add(_fileNames[i]);
        }

        if (File.Exists(outputPath) == true)
        {
            List<FileHashInfo> hashList = UpdateDef(selectedFiles.ToArray(), outputPath);
            jsonStr = JsonConvert.SerializeObject(hashList, Formatting.Indented);
            File.WriteAllText(outputPath, jsonStr);

            for (int i = 0; i < selectedFiles.Count; ++i)
                UnityEngine.Debug.Log($"Change item: {selectedFiles[i]}");
        }
        else
        {
            List<FileHashInfo> hashList = UploadAll(selectedFiles.ToArray());
            jsonStr = JsonConvert.SerializeObject(hashList, Formatting.Indented);
            File.WriteAllText(outputPath, jsonStr);
        }

        UnityEngine.Debug.Log("Hash info saved to " + outputPath);
    }

    public List<FileHashInfo> GetHashList(string[] files)
    {
        List<FileHashInfo> hashList = new List<FileHashInfo>();
        for (int i = 0; i < files.Length; i++)
        {
            FileHashInfo item = SaveHashInfoToFile($"{_binOutputFolderPath}/{RemoveNumberAndConvertToUpper(files[i])}Table.bin");
            if (item == null)
            {
                UnityEngine.Debug.LogError($"Failed to save hash info: {_binOutputFolderPath}/{RemoveNumberAndConvertToUpper(files[i])}Table.bin");
                continue;
            }

            hashList.Add(item);
        }

        return hashList;
    }

    public List<FileHashInfo> UpdateDef(string[] files, string settingFilePath)
    {
        List<FileHashInfo> hashList = GetHashList(files);

        // read json file
        List<FileHashInfo> dataInfo = JsonConvert.DeserializeObject<List<FileHashInfo>>(File.ReadAllText(settingFilePath));
        for (int i = 0; i < hashList.Count; i++)
        {
            bool isExist = false;
            for (int j = 0; j < dataInfo.Count; j++)
            {
                if (hashList[i].fileName == dataInfo[j].fileName)
                {
                    dataInfo[j].md5Hash = hashList[i].md5Hash;
                    isExist = true;
                    break;
                }
            }

            if (isExist == false)
            {
                dataInfo.Add(hashList[i]);
            }
        }

        return dataInfo;
    }

    public List<FileHashInfo> UploadAll(string[] files)
    {
        return GetHashList(files);
    }


    public FileHashInfo SaveHashInfoToFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            string fileName = Path.GetFileName(filePath);
            string md5Hash = HashUtils.GetMD5Hash(filePath);

            FileHashInfo hashInfo = new FileHashInfo { fileName = fileName, md5Hash = md5Hash };
            return hashInfo;
        }
        else
        {
            UnityEngine.Debug.LogError("File not found: " + filePath);
        }

        return null;
    }

    private string RemoveNumberAndConvertToUpper(string input)
    {
        string result = "";
        bool shouldConvertToUpper = false;

        foreach (char c in input)
        {
            if (char.IsDigit(c))
            {
                // 숫자일 경우 무시
                continue;
            }
            else if (c == '_')
            {
                shouldConvertToUpper = true;
            }
            else
            {
                // 대문자로 변환 후 추가
                if (shouldConvertToUpper)
                {
                    result += char.ToUpper(c);
                    shouldConvertToUpper = false;
                }
                else
                {
                    result += c;
                }
            }
        }

        return result.Replace(".xlsx", "");
    }

    private void ProcessExitedEventHandler(object sender, EventArgs e)
    {
        UnityEngine.Debug.Log("External process has exited.");
    }
}

public class TableDataValidateCheck : EditorWindow
{
    private List<string> _patchList = new List<string>();
    private List<BaseTable> _duplicateItems = new List<BaseTable>();
    private Vector2 _scrollPosPatchList;
    private Vector2 _scrollPosDuplicateItems;
    private bool _duplicateItemsLoaded = false;
    private bool _patchListLoaded = false;
    private string _cdnObjectAddress = "https://EEA-1311333533.cos.ap-seoul.myqcloud.com/table";
    public static void Init()
    {
        TableDataValidateCheck window = GetWindow<TableDataValidateCheck>();
        window.titleContent = new GUIContent("TableDataValidateCheck");
        window._patchList.Clear();
        window._duplicateItems.Clear();
        window._patchListLoaded = false;
        window._duplicateItemsLoaded = false;

        TableManager.Instance.LoadLocal4EditorOnly(() =>
        {
            window._duplicateItemsLoaded = true;
            window._duplicateItems = TableManager.Instance.GetDuplicateTable();
        });

        window.LoadData();
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("Patch List", EditorStyles.boldLabel);
        _scrollPosPatchList = EditorGUILayout.BeginScrollView(_scrollPosPatchList, GUILayout.Height(200));
        if (_patchList != null && _patchList.Count > 0)
        {
            foreach (string patch in _patchList)
            {
                GUILayout.Label(patch);
            }
        }
        else
        {
            if (_duplicateItemsLoaded == true)
                GUILayout.Label("No patches found.");
            else
                GUILayout.Label("Loading...");
        }
        EditorGUILayout.EndScrollView();

        GUILayout.Label("Duplicate Items", EditorStyles.boldLabel);
        _scrollPosDuplicateItems = EditorGUILayout.BeginScrollView(_scrollPosDuplicateItems, GUILayout.Height(200));
        if (_duplicateItems != null && _duplicateItems.Count > 0)
        {
            foreach (BaseTable item in _duplicateItems)
            {
                GUILayout.Label(item.Code.ToString()); // Assumes BaseTable has a meaningful ToString() implementation
            }
        }
        else
        {
            if (_patchListLoaded == true)
                GUILayout.Label("No duplicate items found.");
            else
                GUILayout.Label("Loading...");
        }
        EditorGUILayout.EndScrollView();
    }

    private async void LoadData()
    {
        _patchListLoaded = false;
        string env = EditorPrefs.GetString(EditorPrefPathKey.EnvPrefixKey, "Dev");
        string projectRootPath = Path.GetDirectoryName(Application.dataPath);
        string localPath = $"{projectRootPath}/ServerData/DataTable/{env}/bin";
        string settings = File.ReadAllText($"{localPath}/Setting.json");

        string url = $"{_cdnObjectAddress}/{env}";
        string onCloudSettingFile = await TableManager.Instance.DownloadTextAsync($"{url}/Setting.json");

        List<FileHashInfo> curVersion = JsonConvert.DeserializeObject<List<FileHashInfo>>(settings);
        List<FileHashInfo> oldVersion = JsonConvert.DeserializeObject<List<FileHashInfo>>(onCloudSettingFile);

        _patchList = TableManager.Instance.ComparePatchList(curVersion, oldVersion);
        _patchListLoaded = true;
        Repaint(); // Force the window to repaint to update the GUI
    }
}