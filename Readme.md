# PhobosDemo - Phobos 插件开发示例

这是一个完整的 Phobos 插件开发示例，展示了所有可用的接口和最佳实践。

## 📁 项目结构

```
PhobosDemo/
├── PhobosDemo.csproj      # 项目文件
├── DemoPlugin.cs          # 插件主类
├── DemoPluginUI.cs        # 插件 UI
├── README.md              # 说明文档
├── Shared/                # 共享文件（从 Phobos 复制）
│   ├── Interface/
│   │   └── IPhobosPlugin.cs
│   └── Class/
│       └── PCPluginBase.cs
└── Assets/                # 资源文件（可选）
    └── icon.png
```

## 🔧 需要从 Phobos 复制的文件

在开发插件时，需要将以下文件复制到 `Shared` 目录：

| 源文件 (Phobos) | 目标文件 (PhobosDemo) |
|----------------|----------------------|
| `Shared/Interface/IPhobosPlugin.cs` | `Shared/Interface/IPhobosPlugin.cs` |
| `Class/Plugin/PCPluginBase.cs` | `Shared/Class/PCPluginBase.cs` |

### 复制命令

```bash
# 在 Phobos 项目根目录执行
cp Shared/Interface/IPhobosPlugin.cs ../PhobosDemo/Shared/Interface/
cp Class/Plugin/PCPluginBase.cs ../PhobosDemo/Shared/Class/
```

## 🚀 快速开始

### 1. 修改插件元数据

在 `DemoPlugin.cs` 中修改 `_metadata`：

```csharp
private static readonly PluginMetadata _metadata = new()
{
    PackageName = "com.yourcompany.yourplugin",  // 修改为你的包名
    Name = "Your Plugin Name",
    Manufacturer = "Your Name",
    Version = "1.0.0",
    // ...
};
```

### 2. 实现生命周期方法

```csharp
// 安装时（首次）
public override async Task<RequestResult> OnInstall(params object[] args)

// 启动时（每次打开）
public override async Task<RequestResult> OnLaunch(params object[] args)

// 关闭时
public override async Task<RequestResult> OnClosing(params object[] args)

// 卸载时
public override async Task<RequestResult> OnUninstall(params object[] args)

// 更新时
public override async Task<RequestResult> OnUpdate(string oldVersion, string newVersion, params object[] args)
```

### 3. 处理命令

```csharp
public override async Task<RequestResult> Run(params object[] args)
{
    var command = args[0]?.ToString();
    return command switch
    {
        "mycommand" => await HandleMyCommand(args[1..]),
        _ => new RequestResult { Success = false, Message = "Unknown command" }
    };
}
```

## 📚 可用接口

### 配置读写

```csharp
// 读取本插件配置
var result = await ReadConfig("key");

// 写入本插件配置
await WriteConfig("key", "value");

// 读取其他插件配置（需要授权）
await ReadConfig("key", "other.plugin.packagename");

// 读取系统配置
await ReadSysConfig("Theme");

// 写入系统配置
await WriteSysConfig("Theme", "dark");
```

### 协议注册

```csharp
// 注册协议处理
await Link(new LinkAssociation
{
    Protocol = "myprotocol",           // myprotocol://
    Name = "MyPlugin.Handler",         // 处理器名称
    Description = "Open with My Plugin",
    Command = "Run:protocol:%1"        // %1=完整链接, %0=无协议头链接
});

// 设置为默认打开方式
await LinkDefault("myprotocol");
```

### 启动项

```csharp
// 注册随 Phobos 启动
var result = await BootWithPhobos("AutoStart", priority: 100);
var uuid = result.UUID;

// 移除指定启动项
await RemoveBootWithPhobos(uuid);

// 移除本插件所有启动项
await RemoveBootWithPhobos();

// 获取本插件的启动项列表
var items = await GetBootItems();
```

### 向主程序请求信息

```csharp
// 可用的请求类型
var username = await RequestPhobos("username");      // 用户名
var machine = await RequestPhobos("machinename");    // 机器名
var dir = await RequestPhobos("currentdirectory");   // 当前目录
var plugins = await RequestPhobos("pluginsdirectory"); // 插件目录
var caller = await RequestPhobos("caller");          // 调用者信息
```

### 向主程序发送请求

```csharp
await Request("command", OnCallback, arg1, arg2);

void OnCallback(RequestResult result)
{
    // 处理回调
}
```

## 🎨 UI 开发

插件 UI 通过 `ContentArea` 属性提供：

```csharp
public override FrameworkElement? ContentArea => _contentArea;

public override async Task<RequestResult> OnLaunch(params object[] args)
{
    _contentArea = new MyPluginUI(this);
    return new RequestResult { Success = true };
}
```

## 📦 构建和部署

### 构建

```bash
dotnet build -c Release
```

### 部署

将生成的 DLL 文件复制到 Phobos 插件目录：

```
%APPDATA%\Phobos\Plugins\com.yourcompany.yourplugin\
```

或者在项目文件中启用自动复制：

```xml
<Target Name="CopyToPhobosPlugins" AfterTargets="Build">
    ...
</Target>
```

## ⚠️ 注意事项

1. **包名唯一性**：`PackageName` 必须全局唯一，建议使用反向域名格式
2. **版本号**：使用语义化版本（SemVer），如 `1.0.0`
3. **资源释放**：在 `OnClosing` 中释放所有资源
4. **异常处理**：所有公开方法都应包含异常处理
5. **配置持久化**：重要状态应保存到配置中

## 📝 命令格式

Run 方法的标准命令格式：

| 命令 | 参数 | 说明 |
|-----|------|-----|
| `boot` | command | 启动项执行 |
| `pluginmessage` | sourcePlugin, message, args... | 插件间消息 |
| `protocol` | url | 协议处理 |
| 自定义 | ... | 你的命令 |

## 🔗 相关链接

- [Phobos 主项目](../Phobos/)
- [API 文档](docs/API.md)
- [更新日志](CHANGELOG.md)