# FocusFlow（WinUI 3）

这是一个使用 **WinUI 3 + C#** 实现的桌面应用，包含：

- 任务管理（添加 / 完成或撤销 / 删除）
- 本地持久化（保存到 `%LocalAppData%\BusinessFinal\tasks.json`）
- 25 分钟番茄钟（开始 / 暂停 / 重置）

## 运行环境

- Windows 10 1809+ / Windows 11
- .NET 8 SDK
- Visual Studio 2022（安装 Windows App SDK / WinUI 工作负载）

## 运行

```bash
dotnet restore
dotnet build
```

建议直接用 Visual Studio 打开 `BusinessFinal.WinUI.csproj` 后运行。
