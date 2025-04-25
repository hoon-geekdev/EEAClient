using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TableData;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using EEA.Define;

[Serializable]
public class FileHashInfo
{
    public string fileName;
    public string md5Hash;
}

namespace EEA.Manager
{
    public class TableManager : SingletonMono<TableManager>
    {
#if DEVELOPMENT
        private string _url = "https://EEA-1311333533.cos.ap-seoul.myqcloud.com/table/Dev";
#elif USE_LOCAL_TABLE && UNITY_EDITOR
        private string _url = "";
#else
        private string _url = "https://EEA-1311333533.cos.ap-seoul.myqcloud.com/table/Dev";
#endif
        private List<string> _patchList = new List<string>();
        private int _retryCount = 0;
        private Dictionary<string, Dictionary<int, BaseTable>> _tables = new Dictionary<string, Dictionary<int, BaseTable>>();
        private Dictionary<int, BaseTable> _codeByTables = new Dictionary<int, BaseTable>();
        private List<BaseTable> _duplicateTable = new List<BaseTable>();

        public bool IsLoadedData { get { return _codeByTables.Count > 0; } }

        public void OnDestroy()
        {
            _tables.Clear();
            _codeByTables.Clear();
            _patchList.Clear();
            _duplicateTable.Clear();
            _retryCount = 0;
        }

        public bool isLoadedData()
        {
            return _codeByTables.Count > 0;
        }

        public void Load(Action onComplete)
        {
            _tables.Clear();
            _codeByTables.Clear();
            _patchList.Clear();
            _duplicateTable.Clear();
            _retryCount = 0;

#if USE_LOCAL_TABLE && UNITY_EDITOR
        LoadStartLocalData(onComplete);
#else
            LoadStart((settingPath, settings) => {
                // download complete setting file
                File.WriteAllText(settingPath, settings);

                if (onComplete != null)
                    onComplete();
            });
#endif
        }

        public void LoadLocal4EditorOnly(Action onComplete)
        {
#if UNITY_EDITOR
            _tables.Clear();
            _codeByTables.Clear();
            _patchList.Clear();
            _duplicateTable.Clear();
            _retryCount = 0;

            LoadStartLocalData(onComplete);
#endif
        }

        public T GetData<T>(int code) where T : BaseTable
        {
            if (code <= 0)
                return null;

            if (HasCode(code) == false)
                return null;

            return (T)_codeByTables[code];
        }

        public bool HasCode(int code)
        {
            return _codeByTables.ContainsKey(code);
        }

        public bool HasData(int code, string column = "")
        {
            if (HasCode(code) == false)
                return false;

            // TODO: Column Check

            BaseTable table = GetData<BaseTable>(code);
            return table != null;
        }

        public Dictionary<int, BaseTable> GetDatas<T>() where T : BaseTable
        {
            string name = typeof(T).Name;
            if (_tables.ContainsKey(name) == false)
                return null;

            return _tables[name];
        }

        public List<T> GetDataList<T>() where T : BaseTable
        {
            string name = typeof(T).Name;
            if (_tables.TryGetValue(name, out Dictionary<int, BaseTable>? table))
            {
                return table.Values.Cast<T>().ToList();
            }

            return new List<T>();
        }

        private async void LoadStart(Action<string, string> onComplete)
        {
            try
            {
                Debug.Log($"GameDataManager::LoadStart - url is :{_url}");
                string oldSettings = "";
                string settingPath = Application.persistentDataPath + "/Setting.json";
                if (File.Exists(settingPath) == true)
                    oldSettings = File.ReadAllText(settingPath);

                string settings = await DownloadTextAsync($"{_url}/Setting.json");

                List<FileHashInfo> curVersion = JsonConvert.DeserializeObject<List<FileHashInfo>>(settings);
                List<FileHashInfo> oldVersion = JsonConvert.DeserializeObject<List<FileHashInfo>>(oldSettings);

                _patchList = ComparePatchList(curVersion, oldVersion);

                List<FileHashInfo> localDatas = curVersion.FindAll(x => _patchList.Contains(x.fileName) == false);
                await LoadData(localDatas);
                await DownloadPatchData(_patchList, _url, () =>
                {
                    if (onComplete != null)
                        onComplete(settingPath, settings);
                });
            }
            catch (Exception e)
            {
                Debug.Log($"GameDataManager::LoadStart Fail - url is :{_url}");
                Debug.LogError(e.Message);
            }
        }

