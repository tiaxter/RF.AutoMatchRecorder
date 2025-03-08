using OBSStudioClient;
using OBSStudioClient.Messages;

namespace RF.AutoMatchRecorder.Modules;

public class Obs
{
    private static Obs? _instance;
    private readonly ObsClient _client = new();
    
    private bool _isAuthenticated;
    private bool _isConnected;
    private const int ConnectionTries = 10;
    
    public static Obs Instance
    {
        get
        {
            return _instance ??= new Obs();
        }
    }

    private Obs()
    {
        _client.PropertyChanged += (_, args) =>
        {
            _isAuthenticated = args.PropertyName == "ConnectionState";
        };

        Connect();
    }

    private async void Connect()
    {
        var tries = 0;
        
        var host = Plugin.Instance.ConfigObsHost.Value;
        var port = Plugin.Instance.ConfigObsPort.Value;
        var password = Plugin.Instance.ConfigObsPassword.Value;

        _isConnected = await _client.ConnectAsync(true,
            password,
            host,
            port
        );

        if (!_isConnected)
        {
            Logger.Log("Failed to connect to OBS");
            return;
        }

        while (!_isAuthenticated && tries < ConnectionTries)
        {
            Thread.Sleep(100);
            tries++;
        }

        if (tries == ConnectionTries)
        {
            Logger.Log("Failed to connect to OBS. Reach maximum number of tries");
        }
    }

    public async void StartRecord(string songName)
    {
        var actionName = "Start record";
        
        try
        {
            if (!_isConnected)
            {
                Connect();
            }
            
            Logger.Log(actionName);

            RequestBatchMessage batch = new();
            batch.AddSetRecordDirectoryRequest(GenerateOutputPath(songName));
            batch.AddSleepRequest(200, null);
            batch.AddStartRecordRequest();
            batch.AddSleepRequest(200, null);
            await _client.SendRequestBatchAsync(batch, 1000);
        }
        catch (Exception ex)
        {
            Logger.Log($"{actionName}: {ex.Message}");
        }
    }

    public async void StopRecord()
    {
        var actionName = "Stop record";
        
        try
        {
            if (!_isConnected)
            {
                Connect();
            }
            
            Logger.Log(actionName);

            RequestBatchMessage batch = new();
            batch.AddStopRecordRequest();
            batch.AddSleepRequest(200, null);
            await _client.SendRequestBatchAsync(batch, 1000);
        }
        catch (Exception ex)
        {
            Logger.Log($"{actionName}: {ex.Message}");
        }
    }

    public async void PauseRecord()
    {
         var actionName = "Stop record";
         
         try
         {
             if (!_isConnected)
             {
                 Connect();
             }
             
             Logger.Log(actionName);

             await _client.PauseRecord();
         }
         catch (Exception ex)
         {
             Logger.Log($"{actionName}: {ex.Message}");
         }
    }

    public async void ResumeRecord()
    {
        var actionName = "Resume record";

        try
        {
            if (!_isConnected)
            {
                Connect();
            }

            Logger.Log(actionName);

            await _client.ResumeRecord();
        }
        catch (Exception ex)
        {
            Logger.Log($"{actionName}: {ex.Message}");
        }
    }
    
    private string GenerateOutputPath(string songName)
    {
        var videoPath = Plugin.Instance.ConfigObsOutputVideoPath.Value;


        var path = videoPath;
        if (!Path.IsPathRooted(videoPath))
        {
            path = Path.Combine(
                Environment.CurrentDirectory,
                Plugin.Instance.ConfigObsOutputVideoPath.Value
            );
        }
        
        // create folder structure

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        
        var songPath = Path.Combine(path, songName);
        if (!Directory.Exists(songPath))
        {
            Directory.CreateDirectory(songPath);
        }
        
        var today = DateTime.Now.ToString("yyyy-MM-dd");
        
        var recordPath = Path.Combine(songPath, today);
        if (!Directory.Exists(recordPath))
        {
            Directory.CreateDirectory(recordPath);
        }
        
        return recordPath;
    }
}