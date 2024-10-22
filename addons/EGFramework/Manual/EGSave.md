# SaveTools（EGSave）模块使用说明

---

SaveTools使用了两种数据格式，一种是表式存储，一种是对象存储，统一使用key-value方式存储数据，通过唯一key值锁定对应的数据。如果是只读数据，则需指定唯一标识key，读写数据，这个key值则为文件路径（唯一）。

在使用该库时，一定要保证该数据被加载。可以重复读取数据文件，如果外部进行了修改，新读取的数据会覆盖掉原来的数据。

需要在对应Godot的Node类下实现接口 `IEGFramework`

下面演示一个打开Res文件夹的示例：

```csharp
public partial class EGSaveTest : Node,IEGFramework{
    public override void _Ready()
    {
        base._Ready();
        this.EGSave().OpenResPath();
    }
}
```

数据工具类开发状态如下：

只读&非只读数据：

- [x] CSV

- [x] Json

- [ ] XML

- [ ] etc...

非只读数据：

- [x] LiteDB

- [x] Byte

- [ ] Sqlite

- [ ] Other DataBase

- [ ] etc...

# API参考

---

# EGSave



## 属性

暂无



## 方法

| 方法名                                                               | 简介               |
| ----------------------------------------------------------------- | ---------------- |
| void LoadDataFile<TSaveData>(string path)                         | 加载数据文件（需要路径）     |
| void ReadData<TReadOnlyData>(string key,string data)              | 读取数据（需获取字符串原始值）  |
| void ReadData<TReadOnlyData>(string key,byte[] data)              | 读取数据（需获取字节流原始值）  |
| void LoadObjectFile<TSaveObject>(string path)                     | 加载对象文件（需要路径）     |
| void ReadObject<TReadOnlyObject>(string key,string data)          | 读取对象（需获取字符串原始值）  |
| void ReadObject<TReadOnlyObject>(string key,byte[] data)          | 读取对象（需获取字节流原始值）  |
| void Unload(string keyOrPath)                                     | 卸载数据             |
| List<string> GetKeys()                                            | 获取所有加载过的key或者路径值 |
| void SetObject<TObject>(string path,string objectKey,TObject obj) | 设置对象（写入文件）       |
| TObject GetObject<TObject>(string path,string key)                | 获取对象（读取文件）       |
| void SetData<TData>(string path,string dataKey,TData data,int id) | 设置数据（写入文件）       |
| TData GetData<TData>(string path,string key,int id)               | 获取单个数据（读取文件）     |
| IEnumerable<TData> GetAllData<TData>(string path,string key)      | 获取全部数据（读取文件）     |
| OpenResPath()                                                     | 打开Res文件目录        |
| OpenUserPath()                                                    | 打开User文件目录       |



## 扩展方法

| 方法名                                         | 简介             |
| ------------------------------------------- | -------------- |
| this.EGSave()                               | 使用存储模块         |
| [string].GetGodotResPath(this string path)  | 转为res文件下的相对路径  |
| [string].GetGodotUserPath(this string path) | 转为User文件下的相对路径 |



## 属性说明

暂无



## 方法说明

### LoadDataFile<T>(string path)

从路径中加载数据文件，因为是可读写，所以必须指定对应的文件路径位置，如果不存在该文件则会新建对应数据文件。

- T：指代任何一个数据存储工具类

- Path：文件的相对路径&绝对路径

```csharp
string Path1 = "Data/Test1.csv".GetGodotResPath();
this.EGSave().LoadDataFile<EGCsvSave>(Path1);
```



### ReadData<T>(string key,string data)

### ReadData<T>(string key,byte[] data)

从文本数据中读取数据，并且赋予对应的key值方便索引到该数据位置上。由于是只读数据，不会创建文件，也不会存在路径信息。

- T：指代任何一个数据工具类

- key：数据的索引，用于定位数据内容。

- data：原始数据，字符串格式或者byte[]字节流。

```csharp
FileAccess testCsv = FileAccess.Open("res://TestCsv.csv", FileAccess.ModeFlags.Read);
this.EGSave().ReadData<EGCsvSave>("TestCsv",testCsv.GetAsText());
```



### LoadObjectFile<T>(string path)

从路径中加载对象文件，因为是可读写，所以必须指定对应的文件路径位置，如果不存在该文件则会新建对应数据文件。

- T：指代任何一个对象存储工具类

- Path：文件的相对路径&绝对路径

```csharp
string Path2 = "Data1.json".GetGodotResPath();
this.EGSave().LoadObjectFile<EGJsonSave>(Path2);
```



### ReadObject<T>(string key,string data)

### ReadObject<T>(string key,byte[] data)

从文本数据中读取对象，并且赋予对应的key值方便索引到该数据位置上。由于是只读对象，不会创建文件，也不会存在路径信息。

- T：指代任何一个对象工具类

- key：对象的索引，用于定位对象内容。

- data：原始数据，字符串格式或者byte[]字节流。

```csharp
FileAccess testJson = FileAccess.Open("res://TestJson.json", FileAccess.ModeFlags.Read);
this.EGSave().ReadObject<EGJsonSave>("TestJson",testJson.GetAsText());
```



### Unload(string keyOrPath)

卸载已读取或者加载的数据，并删除对应key值下的数据，此方法执行后，不再能获取对应的数据，也不能写入文件，需要重新加载。

- keyOrPath：key值或者路径值

```csharp
this.EGSave().Unload("TestCsv");
```



### List<string> GetKeys()

获取所有加载过的key值或者路径值。

```csharp
List<string> keys = this.EGSave().GetKeys();
```







# 接口说明

---

只读&非只读数据说明：

只读数据不实现写数据功能，非只读需要实现写数据功能。

正常数据通过Path加载，只读数据则是通过string加载，无法对其中Path进行写入操作。

| 接口名称                  | 接口简介   |
| --------------------- | ------ |
| IEGSave               | 读写数据加载 |
| IEGSaveReadOnly       | 只读数据加载 |
| IEGSaveObjectReadOnly | 只读对象   |
| IEGSaveDataReadOnly   | 只读数据   |
| IEGSaveObject         | 读写对象   |
| IEGSaveData           | 读写数据   |

## IEGSave

### 描述

通用的存储数据加载接口，通过Path加载文件的数据。

### 方法说明

void InitSaveFile(string path)

> 通过文件路径加载存储文件

## IEGSaveReadOnly
