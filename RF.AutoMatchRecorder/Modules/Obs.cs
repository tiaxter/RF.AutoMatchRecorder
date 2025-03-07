using OBSStudioClient;
using OBSStudioClient.Messages;

namespace RF.AutoMatchRecorder.Modules;

public class Obs
{
    private static Obs? _instance;
    private readonly ObsClient _client = new();
    
    private bool _isAuthenticated;
    private const int ConnectionTries = 10;
    
    public static Obs Instance
    {
        get
        {
            return _instance ??= new Obs();
        }
    }

    public Obs()
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

        var isConnected = await _client.ConnectAsync(true,
            password,
            host,
            port
        );

        if (!isConnected)
        {
            throw new Exception("Failed to connect to OBS");
        }

        while (!_isAuthenticated && tries < ConnectionTries)
        {
            await Task.Delay(100);
            tries++;
        }

        if (tries == ConnectionTries)
        {
            throw new Exception("Failed to connect to OBS. Reach maximum number of tries");
        }
    }

    public async void StartRecord(string songName)
    {
        try
        {
            Logger.Log("Starting record...");
        
            RequestBatchMessage batch = new();
            batch.AddSetRecordDirectoryRequest(GenerateOutputPath(songName));
            batch.AddSleepRequest(200, null);
            batch.AddStartRecordRequest();
            batch.AddSleepRequest(200, null);
            await _client.SendRequestBatchAsync(batch, 1000);
        }
        catch (Exception ex)
        {
            Logger.Log($"Start recording failed: {ex.Message}");
        }
    }

    public async void StopRecord()
    {
        try
        {
            Logger.Log("Stop recording...");
            
            RequestBatchMessage batch = new();
            batch.AddStopRecordRequest();
            batch.AddSleepRequest(200, null);
            await _client.SendRequestBatchAsync(batch, 1000);
        }
        catch (Exception ex)
        {
            Logger.Log($"Stop recording failed: {ex.Message}");
        }
        
    }

    public async void PauseRecord()
    {
        try
        {
            Logger.Log("Pause recording...");
            await _client.PauseRecord();
        }
        catch (Exception ex)
        {
            Logger.Log($"Pause recording failed: {ex.Message}");
        }
    }

    public async void ResumeRecord()
    {
        try
        {
            Logger.Log("Resume recording...");
            await _client.ResumeRecord();
        }
        catch (Exception ex)
        {
            Logger.Log($"Resume recording failed: {ex.Message}");
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