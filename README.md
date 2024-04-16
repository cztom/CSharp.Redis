# CSharp.Redis
Redis 帮助类
1.在 appsettings.json 文件中配置 Redis 连接字符串
{
     "RedisOptions": {
         "ConnectionString": "127.0.0.1:6379,password=123123,connectTimeout=1000,connectRetry=1,syncTimeout=10000",
         "DbNumber": 0
     }
}

2.在 Startup.cs 中注入
builder.Services.AddRedis(builder.Configuration.GetSection(nameof(RedisOptions)));
