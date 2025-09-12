namespace Infrastructure.Utilities;

/// <summary>
/// 雪花ID生成器
/// </summary>
public class SnowflakeId
{
    /// <summary>
    /// 起始时间戳（2010-11-04 09:42:54）
    /// </summary>
    public const long Twepoch = 1288834974657L;

    /// <summary>
    /// 工作机器ID位数
    /// </summary>
    private const int WorkerIdBits = 5;

    /// <summary>
    /// 数据中心ID位数
    /// </summary>
    private const int DatacenterIdBits = 5;

    /// <summary>
    /// 序列号位数
    /// </summary>
    private const int SequenceBits = 12;

    /// <summary>
    /// 最大工作机器ID
    /// </summary>
    private const long MaxWorkerId = 31L;

    /// <summary>
    /// 最大数据中心ID
    /// </summary>
    private const long MaxDatacenterId = 31L;

    /// <summary>
    /// 工作机器ID左移位数
    /// </summary>
    private const int WorkerIdShift = 12;

    /// <summary>
    /// 数据中心ID左移位数
    /// </summary>
    private const int DatacenterIdShift = 17;

    /// <summary>
    /// 时间戳左移位数
    /// </summary>
    public const int TimestampLeftShift = 22;

    /// <summary>
    /// 序列号掩码
    /// </summary>
    private const long SequenceMask = 4095L;

    /// <summary>
    /// 单例实例
    /// </summary>
    private static SnowflakeId? _snowflakeId;

    /// <summary>
    /// 线程锁
    /// </summary>
    private readonly object _lock = new object();

    /// <summary>
    /// 静态锁
    /// </summary>
    private static readonly object SLock = new object();

    /// <summary>
    /// 上次生成ID的时间戳
    /// </summary>
    private long _lastTimestamp = -1L;

    /// <summary>
    /// 工作机器ID
    /// </summary>
    public long WorkerId { get; protected set; }

    /// <summary>
    /// 数据中心ID
    /// </summary>
    public long DatacenterId { get; protected set; }

    /// <summary>
    /// 序列号
    /// </summary>
    public long Sequence { get; internal set; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="workerId">工作机器ID</param>
    /// <param name="datacenterId">数据中心ID</param>
    /// <param name="sequence">序列号</param>
    public SnowflakeId(long workerId, long datacenterId, long sequence = 0L)
    {
        WorkerId = workerId;
        DatacenterId = datacenterId;
        Sequence = sequence;
        
        if (workerId > MaxWorkerId || workerId < 0)
        {
            throw new ArgumentException($"工作机器ID不能大于 {MaxWorkerId} 或小于 0");
        }

        if (datacenterId > MaxDatacenterId || datacenterId < 0)
        {
            throw new ArgumentException($"数据中心ID不能大于 {MaxDatacenterId} 或小于 0");
        }
    }

    /// <summary>
    /// 获取默认实例
    /// </summary>
    /// <returns>SnowflakeId实例</returns>
    public static SnowflakeId Default()
    {
        lock (SLock)
        {
            if (_snowflakeId != null)
            {
                return _snowflakeId;
            }

            Random random = new Random();
            
            // 尝试从环境变量获取工作机器ID
            if (!int.TryParse(Environment.GetEnvironmentVariable("CAP_WORKERID", EnvironmentVariableTarget.Machine), out var workerId))
            {
                workerId = random.Next((int)MaxWorkerId);
            }

            // 尝试从环境变量获取数据中心ID
            if (!int.TryParse(Environment.GetEnvironmentVariable("CAP_DATACENTERID", EnvironmentVariableTarget.Machine), out var datacenterId))
            {
                datacenterId = random.Next((int)MaxDatacenterId);
            }

            return _snowflakeId = new SnowflakeId(workerId, datacenterId, 0L);
        }
    }

    /// <summary>
    /// 生成下一个ID
    /// </summary>
    /// <returns>雪花ID</returns>
    public virtual long NextId()
    {
        lock (_lock)
        {
            long timestamp = TimeGen();
            
            if (timestamp < _lastTimestamp)
            {
                throw new Exception($"时钟回拨异常：时钟向后移动了 {_lastTimestamp - timestamp} 毫秒，拒绝生成ID");
            }

            if (_lastTimestamp == timestamp)
            {
                Sequence = (Sequence + 1) & SequenceMask;
                if (Sequence == 0)
                {
                    timestamp = TilNextMillis(_lastTimestamp);
                }
            }
            else
            {
                Sequence = 0L;
            }

            _lastTimestamp = timestamp;
            
            return ((timestamp - Twepoch) << TimestampLeftShift) | 
                   (DatacenterId << DatacenterIdShift) | 
                   (WorkerId << WorkerIdShift) | 
                   Sequence;
        }
    }

    /// <summary>
    /// 等待下一毫秒
    /// </summary>
    /// <param name="lastTimestamp">上次时间戳</param>
    /// <returns>下一毫秒时间戳</returns>
    protected virtual long TilNextMillis(long lastTimestamp)
    {
        long timestamp;
        for (timestamp = TimeGen(); timestamp <= lastTimestamp; timestamp = TimeGen())
        {
            // 等待下一毫秒
        }

        return timestamp;
    }

    /// <summary>
    /// 获取当前时间戳
    /// </summary>
    /// <returns>当前时间戳</returns>
    protected virtual long TimeGen()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    /// <summary>
    /// 解析雪花ID
    /// </summary>
    /// <param name="id">雪花ID</param>
    /// <returns>解析结果</returns>
    public static SnowflakeIdInfo ParseId(long id)
    {
        long timestamp = (id >> TimestampLeftShift) + Twepoch;
        long datacenterId = (id >> DatacenterIdShift) & MaxDatacenterId;
        long workerId = (id >> WorkerIdShift) & MaxWorkerId;
        long sequence = id & SequenceMask;

        return new SnowflakeIdInfo
        {
            Id = id,
            Timestamp = timestamp,
            DateTime = DateTimeOffset.FromUnixTimeMilliseconds(timestamp).DateTime,
            DatacenterId = datacenterId,
            WorkerId = workerId,
            Sequence = sequence
        };
    }

    /// <summary>
    /// 生成字符串格式的ID
    /// </summary>
    /// <returns>字符串ID</returns>
    public string NextIdString()
    {
        return NextId().ToString();
    }
}

/// <summary>
/// 雪花ID信息
/// </summary>
public class SnowflakeIdInfo
{
    /// <summary>
    /// 雪花ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 时间戳
    /// </summary>
    public long Timestamp { get; set; }

    /// <summary>
    /// 日期时间
    /// </summary>
    public DateTime DateTime { get; set; }

    /// <summary>
    /// 数据中心ID
    /// </summary>
    public long DatacenterId { get; set; }

    /// <summary>
    /// 工作机器ID
    /// </summary>
    public long WorkerId { get; set; }

    /// <summary>
    /// 序列号
    /// </summary>
    public long Sequence { get; set; }

    /// <summary>
    /// 转换为字符串
    /// </summary>
    /// <returns>字符串表示</returns>
    public override string ToString()
    {
        return $"Id: {Id}, DateTime: {DateTime:yyyy-MM-dd HH:mm:ss.fff}, DatacenterId: {DatacenterId}, WorkerId: {WorkerId}, Sequence: {Sequence}";
    }
}
