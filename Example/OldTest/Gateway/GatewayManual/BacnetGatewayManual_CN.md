# Bacnet软网关文档

---

# 拓扑一览

Bacnet设备<------>Bacnet软网关<-------->第三方应用

# 1.网关配置

使用HttpServer模式时，需要注意如果非127.0.0.1这个回环地址，需要手动添加prefix权限或者以管理员身份运行该网关。

目前是以Json来进行网关的启动配置项，后续会新增相关GUI优化使用体验。

## 配置路径

配置网关的配置文件存放于 `SaveData\Default.json`这个文件内，找到如下片段

```json
  "EGFramework.Examples.MqttGateway.DataBacnetGatewaySetting": {
    "MqttHost": "192.168.1.220",
    "HttpServerPrefix": "http://127.0.0.1:5000/",
    "RequestTheme": "/LocalBacnetRequest",
    "ResponseTheme": "/LocalBacnetResponse"
  }
```

## 配置说明（此处值均为String）

| 声明                | 键值               | 示例                     | 备注               |
| ----------------- | ---------------- | ---------------------- | ---------------- |
| MQTT服务器地址（string） | MqttHost         | 192.168.1.220          | 需要改成实际的服务器地址     |
| HttpServer前缀地址    | HttpServerPrefix | http://127.0.0.1:5000/ | 需要以`/`符号结尾       |
| 请求主题（string）      | RequestTheme     | /LocalBacnetRequest    | 发布请求的订阅地址，参考1.1  |
| 响应主题（string）      | ResponseTheme    | /LocalBacnetResponse   | 接受响应的的订阅地址，参考1.2 |

## 补充说明

### 1.1 请求主题

开发第三方应用时，您仅需要把请求的内容发布到该主题下即可，网关会自动订阅该主题并接受该主题发布的所有消息，并将处理结果发布在响应主题上。

### 1.2 响应主题

开发第三方应用时，您需要订阅该主题来接收处理结果的响应，根据您发布到请求的内容，网关请求物理设备的处理结果会发布到该主题下，您需要根据内容自行判断是哪条请求的响应。

### 1.3 关于HttpSever模式

如果您使用httpServer作为网关请求时，仅需请求对应的接口地址即可。



# 2.WhoIs 设备列表查询

## http接口地址

/WhoIs

## 请求结构说明

Bacnet请求构成

| 声明             | 键值           | 示例             | 备注            |
| -------------- | ------------ | -------------- | ------------- |
| 功能码（string）    | FunctionCode | OperateRequest | 固定值           |
| 操作码（Enum-uint） | OperateCode  | 0~1024         | 需要查询操作码对照表T-0 |

请求案例

注意：MQTT请求设备列表时，需要等待2秒的设备扫描时间，可能不会立刻返回设备列表的结果。HttpServer不会出现此情况，但Http请求时仅使用设备缓存。

示例：

```json
{
  "FunctionCode":"OperateRequest",
  "OperateCode":2
}
```

响应案例

```json
{
  "DevicesList": [
    1
  ],
  "FunctionCode": "WhoIsResponse"
}
```

# 3.Bacnet 单寄存器读写

## http接口地址

【读单个寄存器】/ReadRegisterProperty

【写单个寄存器】/WriteRegisterProperty

## 请求结构说明

Bacnet请求构成

| 声明                  | 键值              | 示例             | 备注                      |
| ------------------- | --------------- | -------------- | ----------------------- |
| 功能码（string）         | FunctionCode    | OperateRequest | 固定值                     |
| 操作码（Enum-uint）      | OperateCode     | 0~1024         | 需要查询操作码对照表T-0           |
| 设备ID（uint）          | DeviceId        | 0~4294967295   |                         |
| 寄存器类型（Enum-uint）    | ObjectTypes     | 0~1024         | 需要查询寄存器类型对照表T-1         |
| 寄存器地址（uint）         | RegisterAddress | 0~4294967295   |                         |
| 寄存器Property类型（Enum） | PropertyIds     | 0~4194303      | 需要查询寄存器Property类型对照表T-2 |
| 要写入寄存器的值（object）    | Value           | 任意数值或者字符       |                         |
| 要写入寄存器的值类型（Enum）    | ValueType       | 1-5            | 需要查询Bacnet数据类型对照表T-3    |

## 响应结构说明

bacnet响应构成

| 声明                        | 键值           | 示例                             | 备注                   |
| ------------------------- | ------------ | ------------------------------ | -------------------- |
| 功能码（string）               | FunctionCode | OperateResponse                | 固定值                  |
| 是否成功（bool）                | IsSuccess    | true&false                     |                      |
| 请求体（Request）              | Request      | 参考2 Bacnet请求-Json结构说明          |                      |
| 数据体（IList）                | ValueQuery   | [ {  "Tag": 4,  "Value":12.0}] |                      |
| BacnetValue-Tag（Enum）     |              | 4                              | 需要查询Bacnet数据类型对照表T-3 |
| BacnetValue-Value（object） |              | 12.0                           |                      |
| 失败原因（string）              | FailedReason | -                              | 待开发                  |

## Bacnet 读寄存器请求

示例：

