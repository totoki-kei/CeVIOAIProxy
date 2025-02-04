using Microsoft.Win32;
using Newtonsoft.Json;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace CeVIOAIProxy
{
    public partial class Config : JsonConfigBase
    {
        #region Lazy Singleton

        private readonly static Lazy<Config> instance = new Lazy<Config>(() => Load<Config>(_fileName, out bool _));

        public static Config Instance => instance.Value;

        #endregion Lazy Singleton

        public Config()
        {
            this.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(this.Cast))
                {
                    this.OnCastChanged?.Invoke(this, new EventArgs());
                }

                if (e.PropertyName == nameof(this.IsEnabledTextPolling) ||
                    e.PropertyName == nameof(this.CommentTextFilePath))
                {
                    this.OnCommentFileSubscriberChanged?.Invoke(this, new EventArgs());
                }
            };
        }

        [JsonIgnore]
        public static readonly string AppData = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "anoyetta",
            "CeVIOAIProxy");

        private static readonly string _fileName = Path.Combine(AppData, @"CeVIOAIProxy.config.json");

        public override string FileName => _fileName;

        public event EventHandler OnCastChanged;

        public event EventHandler OnCommentFileSubscriberChanged;

        [JsonIgnore]
        public string AppName
        {
            get
            {
                var att = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                return att.Length == 0 ? string.Empty : ((AssemblyTitleAttribute)att[0]).Title;
            }
        }

        [JsonIgnore]
        public string AppNameWithVersion => $"{this.AppName} - {this.AppVersionString}";

        [JsonIgnore]
        public Version AppVersion => Assembly.GetExecutingAssembly().GetName().Version;

        [JsonIgnore]
        public string AppVersionString => $"v{this.AppVersion}(Modified by totoki-kei)";

        private bool isStartupWithWindows;

        [JsonProperty(PropertyName = "is_startup_with_windows")]
        public bool IsStartupWithWindows
        {
            get => this.isStartupWithWindows;
            set
            {
                if (this.SetProperty(ref this.isStartupWithWindows, value))
                {
                    this.SetStartup(value);
                }
            }
        }

        public async void SetStartup(
            bool isStartup) =>
            await Task.Run(() =>
            {
                using (var regkey = Registry.CurrentUser.OpenSubKey(
                    @"Software\Microsoft\Windows\CurrentVersion\Run",
                    true))
                {
                    var att = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                    var product = att.Length == 0 ? string.Empty : ((AssemblyProductAttribute)att[0]).Product;

                    if (isStartup)
                    {
                        regkey.SetValue(
                            product,
                            $"\"{Assembly.GetExecutingAssembly().Location}\"");
                    }
                    else
                    {
                        regkey.DeleteValue(
                            product,
                            false);
                    }
                }
            });

        private double x = 100;

        [JsonProperty(PropertyName = "x")]
        public double X
        {
            get => this.x;
            set => this.SetProperty(ref this.x, Math.Round(value));
        }

        private double y = 100;

        [JsonProperty(PropertyName = "y")]
        public double Y
        {
            get => this.y;
            set => this.SetProperty(ref this.y, Math.Round(value));
        }

        private bool isMinimizeStartup;

        [JsonProperty(PropertyName = "is_minimize_startup")]
        public bool IsMinimizeStartup
        {
            get => this.isMinimizeStartup;
            set => this.SetProperty(ref this.isMinimizeStartup, value);
        }

        private int tcpServerPort = TcpServerPortDefaultValue;

        [JsonProperty(PropertyName = "tcp_server_port")]
        public int TcpServerPort
        {
            get => this.tcpServerPort;
            set => this.SetProperty(ref this.tcpServerPort, value);
        }

        private bool isEnabledIPCServer;

        [JsonProperty(PropertyName = "is_enabled_ipc_server")]
        public bool IsEnabledIPCServer
        {
            get => this.isEnabledIPCServer;
            set => this.SetProperty(ref this.isEnabledIPCServer, value);
        }

        private string ipcChannelName;

        [JsonProperty(PropertyName = "ipc_channel_name")]
        public string IPCChannelName
        {
            get => this.ipcChannelName;
            set => this.SetProperty(ref this.ipcChannelName, value);
        }

        private bool isEnabledRestApiServer;

        [JsonProperty(PropertyName = "is_enabled_rest_api_server")]
        public bool IsEnabledRestApiServer
        {
            get => this.isEnabledRestApiServer;
            set => this.SetProperty(ref this.isEnabledRestApiServer, value);
        }

        private int restApiPortNo;

        [JsonProperty(PropertyName = "rest_api_port_no")]
        public int RestApiPortNo
        {
            get => this.restApiPortNo;
            set => this.SetProperty(ref this.restApiPortNo, value);
        }

        private bool isEnabledTextPolling;

        [JsonProperty(PropertyName = "is_enabled_text_polling")]
        public bool IsEnabledTextPolling
        {
            get => this.isEnabledTextPolling;
            set => this.SetProperty(ref this.isEnabledTextPolling, value);
        }

        private string commentTextFilePath;

        [JsonProperty(PropertyName = "comment_text_file_path")]
        public string CommentTextFilePath
        {
            get => this.commentTextFilePath;
            set => this.SetProperty(ref this.commentTextFilePath, value);
        }

        private string cast;

        [JsonProperty(PropertyName = "cast")]
        public string Cast
        {
            get => this.cast;
            set => this.SetProperty(ref this.cast, value);
        }

        private uint volume = CeVIOBasicParameterDefaultValue;

        [JsonProperty(PropertyName = "volume")]
        public uint Volume
        {
            get => this.volume;
            set => this.SetProperty(ref this.volume, value);
        }

        private uint speed = CeVIOBasicParameterDefaultValue;

        [JsonProperty(PropertyName = "speed")]
        public uint Speed
        {
            get => this.speed;
            set => this.SetProperty(ref this.speed, value);
        }

        private uint tone = CeVIOBasicParameterDefaultValue;

        [JsonProperty(PropertyName = "tone")]
        public uint Tone
        {
            get => this.tone;
            set => this.SetProperty(ref this.tone, value);
        }

        private uint alpha = CeVIOBasicParameterDefaultValue;

        [JsonProperty(PropertyName = "alpha")]
        public uint Alpha
        {
            get => this.alpha;
            set => this.SetProperty(ref this.alpha, value);
        }

        private uint toneScale = CeVIOBasicParameterDefaultValue;

        [JsonProperty(PropertyName = "tone_scale")]
        public uint ToneScale
        {
            get => this.toneScale;
            set => this.SetProperty(ref this.toneScale, value);
        }

        private ObservableCollection<CeVIOTalkerComponent> components = new ObservableCollection<CeVIOTalkerComponent>();

        [JsonProperty(PropertyName = "components")]
        public ObservableCollection<CeVIOTalkerComponent> Components
        {
            get => this.components;
            set => this.SetProperty(ref this.components, value);
        }
    }

    public class CeVIOTalkerComponent : BindableBase
    {
        private string cast;

        [JsonProperty(PropertyName = "cast")]
        public string Cast
        {
            get => this.cast;
            set => this.SetProperty(ref this.cast, value);
        }

        private string id;

        [JsonProperty(PropertyName = "id")]
        public string ID
        {
            get => this.id;
            set => this.SetProperty(ref this.id, value);
        }

        private string name;

        [JsonProperty(PropertyName = "name")]
        public string Name
        {
            get => this.name;
            set => this.SetProperty(ref this.name, value);
        }

        private uint value;

        [JsonProperty(PropertyName = "value")]
        public uint Value
        {
            get => this.value;
            set => this.SetProperty(ref this.value, value);
        }
    }
}