        private async Task DownloadPatchData(List<string> patchList, string baseUri, Action onComplete)
        {
            try
            {
                List<Task> downloadTasks = new List<Task>();
                foreach (string fileName in patchList)
                {
                    Task task = DownloadBinaryAsync($"{baseUri}/", fileName);
                    downloadTasks.Add(task);
                }

                await Task.WhenAll(downloadTasks);

                if (_patchList.Count <= 0 && onComplete != null)
                    onComplete();
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
            finally
            {
                if (_patchList.Count > 0 && _retryCount < 5)
                {
                    _retryCount++;
                    foreach (string fileName in _patchList)
                    {
                        Debug.LogError($"Download retry: {fileName}");
                    }
                    await DownloadPatchData(_patchList, baseUri, onComplete);
                }
            }
        }

        private async void LoadStartLocalData(Action onComplete)
        {
#if UNITY_EDITOR
            try
            {
                string env = EditorPrefs.GetString(EditorPrefPathKey.EnvPrefixKey, "Dev");
                string projectRootPath = Path.GetDirectoryName(Application.dataPath);
                string localPath = $"{projectRootPath}/ServerData/DataTable/{env}/bin";
                string settings = File.ReadAllText($"{localPath}/Setting.json");
                List<FileHashInfo> curVersion = JsonConvert.DeserializeObject<List<FileHashInfo>>(settings);

                List<Task> loadTasks = new List<Task>();

                foreach (FileHashInfo fileInfo in curVersion)
                {
                    string filePath = localPath + $"/{fileInfo.fileName}";
                    if (File.Exists(filePath) == false)
                    {
                        Debug.LogError($"Not found file: {fileInfo.fileName}");
                    }
                    else
                    {
                        Task task = ReadBytesAsync(filePath, fileInfo.fileName);
                        loadTasks.Add(task);
                    }
                }

                await Task.WhenAll(loadTasks);

                if (onComplete != null)
                    onComplete();
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
#endif
        }


        private void AddData(BaseTable[] datas, string name)
        {
            Dictionary<int, BaseTable> data = new Dictionary<int, BaseTable>();
            for (int i = 0; i < datas.Length; i++)
            {
                if (_codeByTables.ContainsKey(datas[i].Code) == true || data.ContainsKey(datas[i].Code) == true)
                {
                    _duplicateTable.Add(datas[i]);
                    Debug.LogError($"{name}, Same key: {datas[i].Code}");
                }
                else
                {
                    _codeByTables.Add(datas[i].Code, datas[i]);
                    data.Add(datas[i].Code, datas[i]);
                }
            }

            if (_tables.ContainsKey(name) == false)
                _tables.Add(name, data);
            else
                _tables[name] = data;
        }

        private async Task LoadData(List<FileHashInfo> files)
        {
            List<Task> loadTasks = new List<Task>();

            foreach (FileHashInfo fileInfo in files)
            {
                string filePath = Application.persistentDataPath + $"/{fileInfo.fileName}";
                if (File.Exists(filePath) == false)
                {
                    if (_patchList.Contains(fileInfo.fileName) == false)
                        _patchList.Add(fileInfo.fileName);
                }
                else if (_patchList.Contains(fileInfo.fileName) == false)
                {
                    Task task = ReadBytesAsync(filePath, fileInfo.fileName);
                    loadTasks.Add(task);
                }
            }

            await Task.WhenAll(loadTasks);
        }

        public async Task ReadBytesAsync(string path, string fileName)
        {
            try
            {
                using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true))
                {
                    byte[] buffer = new byte[stream.Length];
                    await stream.ReadAsync(buffer, 0, (int)stream.Length);

                    if (buffer == null || buffer.Length <= 0)
                    {
                        if (_patchList.Contains(fileName) == false)
                            _patchList.Add(fileName);
                    }
                    else
                    {
                        string name = fileName.Replace(".bin", "");
                        AddData(DataTableGenerate.BinToClass(buffer, name), name);
                        Debug.Log("Load local table: " + name);
                    }
                }
            }
            catch (Exception e)
            {
                if (_patchList.Contains(fileName) == false)
                    _patchList.Add(fileName);

                Debug.LogError($"Load fail: {fileName} \r\nError: {e.Message}");
            }
        }

        public List<string> ComparePatchList(List<FileHashInfo> curVersion, List<FileHashInfo> oldVersion)
        {
            List<string> downloadList = new List<string>();

            if (oldVersion == null || oldVersion.Count <= 0)
            {
                foreach (FileHashInfo info in curVersion)
                    downloadList.Add(info.fileName);
            }
            else
            {
                foreach (FileHashInfo info in curVersion)
                {
                    FileHashInfo oldInfo = oldVersion.Find(x => x.fileName == info.fileName);
                    if (oldInfo == null || oldInfo.md5Hash != info.md5Hash)
                        downloadList.Add(info.fileName);
                }
            }

            return downloadList;
        }

        public async Task<string> DownloadTextAsync(string uri)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(uri))
            {
                var operation = request.SendWebRequest();
                while (!operation.isDone)
                    await Task.Yield();

                if (request.result != UnityWebRequest.Result.Success)
                    Debug.LogError("Error: " + request.error);
                else
                {
                    string data = request.downloadHandler.text;
                    Debug.Log("Download complete: " + uri);
                    return data;
                }
            }

            return "";
        }

        private async Task DownloadBinaryAsync(string baseUri, string file)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(baseUri + file))
            {
                var operation = request.SendWebRequest();
                while (!operation.isDone)
                {
                    await Task.Yield();
                }

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Error: " + request.error);
                }
                else
                {
                    byte[] data = request.downloadHandler.data;
                    string className = file.Replace(".bin", "");
                    AddData(DataTableGenerate.BinToClass(data, className), className);

                    string filePath = $"{Application.persistentDataPath}/{file}";
                    File.WriteAllBytes(filePath, data);

                    _patchList.Remove(file);
                    Debug.Log("Download complete: " + file);
                }
            }
        }

        public List<BaseTable> GetDuplicateTable()
        {
            return _duplicateTable;
        }
    }

}