```json
{
  "FunctionCode":"OperateRequest",
  "OperateCode":0,
  "DeviceId":1,
  "ObjectTypes":0,
  "RegisterAddress":3000164,
  "PropertyIds":85
}
```

## Bacnet 写寄存器请求

示例：

```json
{
  "FunctionCode":"OperateRequest",
  "OperateCode":1,
  "DeviceId":1,
  "ObjectTypes":2,
  "RegisterAddress":3000637,
  "PropertyIds":85,
  "Value":10.0,
  "ValueType":4
}
```

## Bacnet响应案例

查询&写入响应

```json
{
  "FunctionCode": "OperateResponse",
  "IsSuccess": true,
  "Request": {
    "FunctionCode": "OperateRequest",
    "OperateCode": 0,
    "DeviceId": 1,
    "ObjectTypes": 0,
    "RegisterAddress": 3000164,
    "PropertyIds": 85,
    "Value": null
  },
  "ValueQuery": [
    {
      "Tag": 4,
      "Value": 12.0
    }
  ],
  "FailedReason": null
}
```

# 4.Bacnet 读多个寄存器

## http接口地址

/ReadMultiRegister

## 请求结构说明

Bacnet请求构成

| 声明                  | 键值              | 示例               | 备注                      |
| ------------------- | --------------- | ---------------- | ----------------------- |
| 功能码（string）         | FunctionCode    | ReadMultiRequest | 固定值                     |
| 操作码（Enum-uint）      | OperateCode     | 3                | 需要查询操作码对照表T-0，固定值3      |
| 设备ID（uint）          | DeviceId        | 0~4294967295     |                         |
| 数组                  | RegisterInfos   |                  |                         |
| 数组对象结构              | -               | -                | -                       |
| 寄存器类型（Enum-uint）    | ObjectTypes     | 0~1024           | 需要查询寄存器类型对照表T-1         |
| 寄存器地址（uint）         | RegisterAddress | 0~4294967295     |                         |
| 寄存器Property类型（Enum） | PropertyIds     | 0~4194303        | 需要查询寄存器Property类型对照表T-2 |

## 响应结构说明

bacnet响应构成

| 声明                        | 键值              | 示例                          | 备注                   |
| ------------------------- | --------------- | --------------------------- | -------------------- |
| 功能码（string）               | FunctionCode    | OperateResponse             | 固定值                  |
| 是否成功（bool）                | IsSuccess       | true&false                  |                      |
| 查询数组                      | RegisterInfos   |                             |                      |
| 数组对象结构                    | -               | -                           | -                    |
| 寄存器类型（Enum-uint）          | ObjectTypes     | 0~1024                      | 需要查询寄存器类型对照表T-1      |
| 寄存器地址（uint）               | RegisterAddress | 0~4294967295                |                      |
| 寄存器Property类型（Enum）       | PropertyIds     | 0~4194303                   |                      |
| 数据体                       | Value           | {  "Tag": 4,  "Value":12.0} |                      |
| BacnetValue-Tag（Enum）     |                 | 4                           | 需要查询Bacnet数据类型对照表T-3 |
| BacnetValue-Value（object） |                 | 12.0                        |                      |
| 失败原因（string）              | FailedReason    | -                           | 待开发                  |

## Bacnet 读多个寄存器请求

示例：

```json
{
  "FunctionCode":"ReadMultiRequest",
  "OperateCode":3,
  "DeviceId":1,
  "RegisterInfos":[
    {
        "ObjectTypes":0,
        "RegisterAddress":0,
        "PropertyIds":85
    },
    {
        "ObjectTypes":2,
        "RegisterAddress":1,
        "PropertyIds":85
    },
    {
        "ObjectTypes":0,
        "RegisterAddress":2,
        "PropertyIds":85
    }
  ]
}
```

## Bacnet读多个寄存器响应

示例：

```json
{
  "FunctionCode": "ReadMultiResponse",
  "IsSuccess": true,
  "RegisterInfos": [
    {
      "ObjectTypes": 0,
      "RegisterAddress": 0,
      "PropertyIds": 85,
      "IsSuccess": true,
      "Value": {
        "Tag": 4,
        "Value": 12.0
      }
    },
    {
      "ObjectTypes": 2,
      "RegisterAddress": 1,
      "PropertyIds": 85,
      "IsSuccess": true,
      "Value": {
        "Tag": 2,
        "Value": 21
      }
    },
    {
      "ObjectTypes": 0,
      "RegisterAddress": 2,
      "PropertyIds": 85,
      "IsSuccess": true,
      "Value": {
        "Tag": 4,
        "Value": 5.0
      }
    }
  ],
  "FailedReason": null
}
```

# 附表

## T-0 操作码对照表

| 操作类型                 | 对应编号 | 说明            |
| -------------------- | ---- | ------------- |
| ReadPropertyRequest  | 0    | 读单个寄存器请求      |
| WritePropertyRequest | 1    | 写单个寄存器请求      |
| WhoIsRequest         | 2    | Who is 设备列表请求 |
| ReadMultiRequest     | 3    | 读多个寄存器请求      |

原始数据

```csharp
    public enum EGBacnetOperateCode{
        ReadPropertyRequest = 0,
        WritePropertyRequest = 1,
        WhoIsRequest = 2
    }
```

