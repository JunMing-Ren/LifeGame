# LifeGame 快速开始

## 项目简介

LifeGame 是一个 AI 驱动的剧情选择类文字游戏，玩家通过选择关键词，AI 生成完整剧情树，玩家进行选择达成不同结局。

## 快速开始

### 1. 克隆项目

```bash
git clone <your-repo-url>
cd LifeGame
```

### 2. 还原依赖

```bash
dotnet restore
```

### 3. 运行项目

```bash
cd src/LifeGame.Desktop
dotnet run
```

### 4. 发布为可执行文件

```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

## 项目结构

```
LifeGame/
├── LifeGame.sln                     # 解决方案文件
├── docs/                            # 文档
│   ├── requirements.md             # 需求文档
│   └── tech-stack.md               # 技术选型文档
└── src/
    └── LifeGame.Desktop/            # WPF 应用
        ├── Models/                  # 数据模型
        ├── Data/                    # 数据库上下文
        ├── Services/                # 服务层
        ├── ViewModels/              # MVVM 视图模型
        ├── Views/                   # XAML 视图
        ├── Converters/              # 数据转换器
        └── App.xaml                 # 应用入口
```

## 当前功能（最小可运行实例）

- ✅ 主菜单界面
- ✅ 游戏设置向导（性别、年龄、背景、地点、主题选择）
- ✅ 演示剧情生成（模拟 AI 生成，使用预设数据）
- ✅ 剧情浏览与选项选择
- ✅ 结局展示
- ✅ 游戏存档保存（SQLite）
- ✅ 历史记录查看
- ✅ 设置界面（AI 配置预留）
- ⚠️ AI 集成（待实现）

## 后续开发

详见 [开发计划文档](./docs/development-plan.md)

## 环境要求

- Windows 10/11
- .NET 8 SDK
- Visual Studio 2022（推荐）或 VS Code

## 技术栈

- WPF (.NET 8)
- C# 12
- Entity Framework Core 8 + SQLite
- CommunityToolkit.Mvvm
- Serilog

## 许可证

MIT License
