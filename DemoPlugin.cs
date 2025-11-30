using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Phobos.Shared.Class;
using Phobos.Shared.Interface;

namespace PhobosDemo
{
    /// <summary>
    /// Phobos 插件示例
    /// 包含所有可用接口的预留实现
    /// </summary>
    public class DemoPlugin : PCPluginBase
    {
        #region 元数据定义

        private static readonly PluginMetadata _metadata = new()
        {
            // 必填：插件包名（全局唯一，建议使用反向域名格式）
            PackageName = "com.example.demo",

            // 必填：插件名称
            Name = "Demo Plugin",

            // 必填：开发者/公司名称
            Manufacturer = "Your Name",

            // 必填：版本号（语义化版本）
            Version = "1.0.0",

            // 可选：插件密钥（用于验证）
            Secret = "",

            // 可选：数据库键前缀（默认使用 PackageName）
            DatabaseKey = "com.example.demo",

            // 可选：依赖项
            Dependencies = new List<PluginDependency>
            {
                // 示例：依赖 Logger 插件
                // new PluginDependency 
                // { 
                //     PackageName = "com.phobos.logger", 
                //     MinVersion = "1.0.0",
                //     IsOptional = true // 可选依赖
                // }
            },

            // 可选：本地化名称
            LocalizedNames = new Dictionary<string, string>
            {
                { "en-US", "Demo Plugin" },
                { "zh-CN", "演示插件" },
                { "zh-TW", "演示外掛" },
                { "ja-JP", "デモプラグイン" }
            },

            // 可选：本地化描述
            LocalizedDescriptions = new Dictionary<string, string>
            {
                { "en-US", "A demo plugin showing all available interfaces" },
                { "zh-CN", "一个展示所有可用接口的演示插件" },
                { "zh-TW", "一個展示所有可用介面的演示外掛" },
                { "ja-JP", "利用可能なすべてのインターフェースを示すデモプラグイン" }
            }
        };

        public override PluginMetadata Metadata => _metadata;

        #endregion

        #region UI 组件

        private DemoPluginUI? _contentArea;

        public override FrameworkElement? ContentArea => _contentArea;

        #endregion

        #region 生命周期方法

        /// <summary>
        /// 安装时调用（仅首次安装时）
        /// 用于：注册协议、创建默认配置、初始化资源等
        /// </summary>
        public override async Task<RequestResult> OnInstall(params object[] args)
        {
            try
            {
                // 注册协议关联
                await RegisterProtocols();

                // 创建默认配置
                await CreateDefaultConfig();

                // 注册启动项（可选）
                // await RegisterBootItem();

                return new RequestResult
                {
                    Success = true,
                    Message = "Demo Plugin installed successfully"
                };
            }
            catch (Exception ex)
            {
                return new RequestResult
                {
                    Success = false,
                    Message = $"Installation failed: {ex.Message}",
                    Error = ex
                };
            }
        }

        /// <summary>
        /// 启动时调用（每次打开插件窗口时）
        /// 用于：初始化 UI、加载配置、恢复状态等
        /// </summary>
        public override async Task<RequestResult> OnLaunch(params object[] args)
        {
            try
            {
                // 创建 UI
                _contentArea = new DemoPluginUI(this);

                // 加载配置
                await LoadConfig();

                // 如果有启动参数，处理它们
                if (args.Length > 0)
                {
                    await HandleLaunchArgs(args);
                }

                return new RequestResult
                {
                    Success = true,
                    Message = "Demo Plugin launched"
                };
            }
            catch (Exception ex)
            {
                return new RequestResult
                {
                    Success = false,
                    Message = $"Launch failed: {ex.Message}",
                    Error = ex
                };
            }
        }

        /// <summary>
        /// 关闭时调用（窗口关闭或插件停止时）
        /// 用于：保存状态、释放资源、清理临时文件等
        /// </summary>
        public override async Task<RequestResult> OnClosing(params object[] args)
        {
            try
            {
                // 保存配置
                await SaveConfig();

                // 清理资源
                _contentArea = null;

                return new RequestResult
                {
                    Success = true,
                    Message = "Demo Plugin closing"
                };
            }
            catch (Exception ex)
            {
                return new RequestResult
                {
                    Success = false,
                    Message = $"Closing failed: {ex.Message}",
                    Error = ex
                };
            }
        }