## T-1 寄存器类型对照表

因该对照表过于长，所以仅列出部分常用类型

| 寄存器类型             | 对应编号 | 说明   |
| ----------------- | ---- | ---- |
| Analog Value      | 2    | 可读可写 |
| Analog Input      | 0    |      |
| Multi-state Value | 19   |      |
| Binary Input      | 3    |      |
| Analog Output     | 1    |      |
| Binary Value      | 5    |      |
| Program           | 16   |      |
| Trendlog Multiple | 27   |      |

原始数据

```csharp
public enum BacnetObjectTypes : uint
{
    OBJECT_ANALOG_INPUT = 0u,
    OBJECT_ANALOG_OUTPUT = 1u,
    OBJECT_ANALOG_VALUE = 2u,
    OBJECT_BINARY_INPUT = 3u,
    OBJECT_BINARY_OUTPUT = 4u,
    OBJECT_BINARY_VALUE = 5u,
    OBJECT_CALENDAR = 6u,
    OBJECT_COMMAND = 7u,
    OBJECT_DEVICE = 8u,
    OBJECT_EVENT_ENROLLMENT = 9u,
    OBJECT_FILE = 10u,
    OBJECT_GROUP = 11u,
    OBJECT_LOOP = 12u,
    OBJECT_MULTI_STATE_INPUT = 13u,
    OBJECT_MULTI_STATE_OUTPUT = 14u,
    OBJECT_NOTIFICATION_CLASS = 15u,
    OBJECT_PROGRAM = 16u,
    OBJECT_SCHEDULE = 17u,
    OBJECT_AVERAGING = 18u,
    OBJECT_MULTI_STATE_VALUE = 19u,
    OBJECT_TRENDLOG = 20u,
    OBJECT_LIFE_SAFETY_POINT = 21u,
    OBJECT_LIFE_SAFETY_ZONE = 22u,
    OBJECT_ACCUMULATOR = 23u,
    OBJECT_PULSE_CONVERTER = 24u,
    OBJECT_EVENT_LOG = 25u,
    OBJECT_GLOBAL_GROUP = 26u,
    OBJECT_TREND_LOG_MULTIPLE = 27u,
    OBJECT_LOAD_CONTROL = 28u,
    OBJECT_STRUCTURED_VIEW = 29u,
    OBJECT_ACCESS_DOOR = 30u,
    OBJECT_TIMER = 31u,
    OBJECT_ACCESS_CREDENTIAL = 32u,
    OBJECT_ACCESS_POINT = 33u,
    OBJECT_ACCESS_RIGHTS = 34u,
    OBJECT_ACCESS_USER = 35u,
    OBJECT_ACCESS_ZONE = 36u,
    OBJECT_CREDENTIAL_DATA_INPUT = 37u,
    OBJECT_NETWORK_SECURITY = 38u,
    OBJECT_BITSTRING_VALUE = 39u,
    OBJECT_CHARACTERSTRING_VALUE = 40u,
    OBJECT_DATE_PATTERN_VALUE = 41u,
    OBJECT_DATE_VALUE = 42u,
    OBJECT_DATETIME_PATTERN_VALUE = 43u,
    OBJECT_DATETIME_VALUE = 44u,
    OBJECT_INTEGER_VALUE = 45u,
    OBJECT_LARGE_ANALOG_VALUE = 46u,
    OBJECT_OCTETSTRING_VALUE = 47u,
    OBJECT_POSITIVE_INTEGER_VALUE = 48u,
    OBJECT_TIME_PATTERN_VALUE = 49u,
    OBJECT_TIME_VALUE = 50u,
    OBJECT_NOTIFICATION_FORWARDER = 51u,
    OBJECT_ALERT_ENROLLMENT = 52u,
    OBJECT_CHANNEL = 53u,
    OBJECT_LIGHTING_OUTPUT = 54u,
    OBJECT_BINARY_LIGHTING_OUTPUT = 55u,
    OBJECT_PROPRIETARY_MIN = 128u,
    OBJECT_PROPRIETARY_MAX = 1023u,
    MAX_BACNET_OBJECT_TYPE = 1024u,
    MAX_ASHRAE_OBJECT_TYPE = 56u
}
```

## T-2 寄存器Property类型对照表

因该对照表过于长，所以仅列出部分常用类型

| Property类型         | 对应编号 | 说明  |
| ------------------ | ---- | --- |
| description        | 28   |     |
| object-identifier  | 75   |     |
| object-type        | 79   |     |
| object-name        | 77   |     |
| event-state        | 36   |     |
| status-flags       | 111  |     |
| units              | 117  |     |
| out-of-service     | 81   |     |
| present-value      | 85   |     |
| relinquish-default | 104  |     |
| priority-array     | 87   |     |
| number-of-states   | 74   |     |
| state-text         | 110  |     |
| program-state      | 92   |     |
| program-change     | 0    |     |
| buffer-size        | 126  |     |
| record-count       | 141  |     |
| total-record-count | 145  |     |
| stop-when-full     | 144  |     |
| logging-type       | 197  |     |
| enable             | 133  |     |

原始数据

