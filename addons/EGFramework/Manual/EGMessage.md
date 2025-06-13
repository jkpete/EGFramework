# Protocol Tools 使用指南

Protocol Tools允许您在任何通讯物理层发送信息、接受服务器返回的信息、过滤您的脏数据。

目前工具以发布+订阅的方式处理消息，后续会增加问答的方式处理消息。

本指南中，您将学到以下内容

- 如何在互联网&串口通讯&文件读写&进程吞吐中，发送您的请求，获取您的响应消息。
- 如何自定义消息类，发送您的消息，处理远端响应的消息。
- 自定义插拔式的开启或关闭消息处理功能。
- Protocol Tools的消息解耦原理

---

# 目录

[TOC]



---

# 发送请求部分

## 1. 定义您的请求

发送请求是通讯的第一步，如果您只需要连接到对应的服务器，接受对应的推送，而无需发送任何消息请移步至 [4. 定义您的响应](## 4. 定义您的响应)

新建一个结构类`DataStudent`

```csharp
    public struct DataStudent{
        public string Name { get; set; }
        public int Age;
        public int ID;
        public DataStudent(string name,int age){
            Name = name;
            Age = age;
            ID = 0;
        }
    }
```

实现IRequest接口

IRequest是输出最终结果的接口，如果您需要发送字符类型的数据格式，请实现`ToProtocolData`这个方法，如果您需要发送Hex类型的数据格式，请实现`ToProcotolByteData`这个方法。

注意：切勿实现两个方法，实现两个方法会同时发送字符类型+Hex类型两种数据格式的拼接（字符在前，Hex在后，因为字符根据Encoding会转码成未知的Hex数据，不推荐这样做），请实现其中一个，另外一个使用 `return null` 来填补空缺

```csharp
public struct DataStudent :IRequest{
        public string Name { get; set; }
        public int Age;
        public int ID;
        public DataStudent(string name,int age){
            Name = name;
            Age = age;
            ID = 0;
        }

        public string ToProtocolData()
        {
            return JsonConvert.SerializeObject(this);
        }

        public byte[] ToProtocolByteData()
        {
            return null;
        }
    }
```



## 2. 发送您的请求

定义好您的请求后，在特定类的代码域里可以发送您的请求，该类需要实现`IEGFramework`

该方法包含三个参数，实现`IRequest`的数据对象，发送对象（sender），以及协议类型。

sender

```csh
    public partial class ViewTestStudent : Node,IEGFramework
    {
        public override void _Ready()
        {
            this.EGSendMessage<DataStudent>(new DataStudent(), "127.0.0.1:6000", ProtocolType.TCPClient);
        }
    }
```

## 3.设定您的发送间隔

您有没有发现，在您连续发送两条消息的时候，会产生一个0.1s左右的间隔，这个设计是为了防止连续多次发送消息的时候产生粘包现象，如果您需要立即发送，或者是延长这个间隔，可以通过以下方式设定发送间隔。

```csharp
// 两消息之间设定无间隔，立刻发送 
this.GetModule<EGMessage>().SetDelay(0);
// 两消息之间设定间隔1s后再发送 
this.GetModule<EGMessage>().SetDelay(1000);
```





---

# 处理响应部分

## 4.定义您的响应

