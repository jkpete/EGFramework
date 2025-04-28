# EGFramework 使用手册

---

# 引言

> `EGFramework`全称Everyone's Godot framework，基于`Godot`开源引擎，使用C#编写，目前仅兼容`Godot4.3 - .NET`，部分兼容`Godot3.5 - .NET`，是完全开源的组件式，功能分布的框架，使用时可以自己根据需求安装对应的`Module`来实现对应的功能，同样也可以裁剪对应的`Module`来删减对应的功能。
>
> `Module` 是`EGFramework`的核心组成部分，也是`EGFramework`中重要的扩展依据。
>
> 目前该框架可能还有很多欠缺与不足，最终目的是做一个方便可靠好用的通讯&存储&资源生成&界面生成框架、用户无需过多学习、开箱即用。

# 关于Godot引擎（摘自[Godot Engine (4.x) 简体中文文档](https://docs.godotengine.org/zh-cn/4.x/about/introduction.html)）

> Godot 引擎是一款功能丰富的跨平台游戏引擎，可以通过统一的界面创建 2D 和 3D 
> 游戏。它提供了一套全面的通用工具，因此用户可以专注于制作游戏，而无需重新发明轮子。游戏可以一键导出到多个平台，包括主流的桌面平台（Linux、macOS、Windows）、移动平台（Android、iOS）、基于
>  Web 的平台以及主机平台。
> 
> Godot 在 [宽松的 MIT 许可证](https://docs.godotengine.org/zh-cn/4.x/about/complying_with_licenses.html#doc-complying-with-licenses) 下完全自由且开源，没有附加条件、没有抽成、什么都没有。用户的游戏乃至引擎的每一行代码，都归用户自己所有。Godot 的开发完全独立且由社区驱动，允许用户为满足需求重塑引擎。它受到不以盈利为目标的 [Godot 基金会](https://godot.foundation/)支持。

---



# 一、准备工作

## 1.下载开发环境

本章节所有环境均在开源IDE `VSCode`，`Godot4.3 - .NET` 的环境下进行，请确保已经下载好`VSCode`，`Godot4.3 - .NET`，<b>本项目Godot版本可能并非最新</b>，但作者会尽可能的升级至最新版的Godot，在此之间请按照如下方式进行框架的集成工作，如果您有购买Rider或者其他编码工具可以根据自己的喜好来替代VSCode。

## 2.VSCode 插件安装

打开VSCode， 按下`Ctrl+Shift+X`，在搜索栏里面搜索`C# Tools for Godot`，安装该插件，因为该插件可能依赖于`C#`插件，同时需要安装`C#`插件。

同上，搜索并安装`NuGet Package Manager GUI`插件，用于Nuget包的安装与管理。

## 3.Nuget包的安装

打开VSCode，将如下部分替换或增加至您名称为*.csproj后缀的工程文件下。

```xml
<ItemGroup>
    <PackageReference Include="System.IO.Ports" Version="8.0.0" />
    <PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
    <PackageReference Include="Microsoft.Data.Sqlite" Version="8.0.1" />
    <PackageReference Include="System.Text.Encoding.CodePages" Version="8.0.0" />
    <PackageReference Include="WebDav.Client" Version="2.8.0" />
    <PackageReference Include="MQTTnet" Version="4.3.3.952" />
    <PackageReference Include="Makaretu.Dns.Multicast" Version="0.27.0" />
    <PackageReference Include="Microsoft.Extensions.Configuration.Ini" Version="8.0.0" />
    <PackageReference Include="LiteDB" Version="5.0.21" />
    <PackageReference Include="BACnet" Version="2.0.4" />
    <PackageReference Include="MySql.Data" Version="9.1.0" />
    <PackageReference Include="Dapper" Version="2.1.35" />
    <PackageReference Include="SSH.NET" Version="2024.2.0" />
    <PackageReference Include="StackExchange.Redis" Version="2.8.31" />
    <PackageReference Include="FluentFTP" Version="52.1.0" />
  </ItemGroup>
```

首次编译确保计算机可以联网，并可以连接该网站[NuGet Gallery | Home](https://www.nuget.org/)

注意：这些Nuget包仅为目前框架版本所用，后续可能会有新的Nuget包导入，会在此处列出。

## 4.删除不需要的Module（可选）

如果您只想使用部分功能，或者不想安装对应的Nuget包依赖，可以直接删除对应的Module脚本文件。

无需担心，除了部分Module存在相关依赖，大部分Module是支持直接删除的。

---



# 二、框架简介

由于该框架使用了较多的Nuget包实现相关的功能，所以安装较为繁琐，同时因为用到较多的扩展方法，提示词可能比较冗余。您可以通过删除对应的Module来定制化自己的框架，也可以新增自己的Module来扩展自己的框架。

如果使用的Nuget包牵扯有侵犯您的许可，请联系作者Mail：1031139173@qq.com，作者尽可能第一时间删除对应的Module功能。

第三方许可文件均存放在目录addons\EGFramework\License_Third_Part下面

## 2.1 使用框架

添加using，并继承接口IEGFramework，即可使用该框架了。

```csharp
using Godot;
using static Godot.GD;
using System.Collections.Generic;
using EGFramework;

public partial class EGTest : Node,IEGFramework{
    
}
```

更多可参考Example。

## 2.2 发送与监听消息



## 2.3 使用本地存储



# 三、EGFramework-API

目前已有的EGFramework功能组件。标记是目前已编写完成，未标记是未编写完成或未测试。

- [x] ProtocolTools&ProtocolExtension
- [x] SaveTools
- [x] OtherTools
- [x] Extension
- [x] NodeExtension（仅Godot下可用）
- [ ] GenerateTools
- [ ] UITools

### 3.1 消息篇（EGMessage-ProtocolTools）

---

#### 通讯支持

- [x] TCPClient
- [x] TCPServer
- [x] UDP（Listen&Send）
- [x] SerialPort
- [x] Ssh
- [x] WebSocketClient
- [x] Bacnet
- [x] MQTT
- [ ] HttpClient
- [ ] HttpServer

#### 异步处理支持

- [x] FileStream
- [x] Process

### 3.2 存储篇（EGMessage-ProtocolTools）

#### 键值对象存储支持

- [x] Json
- [x] Redis
- [ ] Byte

#### 数据存储支持

- [x] Csv
- [x] LiteDB
- [x] MySQL（Dapper）
- [x] Sqlite（Dapper）
- [x] Dapper

#### 文件存储支持

- [x] FTP
- [x] LocalFile
- [ ] Sftp
- [ ] WebDav
