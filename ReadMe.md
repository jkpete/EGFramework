# EGFramework 使用手册

---

# 引言

> `EGFramework`全称Everyone's Godot framework，基于`Godot`开源引擎，使用C#编写，目前仅兼容`Godot4.3 - .NET`，部分兼容`Godot3.5 - .NET`，是完全开源的组件式，功能分布的框架，使用时可以自己根据需求安装对应的`Module`来实现对应的功能，同样也可以裁剪对应的`Module`来删减对应的功能。
> 
> `Module` 是`EGFramework`的核心组成部分，也是`EGFramework`中重要的扩展依据。详细可以查看第二部分-Module。

# 关于Godot引擎（摘自[Godot Engine (4.x) 简体中文文档](https://docs.godotengine.org/zh-cn/4.x/about/introduction.html)）

> Godot 引擎是一款功能丰富的跨平台游戏引擎，可以通过统一的界面创建 2D 和 3D 
> 游戏。它提供了一套全面的通用工具，因此用户可以专注于制作游戏，而无需重新发明轮子。游戏可以一键导出到多个平台，包括主流的桌面平台（Linux、macOS、Windows）、移动平台（Android、iOS）、基于
>  Web 的平台以及主机平台。
> 
> Godot 在 [宽松的 MIT 许可证](https://docs.godotengine.org/zh-cn/4.x/about/complying_with_licenses.html#doc-complying-with-licenses) 下完全自由且开源，没有附加条件、没有抽成、什么都没有。用户的游戏乃至引擎的每一行代码，都归用户自己所有。Godot 的开发完全独立且由社区驱动，允许用户为满足需求重塑引擎。它受到不以盈利为目标的 [Godot 基金会](https://godot.foundation/)支持。

# 一、准备工作

## 1.下载开发环境

本章节所有环境均在开源IDE `VSCode`，`Godot4.2 - .NET` 的环境下进行，请确保已经下载好`VSCode`，`Godot4.2 - .NET`，如果您有购买Rider或者其他编码工具可以根据自己的喜好来替代VSCode。

## 2.VSCode 插件安装

打开VSCode， 按下`Ctrl+Shift+X`，在搜索栏里面搜索`C# Tools for Godot`，安装该插件，因为该插件可能依赖于`C#`插件，同时需要安装`C#`插件。

同上，搜索并安装`NuGet Package Manager GUI`插件，用于Nuget包的安装与管理。

## 3.Nuget包的安装

打开VSCode，按下`Ctrl+Shift+P`，在搜索栏里面搜索`Nuget Package ManagerGUI`，选择右侧`Install New Pakage` ，依次安装以下依赖包：

- System.IO.Ports（仅EGSerialPort 使用）

- Newtonsoft.Json（使用场合较多）

- Microsoft.Data.Sqlite（仅EGSQLite使用）

- System.Text.Encoding.CodePages（ProtocolTools目录下均有使用）

注意：这些Nuget包仅为目前框架版本所用，后续可能会有新的Nuget包导入，会在此处列出。

## 4.删除不需要的Module（可选）

如果您只想使用部分功能，或者不想安装对应的Nuget包依赖，可以直接删除对应的Module脚本文件。

无需担心，除了部分Module存在相关依赖，大部分Module是支持直接删除的。

## 5.EG插件库简介

### 5.1 EGSave篇

---

#### Sqlite数据持久化扩展：

注意：本功能需要依赖安装 `Microsoft.Data.Sqlite` Nuget扩展，将以下代码放入*.csproj 工程文件中，或者通过Nuget安装上述包。

```xml
<PackageReference Include="Microsoft.Data.Sqlite" Version="8.0.1" />
```

使用案例（保存数据=>保存单一数据，目前来看该功能仍需改进）：

```csharp
    public partial class EGSaveTest : Node,IEGFramework
    {
        public override void _Ready()
        {
            TestSqlite();
        }

        public void TestSqlite(){
            // string result = this.EGSqlite().CreateTable<SqliteBackpackItem>();
            this.EGSqlite().SaveData(new SqliteBackpackItem{
                ItemID = 10,
                ItemCount = 1,
                BackpackID = 1,
            });
            GD.Print(this.EGSqlite().ExceptionMsg);
        }
    }
    public struct SqliteBackpackItem{
        public int ItemID;
        public int ItemCount;
        public int BackpackID;
    }
```

结构类要求：保存字段不能使用 {get；set;}，所有数据会自动生成自增序列的ID，字段名称不能为ID。

# 二、框架简介

由于该框架使用了较多的Nuget包实现相关的功能，所以安装较为繁琐，同时因为用到较多的扩展方法，提示词可能比较冗余。您可以通过删除对应的Module来定制化自己的框架，也可以新增自己的Module来扩展自己的框架。

本框架所使用的一切Nuget包均满足MIT开源协议，如果使用的Nuget包牵扯有其他协议请联系作者QQ：1031139173，会删除对应的Module功能保证该框架满足MIT协议。

协议文件均存放在目录addons\EGFramework\License_Third_Part下面

## 1.使用框架

添加using，并继承接口IEGFramework，即可使用该框架了。

以下以一个EGSave的例子，来展示一下框架的存档功能，其他详细用法可以参阅查看Manual-EGSave对应的部分。

```csharp
using Godot;
using static Godot.GD;
using System.Collections.Generic;
using EGFramework;

public partial class EGTest : Node,IEGFramework{
    DataTest dataTest = this.EGSave().GetDataByFile<DataTest>();
    if (dataTest == null)
    {
        dataTest = new DataTest();
        dataTest.PlayerName = "Player1";
        dataTest.Hp = 100;
        this.EGSave().SetDataToFile(dataTest);
    }
}
public class DataTest{
    public string PlayerName;
    public int Hp;
}
```

## 2.直接使用Module

继承接口IEGFramework时，可以直接通过this.GetModule这个扩展方法直接获取该模块。上面的代码可以通过直接调用的方式改写成如下：

```csharp
using Godot;
using static Godot.GD;
using System.Collections.Generic;
using EGFramework;

public partial class EGTest : Node,IEGFramework{
    DataTest dataTest = this.GetModule<EGSave>().GetDataByFile<DataTest>();
    if (dataTest == null)
    {
        dataTest = new DataTest();
        dataTest.PlayerName = "Player1";
        dataTest.Hp = 100;
        this.GetModule<EGSave>().SetDataToFile(dataTest);
    }
}
public class DataTest{
    public string PlayerName;
    public int Hp;
}
```

## 3.扩展框架（编写Module）

编写框架时，要用到IModule这个接口，任何继承了该接口的类均视为Module，可以被上面的方法Get到。

同时我们提供了一个简单的实现接口的抽象类EGModule，继承该类等同于实现接口IModule。

注意的是，因为C#仅支持单继承，如果您需要继承其他工具类或者是Godot的对应类，您应该使用接口而不是抽象类，具体用法可以参考ProtocolTools.EGProtocolSchedule这个类，它通过一个Node的生命周期的_Process()方法实现了特定线程中的消息拾取到主线程的功能。

一个简单的单例对象扩展可以写成如下：

```csharp
using System;
using System.Collections.Generic;

namespace EGFramework
{
    public interface IEGObject
    {
        void RegisterObject<T>(T object_);
        T GetObject<T>() where T : class,new();

    }
    public class EGObject : EGModule,IEGObject
    {
        private IOCContainer ObjectContainer = new IOCContainer();
        public override void Init()
        {

        }

        public TObject GetObject<TObject>() where TObject : class,new()
        {
            if (!ObjectContainer.self.ContainsKey(typeof(TObject)))
            {
                this.RegisterObject(new TObject());
            }
            return ObjectContainer.Get<TObject>();
        }

        public void RegisterObject<TObject>(TObject object_)
        {
            ObjectContainer.Register(object_);
        }

        public bool ContainsObject<TObject>(){
            return ObjectContainer.self.ContainsKey(typeof(TObject));
        }
    }

    public static class CanGetObjectExtension
    {
        public static T EGGetObject<T>(this IEGFramework self) where T : class,new()
        {
            return EGArchitectureImplement.Interface.GetModule<EGObject>().GetObject<T>();
        }
    }
    public static class CanRegisterObjectExtension
    {
        public static void EGRegisterObject<T>(this IArchitecture self,T object_) where T : class,new()
        {
            self.GetModule<EGObject>().RegisterObject(object_);
        }
        public static void EGRegisterObject<T>(this IEGFramework self,T object_) where T : class,new()
        {
            EGArchitectureImplement.Interface.GetModule<EGObject>().RegisterObject(object_);
        }
    }

    public static class CanContainsObjectExtension{
        public static bool EGContainsObject<T>(this IEGFramework self)
        {
            return EGArchitectureImplement.Interface.GetModule<EGObject>().ContainsObject<T>();
        }
    }

}
```