```csharp
public enum BacnetPropertyIds
{
    PROP_ACKED_TRANSITIONS = 0,
    PROP_ACK_REQUIRED = 1,
    PROP_ACTION = 2,
    PROP_ACTION_TEXT = 3,
    PROP_ACTIVE_TEXT = 4,
    PROP_ACTIVE_VT_SESSIONS = 5,
    PROP_ALARM_VALUE = 6,
    PROP_ALARM_VALUES = 7,
    PROP_ALL = 8,
    PROP_ALL_WRITES_SUCCESSFUL = 9,
    PROP_APDU_SEGMENT_TIMEOUT = 10,
    PROP_APDU_TIMEOUT = 11,
    PROP_APPLICATION_SOFTWARE_VERSION = 12,
    PROP_ARCHIVE = 13,
    PROP_BIAS = 14,
    PROP_CHANGE_OF_STATE_COUNT = 15,
    PROP_CHANGE_OF_STATE_TIME = 16,
    PROP_NOTIFICATION_CLASS = 17,
    PROP_BLANK_1 = 18,
    PROP_CONTROLLED_VARIABLE_REFERENCE = 19,
    PROP_CONTROLLED_VARIABLE_UNITS = 20,
    PROP_CONTROLLED_VARIABLE_VALUE = 21,
    PROP_COV_INCREMENT = 22,
    PROP_DATE_LIST = 23,
    PROP_DAYLIGHT_SAVINGS_STATUS = 24,
    PROP_DEADBAND = 25,
    PROP_DERIVATIVE_CONSTANT = 26,
    PROP_DERIVATIVE_CONSTANT_UNITS = 27,
    PROP_DESCRIPTION = 28,
    PROP_DESCRIPTION_OF_HALT = 29,
    PROP_DEVICE_ADDRESS_BINDING = 30,
    PROP_DEVICE_TYPE = 31,
    PROP_EFFECTIVE_PERIOD = 32,
    PROP_ELAPSED_ACTIVE_TIME = 33,
    PROP_ERROR_LIMIT = 34,
    PROP_EVENT_ENABLE = 35,
    PROP_EVENT_STATE = 36,
    PROP_EVENT_TYPE = 37,
    PROP_EXCEPTION_SCHEDULE = 38,
    PROP_FAULT_VALUES = 39,
    PROP_FEEDBACK_VALUE = 40,
    PROP_FILE_ACCESS_METHOD = 41,
    PROP_FILE_SIZE = 42,
    PROP_FILE_TYPE = 43,
    PROP_FIRMWARE_REVISION = 44,
    PROP_HIGH_LIMIT = 45,
    PROP_INACTIVE_TEXT = 46,
    PROP_IN_PROCESS = 47,
    PROP_INSTANCE_OF = 48,
    PROP_INTEGRAL_CONSTANT = 49,
    PROP_INTEGRAL_CONSTANT_UNITS = 50,
    PROP_ISSUE_CONFIRMED_NOTIFICATIONS = 51,
    PROP_LIMIT_ENABLE = 52,
    PROP_LIST_OF_GROUP_MEMBERS = 53,
    PROP_LIST_OF_OBJECT_PROPERTY_REFERENCES = 54,
    PROP_LIST_OF_SESSION_KEYS = 55,
    PROP_LOCAL_DATE = 56,
    PROP_LOCAL_TIME = 57,
    PROP_LOCATION = 58,
    PROP_LOW_LIMIT = 59,
    PROP_MANIPULATED_VARIABLE_REFERENCE = 60,
    PROP_MAXIMUM_OUTPUT = 61,
    PROP_MAX_APDU_LENGTH_ACCEPTED = 62,
    PROP_MAX_INFO_FRAMES = 63,
    PROP_MAX_MASTER = 64,
    PROP_MAX_PRES_VALUE = 65,
    PROP_MINIMUM_OFF_TIME = 66,
    PROP_MINIMUM_ON_TIME = 67,
    PROP_MINIMUM_OUTPUT = 68,
    PROP_MIN_PRES_VALUE = 69,
    PROP_MODEL_NAME = 70,
    PROP_MODIFICATION_DATE = 71,
    PROP_NOTIFY_TYPE = 72,
    PROP_NUMBER_OF_APDU_RETRIES = 73,
    PROP_NUMBER_OF_STATES = 74,
    PROP_OBJECT_IDENTIFIER = 75,
    PROP_OBJECT_LIST = 76,
    PROP_OBJECT_NAME = 77,
    PROP_OBJECT_PROPERTY_REFERENCE = 78,
    PROP_OBJECT_TYPE = 79,
    PROP_OPTIONAL = 80,
    PROP_OUT_OF_SERVICE = 81,
    PROP_OUTPUT_UNITS = 82,
    PROP_EVENT_PARAMETERS = 83,
    PROP_POLARITY = 84,
    PROP_PRESENT_VALUE = 85,
    PROP_PRIORITY = 86,
    PROP_PRIORITY_ARRAY = 87,
    PROP_PRIORITY_FOR_WRITING = 88,
    PROP_PROCESS_IDENTIFIER = 89,
    PROP_PROGRAM_CHANGE = 90,
    PROP_PROGRAM_LOCATION = 91,
    PROP_PROGRAM_STATE = 92,
    PROP_PROPORTIONAL_CONSTANT = 93,
    PROP_PROPORTIONAL_CONSTANT_UNITS = 94,
    PROP_PROTOCOL_CONFORMANCE_CLASS = 95,
    PROP_PROTOCOL_OBJECT_TYPES_SUPPORTED = 96,
    PROP_PROTOCOL_SERVICES_SUPPORTED = 97,
    PROP_PROTOCOL_VERSION = 98,
    PROP_READ_ONLY = 99,
    PROP_REASON_FOR_HALT = 100,
    PROP_RECIPIENT = 101,
    PROP_RECIPIENT_LIST = 102,
    PROP_RELIABILITY = 103,
    PROP_RELINQUISH_DEFAULT = 104,
    PROP_REQUIRED = 105,
    PROP_RESOLUTION = 106,
    PROP_SEGMENTATION_SUPPORTED = 107,
    PROP_SETPOINT = 108,
    PROP_SETPOINT_REFERENCE = 109,
    PROP_STATE_TEXT = 110,
    PROP_STATUS_FLAGS = 111,
    PROP_SYSTEM_STATUS = 112,
    PROP_TIME_DELAY = 113,
    PROP_TIME_OF_ACTIVE_TIME_RESET = 114,
    PROP_TIME_OF_STATE_COUNT_RESET = 115,
    PROP_TIME_SYNCHRONIZATION_RECIPIENTS = 116,
    PROP_UNITS = 117,
    PROP_UPDATE_INTERVAL = 118,
    PROP_UTC_OFFSET = 119,
    PROP_VENDOR_IDENTIFIER = 120,
    PROP_VENDOR_NAME = 121,
    PROP_VT_CLASSES_SUPPORTED = 122,
    PROP_WEEKLY_SCHEDULE = 123,
    PROP_ATTEMPTED_SAMPLES = 124,
    PROP_AVERAGE_VALUE = 125,
    PROP_BUFFER_SIZE = 126,
    PROP_CLIENT_COV_INCREMENT = 127,
    PROP_COV_RESUBSCRIPTION_INTERVAL = 128,
    PROP_CURRENT_NOTIFY_TIME = 129,
    PROP_EVENT_TIME_STAMPS = 130,
    PROP_LOG_BUFFER = 131,
    PROP_LOG_DEVICE_OBJECT_PROPERTY = 132,
    PROP_ENABLE = 133,
    PROP_LOG_INTERVAL = 134,
    PROP_MAXIMUM_VALUE = 135,
    PROP_MINIMUM_VALUE = 136,
    PROP_NOTIFICATION_THRESHOLD = 137,
    PROP_PREVIOUS_NOTIFY_TIME = 138,
    PROP_PROTOCOL_REVISION = 139,
    PROP_RECORDS_SINCE_NOTIFICATION = 140,
    PROP_RECORD_COUNT = 141,
    PROP_START_TIME = 142,
    PROP_STOP_TIME = 143,
    PROP_STOP_WHEN_FULL = 144,
    PROP_TOTAL_RECORD_COUNT = 145,
    PROP_VALID_SAMPLES = 146,
    PROP_WINDOW_INTERVAL = 147,
    PROP_WINDOW_SAMPLES = 148,
    PROP_MAXIMUM_VALUE_TIMESTAMP = 149,
    PROP_MINIMUM_VALUE_TIMESTAMP = 150,
    PROP_VARIANCE_VALUE = 151,
    PROP_ACTIVE_COV_SUBSCRIPTIONS = 152,
    PROP_BACKUP_FAILURE_TIMEOUT = 153,
    PROP_CONFIGURATION_FILES = 154,
    PROP_DATABASE_REVISION = 155,
    PROP_DIRECT_READING = 156,
    PROP_LAST_RESTORE_TIME = 157,
    PROP_MAINTENANCE_REQUIRED = 158,
    PROP_MEMBER_OF = 159,
    PROP_MODE = 160,
    PROP_OPERATION_EXPECTED = 161,
    PROP_SETTING = 162,
    PROP_SILENCED = 163,
    PROP_TRACKING_VALUE = 164,
    PROP_ZONE_MEMBERS = 165,
    PROP_LIFE_SAFETY_ALARM_VALUES = 166,
    PROP_MAX_SEGMENTS_ACCEPTED = 167,
    PROP_PROFILE_NAME = 168,
    PROP_AUTO_SLAVE_DISCOVERY = 169,
    PROP_MANUAL_SLAVE_ADDRESS_BINDING = 170,
    PROP_SLAVE_ADDRESS_BINDING = 171,
    PROP_SLAVE_PROXY_ENABLE = 172,
    PROP_LAST_NOTIFY_RECORD = 173,
    PROP_SCHEDULE_DEFAULT = 174,
    PROP_ACCEPTED_MODES = 175,
    PROP_ADJUST_VALUE = 176,
    PROP_COUNT = 177,
    PROP_COUNT_BEFORE_CHANGE = 178,
    PROP_COUNT_CHANGE_TIME = 179,
    PROP_COV_PERIOD = 180,
    PROP_INPUT_REFERENCE = 181,
    PROP_LIMIT_MONITORING_INTERVAL = 182,
    PROP_LOGGING_OBJECT = 183,
    PROP_LOGGING_RECORD = 184,
    PROP_PRESCALE = 185,
    PROP_PULSE_RATE = 186,
    PROP_SCALE = 187,
    PROP_SCALE_FACTOR = 188,
    PROP_UPDATE_TIME = 189,
    PROP_VALUE_BEFORE_CHANGE = 190,
    PROP_VALUE_SET = 191,
    PROP_VALUE_CHANGE_TIME = 192,
    PROP_ALIGN_INTERVALS = 193,
    PROP_INTERVAL_OFFSET = 195,
    PROP_LAST_RESTART_REASON = 196,
    PROP_LOGGING_TYPE = 197,
    PROP_RESTART_NOTIFICATION_RECIPIENTS = 202,
    PROP_TIME_OF_DEVICE_RESTART = 203,
    PROP_TIME_SYNCHRONIZATION_INTERVAL = 204,
    PROP_TRIGGER = 205,
    PROP_UTC_TIME_SYNCHRONIZATION_RECIPIENTS = 206,
    PROP_NODE_SUBTYPE = 207,
    PROP_NODE_TYPE = 208,
    PROP_STRUCTURED_OBJECT_LIST = 209,
    PROP_SUBORDINATE_ANNOTATIONS = 210,
    PROP_SUBORDINATE_LIST = 211,
    PROP_ACTUAL_SHED_LEVEL = 212,
    PROP_DUTY_WINDOW = 213,
    PROP_EXPECTED_SHED_LEVEL = 214,
    PROP_FULL_DUTY_BASELINE = 215,
    PROP_REQUESTED_SHED_LEVEL = 218,
    PROP_SHED_DURATION = 219,
    PROP_SHED_LEVEL_DESCRIPTIONS = 220,
    PROP_SHED_LEVELS = 221,
    PROP_STATE_DESCRIPTION = 222,
    PROP_DOOR_ALARM_STATE = 226,
    PROP_DOOR_EXTENDED_PULSE_TIME = 227,
    PROP_DOOR_MEMBERS = 228,
    PROP_DOOR_OPEN_TOO_LONG_TIME = 229,
    PROP_DOOR_PULSE_TIME = 230,
    PROP_DOOR_STATUS = 231,
    PROP_DOOR_UNLOCK_DELAY_TIME = 232,
    PROP_LOCK_STATUS = 233,
    PROP_MASKED_ALARM_VALUES = 234,
    PROP_SECURED_STATUS = 235,
    PROP_ABSENTEE_LIMIT = 244,
    PROP_ACCESS_ALARM_EVENTS = 245,
    PROP_ACCESS_DOORS = 246,
    PROP_ACCESS_EVENT = 247,
    PROP_ACCESS_EVENT_AUTHENTICATION_FACTOR = 248,
    PROP_ACCESS_EVENT_CREDENTIAL = 249,
    PROP_ACCESS_EVENT_TIME = 250,
    PROP_ACCESS_TRANSACTION_EVENTS = 251,
    PROP_ACCOMPANIMENT = 252,
    PROP_ACCOMPANIMENT_TIME = 253,
    PROP_ACTIVATION_TIME = 254,
    PROP_ACTIVE_AUTHENTICATION_POLICY = 255,
    PROP_ASSIGNED_ACCESS_RIGHTS = 256,
    PROP_AUTHENTICATION_FACTORS = 257,
    PROP_AUTHENTICATION_POLICY_LIST = 258,
    PROP_AUTHENTICATION_POLICY_NAMES = 259,
    PROP_AUTHENTICATION_STATUS = 260,
    PROP_AUTHORIZATION_MODE = 261,
    PROP_BELONGS_TO = 262,
    PROP_CREDENTIAL_DISABLE = 263,
    PROP_CREDENTIAL_STATUS = 264,
    PROP_CREDENTIALS = 265,
    PROP_CREDENTIALS_IN_ZONE = 266,
    PROP_DAYS_REMAINING = 267,
    PROP_ENTRY_POINTS = 268,
    PROP_EXIT_POINTS = 269,
    PROP_EXPIRY_TIME = 270,
    PROP_EXTENDED_TIME_ENABLE = 271,
    PROP_FAILED_ATTEMPT_EVENTS = 272,
    PROP_FAILED_ATTEMPTS = 273,
    PROP_FAILED_ATTEMPTS_TIME = 274,
    PROP_LAST_ACCESS_EVENT = 275,
    PROP_LAST_ACCESS_POINT = 276,
    PROP_LAST_CREDENTIAL_ADDED = 277,
    PROP_LAST_CREDENTIAL_ADDED_TIME = 278,
    PROP_LAST_CREDENTIAL_REMOVED = 279,
    PROP_LAST_CREDENTIAL_REMOVED_TIME = 280,
    PROP_LAST_USE_TIME = 281,
    PROP_LOCKOUT = 282,
    PROP_LOCKOUT_RELINQUISH_TIME = 283,
    PROP_MASTER_EXEMPTION = 284,
    PROP_MAX_FAILED_ATTEMPTS = 285,
    PROP_MEMBERS = 286,
    PROP_MUSTER_POINT = 287,
    PROP_NEGATIVE_ACCESS_RULES = 288,
    PROP_NUMBER_OF_AUTHENTICATION_POLICIES = 289,
    PROP_OCCUPANCY_COUNT = 290,
    PROP_OCCUPANCY_COUNT_ADJUST = 291,
    PROP_OCCUPANCY_COUNT_ENABLE = 292,
    PROP_OCCUPANCY_EXEMPTION = 293,
    PROP_OCCUPANCY_LOWER_LIMIT = 294,
    PROP_OCCUPANCY_LOWER_LIMIT_ENFORCED = 295,
    PROP_OCCUPANCY_STATE = 296,
    PROP_OCCUPANCY_UPPER_LIMIT = 297,
    PROP_OCCUPANCY_UPPER_LIMIT_ENFORCED = 298,
    PROP_PASSBACK_EXEMPTION = 299,
    PROP_PASSBACK_MODE = 300,
    PROP_PASSBACK_TIMEOUT = 301,
    PROP_POSITIVE_ACCESS_RULES = 302,
    PROP_REASON_FOR_DISABLE = 303,
    PROP_SUPPORTED_FORMATS = 304,
    PROP_SUPPORTED_FORMAT_CLASSES = 305,
    PROP_THREAT_AUTHORITY = 306,
    PROP_THREAT_LEVEL = 307,
    PROP_TRACE_FLAG = 308,
    PROP_TRANSACTION_NOTIFICATION_CLASS = 309,
    PROP_USER_EXTERNAL_IDENTIFIER = 310,
    PROP_USER_INFORMATION_REFERENCE = 311,
    PROP_USER_NAME = 317,
    PROP_USER_TYPE = 318,
    PROP_USES_REMAINING = 319,
    PROP_ZONE_FROM = 320,
    PROP_ZONE_TO = 321,
    PROP_ACCESS_EVENT_TAG = 322,
    PROP_GLOBAL_IDENTIFIER = 323,
    PROP_VERIFICATION_TIME = 326,
    PROP_BASE_DEVICE_SECURITY_POLICY = 327,
    PROP_DISTRIBUTION_KEY_REVISION = 328,
    PROP_DO_NOT_HIDE = 329,
    PROP_KEY_SETS = 330,
    PROP_LAST_KEY_SERVER = 331,
    PROP_NETWORK_ACCESS_SECURITY_POLICIES = 332,
    PROP_PACKET_REORDER_TIME = 333,
    PROP_SECURITY_PDU_TIMEOUT = 334,
    PROP_SECURITY_TIME_WINDOW = 335,
    PROP_SUPPORTED_SECURITY_ALGORITHM = 336,
    PROP_UPDATE_KEY_SET_TIMEOUT = 337,
    PROP_BACKUP_AND_RESTORE_STATE = 338,
    PROP_BACKUP_PREPARATION_TIME = 339,
    PROP_RESTORE_COMPLETION_TIME = 340,
    PROP_RESTORE_PREPARATION_TIME = 341,
    PROP_BIT_MASK = 342,
    PROP_BIT_TEXT = 343,
    PROP_IS_UTC = 344,
    PROP_GROUP_MEMBERS = 345,
    PROP_GROUP_MEMBER_NAMES = 346,
    PROP_MEMBER_STATUS_FLAGS = 347,
    PROP_REQUESTED_UPDATE_INTERVAL = 348,
    PROP_COVU_PERIOD = 349,
    PROP_COVU_RECIPIENTS = 350,
    PROP_EVENT_MESSAGE_TEXTS = 351,
    PROP_EVENT_MESSAGE_TEXTS_CONFIG = 352,
    PROP_EVENT_DETECTION_ENABLE = 353,
    PROP_EVENT_ALGORITHM_INHIBIT = 354,
    PROP_EVENT_ALGORITHM_INHIBIT_REF = 355,
    PROP_TIME_DELAY_NORMAL = 356,
    PROP_RELIABILITY_EVALUATION_INHIBIT = 357,
    PROP_FAULT_PARAMETERS = 358,
    PROP_FAULT_TYPE = 359,
    PROP_LOCAL_FORWARDING_ONLY = 360,
    PROP_PROCESS_IDENTIFIER_FILTER = 361,
    PROP_SUBSCRIBED_RECIPIENTS = 362,
    PROP_PORT_FILTER = 363,
    PROP_AUTHORIZATION_EXEMPTIONS = 364,
    PROP_ALLOW_GROUP_DELAY_INHIBIT = 365,
    PROP_CHANNEL_NUMBER = 366,
    PROP_CONTROL_GROUPS = 367,
    PROP_EXECUTION_DELAY = 368,
    PROP_LAST_PRIORITY = 369,
    PROP_WRITE_STATUS = 370,
    PROP_PROPERTY_LIST = 371,
    PROP_SERIAL_NUMBER = 372,
    PROP_BLINK_WARN_ENABLE = 373,
    PROP_DEFAULT_FADE_TIME = 374,
    PROP_DEFAULT_RAMP_RATE = 375,
    PROP_DEFAULT_STEP_INCREMENT = 376,
    PROP_EGRESS_TIME = 377,
    PROP_IN_PROGRESS = 378,
    PROP_INSTANTANEOUS_POWER = 379,
    PROP_LIGHTING_COMMAND = 380,
    PROP_LIGHTING_COMMAND_DEFAULT_PRIORITY = 381,
    PROP_MAX_ACTUAL_VALUE = 382,
    PROP_MIN_ACTUAL_VALUE = 383,
    PROP_POWER = 384,
    PROP_TRANSITION = 385,
    PROP_EGRESS_ACTIVE = 386,
    PROP_INTERFACE_VALUE = 387,
    PROP_FAULT_HIGH_LIMIT = 388,
    PROP_FAULT_LOW_LIMIT = 389,
    PROP_LOW_DIFF_LIMIT = 390,
    PROP_STRIKE_COUNT = 391,
    PROP_TIME_OF_STRIKE_COUNT_RESET = 392,
    PROP_DEFAULT_TIMEOUT = 393,
    PROP_INITIAL_TIMEOUT = 394,
    PROP_LAST_STATE_CHANGE = 395,
    PROP_STATE_CHANGE_VALUES = 396,
    PROP_TIMER_RUNNING = 397,
    PROP_TIMER_STATE = 398,
    PROP_COMMAND_TIME_ARRAY = 430,
    PROP_CURRENT_COMMAND_PRIORITY = 431,
    PROP_LAST_COMMAND_TIME = 432,
    PROP_VALUE_SOURCE = 433,
    PROP_VALUE_SOURCE_ARRAY = 434,
    MAX_BACNET_PROPERTY_ID = 4194303
}
```

