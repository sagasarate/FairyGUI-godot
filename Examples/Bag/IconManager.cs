using Godot;
using System.Collections;
using System.Collections.Generic;
using FairyGUI;

public delegate void LoadCompleteCallback(NTexture texture);
public delegate void LoadErrorCallback(string error);

/// <summary>
/// Use to load icons from asset bundle, and pool them
/// </summary>
public partial class IconManager : Node
{
    static IconManager _instance;
    public static IconManager inst
    {
        get
        {
            if (_instance == null)
            {
                _instance = new IconManager();
                Stage.inst.AddChild(_instance);
            }
            return _instance;
        }
    }

    public const int POOL_CHECK_TIME = 30;
    public const int MAX_POOL_SIZE = 10;

    List<LoadItem> _items;
    bool _started;
    Hashtable _pool;
    string _basePath;

    public IconManager()
    {
        _items = new List<LoadItem>();
        _pool = new Hashtable();
        _basePath = "res://Examples/Resources/Icons/";
    }

    public void LoadIcon(string resPath,
                    LoadCompleteCallback onSuccess,
                    LoadErrorCallback onFail)
    {
        resPath = _basePath.PathJoin(resPath + ".png");
        Error err = ResourceLoader.LoadThreadedRequest(resPath);
        if (err != Error.Ok)
        {
            onFail($"Failed to start threaded loading: {err}");
            return;
        }
        LoadItem item = new LoadItem();
        item.resPath = resPath;
        item.onSuccess = onSuccess;
        item.onFail = onFail;
        _items.Add(item);
    }

    public override void _Process(double delta)
    {
        for (int i = _items.Count - 1; i >= 0; i--)
        {
            var item = _items[i];
            ResourceLoader.ThreadLoadStatus _loadingStatus = ResourceLoader.LoadThreadedGetStatus(item.resPath);

            switch (_loadingStatus)
            {
                case ResourceLoader.ThreadLoadStatus.InProgress:
                    break;
                case ResourceLoader.ThreadLoadStatus.Loaded:
                    // 步骤3: 获取资源
                    var resource = ResourceLoader.LoadThreadedGet(item.resPath);
                    if (resource is Texture2D tex)
                    {
                        item.onSuccess(new NTexture(tex));
                    }
                    else
                    {
                        resource.Dispose();
                        item.onFail($"not a texture2d resource: {item.resPath}");
                    }
                    _items.RemoveAt(i);
                    break;
                case ResourceLoader.ThreadLoadStatus.Failed:
                    item.onFail($"Failed load: {item.resPath}");
                    _items.RemoveAt(i);
                    break;
                case ResourceLoader.ThreadLoadStatus.InvalidResource:
                    item.onFail($"invalid resource: {item.resPath}");
                    _items.RemoveAt(i);
                    break;
                default:
                    item.onFail($"unknow error: {item.resPath}");
                    _items.RemoveAt(i);
                    break;
            }
        }
        FreeIdleIcons();
    }

    void FreeIdleIcons()
    {
        int cnt = _pool.Count;
        if (cnt > MAX_POOL_SIZE)
        {
            ArrayList toRemove = null;
            foreach (DictionaryEntry de in _pool)
            {
                string key = (string)de.Key;
                NTexture texture = (NTexture)de.Value;
                if (texture.refCount == 0)
                {
                    if (toRemove == null)
                        toRemove = new ArrayList();
                    toRemove.Add(key);
                    texture.Dispose();

                    //Debug.Log("free icon " + de.Key);

                    cnt--;
                    if (cnt <= 8)
                        break;
                }
            }
            if (toRemove != null)
            {
                foreach (string key in toRemove)
                    _pool.Remove(key);
            }
        }
    }

}

class LoadItem
{
    public string resPath;
    public LoadCompleteCallback onSuccess;
    public LoadErrorCallback onFail;
}
