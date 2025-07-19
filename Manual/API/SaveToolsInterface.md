# IEGSave 接口说明

---

只读&非只读数据说明：

只读数据不实现写数据功能，非只读需要实现写数据功能。

正常数据通过Path加载，只读数据则是通过string加载，无法对其中Path进行写入操作。

| 接口名称              | 接口简介     |
| --------------------- | ------------ |
| IEGSave               | 读写数据加载 |
| IEGSaveReadOnly       | 只读数据加载 |
| IEGSaveObjectReadOnly | 只读对象     |
| IEGSaveObject         | 读写对象     |
| IEGSaveDataReadOnly   | 只读数据     |
| IEGSaveData           | 读写数据     |

## IEGSave

### 描述

通用的存储数据加载接口，通过Path加载文件的数据。

### 方法说明

### void InitSaveFile(string path)

> 通过文件路径加载存储文件

## IEGSaveReadOnly

只读文件的数据加载接口，通过字符串或者字节流加载成对应的数据对象。

### void InitReadOnly(string data);

> 通过字符串加载文件内容，需要先从文本文件中读取，请求服务或者其他方式获取内容。

### void InitReadOnly(byte[] data);

> 通过字节流加载文件内容，需要先从字节流文件中读取，请求服务或者其他方式获取内容。

## IEGSaveObjectReadOnly

只读对象文件的获取数据接口

### T GetObject<T>(string objectKey) where T: new();

> 通过键获取对应的对象，如果是单个对象文件的话，则传空字符串即可。

## IEGSaveObject : IEGSaveObjectReadOnly

对象文件的获取&写入数据接口

### void SetObject<T>(string objectKey,T obj);

> 将key值与key对应的对象写入到该文件下。

## IEGSaveDataReadOnly

只读数据文件的获取数据接口

### T GetData<T>(string dataKey,object id) where T : new();

> 用于获取指定条目的数据对象。

### IEnumerable<T> GetAll<T>(string dataKey) where T : new();

> 用于获取key值下的所有列表数据

### IEnumerable<T> FindData<T>(string dataKey,Expression<Func<T, bool>> expression) where T : new();

> 用于查找key值下的所有满足条件的列表数据

## IEGSaveData

数据文件的获取&写入数据接口

### void SetData<TData>(string dataKey,TData data,object id);

> 将key值与key对应的对象的写入到该文件对应的位置（id）下，如果存在数据则进行覆盖。