        /// <summary>
        /// 卸载时调用（用户卸载插件时）
        /// 用于：清理数据、注销协议、删除配置等
        /// </summary>
        public override async Task<RequestResult> OnUninstall(params object[] args)
        {
            try
            {
                // 移除启动项
                await RemoveBootWithPhobos();

                // 其他清理工作...

                return new RequestResult
                {
                    Success = true,
                    Message = "Demo Plugin uninstalled"
                };
            }
            catch (Exception ex)
            {
                return new RequestResult
                {
                    Success = false,
                    Message = $"Uninstall failed: {ex.Message}",
                    Error = ex
                };
            }
        }

        /// <summary>
        /// 更新时调用（插件版本更新时）
        /// 用于：数据迁移、配置升级等
        /// </summary>
        public override async Task<RequestResult> OnUpdate(string oldVersion, string newVersion, params object[] args)
        {
            try
            {
                // 版本迁移逻辑
                if (CompareVersion(oldVersion, "1.0.0") < 0)
                {
                    // 从 1.0.0 之前的版本升级
                    await MigrateFromPreV1();
                }

                return new RequestResult
                {
                    Success = true,
                    Message = $"Updated from {oldVersion} to {newVersion}"
                };
            }
            catch (Exception ex)
            {
                return new RequestResult
                {
                    Success = false,
                    Message = $"Update failed: {ex.Message}",
                    Error = ex
                };
            }
        }

        #endregion

        #region 命令处理

        /// <summary>
        /// 主程序/其他插件调用此插件的入口
        /// 用于：处理命令、响应请求、插件间通信等
        /// </summary>
        public override async Task<RequestResult> Run(params object[] args)
        {
            if (args.Length == 0)
            {
                return new RequestResult
                {
                    Success = false,
                    Message = "No command specified"
                };
            }

            var command = args[0]?.ToString()?.ToLowerInvariant() ?? string.Empty;
            var commandArgs = args.Length > 1 ? args[1..] : Array.Empty<object>();

            return command switch
            {
                // 启动项执行
                "boot" => await HandleBootCommand(commandArgs),

                // 插件间消息
                "pluginmessage" => await HandlePluginMessage(commandArgs),

                // 协议处理
                "protocol" => await HandleProtocolCommand(commandArgs),

                // 自定义命令
                "hello" => await HandleHelloCommand(commandArgs),
                "calculate" => await HandleCalculateCommand(commandArgs),
                "getdata" => await HandleGetDataCommand(commandArgs),

                // 未知命令
                _ => new RequestResult
                {
                    Success = false,
                    Message = $"Unknown command: {command}"
                }
            };
        }

        #endregion

        #region 协议注册

        /// <summary>
        /// 注册协议关联
        /// </summary>
        private async Task RegisterProtocols()
        {
            // 注册自定义协议: demo://
            await Link(new LinkAssociation
            {
                Protocol = "demo",
                Name = "DemoPlugin.Open",
                Description = "Open with Demo Plugin",
                Command = "Run:protocol:%1", // %1 = 完整链接, %0 = 不含协议头的链接
                LocalizedDescriptions = new Dictionary<string, string>
                {
                    { "en-US", "Open with Demo Plugin" },
                    { "zh-CN", "使用演示插件打开" }
                }
            });

            // 注册应用协议: Phobos.Demo://
            await Link(new LinkAssociation
            {
                Protocol = "Phobos.Demo",
                Name = "DemoPlugin.Launch",
                Description = "Launch Demo Plugin",
                Command = "Launch"
            });

            // 可选：设置为默认打开方式
            // await LinkDefault("demo");
        }

        #endregion

        #region 配置管理

