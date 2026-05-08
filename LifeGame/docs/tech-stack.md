# LifeGame 技术选型文档

## 项目信息

| 项目 | 内容 |
|------|------|
| 项目名称 | LifeGame |
| 目标平台 | Windows 10/11+ |
| 技术栈 | C# / WPF / .NET 8 |

---

## 技术选型

### 核心技术栈

| 组件 | 技术选型 | 版本 | 官网 |
|------|----------|------|------|
| **UI框架** | WPF | .NET 8 | https://learn.microsoft.com/dotnet/wpf/ |
| **语言** | C# | 12 | https://learn.microsoft.com/dotnet/csharp/ |
| **运行时** | .NET | 8 (LTS) | https://dotnet.microsoft.com/ |
| **AI集成** | OpenAI .NET SDK | 最新 | https://github.com/openai/openai-dotnet |
| **数据存储** | SQLite | - | https://learn.microsoft.com/ef/core/ |
| **ORM** | Entity Framework Core | 8.x | https://learn.microsoft.com/ef/core/ |
| **日志** | Serilog | 最新 | https://serilog.net/ |
| **MVVM框架** | CommunityToolkit.Mvvm | 最新 | https://github.com/CommunityToolkit/dotnet |

### 依赖包列表

```xml
<!-- WPF 应用 -->
<PackageReference Include="CommunityToolkit.Mvvm" Version="8.2.2" />
<PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="8.0.0" />
<PackageReference Include="Serilog" Version="3.1.1" />
<PackageReference Include="Serilog.Sinks.File" Version="5.0.0" />

<!-- 数据层 -->
<PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.0" />

<!-- AI集成 -->
<PackageReference Include="OpenAI" Version="2.0.0" />
```

---

## 项目架构

```
LifeGame/
├── LifeGame.Core/           # 核心业务逻辑层
│   ├── Models/              # 数据模型 (Entity)
│   │   ├── GameRecord.cs    # 游戏记录实体
│   │   ├── StoryNode.cs     # 剧情节点实体
│   │   └── GameSettings.cs  # 游戏设置实体
│   ├── Services/            # 服务层
│   │   ├── IGameService.cs  # 游戏服务接口
│   │   ├── GameService.cs   # 游戏服务实现
│   │   ├── IAiService.cs   # AI服务接口
│   │   ├── AiService.cs    # AI服务实现
│   │   ├── IArchiveService.cs    # 存档服务接口
│   │   └── ArchiveService.cs      # 存档服务实现
│   ├── Data/                # 数据层
│   │   ├── LifeGameDbContext.cs   # EF Core数据库上下文
│   │   └── Migrations/             # 数据库迁移
│   └── Interfaces/          # 接口定义
│
├── LifeGame.Desktop/        # WPF桌面应用
│   ├── ViewModels/          # MVVM视图模型
│   │   ├── MainViewModel.cs
│   │   ├── GameViewModel.cs
│   │   └── SettingsViewModel.cs
│   ├── Views/               # XAML视图
│   │   ├── MainWindow.xaml
│   │   ├── GameView.xaml
│   │   └── SettingsView.xaml
│   ├── Converters/          # 数据转换器
│   ├── Services/            # 客户端服务
│   └── App.xaml             # 应用入口
│
├── LifeGame.sln             # 解决方案文件
└── README.md                # 项目说明
```

---

## 技术选型理由

### 为什么选择 WPF？

1. **Windows原生**：与Windows系统深度集成，UI体验好
2. **MVVM模式**：天然支持数据绑定，代码结构清晰
3. **XAML设计**：UI与逻辑分离，便于维护
4. **成熟稳定**：企业级应用广泛使用，文档丰富
5. **性能优秀**：硬件加速，渲染性能好

### 为什么选择 EF Core + SQLite？

1. **熟悉度高**：你熟悉EF Core，开发效率高
2. **嵌入式**：SQLite无需独立安装，DLL打包进应用
3. **功能完整**：支持迁移、查询、事务
4. **数据管理**：支持存档筛选、统计、分页
5. **跨平台**：如需未来迁移到其他平台

### 为什么选择 CommunityToolkit.Mvvm？

1. **微软官方**：与.NET生态深度集成
2. **轻量级**：不引入过多抽象
3. **功能完整**：内置ObservableObject、RelayCommand等
4. **代码生成**：支持source generator，减少样板代码

---

## 开发环境要求

### 必需环境

| 环境 | 版本 | 说明 |
|------|------|------|
| **操作系统** | Windows 10/11 | 推荐最新版本 |
| **Visual Studio** | 2022 (17.8+) | 推荐，支持.NET 8 |
| **.NET SDK** | 8.0 | LTS版本 |

### 可选环境

| 环境 | 版本 | 说明 |
|------|------|------|
| **VS Code** | 最新 | 轻量级编辑器 |
| **Git** | 最新 | 版本控制 |

### 安装检查命令

```powershell
# 检查 .NET SDK 版本
dotnet --version

# 检查 Visual Studio
code --version
```

---

## 备注

- 所有技术选型均基于稳定版本和生产可用
- AI服务通过OpenAI .NET SDK集成，支持OpenAI/Azure/本地模型
- 数据库文件存储在用户AppData目录

---

**文档版本**：v1.0
**创建日期**：2026-05-08
**最后更新**：2026-05-08
