using System.ComponentModel.DataAnnotations;

namespace CSharp.Redis
{
    /// <summary>
    /// Redis 在 appsettings.json 中的配置类
    /// </summary>
    public class RedisOptions
    {
        /// <summary>
        /// Redis 连接字符串
        /// </summary>
        [Required(ErrorMessage = "redis connection string is required")]
        public string ConnectionString { get; set; }

        /// <summary>
        /// Redis 数据库编号
        /// </summary>
        [Range(0, 15, ErrorMessage = "redis db number must be between 0 and 15")]
        public int DbNumber { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public RedisOptions()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connectionString">Redis 连接字符串</param>
        /// <param name="dbNumber">Redis 数据库编号</param>
        public RedisOptions(string connectionString, int dbNumber)
        {
            ConnectionString = connectionString;
            DbNumber = dbNumber;
        }
    }
}
