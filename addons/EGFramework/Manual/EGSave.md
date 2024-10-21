# SaveTools（EGSave）模块使用说明

---

SaveTools使用了两种数据格式，一种是表式存储，一种是对象存储，统一使用key-value方式存储数据，通过唯一key值锁定对应的数据。

在使用该库时，一定要保证该数据被加载。

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

## 属性

暂无

## 方法

| 方法名                                                               | 简介                 |
| ----------------------------------------------------------------- | ------------------ |
| void LoadDataFile<TSaveData>(string path)                         | 加载数据文件（需要路径）       |
| void ReadData<TReadOnlyData>(string key,string data)              | 读取数据（需获取string原始值） |
| void LoadObjectFile<TSaveObject>(string path)                     | 加载对象文件（需要路径）       |
| void ReadObject<TReadOnlyObject>(string key,string data)          | 读取对象（需获取string原始值） |
| void SetObject<TObject>(string path,string objectKey,TObject obj) | 设置对象（写入文件）         |
| TObject GetObject<TObject>(string path,string key)                | 获取对象（读取文件）         |
| void SetData<TData>(string path,string dataKey,TData data,int id) | 设置数据（写入文件）         |
| TData GetData<TData>(string path,string key,int id)               | 获取单个数据（读取文件）       |
| IEnumerable<TData> GetAllData<TData>(string path,string key)      | 获取全部数据（读取文件）       |
| OpenResPath()                                                     | 打开Res文件目录          |
| OpenUserPath()                                                    | 打开User文件目录         |

## 扩展方法

| 方法名                                         | 简介             |
| ------------------------------------------- | -------------- |
| this.EGSave()                               | 使用存储模块         |
| [string].GetGodotResPath(this string path)  | 转为res文件下的相对路径  |
| [string].GetGodotUserPath(this string path) | 转为User文件下的相对路径 |

## 属性说明

暂无

## 方法说明

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