        // 配置键常量
        private const string CONFIG_LAST_INPUT = "LastInput";
        private const string CONFIG_WINDOW_STATE = "WindowState";
        private const string CONFIG_USER_PREFERENCE = "UserPreference";

        /// <summary>
        /// 创建默认配置
        /// </summary>
        private async Task CreateDefaultConfig()
        {
            // 检查并创建默认配置
            var existing = await ReadConfig(CONFIG_USER_PREFERENCE);
            if (!existing.Success)
            {
                await WriteConfig(CONFIG_USER_PREFERENCE, "default");
            }
        }

        /// <summary>
        /// 加载配置
        /// </summary>
        private async Task LoadConfig()
        {
            var lastInput = await ReadConfig(CONFIG_LAST_INPUT);
            if (lastInput.Success && _contentArea != null)
            {
                _contentArea.SetLastInput(lastInput.Value);
            }
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        private async Task SaveConfig()
        {
            if (_contentArea != null)
            {
                var currentInput = _contentArea.GetCurrentInput();
                await WriteConfig(CONFIG_LAST_INPUT, currentInput);
            }
        }

        /// <summary>
        /// 读取其他插件的配置（需要授权）
        /// </summary>
        public async Task<string?> ReadOtherPluginConfig(string packageName, string key)
        {
            var result = await ReadConfig(key, packageName);
            return result.Success ? result.Value : null;
        }

        /// <summary>
        /// 读取系统配置
        /// </summary>
        public async Task<string?> ReadSystemConfig(string key)
        {
            var result = await ReadSysConfig(key);
            return result.Success ? result.Value : null;
        }

        #endregion

        #region 启动项管理

        /// <summary>
        /// 注册启动项
        /// </summary>
        public async Task<string?> RegisterBootItem()
        {
            var result = await BootWithPhobos(
                command: "AutoStart",  // 启动时执行的命令
                priority: 100          // 优先级（数字越小越先执行）
            );

            return result.Success ? result.UUID : null;
        }

        /// <summary>
        /// 获取本插件的所有启动项
        /// </summary>
        public async Task<List<object>> GetMyBootItems()
        {
            return await GetBootItems();
        }

        #endregion

        #region 命令处理实现

        private async Task HandleLaunchArgs(object[] args)
        {
            // 处理启动参数
            foreach (var arg in args)
            {
                System.Diagnostics.Debug.WriteLine($"Launch arg: {arg}");
            }
            await Task.CompletedTask;
        }

        private async Task<RequestResult> HandleBootCommand(object[] args)
        {
            // 处理启动项执行
            var command = args.Length > 0 ? args[0]?.ToString() : string.Empty;
            System.Diagnostics.Debug.WriteLine($"Boot command: {command}");

            return await Task.FromResult(new RequestResult
            {
                Success = true,
                Message = $"Boot command executed: {command}"
            });
        }

        private async Task<RequestResult> HandlePluginMessage(object[] args)
        {
            // 处理来自其他插件的消息
            var sourcePlugin = args.Length > 0 ? args[0]?.ToString() : "unknown";
            var message = args.Length > 1 ? args[1]?.ToString() : string.Empty;

            System.Diagnostics.Debug.WriteLine($"Message from {sourcePlugin}: {message}");

            return await Task.FromResult(new RequestResult
            {
                Success = true,
                Message = $"Received message from {sourcePlugin}",
                Data = new List<object> { $"Reply to: {message}" }
            });
        }

        private async Task<RequestResult> HandleProtocolCommand(object[] args)
        {
            // 处理协议调用
            var url = args.Length > 0 ? args[0]?.ToString() : string.Empty;
            System.Diagnostics.Debug.WriteLine($"Protocol: {url}");

            // 解析并处理协议
            // demo://action/param1/param2

            return await Task.FromResult(new RequestResult
            {
                Success = true,
                Message = $"Protocol handled: {url}"
            });
        }

        private async Task<RequestResult> HandleHelloCommand(object[] args)
        {
            var name = args.Length > 0 ? args[0]?.ToString() : "World";

            return await Task.FromResult(new RequestResult
            {
                Success = true,
                Message = $"Hello, {name}!",
                Data = new List<object> { DateTime.Now }
            });
        }

        private async Task<RequestResult> HandleCalculateCommand(object[] args)
        {
            if (args.Length < 2)
            {
                return new RequestResult
                {
                    Success = false,
                    Message = "Need two numbers"
                };
            }

            if (double.TryParse(args[0]?.ToString(), out var a) &&
                double.TryParse(args[1]?.ToString(), out var b))
            {
                var result = a + b;
                return await Task.FromResult(new RequestResult
                {
                    Success = true,
                    Message = $"{a} + {b} = {result}",
                    Data = new List<object> { result }
                });
            }

            return new RequestResult
            {
                Success = false,
                Message = "Invalid numbers"
            };
        }

        private async Task<RequestResult> HandleGetDataCommand(object[] args)
        {
            // 返回一些数据
            var data = new List<object>
            {
                new { Name = "Item1", Value = 100 },
                new { Name = "Item2", Value = 200 },
                new { Name = "Item3", Value = 300 }
            };

            return await Task.FromResult(new RequestResult
            {
                Success = true,
                Message = "Data retrieved",
                Data = data
            });
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 向主程序请求数据
        /// </summary>
        public async Task<string?> GetPhobosInfo(string infoType)
        {
            var result = await RequestPhobos(infoType);
            return result.Count > 0 ? result[0]?.ToString() : null;
        }

        /// <summary>
        /// 向主程序发送命令
        /// </summary>
        public async Task<RequestResult> SendRequest(string command, params object[] args)
        {
            return await Request(command, OnRequestCallback, args);
        }

        private void OnRequestCallback(RequestResult result)
        {
            // 请求回调处理
            System.Diagnostics.Debug.WriteLine($"Request callback: {result.Success} - {result.Message}");
        }

        /// <summary>
        /// 版本比较
        /// </summary>
        private static int CompareVersion(string v1, string v2)
        {
            var parts1 = v1.Split('.');
            var parts2 = v2.Split('.');
            var maxLen = Math.Max(parts1.Length, parts2.Length);

            for (int i = 0; i < maxLen; i++)
            {
                var p1 = i < parts1.Length && int.TryParse(parts1[i], out var n1) ? n1 : 0;
                var p2 = i < parts2.Length && int.TryParse(parts2[i], out var n2) ? n2 : 0;
                if (p1 != p2) return p1.CompareTo(p2);
            }
            return 0;
        }

        /// <summary>
        /// 版本迁移示例
        /// </summary>
        private async Task MigrateFromPreV1()
        {
            // 执行迁移逻辑
            await Task.CompletedTask;
        }

        #endregion

        #region 公开方法（供 UI 调用）

        /// <summary>
        /// 测试配置读写
        /// </summary>
        public async Task<string> TestConfigReadWrite(string key, string value)
        {
            var writeResult = await WriteConfig(key, value);
            if (!writeResult.Success)
            {
                return $"Write failed: {writeResult.Message}";
            }

            var readResult = await ReadConfig(key);
            if (!readResult.Success)
            {
                return $"Read failed: {readResult.Message}";
            }

            return $"Written: {value}, Read: {readResult.Value}";
        }

        /// <summary>
        /// 测试启动项
        /// </summary>
        public async Task<string> TestBootItem()
        {
            var uuid = await RegisterBootItem();
            if (string.IsNullOrEmpty(uuid))
            {
                return "Failed to register boot item";
            }

            var items = await GetMyBootItems();
            return $"Boot item registered: {uuid}, Total items: {items.Count}";
        }

        /// <summary>
        /// 测试系统信息获取
        /// </summary>
        public async Task<string> TestGetSystemInfo()
        {
            var username = await GetPhobosInfo("username");
            var machineName = await GetPhobosInfo("machinename");
            var pluginsDir = await GetPhobosInfo("pluginsdirectory");

            return $"User: {username}\nMachine: {machineName}\nPlugins: {pluginsDir}";
        }

        #endregion

        #region 事件订阅示例

        /// <summary>
        /// 订阅主题变更事件
        /// </summary>
        public async Task<bool> SubscribeThemeChanged()
        {
            var result = await Subscribe(PhobosEventIds.Theme, PhobosEventNames.ThemeChanged);
            return result.Success;
        }

        /// <summary>
        /// 订阅语言变更事件
        /// </summary>
        public async Task<bool> SubscribeLanguageChanged()
        {
            var result = await Subscribe(PhobosEventIds.Language, PhobosEventNames.LanguageChanged);
            return result.Success;
        }

        /// <summary>
        /// 订阅插件安装事件
        /// </summary>
        public async Task<bool> SubscribePluginInstalled()
        {
            var result = await Subscribe(PhobosEventIds.Plugin, PhobosEventNames.PluginInstalled);
            return result.Success;
        }

        /// <summary>
        /// 处理接收到的事件
        /// </summary>
        public override async Task OnEventReceived(string eventId, string eventName, params object[] args)
        {
            await base.OnEventReceived(eventId, eventName, args);

            System.Diagnostics.Debug.WriteLine($"[DemoPlugin] Event received: {eventId}.{eventName}");

            switch (eventId)
            {
                case PhobosEventIds.Theme:
                    await HandleThemeEvent(eventName, args);
                    break;

                case PhobosEventIds.Language:
                    await HandleLanguageEvent(eventName, args);
                    break;

                case PhobosEventIds.Plugin:
                    await HandlePluginEvent(eventName, args);
                    break;

                case PhobosEventIds.System:
                    await HandleSystemEvent(eventName, args);
                    break;
            }
        }

        private async Task HandleThemeEvent(string eventName, object[] args)
        {
            if (eventName == PhobosEventNames.ThemeChanged)
            {
                // 主题已变更，刷新 UI
                _contentArea?.Log($"Theme changed to: {(args.Length > 0 ? args[0] : "unknown")}");

                // 如果有自定义窗口，重新应用主题
                // ApplyThemeToWindow(myCustomWindow);
            }
            await Task.CompletedTask;
        }

        private async Task HandleLanguageEvent(string eventName, object[] args)
        {
            if (eventName == PhobosEventNames.LanguageChanged)
            {
                var newLanguage = args.Length > 0 ? args[0]?.ToString() : "unknown";
                _contentArea?.Log($"Language changed to: {newLanguage}");

                // 重新加载本地化字符串
            }
            await Task.CompletedTask;
        }

        private async Task HandlePluginEvent(string eventName, object[] args)
        {
            var pluginName = args.Length > 0 ? args[0]?.ToString() : "unknown";
            _contentArea?.Log($"Plugin event: {eventName} - {pluginName}");
            await Task.CompletedTask;
        }

        private async Task HandleSystemEvent(string eventName, object[] args)
        {
            _contentArea?.Log($"System event: {eventName}");

            if (eventName == PhobosEventNames.SystemShutdown)
            {
                // 保存状态
                await SaveConfig();
            }
            await Task.CompletedTask;
        }

        #endregion

        #region 自定义窗口示例

        /// <summary>
        /// 打开一个应用了主题的自定义窗口
        /// </summary>
        public void OpenThemedWindow()
        {
            var window = new Window
            {
                Title = "Demo Plugin Window",
                Width = 400,
                Height = 300,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            // 应用主程序主题
            ApplyThemeToWindow(window);

            // 创建内容
            var grid = new System.Windows.Controls.Grid();
            var button = new System.Windows.Controls.Button
            {
                Content = "Themed Button",
                Width = 120,
                Height = 32,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            button.Click += (s, e) => MessageBox.Show("Button clicked!", "Demo");
            grid.Children.Add(button);
            window.Content = grid;

            window.Show();
        }

        /// <summary>
        /// 获取当前主题资源用于自定义控件
        /// </summary>
        public ResourceDictionary? GetCurrentTheme()
        {
            return GetMergedDictionaries();
        }

        #endregion
    }
}