## T-3 Bacnet数据类型对照表

因该对照表过于长，所以仅列出部分常用类型

| Property类型       | 对应编号 | 说明    |
| ---------------- | ---- | ----- |
| NULL             | 0    | 空     |
| BOOLEAN          | 1    | 布尔值   |
| UNSIGNED_INT     | 2    | 无符号整型 |
| SIGNED_INT       | 3    | 整形    |
| REAL             | 4    | 实数浮点  |
| DOUBLE           | 5    | 双精度浮点 |
| OCTET_STRING     | 6    |       |
| CHARACTER_STRING | 7    |       |
| BIT_STRING       | 8    |       |
| ENUMERATED       | 9    |       |
| DATE             | 10   | 日期    |
| TIME             | 11   | 时间    |

原始数据：

```csharp
public enum BacnetApplicationTags
{
    BACNET_APPLICATION_TAG_NULL,
    BACNET_APPLICATION_TAG_BOOLEAN,
    BACNET_APPLICATION_TAG_UNSIGNED_INT,
    BACNET_APPLICATION_TAG_SIGNED_INT,
    BACNET_APPLICATION_TAG_REAL,
    BACNET_APPLICATION_TAG_DOUBLE,
    BACNET_APPLICATION_TAG_OCTET_STRING,
    BACNET_APPLICATION_TAG_CHARACTER_STRING,
    BACNET_APPLICATION_TAG_BIT_STRING,
    BACNET_APPLICATION_TAG_ENUMERATED,
    BACNET_APPLICATION_TAG_DATE,
    BACNET_APPLICATION_TAG_TIME,
    BACNET_APPLICATION_TAG_OBJECT_ID,
    BACNET_APPLICATION_TAG_RESERVE1,
    BACNET_APPLICATION_TAG_RESERVE2,
    BACNET_APPLICATION_TAG_RESERVE3,
    MAX_BACNET_APPLICATION_TAG,
    BACNET_APPLICATION_TAG_EMPTYLIST,
    BACNET_APPLICATION_TAG_WEEKNDAY,
    BACNET_APPLICATION_TAG_DATERANGE,
    BACNET_APPLICATION_TAG_DATETIME,
    BACNET_APPLICATION_TAG_TIMESTAMP,
    BACNET_APPLICATION_TAG_ERROR,
    BACNET_APPLICATION_TAG_DEVICE_OBJECT_PROPERTY_REFERENCE,
    BACNET_APPLICATION_TAG_DEVICE_OBJECT_REFERENCE,
    BACNET_APPLICATION_TAG_OBJECT_PROPERTY_REFERENCE,
    BACNET_APPLICATION_TAG_DESTINATION,
    BACNET_APPLICATION_TAG_RECIPIENT,
    BACNET_APPLICATION_TAG_COV_SUBSCRIPTION,
    BACNET_APPLICATION_TAG_CALENDAR_ENTRY,
    BACNET_APPLICATION_TAG_WEEKLY_SCHEDULE,
    BACNET_APPLICATION_TAG_SPECIAL_EVENT,
    BACNET_APPLICATION_TAG_READ_ACCESS_SPECIFICATION,
    BACNET_APPLICATION_TAG_READ_ACCESS_RESULT,
    BACNET_APPLICATION_TAG_LIGHTING_COMMAND,
    BACNET_APPLICATION_TAG_CONTEXT_SPECIFIC_DECODED,
    BACNET_APPLICATION_TAG_CONTEXT_SPECIFIC_ENCODED,
    BACNET_APPLICATION_TAG_LOG_RECORD
}
```
