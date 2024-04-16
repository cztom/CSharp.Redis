using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Net;
using Timer = System.Timers.Timer;

namespace CSharp.Redis
{
    /// <summary>
    /// Redis 帮助类
    /// <code>
    /// 1.在 appsettings.json 文件中配置 Redis 连接字符串
    /// {
    ///     "RedisOptions": {
    ///         "ConnectionString": "127.0.0.1:6379,password=123123,connectTimeout=1000,connectRetry=1,syncTimeout=10000",
    ///         "DbNumber": 0
    ///     }
    /// }
    /// 
    /// 2.在 Startup.cs 中注入
    /// builder.Services.AddRedis(builder.Configuration.GetSection(nameof(RedisOptions)));
    /// </code>
    /// </summary>
    public class RedisHelper : IRedisHelper
    {
        private ConnectionMultiplexer _conn;
        private IDatabase _db;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="options"></param>
        public RedisHelper(IOptionsMonitor<RedisOptions> options) : this(options.CurrentValue)
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="options"></param>
        public RedisHelper(RedisOptions options)
        {
            _conn = ConnectionMultiplexer.Connect(options.ConnectionString);
            _db = _conn.GetDatabase(options.DbNumber);
        }

        /// <summary>
        /// 选择数据库
        /// </summary>
        /// <param name="dbNumber">数据库编号（默认是编号0的数据库）</param>
        public void GetDatabase(int dbNumber = 0)
        {
            _db = _conn.GetDatabase(dbNumber);
        }

        #region Geo

        /// <summary>
        /// Add the specified member to the set stored at key.
        /// Specified members that are already a member of this set are ignored.
        /// If key does not exist, a new set is created before adding the specified members.
        /// </summary>
        /// <param name="key">The key of the set.</param>
        /// <param name="longitude">The longitude of geo entry.</param>
        /// <param name="latitude">The latitude of the geo entry.</param>
        /// <param name="member">The value to set at this entry.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns><see langword="true"/> if the specified member was not already present in the set, else <see langword="false"/>.</returns>
        /// <remarks><seealso href="https://redis.io/commands/geoadd"/></remarks>
        public bool GeoAdd(RedisKey key, double longitude, double latitude, RedisValue member, CommandFlags flags = CommandFlags.None)
        {
            return _db.GeoAdd(key, longitude, latitude, member, flags);
        }

        /// <summary>
        /// Add the specified member to the set stored at key.
        /// Specified members that are already a member of this set are ignored.
        /// If key does not exist, a new set is created before adding the specified members.
        /// </summary>
        /// <param name="key">The key of the set.</param>
        /// <param name="value">The geo value to store.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns><see langword="true"/> if the specified member was not already present in the set, else <see langword="false"/>.</returns>
        /// <remarks><seealso href="https://redis.io/commands/geoadd"/></remarks>
        public bool GeoAdd(RedisKey key, GeoEntry value, CommandFlags flags = CommandFlags.None)
        {
            return _db.GeoAdd(key, value, flags);
        }

        /// <summary>
        /// Add the specified members to the set stored at key.
        /// Specified members that are already a member of this set are ignored.
        /// If key does not exist, a new set is created before adding the specified members.
        /// </summary>
        /// <param name="key">The key of the set.</param>
        /// <param name="values">The geo values add to the set.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The number of elements that were added to the set, not including all the elements already present into the set.</returns>
        /// <remarks><seealso href="https://redis.io/commands/geoadd"/></remarks>
        public long GeoAdd(RedisKey key, GeoEntry[] values, CommandFlags flags = CommandFlags.None)
        {
            return _db.GeoAdd(key, values, flags);
        }

        /// <summary>
        /// Removes the specified member from the geo sorted set stored at key.
        /// Non existing members are ignored.
        /// </summary>
        /// <param name="key">The key of the set.</param>
        /// <param name="member">The geo value to remove.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns><see langword="true"/> if the member existed in the sorted set and was removed, else <see langword="false"/>.</returns>
        /// <remarks><seealso href="https://redis.io/commands/zrem"/></remarks>
        public bool GeoRemove(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None)
        {
            return _db.GeoRemove(key, member, flags);
        }

        /// <summary>
        /// Return the distance between two members in the geospatial index represented by the sorted set.
        /// </summary>
        /// <param name="key">The key of the set.</param>
        /// <param name="member1">The first member to check.</param>
        /// <param name="member2">The second member to check.</param>
        /// <param name="unit">The unit of distance to return (defaults to meters).</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The command returns the distance as a double (represented as a string) in the specified unit, or <see langword="null"/> if one or both the elements are missing.</returns>
        /// <remarks><seealso href="https://redis.io/commands/geodist"/></remarks>
        public double? GeoDistance(RedisKey key, RedisValue member1, RedisValue member2, GeoUnit unit = GeoUnit.Meters, CommandFlags flags = CommandFlags.None)
        {
            return _db.GeoDistance(key, member1, member2, unit, flags);
        }


        /// <summary>
        /// Return valid Geohash strings representing the position of one or more elements in a sorted set value representing a geospatial index (where elements were added using GEOADD).
        /// </summary>
        /// <param name="key">The key of the set.</param>
        /// <param name="members">The members to get.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The command returns an array where each element is the Geohash corresponding to each member name passed as argument to the command.</returns>
        /// <remarks><seealso href="https://redis.io/commands/geohash"/></remarks>
        public string?[] GeoHash(RedisKey key, RedisValue[] members, CommandFlags flags = CommandFlags.None)
        {
            return _db.GeoHash(key, members, flags);
        }

        /// <summary>
        /// Return valid Geohash strings representing the position of one or more elements in a sorted set value representing a geospatial index (where elements were added using GEOADD).
        /// </summary>
        /// <param name="key">The key of the set.</param>
        /// <param name="member">The member to get.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The command returns an array where each element is the Geohash corresponding to each member name passed as argument to the command.</returns>
        /// <remarks><seealso href="https://redis.io/commands/geohash"/></remarks>
        public string? GeoHash(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None)
        {
            return _db.GeoHash(key, member, flags);
        }

        /// <summary>
        /// Return the positions (longitude,latitude) of all the specified members of the geospatial index represented by the sorted set at key.
        /// </summary>
        /// <param name="key">The key of the set.</param>
        /// <param name="members">The members to get.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>
        /// The command returns an array where each element is a two elements array representing longitude and latitude (x,y) of each member name passed as argument to the command.
        /// Non existing elements are reported as NULL elements of the array.
        /// </returns>
        /// <remarks><seealso href="https://redis.io/commands/geopos"/></remarks>
        public GeoPosition?[] GeoPosition(RedisKey key, RedisValue[] members, CommandFlags flags = CommandFlags.None)
        {
            return _db.GeoPosition(key, members, flags);
        }

        /// <summary>
        /// Return the positions (longitude,latitude) of all the specified members of the geospatial index represented by the sorted set at key.
        /// </summary>
        /// <param name="key">The key of the set.</param>
        /// <param name="member">The member to get.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>
        /// The command returns an array where each element is a two elements array representing longitude and latitude (x,y) of each member name passed as argument to the command.
        /// Non existing elements are reported as NULL elements of the array.
        /// </returns>
        /// <remarks><seealso href="https://redis.io/commands/geopos"/></remarks>
        public GeoPosition? GeoPosition(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None)
        {
            return _db.GeoPosition(key, member, flags);
        }

        /// <summary>
        /// Return the members of a sorted set populated with geospatial information using GEOADD, which are
        /// within the borders of the area specified with the center location and the maximum distance from the center (the radius).
        /// </summary>
        /// <param name="key">The key of the set.</param>
        /// <param name="member">The member to get a radius of results from.</param>
        /// <param name="radius">The radius to check.</param>
        /// <param name="unit">The unit of <paramref name="radius"/> (defaults to meters).</param>
        /// <param name="count">The count of results to get, -1 for unlimited.</param>
        /// <param name="order">The order of the results.</param>
        /// <param name="options">The search options to use.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The results found within the radius, if any.</returns>
        /// <remarks><seealso href="https://redis.io/commands/georadius"/></remarks>
        public GeoRadiusResult[] GeoRadius(RedisKey key, RedisValue member, double radius, GeoUnit unit = GeoUnit.Meters, int count = -1, Order? order = null, GeoRadiusOptions options = GeoRadiusOptions.Default, CommandFlags flags = CommandFlags.None)
        {
            return _db.GeoRadius(key, member, radius, unit, count, order, options, flags);
        }

        /// <summary>
        /// Return the members of a sorted set populated with geospatial information using GEOADD, which are
        /// within the borders of the area specified with the center location and the maximum distance from the center (the radius).
        /// </summary>
        /// <param name="key">The key of the set.</param>
        /// <param name="longitude">The longitude of the point to get a radius of results from.</param>
        /// <param name="latitude">The latitude of the point to get a radius of results from.</param>
        /// <param name="radius">The radius to check.</param>
        /// <param name="unit">The unit of <paramref name="radius"/> (defaults to meters).</param>
        /// <param name="count">The count of results to get, -1 for unlimited.</param>
        /// <param name="order">The order of the results.</param>
        /// <param name="options">The search options to use.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The results found within the radius, if any.</returns>
        /// <remarks><seealso href="https://redis.io/commands/georadius"/></remarks>
        public GeoRadiusResult[] GeoRadius(RedisKey key, double longitude, double latitude, double radius, GeoUnit unit = GeoUnit.Meters, int count = -1, Order? order = null, GeoRadiusOptions options = GeoRadiusOptions.Default, CommandFlags flags = CommandFlags.None)
        {
            return _db.GeoRadius(key, longitude, latitude, radius, unit, count, order, options, flags);
        }

        /// <summary>
        /// Return the members of the geo-encoded sorted set stored at <paramref name="key"/> bounded by the provided
        /// <paramref name="shape"/>, centered at the provided set <paramref name="member"/>.
        /// </summary>
        /// <param name="key">The key of the set.</param>
        /// <param name="member">The set member to use as the center of the shape.</param>
        /// <param name="shape">The shape to use to bound the geo search.</param>
        /// <param name="count">The maximum number of results to pull back.</param>
        /// <param name="demandClosest">Whether or not to terminate the search after finding <paramref name="count"/> results. Must be true of count is -1.</param>
        /// <param name="order">The order to sort by (defaults to unordered).</param>
        /// <param name="options">The search options to use.</param>
        /// <param name="flags">The flags for this operation.</param>
        /// <returns>The results found within the shape, if any.</returns>
        /// <remarks><seealso href="https://redis.io/commands/geosearch"/></remarks>
        public GeoRadiusResult[] GeoSearch(RedisKey key, RedisValue member, GeoSearchShape shape, int count = -1, bool demandClosest = true, Order? order = null, GeoRadiusOptions options = GeoRadiusOptions.Default, CommandFlags flags = CommandFlags.None)
        {
            return _db.GeoSearch(key, member, shape, count, demandClosest, order, options, flags);
        }

        /// <summary>
        /// Return the members of the geo-encoded sorted set stored at <paramref name="key"/> bounded by the provided
        /// <paramref name="shape"/>, centered at the point provided by the <paramref name="longitude"/> and <paramref name="latitude"/>.
        /// </summary>
        /// <param name="key">The key of the set.</param>
        /// <param name="longitude">The longitude of the center point.</param>
        /// <param name="latitude">The latitude of the center point.</param>
        /// <param name="shape">The shape to use to bound the geo search.</param>
        /// <param name="count">The maximum number of results to pull back.</param>
        /// <param name="demandClosest">Whether or not to terminate the search after finding <paramref name="count"/> results. Must be true of count is -1.</param>
        /// <param name="order">The order to sort by (defaults to unordered).</param>
        /// <param name="options">The search options to use.</param>
        /// <param name="flags">The flags for this operation.</param>
        /// <returns>The results found within the shape, if any.</returns>
        /// <remarks><seealso href="https://redis.io/commands/geosearch"/></remarks>
        public GeoRadiusResult[] GeoSearch(RedisKey key, double longitude, double latitude, GeoSearchShape shape, int count = -1, bool demandClosest = true, Order? order = null, GeoRadiusOptions options = GeoRadiusOptions.Default, CommandFlags flags = CommandFlags.None)
        {
            return _db.GeoSearch(key, longitude, latitude, shape, count, demandClosest, order, options, flags);
        }

        /// <summary>
        /// Stores the members of the geo-encoded sorted set stored at <paramref name="sourceKey"/> bounded by the provided
        /// <paramref name="shape"/>, centered at the provided set <paramref name="member"/>.
        /// </summary>
        /// <param name="sourceKey">The key of the set.</param>
        /// <param name="destinationKey">The key to store the result at.</param>
        /// <param name="member">The set member to use as the center of the shape.</param>
        /// <param name="shape">The shape to use to bound the geo search.</param>
        /// <param name="count">The maximum number of results to pull back.</param>
        /// <param name="demandClosest">Whether or not to terminate the search after finding <paramref name="count"/> results. Must be true of count is -1.</param>
        /// <param name="order">The order to sort by (defaults to unordered).</param>
        /// <param name="storeDistances">If set to true, the resulting set will be a regular sorted-set containing only distances, rather than a geo-encoded sorted-set.</param>
        /// <param name="flags">The flags for this operation.</param>
        /// <returns>The size of the set stored at <paramref name="destinationKey"/>.</returns>
        /// <remarks><seealso href="https://redis.io/commands/geosearchstore"/></remarks>
        public long GeoSearchAndStore(RedisKey sourceKey, RedisKey destinationKey, RedisValue member, GeoSearchShape shape, int count = -1, bool demandClosest = true, Order? order = null, bool storeDistances = false, CommandFlags flags = CommandFlags.None)
        {
            return _db.GeoSearchAndStore(sourceKey, destinationKey, member, shape, count, demandClosest, order, storeDistances, flags);
        }

        /// <summary>
        /// Stores the members of the geo-encoded sorted set stored at <paramref name="sourceKey"/> bounded by the provided
        /// <paramref name="shape"/>, centered at the point provided by the <paramref name="longitude"/> and <paramref name="latitude"/>.
        /// </summary>
        /// <param name="sourceKey">The key of the set.</param>
        /// <param name="destinationKey">The key to store the result at.</param>
        /// <param name="longitude">The longitude of the center point.</param>
        /// <param name="latitude">The latitude of the center point.</param>
        /// <param name="shape">The shape to use to bound the geo search.</param>
        /// <param name="count">The maximum number of results to pull back.</param>
        /// <param name="demandClosest">Whether or not to terminate the search after finding <paramref name="count"/> results. Must be true of count is -1.</param>
        /// <param name="order">The order to sort by (defaults to unordered).</param>
        /// <param name="storeDistances">If set to true, the resulting set will be a regular sorted-set containing only distances, rather than a geo-encoded sorted-set.</param>
        /// <param name="flags">The flags for this operation.</param>
        /// <returns>The size of the set stored at <paramref name="destinationKey"/>.</returns>
        /// <remarks><seealso href="https://redis.io/commands/geosearchstore"/></remarks>
        public long GeoSearchAndStore(RedisKey sourceKey, RedisKey destinationKey, double longitude, double latitude, GeoSearchShape shape, int count = -1, bool demandClosest = true, Order? order = null, bool storeDistances = false, CommandFlags flags = CommandFlags.None)
        {
            return _db.GeoSearchAndStore(sourceKey, destinationKey, longitude, latitude, shape, count, demandClosest, order, storeDistances, flags);
        }

        #endregion

        #region Geo Async

        /// <summary>
        /// Add the specified member to the set stored at key.
        /// Specified members that are already a member of this set are ignored.
        /// If key does not exist, a new set is created before adding the specified members.
        /// </summary>
        /// <param name="key">The key of the set.</param>
        /// <param name="longitude">The longitude of geo entry.</param>
        /// <param name="latitude">The latitude of the geo entry.</param>
        /// <param name="member">The value to set at this entry.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns><see langword="true"/> if the specified member was not already present in the set, else <see langword="false"/>.</returns>
        /// <remarks><seealso href="https://redis.io/commands/geoadd"/></remarks>
        public async Task<bool> GeoAddAsync<T>(string key, double longitude, double latitude, T member, CommandFlags flags = CommandFlags.None)
        {
            return await _db.GeoAddAsync(key, longitude, latitude, member.ToRedisValue<T>(), flags);
        }

        /// <summary>
        /// Add the specified member to the set stored at key.
        /// Specified members that are already a member of this set are ignored.
        /// If key does not exist, a new set is created before adding the specified members.
        /// </summary>
        /// <param name="key">The key of the set.</param>
        /// <param name="value">The geo value to store.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns><see langword="true"/> if the specified member was not already present in the set, else <see langword="false"/>.</returns>
        /// <remarks><seealso href="https://redis.io/commands/geoadd"/></remarks>
        public async Task<bool> GeoAddAsync(string key, GeoEntry value, CommandFlags flags = CommandFlags.None)
        {
            return await _db.GeoAddAsync(key, value, flags);
        }

        /// <summary>
        /// Add the specified members to the set stored at key.
        /// Specified members that are already a member of this set are ignored.
        /// If key does not exist, a new set is created before adding the specified members.
        /// </summary>
        /// <param name="key">The key of the set.</param>
        /// <param name="values">The geo values add to the set.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The number of elements that were added to the set, not including all the elements already present into the set.</returns>
        /// <remarks><seealso href="https://redis.io/commands/geoadd"/></remarks>
        public async Task<long> GeoAddAsync(string key, GeoEntry[] values, CommandFlags flags = CommandFlags.None)
        {
            return await _db.GeoAddAsync(key, values, flags);
        }

        /// <summary>
        /// Removes the specified member from the geo sorted set stored at key.
        /// Non existing members are ignored.
        /// </summary>
        /// <param name="key">The key of the set.</param>
        /// <param name="member">The geo value to remove.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns><see langword="true"/> if the member existed in the sorted set and was removed, else <see langword="false"/>.</returns>
        /// <remarks><seealso href="https://redis.io/commands/zrem"/></remarks>
        public async Task<bool> GeoRemoveAsync<T>(string key, T member, CommandFlags flags = CommandFlags.None)
        {
            return await _db.GeoRemoveAsync(key, member.ToRedisValue<T>(), flags);
        }

        /// <summary>
        /// Return the distance between two members in the geospatial index represented by the sorted set.
        /// </summary>
        /// <param name="key">The key of the set.</param>
        /// <param name="member1">The first member to check.</param>
        /// <param name="member2">The second member to check.</param>
        /// <param name="unit">The unit of distance to return (defaults to meters).</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The command returns the distance as a double (represented as a string) in the specified unit, or <see langword="null"/> if one or both the elements are missing.</returns>
        /// <remarks><seealso href="https://redis.io/commands/geodist"/></remarks>
        public async Task<double?> GeoDistanceAsync<T>(string key, T member1, T member2, GeoUnit unit = GeoUnit.Meters, CommandFlags flags = CommandFlags.None)
        {
            return await _db.GeoDistanceAsync(key, member1.ToRedisValue<T>(), member2.ToRedisValue<T>(), unit, flags);
        }

        /// <summary>
        /// Return valid Geohash strings representing the position of one or more elements in a sorted set value representing a geospatial index (where elements were added using GEOADD).
        /// </summary>
        /// <param name="key">The key of the set.</param>
        /// <param name="members">The members to get.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The command returns an array where each element is the Geohash corresponding to each member name passed as argument to the command.</returns>
        /// <remarks><seealso href="https://redis.io/commands/geohash"/></remarks>
        public async Task<string?[]> GeoHashAsync<T>(string key, T[] members, CommandFlags flags = CommandFlags.None)
        {
            return await _db.GeoHashAsync(key, members.ToRedisValues<T>(), flags);
        }

        /// <summary>
        /// Return valid Geohash strings representing the position of one or more elements in a sorted set value representing a geospatial index (where elements were added using GEOADD).
        /// </summary>
        /// <param name="key">The key of the set.</param>
        /// <param name="member">The member to get.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The command returns an array where each element is the Geohash corresponding to each member name passed as argument to the command.</returns>
        /// <remarks><seealso href="https://redis.io/commands/geohash"/></remarks>
        public async Task<string?> GeoHashAsync<T>(string key, T member, CommandFlags flags = CommandFlags.None)
        {
            return await _db.GeoHashAsync(key, member.ToRedisValue<T>(), flags);
        }

        /// <summary>
        /// Return the positions (longitude,latitude) of all the specified members of the geospatial index represented by the sorted set at key.
        /// </summary>
        /// <param name="key">The key of the set.</param>
        /// <param name="members">The members to get.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>
        /// The command returns an array where each element is a two elements array representing longitude and latitude (x,y) of each member name passed as argument to the command.
        /// Non existing elements are reported as NULL elements of the array.
        /// </returns>
        /// <remarks><seealso href="https://redis.io/commands/geopos"/></remarks>
        public async Task<GeoPosition?[]> GeoPositionAsync<T>(string key, T[] members, CommandFlags flags = CommandFlags.None)
        {
            return await _db.GeoPositionAsync(key, members.ToRedisValues<T>(), flags);
        }

        /// <summary>
        /// Return the positions (longitude,latitude) of all the specified members of the geospatial index represented by the sorted set at key.
        /// </summary>
        /// <param name="key">The key of the set.</param>
        /// <param name="member">The member to get.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>
        /// The command returns an array where each element is a two elements array representing longitude and latitude (x,y) of each member name passed as argument to the command.
        /// Non existing elements are reported as NULL elements of the array.
        /// </returns>
        /// <remarks><seealso href="https://redis.io/commands/geopos"/></remarks>
        public async Task<GeoPosition?> GeoPositionAsync<T>(string key, T member, CommandFlags flags = CommandFlags.None)
        {
            return await _db.GeoPositionAsync(key, member.ToRedisValue<T>(), flags);
        }

        /// <summary>
        /// Return the members of a sorted set populated with geospatial information using GEOADD, which are
        /// within the borders of the area specified with the center location and the maximum distance from the center (the radius).
        /// </summary>
        /// <param name="key">The key of the set.</param>
        /// <param name="member">The member to get a radius of results from.</param>
        /// <param name="radius">The radius to check.</param>
        /// <param name="unit">The unit of <paramref name="radius"/> (defaults to meters).</param>
        /// <param name="count">The count of results to get, -1 for unlimited.</param>
        /// <param name="order">The order of the results.</param>
        /// <param name="options">The search options to use.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The results found within the radius, if any.</returns>
        /// <remarks><seealso href="https://redis.io/commands/georadius"/></remarks>
        public async Task<GeoRadiusResult[]> GeoRadiusAsync<T>(string key, T member, double radius, GeoUnit unit = GeoUnit.Meters, int count = -1, Order? order = null, GeoRadiusOptions options = GeoRadiusOptions.Default, CommandFlags flags = CommandFlags.None)
        {
            return await _db.GeoRadiusAsync(key, member.ToRedisValue<T>(), radius, unit, count, order, options, flags);
        }

        /// <summary>
        /// Return the members of a sorted set populated with geospatial information using GEOADD, which are
        /// within the borders of the area specified with the center location and the maximum distance from the center (the radius).
        /// </summary>
        /// <param name="key">The key of the set.</param>
        /// <param name="longitude">The longitude of the point to get a radius of results from.</param>
        /// <param name="latitude">The latitude of the point to get a radius of results from.</param>
        /// <param name="radius">The radius to check.</param>
        /// <param name="unit">The unit of <paramref name="radius"/> (defaults to meters).</param>
        /// <param name="count">The count of results to get, -1 for unlimited.</param>
        /// <param name="order">The order of the results.</param>
        /// <param name="options">The search options to use.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The results found within the radius, if any.</returns>
        /// <remarks><seealso href="https://redis.io/commands/georadius"/></remarks>
        public async Task<GeoRadiusResult[]> GeoRadiusAsync(string key, double longitude, double latitude, double radius, GeoUnit unit = GeoUnit.Meters, int count = -1, Order? order = null, GeoRadiusOptions options = GeoRadiusOptions.Default, CommandFlags flags = CommandFlags.None)
        {
            return await _db.GeoRadiusAsync(key, longitude, latitude, radius, unit, count, order, options, flags);
        }

        /// <summary>
        /// Return the members of the geo-encoded sorted set stored at <paramref name="key"/> bounded by the provided
        /// <paramref name="shape"/>, centered at the provided set <paramref name="member"/>.
        /// </summary>
        /// <param name="key">The key of the set.</param>
        /// <param name="member">The set member to use as the center of the shape.</param>
        /// <param name="shape">The shape to use to bound the geo search.</param>
        /// <param name="count">The maximum number of results to pull back.</param>
        /// <param name="demandClosest">Whether or not to terminate the search after finding <paramref name="count"/> results. Must be true of count is -1.</param>
        /// <param name="order">The order to sort by (defaults to unordered).</param>
        /// <param name="options">The search options to use.</param>
        /// <param name="flags">The flags for this operation.</param>
        /// <returns>The results found within the shape, if any.</returns>
        /// <remarks><seealso href="https://redis.io/commands/geosearch"/></remarks>
        public async Task<GeoRadiusResult[]> GeoSearchAsync<T>(string key, T member, GeoSearchShape shape, int count = -1, bool demandClosest = true, Order? order = null, GeoRadiusOptions options = GeoRadiusOptions.Default, CommandFlags flags = CommandFlags.None)
        {
            return await _db.GeoSearchAsync(key, member.ToRedisValue<T>(), shape, count, demandClosest, order, options, flags);
        }

        /// <summary>
        /// Return the members of the geo-encoded sorted set stored at <paramref name="key"/> bounded by the provided
        /// <paramref name="shape"/>, centered at the point provided by the <paramref name="longitude"/> and <paramref name="latitude"/>.
        /// </summary>
        /// <param name="key">The key of the set.</param>
        /// <param name="longitude">The longitude of the center point.</param>
        /// <param name="latitude">The latitude of the center point.</param>
        /// <param name="shape">The shape to use to bound the geo search.</param>
        /// <param name="count">The maximum number of results to pull back.</param>
        /// <param name="demandClosest">Whether or not to terminate the search after finding <paramref name="count"/> results. Must be true of count is -1.</param>
        /// <param name="order">The order to sort by (defaults to unordered).</param>
        /// <param name="options">The search options to use.</param>
        /// <param name="flags">The flags for this operation.</param>
        /// <returns>The results found within the shape, if any.</returns>
        /// <remarks><seealso href="https://redis.io/commands/geosearch"/></remarks>
        public async Task<GeoRadiusResult[]> GeoSearchAsync(string key, double longitude, double latitude, GeoSearchShape shape, int count = -1, bool demandClosest = true, Order? order = null, GeoRadiusOptions options = GeoRadiusOptions.Default, CommandFlags flags = CommandFlags.None)
        {
            return await _db.GeoSearchAsync(key, longitude, latitude, shape, count, demandClosest, order, options, flags);
        }

        /// <summary>
        /// Stores the members of the geo-encoded sorted set stored at <paramref name="sourceKey"/> bounded by the provided
        /// <paramref name="shape"/>, centered at the provided set <paramref name="member"/>.
        /// </summary>
        /// <param name="sourceKey">The key of the set.</param>
        /// <param name="destinationKey">The key to store the result at.</param>
        /// <param name="member">The set member to use as the center of the shape.</param>
        /// <param name="shape">The shape to use to bound the geo search.</param>
        /// <param name="count">The maximum number of results to pull back.</param>
        /// <param name="demandClosest">Whether or not to terminate the search after finding <paramref name="count"/> results. Must be true of count is -1.</param>
        /// <param name="order">The order to sort by (defaults to unordered).</param>
        /// <param name="storeDistances">If set to true, the resulting set will be a regular sorted-set containing only distances, rather than a geo-encoded sorted-set.</param>
        /// <param name="flags">The flags for this operation.</param>
        /// <returns>The size of the set stored at <paramref name="destinationKey"/>.</returns>
        /// <remarks><seealso href="https://redis.io/commands/geosearchstore"/></remarks>
        public async Task<long> GeoSearchAndStoreAsync<T>(string sourceKey, string destinationKey, T member, GeoSearchShape shape, int count = -1, bool demandClosest = true, Order? order = null, bool storeDistances = false, CommandFlags flags = CommandFlags.None)
        {
            return await _db.GeoSearchAndStoreAsync(sourceKey, destinationKey, member.ToRedisValue<T>(), shape, count, demandClosest, order, storeDistances, flags);
        }

        /// <summary>
        /// Stores the members of the geo-encoded sorted set stored at <paramref name="sourceKey"/> bounded by the provided
        /// <paramref name="shape"/>, centered at the point provided by the <paramref name="longitude"/> and <paramref name="latitude"/>.
        /// </summary>
        /// <param name="sourceKey">The key of the set.</param>
        /// <param name="destinationKey">The key to store the result at.</param>
        /// <param name="longitude">The longitude of the center point.</param>
        /// <param name="latitude">The latitude of the center point.</param>
        /// <param name="shape">The shape to use to bound the geo search.</param>
        /// <param name="count">The maximum number of results to pull back.</param>
        /// <param name="demandClosest">Whether or not to terminate the search after finding <paramref name="count"/> results. Must be true of count is -1.</param>
        /// <param name="order">The order to sort by (defaults to unordered).</param>
        /// <param name="storeDistances">If set to true, the resulting set will be a regular sorted-set containing only distances, rather than a geo-encoded sorted-set.</param>
        /// <param name="flags">The flags for this operation.</param>
        /// <returns>The size of the set stored at <paramref name="destinationKey"/>.</returns>
        /// <remarks><seealso href="https://redis.io/commands/geosearchstore"/></remarks>
        public async Task<long> GeoSearchAndStoreAsync(string sourceKey, string destinationKey, double longitude, double latitude, GeoSearchShape shape, int count = -1, bool demandClosest = true, Order? order = null, bool storeDistances = false, CommandFlags flags = CommandFlags.None)
        {
            return await _db.GeoSearchAndStoreAsync(sourceKey, destinationKey, longitude, latitude, shape, count, demandClosest, order, storeDistances, flags);
        }

        #endregion

        #region Hash

        /// <summary>
        /// Decrements the number stored at field in the hash stored at key by decrement.
        /// If key does not exist, a new key holding a hash is created.
        /// If field does not exist the value is set to 0 before the operation is performed.
        /// </summary>
        /// <param name="key">The key of the hash.</param>
        /// <param name="hashField">The field in the hash to decrement.</param>
        /// <param name="value">The amount to decrement by.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The value at field after the decrement operation.</returns>
        /// <remarks>
        /// <para>The range of values supported by HINCRBY is limited to 64 bit signed integers.</para>
        /// <para><seealso href="https://redis.io/commands/hincrby"/></para>
        /// </remarks>
        public long HashDecrement(RedisKey key, RedisValue hashField, long value = 1, CommandFlags flags = CommandFlags.None)
        {
            return _db.HashDecrement(key, hashField, value, flags);
        }

        /// <summary>
        /// Decrement the specified field of an hash stored at key, and representing a floating point number, by the specified decrement.
        /// If the field does not exist, it is set to 0 before performing the operation.
        /// </summary>
        /// <param name="key">The key of the hash.</param>
        /// <param name="hashField">The field in the hash to decrement.</param>
        /// <param name="value">The amount to decrement by.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The value at field after the decrement operation.</returns>
        /// <remarks>
        /// <para>The precision of the output is fixed at 17 digits after the decimal point regardless of the actual internal precision of the computation.</para>
        /// <para><seealso href="https://redis.io/commands/hincrbyfloat"/></para>
        /// </remarks>
        public double HashDecrement(RedisKey key, RedisValue hashField, double value, CommandFlags flags = CommandFlags.None)
        {
            return _db.HashDecrement(key, hashField, value, flags);
        }

        /// <summary>
        /// Removes the specified fields from the hash stored at key.
        /// Non-existing fields are ignored. Non-existing keys are treated as empty hashes and this command returns 0.
        /// </summary>
        /// <param name="key">The key of the hash.</param>
        /// <param name="hashField">The field in the hash to delete.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns><see langword="true"/> if the field was removed.</returns>
        /// <remarks><seealso href="https://redis.io/commands/hdel"/></remarks>
        public bool HashDelete(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
        {
            return _db.HashDelete(key, hashField, flags);
        }

        /// <summary>
        /// Removes the specified fields from the hash stored at key.
        /// Non-existing fields are ignored. Non-existing keys are treated as empty hashes and this command returns 0.
        /// </summary>
        /// <param name="key">The key of the hash.</param>
        /// <param name="hashFields">The fields in the hash to delete.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The number of fields that were removed.</returns>
        /// <remarks><seealso href="https://redis.io/commands/hdel"/></remarks>
        public long HashDelete(RedisKey key, RedisValue[] hashFields, CommandFlags flags = CommandFlags.None)
        {
            return _db.HashDelete(key, hashFields, flags);
        }

        /// <summary>
        /// Returns if field is an existing field in the hash stored at key.
        /// </summary>
        /// <param name="key">The key of the hash.</param>
        /// <param name="hashField">The field in the hash to check.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns><see langword="true"/> if the hash contains field, <see langword="false"/> if the hash does not contain field, or key does not exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/hexists"/></remarks>
        public bool HashExists(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
        {
            return _db.HashExists(key, hashField, flags);
        }

        /// <summary>
        /// Returns the value associated with field in the hash stored at key.
        /// </summary>
        /// <param name="key">The key of the hash.</param>
        /// <param name="hashField">The field in the hash to get.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The value associated with field, or nil when field is not present in the hash or key does not exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/hget"/></remarks>
        public RedisValue HashGet(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
        {
            return _db.HashGet(key, hashField, flags);
        }

        /// <summary>
        /// Returns the value associated with field in the hash stored at key.
        /// </summary>
        /// <param name="key">The key of the hash.</param>
        /// <param name="hashField">The field in the hash to get.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The value associated with field, or nil when field is not present in the hash or key does not exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/hget"/></remarks>
        public Lease<byte>? HashGetLease(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
        {
            return _db.HashGetLease(key, hashField, flags);
        }

        /// <summary>
        /// Returns the values associated with the specified fields in the hash stored at key.
        /// For every field that does not exist in the hash, a nil value is returned.Because a non-existing keys are treated as empty hashes, running HMGET against a non-existing key will return a list of nil values.
        /// </summary>
        /// <param name="key">The key of the hash.</param>
        /// <param name="hashFields">The fields in the hash to get.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>List of values associated with the given fields, in the same order as they are requested.</returns>
        /// <remarks><seealso href="https://redis.io/commands/hmget"/></remarks>
        public RedisValue[] HashGet(RedisKey key, RedisValue[] hashFields, CommandFlags flags = CommandFlags.None)
        {
            return _db.HashGet(key, hashFields, flags);
        }

        /// <summary>
        /// Returns all fields and values of the hash stored at key.
        /// </summary>
        /// <param name="key">The key of the hash to get all entries from.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>List of fields and their values stored in the hash, or an empty list when key does not exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/hgetall"/></remarks>
        public HashEntry[] HashGetAll(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _db.HashGetAll(key, flags);
        }

        /// <summary>
        /// Increments the number stored at field in the hash stored at key by increment.
        /// If key does not exist, a new key holding a hash is created.
        /// If field does not exist the value is set to 0 before the operation is performed.
        /// </summary>
        /// <param name="key">The key of the hash.</param>
        /// <param name="hashField">The field in the hash to increment.</param>
        /// <param name="value">The amount to increment by.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The value at field after the increment operation.</returns>
        /// <remarks>
        /// <para>The range of values supported by <c>HINCRBY</c> is limited to 64 bit signed integers.</para>
        /// <para><seealso href="https://redis.io/commands/hincrby"/></para>
        /// </remarks>
        public long HashIncrement(RedisKey key, RedisValue hashField, long value = 1, CommandFlags flags = CommandFlags.None)
        {
            return _db.HashIncrement(key, hashField, value, flags);
        }

        /// <summary>
        /// Increment the specified field of an hash stored at key, and representing a floating point number, by the specified increment.
        /// If the field does not exist, it is set to 0 before performing the operation.
        /// </summary>
        /// <param name="key">The key of the hash.</param>
        /// <param name="hashField">The field in the hash to increment.</param>
        /// <param name="value">The amount to increment by.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The value at field after the increment operation.</returns>
        /// <remarks>
        /// <para>The precision of the output is fixed at 17 digits after the decimal point regardless of the actual internal precision of the computation.</para>
        /// <para><seealso href="https://redis.io/commands/hincrbyfloat"/></para>
        /// </remarks>
        public double HashIncrement(RedisKey key, RedisValue hashField, double value, CommandFlags flags = CommandFlags.None)
        {
            return _db.HashIncrement(key, hashField, value, flags);
        }

        /// <summary>
        /// Returns all field names in the hash stored at key.
        /// </summary>
        /// <param name="key">The key of the hash.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>List of fields in the hash, or an empty list when key does not exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/hkeys"/></remarks>
        public RedisValue[] HashKeys(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _db.HashKeys(key, flags);
        }

        /// <summary>
        /// Returns the number of fields contained in the hash stored at key.
        /// </summary>
        /// <param name="key">The key of the hash.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The number of fields in the hash, or 0 when key does not exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/hlen"/></remarks>
        public long HashLength(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _db.HashLength(key, flags);
        }

        /// <summary>
        /// Gets a random field from the hash at <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key of the hash.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>A random hash field name or <see cref="RedisValue.Null"/> if the hash does not exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/hrandfield"/></remarks>
        public RedisValue HashRandomField(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _db.HashRandomField(key, flags);
        }

        /// <summary>
        /// Gets <paramref name="count"/> field names from the hash at <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key of the hash.</param>
        /// <param name="count">The number of fields to return.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>An array of hash field names of size of at most <paramref name="count"/>, or <see cref="Array.Empty{RedisValue}"/> if the hash does not exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/hrandfield"/></remarks>
        public RedisValue[] HashRandomFields(RedisKey key, long count, CommandFlags flags = CommandFlags.None)
        {
            return _db.HashRandomFields(key, count, flags);
        }

        /// <summary>
        /// Gets <paramref name="count"/> field names and values from the hash at <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key of the hash.</param>
        /// <param name="count">The number of fields to return.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>An array of hash entries of size of at most <paramref name="count"/>, or <see cref="Array.Empty{HashEntry}"/> if the hash does not exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/hrandfield"/></remarks>
        public HashEntry[] HashRandomFieldsWithValues(RedisKey key, long count, CommandFlags flags = CommandFlags.None)
        {
            return _db.HashRandomFieldsWithValues(key, count, flags);
        }

        /// <summary>
        /// Sets the specified fields to their respective values in the hash stored at key.
        /// This command overwrites any specified fields that already exist in the hash, leaving other unspecified fields untouched.
        /// If key does not exist, a new key holding a hash is created.
        /// </summary>
        /// <param name="key">The key of the hash.</param>
        /// <param name="hashFields">The entries to set in the hash.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <remarks><seealso href="https://redis.io/commands/hmset"/></remarks>
        public void HashSet(RedisKey key, HashEntry[] hashFields, CommandFlags flags = CommandFlags.None)
        {
            _db.HashSet(key, hashFields, flags);
        }

        /// <summary>
        /// Sets field in the hash stored at key to value.
        /// If key does not exist, a new key holding a hash is created.
        /// If field already exists in the hash, it is overwritten.
        /// </summary>
        /// <param name="key">The key of the hash.</param>
        /// <param name="hashField">The field to set in the hash.</param>
        /// <param name="value">The value to set.</param>
        /// <param name="when">Which conditions under which to set the field value (defaults to always).</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns><see langword="true"/> if field is a new field in the hash and value was set, <see langword="false"/> if field already exists in the hash and the value was updated.</returns>
        /// <remarks>
        /// <seealso href="https://redis.io/commands/hset"/>,
        /// <seealso href="https://redis.io/commands/hsetnx"/>
        /// </remarks>
        public bool HashSet(RedisKey key, RedisValue hashField, RedisValue value, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            return _db.HashSet(key, hashField, value, when, flags);
        }

        /// <summary>
        /// Returns the string length of the value associated with field in the hash stored at key.
        /// </summary>
        /// <param name="key">The key of the hash.</param>
        /// <param name="hashField">The field containing the string</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The length of the string at field, or 0 when key does not exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/hstrlen"/></remarks>
        public long HashStringLength(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
        {
            return _db.HashStringLength(key, hashField, flags);
        }

        /// <summary>
        /// Returns all values in the hash stored at key.
        /// </summary>
        /// <param name="key">The key of the hash.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>List of values in the hash, or an empty list when key does not exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/hvals"/></remarks>
        public RedisValue[] HashValues(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _db.HashValues(key, flags);
        }

        /// <summary>
        /// 判断该字段是否存在 Hash 中
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="field">字段</param>
        /// <returns></returns>
        public bool HashExists(RedisKey key, RedisValue field)
        {
            return _db.HashExists(key, field);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public ConcurrentDictionary<string, string> HashGet(RedisKey key)
        {
            return _db.HashGetAll(key).ToConcurrentDictionary();
        }

        /// <summary>
        /// 在 Hash 中获取值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="field">字段</param>
        /// <returns></returns>
        public RedisValue HashGet(RedisKey key, RedisValue field)
        {
            return _db.HashGet(key, field);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public ConcurrentDictionary<string, string> HashGetFields(RedisKey key, IEnumerable<string> fields)
        {
            return _db.HashGet(key, fields.ToRedisValues()).ToConcurrentDictionary(fields);
        }

        /// <summary>
        /// 在 Hash 设定值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="field">字段</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public bool HashSet(RedisKey key, RedisValue field, RedisValue value)
        {
            return _db.HashSet(key, field, value);
        }

        /// <summary>
        /// 在 Hash 设定值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="entries">字段容器</param>
        /// <returns></returns>
        public void HashSet(RedisKey key, ConcurrentDictionary<string, string> entries)
        {
            var val = entries.ToHashEntries();
            if (val != null)
                _db.HashSet(key, val);
        }

        /// <summary>
        /// 在 Hash 中设置多个值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="fields"></param>
        public void HashSetFields(RedisKey key, ConcurrentDictionary<string, string> fields)
        {
            if (fields == null || !fields.Any())
                return;

            var hs = HashGet(key);
            foreach (var field in fields)
            {
                //if(!hs.ContainsKey(field.Key))
                //    continue;

                hs[field.Key] = field.Value;
            }

            HashSet(key, hs);
        }

        /// <summary>
        /// 从 Hash 中移除指定字段
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public bool HashDelete(RedisKey key, RedisValue field)
        {
            return _db.HashDelete(key, field);
        }

        /// <summary>
        /// 从 Hash 中删除指定 key 对应的多个指定的字段
        /// </summary>
        /// <param name="key"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public bool HashDeleteFields(RedisKey key, RedisValue[] fields)
        {
            if (fields == null || !fields.Any())
                return false;

            var success = true;
            foreach (var field in fields)
            {
                if (!_db.HashDelete(key, field))
                    success = false;
            }

            return success;
        }

        /// <summary>
        /// 根据键获取 Hash 中的所有值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public RedisValue[] HashValues(RedisKey key)
        {
            return _db.HashValues(key);
        }

        #endregion

        #region Hash Async

        /// <summary>
        /// Decrements the number stored at field in the hash stored at key by decrement.
        /// If key does not exist, a new key holding a hash is created.
        /// If field does not exist the value is set to 0 before the operation is performed.
        /// </summary>
        /// <param name="key">The key of the hash.</param>
        /// <param name="hashField">The field in the hash to decrement.</param>
        /// <param name="value">The amount to decrement by.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The value at field after the decrement operation.</returns>
        /// <remarks>
        /// <para>The range of values supported by HINCRBY is limited to 64 bit signed integers.</para>
        /// <para><seealso href="https://redis.io/commands/hincrby"/></para>
        /// </remarks>
        public async Task<long> HashDecrementAsync(RedisKey key, RedisValue hashField, long value = 1, CommandFlags flags = CommandFlags.None)
        {
            return await _db.HashDecrementAsync(key, hashField, value, flags);
        }

        /// <summary>
        /// Decrement the specified field of an hash stored at key, and representing a floating point number, by the specified decrement.
        /// If the field does not exist, it is set to 0 before performing the operation.
        /// </summary>
        /// <param name="key">The key of the hash.</param>
        /// <param name="hashField">The field in the hash to decrement.</param>
        /// <param name="value">The amount to decrement by.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The value at field after the decrement operation.</returns>
        /// <remarks>
        /// <para>The precision of the output is fixed at 17 digits after the decimal point regardless of the actual internal precision of the computation.</para>
        /// <para><seealso href="https://redis.io/commands/hincrbyfloat"/></para>
        /// </remarks>
        public async Task<double> HashDecrementAsync(RedisKey key, RedisValue hashField, double value, CommandFlags flags = CommandFlags.None)
        {
            return await _db.HashDecrementAsync(key, hashField, value, flags);
        }

        /// <summary>
        /// Removes the specified fields from the hash stored at key.
        /// Non-existing fields are ignored. Non-existing keys are treated as empty hashes and this command returns 0.
        /// </summary>
        /// <param name="key">The key of the hash.</param>
        /// <param name="hashField">The field in the hash to delete.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns><see langword="true"/> if the field was removed.</returns>
        /// <remarks><seealso href="https://redis.io/commands/hdel"/></remarks>
        public async Task<bool> HashDeleteAsync(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
        {
            return await _db.HashDeleteAsync(key, hashField, flags);
        }

        /// <summary>
        /// Removes the specified fields from the hash stored at key.
        /// Non-existing fields are ignored. Non-existing keys are treated as empty hashes and this command returns 0.
        /// </summary>
        /// <param name="key">The key of the hash.</param>
        /// <param name="hashFields">The fields in the hash to delete.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The number of fields that were removed.</returns>
        /// <remarks><seealso href="https://redis.io/commands/hdel"/></remarks>
        public async Task<long> HashDeleteAsync(RedisKey key, RedisValue[] hashFields, CommandFlags flags = CommandFlags.None)
        {
            return await _db.HashDeleteAsync(key, hashFields, flags);
        }

        /// <summary>
        /// Returns if field is an existing field in the hash stored at key.
        /// </summary>
        /// <param name="key">The key of the hash.</param>
        /// <param name="hashField">The field in the hash to check.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns><see langword="true"/> if the hash contains field, <see langword="false"/> if the hash does not contain field, or key does not exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/hexists"/></remarks>
        public async Task<bool> HashExistsAsync(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
        {
            return await _db.HashExistsAsync(key, hashField, flags);
        }

        /// <summary>
        /// Returns the value associated with field in the hash stored at key.
        /// </summary>
        /// <param name="key">The key of the hash.</param>
        /// <param name="hashField">The field in the hash to get.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The value associated with field, or nil when field is not present in the hash or key does not exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/hget"/></remarks>
        public async Task<RedisValue> HashGetAsync(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
        {
            return await _db.HashGetAsync(key, hashField, flags);
        }

        /// <summary>
        /// Returns the value associated with field in the hash stored at key.
        /// </summary>
        /// <param name="key">The key of the hash.</param>
        /// <param name="hashField">The field in the hash to get.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The value associated with field, or nil when field is not present in the hash or key does not exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/hget"/></remarks>
        public async Task<Lease<byte>?> HashGetLeaseAsync(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
        {
            return await _db.HashGetLeaseAsync(key, hashField, flags);
        }

        /// <summary>
        /// Returns the values associated with the specified fields in the hash stored at key.
        /// For every field that does not exist in the hash, a nil value is returned.Because a non-existing keys are treated as empty hashes, running HMGET against a non-existing key will return a list of nil values.
        /// </summary>
        /// <param name="key">The key of the hash.</param>
        /// <param name="hashFields">The fields in the hash to get.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>List of values associated with the given fields, in the same order as they are requested.</returns>
        /// <remarks><seealso href="https://redis.io/commands/hmget"/></remarks>
        public async Task<RedisValue[]> HashGetAsync(RedisKey key, RedisValue[] hashFields, CommandFlags flags = CommandFlags.None)
        {
            return await _db.HashGetAsync(key, hashFields, flags);
        }

        /// <summary>
        /// Returns all fields and values of the hash stored at key.
        /// </summary>
        /// <param name="key">The key of the hash to get all entries from.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>List of fields and their values stored in the hash, or an empty list when key does not exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/hgetall"/></remarks>
        public async Task<HashEntry[]> HashGetAllAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return await _db.HashGetAllAsync(key, flags);
        }

        /// <summary>
        /// Increments the number stored at field in the hash stored at key by increment.
        /// If key does not exist, a new key holding a hash is created.
        /// If field does not exist the value is set to 0 before the operation is performed.
        /// </summary>
        /// <param name="key">The key of the hash.</param>
        /// <param name="hashField">The field in the hash to increment.</param>
        /// <param name="value">The amount to increment by.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The value at field after the increment operation.</returns>
        /// <remarks>
        /// <para>The range of values supported by <c>HINCRBY</c> is limited to 64 bit signed integers.</para>
        /// <para><seealso href="https://redis.io/commands/hincrby"/></para>
        /// </remarks>
        public async Task<long> HashIncrementAsync(RedisKey key, RedisValue hashField, long value = 1, CommandFlags flags = CommandFlags.None)
        {
            return await _db.HashIncrementAsync(key, hashField, value, flags);
        }

        /// <summary>
        /// Increment the specified field of an hash stored at key, and representing a floating point number, by the specified increment.
        /// If the field does not exist, it is set to 0 before performing the operation.
        /// </summary>
        /// <param name="key">The key of the hash.</param>
        /// <param name="hashField">The field in the hash to increment.</param>
        /// <param name="value">The amount to increment by.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The value at field after the increment operation.</returns>
        /// <remarks>
        /// <para>The precision of the output is fixed at 17 digits after the decimal point regardless of the actual internal precision of the computation.</para>
        /// <para><seealso href="https://redis.io/commands/hincrbyfloat"/></para>
        /// </remarks>
        public async Task<double> HashIncrementAsync(RedisKey key, RedisValue hashField, double value, CommandFlags flags = CommandFlags.None)
        {
            return await _db.HashIncrementAsync(key, hashField, value, flags);
        }

        /// <summary>
        /// Returns all field names in the hash stored at key.
        /// </summary>
        /// <param name="key">The key of the hash.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>List of fields in the hash, or an empty list when key does not exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/hkeys"/></remarks>
        public async Task<RedisValue[]> HashKeysAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return await _db.HashKeysAsync(key, flags);
        }

        /// <summary>
        /// Returns the number of fields contained in the hash stored at key.
        /// </summary>
        /// <param name="key">The key of the hash.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The number of fields in the hash, or 0 when key does not exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/hlen"/></remarks>
        public async Task<long> HashLengthAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return await _db.HashLengthAsync(key, flags);
        }

        /// <summary>
        /// Gets a random field from the hash at <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key of the hash.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>A random hash field name or <see cref="RedisValue.Null"/> if the hash does not exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/hrandfield"/></remarks>
        public async Task<RedisValue> HashRandomFieldAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return await _db.HashRandomFieldAsync(key, flags);
        }

        /// <summary>
        /// Gets <paramref name="count"/> field names from the hash at <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key of the hash.</param>
        /// <param name="count">The number of fields to return.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>An array of hash field names of size of at most <paramref name="count"/>, or <see cref="Array.Empty{RedisValue}"/> if the hash does not exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/hrandfield"/></remarks>
        public async Task<RedisValue[]> HashRandomFieldsAsync(RedisKey key, long count, CommandFlags flags = CommandFlags.None)
        {
            return await _db.HashRandomFieldsAsync(key, count, flags);
        }

        /// <summary>
        /// Gets <paramref name="count"/> field names and values from the hash at <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key of the hash.</param>
        /// <param name="count">The number of fields to return.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>An array of hash entries of size of at most <paramref name="count"/>, or <see cref="Array.Empty{HashEntry}"/> if the hash does not exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/hrandfield"/></remarks>
        public async Task<HashEntry[]> HashRandomFieldsWithValuesAsync(RedisKey key, long count, CommandFlags flags = CommandFlags.None)
        {
            return await _db.HashRandomFieldsWithValuesAsync(key, count, flags);
        }

        /// <summary>
        /// Sets the specified fields to their respective values in the hash stored at key.
        /// This command overwrites any specified fields that already exist in the hash, leaving other unspecified fields untouched.
        /// If key does not exist, a new key holding a hash is created.
        /// </summary>
        /// <param name="key">The key of the hash.</param>
        /// <param name="hashFields">The entries to set in the hash.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <remarks><seealso href="https://redis.io/commands/hmset"/></remarks>
        public async Task HashSetAsync(RedisKey key, HashEntry[] hashFields, CommandFlags flags = CommandFlags.None)
        {
            await _db.HashSetAsync(key, hashFields, flags);
        }

        /// <summary>
        /// Sets field in the hash stored at key to value.
        /// If key does not exist, a new key holding a hash is created.
        /// If field already exists in the hash, it is overwritten.
        /// </summary>
        /// <param name="key">The key of the hash.</param>
        /// <param name="hashField">The field to set in the hash.</param>
        /// <param name="value">The value to set.</param>
        /// <param name="when">Which conditions under which to set the field value (defaults to always).</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns><see langword="true"/> if field is a new field in the hash and value was set, <see langword="false"/> if field already exists in the hash and the value was updated.</returns>
        /// <remarks>
        /// <seealso href="https://redis.io/commands/hset"/>,
        /// <seealso href="https://redis.io/commands/hsetnx"/>
        /// </remarks>
        public async Task<bool> HashSetAsync(RedisKey key, RedisValue hashField, RedisValue value, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            return await _db.HashSetAsync(key, hashField, value, when, flags);
        }

        /// <summary>
        /// Returns the string length of the value associated with field in the hash stored at key.
        /// </summary>
        /// <param name="key">The key of the hash.</param>
        /// <param name="hashField">The field containing the string</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The length of the string at field, or 0 when key does not exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/hstrlen"/></remarks>
        public async Task<long> HashStringLengthAsync(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
        {
            return await _db.HashStringLengthAsync(key, hashField, flags);
        }

        /// <summary>
        /// Returns all values in the hash stored at key.
        /// </summary>
        /// <param name="key">The key of the hash.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>List of values in the hash, or an empty list when key does not exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/hvals"/></remarks>
        public async Task<RedisValue[]> HashValuesAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return await _db.HashValuesAsync(key, flags);
        }

        /// <summary>
        /// 判断该字段是否存在 Hash 中
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="field">字段</param>
        /// <returns></returns>
        public async Task<bool> HashExistsAsync(RedisKey key, string field)
        {
            return await _db.HashExistsAsync(key, field);
        }

        /// <summary>
        /// 从 Hash 中获取值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<ConcurrentDictionary<string, string>> HashGetAsync(RedisKey key) =>
            (await _db.HashGetAllAsync(key)).ToConcurrentDictionary();

        /// <summary>
        /// 在 Hash 中获取值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="field">字段</param>
        /// <returns></returns>
        public async Task<string> HashGetAsync(RedisKey key, string field)
        {
            return (await _db.HashGetAsync(key, field)).ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public async Task<ConcurrentDictionary<string, string>> HashGetFieldsAsync(RedisKey key, IEnumerable<string> fields) =>
            (await _db.HashGetAsync(key, fields.ToRedisValues())).ToConcurrentDictionary(fields);

        /// <summary>
        /// 在 Hash 设定值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="field">字段</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public async Task<bool> HashSetAsync(RedisKey key, string field, string value)
        {
            return await _db.HashSetAsync(key, field, value);
        }

        /// <summary>
        /// 在 Hash 设定值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="entries">字段容器</param>
        /// <returns></returns>
        public async Task HashSetAsync(RedisKey key, ConcurrentDictionary<string, string> entries)
        {
            var val = entries.ToHashEntries();
            if (val != null)
                await _db.HashSetAsync(key, val);
        }

        /// <summary>
        /// 在 Hash中设置多个字段
        /// </summary>
        /// <param name="key"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public async Task HashSetFieldsAsync(RedisKey key, ConcurrentDictionary<string, string> fields)
        {
            if (fields == null || !fields.Any())
                return;

            var hs = await HashGetAsync(key);
            foreach (var field in fields)
            {
                //if(!hs.ContainsKey(field.Key))
                //    continue;

                hs[field.Key] = field.Value;
            }

            await HashSetAsync(key, hs);
        }

        /// <summary>
        /// 删除 Hash 值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<bool> HashDeleteAsync(RedisKey key) => await KeyDeleteAsync(new string[] { key }) > 0;

        /// <summary>
        /// 从 Hash 中移除指定字段
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public async Task<bool> HashDeleteAsync(RedisKey key, string field)
        {
            return await _db.HashDeleteAsync(key, field);
        }

        /// <summary>
        /// 从 Hash 中删除指定 key 对应的多个指定的字段
        /// </summary>
        /// <param name="key"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public async Task<bool> HashDeleteFieldsAsync(RedisKey key, IEnumerable<string> fields)
        {
            if (fields == null || !fields.Any())
                return false;

            var success = true;
            foreach (var field in fields)
            {
                if (!await _db.HashDeleteAsync(key, field))
                    success = false;
            }

            return success;
        }

        #endregion

        #region HyperLogLog

        /// <summary>
        /// Adds the element to the HyperLogLog data structure stored at the variable name specified as first argument.
        /// </summary>
        /// <param name="key">The key of the hyperloglog.</param>
        /// <param name="value">The value to add.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns><see langword="true"/> if at least 1 HyperLogLog internal register was altered, <see langword="false"/> otherwise.</returns>
        /// <remarks><seealso href="https://redis.io/commands/pfadd"/></remarks>
        public bool HyperLogLogAdd(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            return _db.HyperLogLogAdd(key, value, flags);
        }

        /// <summary>
        /// Adds all the element arguments to the HyperLogLog data structure stored at the variable name specified as first argument.
        /// </summary>
        /// <param name="key">The key of the hyperloglog.</param>
        /// <param name="values">The values to add.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns><see langword="true"/> if at least 1 HyperLogLog internal register was altered, <see langword="false"/> otherwise.</returns>
        /// <remarks><seealso href="https://redis.io/commands/pfadd"/></remarks>
        public bool HyperLogLogAdd(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
            return _db.HyperLogLogAdd(key, values, flags);
        }

        /// <summary>
        /// Returns the approximated cardinality computed by the HyperLogLog data structure stored at the specified variable, or 0 if the variable does not exist.
        /// </summary>
        /// <param name="key">The key of the hyperloglog.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The approximated number of unique elements observed via HyperLogLogAdd.</returns>
        /// <remarks><seealso href="https://redis.io/commands/pfcount"/></remarks>
        public long HyperLogLogLength(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _db.HyperLogLogLength(key, flags);
        }

        /// <summary>
        /// Returns the approximated cardinality of the union of the HyperLogLogs passed, by internally merging the HyperLogLogs stored at the provided keys into a temporary hyperLogLog, or 0 if the variable does not exist.
        /// </summary>
        /// <param name="keys">The keys of the hyperloglogs.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The approximated number of unique elements observed via HyperLogLogAdd.</returns>
        /// <remarks><seealso href="https://redis.io/commands/pfcount"/></remarks>
        public long HyperLogLogLength(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            return _db.HyperLogLogLength(keys, flags);
        }

        /// <summary>
        /// Merge multiple HyperLogLog values into an unique value that will approximate the cardinality of the union of the observed Sets of the source HyperLogLog structures.
        /// </summary>
        /// <param name="destination">The key of the merged hyperloglog.</param>
        /// <param name="first">The key of the first hyperloglog to merge.</param>
        /// <param name="second">The key of the first hyperloglog to merge.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <remarks><seealso href="https://redis.io/commands/pfmerge"/></remarks>
        public void HyperLogLogMerge(RedisKey destination, RedisKey first, RedisKey second, CommandFlags flags = CommandFlags.None)
        {
            _db.HyperLogLogMerge(destination, first, second, flags);
        }

        /// <summary>
        /// Merge multiple HyperLogLog values into an unique value that will approximate the cardinality of the union of the observed Sets of the source HyperLogLog structures.
        /// </summary>
        /// <param name="destination">The key of the merged hyperloglog.</param>
        /// <param name="sourceKeys">The keys of the hyperloglogs to merge.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <remarks><seealso href="https://redis.io/commands/pfmerge"/></remarks>
        public void HyperLogLogMerge(RedisKey destination, RedisKey[] sourceKeys, CommandFlags flags = CommandFlags.None)
        {
            _db.HyperLogLogMerge(destination, sourceKeys, flags);
        }

        #endregion

        #region HyperLogLog Async

        /// <summary>
        /// Adds the element to the HyperLogLog data structure stored at the variable name specified as first argument.
        /// </summary>
        /// <param name="key">The key of the hyperloglog.</param>
        /// <param name="value">The value to add.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns><see langword="true"/> if at least 1 HyperLogLog internal register was altered, <see langword="false"/> otherwise.</returns>
        /// <remarks><seealso href="https://redis.io/commands/pfadd"/></remarks>
        public async Task<bool> HyperLogLogAddAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            return await _db.HyperLogLogAddAsync(key, value, flags);
        }

        /// <summary>
        /// Adds all the element arguments to the HyperLogLog data structure stored at the variable name specified as first argument.
        /// </summary>
        /// <param name="key">The key of the hyperloglog.</param>
        /// <param name="values">The values to add.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns><see langword="true"/> if at least 1 HyperLogLog internal register was altered, <see langword="false"/> otherwise.</returns>
        /// <remarks><seealso href="https://redis.io/commands/pfadd"/></remarks>
        public async Task<bool> HyperLogLogAddAsync(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
            return await _db.HyperLogLogAddAsync(key, values, flags);
        }

        /// <summary>
        /// Returns the approximated cardinality computed by the HyperLogLog data structure stored at the specified variable, or 0 if the variable does not exist.
        /// </summary>
        /// <param name="key">The key of the hyperloglog.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The approximated number of unique elements observed via HyperLogLogAdd.</returns>
        /// <remarks><seealso href="https://redis.io/commands/pfcount"/></remarks>
        public async Task<long> HyperLogLogLengthAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return await _db.HyperLogLogLengthAsync(key, flags);
        }

        /// <summary>
        /// Returns the approximated cardinality of the union of the HyperLogLogs passed, by internally merging the HyperLogLogs stored at the provided keys into a temporary hyperLogLog, or 0 if the variable does not exist.
        /// </summary>
        /// <param name="keys">The keys of the hyperloglogs.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The approximated number of unique elements observed via HyperLogLogAdd.</returns>
        /// <remarks><seealso href="https://redis.io/commands/pfcount"/></remarks>
        public async Task<long> HyperLogLogLengthAsync(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            return await _db.HyperLogLogLengthAsync(keys, flags);
        }

        /// <summary>
        /// Merge multiple HyperLogLog values into an unique value that will approximate the cardinality of the union of the observed Sets of the source HyperLogLog structures.
        /// </summary>
        /// <param name="destination">The key of the merged hyperloglog.</param>
        /// <param name="first">The key of the first hyperloglog to merge.</param>
        /// <param name="second">The key of the first hyperloglog to merge.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <remarks><seealso href="https://redis.io/commands/pfmerge"/></remarks>
        public async Task HyperLogLogMergeAsync(RedisKey destination, RedisKey first, RedisKey second, CommandFlags flags = CommandFlags.None)
        {
            await _db.HyperLogLogMergeAsync(destination, first, second, flags);
        }

        /// <summary>
        /// Merge multiple HyperLogLog values into an unique value that will approximate the cardinality of the union of the observed Sets of the source HyperLogLog structures.
        /// </summary>
        /// <param name="destination">The key of the merged hyperloglog.</param>
        /// <param name="sourceKeys">The keys of the hyperloglogs to merge.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <remarks><seealso href="https://redis.io/commands/pfmerge"/></remarks>
        public async Task HyperLogLogMergeAsync(RedisKey destination, RedisKey[] sourceKeys, CommandFlags flags = CommandFlags.None)
        {
            await _db.HyperLogLogMergeAsync(destination, sourceKeys, flags);
        }

        #endregion

        #region Key

        /// <summary>
        /// Copies the value from the <paramref name="sourceKey"/> to the specified <paramref name="destinationKey"/>.
        /// </summary>
        /// <param name="sourceKey">The key of the source value to copy.</param>
        /// <param name="destinationKey">The destination key to copy the source to.</param>
        /// <param name="destinationDatabase">The database ID to store <paramref name="destinationKey"/> in. If default (-1), current database is used.</param>
        /// <param name="replace">Whether to overwrite an existing values at <paramref name="destinationKey"/>. If <see langword="false"/> and the key exists, the copy will not succeed.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns><see langword="true"/> if key was copied. <see langword="false"/> if key was not copied.</returns>
        /// <remarks><seealso href="https://redis.io/commands/copy"/></remarks>
        public bool KeyCopy(RedisKey sourceKey, RedisKey destinationKey, int destinationDatabase = -1, bool replace = false, CommandFlags flags = CommandFlags.None)
        {
            return _db.KeyCopy(sourceKey, destinationKey, destinationDatabase, replace, flags);
        }

        /// <summary>
        /// Removes the specified key. A key is ignored if it does not exist.
        /// If UNLINK is available (Redis 4.0+), it will be used.
        /// </summary>
        /// <param name="key">The key to delete.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns><see langword="true"/> if the key was removed.</returns>
        /// <remarks>
        /// <seealso href="https://redis.io/commands/del"/>,
        /// <seealso href="https://redis.io/commands/unlink"/>
        /// </remarks>
        public bool KeyDelete(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _db.KeyDelete(key, flags);
        }

        /// <summary>
        /// Removes the specified keys. A key is ignored if it does not exist.
        /// If UNLINK is available (Redis 4.0+), it will be used.
        /// </summary>
        /// <param name="keys">The keys to delete.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The number of keys that were removed.</returns>
        /// <remarks>
        /// <seealso href="https://redis.io/commands/del"/>,
        /// <seealso href="https://redis.io/commands/unlink"/>
        /// </remarks>
        public long KeyDelete(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            return _db.KeyDelete(keys, flags);
        }

        /// <summary>
        /// Serialize the value stored at key in a Redis-specific format and return it to the user.
        /// The returned value can be synthesized back into a Redis key using the RESTORE command.
        /// </summary>
        /// <param name="key">The key to dump.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The serialized value.</returns>
        /// <remarks><seealso href="https://redis.io/commands/dump"/></remarks>
        public byte[]? KeyDump(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _db.KeyDump(key, flags);
        }

        /// <summary>
        /// Returns the internal encoding for the Redis object stored at <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key to dump.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The Redis encoding for the value or <see langword="null"/> is the key does not exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/object-encoding"/></remarks>
        public string? KeyEncoding(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _db.KeyEncoding(key, flags);
        }

        /// <summary>
        /// Returns if key exists.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns><see langword="true"/> if the key exists. <see langword="false"/> if the key does not exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/exists"/></remarks>
        public bool KeyExists(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _db.KeyExists(key, flags);
        }

        /// <summary>
        /// Indicates how many of the supplied keys exists.
        /// </summary>
        /// <param name="keys">The keys to check.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The number of keys that existed.</returns>
        /// <remarks><seealso href="https://redis.io/commands/exists"/></remarks>
        public long KeyExists(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            return _db.KeyExists(keys, flags);
        }

        /// <summary>
        /// Set a timeout on <paramref name="key"/>.
        /// After the timeout has expired, the key will automatically be deleted.
        /// A key with an associated timeout is said to be volatile in Redis terminology.
        /// </summary>
        /// <param name="key">The key to set the expiration for.</param>
        /// <param name="expiry">The timeout to set.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns><see langword="true"/> if the timeout was set. <see langword="false"/> if key does not exist or the timeout could not be set.</returns>
        /// <remarks>
        /// If key is updated before the timeout has expired, then the timeout is removed as if the PERSIST command was invoked on key.
        /// <para>
        /// For Redis versions &lt; 2.1.3, existing timeouts cannot be overwritten.
        /// So, if key already has an associated timeout, it will do nothing and return 0.
        /// </para>
        /// <para>
        /// Since Redis 2.1.3, you can update the timeout of a key.
        /// It is also possible to remove the timeout using the PERSIST command.
        /// See the page on key expiry for more information.
        /// </para>
        /// <para>
        /// <seealso href="https://redis.io/commands/expire"/>,
        /// <seealso href="https://redis.io/commands/pexpire"/>,
        /// <seealso href="https://redis.io/commands/persist"/>
        /// </para>
        /// </remarks>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool KeyExpire(RedisKey key, TimeSpan? expiry, CommandFlags flags = CommandFlags.None)
        {
            return _db.KeyExpire(key, expiry, flags);
        }

        /// <summary>
        /// Set a timeout on <paramref name="key"/>.
        /// After the timeout has expired, the key will automatically be deleted.
        /// A key with an associated timeout is said to be volatile in Redis terminology.
        /// </summary>
        /// <param name="key">The key to set the expiration for.</param>
        /// <param name="expiry">The timeout to set.</param>
        /// <param name="when">In Redis 7+, we choose under which condition the expiration will be set using <see cref="ExpireWhen"/>.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns><see langword="true"/> if the timeout was set. <see langword="false"/> if key does not exist or the timeout could not be set.</returns>
        /// <remarks>
        /// <seealso href="https://redis.io/commands/expire"/>,
        /// <seealso href="https://redis.io/commands/pexpire"/>
        /// </remarks>
        public bool KeyExpire(RedisKey key, TimeSpan? expiry, ExpireWhen when = ExpireWhen.Always, CommandFlags flags = CommandFlags.None)
        {
            return _db.KeyExpire(key, expiry, when, flags);
        }

        /// <summary>
        /// Set a timeout on <paramref name="key"/>.
        /// After the timeout has expired, the key will automatically be deleted.
        /// A key with an associated timeout is said to be volatile in Redis terminology.
        /// </summary>
        /// <param name="key">The key to set the expiration for.</param>
        /// <param name="expiry">The exact date to expiry to set.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns><see langword="true"/> if the timeout was set. <see langword="false"/> if key does not exist or the timeout could not be set.</returns>
        /// <remarks>
        /// If key is updated before the timeout has expired, then the timeout is removed as if the PERSIST command was invoked on key.
        /// <para>
        /// For Redis versions &lt; 2.1.3, existing timeouts cannot be overwritten.
        /// So, if key already has an associated timeout, it will do nothing and return 0.
        /// </para>
        /// <para>
        /// Since Redis 2.1.3, you can update the timeout of a key.
        /// It is also possible to remove the timeout using the PERSIST command.
        /// See the page on key expiry for more information.
        /// </para>
        /// <para>
        /// <seealso href="https://redis.io/commands/expireat"/>,
        /// <seealso href="https://redis.io/commands/pexpireat"/>,
        /// <seealso href="https://redis.io/commands/persist"/>
        /// </para>
        /// </remarks>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool KeyExpire(RedisKey key, DateTime? expiry, CommandFlags flags)
        {
            return _db.KeyExpire(key, expiry, flags);
        }

        /// <summary>
        /// Set a timeout on <paramref name="key"/>.
        /// After the timeout has expired, the key will automatically be deleted.
        /// A key with an associated timeout is said to be volatile in Redis terminology.
        /// </summary>
        /// <param name="key">The key to set the expiration for.</param>
        /// <param name="expiry">The timeout to set.</param>
        /// <param name="when">In Redis 7+, we choose under which condition the expiration will be set using <see cref="ExpireWhen"/>.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns><see langword="true"/> if the timeout was set. <see langword="false"/> if key does not exist or the timeout could not be set.</returns>
        /// <remarks>
        /// <seealso href="https://redis.io/commands/expire"/>,
        /// <seealso href="https://redis.io/commands/pexpire"/>
        /// </remarks>
        public bool KeyExpire(RedisKey key, DateTime? expiry, ExpireWhen when = ExpireWhen.Always, CommandFlags flags = CommandFlags.None)
        {
            return _db.KeyExpire(key, expiry, when, flags);
        }

        /// <summary>
        /// Returns the absolute time at which the given <paramref name="key"/> will expire, if it exists and has an expiration.
        /// </summary>
        /// <param name="key">The key to get the expiration for.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The time at which the given key will expire, or <see langword="null"/> if the key does not exist or has no associated expiration time.</returns>
        /// <remarks>
        /// <seealso href="https://redis.io/commands/expiretime"/>,
        /// <seealso href="https://redis.io/commands/pexpiretime"/>
        /// </remarks>
        public DateTime? KeyExpireTime(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _db.KeyExpireTime(key, flags);
        }

        /// <summary>
        /// Returns the logarithmic access frequency counter of the object stored at <paramref name="key"/>.
        /// The command is only available when the <c>maxmemory-policy</c> configuration directive is set to
        /// one of <see href="https://redis.io/docs/manual/eviction/#the-new-lfu-mode">the LFU policies</see>.
        /// </summary>
        /// <param name="key">The key to get a frequency count for.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The number of logarithmic access frequency counter, (<see langword="null"/> if the key does not exist).</returns>
        /// <remarks><seealso href="https://redis.io/commands/object-freq"/></remarks>
        public long? KeyFrequency(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _db.KeyFrequency(key, flags);
        }

        /// <summary>
        /// Returns the time since the object stored at the specified key is idle (not requested by read or write operations).
        /// </summary>
        /// <param name="key">The key to get the time of.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The time since the object stored at the specified key is idle.</returns>
        /// <remarks><seealso href="https://redis.io/commands/object"/></remarks>
        public TimeSpan? KeyIdleTime(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _db.KeyIdleTime(key, flags);
        }

        /// <summary>
        /// Move key from the currently selected database (see SELECT) to the specified destination database.
        /// When key already exists in the destination database, or it does not exist in the source database, it does nothing.
        /// It is possible to use MOVE as a locking primitive because of this.
        /// </summary>
        /// <param name="key">The key to move.</param>
        /// <param name="database">The database to move the key to.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns><see langword="true"/> if key was moved. <see langword="false"/> if key was not moved.</returns>
        /// <remarks><seealso href="https://redis.io/commands/move"/></remarks>
        public bool KeyMove(RedisKey key, int database, CommandFlags flags = CommandFlags.None)
        {
            return _db.KeyMove(key, database, flags);
        }

        /// <summary>
        /// Remove the existing timeout on key, turning the key from volatile (a key with an expire set) to persistent (a key that will never expire as no timeout is associated).
        /// </summary>
        /// <param name="key">The key to persist.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns><see langword="true"/> if the timeout was removed. <see langword="false"/> if key does not exist or does not have an associated timeout.</returns>
        /// <remarks><seealso href="https://redis.io/commands/persist"/></remarks>
        public bool KeyPersist(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _db.KeyPersist(key, flags);
        }

        /// <summary>
        /// Return a random key from the currently selected database.
        /// </summary>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The random key, or nil when the database is empty.</returns>
        /// <remarks><seealso href="https://redis.io/commands/randomkey"/></remarks>
        public RedisKey KeyRandom(CommandFlags flags = CommandFlags.None)
        {
            return _db.KeyRandom(flags);
        }

        /// <summary>
        /// Returns the reference count of the object stored at <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key to get a reference count for.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The number of references (<see langword="Null"/> if the key does not exist).</returns>
        /// <remarks><seealso href="https://redis.io/commands/object-refcount"/></remarks>
        public long? KeyRefCount(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _db.KeyRefCount(key, flags);
        }

        /// <summary>
        /// Renames <paramref name="key"/> to <paramref name="newKey"/>.
        /// It returns an error when the source and destination names are the same, or when key does not exist.
        /// </summary>
        /// <param name="key">The key to rename.</param>
        /// <param name="newKey">The key to rename to.</param>
        /// <param name="when">What conditions to rename under (defaults to always).</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns><see langword="true"/> if the key was renamed, <see langword="false"/> otherwise.</returns>
        /// <remarks>
        /// <seealso href="https://redis.io/commands/rename"/>,
        /// <seealso href="https://redis.io/commands/renamenx"/>
        /// </remarks>
        public bool KeyRename(RedisKey key, RedisKey newKey, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            return _db.KeyRename(key, newKey, when, flags);
        }

        /// <summary>
        /// Create a key associated with a value that is obtained by deserializing the provided serialized value (obtained via DUMP).
        /// If <paramref name="expiry"/> is 0 the key is created without any expire, otherwise the specified expire time (in milliseconds) is set.
        /// </summary>
        /// <param name="key">The key to restore.</param>
        /// <param name="value">The value of the key.</param>
        /// <param name="expiry">The expiry to set.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <remarks><seealso href="https://redis.io/commands/restore"/></remarks>
        public void KeyRestore(RedisKey key, byte[] value, TimeSpan? expiry = null, CommandFlags flags = CommandFlags.None)
        {
            _db.KeyRestore(key, value, expiry, flags);
        }

        /// <summary>
        /// Returns the remaining time to live of a key that has a timeout.
        /// This introspection capability allows a Redis client to check how many seconds a given key will continue to be part of the dataset.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>TTL, or nil when key does not exist or does not have a timeout.</returns>
        /// <remarks><seealso href="https://redis.io/commands/ttl"/></remarks>
        public TimeSpan? KeyTimeToLive(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _db.KeyTimeToLive(key, flags);
        }

        /// <summary>
        /// Alters the last access time of a key.
        /// </summary>
        /// <param name="key">The key to touch.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns><see langword="true"/> if the key was touched, <see langword="false"/> otherwise.</returns>
        /// <remarks><seealso href="https://redis.io/commands/touch"/></remarks>
        public bool KeyTouch(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _db.KeyTouch(key, flags);
        }

        /// <summary>
        /// Alters the last access time of the specified <paramref name="keys"/>. A key is ignored if it does not exist.
        /// </summary>
        /// <param name="keys">The keys to touch.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The number of keys that were touched.</returns>
        /// <remarks><seealso href="https://redis.io/commands/touch"/></remarks>
        public long KeyTouch(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            return _db.KeyTouch(keys, flags);
        }

        /// <summary>
        /// Returns the string representation of the type of the value stored at key.
        /// The different types that can be returned are: string, list, set, zset and hash.
        /// </summary>
        /// <param name="key">The key to get the type of.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>Type of key, or none when key does not exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/type"/></remarks>
        public RedisType KeyType(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _db.KeyType(key, flags);
        }

        /// <summary>
        /// 返回所有 key
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetAllKeys() =>
            _conn.GetEndPoints().Select(endPoint => _conn.GetServer(endPoint))
                .SelectMany(server => server.Keys().ToStrings());

        /// <summary>
        /// 返回所有 key
        /// </summary>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        public IEnumerable<string> GetAllKeys(EndPoint endPoint) => _conn.GetServer(endPoint).Keys().ToStrings();

        /// <summary>
        /// 判断给定的 key 是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool KeyExists(RedisKey key)
        {
            return _db.KeyExists(key);
        }

        /// <summary>
        /// 返回所提供的 keys 存在的数量
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public long KeyExists(IEnumerable<string> keys)
        {
            return _db.KeyExists(keys.Select(k => (RedisKey)k).ToArray());
        }

        /// <summary>
        /// 删除指定 key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool KeyDelete(RedisKey key)
        {
            return _db.KeyDelete(key);
        }

        /// <summary>
        /// 删除多个Key
        /// </summary>
        /// <param name="keys">待删除的key集合</param>
        /// <returns>删除key的数量</returns>
        public long KeyDelete(IEnumerable<string> keys)
        {
            return _db.KeyDelete(keys.Select(k => (RedisKey)k).ToArray());
        }

        /// <summary>
        /// 设置指定key过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public bool KeyExpire(RedisKey key, TimeSpan? expiry)
        {
            return _db.KeyExpire(key, expiry);
        }

        /// <summary>
        /// 设置指定key过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public bool KeyExpire(RedisKey key, DateTime? expiry)
        {
            return _db.KeyExpire(key, expiry);
        }

        #endregion

        #region Key Async

        /// <summary>
        /// Copies the value from the <paramref name="sourceKey"/> to the specified <paramref name="destinationKey"/>.
        /// </summary>
        /// <param name="sourceKey">The key of the source value to copy.</param>
        /// <param name="destinationKey">The destination key to copy the source to.</param>
        /// <param name="destinationDatabase">The database ID to store <paramref name="destinationKey"/> in. If default (-1), current database is used.</param>
        /// <param name="replace">Whether to overwrite an existing values at <paramref name="destinationKey"/>. If <see langword="false"/> and the key exists, the copy will not succeed.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns><see langword="true"/> if key was copied. <see langword="false"/> if key was not copied.</returns>
        /// <remarks><seealso href="https://redis.io/commands/copy"/></remarks>
        public async Task<bool> KeyCopyAsync(RedisKey sourceKey, RedisKey destinationKey, int destinationDatabase = -1, bool replace = false, CommandFlags flags = CommandFlags.None)
        {
            return await _db.KeyCopyAsync(sourceKey, destinationKey, destinationDatabase, replace, flags);
        }

        /// <summary>
        /// Removes the specified key. A key is ignored if it does not exist.
        /// If UNLINK is available (Redis 4.0+), it will be used.
        /// </summary>
        /// <param name="key">The key to delete.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns><see langword="true"/> if the key was removed.</returns>
        /// <remarks>
        /// <seealso href="https://redis.io/commands/del"/>,
        /// <seealso href="https://redis.io/commands/unlink"/>
        /// </remarks>
        public async Task<bool> KeyDeleteAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return await _db.KeyDeleteAsync(key, flags);
        }

        /// <summary>
        /// Removes the specified keys. A key is ignored if it does not exist.
        /// If UNLINK is available (Redis 4.0+), it will be used.
        /// </summary>
        /// <param name="keys">The keys to delete.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The number of keys that were removed.</returns>
        /// <remarks>
        /// <seealso href="https://redis.io/commands/del"/>,
        /// <seealso href="https://redis.io/commands/unlink"/>
        /// </remarks>
        public async Task<long> KeyDeleteAsync(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            return await _db.KeyDeleteAsync(keys, flags);
        }

        /// <summary>
        /// Serialize the value stored at key in a Redis-specific format and return it to the user.
        /// The returned value can be synthesized back into a Redis key using the RESTORE command.
        /// </summary>
        /// <param name="key">The key to dump.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The serialized value.</returns>
        /// <remarks><seealso href="https://redis.io/commands/dump"/></remarks>
        public async Task<byte[]?> KeyDumpAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return await _db.KeyDumpAsync(key, flags);
        }

        /// <summary>
        /// Returns the internal encoding for the Redis object stored at <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key to dump.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The Redis encoding for the value or <see langword="null"/> is the key does not exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/object-encoding"/></remarks>
        public async Task<string?> KeyEncodingAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return await _db.KeyEncodingAsync(key, flags);
        }

        /// <summary>
        /// Returns if key exists.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns><see langword="true"/> if the key exists. <see langword="false"/> if the key does not exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/exists"/></remarks>
        public async Task<bool> KeyExistsAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return await _db.KeyExistsAsync(key, flags);
        }

        /// <summary>
        /// Indicates how many of the supplied keys exists.
        /// </summary>
        /// <param name="keys">The keys to check.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The number of keys that existed.</returns>
        /// <remarks><seealso href="https://redis.io/commands/exists"/></remarks>
        public async Task<long> KeyExistsAsync(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            return await _db.KeyExistsAsync(keys, flags);
        }

        /// <summary>
        /// Set a timeout on <paramref name="key"/>.
        /// After the timeout has expired, the key will automatically be deleted.
        /// A key with an associated timeout is said to be volatile in Redis terminology.
        /// </summary>
        /// <param name="key">The key to set the expiration for.</param>
        /// <param name="expiry">The timeout to set.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns><see langword="true"/> if the timeout was set. <see langword="false"/> if key does not exist or the timeout could not be set.</returns>
        /// <remarks>
        /// If key is updated before the timeout has expired, then the timeout is removed as if the PERSIST command was invoked on key.
        /// <para>
        /// For Redis versions &lt; 2.1.3, existing timeouts cannot be overwritten.
        /// So, if key already has an associated timeout, it will do nothing and return 0.
        /// </para>
        /// <para>
        /// Since Redis 2.1.3, you can update the timeout of a key.
        /// It is also possible to remove the timeout using the PERSIST command.
        /// See the page on key expiry for more information.
        /// </para>
        /// <para>
        /// <seealso href="https://redis.io/commands/expire"/>,
        /// <seealso href="https://redis.io/commands/pexpire"/>,
        /// <seealso href="https://redis.io/commands/persist"/>
        /// </para>
        /// </remarks>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public async Task<bool> KeyExpireAsync(RedisKey key, TimeSpan? expiry, CommandFlags flags)
        {
            return await _db.KeyExpireAsync(key, expiry, flags);
        }

        /// <summary>
        /// Set a timeout on <paramref name="key"/>.
        /// After the timeout has expired, the key will automatically be deleted.
        /// A key with an associated timeout is said to be volatile in Redis terminology.
        /// </summary>
        /// <param name="key">The key to set the expiration for.</param>
        /// <param name="expiry">The timeout to set.</param>
        /// <param name="when">In Redis 7+, we choose under which condition the expiration will be set using <see cref="ExpireWhen"/>.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns><see langword="true"/> if the timeout was set. <see langword="false"/> if key does not exist or the timeout could not be set.</returns>
        /// <remarks>
        /// <seealso href="https://redis.io/commands/expire"/>,
        /// <seealso href="https://redis.io/commands/pexpire"/>
        /// </remarks>
        public async Task<bool> KeyExpireAsync(RedisKey key, TimeSpan? expiry, ExpireWhen when = ExpireWhen.Always, CommandFlags flags = CommandFlags.None)
        {
            return await _db.KeyExpireAsync(key, expiry, when, flags);
        }

        /// <summary>
        /// Set a timeout on <paramref name="key"/>.
        /// After the timeout has expired, the key will automatically be deleted.
        /// A key with an associated timeout is said to be volatile in Redis terminology.
        /// </summary>
        /// <param name="key">The key to set the expiration for.</param>
        /// <param name="expiry">The exact date to expiry to set.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns><see langword="true"/> if the timeout was set. <see langword="false"/> if key does not exist or the timeout could not be set.</returns>
        /// <remarks>
        /// If key is updated before the timeout has expired, then the timeout is removed as if the PERSIST command was invoked on key.
        /// <para>
        /// For Redis versions &lt; 2.1.3, existing timeouts cannot be overwritten.
        /// So, if key already has an associated timeout, it will do nothing and return 0.
        /// </para>
        /// <para>
        /// Since Redis 2.1.3, you can update the timeout of a key.
        /// It is also possible to remove the timeout using the PERSIST command.
        /// See the page on key expiry for more information.
        /// </para>
        /// <para>
        /// <seealso href="https://redis.io/commands/expireat"/>,
        /// <seealso href="https://redis.io/commands/pexpireat"/>,
        /// <seealso href="https://redis.io/commands/persist"/>
        /// </para>
        /// </remarks>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public async Task<bool> KeyExpireAsync(RedisKey key, DateTime? expiry, CommandFlags flags)
        {
            return await _db.KeyExpireAsync(key, expiry, flags);
        }

        /// <summary>
        /// Set a timeout on <paramref name="key"/>.
        /// After the timeout has expired, the key will automatically be deleted.
        /// A key with an associated timeout is said to be volatile in Redis terminology.
        /// </summary>
        /// <param name="key">The key to set the expiration for.</param>
        /// <param name="expiry">The timeout to set.</param>
        /// <param name="when">In Redis 7+, we choose under which condition the expiration will be set using <see cref="ExpireWhen"/>.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns><see langword="true"/> if the timeout was set. <see langword="false"/> if key does not exist or the timeout could not be set.</returns>
        /// <remarks>
        /// <seealso href="https://redis.io/commands/expire"/>,
        /// <seealso href="https://redis.io/commands/pexpire"/>
        /// </remarks>
        public async Task<bool> KeyExpireAsync(RedisKey key, DateTime? expiry, ExpireWhen when = ExpireWhen.Always, CommandFlags flags = CommandFlags.None)
        {
            return await _db.KeyExpireAsync(key, expiry, when, flags);
        }

        /// <summary>
        /// Returns the absolute time at which the given <paramref name="key"/> will expire, if it exists and has an expiration.
        /// </summary>
        /// <param name="key">The key to get the expiration for.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The time at which the given key will expire, or <see langword="null"/> if the key does not exist or has no associated expiration time.</returns>
        /// <remarks>
        /// <seealso href="https://redis.io/commands/expiretime"/>,
        /// <seealso href="https://redis.io/commands/pexpiretime"/>
        /// </remarks>
        public async Task<DateTime?> KeyExpireTimeAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return await _db.KeyExpireTimeAsync(key, flags);
        }

        /// <summary>
        /// Returns the logarithmic access frequency counter of the object stored at <paramref name="key"/>.
        /// The command is only available when the <c>maxmemory-policy</c> configuration directive is set to
        /// one of <see href="https://redis.io/docs/manual/eviction/#the-new-lfu-mode">the LFU policies</see>.
        /// </summary>
        /// <param name="key">The key to get a frequency count for.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The number of logarithmic access frequency counter, (<see langword="null"/> if the key does not exist).</returns>
        /// <remarks><seealso href="https://redis.io/commands/object-freq"/></remarks>
        public async Task<long?> KeyFrequencyAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return await _db.KeyFrequencyAsync(key, flags);
        }

        /// <summary>
        /// Returns the time since the object stored at the specified key is idle (not requested by read or write operations).
        /// </summary>
        /// <param name="key">The key to get the time of.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The time since the object stored at the specified key is idle.</returns>
        /// <remarks><seealso href="https://redis.io/commands/object"/></remarks>
        public async Task<TimeSpan?> KeyIdleTimeAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return await _db.KeyIdleTimeAsync(key, flags);
        }

        /// <summary>
        /// Move key from the currently selected database (see SELECT) to the specified destination database.
        /// When key already exists in the destination database, or it does not exist in the source database, it does nothing.
        /// It is possible to use MOVE as a locking primitive because of this.
        /// </summary>
        /// <param name="key">The key to move.</param>
        /// <param name="database">The database to move the key to.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns><see langword="true"/> if key was moved. <see langword="false"/> if key was not moved.</returns>
        /// <remarks><seealso href="https://redis.io/commands/move"/></remarks>
        public async Task<bool> KeyMoveAsync(RedisKey key, int database, CommandFlags flags = CommandFlags.None)
        {
            return await _db.KeyMoveAsync(key, database, flags);
        }

        /// <summary>
        /// Remove the existing timeout on key, turning the key from volatile (a key with an expire set) to persistent (a key that will never expire as no timeout is associated).
        /// </summary>
        /// <param name="key">The key to persist.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns><see langword="true"/> if the timeout was removed. <see langword="false"/> if key does not exist or does not have an associated timeout.</returns>
        /// <remarks><seealso href="https://redis.io/commands/persist"/></remarks>
        public async Task<bool> KeyPersistAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return await _db.KeyPersistAsync(key, flags);
        }

        /// <summary>
        /// Return a random key from the currently selected database.
        /// </summary>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The random key, or nil when the database is empty.</returns>
        /// <remarks><seealso href="https://redis.io/commands/randomkey"/></remarks>
        public async Task<RedisKey> KeyRandomAsync(CommandFlags flags = CommandFlags.None)
        {
            return await _db.KeyRandomAsync(flags);
        }

        /// <summary>
        /// Returns the reference count of the object stored at <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key to get a reference count for.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The number of references (<see langword="Null"/> if the key does not exist).</returns>
        /// <remarks><seealso href="https://redis.io/commands/object-refcount"/></remarks>
        public async Task<long?> KeyRefCountAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return await _db.KeyRefCountAsync(key, flags);
        }

        /// <summary>
        /// Renames <paramref name="key"/> to <paramref name="newKey"/>.
        /// It returns an error when the source and destination names are the same, or when key does not exist.
        /// </summary>
        /// <param name="key">The key to rename.</param>
        /// <param name="newKey">The key to rename to.</param>
        /// <param name="when">What conditions to rename under (defaults to always).</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns><see langword="true"/> if the key was renamed, <see langword="false"/> otherwise.</returns>
        /// <remarks>
        /// <seealso href="https://redis.io/commands/rename"/>,
        /// <seealso href="https://redis.io/commands/renamenx"/>
        /// </remarks>
        public async Task<bool> KeyRenameAsync(RedisKey key, RedisKey newKey, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            return await _db.KeyRenameAsync(key, newKey, when, flags);
        }

        /// <summary>
        /// Create a key associated with a value that is obtained by deserializing the provided serialized value (obtained via DUMP).
        /// If <paramref name="expiry"/> is 0 the key is created without any expire, otherwise the specified expire time (in milliseconds) is set.
        /// </summary>
        /// <param name="key">The key to restore.</param>
        /// <param name="value">The value of the key.</param>
        /// <param name="expiry">The expiry to set.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <remarks><seealso href="https://redis.io/commands/restore"/></remarks>
        public async Task KeyRestoreAsync(RedisKey key, byte[] value, TimeSpan? expiry = null, CommandFlags flags = CommandFlags.None)
        {
            await _db.KeyRestoreAsync(key, value, expiry, flags);
        }

        /// <summary>
        /// Returns the remaining time to live of a key that has a timeout.
        /// This introspection capability allows a Redis client to check how many seconds a given key will continue to be part of the dataset.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>TTL, or nil when key does not exist or does not have a timeout.</returns>
        /// <remarks><seealso href="https://redis.io/commands/ttl"/></remarks>
        public async Task<TimeSpan?> KeyTimeToLiveAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return await _db.KeyTimeToLiveAsync(key, flags);
        }

        /// <summary>
        /// Alters the last access time of a key.
        /// </summary>
        /// <param name="key">The key to touch.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns><see langword="true"/> if the key was touched, <see langword="false"/> otherwise.</returns>
        /// <remarks><seealso href="https://redis.io/commands/touch"/></remarks>
        public async Task<bool> KeyTouchAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return await _db.KeyTouchAsync(key, flags);
        }

        /// <summary>
        /// Alters the last access time of the specified <paramref name="keys"/>. A key is ignored if it does not exist.
        /// </summary>
        /// <param name="keys">The keys to touch.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The number of keys that were touched.</returns>
        /// <remarks><seealso href="https://redis.io/commands/touch"/></remarks>
        public async Task<long> KeyTouchAsync(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            return await _db.KeyTouchAsync(keys, flags);
        }

        /// <summary>
        /// Returns the string representation of the type of the value stored at key.
        /// The different types that can be returned are: string, list, set, zset and hash.
        /// </summary>
        /// <param name="key">The key to get the type of.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>Type of key, or none when key does not exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/type"/></remarks>
        public async Task<RedisType> KeyTypeAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return await _db.KeyTypeAsync(key, flags);
        }

        /// <summary>
        /// 判断给定的 key 是否存在
        /// </summary>
        /// <param name="key">键值</param>
        /// <returns></returns>
        public async Task<bool> KeyExistsAsync(string key) => await _db.KeyExistsAsync(key);

        /// <summary>
        /// 返回所提供的 keys 存在的数量
        /// </summary>
        /// <param name="keys">键值列</param>
        /// <returns></returns>
        public async Task<long> KeyExistsAsync(IEnumerable<string> keys) => await _db.KeyExistsAsync(keys.Select(k => (RedisKey)k).ToArray());

        /// <summary>
        /// 删除指定 key
        /// </summary>
        /// <param name="key">键值</param>
        /// <returns></returns>
        public async Task<bool> KeyDeleteAsync(string key) => await _db.KeyDeleteAsync((RedisKey)key);

        /// <summary>
        /// 删除多个 key
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public async Task<long> KeyDeleteAsync(IEnumerable<string> keys) => await _db.KeyDeleteAsync(keys.Select(k => (RedisKey)k).ToArray());

        /// <summary>
        /// Set a timeout on <paramref name="key"/>.
        /// After the timeout has expired, the key will automatically be deleted.
        /// A key with an associated timeout is said to be volatile in Redis terminology.
        /// </summary>
        /// <param name="key">The key to set the expiration for.</param>
        /// <param name="expiry">The timeout to set.</param>
        /// <returns><see langword="true"/> if the timeout was set. <see langword="false"/> if key does not exist or the timeout could not be set.</returns>
        /// <remarks>
        /// If key is updated before the timeout has expired, then the timeout is removed as if the PERSIST command was invoked on key.
        /// <para>
        /// For Redis versions &lt; 2.1.3, existing timeouts cannot be overwritten.
        /// So, if key already has an associated timeout, it will do nothing and return 0.
        /// </para>
        /// <para>
        /// Since Redis 2.1.3, you can update the timeout of a key.
        /// It is also possible to remove the timeout using the PERSIST command.
        /// See the page on key expiry for more information.
        /// </para>
        /// <para>
        /// <seealso href="https://redis.io/commands/expire"/>,
        /// <seealso href="https://redis.io/commands/pexpire"/>,
        /// <seealso href="https://redis.io/commands/persist"/>
        /// </para>
        /// </remarks>
        public async Task<bool> KeyExpireAsync(string key, TimeSpan? expiry) => await _db.KeyExpireAsync(key, expiry);

        /// <summary>
        /// Set a timeout on <paramref name="key"/>.
        /// After the timeout has expired, the key will automatically be deleted.
        /// A key with an associated timeout is said to be volatile in Redis terminology.
        /// </summary>
        /// <param name="key">The key to set the expiration for.</param>
        /// <param name="expiry">The timeout to set.</param>
        /// <returns><see langword="true"/> if the timeout was set. <see langword="false"/> if key does not exist or the timeout could not be set.</returns>
        /// <remarks>
        /// <seealso href="https://redis.io/commands/expire"/>,
        /// <seealso href="https://redis.io/commands/pexpire"/>
        /// </remarks>
        public async Task<bool> KeyExpireAsync(string key, DateTime? expiry) => await _db.KeyExpireAsync(key, expiry);

        #endregion

        #region List

        /// <summary>
        /// Returns the element at index in the list stored at key.
        /// The index is zero-based, so 0 means the first element, 1 the second element and so on.
        /// Negative indices can be used to designate elements starting at the tail of the list.
        /// Here, -1 means the last element, -2 means the penultimate and so forth.
        /// </summary>
        /// <param name="key">The key of the list.</param>
        /// <param name="index">The index position to get the value at.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The requested element, or nil when index is out of range.</returns>
        /// <remarks><seealso href="https://redis.io/commands/lindex"/></remarks>
        public RedisValue ListGetByIndex(RedisKey key, long index, CommandFlags flags = CommandFlags.None)
        {
            //return (_db.ListGetByIndex(key, index, flags)).ToObject<T>();
            return _db.ListGetByIndex(key, index, flags);
        }

        /// <summary>
        /// Inserts value in the list stored at key either before or after the reference value pivot.
        /// When key does not exist, it is considered an empty list and no operation is performed.
        /// </summary>
        /// <param name="key">The key of the list.</param>
        /// <param name="pivot">The value to insert after.</param>
        /// <param name="value">The value to insert.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The length of the list after the insert operation, or -1 when the value pivot was not found.</returns>
        /// <remarks><seealso href="https://redis.io/commands/linsert"/></remarks>
        public long ListInsertAfter(RedisKey key, RedisValue pivot, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            //return _db.ListInsertAfter(key, pivot.ToRedisValue(), value, flags);
            return _db.ListInsertAfter(key, pivot, value, flags);
        }

        /// <summary>
        /// Inserts value in the list stored at key either before or after the reference value pivot.
        /// When key does not exist, it is considered an empty list and no operation is performed.
        /// </summary>
        /// <param name="key">The key of the list.</param>
        /// <param name="pivot">The value to insert before.</param>
        /// <param name="value">The value to insert.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The length of the list after the insert operation, or -1 when the value pivot was not found.</returns>
        /// <remarks><seealso href="https://redis.io/commands/linsert"/></remarks>
        public long ListInsertBefore(RedisKey key, RedisValue pivot, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            //return _db.ListInsertBefore(key, pivot.ToRedisValue(), value, flags);
            return _db.ListInsertBefore(key, pivot, value, flags);
        }

        /// <summary>
        /// Removes and returns the first element of the list stored at key.
        /// </summary>
        /// <param name="key">The key of the list.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The value of the first element, or nil when key does not exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/lpop"/></remarks>
        public RedisValue ListLeftPop(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            //return (_db.ListLeftPop(key, flags)).ToObject<T>();
            return _db.ListLeftPop(key, flags);
        }

        /// <summary>
        /// Removes and returns count elements from the head of the list stored at key.
        /// If the list contains less than count elements, removes and returns the number of elements in the list.
        /// </summary>
        /// <param name="key">The key of the list.</param>
        /// <param name="count">The number of elements to remove</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>Array of values that were popped, or nil if the key doesn't exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/lpop"/></remarks>
        public RedisValue[] ListLeftPop(RedisKey key, long count, CommandFlags flags = CommandFlags.None)
        {
            //return (_db.ListLeftPop(key, count, flags)).Select(v => v.ToObject<T>()).ToArray();
            return _db.ListLeftPop(key, count, flags);
        }

        /// <summary>
        /// Removes and returns at most <paramref name="count"/> elements from the first non-empty list in <paramref name="keys"/>.
        /// Starts on the left side of the list.
        /// </summary>
        /// <param name="keys">The keys to look through for elements to pop.</param>
        /// <param name="count">The maximum number of elements to pop from the list.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>A span of contiguous elements from the list, or <see cref="ListPopResult.Null"/> if no non-empty lists are found.</returns>
        /// <remarks><seealso href="https://redis.io/commands/lmpop"/></remarks>
        public ListPopResult ListLeftPop(RedisKey[] keys, long count, CommandFlags flags = CommandFlags.None)
        {
            //var redisKeys = keys.Select(k => (RedisKey)k).ToArray();
            return _db.ListLeftPop(keys, count, flags);
        }

        /// <summary>
        /// Scans through the list stored at <paramref name="key"/> looking for <paramref name="element"/>, returning the 0-based
        /// index of the first matching element.
        /// </summary>
        /// <param name="key">The key of the list.</param>
        /// <param name="element">The element to search for.</param>
        /// <param name="rank">The rank of the first element to return, within the sub-list of matching indexes in the case of multiple matches.</param>
        /// <param name="maxLength">The maximum number of elements to scan through before stopping, defaults to 0 (a full scan of the list.)</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The 0-based index of the first matching element, or -1 if not found.</returns>
        /// <remarks><seealso href="https://redis.io/commands/lpos"/></remarks>
        public long ListPosition(RedisKey key, RedisValue element, long rank = 1, long maxLength = 0, CommandFlags flags = CommandFlags.None)
        {
            //return _db.ListPosition(key, element.ToRedisValue(), rank, maxLength, flags);
            return _db.ListPosition(key, element, rank, maxLength, flags);
        }

        /// <summary>
        /// Scans through the list stored at <paramref name="key"/> looking for <paramref name="count"/> instances of <paramref name="element"/>, returning the 0-based
        /// indexes of any matching elements.
        /// </summary>
        /// <param name="key">The key of the list.</param>
        /// <param name="element">The element to search for.</param>
        /// <param name="count">The number of matches to find. A count of 0 will return the indexes of all occurrences of the element.</param>
        /// <param name="rank">The rank of the first element to return, within the sub-list of matching indexes in the case of multiple matches.</param>
        /// <param name="maxLength">The maximum number of elements to scan through before stopping, defaults to 0 (a full scan of the list.)</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>An array of at most <paramref name="count"/> of indexes of matching elements. If none are found, and empty array is returned.</returns>
        /// <remarks><seealso href="https://redis.io/commands/lpos"/></remarks>
        public long[] ListPositions(RedisKey key, RedisValue element, long count, long rank = 1, long maxLength = 0, CommandFlags flags = CommandFlags.None)
        {
            //return _db.ListPositions(key, element.ToRedisValue(), count, rank, maxLength, flags);
            return _db.ListPositions(key, element, count, rank, maxLength, flags);
        }

        /// <summary>
        /// Insert the specified value at the head of the list stored at key.
        /// If key does not exist, it is created as empty list before performing the push operations.
        /// </summary>
        /// <param name="key">The key of the list.</param>
        /// <param name="value">The value to add to the head of the list.</param>
        /// <param name="when">Which conditions to add to the list under (defaults to always).</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The length of the list after the push operations.</returns>
        /// <remarks>
        /// <seealso href="https://redis.io/commands/lpush"/>,
        /// <seealso href="https://redis.io/commands/lpushx"/>
        /// </remarks>
        public long ListLeftPush(RedisKey key, RedisValue value, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            //return _db.ListLeftPush(key, value.ToRedisValue(), when, flags);
            return _db.ListLeftPush(key, value, when, flags);
        }

        /// <summary>
        /// Insert the specified value at the head of the list stored at key.
        /// If key does not exist, it is created as empty list before performing the push operations.
        /// </summary>
        /// <param name="key">The key of the list.</param>
        /// <param name="values">The value to add to the head of the list.</param>
        /// <param name="when">Which conditions to add to the list under (defaults to always).</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The length of the list after the push operations.</returns>
        /// <remarks>
        /// <seealso href="https://redis.io/commands/lpush"/>,
        /// <seealso href="https://redis.io/commands/lpushx"/>
        /// </remarks>
        public long ListLeftPush(RedisKey key, RedisValue[] values, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            //return _db.ListLeftPush(key, values.Select(v => v.ToRedisValue()).ToArray(), when, flags);
            return _db.ListLeftPush(key, values, when, flags);
        }

        /// <summary>
        /// Insert all the specified values at the head of the list stored at key.
        /// If key does not exist, it is created as empty list before performing the push operations.
        /// Elements are inserted one after the other to the head of the list, from the leftmost element to the rightmost element.
        /// So for instance the command <c>LPUSH mylist a b c</c> will result into a list containing c as first element, b as second element and a as third element.
        /// </summary>
        /// <param name="key">The key of the list.</param>
        /// <param name="values">The values to add to the head of the list.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The length of the list after the push operations.</returns>
        /// <remarks><seealso href="https://redis.io/commands/lpush"/></remarks>
        public long ListLeftPush(RedisKey key, RedisValue[] values, CommandFlags flags)
        {
            //return _db.ListLeftPush(key, values.Select(v => v.ToRedisValue()).ToArray(), flags);
            return _db.ListLeftPush(key, values, flags);
        }

        /// <summary>
        /// Returns the length of the list stored at key. If key does not exist, it is interpreted as an empty list and 0 is returned.
        /// </summary>
        /// <param name="key">The key of the list.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The length of the list at key.</returns>
        /// <remarks><seealso href="https://redis.io/commands/llen"/></remarks>
        public long ListLength(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _db.ListLength(key, flags);
        }

        /// <summary>
        /// Returns and removes the first or last element of the list stored at <paramref name="sourceKey"/>, and pushes the element
        /// as the first or last element of the list stored at <paramref name="destinationKey"/>.
        /// </summary>
        /// <param name="sourceKey">The key of the list to remove from.</param>
        /// <param name="destinationKey">The key of the list to move to.</param>
        /// <param name="sourceSide">What side of the <paramref name="sourceKey"/> list to remove from.</param>
        /// <param name="destinationSide">What side of the <paramref name="destinationKey"/> list to move to.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The element being popped and pushed or <see cref="RedisValue.Null"/> if there is no element to move.</returns>
        /// <remarks><seealso href="https://redis.io/commands/lmove"/></remarks>
        public RedisValue ListMove(RedisKey sourceKey, RedisKey destinationKey, ListSide sourceSide, ListSide destinationSide, CommandFlags flags = CommandFlags.None)
        {
            //return (_db.ListMove(sourceKey, destinationKey, sourceSide, destinationSide, flags)).ToObject<T>();
            return _db.ListMove(sourceKey, destinationKey, sourceSide, destinationSide, flags);
        }

        /// <summary>
        /// Returns the specified elements of the list stored at key.
        /// The offsets start and stop are zero-based indexes, with 0 being the first element of the list (the head of the list), 1 being the next element and so on.
        /// These offsets can also be negative numbers indicating offsets starting at the end of the list.For example, -1 is the last element of the list, -2 the penultimate, and so on.
        /// Note that if you have a list of numbers from 0 to 100, LRANGE list 0 10 will return 11 elements, that is, the rightmost item is included.
        /// </summary>
        /// <param name="key">The key of the list.</param>
        /// <param name="start">The start index of the list.</param>
        /// <param name="stop">The stop index of the list.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>List of elements in the specified range.</returns>
        /// <remarks><seealso href="https://redis.io/commands/lrange"/></remarks>
        public RedisValue[] ListRange(RedisKey key, long start = 0, long stop = -1, CommandFlags flags = CommandFlags.None)
        {
            //return (_db.ListRange(key, start, stop, flags)).Select(v => v.ToObject<T>()).ToArray();
            return _db.ListRange(key, start, stop, flags);
        }

        /// <summary>
        /// Removes the first count occurrences of elements equal to value from the list stored at key.
        /// The count argument influences the operation in the following ways:
        /// <list type="bullet">
        ///     <item>count &gt; 0: Remove elements equal to value moving from head to tail.</item>
        ///     <item>count &lt; 0: Remove elements equal to value moving from tail to head.</item>
        ///     <item>count = 0: Remove all elements equal to value.</item>
        /// </list>
        /// </summary>
        /// <param name="key">The key of the list.</param>
        /// <param name="value">The value to remove from the list.</param>
        /// <param name="count">The count behavior (see method summary).</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The number of removed elements.</returns>
        /// <remarks><seealso href="https://redis.io/commands/lrem"/></remarks>
        public long ListRemove(RedisKey key, RedisValue value, long count = 0, CommandFlags flags = CommandFlags.None)
        {
            //return _db.ListRemove(key, value.ToRedisValue(), count, flags);
            return _db.ListRemove(key, value, count, flags);
        }

        /// <summary>
        /// Removes and returns the last element of the list stored at key.
        /// </summary>
        /// <param name="key">The key of the list.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The element being popped.</returns>
        /// <remarks><seealso href="https://redis.io/commands/rpop"/></remarks>
        public RedisValue ListRightPop(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            //return (_db.ListRightPop(key, flags)).ToObject<T>();
            return _db.ListRightPop(key, flags);
        }

        /// <summary>
        /// Removes and returns count elements from the end the list stored at key.
        /// If the list contains less than count elements, removes and returns the number of elements in the list.
        /// </summary>
        /// <param name="key">The key of the list.</param>
        /// <param name="count">The number of elements to pop</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>Array of values that were popped, or nil if the key doesn't exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/rpop"/></remarks>
        public RedisValue[] ListRightPop(RedisKey key, long count, CommandFlags flags = CommandFlags.None)
        {
            //return (_db.ListRightPop(key, count, flags)).Select(v => v.ToObject<T>()).ToArray();
            return _db.ListRightPop(key, count, flags);
        }

        /// <summary>
        /// Removes and returns at most <paramref name="count"/> elements from the first non-empty list in <paramref name="keys"/>.
        /// Starts on the right side of the list.
        /// </summary>
        /// <param name="keys">The keys to look through for elements to pop.</param>
        /// <param name="count">The maximum number of elements to pop from the list.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>A span of contiguous elements from the list, or <see cref="ListPopResult.Null"/> if no non-empty lists are found.</returns>
        /// <remarks><seealso href="https://redis.io/commands/lmpop"/></remarks>
        public ListPopResult ListRightPop(RedisKey[] keys, long count, CommandFlags flags = CommandFlags.None)
        {
            //var redisKeys = keys.Select(k => (RedisKey)k).ToArray();
            return _db.ListRightPop(keys, count, flags);
        }

        /// <summary>
        /// Atomically returns and removes the last element (tail) of the list stored at source, and pushes the element at the first element (head) of the list stored at destination.
        /// </summary>
        /// <param name="source">The key of the source list.</param>
        /// <param name="destination">The key of the destination list.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The element being popped and pushed.</returns>
        /// <remarks><seealso href="https://redis.io/commands/rpoplpush"/></remarks>
        public RedisValue ListRightPopLeftPush(RedisKey source, RedisKey destination, CommandFlags flags = CommandFlags.None)
        {
            //return (_db.ListRightPopLeftPush(source, destination, flags)).ToObject<T>();
            return _db.ListRightPopLeftPush(source, destination, flags);
        }

        /// <summary>
        /// Insert the specified value at the tail of the list stored at key.
        /// If key does not exist, it is created as empty list before performing the push operation.
        /// </summary>
        /// <param name="key">The key of the list.</param>
        /// <param name="value">The value to add to the tail of the list.</param>
        /// <param name="when">Which conditions to add to the list under.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The length of the list after the push operation.</returns>
        /// <remarks>
        /// <seealso href="https://redis.io/commands/rpush"/>,
        /// <seealso href="https://redis.io/commands/rpushx"/>
        /// </remarks>
        public long ListRightPush(RedisKey key, RedisValue value, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            //return _db.ListRightPush(key, value.ToRedisValue(), when, flags);
            return _db.ListRightPush(key, value, when, flags);
        }

        /// <summary>
        /// Insert the specified value at the tail of the list stored at key.
        /// If key does not exist, it is created as empty list before performing the push operation.
        /// </summary>
        /// <param name="key">The key of the list.</param>
        /// <param name="values">The values to add to the tail of the list.</param>
        /// <param name="when">Which conditions to add to the list under.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The length of the list after the push operation.</returns>
        /// <remarks>
        /// <seealso href="https://redis.io/commands/rpush"/>,
        /// <seealso href="https://redis.io/commands/rpushx"/>
        /// </remarks>
        public long ListRightPush(RedisKey key, RedisValue[] values, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            //return _db.ListRightPush(key, values.Select(v => v.ToRedisValue()).ToArray(), when, flags);
            return _db.ListRightPush(key, values, when, flags);
        }

        /// <summary>
        /// Insert all the specified values at the tail of the list stored at key.
        /// If key does not exist, it is created as empty list before performing the push operation.
        /// Elements are inserted one after the other to the tail of the list, from the leftmost element to the rightmost element.
        /// So for instance the command <c>RPUSH mylist a b c</c> will result into a list containing a as first element, b as second element and c as third element.
        /// </summary>
        /// <param name="key">The key of the list.</param>
        /// <param name="values">The values to add to the tail of the list.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The length of the list after the push operation.</returns>
        /// <remarks><seealso href="https://redis.io/commands/rpush"/></remarks>
        public long ListRightPush(RedisKey key, RedisValue[] values, CommandFlags flags)
        {
            //return _db.ListRightPushAsync(key, values.Select(v => v.ToRedisValue()).ToArray(), flags);
            return _db.ListRightPush(key, values, flags);
        }

        /// <summary>
        /// Sets the list element at index to value.
        /// For more information on the index argument, see <see cref="ListGetByIndex(RedisKey, long, CommandFlags)"/>.
        /// An error is returned for out of range indexes.
        /// </summary>
        /// <param name="key">The key of the list.</param>
        /// <param name="index">The index to set the value at.</param>
        /// <param name="value">The values to add to the list.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <remarks><seealso href="https://redis.io/commands/lset"/></remarks>
        public void ListSetByIndex(RedisKey key, long index, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            //_db.ListSetByIndex(key, index, value.ToRedisValue(), flags);
            _db.ListSetByIndex(key, index, value, flags);
        }

        /// <summary>
        /// Trim an existing list so that it will contain only the specified range of elements specified.
        /// Both start and stop are zero-based indexes, where 0 is the first element of the list (the head), 1 the next element and so on.
        /// For example: <c>LTRIM foobar 0 2</c> will modify the list stored at foobar so that only the first three elements of the list will remain.
        /// start and end can also be negative numbers indicating offsets from the end of the list, where -1 is the last element of the list, -2 the penultimate element and so on.
        /// </summary>
        /// <param name="key">The key of the list.</param>
        /// <param name="start">The start index of the list to trim to.</param>
        /// <param name="stop">The end index of the list to trim to.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <remarks><seealso href="https://redis.io/commands/ltrim"/></remarks>
        public void ListTrim(RedisKey key, long start, long stop, CommandFlags flags = CommandFlags.None)
        {
            _db.ListTrim(key, start, stop, flags);
        }

        /// <summary>
        /// 在列表头部插入值。如果键不存在，先创建再插入值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public long ListLeftPush(RedisKey key, RedisValue value)
        {
            return _db.ListLeftPush(key, value);
        }

        /// <summary>
        /// 在列表尾部插入值。如果键不存在，先创建再插入值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public long Enqueue(RedisKey key, RedisValue value) => _db.ListRightPush(key, value);

        /// <summary>
        /// 在列表尾部插入数组集合。如果键不存在，先创建再插入值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public long Enqueue<T>(RedisKey key, IEnumerable<T> values)
        {
            var redislist = new List<RedisValue>();
            foreach (var item in values)
            {
                redislist.Add(item.ToRedisValue());
            }
            return _db.ListRightPush(key, redislist.ToArray());
        }

        /// <summary>
        /// 移除并返回存储在该键列表的第一个元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Dequeue<T>(RedisKey key) where T : class => (_db.ListLeftPop(key)).ToObject<T>();

        /// <summary>
        /// 返回在该列表上键所对应的元素
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public RedisValue[] ListRange(RedisKey key)
        {
            //var result = _db.ListRange(key);
            //return result.Select(o => o.ToString());
            return _db.ListRange(key);
        }

        /// <summary>
        /// 从队列中读取数据而不出队
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="key"></param>
        /// <param name="start">起始位置</param>
        /// <param name="stop">结束位置</param>
        /// <returns>不指定 start、end 则获取所有数据</returns>
        public IEnumerable<T> ListRange<T>(RedisKey key, long start = 0, long stop = -1) where T : class =>
            (_db.ListRange(key, start, stop)).ToObjects<T>();

        /// <summary>
        /// 列表长度
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long ListLength(RedisKey key)
        {
            return _db.ListLength(key);
        }

        /// <summary>
        /// 删除List中的元素 并返回删除的个数
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">元素</param>
        /// <param name="type">大于零 : 从表头开始向表尾搜索，小于零 : 从表尾开始向表头搜索，等于零：移除表中所有与 VALUE 相等的值</param>
        /// <returns></returns>
        public long ListDelRange(RedisKey key, RedisValue value, long type = 0)
        {
            return _db.ListRemove(key, value, type);
        }

        /// <summary>
        /// 清空指定key 的 List
        /// </summary>
        /// <param name="key"></param>
        public void ListClear(RedisKey key)
        {
            _db.ListTrim(key, 1, 0);
        }

        #endregion

        #region List Async

        /// <summary>
        /// Returns the element at index in the list stored at key.
        /// The index is zero-based, so 0 means the first element, 1 the second element and so on.
        /// Negative indices can be used to designate elements starting at the tail of the list.
        /// Here, -1 means the last element, -2 means the penultimate and so forth.
        /// </summary>
        /// <param name="key">The key of the list.</param>
        /// <param name="index">The index position to get the value at.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The requested element, or nil when index is out of range.</returns>
        /// <remarks><seealso href="https://redis.io/commands/lindex"/></remarks>
        public async Task<RedisValue> ListGetByIndexAsync(RedisKey key, long index, CommandFlags flags = CommandFlags.None)
        {
            //return (await _db.ListGetByIndexAsync(key, index, flags)).ToObject<T>();
            return await _db.ListGetByIndexAsync(key, index, flags);
        }

        /// <summary>
        /// Inserts value in the list stored at key either before or after the reference value pivot.
        /// When key does not exist, it is considered an empty list and no operation is performed.
        /// </summary>
        /// <param name="key">The key of the list.</param>
        /// <param name="pivot">The value to insert after.</param>
        /// <param name="value">The value to insert.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The length of the list after the insert operation, or -1 when the value pivot was not found.</returns>
        /// <remarks><seealso href="https://redis.io/commands/linsert"/></remarks>
        public async Task<long> ListInsertAfterAsync(RedisKey key, RedisValue pivot, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            //return await _db.ListInsertAfterAsync(key, pivot.ToRedisValue(), value, flags);
            return await _db.ListInsertAfterAsync(key, pivot, value, flags);
        }

        /// <summary>
        /// Inserts value in the list stored at key either before or after the reference value pivot.
        /// When key does not exist, it is considered an empty list and no operation is performed.
        /// </summary>
        /// <param name="key">The key of the list.</param>
        /// <param name="pivot">The value to insert before.</param>
        /// <param name="value">The value to insert.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The length of the list after the insert operation, or -1 when the value pivot was not found.</returns>
        /// <remarks><seealso href="https://redis.io/commands/linsert"/></remarks>
        public async Task<long> ListInsertBeforeAsync(RedisKey key, RedisValue pivot, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            //return await _db.ListInsertBeforeAsync(key, pivot.ToRedisValue(), value, flags);
            return await _db.ListInsertBeforeAsync(key, pivot, value, flags);
        }

        /// <summary>
        /// Removes and returns the first element of the list stored at key.
        /// </summary>
        /// <param name="key">The key of the list.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The value of the first element, or nil when key does not exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/lpop"/></remarks>
        public async Task<RedisValue> ListLeftPopAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            //return (await _db.ListLeftPopAsync(key, flags)).ToObject<T>();
            return await _db.ListLeftPopAsync(key, flags);
        }

        /// <summary>
        /// Removes and returns count elements from the head of the list stored at key.
        /// If the list contains less than count elements, removes and returns the number of elements in the list.
        /// </summary>
        /// <param name="key">The key of the list.</param>
        /// <param name="count">The number of elements to remove</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>Array of values that were popped, or nil if the key doesn't exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/lpop"/></remarks>
        public async Task<RedisValue[]> ListLeftPopAsync(RedisKey key, long count, CommandFlags flags = CommandFlags.None)
        {
            //return (await _db.ListLeftPopAsync(key, count, flags)).Select(v => v.ToObject<T>()).ToArray();
            return await _db.ListLeftPopAsync(key, count, flags);
        }

        /// <summary>
        /// Removes and returns at most <paramref name="count"/> elements from the first non-empty list in <paramref name="keys"/>.
        /// Starts on the left side of the list.
        /// </summary>
        /// <param name="keys">The keys to look through for elements to pop.</param>
        /// <param name="count">The maximum number of elements to pop from the list.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>A span of contiguous elements from the list, or <see cref="ListPopResult.Null"/> if no non-empty lists are found.</returns>
        /// <remarks><seealso href="https://redis.io/commands/lmpop"/></remarks>
        public async Task<ListPopResult> ListLeftPopAsync(RedisKey[] keys, long count, CommandFlags flags = CommandFlags.None)
        {
            //var redisKeys = keys.Select(k => (RedisKey)k).ToArray();
            return await _db.ListLeftPopAsync(keys, count, flags);
        }

        /// <summary>
        /// Scans through the list stored at <paramref name="key"/> looking for <paramref name="element"/>, returning the 0-based
        /// index of the first matching element.
        /// </summary>
        /// <param name="key">The key of the list.</param>
        /// <param name="element">The element to search for.</param>
        /// <param name="rank">The rank of the first element to return, within the sub-list of matching indexes in the case of multiple matches.</param>
        /// <param name="maxLength">The maximum number of elements to scan through before stopping, defaults to 0 (a full scan of the list.)</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The 0-based index of the first matching element, or -1 if not found.</returns>
        /// <remarks><seealso href="https://redis.io/commands/lpos"/></remarks>
        public async Task<long> ListPositionAsync(RedisKey key, RedisValue element, long rank = 1, long maxLength = 0, CommandFlags flags = CommandFlags.None)
        {
            //return await _db.ListPositionAsync(key, element.ToRedisValue(), rank, maxLength, flags);
            return await _db.ListPositionAsync(key, element, rank, maxLength, flags);
        }

        /// <summary>
        /// Scans through the list stored at <paramref name="key"/> looking for <paramref name="count"/> instances of <paramref name="element"/>, returning the 0-based
        /// indexes of any matching elements.
        /// </summary>
        /// <param name="key">The key of the list.</param>
        /// <param name="element">The element to search for.</param>
        /// <param name="count">The number of matches to find. A count of 0 will return the indexes of all occurrences of the element.</param>
        /// <param name="rank">The rank of the first element to return, within the sub-list of matching indexes in the case of multiple matches.</param>
        /// <param name="maxLength">The maximum number of elements to scan through before stopping, defaults to 0 (a full scan of the list.)</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>An array of at most <paramref name="count"/> of indexes of matching elements. If none are found, and empty array is returned.</returns>
        /// <remarks><seealso href="https://redis.io/commands/lpos"/></remarks>
        public async Task<long[]> ListPositionsAsync(RedisKey key, RedisValue element, long count, long rank = 1, long maxLength = 0, CommandFlags flags = CommandFlags.None)
        {
            //return await _db.ListPositionsAsync(key, element.ToRedisValue(), count, rank, maxLength, flags);
            return await _db.ListPositionsAsync(key, element, count, rank, maxLength, flags);
        }

        /// <summary>
        /// Insert the specified value at the head of the list stored at key.
        /// If key does not exist, it is created as empty list before performing the push operations.
        /// </summary>
        /// <param name="key">The key of the list.</param>
        /// <param name="value">The value to add to the head of the list.</param>
        /// <param name="when">Which conditions to add to the list under (defaults to always).</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The length of the list after the push operations.</returns>
        /// <remarks>
        /// <seealso href="https://redis.io/commands/lpush"/>,
        /// <seealso href="https://redis.io/commands/lpushx"/>
        /// </remarks>
        public async Task<long> ListLeftPushAsync(RedisKey key, RedisValue value, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            //return await _db.ListLeftPushAsync(key, value.ToRedisValue(), when, flags);
            return await _db.ListLeftPushAsync(key, value, when, flags);
        }

        /// <summary>
        /// Insert the specified value at the head of the list stored at key.
        /// If key does not exist, it is created as empty list before performing the push operations.
        /// </summary>
        /// <param name="key">The key of the list.</param>
        /// <param name="values">The value to add to the head of the list.</param>
        /// <param name="when">Which conditions to add to the list under (defaults to always).</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The length of the list after the push operations.</returns>
        /// <remarks>
        /// <seealso href="https://redis.io/commands/lpush"/>,
        /// <seealso href="https://redis.io/commands/lpushx"/>
        /// </remarks>
        public async Task<long> ListLeftPushAsync(RedisKey key, RedisValue[] values, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            //return await _db.ListLeftPushAsync(key, values.Select(v => v.ToRedisValue()).ToArray(), when, flags);
            return await _db.ListLeftPushAsync(key, values, when, flags);
        }

        /// <summary>
        /// Insert all the specified values at the head of the list stored at key.
        /// If key does not exist, it is created as empty list before performing the push operations.
        /// Elements are inserted one after the other to the head of the list, from the leftmost element to the rightmost element.
        /// So for instance the command <c>LPUSH mylist a b c</c> will result into a list containing c as first element, b as second element and a as third element.
        /// </summary>
        /// <param name="key">The key of the list.</param>
        /// <param name="values">The values to add to the head of the list.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The length of the list after the push operations.</returns>
        /// <remarks><seealso href="https://redis.io/commands/lpush"/></remarks>
        public async Task<long> ListLeftPushAsync(RedisKey key, RedisValue[] values, CommandFlags flags)
        {
            //return await _db.ListLeftPushAsync(key, values.Select(v => v.ToRedisValue()).ToArray(), flags);
            return await _db.ListLeftPushAsync(key, values, flags);
        }

        /// <summary>
        /// Returns the length of the list stored at key. If key does not exist, it is interpreted as an empty list and 0 is returned.
        /// </summary>
        /// <param name="key">The key of the list.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The length of the list at key.</returns>
        /// <remarks><seealso href="https://redis.io/commands/llen"/></remarks>
        public async Task<long> ListLengthAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return await _db.ListLengthAsync(key, flags);
        }

        /// <summary>
        /// Returns and removes the first or last element of the list stored at <paramref name="sourceKey"/>, and pushes the element
        /// as the first or last element of the list stored at <paramref name="destinationKey"/>.
        /// </summary>
        /// <param name="sourceKey">The key of the list to remove from.</param>
        /// <param name="destinationKey">The key of the list to move to.</param>
        /// <param name="sourceSide">What side of the <paramref name="sourceKey"/> list to remove from.</param>
        /// <param name="destinationSide">What side of the <paramref name="destinationKey"/> list to move to.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The element being popped and pushed or <see cref="RedisValue.Null"/> if there is no element to move.</returns>
        /// <remarks><seealso href="https://redis.io/commands/lmove"/></remarks>
        public async Task<RedisValue> ListMoveAsync(RedisKey sourceKey, RedisKey destinationKey, ListSide sourceSide, ListSide destinationSide, CommandFlags flags = CommandFlags.None)
        {
            //return (await _db.ListMoveAsync(sourceKey, destinationKey, sourceSide, destinationSide, flags)).ToObject<T>();
            return await _db.ListMoveAsync(sourceKey, destinationKey, sourceSide, destinationSide, flags);
        }

        /// <summary>
        /// Returns the specified elements of the list stored at key.
        /// The offsets start and stop are zero-based indexes, with 0 being the first element of the list (the head of the list), 1 being the next element and so on.
        /// These offsets can also be negative numbers indicating offsets starting at the end of the list.For example, -1 is the last element of the list, -2 the penultimate, and so on.
        /// Note that if you have a list of numbers from 0 to 100, LRANGE list 0 10 will return 11 elements, that is, the rightmost item is included.
        /// </summary>
        /// <param name="key">The key of the list.</param>
        /// <param name="start">The start index of the list.</param>
        /// <param name="stop">The stop index of the list.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>List of elements in the specified range.</returns>
        /// <remarks><seealso href="https://redis.io/commands/lrange"/></remarks>
        public async Task<RedisValue[]> ListRangeAsync(RedisKey key, long start = 0, long stop = -1, CommandFlags flags = CommandFlags.None)
        {
            //return (await _db.ListRangeAsync(key, start, stop, flags)).Select(v => v.ToObject<T>()).ToArray();
            return await _db.ListRangeAsync(key, start, stop, flags);
        }

        /// <summary>
        /// Removes the first count occurrences of elements equal to value from the list stored at key.
        /// The count argument influences the operation in the following ways:
        /// <list type="bullet">
        ///     <item>count &gt; 0: Remove elements equal to value moving from head to tail.</item>
        ///     <item>count &lt; 0: Remove elements equal to value moving from tail to head.</item>
        ///     <item>count = 0: Remove all elements equal to value.</item>
        /// </list>
        /// </summary>
        /// <param name="key">The key of the list.</param>
        /// <param name="value">The value to remove from the list.</param>
        /// <param name="count">The count behavior (see method summary).</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The number of removed elements.</returns>
        /// <remarks><seealso href="https://redis.io/commands/lrem"/></remarks>
        public async Task<long> ListRemoveAsync(RedisKey key, RedisValue value, long count = 0, CommandFlags flags = CommandFlags.None)
        {
            //return await _db.ListRemoveAsync(key, value.ToRedisValue(), count, flags);
            return await _db.ListRemoveAsync(key, value, count, flags);
        }

        /// <summary>
        /// Removes and returns the last element of the list stored at key.
        /// </summary>
        /// <param name="key">The key of the list.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The element being popped.</returns>
        /// <remarks><seealso href="https://redis.io/commands/rpop"/></remarks>
        public async Task<RedisValue> ListRightPopAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            //return (await _db.ListRightPopAsync(key, flags)).ToObject<T>();
            return await _db.ListRightPopAsync(key, flags);
        }

        /// <summary>
        /// Removes and returns count elements from the end the list stored at key.
        /// If the list contains less than count elements, removes and returns the number of elements in the list.
        /// </summary>
        /// <param name="key">The key of the list.</param>
        /// <param name="count">The number of elements to pop</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>Array of values that were popped, or nil if the key doesn't exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/rpop"/></remarks>
        public async Task<RedisValue[]> ListRightPopAsync(RedisKey key, long count, CommandFlags flags = CommandFlags.None)
        {
            //return (await _db.ListRightPopAsync(key, count, flags)).Select(v => v.ToObject<T>()).ToArray();
            return await _db.ListRightPopAsync(key, count, flags);
        }

        /// <summary>
        /// Removes and returns at most <paramref name="count"/> elements from the first non-empty list in <paramref name="keys"/>.
        /// Starts on the right side of the list.
        /// </summary>
        /// <param name="keys">The keys to look through for elements to pop.</param>
        /// <param name="count">The maximum number of elements to pop from the list.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>A span of contiguous elements from the list, or <see cref="ListPopResult.Null"/> if no non-empty lists are found.</returns>
        /// <remarks><seealso href="https://redis.io/commands/lmpop"/></remarks>
        public async Task<ListPopResult> ListRightPopAsync(RedisKey[] keys, long count, CommandFlags flags = CommandFlags.None)
        {
            //var redisKeys = keys.Select(k => (RedisKey)k).ToArray();
            return await _db.ListRightPopAsync(keys, count, flags);
        }

        /// <summary>
        /// Atomically returns and removes the last element (tail) of the list stored at source, and pushes the element at the first element (head) of the list stored at destination.
        /// </summary>
        /// <param name="source">The key of the source list.</param>
        /// <param name="destination">The key of the destination list.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The element being popped and pushed.</returns>
        /// <remarks><seealso href="https://redis.io/commands/rpoplpush"/></remarks>
        public async Task<RedisValue> ListRightPopLeftPushAsync(RedisKey source, RedisKey destination, CommandFlags flags = CommandFlags.None)
        {
            //return (await _db.ListRightPopLeftPushAsync(source, destination, flags)).ToObject<T>();
            return await _db.ListRightPopLeftPushAsync(source, destination, flags);
        }

        /// <summary>
        /// Insert the specified value at the tail of the list stored at key.
        /// If key does not exist, it is created as empty list before performing the push operation.
        /// </summary>
        /// <param name="key">The key of the list.</param>
        /// <param name="value">The value to add to the tail of the list.</param>
        /// <param name="when">Which conditions to add to the list under.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The length of the list after the push operation.</returns>
        /// <remarks>
        /// <seealso href="https://redis.io/commands/rpush"/>,
        /// <seealso href="https://redis.io/commands/rpushx"/>
        /// </remarks>
        public async Task<long> ListRightPushAsync(RedisKey key, RedisValue value, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            //return await _db.ListRightPushAsync(key, value.ToRedisValue(), when, flags);
            return await _db.ListRightPushAsync(key, value, when, flags);
        }

        /// <summary>
        /// Insert the specified value at the tail of the list stored at key.
        /// If key does not exist, it is created as empty list before performing the push operation.
        /// </summary>
        /// <param name="key">The key of the list.</param>
        /// <param name="values">The values to add to the tail of the list.</param>
        /// <param name="when">Which conditions to add to the list under.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The length of the list after the push operation.</returns>
        /// <remarks>
        /// <seealso href="https://redis.io/commands/rpush"/>,
        /// <seealso href="https://redis.io/commands/rpushx"/>
        /// </remarks>
        public async Task<long> ListRightPushAsync(RedisKey key, RedisValue[] values, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            //return await _db.ListRightPushAsync(key, values.Select(v => v.ToRedisValue()).ToArray(), when, flags);
            return await _db.ListRightPushAsync(key, values, when, flags);
        }

        /// <summary>
        /// Insert all the specified values at the tail of the list stored at key.
        /// If key does not exist, it is created as empty list before performing the push operation.
        /// Elements are inserted one after the other to the tail of the list, from the leftmost element to the rightmost element.
        /// So for instance the command <c>RPUSH mylist a b c</c> will result into a list containing a as first element, b as second element and c as third element.
        /// </summary>
        /// <param name="key">The key of the list.</param>
        /// <param name="values">The values to add to the tail of the list.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The length of the list after the push operation.</returns>
        /// <remarks><seealso href="https://redis.io/commands/rpush"/></remarks>
        public async Task<long> ListRightPushAsync(RedisKey key, RedisValue[] values, CommandFlags flags)
        {
            //return await _db.ListRightPushAsync(key, values.Select(v => v.ToRedisValue()).ToArray(), flags);
            return await _db.ListRightPushAsync(key, values, flags);
        }

        /// <summary>
        /// Sets the list element at index to value.
        /// For more information on the index argument, see <see cref="ListGetByIndexAsync(RedisKey, long, CommandFlags)"/>.
        /// An error is returned for out of range indexes.
        /// </summary>
        /// <param name="key">The key of the list.</param>
        /// <param name="index">The index to set the value at.</param>
        /// <param name="value">The values to add to the list.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <remarks><seealso href="https://redis.io/commands/lset"/></remarks>
        public async Task ListSetByIndexAsync(RedisKey key, long index, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            //await _db.ListSetByIndexAsync(key, index, value.ToRedisValue(), flags);
            await _db.ListSetByIndexAsync(key, index, value, flags);
        }

        /// <summary>
        /// Trim an existing list so that it will contain only the specified range of elements specified.
        /// Both start and stop are zero-based indexes, where 0 is the first element of the list (the head), 1 the next element and so on.
        /// For example: <c>LTRIM foobar 0 2</c> will modify the list stored at foobar so that only the first three elements of the list will remain.
        /// start and end can also be negative numbers indicating offsets from the end of the list, where -1 is the last element of the list, -2 the penultimate element and so on.
        /// </summary>
        /// <param name="key">The key of the list.</param>
        /// <param name="start">The start index of the list to trim to.</param>
        /// <param name="stop">The end index of the list to trim to.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <remarks><seealso href="https://redis.io/commands/ltrim"/></remarks>
        public async Task ListTrimAsync(RedisKey key, long start, long stop, CommandFlags flags = CommandFlags.None)
        {
            await _db.ListTrimAsync(key, start, stop, flags);
        }

        /// <summary>
        /// 在列表头部插入值。如果键不存在，先创建再插入值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<long> ListLeftPushAsync(RedisKey key, RedisValue value)
        {
            return await _db.ListLeftPushAsync(key, value);
        }

        /// <summary>
        /// 在列表尾部插入值。如果键不存在，先创建再插入值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<long> EnqueueAsync(RedisKey key, RedisValue value) => await _db.ListRightPushAsync(key, value);

        /// <summary>
        /// 在列表尾部插入数组集合。如果键不存在，先创建再插入值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public async Task<long> EnqueueAsync<T>(RedisKey key, IEnumerable<T> values)
        {
            var redislist = new List<RedisValue>();
            foreach (var item in values)
            {
                redislist.Add(item.ToRedisValue());
            }
            return await _db.ListRightPushAsync(key, redislist.ToArray());
        }

        /// <summary>
        /// 移除并返回存储在该键列表的第一个元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<T> DequeueAsync<T>(RedisKey key) where T : class => (await _db.ListLeftPopAsync(key)).ToObject<T>();

        /// <summary>
        /// 返回在该列表上键所对应的元素
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<RedisValue[]> ListRangeAsync(RedisKey key)
        {
            //var result = await _db.ListRangeAsync(key);
            //return result.Select(o => o.ToString());
            return await _db.ListRangeAsync(key);
        }

        /// <summary>
        /// 从队列中读取数据而不出队
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="key"></param>
        /// <param name="start">起始位置</param>
        /// <param name="stop">结束位置</param>
        /// <returns>不指定 start、end 则获取所有数据</returns>
        public async Task<IEnumerable<T>> ListRangeAsync<T>(RedisKey key, long start = 0, long stop = -1) where T : class =>
            (await _db.ListRangeAsync(key, start, stop)).ToObjects<T>();

        /// <summary>
        /// 列表长度
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<long> ListLengthAsync(RedisKey key)
        {
            return await _db.ListLengthAsync(key);
        }

        /// <summary>
        /// 删除List中的元素 并返回删除的个数
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">元素</param>
        /// <param name="type">大于零 : 从表头开始向表尾搜索，小于零 : 从表尾开始向表头搜索，等于零：移除表中所有与 VALUE 相等的值</param>
        /// <returns></returns>
        public async Task<long> ListDelRangeAsync(RedisKey key, RedisValue value, long type = 0)
        {
            return await _db.ListRemoveAsync(key, value, type);
        }

        /// <summary>
        /// 清空指定key 的 List
        /// </summary>
        /// <param name="key"></param>
        public async Task ListClearAsync(RedisKey key)
        {
            await _db.ListTrimAsync(key, 1, 0);
        }

        #endregion

        #region String

        /// <summary>
        /// If key already exists and is a string, this command appends the value at the end of the string.
        /// If key does not exist it is created and set as an empty string, so APPEND will be similar to SET in this special case.
        /// </summary>
        /// <param name="key">The key of the string.</param>
        /// <param name="value">The value to append to the string.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The length of the string after the append operation.</returns>
        /// <remarks><seealso href="https://redis.io/commands/append"/></remarks>
        public long StringAppend(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            //return _db.StringAppend(key, value.ToRedisValue<T>(), flags);
            return _db.StringAppend(key, value, flags);
        }

        /// <inheritdoc cref="StringBitCount(RedisKey, long, long, StringIndexType, CommandFlags)" />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public long StringBitCount(RedisKey key, long start, long end, CommandFlags flags)
        {
            return _db.StringBitCount(key, start, end, flags);
        }

        /// <summary>
        /// Count the number of set bits (population counting) in a string.
        /// By default all the bytes contained in the string are examined.
        /// It is possible to specify the counting operation only in an interval passing the additional arguments start and end.
        /// Like for the GETRANGE command start and end can contain negative values in order to index bytes starting from the end of the string, where -1 is the last byte, -2 is the penultimate, and so forth.
        /// </summary>
        /// <param name="key">The key of the string.</param>
        /// <param name="start">The start byte to count at.</param>
        /// <param name="end">The end byte to count at.</param>
        /// <param name="indexType">In Redis 7+, we can choose if <paramref name="start"/> and <paramref name="end"/> specify a bit index or byte index (defaults to <see cref="StringIndexType.Byte"/>).</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The number of bits set to 1.</returns>
        /// <remarks><seealso href="https://redis.io/commands/bitcount"/></remarks>
        public long StringBitCount(RedisKey key, long start = 0, long end = -1, StringIndexType indexType = StringIndexType.Byte, CommandFlags flags = CommandFlags.None)
        {
            return _db.StringBitCount(key, start, end, indexType, flags);
        }

        /// <summary>
        /// Perform a bitwise operation between multiple keys (containing string values) and store the result in the destination key.
        /// The BITOP command supports four bitwise operations; note that NOT is a unary operator: the second key should be omitted in this case
        /// and only the first key will be considered.
        /// The result of the operation is always stored at <paramref name="destination"/>.
        /// </summary>
        /// <param name="operation">The operation to perform.</param>
        /// <param name="destination">The destination key to store the result in.</param>
        /// <param name="first">The first key to get the bit value from.</param>
        /// <param name="second">The second key to get the bit value from.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The size of the string stored in the destination key, that is equal to the size of the longest input string.</returns>
        /// <remarks><seealso href="https://redis.io/commands/bitop"/></remarks>
        public long StringBitOperation(Bitwise operation, RedisKey destination, RedisKey first, RedisKey second = default, CommandFlags flags = CommandFlags.None)
        {
            return _db.StringBitOperation(operation, destination, first, second, flags);
        }

        /// <summary>
        /// Perform a bitwise operation between multiple keys (containing string values) and store the result in the destination key.
        /// The BITOP command supports four bitwise operations; note that NOT is a unary operator.
        /// The result of the operation is always stored at <paramref name="destination"/>.
        /// </summary>
        /// <param name="operation">The operation to perform.</param>
        /// <param name="destination">The destination key to store the result in.</param>
        /// <param name="keys">The keys to get the bit values from.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The size of the string stored in the destination key, that is equal to the size of the longest input string.</returns>
        /// <remarks><seealso href="https://redis.io/commands/bitop"/></remarks>
        public long StringBitOperation(Bitwise operation, RedisKey destination, RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            //var redisKeys = keys.Select(x => (RedisKey)x).ToArray();
            return _db.StringBitOperation(operation, destination, keys, flags);
        }

        /// <inheritdoc cref="StringBitPosition(RedisKey, bool, long, long, StringIndexType, CommandFlags)" />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public long StringBitPosition(RedisKey key, bool bit, long start, long end, CommandFlags flags)
        {
            return _db.StringBitPosition(key, bit, start, end, flags);
        }

        /// <summary>
        /// Return the position of the first bit set to 1 or 0 in a string.
        /// The position is returned thinking at the string as an array of bits from left to right where the first byte most significant bit is at position 0, the second byte most significant bit is at position 8 and so forth.
        /// A <paramref name="start"/> and <paramref name="end"/> may be specified - these are in bytes, not bits.
        /// <paramref name="start"/> and <paramref name="end"/> can contain negative values in order to index bytes starting from the end of the string, where -1 is the last byte, -2 is the penultimate, and so forth.
        /// </summary>
        /// <param name="key">The key of the string.</param>
        /// <param name="bit">True to check for the first 1 bit, false to check for the first 0 bit.</param>
        /// <param name="start">The position to start looking (defaults to 0).</param>
        /// <param name="end">The position to stop looking (defaults to -1, unlimited).</param>
        /// <param name="indexType">In Redis 7+, we can choose if <paramref name="start"/> and <paramref name="end"/> specify a bit index or byte index (defaults to <see cref="StringIndexType.Byte"/>).</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>
        /// The command returns the position of the first bit set to 1 or 0 according to the request.
        /// If we look for set bits(the bit argument is 1) and the string is empty or composed of just zero bytes, -1 is returned.
        /// </returns>
        /// <remarks><seealso href="https://redis.io/commands/bitpos"/></remarks>
        public long StringBitPosition(RedisKey key, bool bit, long start = 0, long end = -1, StringIndexType indexType = StringIndexType.Byte, CommandFlags flags = CommandFlags.None)
        {
            return _db.StringBitPosition(key, bit, start, end, indexType, flags);
        }

        /// <summary>
        /// Decrements the number stored at key by decrement.
        /// If the key does not exist, it is set to 0 before performing the operation.
        /// An error is returned if the key contains a value of the wrong type or contains a string that is not representable as integer.
        /// This operation is limited to 64 bit signed integers.
        /// </summary>
        /// <param name="key">The key of the string.</param>
        /// <param name="value">The amount to decrement by (defaults to 1).</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The value of key after the decrement.</returns>
        /// <remarks>
        /// <seealso href="https://redis.io/commands/decrby"/>,
        /// <seealso href="https://redis.io/commands/decr"/>
        /// </remarks>
        public long StringDecrement(RedisKey key, long value = 1, CommandFlags flags = CommandFlags.None)
        {
            return _db.StringDecrement(key, value, flags);
        }

        /// <summary>
        /// Decrements the string representing a floating point number stored at key by the specified decrement.
        /// If the key does not exist, it is set to 0 before performing the operation.
        /// The precision of the output is fixed at 17 digits after the decimal point regardless of the actual internal precision of the computation.
        /// </summary>
        /// <param name="key">The key of the string.</param>
        /// <param name="value">The amount to decrement by (defaults to 1).</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The value of key after the decrement.</returns>
        /// <remarks><seealso href="https://redis.io/commands/incrbyfloat"/></remarks>
        public double StringDecrement(RedisKey key, double value, CommandFlags flags = CommandFlags.None)
        {
            return _db.StringDecrement(key, value, flags);
        }

        /// <summary>
        /// Get the value of key. If the key does not exist the special value nil is returned.
        /// An error is returned if the value stored at key is not a string, because GET only handles string values.
        /// </summary>
        /// <param name="key">The key of the string.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The value of key, or nil when key does not exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/get"/></remarks>
        public RedisValue StringGet(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            //var value = await _db.StringGet(key, flags);
            //return value.ToObject<T>();

            return _db.StringGet(key, flags);
        }

        /// <summary>
        /// Returns the values of all specified keys.
        /// For every key that does not hold a string value or does not exist, the special value nil is returned.
        /// </summary>
        /// <param name="keys">The keys of the strings.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The values of the strings with nil for keys do not exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/mget"/></remarks>
        public RedisValue[] StringGet(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            //var redisKeys = keys.Select(x => (RedisKey)x).ToArray();
            //return (await _db.StringGet(redisKeys, flags)).Select(x => x.ToObject<T>()).ToArray();

            return _db.StringGet(keys, flags);
        }

        /// <summary>
        /// Get the value of key. If the key does not exist the special value nil is returned.
        /// An error is returned if the value stored at key is not a string, because GET only handles string values.
        /// </summary>
        /// <param name="key">The key of the string.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The value of key, or nil when key does not exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/get"/></remarks>
        public Lease<byte>? StringGetLease(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _db.StringGetLease(key, flags);
        }

        /// <summary>
        /// Returns the bit value at offset in the string value stored at key.
        /// When offset is beyond the string length, the string is assumed to be a contiguous space with 0 bits.
        /// </summary>
        /// <param name="key">The key of the string.</param>
        /// <param name="offset">The offset in the string to get a bit at.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The bit value stored at offset.</returns>
        /// <remarks><seealso href="https://redis.io/commands/getbit"/></remarks>
        public bool StringGetBit(RedisKey key, long offset, CommandFlags flags = CommandFlags.None)
        {
            return _db.StringGetBit(key, offset, flags);
        }

        /// <summary>
        /// Returns the substring of the string value stored at key, determined by the offsets start and end (both are inclusive).
        /// Negative offsets can be used in order to provide an offset starting from the end of the string.
        /// So -1 means the last character, -2 the penultimate and so forth.
        /// </summary>
        /// <param name="key">The key of the string.</param>
        /// <param name="start">The start index of the substring to get.</param>
        /// <param name="end">The end index of the substring to get.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The substring of the string value stored at key.</returns>
        /// <remarks><seealso href="https://redis.io/commands/getrange"/></remarks>
        public RedisValue StringGetRange(RedisKey key, long start, long end, CommandFlags flags = CommandFlags.None)
        {
            //return (await _db.StringGetRange(key, start, end, flags)).ToObject<T>();
            return _db.StringGetRange(key, start, end, flags);
        }

        /// <summary>
        /// Atomically sets key to value and returns the old value stored at key.
        /// </summary>
        /// <param name="key">The key of the string.</param>
        /// <param name="value">The value to replace the existing value with.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The old value stored at key, or nil when key did not exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/getset"/></remarks>
        public RedisValue StringGetSet(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            //return (await _db.StringGetSet(key, value.ToRedisValue<T>(), flags)).ToObject<T>();
            return _db.StringGetSet(key, value, flags);
        }

        /// <summary>
        /// Gets the value of <paramref name="key"/> and update its (relative) expiry.
        /// If the key does not exist, the result will be <see cref="RedisValue.Null"/>.
        /// </summary>
        /// <param name="key">The key of the string.</param>
        /// <param name="expiry">The expiry to set. <see langword="null"/> will remove expiry.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The value of key, or nil when key does not exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/getex"/></remarks>
        public RedisValue StringGetSetExpiry(RedisKey key, TimeSpan? expiry, CommandFlags flags = CommandFlags.None)
        {
            //return (await _db.StringGetSetExpiry(key, expiry, flags)).ToObject<T>();
            return _db.StringGetSetExpiry(key, expiry, flags);
        }

        /// <summary>
        /// Gets the value of <paramref name="key"/> and update its (absolute) expiry.
        /// If the key does not exist, the result will be <see cref="RedisValue.Null"/>.
        /// </summary>
        /// <param name="key">The key of the string.</param>
        /// <param name="expiry">The exact date and time to expire at. <see cref="DateTime.MaxValue"/> will remove expiry.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The value of key, or nil when key does not exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/getex"/></remarks>
        public RedisValue StringGetSetExpiry(RedisKey key, DateTime expiry, CommandFlags flags = CommandFlags.None)
        {
            //return (await _db.StringGetSetExpiry(key, expiry, flags)).ToObject<T>();
            return _db.StringGetSetExpiry(key, expiry, flags);
        }

        /// <summary>
        /// Get the value of key and delete the key.
        /// If the key does not exist the special value nil is returned.
        /// An error is returned if the value stored at key is not a string, because GET only handles string values.
        /// </summary>
        /// <param name="key">The key of the string.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The value of key, or nil when key does not exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/getdelete"/></remarks>
        public RedisValue StringGetDelete(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            //return (_db.StringGetDelete(key, flags)).ToObject<T>();
            return _db.StringGetDelete(key, flags);
        }

        /// <summary>
        /// Get the value of key.
        /// If the key does not exist the special value nil is returned.
        /// An error is returned if the value stored at key is not a string, because GET only handles string values.
        /// </summary>
        /// <param name="key">The key of the string.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The value of key and its expiry, or nil when key does not exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/get"/></remarks>
        public RedisValueWithExpiry StringGetWithExpiry(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _db.StringGetWithExpiry(key, flags);
        }

        /// <summary>
        /// Increments the number stored at key by increment.
        /// If the key does not exist, it is set to 0 before performing the operation.
        /// An error is returned if the key contains a value of the wrong type or contains a string that is not representable as integer.
        /// This operation is limited to 64 bit signed integers.
        /// </summary>
        /// <param name="key">The key of the string.</param>
        /// <param name="value">The amount to increment by (defaults to 1).</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The value of key after the increment.</returns>
        /// <remarks>
        /// <seealso href="https://redis.io/commands/incrby"/>,
        /// <seealso href="https://redis.io/commands/incr"/>
        /// </remarks>
        public long StringIncrement(RedisKey key, long value = 1, CommandFlags flags = CommandFlags.None)
        {
            return _db.StringIncrement(key, value, flags);
        }

        /// <summary>
        /// Increments the string representing a floating point number stored at key by the specified increment.
        /// If the key does not exist, it is set to 0 before performing the operation.
        /// The precision of the output is fixed at 17 digits after the decimal point regardless of the actual internal precision of the computation.
        /// </summary>
        /// <param name="key">The key of the string.</param>
        /// <param name="value">The amount to increment by (defaults to 1).</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The value of key after the increment.</returns>
        /// <remarks><seealso href="https://redis.io/commands/incrbyfloat"/></remarks>
        public double StringIncrement(RedisKey key, double value, CommandFlags flags = CommandFlags.None)
        {
            return _db.StringIncrement(key, value, flags);
        }

        /// <summary>
        /// Returns the length of the string value stored at key.
        /// </summary>
        /// <param name="key">The key of the string.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The length of the string at key, or 0 when key does not exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/strlen"/></remarks>
        public long StringLength(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _db.StringLength(key, flags);
        }

        /// <summary>
        /// Implements the longest common subsequence algorithm between the values at <paramref name="first"/> and <paramref name="second"/>,
        /// returning a string containing the common sequence.
        /// Note that this is different than the longest common string algorithm,
        /// since matching characters in the string does not need to be contiguous.
        /// </summary>
        /// <param name="first">The key of the first string.</param>
        /// <param name="second">The key of the second string.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>A string (sequence of characters) of the LCS match.</returns>
        /// <remarks><seealso href="https://redis.io/commands/lcs"/></remarks>
        public string? StringLongestCommonSubsequence(RedisKey first, RedisKey second, CommandFlags flags = CommandFlags.None)
        {
            return _db.StringLongestCommonSubsequence(first, second, flags);
        }

        /// <summary>
        /// Implements the longest common subsequence algorithm between the values at <paramref name="first"/> and <paramref name="second"/>,
        /// returning the legnth of the common sequence.
        /// Note that this is different than the longest common string algorithm,
        /// since matching characters in the string does not need to be contiguous.
        /// </summary>
        /// <param name="first">The key of the first string.</param>
        /// <param name="second">The key of the second string.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The length of the LCS match.</returns>
        /// <remarks><seealso href="https://redis.io/commands/lcs"/></remarks>
        public long StringLongestCommonSubsequenceLength(RedisKey first, RedisKey second, CommandFlags flags = CommandFlags.None)
        {
            return _db.StringLongestCommonSubsequenceLength(first, second, flags);
        }

        /// <summary>
        /// Implements the longest common subsequence algorithm between the values at <paramref name="first"/> and <paramref name="second"/>,
        /// returning a list of all common sequences.
        /// Note that this is different than the longest common string algorithm,
        /// since matching characters in the string does not need to be contiguous.
        /// </summary>
        /// <param name="first">The key of the first string.</param>
        /// <param name="second">The key of the second string.</param>
        /// <param name="minLength">Can be used to restrict the list of matches to the ones of a given minimum length.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The result of LCS algorithm, based on the given parameters.</returns>
        /// <remarks><seealso href="https://redis.io/commands/lcs"/></remarks>
        public LCSMatchResult StringLongestCommonSubsequenceWithMatches(RedisKey first, RedisKey second, long minLength = 0, CommandFlags flags = CommandFlags.None)
        {
            return _db.StringLongestCommonSubsequenceWithMatches(first, second, minLength, flags);
        }

        /// <inheritdoc cref="StringSet(RedisKey, RedisValue, TimeSpan?, bool, When, CommandFlags)" />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool StringSet(RedisKey key, RedisValue value, TimeSpan? expiry, When when)
        {
            return StringSet(key, value, expiry, when);
        }

        /// <inheritdoc cref="StringSet(RedisKey, RedisValue, TimeSpan?, bool, When, CommandFlags)" />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool StringSet(RedisKey key, RedisValue value, TimeSpan? expiry, When when, CommandFlags flags)
        {
            return StringSet(key, value, expiry, when, flags);
        }

        /// <summary>
        /// Set key to hold the string value. If key already holds a value, it is overwritten, regardless of its type.
        /// </summary>
        /// <param name="key">The key of the string.</param>
        /// <param name="value">The value to set.</param>
        /// <param name="expiry">The expiry to set.</param>
        /// <param name="keepTtl">Whether to maintain the existing key's TTL (KEEPTTL flag).</param>
        /// <param name="when">Which condition to set the value under (defaults to always).</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns><see langword="true"/> if the string was set, <see langword="false"/> otherwise.</returns>
        /// <remarks><seealso href="https://redis.io/commands/set"/></remarks>
        public bool StringSet(RedisKey key, RedisValue value, TimeSpan? expiry = null, bool keepTtl = false, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            //return _db.StringSet(key, value.ToRedisValue<T>(), expiry, keepTtl, when, flags);
            return _db.StringSet(key, value, expiry, keepTtl, when, flags);
        }

        /// <summary>
        /// Sets the given keys to their respective values.
        /// If <see cref="When.NotExists"/> is specified, this will not perform any operation at all even if just a single key already exists.
        /// </summary>
        /// <param name="values">The keys and values to set.</param>
        /// <param name="when">Which condition to set the value under (defaults to always).</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns><see langword="true"/> if the keys were set, <see langword="false"/> otherwise.</returns>
        /// <remarks>
        /// <seealso href="https://redis.io/commands/mset"/>,
        /// <seealso href="https://redis.io/commands/msetnx"/>
        /// </remarks>
        public bool StringSet(KeyValuePair<RedisKey, RedisValue>[] values, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            //KeyValuePair<RedisKey, RedisValue>[] redisValues = values.Select(x => new KeyValuePair<RedisKey, RedisValue>(x.Key, x.Value.ToRedisValue<T>())).ToArray();
            //return await _db.StringSet(redisValues, when, flags);

            return _db.StringSet(values, when, flags);
        }

        /// <summary>
        /// Atomically sets key to value and returns the previous value (if any) stored at <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key of the string.</param>
        /// <param name="value">The value to set.</param>
        /// <param name="expiry">The expiry to set.</param>
        /// <param name="when">Which condition to set the value under (defaults to <see cref="When.Always"/>).</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The previous value stored at <paramref name="key"/>, or nil when key did not exist.</returns>
        /// <remarks>
        /// <para>This method uses the <c>SET</c> command with the <c>GET</c> option introduced in Redis 6.2.0 instead of the deprecated <c>GETSET</c> command.</para>
        /// <para><seealso href="https://redis.io/commands/set"/></para>
        /// </remarks>
        public RedisValue StringSetAndGet(RedisKey key, RedisValue value, TimeSpan? expiry, When when, CommandFlags flags)
        {
            //return (_db.StringSetAndGet(key, value.ToRedisValue<T>(), expiry, false, when, flags)).ToObject<T>();
            return _db.StringSetAndGet(key, value, expiry, false, when, flags);
        }

        /// <summary>
        /// Atomically sets key to value and returns the previous value (if any) stored at <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key of the string.</param>
        /// <param name="value">The value to set.</param>
        /// <param name="expiry">The expiry to set.</param>
        /// <param name="keepTtl">Whether to maintain the existing key's TTL (KEEPTTL flag).</param>
        /// <param name="when">Which condition to set the value under (defaults to <see cref="When.Always"/>).</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The previous value stored at <paramref name="key"/>, or nil when key did not exist.</returns>
        /// <remarks>This method uses the SET command with the GET option introduced in Redis 6.2.0 instead of the deprecated GETSET command.</remarks>
        /// <remarks><seealso href="https://redis.io/commands/set"/></remarks>
        public RedisValue StringSetAndGet(RedisKey key, RedisValue value, TimeSpan? expiry = null, bool keepTtl = false, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            //return (_db.StringSetAndGet(key, value.ToRedisValue<T>(), expiry, keepTtl, when, flags)).ToObject<T>();
            return _db.StringSetAndGet(key, value, expiry, keepTtl, when, flags);
        }

        /// <summary>
        /// Sets or clears the bit at offset in the string value stored at key.
        /// The bit is either set or cleared depending on value, which can be either 0 or 1.
        /// When key does not exist, a new string value is created.The string is grown to make sure it can hold a bit at offset.
        /// </summary>
        /// <param name="key">The key of the string.</param>
        /// <param name="offset">The offset in the string to set <paramref name="bit"/>.</param>
        /// <param name="bit">The bit value to set, true for 1, false for 0.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The original bit value stored at offset.</returns>
        /// <remarks><seealso href="https://redis.io/commands/setbit"/></remarks>
        public bool StringSetBit(RedisKey key, long offset, bool bit, CommandFlags flags = CommandFlags.None)
        {
            return _db.StringSetBit(key, offset, bit, flags);
        }

        /// <summary>
        /// Overwrites part of the string stored at key, starting at the specified offset, for the entire length of value.
        /// If the offset is larger than the current length of the string at key, the string is padded with zero-bytes to make offset fit.
        /// Non-existing keys are considered as empty strings, so this command will make sure it holds a string large enough to be able to set value at offset.
        /// </summary>
        /// <param name="key">The key of the string.</param>
        /// <param name="offset">The offset in the string to overwrite.</param>
        /// <param name="value">The value to overwrite with.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The length of the string after it was modified by the command.</returns>
        /// <remarks><seealso href="https://redis.io/commands/setrange"/></remarks>
        public RedisValue StringSetRange(RedisKey key, long offset, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            //return (_db.StringSetRange(key, offset, value.ToRedisValue<T>(), flags)).ToObject<T>();
            return _db.StringSetRange(key, offset, value, flags);
        }

        /// <summary>
        /// 获取string的length
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long StringLength(RedisKey key) => _db.StringLength(key);

        /// <summary>
        /// 添加一个key 或者覆盖key的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool StringSet(RedisKey key, RedisValue value) => _db.StringSet(key, value, null, false, When.Always);

        /// <summary>
        /// 添加一个key 并有过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiry">过期时间</param>
        /// <returns></returns>
        public bool StringSet(RedisKey key, RedisValue value, TimeSpan? expiry = null) => _db.StringSet(key, value, expiry, false, When.Always);

        /// <summary>
        /// 只有在键不存在的时候才设置
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool StringSetNx(RedisKey key, RedisValue value) => _db.StringSet(key, value, null, false, When.NotExists);

        /// <summary>
        /// Get the value of key. If the key does not exist the special value nil is returned.
        /// An error is returned if the value stored at key is not a string, because GET only handles string values.
        /// </summary>
        /// <param name="key">The key of the string.</param>
        /// <returns>The value of key, or nil when key does not exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/get"/></remarks>
        public T StringGet<T>(RedisKey key) => (_db.StringGet(key)).ToObject<T>();

        /// <summary>
        /// Increments the number stored at key by increment.
        /// If the key does not exist, it is set to 0 before performing the operation.
        /// An error is returned if the key contains a value of the wrong type or contains a string that is not representable as integer.
        /// This operation is limited to 64 bit signed integers.
        /// </summary>
        /// <param name="key">The key of the string.</param>
        /// <param name="value">The amount to increment by (defaults to 1).</param>
        /// <returns>The value of key after the increment.</returns>
        /// <remarks>
        /// <seealso href="https://redis.io/commands/incrby"/>,
        /// <seealso href="https://redis.io/commands/incr"/>
        /// </remarks>
        public long StringIncrement(RedisKey key, int value = 1) => _db.StringIncrement(key, value);

        /// <summary>
        /// Decrements the number stored at key by decrement.
        /// If the key does not exist, it is set to 0 before performing the operation.
        /// An error is returned if the key contains a value of the wrong type or contains a string that is not representable as integer.
        /// This operation is limited to 64 bit signed integers.
        /// </summary>
        /// <param name="key">The key of the string.</param>
        /// <param name="value">The amount to decrement by (defaults to 1).</param>
        /// <returns>The value of key after the decrement.</returns>
        /// <remarks>
        /// <seealso href="https://redis.io/commands/decrby"/>,
        /// <seealso href="https://redis.io/commands/decr"/>
        /// </remarks>
        public long StringDecrement(RedisKey key, int value = 1) => _db.StringDecrement(key, value);

        /// <summary>
        /// If key already exists and is a string, this command appends the value at the end of the string.
        /// If key does not exist it is created and set as an empty string, so APPEND will be similar to SET in this special case.
        /// </summary>
        /// <param name="key">The key of the string.</param>
        /// <param name="value">The value to append to the string.</param>
        /// <returns>The length of the string after the append operation.</returns>
        /// <remarks><seealso href="https://redis.io/commands/append"/></remarks>
        public long StringAppend(RedisKey key, string value) => _db.StringAppend(key, value);

        /// <summary>
        /// Returns the substring of the string value stored at key, determined by the offsets start and end (both are inclusive).
        /// Negative offsets can be used in order to provide an offset starting from the end of the string.
        /// So -1 means the last character, -2 the penultimate and so forth.
        /// </summary>
        /// <param name="key">The key of the string.</param>
        /// <param name="start">The start index of the substring to get.</param>
        /// <param name="end">The end index of the substring to get.</param>
        /// <returns>The substring of the string value stored at key.</returns>
        /// <remarks><seealso href="https://redis.io/commands/getrange"/></remarks>
        public string StringGetRange(RedisKey key, int start, int end)
        {
            var value = _db.StringGetRange(key, start, end);
            return value.ToString();
        }

        #endregion

        #region String Async

        /// <summary>
        /// If key already exists and is a string, this command appends the value at the end of the string.
        /// If key does not exist it is created and set as an empty string, so APPEND will be similar to SET in this special case.
        /// </summary>
        /// <param name="key">The key of the string.</param>
        /// <param name="value">The value to append to the string.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The length of the string after the append operation.</returns>
        /// <remarks><seealso href="https://redis.io/commands/append"/></remarks>
        public async Task<long> StringAppendAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            //return await _db.StringAppendAsync(key, value.ToRedisValue<T>(), flags);
            return await _db.StringAppendAsync(key, value, flags);
        }

        /// <inheritdoc cref="StringBitCountAsync(RedisKey, long, long, StringIndexType, CommandFlags)" />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public async Task<long> StringBitCountAsync(RedisKey key, long start, long end, CommandFlags flags)
        {
            return await _db.StringBitCountAsync(key, start, end, flags);
        }

        /// <summary>
        /// Count the number of set bits (population counting) in a string.
        /// By default all the bytes contained in the string are examined.
        /// It is possible to specify the counting operation only in an interval passing the additional arguments start and end.
        /// Like for the GETRANGE command start and end can contain negative values in order to index bytes starting from the end of the string, where -1 is the last byte, -2 is the penultimate, and so forth.
        /// </summary>
        /// <param name="key">The key of the string.</param>
        /// <param name="start">The start byte to count at.</param>
        /// <param name="end">The end byte to count at.</param>
        /// <param name="indexType">In Redis 7+, we can choose if <paramref name="start"/> and <paramref name="end"/> specify a bit index or byte index (defaults to <see cref="StringIndexType.Byte"/>).</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The number of bits set to 1.</returns>
        /// <remarks><seealso href="https://redis.io/commands/bitcount"/></remarks>
        public async Task<long> StringBitCountAsync(RedisKey key, long start = 0, long end = -1, StringIndexType indexType = StringIndexType.Byte, CommandFlags flags = CommandFlags.None)
        {
            return await _db.StringBitCountAsync(key, start, end, indexType, flags);
        }

        /// <summary>
        /// Perform a bitwise operation between multiple keys (containing string values) and store the result in the destination key.
        /// The BITOP command supports four bitwise operations; note that NOT is a unary operator: the second key should be omitted in this case
        /// and only the first key will be considered.
        /// The result of the operation is always stored at <paramref name="destination"/>.
        /// </summary>
        /// <param name="operation">The operation to perform.</param>
        /// <param name="destination">The destination key to store the result in.</param>
        /// <param name="first">The first key to get the bit value from.</param>
        /// <param name="second">The second key to get the bit value from.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The size of the string stored in the destination key, that is equal to the size of the longest input string.</returns>
        /// <remarks><seealso href="https://redis.io/commands/bitop"/></remarks>
        public async Task<long> StringBitOperationAsync(Bitwise operation, RedisKey destination, RedisKey first, RedisKey second = default, CommandFlags flags = CommandFlags.None)
        {
            return await _db.StringBitOperationAsync(operation, destination, first, second, flags);
        }

        /// <summary>
        /// Perform a bitwise operation between multiple keys (containing string values) and store the result in the destination key.
        /// The BITOP command supports four bitwise operations; note that NOT is a unary operator.
        /// The result of the operation is always stored at <paramref name="destination"/>.
        /// </summary>
        /// <param name="operation">The operation to perform.</param>
        /// <param name="destination">The destination key to store the result in.</param>
        /// <param name="keys">The keys to get the bit values from.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The size of the string stored in the destination key, that is equal to the size of the longest input string.</returns>
        /// <remarks><seealso href="https://redis.io/commands/bitop"/></remarks>
        public async Task<long> StringBitOperationAsync(Bitwise operation, RedisKey destination, RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            //var redisKeys = keys.Select(x => (RedisKey)x).ToArray();
            return await _db.StringBitOperationAsync(operation, destination, keys, flags);
        }

        /// <inheritdoc cref="StringBitPositionAsync(RedisKey, bool, long, long, StringIndexType, CommandFlags)" />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public async Task<long> StringBitPositionAsync(RedisKey key, bool bit, long start, long end, CommandFlags flags)
        {
            return await _db.StringBitPositionAsync(key, bit, start, end, flags);
        }

        /// <summary>
        /// Return the position of the first bit set to 1 or 0 in a string.
        /// The position is returned thinking at the string as an array of bits from left to right where the first byte most significant bit is at position 0, the second byte most significant bit is at position 8 and so forth.
        /// A <paramref name="start"/> and <paramref name="end"/> may be specified - these are in bytes, not bits.
        /// <paramref name="start"/> and <paramref name="end"/> can contain negative values in order to index bytes starting from the end of the string, where -1 is the last byte, -2 is the penultimate, and so forth.
        /// </summary>
        /// <param name="key">The key of the string.</param>
        /// <param name="bit">True to check for the first 1 bit, false to check for the first 0 bit.</param>
        /// <param name="start">The position to start looking (defaults to 0).</param>
        /// <param name="end">The position to stop looking (defaults to -1, unlimited).</param>
        /// <param name="indexType">In Redis 7+, we can choose if <paramref name="start"/> and <paramref name="end"/> specify a bit index or byte index (defaults to <see cref="StringIndexType.Byte"/>).</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>
        /// The command returns the position of the first bit set to 1 or 0 according to the request.
        /// If we look for set bits(the bit argument is 1) and the string is empty or composed of just zero bytes, -1 is returned.
        /// </returns>
        /// <remarks><seealso href="https://redis.io/commands/bitpos"/></remarks>
        public async Task<long> StringBitPositionAsync(RedisKey key, bool bit, long start = 0, long end = -1, StringIndexType indexType = StringIndexType.Byte, CommandFlags flags = CommandFlags.None)
        {
            return await _db.StringBitPositionAsync(key, bit, start, end, indexType, flags);
        }

        /// <summary>
        /// Decrements the number stored at key by decrement.
        /// If the key does not exist, it is set to 0 before performing the operation.
        /// An error is returned if the key contains a value of the wrong type or contains a string that is not representable as integer.
        /// This operation is limited to 64 bit signed integers.
        /// </summary>
        /// <param name="key">The key of the string.</param>
        /// <param name="value">The amount to decrement by (defaults to 1).</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The value of key after the decrement.</returns>
        /// <remarks>
        /// <seealso href="https://redis.io/commands/decrby"/>,
        /// <seealso href="https://redis.io/commands/decr"/>
        /// </remarks>
        public async Task<long> StringDecrementAsync(RedisKey key, long value = 1, CommandFlags flags = CommandFlags.None)
        {
            return await _db.StringDecrementAsync(key, value, flags);
        }

        /// <summary>
        /// Decrements the string representing a floating point number stored at key by the specified decrement.
        /// If the key does not exist, it is set to 0 before performing the operation.
        /// The precision of the output is fixed at 17 digits after the decimal point regardless of the actual internal precision of the computation.
        /// </summary>
        /// <param name="key">The key of the string.</param>
        /// <param name="value">The amount to decrement by (defaults to 1).</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The value of key after the decrement.</returns>
        /// <remarks><seealso href="https://redis.io/commands/incrbyfloat"/></remarks>
        public async Task<double> StringDecrementAsync(RedisKey key, double value, CommandFlags flags = CommandFlags.None)
        {
            return await _db.StringDecrementAsync(key, value, flags);
        }

        /// <summary>
        /// Get the value of key. If the key does not exist the special value nil is returned.
        /// An error is returned if the value stored at key is not a string, because GET only handles string values.
        /// </summary>
        /// <param name="key">The key of the string.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The value of key, or nil when key does not exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/get"/></remarks>
        public async Task<RedisValue> StringGetAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            //var value = await _db.StringGetAsync(key, flags);
            //return value.ToObject<T>();

            return await _db.StringGetAsync(key, flags);
        }

        /// <summary>
        /// Returns the values of all specified keys.
        /// For every key that does not hold a string value or does not exist, the special value nil is returned.
        /// </summary>
        /// <param name="keys">The keys of the strings.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The values of the strings with nil for keys do not exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/mget"/></remarks>
        public async Task<RedisValue[]> StringGetAsync(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            //var redisKeys = keys.Select(x => (RedisKey)x).ToArray();
            //return (await _db.StringGetAsync(redisKeys, flags)).Select(x => x.ToObject<T>()).ToArray();
            var redisKeys = keys.Select(x => (RedisKey)x).ToArray();
            return await _db.StringGetAsync(keys, flags);
        }

        /// <summary>
        /// Get the value of key. If the key does not exist the special value nil is returned.
        /// An error is returned if the value stored at key is not a string, because GET only handles string values.
        /// </summary>
        /// <param name="key">The key of the string.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The value of key, or nil when key does not exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/get"/></remarks>
        public async Task<Lease<byte>?> StringGetLeaseAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return await _db.StringGetLeaseAsync(key, flags);
        }

        /// <summary>
        /// Returns the bit value at offset in the string value stored at key.
        /// When offset is beyond the string length, the string is assumed to be a contiguous space with 0 bits.
        /// </summary>
        /// <param name="key">The key of the string.</param>
        /// <param name="offset">The offset in the string to get a bit at.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The bit value stored at offset.</returns>
        /// <remarks><seealso href="https://redis.io/commands/getbit"/></remarks>
        public async Task<bool> StringGetBitAsync(RedisKey key, long offset, CommandFlags flags = CommandFlags.None)
        {
            return await _db.StringGetBitAsync(key, offset, flags);
        }

        /// <summary>
        /// Returns the substring of the string value stored at key, determined by the offsets start and end (both are inclusive).
        /// Negative offsets can be used in order to provide an offset starting from the end of the string.
        /// So -1 means the last character, -2 the penultimate and so forth.
        /// </summary>
        /// <param name="key">The key of the string.</param>
        /// <param name="start">The start index of the substring to get.</param>
        /// <param name="end">The end index of the substring to get.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The substring of the string value stored at key.</returns>
        /// <remarks><seealso href="https://redis.io/commands/getrange"/></remarks>
        public async Task<RedisValue> StringGetRangeAsync(RedisKey key, long start, long end, CommandFlags flags = CommandFlags.None)
        {
            //return (await _db.StringGetRangeAsync(key, start, end, flags)).ToObject<T>();
            return await _db.StringGetRangeAsync(key, start, end, flags);
        }

        /// <summary>
        /// Atomically sets key to value and returns the old value stored at key.
        /// </summary>
        /// <param name="key">The key of the string.</param>
        /// <param name="value">The value to replace the existing value with.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The old value stored at key, or nil when key did not exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/getset"/></remarks>
        public async Task<RedisValue> StringGetSetAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            //return (await _db.StringGetSetAsync(key, value.ToRedisValue<T>(), flags)).ToObject<T>();
            return await _db.StringGetSetAsync(key, value, flags);
        }

        /// <summary>
        /// Gets the value of <paramref name="key"/> and update its (relative) expiry.
        /// If the key does not exist, the result will be <see cref="RedisValue.Null"/>.
        /// </summary>
        /// <param name="key">The key of the string.</param>
        /// <param name="expiry">The expiry to set. <see langword="null"/> will remove expiry.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The value of key, or nil when key does not exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/getex"/></remarks>
        public async Task<RedisValue> StringGetSetExpiryAsync(RedisKey key, TimeSpan? expiry, CommandFlags flags = CommandFlags.None)
        {
            //return (await _db.StringGetSetExpiryAsync(key, expiry, flags)).ToObject<T>();
            return await _db.StringGetSetExpiryAsync(key, expiry, flags);
        }

        /// <summary>
        /// Gets the value of <paramref name="key"/> and update its (absolute) expiry.
        /// If the key does not exist, the result will be <see cref="RedisValue.Null"/>.
        /// </summary>
        /// <param name="key">The key of the string.</param>
        /// <param name="expiry">The exact date and time to expire at. <see cref="DateTime.MaxValue"/> will remove expiry.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The value of key, or nil when key does not exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/getex"/></remarks>
        public async Task<RedisValue> StringGetSetExpiryAsync(RedisKey key, DateTime expiry, CommandFlags flags = CommandFlags.None)
        {
            //return (await _db.StringGetSetExpiryAsync(key, expiry, flags)).ToObject<T>();
            return await _db.StringGetSetExpiryAsync(key, expiry, flags);
        }

        /// <summary>
        /// Get the value of key and delete the key.
        /// If the key does not exist the special value nil is returned.
        /// An error is returned if the value stored at key is not a string, because GET only handles string values.
        /// </summary>
        /// <param name="key">The key of the string.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The value of key, or nil when key does not exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/getdelete"/></remarks>
        public async Task<RedisValue> StringGetDeleteAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            //return (await _db.StringGetDeleteAsync(key, flags)).ToObject<T>();
            return await _db.StringGetDeleteAsync(key, flags);
        }

        /// <summary>
        /// Get the value of key.
        /// If the key does not exist the special value nil is returned.
        /// An error is returned if the value stored at key is not a string, because GET only handles string values.
        /// </summary>
        /// <param name="key">The key of the string.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The value of key and its expiry, or nil when key does not exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/get"/></remarks>
        public async Task<RedisValueWithExpiry> StringGetWithExpiryAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return await _db.StringGetWithExpiryAsync(key, flags);
        }

        /// <summary>
        /// Increments the number stored at key by increment.
        /// If the key does not exist, it is set to 0 before performing the operation.
        /// An error is returned if the key contains a value of the wrong type or contains a string that is not representable as integer.
        /// This operation is limited to 64 bit signed integers.
        /// </summary>
        /// <param name="key">The key of the string.</param>
        /// <param name="value">The amount to increment by (defaults to 1).</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The value of key after the increment.</returns>
        /// <remarks>
        /// <seealso href="https://redis.io/commands/incrby"/>,
        /// <seealso href="https://redis.io/commands/incr"/>
        /// </remarks>
        public async Task<long> StringIncrementAsync(RedisKey key, long value = 1, CommandFlags flags = CommandFlags.None)
        {
            return await _db.StringIncrementAsync(key, value, flags);
        }

        /// <summary>
        /// Increments the string representing a floating point number stored at key by the specified increment.
        /// If the key does not exist, it is set to 0 before performing the operation.
        /// The precision of the output is fixed at 17 digits after the decimal point regardless of the actual internal precision of the computation.
        /// </summary>
        /// <param name="key">The key of the string.</param>
        /// <param name="value">The amount to increment by (defaults to 1).</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The value of key after the increment.</returns>
        /// <remarks><seealso href="https://redis.io/commands/incrbyfloat"/></remarks>
        public async Task<double> StringIncrementAsync(RedisKey key, double value, CommandFlags flags = CommandFlags.None)
        {
            return await _db.StringIncrementAsync(key, value, flags);
        }

        /// <summary>
        /// Returns the length of the string value stored at key.
        /// </summary>
        /// <param name="key">The key of the string.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The length of the string at key, or 0 when key does not exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/strlen"/></remarks>
        public async Task<long> StringLengthAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return await _db.StringLengthAsync(key, flags);
        }

        /// <summary>
        /// Implements the longest common subsequence algorithm between the values at <paramref name="first"/> and <paramref name="second"/>,
        /// returning a string containing the common sequence.
        /// Note that this is different than the longest common string algorithm,
        /// since matching characters in the string does not need to be contiguous.
        /// </summary>
        /// <param name="first">The key of the first string.</param>
        /// <param name="second">The key of the second string.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>A string (sequence of characters) of the LCS match.</returns>
        /// <remarks><seealso href="https://redis.io/commands/lcs"/></remarks>
        public async Task<string?> StringLongestCommonSubsequenceAsync(RedisKey first, RedisKey second, CommandFlags flags = CommandFlags.None)
        {
            return await _db.StringLongestCommonSubsequenceAsync(first, second, flags);
        }

        /// <summary>
        /// Implements the longest common subsequence algorithm between the values at <paramref name="first"/> and <paramref name="second"/>,
        /// returning the legnth of the common sequence.
        /// Note that this is different than the longest common string algorithm,
        /// since matching characters in the string does not need to be contiguous.
        /// </summary>
        /// <param name="first">The key of the first string.</param>
        /// <param name="second">The key of the second string.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The length of the LCS match.</returns>
        /// <remarks><seealso href="https://redis.io/commands/lcs"/></remarks>
        public async Task<long> StringLongestCommonSubsequenceLengthAsync(RedisKey first, RedisKey second, CommandFlags flags = CommandFlags.None)
        {
            return await _db.StringLongestCommonSubsequenceLengthAsync(first, second, flags);
        }

        /// <summary>
        /// Implements the longest common subsequence algorithm between the values at <paramref name="first"/> and <paramref name="second"/>,
        /// returning a list of all common sequences.
        /// Note that this is different than the longest common string algorithm,
        /// since matching characters in the string does not need to be contiguous.
        /// </summary>
        /// <param name="first">The key of the first string.</param>
        /// <param name="second">The key of the second string.</param>
        /// <param name="minLength">Can be used to restrict the list of matches to the ones of a given minimum length.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The result of LCS algorithm, based on the given parameters.</returns>
        /// <remarks><seealso href="https://redis.io/commands/lcs"/></remarks>
        public async Task<LCSMatchResult> StringLongestCommonSubsequenceWithMatchesAsync(RedisKey first, RedisKey second, long minLength = 0, CommandFlags flags = CommandFlags.None)
        {
            return await _db.StringLongestCommonSubsequenceWithMatchesAsync(first, second, minLength, flags);
        }

        /// <inheritdoc cref="StringSetAsync(RedisKey, RedisValue, TimeSpan?, bool, When, CommandFlags)" />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public async Task<bool> StringSetAsync(RedisKey key, RedisValue value, TimeSpan? expiry, When when)
        {
            return await StringSetAsync(key, value, expiry, when);
        }

        /// <inheritdoc cref="StringSetAsync(RedisKey, RedisValue, TimeSpan?, bool, When, CommandFlags)" />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public async Task<bool> StringSetAsync(RedisKey key, RedisValue value, TimeSpan? expiry, When when, CommandFlags flags)
        {
            return await StringSetAsync(key, value, expiry, when, flags);
        }

        /// <summary>
        /// Set key to hold the string value. If key already holds a value, it is overwritten, regardless of its type.
        /// </summary>
        /// <param name="key">The key of the string.</param>
        /// <param name="value">The value to set.</param>
        /// <param name="expiry">The expiry to set.</param>
        /// <param name="keepTtl">Whether to maintain the existing key's TTL (KEEPTTL flag).</param>
        /// <param name="when">Which condition to set the value under (defaults to always).</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns><see langword="true"/> if the string was set, <see langword="false"/> otherwise.</returns>
        /// <remarks><seealso href="https://redis.io/commands/set"/></remarks>
        public async Task<bool> StringSetAsync(RedisKey key, RedisValue value, TimeSpan? expiry = null, bool keepTtl = false, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            //return await _db.StringSetAsync(key, value.ToRedisValue<T>(), expiry, keepTtl, when, flags);
            return await _db.StringSetAsync(key, value, expiry, keepTtl, when, flags);
        }

        /// <summary>
        /// Sets the given keys to their respective values.
        /// If <see cref="When.NotExists"/> is specified, this will not perform any operation at all even if just a single key already exists.
        /// </summary>
        /// <param name="values">The keys and values to set.</param>
        /// <param name="when">Which condition to set the value under (defaults to always).</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns><see langword="true"/> if the keys were set, <see langword="false"/> otherwise.</returns>
        /// <remarks>
        /// <seealso href="https://redis.io/commands/mset"/>,
        /// <seealso href="https://redis.io/commands/msetnx"/>
        /// </remarks>
        public async Task<bool> StringSetAsync(KeyValuePair<RedisKey, RedisValue>[] values, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            //KeyValuePair<RedisKey, RedisValue>[] redisValues = values.Select(x => new KeyValuePair<RedisKey, RedisValue>(x.Key, x.Value.ToRedisValue<T>())).ToArray();
            //return await _db.StringSetAsync(redisValues, when, flags);

            return await _db.StringSetAsync(values, when, flags);
        }

        /// <summary>
        /// Atomically sets key to value and returns the previous value (if any) stored at <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key of the string.</param>
        /// <param name="value">The value to set.</param>
        /// <param name="expiry">The expiry to set.</param>
        /// <param name="when">Which condition to set the value under (defaults to <see cref="When.Always"/>).</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The previous value stored at <paramref name="key"/>, or nil when key did not exist.</returns>
        /// <remarks>
        /// <para>This method uses the <c>SET</c> command with the <c>GET</c> option introduced in Redis 6.2.0 instead of the deprecated <c>GETSET</c> command.</para>
        /// <para><seealso href="https://redis.io/commands/set"/></para>
        /// </remarks>
        public async Task<RedisValue> StringSetAndGetAsync(RedisKey key, RedisValue value, TimeSpan? expiry, When when, CommandFlags flags)
        {
            //return (await _db.StringSetAndGetAsync(key, value.ToRedisValue<T>(), expiry, false, when, flags)).ToObject<T>();
            return await _db.StringSetAndGetAsync(key, value, expiry, false, when, flags);
        }

        /// <summary>
        /// Atomically sets key to value and returns the previous value (if any) stored at <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key of the string.</param>
        /// <param name="value">The value to set.</param>
        /// <param name="expiry">The expiry to set.</param>
        /// <param name="keepTtl">Whether to maintain the existing key's TTL (KEEPTTL flag).</param>
        /// <param name="when">Which condition to set the value under (defaults to <see cref="When.Always"/>).</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The previous value stored at <paramref name="key"/>, or nil when key did not exist.</returns>
        /// <remarks>This method uses the SET command with the GET option introduced in Redis 6.2.0 instead of the deprecated GETSET command.</remarks>
        /// <remarks><seealso href="https://redis.io/commands/set"/></remarks>
        public async Task<RedisValue> StringSetAndGetAsync(RedisKey key, RedisValue value, TimeSpan? expiry = null, bool keepTtl = false, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            //return (await _db.StringSetAndGetAsync(key, value.ToRedisValue<T>(), expiry, keepTtl, when, flags)).ToObject<T>();
            return await _db.StringSetAndGetAsync(key, value, expiry, keepTtl, when, flags);
        }

        /// <summary>
        /// Sets or clears the bit at offset in the string value stored at key.
        /// The bit is either set or cleared depending on value, which can be either 0 or 1.
        /// When key does not exist, a new string value is created.The string is grown to make sure it can hold a bit at offset.
        /// </summary>
        /// <param name="key">The key of the string.</param>
        /// <param name="offset">The offset in the string to set <paramref name="bit"/>.</param>
        /// <param name="bit">The bit value to set, true for 1, false for 0.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The original bit value stored at offset.</returns>
        /// <remarks><seealso href="https://redis.io/commands/setbit"/></remarks>
        public async Task<bool> StringSetBitAsync(RedisKey key, long offset, bool bit, CommandFlags flags = CommandFlags.None)
        {
            return await _db.StringSetBitAsync(key, offset, bit, flags);
        }

        /// <summary>
        /// Overwrites part of the string stored at key, starting at the specified offset, for the entire length of value.
        /// If the offset is larger than the current length of the string at key, the string is padded with zero-bytes to make offset fit.
        /// Non-existing keys are considered as empty strings, so this command will make sure it holds a string large enough to be able to set value at offset.
        /// </summary>
        /// <param name="key">The key of the string.</param>
        /// <param name="offset">The offset in the string to overwrite.</param>
        /// <param name="value">The value to overwrite with.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The length of the string after it was modified by the command.</returns>
        /// <remarks><seealso href="https://redis.io/commands/setrange"/></remarks>
        public async Task<RedisValue> StringSetRangeAsync(RedisKey key, long offset, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            //return (await _db.StringSetRangeAsync(key, offset, value.ToRedisValue<T>(), flags)).ToObject<T>();
            return await _db.StringSetRangeAsync(key, offset, value, flags);
        }

        /// <summary>
        /// 获取string的length
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<long> StringLengthAsync(RedisKey key) => await _db.StringLengthAsync(key);

        /// <summary>
        /// 添加一个key 或者覆盖key的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<bool> StringSetAsync(RedisKey key, RedisValue value) => await _db.StringSetAsync(key, value, null, false, When.Always);

        /// <summary>
        /// 添加一个key 并有过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiry">过期时间</param>
        /// <returns></returns>
        public async Task<bool> StringSetAsync(RedisKey key, RedisValue value, TimeSpan? expiry = null) => await _db.StringSetAsync(key, value, expiry, false, When.Always);

        /// <summary>
        /// 只有在键不存在的时候才设置
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<bool> StringSetNxAsync(RedisKey key, RedisValue value) => await _db.StringSetAsync(key, value, null, false, When.NotExists);

        /// <summary>
        /// Get the value of key. If the key does not exist the special value nil is returned.
        /// An error is returned if the value stored at key is not a string, because GET only handles string values.
        /// </summary>
        /// <param name="key">The key of the string.</param>
        /// <returns>The value of key, or nil when key does not exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/get"/></remarks>
        public async Task<T> StringGetAsync<T>(RedisKey key) => (await _db.StringGetAsync(key)).ToObject<T>();

        /// <summary>
        /// Increments the number stored at key by increment.
        /// If the key does not exist, it is set to 0 before performing the operation.
        /// An error is returned if the key contains a value of the wrong type or contains a string that is not representable as integer.
        /// This operation is limited to 64 bit signed integers.
        /// </summary>
        /// <param name="key">The key of the string.</param>
        /// <param name="value">The amount to increment by (defaults to 1).</param>
        /// <returns>The value of key after the increment.</returns>
        /// <remarks>
        /// <seealso href="https://redis.io/commands/incrby"/>,
        /// <seealso href="https://redis.io/commands/incr"/>
        /// </remarks>
        public async Task<long> StringIncrementAsync(RedisKey key, int value = 1) => await _db.StringIncrementAsync(key, value);

        /// <summary>
        /// Decrements the number stored at key by decrement.
        /// If the key does not exist, it is set to 0 before performing the operation.
        /// An error is returned if the key contains a value of the wrong type or contains a string that is not representable as integer.
        /// This operation is limited to 64 bit signed integers.
        /// </summary>
        /// <param name="key">The key of the string.</param>
        /// <param name="value">The amount to decrement by (defaults to 1).</param>
        /// <returns>The value of key after the decrement.</returns>
        /// <remarks>
        /// <seealso href="https://redis.io/commands/decrby"/>,
        /// <seealso href="https://redis.io/commands/decr"/>
        /// </remarks>
        public async Task<long> StringDecrementAsync(RedisKey key, int value = 1) => await _db.StringDecrementAsync(key, value);

        /// <summary>
        /// If key already exists and is a string, this command appends the value at the end of the string.
        /// If key does not exist it is created and set as an empty string, so APPEND will be similar to SET in this special case.
        /// </summary>
        /// <param name="key">The key of the string.</param>
        /// <param name="value">The value to append to the string.</param>
        /// <returns>The length of the string after the append operation.</returns>
        /// <remarks><seealso href="https://redis.io/commands/append"/></remarks>
        public async Task<long> StringAppendAsync(RedisKey key, string value) => await _db.StringAppendAsync(key, value);

        /// <summary>
        /// Returns the substring of the string value stored at key, determined by the offsets start and end (both are inclusive).
        /// Negative offsets can be used in order to provide an offset starting from the end of the string.
        /// So -1 means the last character, -2 the penultimate and so forth.
        /// </summary>
        /// <param name="key">The key of the string.</param>
        /// <param name="start">The start index of the substring to get.</param>
        /// <param name="end">The end index of the substring to get.</param>
        /// <returns>The substring of the string value stored at key.</returns>
        /// <remarks><seealso href="https://redis.io/commands/getrange"/></remarks>
        public async Task<string> StringGetRangeAsync(RedisKey key, int start, int end)
        {
            var value = await _db.StringGetRangeAsync(key, start, end);
            return value.ToString();
        }

        #endregion

        #region Set

        /// <summary>
        /// Add the specified member to the set stored at key.
        /// Specified members that are already a member of this set are ignored.
        /// If key does not exist, a new set is created before adding the specified members.
        /// </summary>
        /// <param name="key">The key of the set.</param>
        /// <param name="value">The value to add to the set.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns><see langword="true"/> if the specified member was not already present in the set, else <see langword="false"/>.</returns>
        /// <remarks><seealso href="https://redis.io/commands/sadd"/></remarks>
        public bool SetAdd(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            return _db.SetAdd(key, value, flags);
        }

        /// <summary>
        /// Add the specified members to the set stored at key.
        /// Specified members that are already a member of this set are ignored.
        /// If key does not exist, a new set is created before adding the specified members.
        /// </summary>
        /// <param name="key">The key of the set.</param>
        /// <param name="values">The values to add to the set.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The number of elements that were added to the set, not including all the elements already present into the set.</returns>
        /// <remarks><seealso href="https://redis.io/commands/sadd"/></remarks>
        public long SetAdd(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
            return _db.SetAdd(key, values, flags);
        }

        /// <summary>
        /// Returns the members of the set resulting from the specified operation against the given sets.
        /// </summary>
        /// <param name="operation">The operation to perform.</param>
        /// <param name="first">The key of the first set.</param>
        /// <param name="second">The key of the second set.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>List with members of the resulting set.</returns>
        /// <remarks>
        /// <seealso href="https://redis.io/commands/sunion"/>,
        /// <seealso href="https://redis.io/commands/sinter"/>,
        /// <seealso href="https://redis.io/commands/sdiff"/>
        /// </remarks>
        public RedisValue[] SetCombine(SetOperation operation, RedisKey first, RedisKey second, CommandFlags flags = CommandFlags.None)
        {
            return _db.SetCombine(operation, first, second, flags);
        }

        /// <summary>
        /// Returns the members of the set resulting from the specified operation against the given sets.
        /// </summary>
        /// <param name="operation">The operation to perform.</param>
        /// <param name="keys">The keys of the sets to operate on.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>List with members of the resulting set.</returns>
        /// <remarks>
        /// <seealso href="https://redis.io/commands/sunion"/>,
        /// <seealso href="https://redis.io/commands/sinter"/>,
        /// <seealso href="https://redis.io/commands/sdiff"/>
        /// </remarks>
        public RedisValue[] SetCombine(SetOperation operation, RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            return _db.SetCombine(operation, keys, flags);
        }

        /// <summary>
        /// This command is equal to SetCombine, but instead of returning the resulting set, it is stored in destination.
        /// If destination already exists, it is overwritten.
        /// </summary>
        /// <param name="operation">The operation to perform.</param>
        /// <param name="destination">The key of the destination set.</param>
        /// <param name="first">The key of the first set.</param>
        /// <param name="second">The key of the second set.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The number of elements in the resulting set.</returns>
        /// <remarks>
        /// <seealso href="https://redis.io/commands/sunionstore"/>,
        /// <seealso href="https://redis.io/commands/sinterstore"/>,
        /// <seealso href="https://redis.io/commands/sdiffstore"/>
        /// </remarks>
        public long SetCombineAndStore(SetOperation operation, RedisKey destination, RedisKey first, RedisKey second, CommandFlags flags = CommandFlags.None)
        {
            return _db.SetCombineAndStore(operation, destination, first, second, flags);
        }

        /// <summary>
        /// This command is equal to SetCombine, but instead of returning the resulting set, it is stored in destination.
        /// If destination already exists, it is overwritten.
        /// </summary>
        /// <param name="operation">The operation to perform.</param>
        /// <param name="destination">The key of the destination set.</param>
        /// <param name="keys">The keys of the sets to operate on.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The number of elements in the resulting set.</returns>
        /// <remarks>
        /// <seealso href="https://redis.io/commands/sunionstore"/>,
        /// <seealso href="https://redis.io/commands/sinterstore"/>,
        /// <seealso href="https://redis.io/commands/sdiffstore"/>
        /// </remarks>
        public long SetCombineAndStore(SetOperation operation, RedisKey destination, RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            return _db.SetCombineAndStore(operation, destination, keys, flags);
        }

        /// <summary>
        /// Returns whether <paramref name="value"/> is a member of the set stored at <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key of the set.</param>
        /// <param name="value">The value to check for.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>
        /// <see langword="true"/> if the element is a member of the set.
        /// <see langword="false"/> if the element is not a member of the set, or if key does not exist.
        /// </returns>
        /// <remarks><seealso href="https://redis.io/commands/sismember"/></remarks>
        public bool SetContains(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            return _db.SetContains(key, value, flags);
        }

        /// <summary>
        /// Returns whether each of <paramref name="values"/> is a member of the set stored at <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key of the set.</param>
        /// <param name="values">The members to check for.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>
        /// <see langword="true"/> if the element is a member of the set.
        /// <see langword="false"/> if the element is not a member of the set, or if key does not exist.
        /// </returns>
        /// <remarks><seealso href="https://redis.io/commands/smismember"/></remarks>
        public bool[] SetContains(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
            return _db.SetContains(key, values, flags);
        }

        /// <summary>
        ///   <para>
        ///     Returns the set cardinality (number of elements) of the intersection between the sets stored at the given <paramref name="keys"/>.
        ///   </para>
        ///   <para>
        ///     If the intersection cardinality reaches <paramref name="limit"/> partway through the computation,
        ///     the algorithm will exit and yield <paramref name="limit"/> as the cardinality.
        ///   </para>
        /// </summary>
        /// <param name="keys">The keys of the sets.</param>
        /// <param name="limit">The number of elements to check (defaults to 0 and means unlimited).</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The cardinality (number of elements) of the set, or 0 if key does not exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/scard"/></remarks>
        public long SetIntersectionLength(RedisKey[] keys, long limit = 0, CommandFlags flags = CommandFlags.None)
        {
            return _db.SetIntersectionLength(keys, limit, flags);
        }

        /// <summary>
        /// Returns the set cardinality (number of elements) of the set stored at key.
        /// </summary>
        /// <param name="key">The key of the set.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The cardinality (number of elements) of the set, or 0 if key does not exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/scard"/></remarks>
        public long SetLength(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _db.SetLength(key, flags);
        }

        /// <summary>
        /// Returns all the members of the set value stored at key.
        /// </summary>
        /// <param name="key">The key of the set.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>All elements of the set.</returns>
        /// <remarks><seealso href="https://redis.io/commands/smembers"/></remarks>
        public RedisValue[] SetMembers(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _db.SetMembers(key, flags);
        }

        /// <summary>
        /// Move member from the set at source to the set at destination.
        /// This operation is atomic. In every given moment the element will appear to be a member of source or destination for other clients.
        /// When the specified element already exists in the destination set, it is only removed from the source set.
        /// </summary>
        /// <param name="source">The key of the source set.</param>
        /// <param name="destination">The key of the destination set.</param>
        /// <param name="value">The value to move.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>
        /// <see langword="true"/> if the element is moved.
        /// <see langword="false"/> if the element is not a member of source and no operation was performed.
        /// </returns>
        /// <remarks><seealso href="https://redis.io/commands/smove"/></remarks>
        public bool SetMove(RedisKey source, RedisKey destination, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            return _db.SetMove(source, destination, value, flags);
        }

        /// <summary>
        /// Removes and returns a random element from the set value stored at key.
        /// </summary>
        /// <param name="key">The key of the set.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The removed element, or nil when key does not exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/spop"/></remarks>
        public RedisValue SetPop(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _db.SetPop(key, flags);
        }

        /// <summary>
        /// Removes and returns the specified number of random elements from the set value stored at key.
        /// </summary>
        /// <param name="key">The key of the set.</param>
        /// <param name="count">The number of elements to return.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>An array of elements, or an empty array when key does not exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/spop"/></remarks>
        public RedisValue[] SetPop(RedisKey key, long count, CommandFlags flags = CommandFlags.None)
        {
            return _db.SetPop(key, count, flags);
        }

        /// <summary>
        /// Return a random element from the set value stored at <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key of the set.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The randomly selected element, or <see cref="RedisValue.Null"/> when <paramref name="key"/> does not exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/srandmember"/></remarks>
        public RedisValue SetRandomMember(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _db.SetRandomMember(key, flags);
        }

        /// <summary>
        /// Return an array of count distinct elements if count is positive.
        /// If called with a negative count the behavior changes and the command is allowed to return the same element multiple times.
        /// In this case the number of returned elements is the absolute value of the specified count.
        /// </summary>
        /// <param name="key">The key of the set.</param>
        /// <param name="count">The count of members to get.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>An array of elements, or an empty array when <paramref name="key"/> does not exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/srandmember"/></remarks>
        public RedisValue[] SetRandomMembers(RedisKey key, long count, CommandFlags flags = CommandFlags.None)
        {
            return _db.SetRandomMembers(key, count, flags);
        }

        /// <summary>
        /// Remove the specified member from the set stored at key.
        /// Specified members that are not a member of this set are ignored.
        /// </summary>
        /// <param name="key">The key of the set.</param>
        /// <param name="value">The value to remove.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns><see langword="true"/> if the specified member was already present in the set, <see langword="false"/> otherwise.</returns>
        /// <remarks><seealso href="https://redis.io/commands/srem"/></remarks>
        public bool SetRemove(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            return _db.SetRemove(key, value, flags);
        }

        /// <summary>
        /// Remove the specified members from the set stored at key.
        /// Specified members that are not a member of this set are ignored.
        /// </summary>
        /// <param name="key">The key of the set.</param>
        /// <param name="values">The values to remove.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The number of members that were removed from the set, not including non existing members.</returns>
        /// <remarks><seealso href="https://redis.io/commands/srem"/></remarks>
        public long SetRemove(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
            return _db.SetRemove(key, values, flags);
        }

        /// <summary>
        /// Add the specified member to the set stored at key.
        /// Specified members that are already a member of this set are ignored.
        /// If key does not exist, a new set is created before adding the specified members.
        /// </summary>
        /// <param name="key">The key of the set.</param>
        /// <param name="value">The value to add to the set.</param>
        /// <returns><see langword="true"/> if the specified member was not already present in the set, else <see langword="false"/>.</returns>
        /// <remarks><seealso href="https://redis.io/commands/sadd"/></remarks>
        public bool SetAdd<T>(string key, T value) => _db.SetAdd(key, value.ToRedisValue());

        /// <summary>
        /// Remove the specified members from the set stored at key.
        /// Specified members that are not a member of this set are ignored.
        /// </summary>
        /// <param name="key">The key of the set.</param>
        /// <param name="values">The values to remove.</param>
        /// <returns>The number of members that were removed from the set, not including non existing members.</returns>
        /// <remarks><seealso href="https://redis.io/commands/srem"/></remarks>
        public long SetRemove<T>(string key, IEnumerable<T> values) => _db.SetRemove(key, values.ToRedisValues());

        /// <summary>
        /// Returns all the members of the set value stored at key.
        /// </summary>
        /// <param name="key">The key of the set.</param>
        /// <returns>All elements of the set.</returns>
        /// <remarks><seealso href="https://redis.io/commands/smembers"/></remarks>
        public IEnumerable<T> SetMembers<T>(string key) where T : class => (_db.SetMembers(key)).ToObjects<T>();

        #endregion

        #region Set Async

        /// <summary>
        /// Add the specified member to the set stored at key.
        /// Specified members that are already a member of this set are ignored.
        /// If key does not exist, a new set is created before adding the specified members.
        /// </summary>
        /// <param name="key">The key of the set.</param>
        /// <param name="value">The value to add to the set.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns><see langword="true"/> if the specified member was not already present in the set, else <see langword="false"/>.</returns>
        /// <remarks><seealso href="https://redis.io/commands/sadd"/></remarks>
        public async Task<bool> SetAddAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            return await _db.SetAddAsync(key, value, flags);
        }

        /// <summary>
        /// Add the specified members to the set stored at key.
        /// Specified members that are already a member of this set are ignored.
        /// If key does not exist, a new set is created before adding the specified members.
        /// </summary>
        /// <param name="key">The key of the set.</param>
        /// <param name="values">The values to add to the set.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The number of elements that were added to the set, not including all the elements already present into the set.</returns>
        /// <remarks><seealso href="https://redis.io/commands/sadd"/></remarks>
        public async Task<long> SetAddAsync(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
            return await _db.SetAddAsync(key, values, flags);
        }

        /// <summary>
        /// Returns the members of the set resulting from the specified operation against the given sets.
        /// </summary>
        /// <param name="operation">The operation to perform.</param>
        /// <param name="first">The key of the first set.</param>
        /// <param name="second">The key of the second set.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>List with members of the resulting set.</returns>
        /// <remarks>
        /// <seealso href="https://redis.io/commands/sunion"/>,
        /// <seealso href="https://redis.io/commands/sinter"/>,
        /// <seealso href="https://redis.io/commands/sdiff"/>
        /// </remarks>
        public async Task<RedisValue[]> SetCombineAsync(SetOperation operation, RedisKey first, RedisKey second, CommandFlags flags = CommandFlags.None)
        {
            return await _db.SetCombineAsync(operation, first, second, flags);
        }

        /// <summary>
        /// Returns the members of the set resulting from the specified operation against the given sets.
        /// </summary>
        /// <param name="operation">The operation to perform.</param>
        /// <param name="keys">The keys of the sets to operate on.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>List with members of the resulting set.</returns>
        /// <remarks>
        /// <seealso href="https://redis.io/commands/sunion"/>,
        /// <seealso href="https://redis.io/commands/sinter"/>,
        /// <seealso href="https://redis.io/commands/sdiff"/>
        /// </remarks>
        public async Task<RedisValue[]> SetCombineAsync(SetOperation operation, RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            return await _db.SetCombineAsync(operation, keys, flags);
        }

        /// <summary>
        /// This command is equal to SetCombine, but instead of returning the resulting set, it is stored in destination.
        /// If destination already exists, it is overwritten.
        /// </summary>
        /// <param name="operation">The operation to perform.</param>
        /// <param name="destination">The key of the destination set.</param>
        /// <param name="first">The key of the first set.</param>
        /// <param name="second">The key of the second set.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The number of elements in the resulting set.</returns>
        /// <remarks>
        /// <seealso href="https://redis.io/commands/sunionstore"/>,
        /// <seealso href="https://redis.io/commands/sinterstore"/>,
        /// <seealso href="https://redis.io/commands/sdiffstore"/>
        /// </remarks>
        public async Task<long> SetCombineAndStoreAsync(SetOperation operation, RedisKey destination, RedisKey first, RedisKey second, CommandFlags flags = CommandFlags.None)
        {
            return await _db.SetCombineAndStoreAsync(operation, destination, first, second, flags);
        }

        /// <summary>
        /// This command is equal to SetCombine, but instead of returning the resulting set, it is stored in destination.
        /// If destination already exists, it is overwritten.
        /// </summary>
        /// <param name="operation">The operation to perform.</param>
        /// <param name="destination">The key of the destination set.</param>
        /// <param name="keys">The keys of the sets to operate on.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The number of elements in the resulting set.</returns>
        /// <remarks>
        /// <seealso href="https://redis.io/commands/sunionstore"/>,
        /// <seealso href="https://redis.io/commands/sinterstore"/>,
        /// <seealso href="https://redis.io/commands/sdiffstore"/>
        /// </remarks>
        public async Task<long> SetCombineAndStoreAsync(SetOperation operation, RedisKey destination, RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            return await _db.SetCombineAndStoreAsync(operation, destination, keys, flags);
        }

        /// <summary>
        /// Returns whether <paramref name="value"/> is a member of the set stored at <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key of the set.</param>
        /// <param name="value">The value to check for.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>
        /// <see langword="true"/> if the element is a member of the set.
        /// <see langword="false"/> if the element is not a member of the set, or if key does not exist.
        /// </returns>
        /// <remarks><seealso href="https://redis.io/commands/sismember"/></remarks>
        public async Task<bool> SetContainsAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            return await _db.SetContainsAsync(key, value, flags);
        }

        /// <summary>
        /// Returns whether each of <paramref name="values"/> is a member of the set stored at <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key of the set.</param>
        /// <param name="values">The members to check for.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>
        /// <see langword="true"/> if the element is a member of the set.
        /// <see langword="false"/> if the element is not a member of the set, or if key does not exist.
        /// </returns>
        /// <remarks><seealso href="https://redis.io/commands/smismember"/></remarks>
        public async Task<bool[]> SetContainsAsync(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
            return await _db.SetContainsAsync(key, values, flags);
        }

        /// <summary>
        ///   <para>
        ///     Returns the set cardinality (number of elements) of the intersection between the sets stored at the given <paramref name="keys"/>.
        ///   </para>
        ///   <para>
        ///     If the intersection cardinality reaches <paramref name="limit"/> partway through the computation,
        ///     the algorithm will exit and yield <paramref name="limit"/> as the cardinality.
        ///   </para>
        /// </summary>
        /// <param name="keys">The keys of the sets.</param>
        /// <param name="limit">The number of elements to check (defaults to 0 and means unlimited).</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The cardinality (number of elements) of the set, or 0 if key does not exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/scard"/></remarks>
        public async Task<long> SetIntersectionLengthAsync(RedisKey[] keys, long limit = 0, CommandFlags flags = CommandFlags.None)
        {
            return await _db.SetIntersectionLengthAsync(keys, limit, flags);
        }

        /// <summary>
        /// Returns the set cardinality (number of elements) of the set stored at key.
        /// </summary>
        /// <param name="key">The key of the set.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The cardinality (number of elements) of the set, or 0 if key does not exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/scard"/></remarks>
        public async Task<long> SetLengthAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return await _db.SetLengthAsync(key, flags);
        }

        /// <summary>
        /// Returns all the members of the set value stored at key.
        /// </summary>
        /// <param name="key">The key of the set.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>All elements of the set.</returns>
        /// <remarks><seealso href="https://redis.io/commands/smembers"/></remarks>
        public async Task<RedisValue[]> SetMembersAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return await _db.SetMembersAsync(key, flags);
        }

        /// <summary>
        /// Move member from the set at source to the set at destination.
        /// This operation is atomic. In every given moment the element will appear to be a member of source or destination for other clients.
        /// When the specified element already exists in the destination set, it is only removed from the source set.
        /// </summary>
        /// <param name="source">The key of the source set.</param>
        /// <param name="destination">The key of the destination set.</param>
        /// <param name="value">The value to move.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>
        /// <see langword="true"/> if the element is moved.
        /// <see langword="false"/> if the element is not a member of source and no operation was performed.
        /// </returns>
        /// <remarks><seealso href="https://redis.io/commands/smove"/></remarks>
        public async Task<bool> SetMoveAsync(RedisKey source, RedisKey destination, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            return await _db.SetMoveAsync(source, destination, value, flags);
        }

        /// <summary>
        /// Removes and returns a random element from the set value stored at key.
        /// </summary>
        /// <param name="key">The key of the set.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The removed element, or nil when key does not exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/spop"/></remarks>
        public async Task<RedisValue> SetPopAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return await _db.SetPopAsync(key, flags);
        }

        /// <summary>
        /// Removes and returns the specified number of random elements from the set value stored at key.
        /// </summary>
        /// <param name="key">The key of the set.</param>
        /// <param name="count">The number of elements to return.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>An array of elements, or an empty array when key does not exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/spop"/></remarks>
        public async Task<RedisValue[]> SetPopAsync(RedisKey key, long count, CommandFlags flags = CommandFlags.None)
        {
            return await _db.SetPopAsync(key, count, flags);
        }

        /// <summary>
        /// Return a random element from the set value stored at <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key of the set.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The randomly selected element, or <see cref="RedisValue.Null"/> when <paramref name="key"/> does not exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/srandmember"/></remarks>
        public async Task<RedisValue> SetRandomMemberAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return await _db.SetRandomMemberAsync(key, flags);
        }

        /// <summary>
        /// Return an array of count distinct elements if count is positive.
        /// If called with a negative count the behavior changes and the command is allowed to return the same element multiple times.
        /// In this case the number of returned elements is the absolute value of the specified count.
        /// </summary>
        /// <param name="key">The key of the set.</param>
        /// <param name="count">The count of members to get.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>An array of elements, or an empty array when <paramref name="key"/> does not exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/srandmember"/></remarks>
        public async Task<RedisValue[]> SetRandomMembersAsync(RedisKey key, long count, CommandFlags flags = CommandFlags.None)
        {
            return await _db.SetRandomMembersAsync(key, count, flags);
        }

        /// <summary>
        /// Remove the specified member from the set stored at key.
        /// Specified members that are not a member of this set are ignored.
        /// </summary>
        /// <param name="key">The key of the set.</param>
        /// <param name="value">The value to remove.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns><see langword="true"/> if the specified member was already present in the set, <see langword="false"/> otherwise.</returns>
        /// <remarks><seealso href="https://redis.io/commands/srem"/></remarks>
        public async Task<bool> SetRemoveAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            return await _db.SetRemoveAsync(key, value, flags);
        }

        /// <summary>
        /// Remove the specified members from the set stored at key.
        /// Specified members that are not a member of this set are ignored.
        /// </summary>
        /// <param name="key">The key of the set.</param>
        /// <param name="values">The values to remove.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The number of members that were removed from the set, not including non existing members.</returns>
        /// <remarks><seealso href="https://redis.io/commands/srem"/></remarks>
        public async Task<long> SetRemoveAsync(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
            return await _db.SetRemoveAsync(key, values, flags);
        }

        /// <summary>
        /// Add the specified member to the set stored at key.
        /// Specified members that are already a member of this set are ignored.
        /// If key does not exist, a new set is created before adding the specified members.
        /// </summary>
        /// <param name="key">The key of the set.</param>
        /// <param name="value">The value to add to the set.</param>
        /// <returns><see langword="true"/> if the specified member was not already present in the set, else <see langword="false"/>.</returns>
        /// <remarks><seealso href="https://redis.io/commands/sadd"/></remarks>
        public async Task<bool> SetAddAsync<T>(string key, T value) => await _db.SetAddAsync(key, value.ToRedisValue());

        /// <summary>
        /// Remove the specified members from the set stored at key.
        /// Specified members that are not a member of this set are ignored.
        /// </summary>
        /// <param name="key">The key of the set.</param>
        /// <param name="values">The values to remove.</param>
        /// <returns>The number of members that were removed from the set, not including non existing members.</returns>
        /// <remarks><seealso href="https://redis.io/commands/srem"/></remarks>
        public async Task<long> SetRemoveAsync<T>(string key, IEnumerable<T> values) => await _db.SetRemoveAsync(key, values.ToRedisValues());

        /// <summary>
        /// Returns all the members of the set value stored at key.
        /// </summary>
        /// <param name="key">The key of the set.</param>
        /// <returns>All elements of the set.</returns>
        /// <remarks><seealso href="https://redis.io/commands/smembers"/></remarks>
        public async Task<IEnumerable<T>> SetMembersAsync<T>(string key) where T : class => (await _db.SetMembersAsync(key)).ToObjects<T>();

        #endregion

        #region SortedSet 

        /// <summary>
        /// Sorts a list, set or sorted set (numerically or alphabetically, ascending by default).
        /// By default, the elements themselves are compared, but the values can also be used to perform external key-lookups using the <c>by</c> parameter.
        /// By default, the elements themselves are returned, but external key-lookups (one or many) can be performed instead by specifying
        /// the <c>get</c> parameter (note that <c>#</c> specifies the element itself, when used in <c>get</c>).
        /// Referring to the <a href="https://redis.io/commands/sort">redis SORT documentation </a> for examples is recommended.
        /// When used in hashes, <c>by</c> and <c>get</c> can be used to specify fields using <c>-&gt;</c> notation (again, refer to redis documentation).
        /// Uses <a href="https://redis.io/commands/sort_ro">SORT_RO</a> when possible.
        /// </summary>
        /// <param name="key">The key of the list, set, or sorted set.</param>
        /// <param name="skip">How many entries to skip on the return.</param>
        /// <param name="take">How many entries to take on the return.</param>
        /// <param name="order">The ascending or descending order (defaults to ascending).</param>
        /// <param name="sortType">The sorting method (defaults to numeric).</param>
        /// <param name="by">The key pattern to sort by, if any. e.g. ExternalKey_* would sort by ExternalKey_{listvalue} as a lookup.</param>
        /// <param name="get">The key pattern to sort by, if any e.g. ExternalKey_* would return the value of ExternalKey_{listvalue} for each entry.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The sorted elements, or the external values if <c>get</c> is specified.</returns>
        /// <remarks>
        /// <seealso href="https://redis.io/commands/sort"/>,
        /// <seealso href="https://redis.io/commands/sort_ro"/>
        /// </remarks>
        public RedisValue[] Sort(RedisKey key, long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric, RedisValue by = default, RedisValue[]? get = null, CommandFlags flags = CommandFlags.None)
        {
            return _db.Sort(key, skip, take, order, sortType, by, get, flags);
        }

        /// <summary>
        /// Sorts a list, set or sorted set (numerically or alphabetically, ascending by default).
        /// By default, the elements themselves are compared, but the values can also be used to perform external key-lookups using the <c>by</c> parameter.
        /// By default, the elements themselves are returned, but external key-lookups (one or many) can be performed instead by specifying
        /// the <c>get</c> parameter (note that <c>#</c> specifies the element itself, when used in <c>get</c>).
        /// Referring to the <a href="https://redis.io/commands/sort">redis SORT documentation</a> for examples is recommended.
        /// When used in hashes, <c>by</c> and <c>get</c> can be used to specify fields using <c>-&gt;</c> notation (again, refer to redis documentation).
        /// </summary>
        /// <param name="destination">The destination key to store results in.</param>
        /// <param name="key">The key of the list, set, or sorted set.</param>
        /// <param name="skip">How many entries to skip on the return.</param>
        /// <param name="take">How many entries to take on the return.</param>
        /// <param name="order">The ascending or descending order (defaults to ascending).</param>
        /// <param name="sortType">The sorting method (defaults to numeric).</param>
        /// <param name="by">The key pattern to sort by, if any. e.g. ExternalKey_* would sort by ExternalKey_{listvalue} as a lookup.</param>
        /// <param name="get">The key pattern to sort by, if any e.g. ExternalKey_* would return the value of ExternalKey_{listvalue} for each entry.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The number of elements stored in the new list.</returns>
        /// <remarks><seealso href="https://redis.io/commands/sort"/></remarks>
        public long SortAndStore(RedisKey destination, RedisKey key, long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric, RedisValue by = default, RedisValue[]? get = null, CommandFlags flags = CommandFlags.None)
        {
            return _db.SortAndStore(destination, key, skip, take, order, sortType, by, get, flags);
        }

        /// <inheritdoc cref="SortedSetAdd(RedisKey, RedisValue, double, SortedSetWhen, CommandFlags)" />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool SortedSetAdd(RedisKey key, RedisValue member, double score, CommandFlags flags)
        {
            return _db.SortedSetAdd(key, member, score, flags);
        }

        /// <inheritdoc cref="SortedSetAdd(RedisKey, RedisValue, double, SortedSetWhen, CommandFlags)" />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool SortedSetAdd(RedisKey key, RedisValue member, double score, When when, CommandFlags flags = CommandFlags.None)
        {
            return _db.SortedSetAdd(key, member, score, when, flags);
        }

        /// <summary>
        /// Adds the specified member with the specified score to the sorted set stored at key.
        /// If the specified member is already a member of the sorted set, the score is updated and the element reinserted at the right position to ensure the correct ordering.
        /// </summary>
        /// <param name="key">The key of the sorted set.</param>
        /// <param name="member">The member to add to the sorted set.</param>
        /// <param name="score">The score for the member to add to the sorted set.</param>
        /// <param name="when">What conditions to add the element under (defaults to always).</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns><see langword="true"/> if the value was added. <see langword="false"/> if it already existed (the score is still updated).</returns>
        /// <remarks><seealso href="https://redis.io/commands/zadd"/></remarks>
        public bool SortedSetAdd(RedisKey key, RedisValue member, double score, SortedSetWhen when = SortedSetWhen.Always, CommandFlags flags = CommandFlags.None)
        {
            return _db.SortedSetAdd(key, member, score, when, flags);
        }

        /// <inheritdoc cref="SortedSetAdd(RedisKey, RedisValue, double, SortedSetWhen, CommandFlags)" />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public long SortedSetAdd(RedisKey key, SortedSetEntry[] values, CommandFlags flags)
        {
            return _db.SortedSetAdd(key, values, flags);
        }

        /// <inheritdoc cref="SortedSetAdd(RedisKey, RedisValue, double, SortedSetWhen, CommandFlags)" />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public long SortedSetAdd(RedisKey key, SortedSetEntry[] values, When when, CommandFlags flags = CommandFlags.None)
        {
            return _db.SortedSetAdd(key, values, when, flags);
        }

        /// <summary>
        /// Adds all the specified members with the specified scores to the sorted set stored at key.
        /// If a specified member is already a member of the sorted set, the score is updated and the element reinserted at the right position to ensure the correct ordering.
        /// </summary>
        /// <param name="key">The key of the sorted set.</param>
        /// <param name="values">The members and values to add to the sorted set.</param>
        /// <param name="when">What conditions to add the element under (defaults to always).</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The number of elements added to the sorted sets, not including elements already existing for which the score was updated.</returns>
        /// <remarks><seealso href="https://redis.io/commands/zadd"/></remarks>
        public long SortedSetAdd(RedisKey key, SortedSetEntry[] values, SortedSetWhen when = SortedSetWhen.Always, CommandFlags flags = CommandFlags.None)
        {
            return _db.SortedSetAdd(key, values, when, flags);
        }

        /// <summary>
        /// Computes a set operation for multiple sorted sets (optionally using per-set <paramref name="weights"/>),
        /// optionally performing a specific aggregation (defaults to <see cref="Aggregate.Sum"/>).
        /// <see cref="SetOperation.Difference"/> cannot be used with weights or aggregation.
        /// </summary>
        /// <param name="operation">The operation to perform.</param>
        /// <param name="keys">The keys of the sorted sets.</param>
        /// <param name="weights">The optional weights per set that correspond to <paramref name="keys"/>.</param>
        /// <param name="aggregate">The aggregation method (defaults to <see cref="Aggregate.Sum"/>).</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The resulting sorted set.</returns>
        /// <remarks>
        /// <seealso href="https://redis.io/commands/zunion"/>,
        /// <seealso href="https://redis.io/commands/zinter"/>,
        /// <seealso href="https://redis.io/commands/zdiff"/>
        /// </remarks>
        public RedisValue[] SortedSetCombine(SetOperation operation, RedisKey[] keys, double[]? weights = null, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None)
        {
            return _db.SortedSetCombine(operation, keys, weights, aggregate, flags);
        }

        /// <summary>
        /// Computes a set operation for multiple sorted sets (optionally using per-set <paramref name="weights"/>),
        /// optionally performing a specific aggregation (defaults to <see cref="Aggregate.Sum"/>).
        /// <see cref="SetOperation.Difference"/> cannot be used with weights or aggregation.
        /// </summary>
        /// <param name="operation">The operation to perform.</param>
        /// <param name="keys">The keys of the sorted sets.</param>
        /// <param name="weights">The optional weights per set that correspond to <paramref name="keys"/>.</param>
        /// <param name="aggregate">The aggregation method (defaults to <see cref="Aggregate.Sum"/>).</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The resulting sorted set with scores.</returns>
        /// <remarks>
        /// <seealso href="https://redis.io/commands/zunion"/>,
        /// <seealso href="https://redis.io/commands/zinter"/>,
        /// <seealso href="https://redis.io/commands/zdiff"/>
        /// </remarks>
        public SortedSetEntry[] SortedSetCombineWithScores(SetOperation operation, RedisKey[] keys, double[]? weights = null, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None)
        {
            return _db.SortedSetCombineWithScores(operation, keys, weights, aggregate, flags);
        }

        /// <summary>
        /// Computes a set operation over two sorted sets, and stores the result in destination, optionally performing
        /// a specific aggregation (defaults to sum).
        /// <see cref="SetOperation.Difference"/> cannot be used with aggregation.
        /// </summary>
        /// <param name="operation">The operation to perform.</param>
        /// <param name="destination">The key to store the results in.</param>
        /// <param name="first">The key of the first sorted set.</param>
        /// <param name="second">The key of the second sorted set.</param>
        /// <param name="aggregate">The aggregation method (defaults to sum).</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The number of elements in the resulting sorted set at destination.</returns>
        /// <remarks>
        /// <seealso href="https://redis.io/commands/zunionstore"/>,
        /// <seealso href="https://redis.io/commands/zinterstore"/>,
        /// <seealso href="https://redis.io/commands/zdiffstore"/>
        /// </remarks>
        public long SortedSetCombineAndStore(SetOperation operation, RedisKey destination, RedisKey first, RedisKey second, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None)
        {
            return _db.SortedSetCombineAndStore(operation, destination, first, second, aggregate, flags);
        }

        /// <summary>
        /// Computes a set operation over multiple sorted sets (optionally using per-set weights), and stores the result in destination, optionally performing
        /// a specific aggregation (defaults to sum).
        /// <see cref="SetOperation.Difference"/> cannot be used with aggregation.
        /// </summary>
        /// <param name="operation">The operation to perform.</param>
        /// <param name="destination">The key to store the results in.</param>
        /// <param name="keys">The keys of the sorted sets.</param>
        /// <param name="weights">The optional weights per set that correspond to <paramref name="keys"/>.</param>
        /// <param name="aggregate">The aggregation method (defaults to sum).</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The number of elements in the resulting sorted set at destination.</returns>
        /// <remarks>
        /// <seealso href="https://redis.io/commands/zunionstore"/>,
        /// <seealso href="https://redis.io/commands/zinterstore"/>,
        /// <seealso href="https://redis.io/commands/zdiffstore"/>
        /// </remarks>
        public long SortedSetCombineAndStore(SetOperation operation, RedisKey destination, RedisKey[] keys, double[]? weights = null, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None)
        {
            return _db.SortedSetCombineAndStore(operation, destination, keys, weights, aggregate, flags);
        }

        /// <summary>
        /// Decrements the score of member in the sorted set stored at key by decrement.
        /// If member does not exist in the sorted set, it is added with -decrement as its score (as if its previous score was 0.0).
        /// </summary>
        /// <param name="key">The key of the sorted set.</param>
        /// <param name="member">The member to decrement.</param>
        /// <param name="value">The amount to decrement by.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The new score of member.</returns>
        /// <remarks><seealso href="https://redis.io/commands/zincrby"/></remarks>
        public double SortedSetDecrement(RedisKey key, RedisValue member, double value, CommandFlags flags = CommandFlags.None)
        {
            return _db.SortedSetDecrement(key, member, value, flags);
        }

        /// <summary>
        /// Increments the score of member in the sorted set stored at key by increment. If member does not exist in the sorted set, it is added with increment as its score (as if its previous score was 0.0).
        /// </summary>
        /// <param name="key">The key of the sorted set.</param>
        /// <param name="member">The member to increment.</param>
        /// <param name="value">The amount to increment by.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The new score of member.</returns>
        /// <remarks><seealso href="https://redis.io/commands/zincrby"/></remarks>
        public double SortedSetIncrement(RedisKey key, RedisValue member, double value, CommandFlags flags = CommandFlags.None)
        {
            return _db.SortedSetIncrement(key, member, value, flags);
        }

        /// <summary>
        /// Returns the cardinality of the intersection of the sorted sets at <paramref name="keys"/>.
        /// </summary>
        /// <param name="keys">The keys of the sorted sets.</param>
        /// <param name="limit">If the intersection cardinality reaches <paramref name="limit"/> partway through the computation, the algorithm will exit and yield <paramref name="limit"/> as the cardinality (defaults to 0 meaning unlimited).</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The number of elements in the resulting intersection.</returns>
        /// <remarks><seealso href="https://redis.io/commands/zintercard"/></remarks>
        public long SortedSetIntersectionLength(RedisKey[] keys, long limit = 0, CommandFlags flags = CommandFlags.None)
        {
            return _db.SortedSetIntersectionLength(keys, limit, flags);
        }

        /// <summary>
        /// Returns the sorted set cardinality (number of elements) of the sorted set stored at key.
        /// </summary>
        /// <param name="key">The key of the sorted set.</param>
        /// <param name="min">The min score to filter by (defaults to negative infinity).</param>
        /// <param name="max">The max score to filter by (defaults to positive infinity).</param>
        /// <param name="exclude">Whether to exclude <paramref name="min"/> and <paramref name="max"/> from the range check (defaults to both inclusive).</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The cardinality (number of elements) of the sorted set, or 0 if key does not exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/zcard"/></remarks>
        public long SortedSetLength(RedisKey key, double min = double.NegativeInfinity, double max = double.PositiveInfinity, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
            return _db.SortedSetLength(key, min, max, exclude, flags);
        }

        /// <summary>
        /// When all the elements in a sorted set are inserted with the same score, in order to force lexicographical ordering.
        /// This command returns the number of elements in the sorted set at key with a value between min and max.
        /// </summary>
        /// <param name="key">The key of the sorted set.</param>
        /// <param name="min">The min value to filter by.</param>
        /// <param name="max">The max value to filter by.</param>
        /// <param name="exclude">Whether to exclude <paramref name="min"/> and <paramref name="max"/> from the range check (defaults to both inclusive).</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The number of elements in the specified score range.</returns>
        /// <remarks><seealso href="https://redis.io/commands/zlexcount"/></remarks>
        public long SortedSetLengthByValue(RedisKey key, RedisValue min, RedisValue max, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
            return _db.SortedSetLengthByValue(key, min, max, exclude, flags);
        }

        /// <summary>
        /// Returns a random element from the sorted set value stored at <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key of the sorted set.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The randomly selected element, or <see cref="RedisValue.Null"/> when <paramref name="key"/> does not exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/zrandmember"/></remarks>
        public RedisValue SortedSetRandomMember(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _db.SortedSetRandomMember(key, flags);
        }

        /// <summary>
        /// Returns an array of random elements from the sorted set value stored at <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key of the sorted set.</param>
        /// <param name="count">
        ///   <para>
        ///     If the provided count argument is positive, returns an array of distinct elements.
        ///     The array's length is either <paramref name="count"/> or the sorted set's cardinality (ZCARD), whichever is lower.
        ///   </para>
        ///   <para>
        ///     If called with a negative count, the behavior changes and the command is allowed to return the same element multiple times.
        ///     In this case, the number of returned elements is the absolute value of the specified count.
        ///   </para>
        /// </param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The randomly selected elements, or an empty array when <paramref name="key"/> does not exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/zrandmember"/></remarks>
        public RedisValue[] SortedSetRandomMembers(RedisKey key, long count, CommandFlags flags = CommandFlags.None)
        {
            return _db.SortedSetRandomMembers(key, count, flags);
        }

        /// <summary>
        /// Returns an array of random elements from the sorted set value stored at <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key of the sorted set.</param>
        /// <param name="count">
        ///   <para>
        ///     If the provided count argument is positive, returns an array of distinct elements.
        ///     The array's length is either <paramref name="count"/> or the sorted set's cardinality (ZCARD), whichever is lower.
        ///   </para>
        ///   <para>
        ///     If called with a negative count, the behavior changes and the command is allowed to return the same element multiple times.
        ///     In this case, the number of returned elements is the absolute value of the specified count.
        ///   </para>
        /// </param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The randomly selected elements with scores, or an empty array when <paramref name="key"/> does not exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/zrandmember"/></remarks>
        public SortedSetEntry[] SortedSetRandomMembersWithScores(RedisKey key, long count, CommandFlags flags = CommandFlags.None)
        {
            return _db.SortedSetRandomMembersWithScores(key, count, flags);
        }

        /// <summary>
        /// Returns the specified range of elements in the sorted set stored at key.
        /// By default the elements are considered to be ordered from the lowest to the highest score.
        /// Lexicographical order is used for elements with equal score.
        /// Both start and stop are zero-based indexes, where 0 is the first element, 1 is the next element and so on.
        /// They can also be negative numbers indicating offsets from the end of the sorted set, with -1 being the last element of the sorted set, -2 the penultimate element and so on.
        /// </summary>
        /// <param name="key">The key of the sorted set.</param>
        /// <param name="start">The start index to get.</param>
        /// <param name="stop">The stop index to get.</param>
        /// <param name="order">The order to sort by (defaults to ascending).</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>List of elements in the specified range.</returns>
        /// <remarks>
        /// <seealso href="https://redis.io/commands/zrange"/>,
        /// <seealso href="https://redis.io/commands/zrevrange"/>
        /// </remarks>
        public RedisValue[] SortedSetRangeByRank(RedisKey key, long start = 0, long stop = -1, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            return _db.SortedSetRangeByRank(key, start, stop, order, flags);
        }

        /// <summary>
        /// Takes the specified range of elements in the sorted set of the <paramref name="sourceKey"/>
        /// and stores them in a new sorted set at the <paramref name="destinationKey"/>.
        /// </summary>
        /// <param name="sourceKey">The sorted set to take the range from.</param>
        /// <param name="destinationKey">Where the resulting set will be stored.</param>
        /// <param name="start">The starting point in the sorted set. If <paramref name="sortedSetOrder"/> is <see cref="SortedSetOrder.ByLex"/>, this should be a string.</param>
        /// <param name="stop">The stopping point in the range of the sorted set. If <paramref name="sortedSetOrder"/> is <see cref="SortedSetOrder.ByLex"/>, this should be a string.</param>
        /// <param name="sortedSetOrder">The ordering criteria to use for the range. Choices are <see cref="SortedSetOrder.ByRank"/>, <see cref="SortedSetOrder.ByScore"/>, and <see cref="SortedSetOrder.ByLex"/> (defaults to <see cref="SortedSetOrder.ByRank"/>).</param>
        /// <param name="exclude">Whether to exclude <paramref name="start"/> and <paramref name="stop"/> from the range check (defaults to both inclusive).</param>
        /// <param name="order">
        /// The direction to consider the <paramref name="start"/> and <paramref name="stop"/> in.
        /// If <see cref="Order.Ascending"/>, the <paramref name="start"/> must be smaller than the <paramref name="stop"/>.
        /// If <see cref="Order.Descending"/>, <paramref name="stop"/> must be smaller than <paramref name="start"/>.
        /// </param>
        /// <param name="skip">The number of elements into the sorted set to skip. Note: this iterates after sorting so incurs O(n) cost for large values.</param>
        /// <param name="take">The maximum number of elements to pull into the new (<paramref name="destinationKey"/>) set.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The cardinality of (number of elements in) the newly created sorted set.</returns>
        /// <remarks><seealso href="https://redis.io/commands/zrangestore"/></remarks>
        public long SortedSetRangeAndStore(
            RedisKey sourceKey,
            RedisKey destinationKey,
            RedisValue start,
            RedisValue stop,
            SortedSetOrder sortedSetOrder = SortedSetOrder.ByRank,
            Exclude exclude = Exclude.None,
            Order order = Order.Ascending,
            long skip = 0,
            long? take = null,
            CommandFlags flags = CommandFlags.None)
        {
            return _db.SortedSetRangeAndStore(sourceKey, destinationKey, start, stop, sortedSetOrder, exclude, order, skip, take, flags);
        }

        /// <summary>
        /// Returns the specified range of elements in the sorted set stored at key.
        /// By default the elements are considered to be ordered from the lowest to the highest score.
        /// Lexicographical order is used for elements with equal score.
        /// Both start and stop are zero-based indexes, where 0 is the first element, 1 is the next element and so on.
        /// They can also be negative numbers indicating offsets from the end of the sorted set, with -1 being the last element of the sorted set, -2 the penultimate element and so on.
        /// </summary>
        /// <param name="key">The key of the sorted set.</param>
        /// <param name="start">The start index to get.</param>
        /// <param name="stop">The stop index to get.</param>
        /// <param name="order">The order to sort by (defaults to ascending).</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>List of elements in the specified range.</returns>
        /// <remarks>
        /// <seealso href="https://redis.io/commands/zrange"/>,
        /// <seealso href="https://redis.io/commands/zrevrange"/>
        /// </remarks>
        public SortedSetEntry[] SortedSetRangeByRankWithScores(RedisKey key, long start = 0, long stop = -1, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            return _db.SortedSetRangeByRankWithScores(key, start, stop, order, flags);
        }

        /// <summary>
        /// Returns the specified range of elements in the sorted set stored at key.
        /// By default the elements are considered to be ordered from the lowest to the highest score.
        /// Lexicographical order is used for elements with equal score.
        /// Start and stop are used to specify the min and max range for score values.
        /// Similar to other range methods the values are inclusive.
        /// </summary>
        /// <param name="key">The key of the sorted set.</param>
        /// <param name="start">The minimum score to filter by.</param>
        /// <param name="stop">The maximum score to filter by.</param>
        /// <param name="exclude">Which of <paramref name="start"/> and <paramref name="stop"/> to exclude (defaults to both inclusive).</param>
        /// <param name="order">The order to sort by (defaults to ascending).</param>
        /// <param name="skip">How many items to skip.</param>
        /// <param name="take">How many items to take.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>List of elements in the specified score range.</returns>
        /// <remarks>
        /// <seealso href="https://redis.io/commands/zrangebyscore"/>,
        /// <seealso href="https://redis.io/commands/zrevrangebyscore"/>
        /// </remarks>
        public RedisValue[] SortedSetRangeByScore(RedisKey key,
            double start = double.NegativeInfinity,
            double stop = double.PositiveInfinity,
            Exclude exclude = Exclude.None,
            Order order = Order.Ascending,
            long skip = 0,
            long take = -1,
            CommandFlags flags = CommandFlags.None)
        {
            return _db.SortedSetRangeByScore(key, start, stop, exclude, order, skip, take, flags);
        }

        /// <summary>
        /// Returns the specified range of elements in the sorted set stored at key.
        /// By default the elements are considered to be ordered from the lowest to the highest score.
        /// Lexicographical order is used for elements with equal score.
        /// Start and stop are used to specify the min and max range for score values.
        /// Similar to other range methods the values are inclusive.
        /// </summary>
        /// <param name="key">The key of the sorted set.</param>
        /// <param name="start">The minimum score to filter by.</param>
        /// <param name="stop">The maximum score to filter by.</param>
        /// <param name="exclude">Which of <paramref name="start"/> and <paramref name="stop"/> to exclude (defaults to both inclusive).</param>
        /// <param name="order">The order to sort by (defaults to ascending).</param>
        /// <param name="skip">How many items to skip.</param>
        /// <param name="take">How many items to take.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>List of elements in the specified score range.</returns>
        /// <remarks>
        /// <seealso href="https://redis.io/commands/zrangebyscore"/>,
        /// <seealso href="https://redis.io/commands/zrevrangebyscore"/>
        /// </remarks>
        public SortedSetEntry[] SortedSetRangeByScoreWithScores(RedisKey key,
            double start = double.NegativeInfinity,
            double stop = double.PositiveInfinity,
            Exclude exclude = Exclude.None,
            Order order = Order.Ascending,
            long skip = 0,
            long take = -1,
            CommandFlags flags = CommandFlags.None)
        {
            return _db.SortedSetRangeByScoreWithScores(key, start, stop, exclude, order, skip, take, flags);
        }

        /// <summary>
        /// When all the elements in a sorted set are inserted with the same score, in order to force lexicographical ordering.
        /// This command returns all the elements in the sorted set at key with a value between min and max.
        /// </summary>
        /// <param name="key">The key of the sorted set.</param>
        /// <param name="min">The min value to filter by.</param>
        /// <param name="max">The max value to filter by.</param>
        /// <param name="exclude">Which of <paramref name="min"/> and <paramref name="max"/> to exclude (defaults to both inclusive).</param>
        /// <param name="skip">How many items to skip.</param>
        /// <param name="take">How many items to take.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>List of elements in the specified score range.</returns>
        /// <remarks><seealso href="https://redis.io/commands/zrangebylex"/></remarks>
        public RedisValue[] SortedSetRangeByValue(RedisKey key,
            RedisValue min,
            RedisValue max,
            Exclude exclude,
            long skip,
            long take = -1,
            CommandFlags flags = CommandFlags.None) // defaults removed to avoid ambiguity with overload with order
        {
            return SortedSetRangeByValue(key, min, max, exclude, skip, take, flags);
        }

        /// <summary>
        /// When all the elements in a sorted set are inserted with the same score, in order to force lexicographical ordering.
        /// This command returns all the elements in the sorted set at key with a value between min and max.
        /// </summary>
        /// <param name="key">The key of the sorted set.</param>
        /// <param name="min">The min value to filter by.</param>
        /// <param name="max">The max value to filter by.</param>
        /// <param name="exclude">Which of <paramref name="min"/> and <paramref name="max"/> to exclude (defaults to both inclusive).</param>
        /// <param name="order">Whether to order the data ascending or descending</param>
        /// <param name="skip">How many items to skip.</param>
        /// <param name="take">How many items to take.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>List of elements in the specified score range.</returns>
        /// <remarks>
        /// <seealso href="https://redis.io/commands/zrangebylex"/>,
        /// <seealso href="https://redis.io/commands/zrevrangebylex"/>
        /// </remarks>
        public RedisValue[] SortedSetRangeByValue(RedisKey key,
            RedisValue min = default,
            RedisValue max = default,
            Exclude exclude = Exclude.None,
            Order order = Order.Ascending,
            long skip = 0,
            long take = -1,
            CommandFlags flags = CommandFlags.None)
        {
            return _db.SortedSetRangeByValue(key, min, max, exclude, order, skip, take, flags);
        }

        /// <summary>
        /// Returns the rank of member in the sorted set stored at key, by default with the scores ordered from low to high.
        /// The rank (or index) is 0-based, which means that the member with the lowest score has rank 0.
        /// </summary>
        /// <param name="key">The key of the sorted set.</param>
        /// <param name="member">The member to get the rank of.</param>
        /// <param name="order">The order to sort by (defaults to ascending).</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>If member exists in the sorted set, the rank of member. If member does not exist in the sorted set or key does not exist, <see langword="null"/>.</returns>
        /// <remarks>
        /// <seealso href="https://redis.io/commands/zrank"/>,
        /// <seealso href="https://redis.io/commands/zrevrank"/>
        /// </remarks>
        public long? SortedSetRank(RedisKey key, RedisValue member, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            return _db.SortedSetRank(key, member, order, flags);
        }

        /// <summary>
        /// Removes the specified member from the sorted set stored at key. Non existing members are ignored.
        /// </summary>
        /// <param name="key">The key of the sorted set.</param>
        /// <param name="member">The member to remove.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns><see langword="true"/> if the member existed in the sorted set and was removed. <see langword="false"/> otherwise.</returns>
        /// <remarks><seealso href="https://redis.io/commands/zrem"/></remarks>
        public bool SortedSetRemove(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None)
        {
            return _db.SortedSetRemove(key, member, flags);
        }

        /// <summary>
        /// Removes the specified members from the sorted set stored at key. Non existing members are ignored.
        /// </summary>
        /// <param name="key">The key of the sorted set.</param>
        /// <param name="members">The members to remove.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The number of members removed from the sorted set, not including non existing members.</returns>
        /// <remarks><seealso href="https://redis.io/commands/zrem"/></remarks>
        public long SortedSetRemove(RedisKey key, RedisValue[] members, CommandFlags flags = CommandFlags.None)
        {
            return _db.SortedSetRemove(key, members, flags);
        }

        /// <summary>
        /// Removes all elements in the sorted set stored at key with rank between start and stop.
        /// Both start and stop are 0 -based indexes with 0 being the element with the lowest score.
        /// These indexes can be negative numbers, where they indicate offsets starting at the element with the highest score.
        /// For example: -1 is the element with the highest score, -2 the element with the second highest score and so forth.
        /// </summary>
        /// <param name="key">The key of the sorted set.</param>
        /// <param name="start">The minimum rank to remove.</param>
        /// <param name="stop">The maximum rank to remove.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The number of elements removed.</returns>
        /// <remarks><seealso href="https://redis.io/commands/zremrangebyrank"/></remarks>
        public long SortedSetRemoveRangeByRank(RedisKey key, long start, long stop, CommandFlags flags = CommandFlags.None)
        {
            return _db.SortedSetRemoveRangeByRank(key, start, stop, flags);
        }

        /// <summary>
        /// Removes all elements in the sorted set stored at key with a score between min and max (inclusive by default).
        /// </summary>
        /// <param name="key">The key of the sorted set.</param>
        /// <param name="start">The minimum score to remove.</param>
        /// <param name="stop">The maximum score to remove.</param>
        /// <param name="exclude">Which of <paramref name="start"/> and <paramref name="stop"/> to exclude (defaults to both inclusive).</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The number of elements removed.</returns>
        /// <remarks><seealso href="https://redis.io/commands/zremrangebyscore"/></remarks>
        public long SortedSetRemoveRangeByScore(RedisKey key, double start, double stop, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
            return _db.SortedSetRemoveRangeByScore(key, start, stop, exclude, flags);
        }

        /// <summary>
        /// When all the elements in a sorted set are inserted with the same score, in order to force lexicographical ordering.
        /// This command removes all elements in the sorted set stored at key between the lexicographical range specified by min and max.
        /// </summary>
        /// <param name="key">The key of the sorted set.</param>
        /// <param name="min">The minimum value to remove.</param>
        /// <param name="max">The maximum value to remove.</param>
        /// <param name="exclude">Which of <paramref name="min"/> and <paramref name="max"/> to exclude (defaults to both inclusive).</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The number of elements removed.</returns>
        /// <remarks><seealso href="https://redis.io/commands/zremrangebylex"/></remarks>
        public long SortedSetRemoveRangeByValue(RedisKey key, RedisValue min, RedisValue max, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
            return _db.SortedSetRemoveRangeByValue(key, min, max, exclude, flags);
        }

        /// <summary>
        /// Returns the score of member in the sorted set at key.
        /// If member does not exist in the sorted set, or key does not exist, nil is returned.
        /// </summary>
        /// <param name="key">The key of the sorted set.</param>
        /// <param name="member">The member to get a score for.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The score of the member.</returns>
        /// <remarks><seealso href="https://redis.io/commands/zscore"/></remarks>
        public double? SortedSetScore(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None)
        {
            return _db.SortedSetScore(key, member, flags);
        }

        /// <summary>
        /// Returns the scores of members in the sorted set at <paramref name="key"/>.
        /// If a member does not exist in the sorted set, or key does not exist, <see langword="null"/> is returned.
        /// </summary>
        /// <param name="key">The key of the sorted set.</param>
        /// <param name="members">The members to get a score for.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>
        /// The scores of the members in the same order as the <paramref name="members"/> array.
        /// If a member does not exist in the set, <see langword="null"/> is returned.
        /// </returns>
        /// <remarks><seealso href="https://redis.io/commands/zmscore"/></remarks>
        public double?[] SortedSetScores(RedisKey key, RedisValue[] members, CommandFlags flags = CommandFlags.None)
        {
            return _db.SortedSetScores(key, members, flags);
        }

        /// <summary>
        /// Same as <see cref="SortedSetAdd(RedisKey, SortedSetEntry[], SortedSetWhen, CommandFlags)" /> but return the number of the elements changed.
        /// </summary>
        /// <param name="key">The key of the sorted set.</param>
        /// <param name="member">The member to add/update to the sorted set.</param>
        /// <param name="score">The score for the member to add/update to the sorted set.</param>
        /// <param name="when">What conditions to add the element under (defaults to always).</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The number of elements changed.</returns>
        /// <remarks><seealso href="https://redis.io/commands/zadd"/></remarks>
        public bool SortedSetUpdate(RedisKey key, RedisValue member, double score, SortedSetWhen when = SortedSetWhen.Always, CommandFlags flags = CommandFlags.None)
        {
            return _db.SortedSetAdd(key, member, score, when, flags);
        }

        /// <summary>
        /// Same as <see cref="SortedSetAdd(RedisKey, SortedSetEntry[], SortedSetWhen, CommandFlags)" /> but return the number of the elements changed.
        /// </summary>
        /// <param name="key">The key of the sorted set.</param>
        /// <param name="values">The members and values to add/update to the sorted set.</param>
        /// <param name="when">What conditions to add the element under (defaults to always).</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The number of elements changed.</returns>
        /// <remarks><seealso href="https://redis.io/commands/zadd"/></remarks>
        public long SortedSetUpdate(RedisKey key, SortedSetEntry[] values, SortedSetWhen when = SortedSetWhen.Always, CommandFlags flags = CommandFlags.None)
        {
            return _db.SortedSetAdd(key, values, when, flags);
        }

        /// <summary>
        /// Removes and returns the first element from the sorted set stored at key, by default with the scores ordered from low to high.
        /// </summary>
        /// <param name="key">The key of the sorted set.</param>
        /// <param name="order">The order to sort by (defaults to ascending).</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The removed element, or nil when key does not exist.</returns>
        /// <remarks>
        /// <seealso href="https://redis.io/commands/zpopmin"/>,
        /// <seealso href="https://redis.io/commands/zpopmax"/>
        /// </remarks>
        public SortedSetEntry? SortedSetPop(RedisKey key, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            return _db.SortedSetPop(key, order, flags);
        }

        /// <summary>
        /// Removes and returns the specified number of first elements from the sorted set stored at key, by default with the scores ordered from low to high.
        /// </summary>
        /// <param name="key">The key of the sorted set.</param>
        /// <param name="count">The number of elements to return.</param>
        /// <param name="order">The order to sort by (defaults to ascending).</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>An array of elements, or an empty array when key does not exist.</returns>
        /// <remarks>
        /// <seealso href="https://redis.io/commands/zpopmin"/>,
        /// <seealso href="https://redis.io/commands/zpopmax"/>
        /// </remarks>
        public SortedSetEntry[] SortedSetPop(RedisKey key, long count, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            return _db.SortedSetPop(key, count, order, flags);
        }

        /// <summary>
        /// Removes and returns up to <paramref name="count"/> entries from the first non-empty sorted set in <paramref name="keys"/>.
        /// Returns <see cref="SortedSetPopResult.Null"/> if none of the sets exist or contain any elements.
        /// </summary>
        /// <param name="keys">The keys to check.</param>
        /// <param name="count">The maximum number of records to pop out of the sorted set.</param>
        /// <param name="order">The order to sort by when popping items out of the set.</param>
        /// <param name="flags">The flags to use for the operation.</param>
        /// <returns>A contiguous collection of sorted set entries with the key they were popped from, or <see cref="SortedSetPopResult.Null"/> if no non-empty sorted sets are found.</returns>
        /// <remarks><seealso href="https://redis.io/commands/zmpop"/></remarks>
        public SortedSetPopResult SortedSetPop(RedisKey[] keys, long count, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            return _db.SortedSetPop(keys, count, order, flags);
        }

        /// <summary>
        /// 有序集合/定时任务延迟队列用的多
        /// </summary>
        /// <param name="key"></param>
        /// <param name="member">元素</param>
        /// <param name="score">分数</param>
        /// <returns></returns>
        public bool SortedSetAdd(string key, string member, double score) => _db.SortedSetAdd(key, member, score);

        /// <summary>
        /// Removes the specified members from the sorted set stored at key. Non existing members are ignored.
        /// </summary>
        /// <param name="key">The key of the sorted set.</param>
        /// <param name="members">The members to remove.</param>
        /// <returns>The number of members removed from the sorted set, not including non existing members.</returns>
        /// <remarks><seealso href="https://redis.io/commands/zrem"/></remarks>
        public long SortedSetRemove(string key, IEnumerable<string> members) => _db.SortedSetRemove(key, members.ToRedisValues());

        /// <summary>
        /// Increments the score of member in the sorted set stored at key by increment. If member does not exist in the sorted set, it is added with increment as its score (as if its previous score was 0.0).
        /// </summary>
        /// <param name="key">The key of the sorted set.</param>
        /// <param name="member">The member to increment.</param>
        /// <param name="value">The amount to increment by.</param>
        /// <returns>The new score of member.</returns>
        /// <remarks><seealso href="https://redis.io/commands/zincrby"/></remarks>
        public double SortedSetIncrement(string key, string member, double value) => _db.SortedSetIncrement(key, member, value);

        /// <summary>
        /// Decrements the score of member in the sorted set stored at key by decrement.
        /// If member does not exist in the sorted set, it is added with -decrement as its score (as if its previous score was 0.0).
        /// </summary>
        /// <param name="key">The key of the sorted set.</param>
        /// <param name="member">The member to decrement.</param>
        /// <param name="value">The amount to decrement by.</param>
        /// <returns>The new score of member.</returns>
        /// <remarks><seealso href="https://redis.io/commands/zincrby"/></remarks>
        public double SortedSetDecrement(string key, string member, double value) => _db.SortedSetDecrement(key, member, value);

        /// <summary>
        /// 按序返回topN
        /// </summary>
        /// <param name="key"></param>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public ConcurrentDictionary<string, double> SortedSetRangeByRankWithScores(string key,
            long start = 0,
            long stop = -1,
            Order order = Order.Ascending) =>
            (_db.SortedSetRangeByRankWithScores(key, start, stop, order)).ToConcurrentDictionary();

        /// <summary>
        /// Returns the specified range of elements in the sorted set stored at key.
        /// By default the elements are considered to be ordered from the lowest to the highest score.
        /// Lexicographical order is used for elements with equal score.
        /// Start and stop are used to specify the min and max range for score values.
        /// Similar to other range methods the values are inclusive.
        /// </summary>
        /// <param name="key">The key of the sorted set.</param>
        /// <param name="start">The minimum score to filter by.</param>
        /// <param name="stop">The maximum score to filter by.</param>
        /// <param name="exclude">Which of <paramref name="start"/> and <paramref name="stop"/> to exclude (defaults to both inclusive).</param>
        /// <param name="order">The order to sort by (defaults to ascending).</param>
        /// <param name="skip">How many items to skip.</param>
        /// <param name="take">How many items to take.</param>
        /// <returns>List of elements in the specified score range.</returns>
        /// <remarks>
        /// <seealso href="https://redis.io/commands/zrangebyscore"/>,
        /// <seealso href="https://redis.io/commands/zrevrangebyscore"/>
        /// </remarks>
        public ConcurrentDictionary<string, double> SortedSetRangeByScoreWithScores(string key,
            double start = double.NegativeInfinity, double stop = double.PositiveInfinity,
            Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1) =>
            (_db.SortedSetRangeByScoreWithScores(key, start, stop, exclude, order, skip, take))
            .ToConcurrentDictionary();

        #endregion

        #region SortedSet Async

        /// <summary>
        /// Sorts a list, set or sorted set (numerically or alphabetically, ascending by default).
        /// By default, the elements themselves are compared, but the values can also be used to perform external key-lookups using the <c>by</c> parameter.
        /// By default, the elements themselves are returned, but external key-lookups (one or many) can be performed instead by specifying
        /// the <c>get</c> parameter (note that <c>#</c> specifies the element itself, when used in <c>get</c>).
        /// Referring to the <a href="https://redis.io/commands/sort">redis SORT documentation </a> for examples is recommended.
        /// When used in hashes, <c>by</c> and <c>get</c> can be used to specify fields using <c>-&gt;</c> notation (again, refer to redis documentation).
        /// Uses <a href="https://redis.io/commands/sort_ro">SORT_RO</a> when possible.
        /// </summary>
        /// <param name="key">The key of the list, set, or sorted set.</param>
        /// <param name="skip">How many entries to skip on the return.</param>
        /// <param name="take">How many entries to take on the return.</param>
        /// <param name="order">The ascending or descending order (defaults to ascending).</param>
        /// <param name="sortType">The sorting method (defaults to numeric).</param>
        /// <param name="by">The key pattern to sort by, if any. e.g. ExternalKey_* would sort by ExternalKey_{listvalue} as a lookup.</param>
        /// <param name="get">The key pattern to sort by, if any e.g. ExternalKey_* would return the value of ExternalKey_{listvalue} for each entry.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The sorted elements, or the external values if <c>get</c> is specified.</returns>
        /// <remarks>
        /// <seealso href="https://redis.io/commands/sort"/>,
        /// <seealso href="https://redis.io/commands/sort_ro"/>
        /// </remarks>
        public async Task<RedisValue[]> SortAsync(RedisKey key, long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric, RedisValue by = default, RedisValue[]? get = null, CommandFlags flags = CommandFlags.None)
        {
            return await _db.SortAsync(key, skip, take, order, sortType, by, get, flags);
        }

        /// <summary>
        /// Sorts a list, set or sorted set (numerically or alphabetically, ascending by default).
        /// By default, the elements themselves are compared, but the values can also be used to perform external key-lookups using the <c>by</c> parameter.
        /// By default, the elements themselves are returned, but external key-lookups (one or many) can be performed instead by specifying
        /// the <c>get</c> parameter (note that <c>#</c> specifies the element itself, when used in <c>get</c>).
        /// Referring to the <a href="https://redis.io/commands/sort">redis SORT documentation</a> for examples is recommended.
        /// When used in hashes, <c>by</c> and <c>get</c> can be used to specify fields using <c>-&gt;</c> notation (again, refer to redis documentation).
        /// </summary>
        /// <param name="destination">The destination key to store results in.</param>
        /// <param name="key">The key of the list, set, or sorted set.</param>
        /// <param name="skip">How many entries to skip on the return.</param>
        /// <param name="take">How many entries to take on the return.</param>
        /// <param name="order">The ascending or descending order (defaults to ascending).</param>
        /// <param name="sortType">The sorting method (defaults to numeric).</param>
        /// <param name="by">The key pattern to sort by, if any. e.g. ExternalKey_* would sort by ExternalKey_{listvalue} as a lookup.</param>
        /// <param name="get">The key pattern to sort by, if any e.g. ExternalKey_* would return the value of ExternalKey_{listvalue} for each entry.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The number of elements stored in the new list.</returns>
        /// <remarks><seealso href="https://redis.io/commands/sort"/></remarks>
        public async Task<long> SortAndStoreAsync(RedisKey destination, RedisKey key, long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric, RedisValue by = default, RedisValue[]? get = null, CommandFlags flags = CommandFlags.None)
        {
            return await _db.SortAndStoreAsync(destination, key, skip, take, order, sortType, by, get, flags);
        }

        /// <inheritdoc cref="SortedSetAddAsync(RedisKey, RedisValue, double, SortedSetWhen, CommandFlags)" />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public async Task<bool> SortedSetAddAsync(RedisKey key, RedisValue member, double score, CommandFlags flags)
        {
            return await _db.SortedSetAddAsync(key, member, score, flags);
        }

        /// <inheritdoc cref="SortedSetAddAsync(RedisKey, RedisValue, double, SortedSetWhen, CommandFlags)" />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public async Task<bool> SortedSetAddAsync(RedisKey key, RedisValue member, double score, When when, CommandFlags flags = CommandFlags.None)
        {
            return await _db.SortedSetAddAsync(key, member, score, when, flags);
        }

        /// <summary>
        /// Adds the specified member with the specified score to the sorted set stored at key.
        /// If the specified member is already a member of the sorted set, the score is updated and the element reinserted at the right position to ensure the correct ordering.
        /// </summary>
        /// <param name="key">The key of the sorted set.</param>
        /// <param name="member">The member to add to the sorted set.</param>
        /// <param name="score">The score for the member to add to the sorted set.</param>
        /// <param name="when">What conditions to add the element under (defaults to always).</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns><see langword="true"/> if the value was added. <see langword="false"/> if it already existed (the score is still updated).</returns>
        /// <remarks><seealso href="https://redis.io/commands/zadd"/></remarks>
        public async Task<bool> SortedSetAddAsync(RedisKey key, RedisValue member, double score, SortedSetWhen when = SortedSetWhen.Always, CommandFlags flags = CommandFlags.None)
        {
            return await _db.SortedSetAddAsync(key, member, score, when, flags);
        }

        /// <inheritdoc cref="SortedSetAddAsync(RedisKey, RedisValue, double, SortedSetWhen, CommandFlags)" />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public async Task<long> SortedSetAddAsync(RedisKey key, SortedSetEntry[] values, CommandFlags flags)
        {
            return await _db.SortedSetAddAsync(key, values, flags);
        }

        /// <inheritdoc cref="SortedSetAddAsync(RedisKey, RedisValue, double, SortedSetWhen, CommandFlags)" />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public async Task<long> SortedSetAddAsync(RedisKey key, SortedSetEntry[] values, When when, CommandFlags flags = CommandFlags.None)
        {
            return await _db.SortedSetAddAsync(key, values, when, flags);
        }

        /// <summary>
        /// Adds all the specified members with the specified scores to the sorted set stored at key.
        /// If a specified member is already a member of the sorted set, the score is updated and the element reinserted at the right position to ensure the correct ordering.
        /// </summary>
        /// <param name="key">The key of the sorted set.</param>
        /// <param name="values">The members and values to add to the sorted set.</param>
        /// <param name="when">What conditions to add the element under (defaults to always).</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The number of elements added to the sorted sets, not including elements already existing for which the score was updated.</returns>
        /// <remarks><seealso href="https://redis.io/commands/zadd"/></remarks>
        public async Task<long> SortedSetAddAsync(RedisKey key, SortedSetEntry[] values, SortedSetWhen when = SortedSetWhen.Always, CommandFlags flags = CommandFlags.None)
        {
            return await _db.SortedSetAddAsync(key, values, when, flags);
        }

        /// <summary>
        /// Computes a set operation for multiple sorted sets (optionally using per-set <paramref name="weights"/>),
        /// optionally performing a specific aggregation (defaults to <see cref="Aggregate.Sum"/>).
        /// <see cref="SetOperation.Difference"/> cannot be used with weights or aggregation.
        /// </summary>
        /// <param name="operation">The operation to perform.</param>
        /// <param name="keys">The keys of the sorted sets.</param>
        /// <param name="weights">The optional weights per set that correspond to <paramref name="keys"/>.</param>
        /// <param name="aggregate">The aggregation method (defaults to <see cref="Aggregate.Sum"/>).</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The resulting sorted set.</returns>
        /// <remarks>
        /// <seealso href="https://redis.io/commands/zunion"/>,
        /// <seealso href="https://redis.io/commands/zinter"/>,
        /// <seealso href="https://redis.io/commands/zdiff"/>
        /// </remarks>
        public async Task<RedisValue[]> SortedSetCombineAsync(SetOperation operation, RedisKey[] keys, double[]? weights = null, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None)
        {
            return await _db.SortedSetCombineAsync(operation, keys, weights, aggregate, flags);
        }

        /// <summary>
        /// Computes a set operation for multiple sorted sets (optionally using per-set <paramref name="weights"/>),
        /// optionally performing a specific aggregation (defaults to <see cref="Aggregate.Sum"/>).
        /// <see cref="SetOperation.Difference"/> cannot be used with weights or aggregation.
        /// </summary>
        /// <param name="operation">The operation to perform.</param>
        /// <param name="keys">The keys of the sorted sets.</param>
        /// <param name="weights">The optional weights per set that correspond to <paramref name="keys"/>.</param>
        /// <param name="aggregate">The aggregation method (defaults to <see cref="Aggregate.Sum"/>).</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The resulting sorted set with scores.</returns>
        /// <remarks>
        /// <seealso href="https://redis.io/commands/zunion"/>,
        /// <seealso href="https://redis.io/commands/zinter"/>,
        /// <seealso href="https://redis.io/commands/zdiff"/>
        /// </remarks>
        public async Task<SortedSetEntry[]> SortedSetCombineWithScoresAsync(SetOperation operation, RedisKey[] keys, double[]? weights = null, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None)
        {
            return await _db.SortedSetCombineWithScoresAsync(operation, keys, weights, aggregate, flags);
        }

        /// <summary>
        /// Computes a set operation over two sorted sets, and stores the result in destination, optionally performing
        /// a specific aggregation (defaults to sum).
        /// <see cref="SetOperation.Difference"/> cannot be used with aggregation.
        /// </summary>
        /// <param name="operation">The operation to perform.</param>
        /// <param name="destination">The key to store the results in.</param>
        /// <param name="first">The key of the first sorted set.</param>
        /// <param name="second">The key of the second sorted set.</param>
        /// <param name="aggregate">The aggregation method (defaults to sum).</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The number of elements in the resulting sorted set at destination.</returns>
        /// <remarks>
        /// <seealso href="https://redis.io/commands/zunionstore"/>,
        /// <seealso href="https://redis.io/commands/zinterstore"/>,
        /// <seealso href="https://redis.io/commands/zdiffstore"/>
        /// </remarks>
        public async Task<long> SortedSetCombineAndStoreAsync(SetOperation operation, RedisKey destination, RedisKey first, RedisKey second, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None)
        {
            return await _db.SortedSetCombineAndStoreAsync(operation, destination, first, second, aggregate, flags);
        }

        /// <summary>
        /// Computes a set operation over multiple sorted sets (optionally using per-set weights), and stores the result in destination, optionally performing
        /// a specific aggregation (defaults to sum).
        /// <see cref="SetOperation.Difference"/> cannot be used with aggregation.
        /// </summary>
        /// <param name="operation">The operation to perform.</param>
        /// <param name="destination">The key to store the results in.</param>
        /// <param name="keys">The keys of the sorted sets.</param>
        /// <param name="weights">The optional weights per set that correspond to <paramref name="keys"/>.</param>
        /// <param name="aggregate">The aggregation method (defaults to sum).</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The number of elements in the resulting sorted set at destination.</returns>
        /// <remarks>
        /// <seealso href="https://redis.io/commands/zunionstore"/>,
        /// <seealso href="https://redis.io/commands/zinterstore"/>,
        /// <seealso href="https://redis.io/commands/zdiffstore"/>
        /// </remarks>
        public async Task<long> SortedSetCombineAndStoreAsync(SetOperation operation, RedisKey destination, RedisKey[] keys, double[]? weights = null, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None)
        {
            return await _db.SortedSetCombineAndStoreAsync(operation, destination, keys, weights, aggregate, flags);
        }

        /// <summary>
        /// Decrements the score of member in the sorted set stored at key by decrement.
        /// If member does not exist in the sorted set, it is added with -decrement as its score (as if its previous score was 0.0).
        /// </summary>
        /// <param name="key">The key of the sorted set.</param>
        /// <param name="member">The member to decrement.</param>
        /// <param name="value">The amount to decrement by.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The new score of member.</returns>
        /// <remarks><seealso href="https://redis.io/commands/zincrby"/></remarks>
        public async Task<double> SortedSetDecrementAsync(RedisKey key, RedisValue member, double value, CommandFlags flags = CommandFlags.None)
        {
            return await _db.SortedSetDecrementAsync(key, member, value, flags);
        }

        /// <summary>
        /// Increments the score of member in the sorted set stored at key by increment. If member does not exist in the sorted set, it is added with increment as its score (as if its previous score was 0.0).
        /// </summary>
        /// <param name="key">The key of the sorted set.</param>
        /// <param name="member">The member to increment.</param>
        /// <param name="value">The amount to increment by.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The new score of member.</returns>
        /// <remarks><seealso href="https://redis.io/commands/zincrby"/></remarks>
        public async Task<double> SortedSetIncrementAsync(RedisKey key, RedisValue member, double value, CommandFlags flags = CommandFlags.None)
        {
            return await _db.SortedSetIncrementAsync(key, member, value, flags);
        }

        /// <summary>
        /// Returns the cardinality of the intersection of the sorted sets at <paramref name="keys"/>.
        /// </summary>
        /// <param name="keys">The keys of the sorted sets.</param>
        /// <param name="limit">If the intersection cardinality reaches <paramref name="limit"/> partway through the computation, the algorithm will exit and yield <paramref name="limit"/> as the cardinality (defaults to 0 meaning unlimited).</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The number of elements in the resulting intersection.</returns>
        /// <remarks><seealso href="https://redis.io/commands/zintercard"/></remarks>
        public async Task<long> SortedSetIntersectionLengthAsync(RedisKey[] keys, long limit = 0, CommandFlags flags = CommandFlags.None)
        {
            return await _db.SortedSetIntersectionLengthAsync(keys, limit, flags);
        }

        /// <summary>
        /// Returns the sorted set cardinality (number of elements) of the sorted set stored at key.
        /// </summary>
        /// <param name="key">The key of the sorted set.</param>
        /// <param name="min">The min score to filter by (defaults to negative infinity).</param>
        /// <param name="max">The max score to filter by (defaults to positive infinity).</param>
        /// <param name="exclude">Whether to exclude <paramref name="min"/> and <paramref name="max"/> from the range check (defaults to both inclusive).</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The cardinality (number of elements) of the sorted set, or 0 if key does not exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/zcard"/></remarks>
        public async Task<long> SortedSetLengthAsync(RedisKey key, double min = double.NegativeInfinity, double max = double.PositiveInfinity, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
            return await _db.SortedSetLengthAsync(key, min, max, exclude, flags);
        }

        /// <summary>
        /// When all the elements in a sorted set are inserted with the same score, in order to force lexicographical ordering.
        /// This command returns the number of elements in the sorted set at key with a value between min and max.
        /// </summary>
        /// <param name="key">The key of the sorted set.</param>
        /// <param name="min">The min value to filter by.</param>
        /// <param name="max">The max value to filter by.</param>
        /// <param name="exclude">Whether to exclude <paramref name="min"/> and <paramref name="max"/> from the range check (defaults to both inclusive).</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The number of elements in the specified score range.</returns>
        /// <remarks><seealso href="https://redis.io/commands/zlexcount"/></remarks>
        public async Task<long> SortedSetLengthByValueAsync(RedisKey key, RedisValue min, RedisValue max, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
            return await _db.SortedSetLengthByValueAsync(key, min, max, exclude, flags);
        }

        /// <summary>
        /// Returns a random element from the sorted set value stored at <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key of the sorted set.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The randomly selected element, or <see cref="RedisValue.Null"/> when <paramref name="key"/> does not exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/zrandmember"/></remarks>
        public async Task<RedisValue> SortedSetRandomMemberAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return await _db.SortedSetRandomMemberAsync(key, flags);
        }

        /// <summary>
        /// Returns an array of random elements from the sorted set value stored at <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key of the sorted set.</param>
        /// <param name="count">
        ///   <para>
        ///     If the provided count argument is positive, returns an array of distinct elements.
        ///     The array's length is either <paramref name="count"/> or the sorted set's cardinality (ZCARD), whichever is lower.
        ///   </para>
        ///   <para>
        ///     If called with a negative count, the behavior changes and the command is allowed to return the same element multiple times.
        ///     In this case, the number of returned elements is the absolute value of the specified count.
        ///   </para>
        /// </param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The randomly selected elements, or an empty array when <paramref name="key"/> does not exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/zrandmember"/></remarks>
        public async Task<RedisValue[]> SortedSetRandomMembersAsync(RedisKey key, long count, CommandFlags flags = CommandFlags.None)
        {
            return await _db.SortedSetRandomMembersAsync(key, count, flags);
        }

        /// <summary>
        /// Returns an array of random elements from the sorted set value stored at <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key of the sorted set.</param>
        /// <param name="count">
        ///   <para>
        ///     If the provided count argument is positive, returns an array of distinct elements.
        ///     The array's length is either <paramref name="count"/> or the sorted set's cardinality (ZCARD), whichever is lower.
        ///   </para>
        ///   <para>
        ///     If called with a negative count, the behavior changes and the command is allowed to return the same element multiple times.
        ///     In this case, the number of returned elements is the absolute value of the specified count.
        ///   </para>
        /// </param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The randomly selected elements with scores, or an empty array when <paramref name="key"/> does not exist.</returns>
        /// <remarks><seealso href="https://redis.io/commands/zrandmember"/></remarks>
        public async Task<SortedSetEntry[]> SortedSetRandomMembersWithScoresAsync(RedisKey key, long count, CommandFlags flags = CommandFlags.None)
        {
            return await _db.SortedSetRandomMembersWithScoresAsync(key, count, flags);
        }

        /// <summary>
        /// Returns the specified range of elements in the sorted set stored at key.
        /// By default the elements are considered to be ordered from the lowest to the highest score.
        /// Lexicographical order is used for elements with equal score.
        /// Both start and stop are zero-based indexes, where 0 is the first element, 1 is the next element and so on.
        /// They can also be negative numbers indicating offsets from the end of the sorted set, with -1 being the last element of the sorted set, -2 the penultimate element and so on.
        /// </summary>
        /// <param name="key">The key of the sorted set.</param>
        /// <param name="start">The start index to get.</param>
        /// <param name="stop">The stop index to get.</param>
        /// <param name="order">The order to sort by (defaults to ascending).</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>List of elements in the specified range.</returns>
        /// <remarks>
        /// <seealso href="https://redis.io/commands/zrange"/>,
        /// <seealso href="https://redis.io/commands/zrevrange"/>
        /// </remarks>
        public async Task<RedisValue[]> SortedSetRangeByRankAsync(RedisKey key, long start = 0, long stop = -1, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            return await _db.SortedSetRangeByRankAsync(key, start, stop, order, flags);
        }

        /// <summary>
        /// Takes the specified range of elements in the sorted set of the <paramref name="sourceKey"/>
        /// and stores them in a new sorted set at the <paramref name="destinationKey"/>.
        /// </summary>
        /// <param name="sourceKey">The sorted set to take the range from.</param>
        /// <param name="destinationKey">Where the resulting set will be stored.</param>
        /// <param name="start">The starting point in the sorted set. If <paramref name="sortedSetOrder"/> is <see cref="SortedSetOrder.ByLex"/>, this should be a string.</param>
        /// <param name="stop">The stopping point in the range of the sorted set. If <paramref name="sortedSetOrder"/> is <see cref="SortedSetOrder.ByLex"/>, this should be a string.</param>
        /// <param name="sortedSetOrder">The ordering criteria to use for the range. Choices are <see cref="SortedSetOrder.ByRank"/>, <see cref="SortedSetOrder.ByScore"/>, and <see cref="SortedSetOrder.ByLex"/> (defaults to <see cref="SortedSetOrder.ByRank"/>).</param>
        /// <param name="exclude">Whether to exclude <paramref name="start"/> and <paramref name="stop"/> from the range check (defaults to both inclusive).</param>
        /// <param name="order">
        /// The direction to consider the <paramref name="start"/> and <paramref name="stop"/> in.
        /// If <see cref="Order.Ascending"/>, the <paramref name="start"/> must be smaller than the <paramref name="stop"/>.
        /// If <see cref="Order.Descending"/>, <paramref name="stop"/> must be smaller than <paramref name="start"/>.
        /// </param>
        /// <param name="skip">The number of elements into the sorted set to skip. Note: this iterates after sorting so incurs O(n) cost for large values.</param>
        /// <param name="take">The maximum number of elements to pull into the new (<paramref name="destinationKey"/>) set.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The cardinality of (number of elements in) the newly created sorted set.</returns>
        /// <remarks><seealso href="https://redis.io/commands/zrangestore"/></remarks>
        public async Task<long> SortedSetRangeAndStoreAsync(
            RedisKey sourceKey,
            RedisKey destinationKey,
            RedisValue start,
            RedisValue stop,
            SortedSetOrder sortedSetOrder = SortedSetOrder.ByRank,
            Exclude exclude = Exclude.None,
            Order order = Order.Ascending,
            long skip = 0,
            long? take = null,
            CommandFlags flags = CommandFlags.None)
        {
            return await _db.SortedSetRangeAndStoreAsync(sourceKey, destinationKey, start, stop, sortedSetOrder, exclude, order, skip, take, flags);
        }

        /// <summary>
        /// Returns the specified range of elements in the sorted set stored at key.
        /// By default the elements are considered to be ordered from the lowest to the highest score.
        /// Lexicographical order is used for elements with equal score.
        /// Both start and stop are zero-based indexes, where 0 is the first element, 1 is the next element and so on.
        /// They can also be negative numbers indicating offsets from the end of the sorted set, with -1 being the last element of the sorted set, -2 the penultimate element and so on.
        /// </summary>
        /// <param name="key">The key of the sorted set.</param>
        /// <param name="start">The start index to get.</param>
        /// <param name="stop">The stop index to get.</param>
        /// <param name="order">The order to sort by (defaults to ascending).</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>List of elements in the specified range.</returns>
        /// <remarks>
        /// <seealso href="https://redis.io/commands/zrange"/>,
        /// <seealso href="https://redis.io/commands/zrevrange"/>
        /// </remarks>
        public async Task<SortedSetEntry[]> SortedSetRangeByRankWithScoresAsync(RedisKey key, long start = 0, long stop = -1, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            return await _db.SortedSetRangeByRankWithScoresAsync(key, start, stop, order, flags);
        }

        /// <summary>
        /// Returns the specified range of elements in the sorted set stored at key.
        /// By default the elements are considered to be ordered from the lowest to the highest score.
        /// Lexicographical order is used for elements with equal score.
        /// Start and stop are used to specify the min and max range for score values.
        /// Similar to other range methods the values are inclusive.
        /// </summary>
        /// <param name="key">The key of the sorted set.</param>
        /// <param name="start">The minimum score to filter by.</param>
        /// <param name="stop">The maximum score to filter by.</param>
        /// <param name="exclude">Which of <paramref name="start"/> and <paramref name="stop"/> to exclude (defaults to both inclusive).</param>
        /// <param name="order">The order to sort by (defaults to ascending).</param>
        /// <param name="skip">How many items to skip.</param>
        /// <param name="take">How many items to take.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>List of elements in the specified score range.</returns>
        /// <remarks>
        /// <seealso href="https://redis.io/commands/zrangebyscore"/>,
        /// <seealso href="https://redis.io/commands/zrevrangebyscore"/>
        /// </remarks>
        public async Task<RedisValue[]> SortedSetRangeByScoreAsync(RedisKey key,
            double start = double.NegativeInfinity,
            double stop = double.PositiveInfinity,
            Exclude exclude = Exclude.None,
            Order order = Order.Ascending,
            long skip = 0,
            long take = -1,
            CommandFlags flags = CommandFlags.None)
        {
            return await _db.SortedSetRangeByScoreAsync(key, start, stop, exclude, order, skip, take, flags);
        }

        /// <summary>
        /// Returns the specified range of elements in the sorted set stored at key.
        /// By default the elements are considered to be ordered from the lowest to the highest score.
        /// Lexicographical order is used for elements with equal score.
        /// Start and stop are used to specify the min and max range for score values.
        /// Similar to other range methods the values are inclusive.
        /// </summary>
        /// <param name="key">The key of the sorted set.</param>
        /// <param name="start">The minimum score to filter by.</param>
        /// <param name="stop">The maximum score to filter by.</param>
        /// <param name="exclude">Which of <paramref name="start"/> and <paramref name="stop"/> to exclude (defaults to both inclusive).</param>
        /// <param name="order">The order to sort by (defaults to ascending).</param>
        /// <param name="skip">How many items to skip.</param>
        /// <param name="take">How many items to take.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>List of elements in the specified score range.</returns>
        /// <remarks>
        /// <seealso href="https://redis.io/commands/zrangebyscore"/>,
        /// <seealso href="https://redis.io/commands/zrevrangebyscore"/>
        /// </remarks>
        public async Task<SortedSetEntry[]> SortedSetRangeByScoreWithScoresAsync(RedisKey key,
            double start = double.NegativeInfinity,
            double stop = double.PositiveInfinity,
            Exclude exclude = Exclude.None,
            Order order = Order.Ascending,
            long skip = 0,
            long take = -1,
            CommandFlags flags = CommandFlags.None)
        {
            return await _db.SortedSetRangeByScoreWithScoresAsync(key, start, stop, exclude, order, skip, take, flags);
        }

        /// <summary>
        /// When all the elements in a sorted set are inserted with the same score, in order to force lexicographical ordering.
        /// This command returns all the elements in the sorted set at key with a value between min and max.
        /// </summary>
        /// <param name="key">The key of the sorted set.</param>
        /// <param name="min">The min value to filter by.</param>
        /// <param name="max">The max value to filter by.</param>
        /// <param name="exclude">Which of <paramref name="min"/> and <paramref name="max"/> to exclude (defaults to both inclusive).</param>
        /// <param name="skip">How many items to skip.</param>
        /// <param name="take">How many items to take.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>List of elements in the specified score range.</returns>
        /// <remarks><seealso href="https://redis.io/commands/zrangebylex"/></remarks>
        public async Task<RedisValue[]> SortedSetRangeByValueAsync(RedisKey key,
            RedisValue min,
            RedisValue max,
            Exclude exclude,
            long skip,
            long take = -1,
            CommandFlags flags = CommandFlags.None) // defaults removed to avoid ambiguity with overload with order
        {
            return await SortedSetRangeByValueAsync(key, min, max, exclude, skip, take, flags);
        }

        /// <summary>
        /// When all the elements in a sorted set are inserted with the same score, in order to force lexicographical ordering.
        /// This command returns all the elements in the sorted set at key with a value between min and max.
        /// </summary>
        /// <param name="key">The key of the sorted set.</param>
        /// <param name="min">The min value to filter by.</param>
        /// <param name="max">The max value to filter by.</param>
        /// <param name="exclude">Which of <paramref name="min"/> and <paramref name="max"/> to exclude (defaults to both inclusive).</param>
        /// <param name="order">Whether to order the data ascending or descending</param>
        /// <param name="skip">How many items to skip.</param>
        /// <param name="take">How many items to take.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>List of elements in the specified score range.</returns>
        /// <remarks>
        /// <seealso href="https://redis.io/commands/zrangebylex"/>,
        /// <seealso href="https://redis.io/commands/zrevrangebylex"/>
        /// </remarks>
        public async Task<RedisValue[]> SortedSetRangeByValueAsync(RedisKey key,
            RedisValue min = default,
            RedisValue max = default,
            Exclude exclude = Exclude.None,
            Order order = Order.Ascending,
            long skip = 0,
            long take = -1,
            CommandFlags flags = CommandFlags.None)
        {
            return await _db.SortedSetRangeByValueAsync(key, min, max, exclude, order, skip, take, flags);
        }

        /// <summary>
        /// Returns the rank of member in the sorted set stored at key, by default with the scores ordered from low to high.
        /// The rank (or index) is 0-based, which means that the member with the lowest score has rank 0.
        /// </summary>
        /// <param name="key">The key of the sorted set.</param>
        /// <param name="member">The member to get the rank of.</param>
        /// <param name="order">The order to sort by (defaults to ascending).</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>If member exists in the sorted set, the rank of member. If member does not exist in the sorted set or key does not exist, <see langword="null"/>.</returns>
        /// <remarks>
        /// <seealso href="https://redis.io/commands/zrank"/>,
        /// <seealso href="https://redis.io/commands/zrevrank"/>
        /// </remarks>
        public async Task<long?> SortedSetRankAsync(RedisKey key, RedisValue member, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            return await _db.SortedSetRankAsync(key, member, order, flags);
        }

        /// <summary>
        /// Removes the specified member from the sorted set stored at key. Non existing members are ignored.
        /// </summary>
        /// <param name="key">The key of the sorted set.</param>
        /// <param name="member">The member to remove.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns><see langword="true"/> if the member existed in the sorted set and was removed. <see langword="false"/> otherwise.</returns>
        /// <remarks><seealso href="https://redis.io/commands/zrem"/></remarks>
        public async Task<bool> SortedSetRemoveAsync(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None)
        {
            return await _db.SortedSetRemoveAsync(key, member, flags);
        }

        /// <summary>
        /// Removes the specified members from the sorted set stored at key. Non existing members are ignored.
        /// </summary>
        /// <param name="key">The key of the sorted set.</param>
        /// <param name="members">The members to remove.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The number of members removed from the sorted set, not including non existing members.</returns>
        /// <remarks><seealso href="https://redis.io/commands/zrem"/></remarks>
        public async Task<long> SortedSetRemoveAsync(RedisKey key, RedisValue[] members, CommandFlags flags = CommandFlags.None)
        {
            return await _db.SortedSetRemoveAsync(key, members, flags);
        }

        /// <summary>
        /// Removes all elements in the sorted set stored at key with rank between start and stop.
        /// Both start and stop are 0 -based indexes with 0 being the element with the lowest score.
        /// These indexes can be negative numbers, where they indicate offsets starting at the element with the highest score.
        /// For example: -1 is the element with the highest score, -2 the element with the second highest score and so forth.
        /// </summary>
        /// <param name="key">The key of the sorted set.</param>
        /// <param name="start">The minimum rank to remove.</param>
        /// <param name="stop">The maximum rank to remove.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The number of elements removed.</returns>
        /// <remarks><seealso href="https://redis.io/commands/zremrangebyrank"/></remarks>
        public async Task<long> SortedSetRemoveRangeByRankAsync(RedisKey key, long start, long stop, CommandFlags flags = CommandFlags.None)
        {
            return await _db.SortedSetRemoveRangeByRankAsync(key, start, stop, flags);
        }

        /// <summary>
        /// Removes all elements in the sorted set stored at key with a score between min and max (inclusive by default).
        /// </summary>
        /// <param name="key">The key of the sorted set.</param>
        /// <param name="start">The minimum score to remove.</param>
        /// <param name="stop">The maximum score to remove.</param>
        /// <param name="exclude">Which of <paramref name="start"/> and <paramref name="stop"/> to exclude (defaults to both inclusive).</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The number of elements removed.</returns>
        /// <remarks><seealso href="https://redis.io/commands/zremrangebyscore"/></remarks>
        public async Task<long> SortedSetRemoveRangeByScoreAsync(RedisKey key, double start, double stop, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
            return await _db.SortedSetRemoveRangeByScoreAsync(key, start, stop, exclude, flags);
        }

        /// <summary>
        /// When all the elements in a sorted set are inserted with the same score, in order to force lexicographical ordering.
        /// This command removes all elements in the sorted set stored at key between the lexicographical range specified by min and max.
        /// </summary>
        /// <param name="key">The key of the sorted set.</param>
        /// <param name="min">The minimum value to remove.</param>
        /// <param name="max">The maximum value to remove.</param>
        /// <param name="exclude">Which of <paramref name="min"/> and <paramref name="max"/> to exclude (defaults to both inclusive).</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The number of elements removed.</returns>
        /// <remarks><seealso href="https://redis.io/commands/zremrangebylex"/></remarks>
        public async Task<long> SortedSetRemoveRangeByValueAsync(RedisKey key, RedisValue min, RedisValue max, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
            return await _db.SortedSetRemoveRangeByValueAsync(key, min, max, exclude, flags);
        }

        /// <summary>
        /// Returns the score of member in the sorted set at key.
        /// If member does not exist in the sorted set, or key does not exist, nil is returned.
        /// </summary>
        /// <param name="key">The key of the sorted set.</param>
        /// <param name="member">The member to get a score for.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The score of the member.</returns>
        /// <remarks><seealso href="https://redis.io/commands/zscore"/></remarks>
        public async Task<double?> SortedSetScoreAsync(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None)
        {
            return await _db.SortedSetScoreAsync(key, member, flags);
        }

        /// <summary>
        /// Returns the scores of members in the sorted set at <paramref name="key"/>.
        /// If a member does not exist in the sorted set, or key does not exist, <see langword="null"/> is returned.
        /// </summary>
        /// <param name="key">The key of the sorted set.</param>
        /// <param name="members">The members to get a score for.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>
        /// The scores of the members in the same order as the <paramref name="members"/> array.
        /// If a member does not exist in the set, <see langword="null"/> is returned.
        /// </returns>
        /// <remarks><seealso href="https://redis.io/commands/zmscore"/></remarks>
        public async Task<double?[]> SortedSetScoresAsync(RedisKey key, RedisValue[] members, CommandFlags flags = CommandFlags.None)
        {
            return await _db.SortedSetScoresAsync(key, members, flags);
        }

        /// <summary>
        /// Same as <see cref="SortedSetAddAsync(RedisKey, SortedSetEntry[], SortedSetWhen, CommandFlags)" /> but return the number of the elements changed.
        /// </summary>
        /// <param name="key">The key of the sorted set.</param>
        /// <param name="member">The member to add/update to the sorted set.</param>
        /// <param name="score">The score for the member to add/update to the sorted set.</param>
        /// <param name="when">What conditions to add the element under (defaults to always).</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The number of elements changed.</returns>
        /// <remarks><seealso href="https://redis.io/commands/zadd"/></remarks>
        public async Task<bool> SortedSetUpdateAsync(RedisKey key, RedisValue member, double score, SortedSetWhen when = SortedSetWhen.Always, CommandFlags flags = CommandFlags.None)
        {
            return await _db.SortedSetAddAsync(key, member, score, when, flags);
        }

        /// <summary>
        /// Same as <see cref="SortedSetAddAsync(RedisKey, SortedSetEntry[], SortedSetWhen, CommandFlags)" /> but return the number of the elements changed.
        /// </summary>
        /// <param name="key">The key of the sorted set.</param>
        /// <param name="values">The members and values to add/update to the sorted set.</param>
        /// <param name="when">What conditions to add the element under (defaults to always).</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The number of elements changed.</returns>
        /// <remarks><seealso href="https://redis.io/commands/zadd"/></remarks>
        public async Task<long> SortedSetUpdateAsync(RedisKey key, SortedSetEntry[] values, SortedSetWhen when = SortedSetWhen.Always, CommandFlags flags = CommandFlags.None)
        {
            return await _db.SortedSetAddAsync(key, values, when, flags);
        }

        /// <summary>
        /// Removes and returns the first element from the sorted set stored at key, by default with the scores ordered from low to high.
        /// </summary>
        /// <param name="key">The key of the sorted set.</param>
        /// <param name="order">The order to sort by (defaults to ascending).</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The removed element, or nil when key does not exist.</returns>
        /// <remarks>
        /// <seealso href="https://redis.io/commands/zpopmin"/>,
        /// <seealso href="https://redis.io/commands/zpopmax"/>
        /// </remarks>
        public async Task<SortedSetEntry?> SortedSetPopAsync(RedisKey key, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            return await _db.SortedSetPopAsync(key, order, flags);
        }

        /// <summary>
        /// Removes and returns the specified number of first elements from the sorted set stored at key, by default with the scores ordered from low to high.
        /// </summary>
        /// <param name="key">The key of the sorted set.</param>
        /// <param name="count">The number of elements to return.</param>
        /// <param name="order">The order to sort by (defaults to ascending).</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>An array of elements, or an empty array when key does not exist.</returns>
        /// <remarks>
        /// <seealso href="https://redis.io/commands/zpopmin"/>,
        /// <seealso href="https://redis.io/commands/zpopmax"/>
        /// </remarks>
        public async Task<SortedSetEntry[]> SortedSetPopAsync(RedisKey key, long count, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            return await _db.SortedSetPopAsync(key, count, order, flags);
        }

        /// <summary>
        /// Removes and returns up to <paramref name="count"/> entries from the first non-empty sorted set in <paramref name="keys"/>.
        /// Returns <see cref="SortedSetPopResult.Null"/> if none of the sets exist or contain any elements.
        /// </summary>
        /// <param name="keys">The keys to check.</param>
        /// <param name="count">The maximum number of records to pop out of the sorted set.</param>
        /// <param name="order">The order to sort by when popping items out of the set.</param>
        /// <param name="flags">The flags to use for the operation.</param>
        /// <returns>A contiguous collection of sorted set entries with the key they were popped from, or <see cref="SortedSetPopResult.Null"/> if no non-empty sorted sets are found.</returns>
        /// <remarks><seealso href="https://redis.io/commands/zmpop"/></remarks>
        public async Task<SortedSetPopResult> SortedSetPopAsync(RedisKey[] keys, long count, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            return await _db.SortedSetPopAsync(keys, count, order, flags);
        }

        /// <summary>
        /// 有序集合/定时任务延迟队列用的多
        /// </summary>
        /// <param name="key"></param>
        /// <param name="member">元素</param>
        /// <param name="score">分数</param>
        /// <returns></returns>
        public async Task<bool> SortedSetAddAsync(string key, string member, double score) => await _db.SortedSetAddAsync(key, member, score);

        /// <summary>
        /// Removes the specified members from the sorted set stored at key. Non existing members are ignored.
        /// </summary>
        /// <param name="key">The key of the sorted set.</param>
        /// <param name="members">The members to remove.</param>
        /// <returns>The number of members removed from the sorted set, not including non existing members.</returns>
        /// <remarks><seealso href="https://redis.io/commands/zrem"/></remarks>
        public async Task<long> SortedSetRemoveAsync(string key, IEnumerable<string> members) => await _db.SortedSetRemoveAsync(key, members.ToRedisValues());

        /// <summary>
        /// Increments the score of member in the sorted set stored at key by increment. If member does not exist in the sorted set, it is added with increment as its score (as if its previous score was 0.0).
        /// </summary>
        /// <param name="key">The key of the sorted set.</param>
        /// <param name="member">The member to increment.</param>
        /// <param name="value">The amount to increment by.</param>
        /// <returns>The new score of member.</returns>
        /// <remarks><seealso href="https://redis.io/commands/zincrby"/></remarks>
        public async Task<double> SortedSetIncrementAsync(string key, string member, double value) => await _db.SortedSetIncrementAsync(key, member, value);

        /// <summary>
        /// Decrements the score of member in the sorted set stored at key by decrement.
        /// If member does not exist in the sorted set, it is added with -decrement as its score (as if its previous score was 0.0).
        /// </summary>
        /// <param name="key">The key of the sorted set.</param>
        /// <param name="member">The member to decrement.</param>
        /// <param name="value">The amount to decrement by.</param>
        /// <returns>The new score of member.</returns>
        /// <remarks><seealso href="https://redis.io/commands/zincrby"/></remarks>
        public async Task<double> SortedSetDecrementAsync(string key, string member, double value) => await _db.SortedSetDecrementAsync(key, member, value);

        /// <summary>
        /// 按序返回topN
        /// </summary>
        /// <param name="key"></param>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public async Task<ConcurrentDictionary<string, double>> SortedSetRangeByRankWithScoresAsync(string key,
            long start = 0,
            long stop = -1,
            Order order = Order.Ascending) =>
            (await _db.SortedSetRangeByRankWithScoresAsync(key, start, stop, order)).ToConcurrentDictionary();

        /// <summary>
        /// Returns the specified range of elements in the sorted set stored at key.
        /// By default the elements are considered to be ordered from the lowest to the highest score.
        /// Lexicographical order is used for elements with equal score.
        /// Start and stop are used to specify the min and max range for score values.
        /// Similar to other range methods the values are inclusive.
        /// </summary>
        /// <param name="key">The key of the sorted set.</param>
        /// <param name="start">The minimum score to filter by.</param>
        /// <param name="stop">The maximum score to filter by.</param>
        /// <param name="exclude">Which of <paramref name="start"/> and <paramref name="stop"/> to exclude (defaults to both inclusive).</param>
        /// <param name="order">The order to sort by (defaults to ascending).</param>
        /// <param name="skip">How many items to skip.</param>
        /// <param name="take">How many items to take.</param>
        /// <returns>List of elements in the specified score range.</returns>
        /// <remarks>
        /// <seealso href="https://redis.io/commands/zrangebyscore"/>,
        /// <seealso href="https://redis.io/commands/zrevrangebyscore"/>
        /// </remarks>
        public async Task<ConcurrentDictionary<string, double>> SortedSetRangeByScoreWithScoresAsync(string key,
            double start = double.NegativeInfinity, double stop = double.PositiveInfinity,
            Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1) =>
            (await _db.SortedSetRangeByScoreWithScoresAsync(key, start, stop, exclude, order, skip, take))
            .ToConcurrentDictionary();

        #endregion

        #region Stream 

        /// <summary>
        /// Allow the consumer to mark a pending message as correctly processed. Returns the number of messages acknowledged.
        /// </summary>
        /// <param name="key">The key of the stream.</param>
        /// <param name="groupName">The name of the consumer group that received the message.</param>
        /// <param name="messageId">The ID of the message to acknowledge.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The number of messages acknowledged.</returns>
        /// <remarks><seealso href="https://redis.io/topics/streams-intro"/></remarks>
        public  long StreamAcknowledge(RedisKey key, RedisValue groupName, RedisValue messageId, CommandFlags flags = CommandFlags.None)
        {
            return  _db.StreamAcknowledge(key, groupName, messageId, flags);
        }

        /// <summary>
        /// Allow the consumer to mark a pending message as correctly processed. Returns the number of messages acknowledged.
        /// </summary>
        /// <param name="key">The key of the stream.</param>
        /// <param name="groupName">The name of the consumer group that received the message.</param>
        /// <param name="messageIds">The IDs of the messages to acknowledge.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The number of messages acknowledged.</returns>
        /// <remarks><seealso href="https://redis.io/topics/streams-intro"/></remarks>
        public  long StreamAcknowledge(RedisKey key, RedisValue groupName, RedisValue[] messageIds, CommandFlags flags = CommandFlags.None)
        {
            return  _db.StreamAcknowledge(key, groupName, messageIds, flags);
        }

        /// <summary>
        /// Adds an entry using the specified values to the given stream key.
        /// If key does not exist, a new key holding a stream is created.
        /// The command returns the ID of the newly created stream entry.
        /// </summary>
        /// <param name="key">The key of the stream.</param>
        /// <param name="streamField">The field name for the stream entry.</param>
        /// <param name="streamValue">The value to set in the stream entry.</param>
        /// <param name="messageId">The ID to assign to the stream entry, defaults to an auto-generated ID ("*").</param>
        /// <param name="maxLength">The maximum length of the stream.</param>
        /// <param name="useApproximateMaxLength">If true, the "~" argument is used to allow the stream to exceed max length by a small number. This improves performance when removing messages.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The ID of the newly created message.</returns>
        /// <remarks><seealso href="https://redis.io/commands/xadd"/></remarks>
        public  RedisValue StreamAdd(RedisKey key, RedisValue streamField, RedisValue streamValue, RedisValue? messageId = null, int? maxLength = null, bool useApproximateMaxLength = false, CommandFlags flags = CommandFlags.None)
        {
            return  _db.StreamAdd(key, streamField, streamValue, messageId, maxLength, useApproximateMaxLength, flags);
        }

        /// <summary>
        /// Adds an entry using the specified values to the given stream key.
        /// If key does not exist, a new key holding a stream is created.
        /// The command returns the ID of the newly created stream entry.
        /// </summary>
        /// <param name="key">The key of the stream.</param>
        /// <param name="streamPairs">The fields and their associated values to set in the stream entry.</param>
        /// <param name="messageId">The ID to assign to the stream entry, defaults to an auto-generated ID ("*").</param>
        /// <param name="maxLength">The maximum length of the stream.</param>
        /// <param name="useApproximateMaxLength">If true, the "~" argument is used to allow the stream to exceed max length by a small number. This improves performance when removing messages.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The ID of the newly created message.</returns>
        /// <remarks><seealso href="https://redis.io/commands/xadd"/></remarks>
        public  RedisValue StreamAdd(RedisKey key, NameValueEntry[] streamPairs, RedisValue? messageId = null, int? maxLength = null, bool useApproximateMaxLength = false, CommandFlags flags = CommandFlags.None)
        {
            return  _db.StreamAdd(key, streamPairs, messageId, maxLength, useApproximateMaxLength, flags);
        }

        /// <summary>
        /// Change ownership of messages consumed, but not yet acknowledged, by a different consumer.
        /// Messages that have been idle for more than <paramref name="minIdleTimeInMs"/> will be claimed.
        /// </summary>
        /// <param name="key">The key of the stream.</param>
        /// <param name="consumerGroup">The consumer group.</param>
        /// <param name="claimingConsumer">The consumer claiming the messages(s).</param>
        /// <param name="minIdleTimeInMs">The minimum idle time threshold for pending messages to be claimed.</param>
        /// <param name="startAtId">The starting ID to scan for pending messages that have an idle time greater than <paramref name="minIdleTimeInMs"/>.</param>
        /// <param name="count">The upper limit of the number of entries that the command attempts to claim. If <see langword="null"/>, Redis will default the value to 100.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>An instance of <see cref="StreamAutoClaimResult"/>.</returns>
        /// <remarks><seealso href="https://redis.io/commands/xautoclaim"/></remarks>
        public  StreamAutoClaimResult StreamAutoClaim(RedisKey key, RedisValue consumerGroup, RedisValue claimingConsumer, long minIdleTimeInMs, RedisValue startAtId, int? count = null, CommandFlags flags = CommandFlags.None)
        {
            return  _db.StreamAutoClaim(key, consumerGroup, claimingConsumer, minIdleTimeInMs, startAtId, count, flags);
        }

        /// <summary>
        /// Change ownership of messages consumed, but not yet acknowledged, by a different consumer.
        /// Messages that have been idle for more than <paramref name="minIdleTimeInMs"/> will be claimed.
        /// The result will contain the claimed message IDs instead of a <see cref="StreamEntry"/> instance.
        /// </summary>
        /// <param name="key">The key of the stream.</param>
        /// <param name="consumerGroup">The consumer group.</param>
        /// <param name="claimingConsumer">The consumer claiming the messages(s).</param>
        /// <param name="minIdleTimeInMs">The minimum idle time threshold for pending messages to be claimed.</param>
        /// <param name="startAtId">The starting ID to scan for pending messages that have an idle time greater than <paramref name="minIdleTimeInMs"/>.</param>
        /// <param name="count">The upper limit of the number of entries that the command attempts to claim. If <see langword="null"/>, Redis will default the value to 100.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>An instance of <see cref="StreamAutoClaimIdsOnlyResult"/>.</returns>
        /// <remarks><seealso href="https://redis.io/commands/xautoclaim"/></remarks>
        public  StreamAutoClaimIdsOnlyResult StreamAutoClaimIdsOnly(RedisKey key, RedisValue consumerGroup, RedisValue claimingConsumer, long minIdleTimeInMs, RedisValue startAtId, int? count = null, CommandFlags flags = CommandFlags.None)
        {
            return  _db.StreamAutoClaimIdsOnly(key, consumerGroup, claimingConsumer, minIdleTimeInMs, startAtId, count, flags);
        }

        /// <summary>
        /// Change ownership of messages consumed, but not yet acknowledged, by a different consumer.
        /// This method returns the complete message for the claimed message(s).
        /// </summary>
        /// <param name="key">The key of the stream.</param>
        /// <param name="consumerGroup">The consumer group.</param>
        /// <param name="claimingConsumer">The consumer claiming the given message(s).</param>
        /// <param name="minIdleTimeInMs">The minimum message idle time to allow the reassignment of the message(s).</param>
        /// <param name="messageIds">The IDs of the messages to claim for the given consumer.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The messages successfully claimed by the given consumer.</returns>
        /// <remarks><seealso href="https://redis.io/topics/streams-intro"/></remarks>
        public  StreamEntry[] StreamClaim(RedisKey key, RedisValue consumerGroup, RedisValue claimingConsumer, long minIdleTimeInMs, RedisValue[] messageIds, CommandFlags flags = CommandFlags.None)
        {
            return  _db.StreamClaim(key, consumerGroup, claimingConsumer, minIdleTimeInMs, messageIds, flags);
        }

        /// <summary>
        /// Change ownership of messages consumed, but not yet acknowledged, by a different consumer.
        /// This method returns the IDs for the claimed message(s).
        /// </summary>
        /// <param name="key">The key of the stream.</param>
        /// <param name="consumerGroup">The consumer group.</param>
        /// <param name="claimingConsumer">The consumer claiming the given message(s).</param>
        /// <param name="minIdleTimeInMs">The minimum message idle time to allow the reassignment of the message(s).</param>
        /// <param name="messageIds">The IDs of the messages to claim for the given consumer.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The message IDs for the messages successfully claimed by the given consumer.</returns>
        /// <remarks><seealso href="https://redis.io/topics/streams-intro"/></remarks>
        public  RedisValue[] StreamClaimIdsOnly(RedisKey key, RedisValue consumerGroup, RedisValue claimingConsumer, long minIdleTimeInMs, RedisValue[] messageIds, CommandFlags flags = CommandFlags.None)
        {
            return  _db.StreamClaimIdsOnly(key, consumerGroup, claimingConsumer, minIdleTimeInMs, messageIds, flags);
        }

        /// <summary>
        /// Set the position from which to read a stream for a consumer group.
        /// </summary>
        /// <param name="key">The key of the stream.</param>
        /// <param name="groupName">The name of the consumer group.</param>
        /// <param name="position">The position from which to read for the consumer group.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns><see langword="true"/> if successful, <see langword="false"/> otherwise.</returns>
        /// <remarks><seealso href="https://redis.io/topics/streams-intro"/></remarks>
        public  bool StreamConsumerGroupSetPosition(RedisKey key, RedisValue groupName, RedisValue position, CommandFlags flags = CommandFlags.None)
        {
            return  _db.StreamConsumerGroupSetPosition(key, groupName, position, flags);
        }

        /// <summary>
        /// Retrieve information about the consumers for the given consumer group.
        /// This is the equivalent of calling "XINFO GROUPS key group".
        /// </summary>
        /// <param name="key">The key of the stream.</param>
        /// <param name="groupName">The consumer group name.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>An instance of <see cref="StreamConsumerInfo"/> for each of the consumer group's consumers.</returns>
        /// <remarks><seealso href="https://redis.io/topics/streams-intro"/></remarks>
        public  StreamConsumerInfo[] StreamConsumerInfo(RedisKey key, RedisValue groupName, CommandFlags flags = CommandFlags.None)
        {
            return  _db.StreamConsumerInfo(key, groupName, flags);
        }

        /// <summary>
        /// Create a consumer group for the given stream.
        /// </summary>
        /// <param name="key">The key of the stream.</param>
        /// <param name="groupName">The name of the group to create.</param>
        /// <param name="position">The position to begin reading the stream. Defaults to <see cref="StreamPosition.NewMessages"/>.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns><see langword="true"/> if the group was created, <see langword="false"/> otherwise.</returns>
        /// <remarks><seealso href="https://redis.io/topics/streams-intro"/></remarks>
        public  bool StreamCreateConsumerGroup(RedisKey key, RedisValue groupName, RedisValue? position, CommandFlags flags)
        {
            return  _db.StreamCreateConsumerGroup(key, groupName, position, flags);
        }

        /// <summary>
        /// Create a consumer group for the given stream.
        /// </summary>
        /// <param name="key">The key of the stream.</param>
        /// <param name="groupName">The name of the group to create.</param>
        /// <param name="position">The position to begin reading the stream. Defaults to <see cref="StreamPosition.NewMessages"/>.</param>
        /// <param name="createStream">Create the stream if it does not already exist.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns><see langword="true"/> if the group was created, <see langword="false"/> otherwise.</returns>
        /// <remarks><seealso href="https://redis.io/topics/streams-intro"/></remarks>
        public  bool StreamCreateConsumerGroup(RedisKey key, RedisValue groupName, RedisValue? position = null, bool createStream = true, CommandFlags flags = CommandFlags.None)
        {
            return  _db.StreamCreateConsumerGroup(key, groupName, position, createStream, flags);
        }

        /// <summary>
        /// Delete messages in the stream. This method does not delete the stream.
        /// </summary>
        /// <param name="key">The key of the stream.</param>
        /// <param name="messageIds">The IDs of the messages to delete.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>Returns the number of messages successfully deleted from the stream.</returns>
        /// <remarks><seealso href="https://redis.io/topics/streams-intro"/></remarks>
        public  long StreamDelete(RedisKey key, RedisValue[] messageIds, CommandFlags flags = CommandFlags.None)
        {
            return  _db.StreamDelete(key, messageIds, flags);
        }

        /// <summary>
        /// Delete a consumer from a consumer group.
        /// </summary>
        /// <param name="key">The key of the stream.</param>
        /// <param name="groupName">The name of the consumer group.</param>
        /// <param name="consumerName">The name of the consumer.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The number of messages that were pending for the deleted consumer.</returns>
        /// <remarks><seealso href="https://redis.io/topics/streams-intro"/></remarks>
        public  long StreamDeleteConsumer(RedisKey key, RedisValue groupName, RedisValue consumerName, CommandFlags flags = CommandFlags.None)
        {
            return  _db.StreamDeleteConsumer(key, groupName, consumerName, flags);
        }

        /// <summary>
        /// Delete a consumer group.
        /// </summary>
        /// <param name="key">The key of the stream.</param>
        /// <param name="groupName">The name of the consumer group.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns><see langword="true"/> if deleted, <see langword="false"/> otherwise.</returns>
        /// <remarks><seealso href="https://redis.io/topics/streams-intro"/></remarks>
        public  bool StreamDeleteConsumerGroup(RedisKey key, RedisValue groupName, CommandFlags flags = CommandFlags.None)
        {
            return  _db.StreamDeleteConsumerGroup(key, groupName, flags);
        }

        /// <summary>
        /// Retrieve information about the groups created for the given stream. This is the equivalent of calling "XINFO GROUPS key".
        /// </summary>
        /// <param name="key">The key of the stream.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>An instance of <see cref="StreamGroupInfo"/> for each of the stream's groups.</returns>
        /// <remarks><seealso href="https://redis.io/topics/streams-intro"/></remarks>
        public  StreamGroupInfo[] StreamGroupInfo(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return  _db.StreamGroupInfo(key, flags);
        }

        /// <summary>
        /// Retrieve information about the given stream. This is the equivalent of calling "XINFO STREAM key".
        /// </summary>
        /// <param name="key">The key of the stream.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>A <see cref="StreamInfo"/> instance with information about the stream.</returns>
        /// <remarks><seealso href="https://redis.io/topics/streams-intro"/></remarks>
        public  StreamInfo StreamInfo(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return  _db.StreamInfo(key, flags);
        }

        /// <summary>
        /// Return the number of entries in a stream.
        /// </summary>
        /// <param name="key">The key of the stream.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The number of entries inside the given stream.</returns>
        /// <remarks><seealso href="https://redis.io/commands/xlen"/></remarks>
        public  long StreamLength(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return  _db.StreamLength(key, flags);
        }

        /// <summary>
        /// View information about pending messages for a stream.
        /// A pending message is a message read using StreamReadGroup (XREADGROUP) but not yet acknowledged.
        /// </summary>
        /// <param name="key">The key of the stream.</param>
        /// <param name="groupName">The name of the consumer group</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>
        /// An instance of <see cref="StreamPendingInfo"/>.
        /// <see cref="StreamPendingInfo"/> contains the number of pending messages.
        /// The highest and lowest ID of the pending messages, and the consumers with their pending message count.
        /// </returns>
        /// <remarks>The equivalent of calling XPENDING key group.</remarks>
        /// <remarks><seealso href="https://redis.io/commands/xpending"/></remarks>
        public  StreamPendingInfo StreamPending(RedisKey key, RedisValue groupName, CommandFlags flags = CommandFlags.None)
        {
            return  _db.StreamPending(key, groupName, flags);
        }

        /// <summary>
        /// View information about each pending message.
        /// </summary>
        /// <param name="key">The key of the stream.</param>
        /// <param name="groupName">The name of the consumer group.</param>
        /// <param name="count">The maximum number of pending messages to return.</param>
        /// <param name="consumerName">The consumer name for the pending messages. Pass RedisValue.Null to include pending messages for all consumers.</param>
        /// <param name="minId">The minimum ID from which to read the stream of pending messages. The method will default to reading from the beginning of the stream.</param>
        /// <param name="maxId">The maximum ID to read to within the stream of pending messages. The method will default to reading to the end of the stream.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>An instance of <see cref="StreamPendingMessageInfo"/> for each pending message.</returns>
        /// <remarks>Equivalent of calling XPENDING key group start-id end-id count consumer-name.</remarks>
        /// <remarks><seealso href="https://redis.io/commands/xpending"/></remarks>
        public  StreamPendingMessageInfo[] StreamPendingMessages(RedisKey key, RedisValue groupName, int count, RedisValue consumerName, RedisValue? minId = null, RedisValue? maxId = null, CommandFlags flags = CommandFlags.None)
        {
            return  _db.StreamPendingMessages(key, groupName, count, consumerName, minId, maxId, flags);
        }

        /// <summary>
        /// Read a stream using the given range of IDs.
        /// </summary>
        /// <param name="key">The key of the stream.</param>
        /// <param name="minId">The minimum ID from which to read the stream. The method will default to reading from the beginning of the stream.</param>
        /// <param name="maxId">The maximum ID to read to within the stream. The method will default to reading to the end of the stream.</param>
        /// <param name="count">The maximum number of messages to return.</param>
        /// <param name="messageOrder">The order of the messages. <see cref="Order.Ascending"/> will execute XRANGE and <see cref="Order.Descending"/> will execute XREVRANGE.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>Returns an instance of <see cref="StreamEntry"/> for each message returned.</returns>
        /// <remarks><seealso href="https://redis.io/commands/xrange"/></remarks>
        public  StreamEntry[] StreamRange(RedisKey key, RedisValue? minId = null, RedisValue? maxId = null, int? count = null, Order messageOrder = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            return  _db.StreamRange(key, minId, maxId, count, messageOrder, flags);
        }

        /// <summary>
        /// Read from a single stream.
        /// </summary>
        /// <param name="key">The key of the stream.</param>
        /// <param name="position">The position from which to read the stream.</param>
        /// <param name="count">The maximum number of messages to return.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>Returns an instance of <see cref="StreamEntry"/> for each message returned.</returns>
        /// <remarks>
        /// <para>Equivalent of calling <c>XREAD COUNT num STREAMS key id</c>.</para>
        /// <para><seealso href="https://redis.io/commands/xread"/></para>
        /// </remarks>
        public  StreamEntry[] StreamRead(RedisKey key, RedisValue position, int? count = null, CommandFlags flags = CommandFlags.None)
        {
            return  _db.StreamRead(key, position, count, flags);
        }

        /// <summary>
        /// Read from multiple streams.
        /// </summary>
        /// <param name="streamPositions">Array of streams and the positions from which to begin reading for each stream.</param>
        /// <param name="countPerStream">The maximum number of messages to return from each stream.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>A value of <see cref="RedisStream"/> for each stream.</returns>
        /// <remarks>
        /// <para>Equivalent of calling <c>XREAD COUNT num STREAMS key1 key2 id1 id2</c>.</para>
        /// <para><seealso href="https://redis.io/commands/xread"/></para>
        /// </remarks>
        public  RedisStream[] StreamRead(StreamPosition[] streamPositions, int? countPerStream = null, CommandFlags flags = CommandFlags.None)
        {
            return  _db.StreamRead(streamPositions, countPerStream, flags);
        }

        /// <summary>
        /// Read messages from a stream into an associated consumer group.
        /// </summary>
        /// <param name="key">The key of the stream.</param>
        /// <param name="groupName">The name of the consumer group.</param>
        /// <param name="consumerName">The consumer name.</param>
        /// <param name="position">The position from which to read the stream. Defaults to <see cref="StreamPosition.NewMessages"/> when <see langword="null"/>.</param>
        /// <param name="count">The maximum number of messages to return.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>Returns a value of <see cref="StreamEntry"/> for each message returned.</returns>
        /// <remarks><seealso href="https://redis.io/commands/xreadgroup"/></remarks>
        public  StreamEntry[] StreamReadGroup(RedisKey key, RedisValue groupName, RedisValue consumerName, RedisValue? position, int? count, CommandFlags flags)
        {
            return  _db.StreamReadGroup(key, groupName, consumerName, position, count, flags);
        }

        /// <summary>
        /// Read messages from a stream into an associated consumer group.
        /// </summary>
        /// <param name="key">The key of the stream.</param>
        /// <param name="groupName">The name of the consumer group.</param>
        /// <param name="consumerName">The consumer name.</param>
        /// <param name="position">The position from which to read the stream. Defaults to <see cref="StreamPosition.NewMessages"/> when <see langword="null"/>.</param>
        /// <param name="count">The maximum number of messages to return.</param>
        /// <param name="noAck">When true, the message will not be added to the pending message list.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>Returns a value of <see cref="StreamEntry"/> for each message returned.</returns>
        /// <remarks><seealso href="https://redis.io/commands/xreadgroup"/></remarks>
        public  StreamEntry[] StreamReadGroup(RedisKey key, RedisValue groupName, RedisValue consumerName, RedisValue? position = null, int? count = null, bool noAck = false, CommandFlags flags = CommandFlags.None)
        {
            return  _db.StreamReadGroup(key, groupName, consumerName, position, count, noAck, flags);
        }

        /// <summary>
        /// Read from multiple streams into the given consumer group.
        /// The consumer group with the given <paramref name="groupName"/> will need to have been created for each stream prior to calling this method.
        /// </summary>
        /// <param name="streamPositions">Array of streams and the positions from which to begin reading for each stream.</param>
        /// <param name="groupName">The name of the consumer group.</param>
        /// <param name="consumerName"></param>
        /// <param name="countPerStream">The maximum number of messages to return from each stream.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>A value of <see cref="RedisStream"/> for each stream.</returns>
        /// <remarks>
        /// <para>Equivalent of calling <c>XREADGROUP GROUP groupName consumerName COUNT countPerStream STREAMS stream1 stream2 id1 id2</c>.</para>
        /// <para><seealso href="https://redis.io/commands/xreadgroup"/></para>
        /// </remarks>
        public  RedisStream[] StreamReadGroup(StreamPosition[] streamPositions, RedisValue groupName, RedisValue consumerName, int? countPerStream, CommandFlags flags)
        {
            return  _db.StreamReadGroup(streamPositions, groupName, consumerName, countPerStream, flags);
        }

        /// <summary>
        /// Read from multiple streams into the given consumer group.
        /// The consumer group with the given <paramref name="groupName"/> will need to have been created for each stream prior to calling this method.
        /// </summary>
        /// <param name="streamPositions">Array of streams and the positions from which to begin reading for each stream.</param>
        /// <param name="groupName">The name of the consumer group.</param>
        /// <param name="consumerName"></param>
        /// <param name="countPerStream">The maximum number of messages to return from each stream.</param>
        /// <param name="noAck">When true, the message will not be added to the pending message list.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>A value of <see cref="RedisStream"/> for each stream.</returns>
        /// <remarks>
        /// <para>Equivalent of calling <c>XREADGROUP GROUP groupName consumerName COUNT countPerStream STREAMS stream1 stream2 id1 id2</c>.</para>
        /// <para><seealso href="https://redis.io/commands/xreadgroup"/></para>
        /// </remarks>
        public  RedisStream[] StreamReadGroup(StreamPosition[] streamPositions, RedisValue groupName, RedisValue consumerName, int? countPerStream = null, bool noAck = false, CommandFlags flags = CommandFlags.None)
        {
            return  _db.StreamReadGroup(streamPositions, groupName, consumerName, countPerStream, noAck, flags);
        }

        /// <summary>
        /// Trim the stream to a specified maximum length.
        /// </summary>
        /// <param name="key">The key of the stream.</param>
        /// <param name="maxLength">The maximum length of the stream.</param>
        /// <param name="useApproximateMaxLength">If true, the "~" argument is used to allow the stream to exceed max length by a small number. This improves performance when removing messages.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The number of messages removed from the stream.</returns>
        /// <remarks><seealso href="https://redis.io/topics/streams-intro"/></remarks>
        public  long StreamTrim(RedisKey key, int maxLength, bool useApproximateMaxLength = false, CommandFlags flags = CommandFlags.None)
        {
            return  _db.StreamTrim(key, maxLength, useApproximateMaxLength, flags);
        }

        #endregion

        #region Stream Async

        /// <summary>
        /// Allow the consumer to mark a pending message as correctly processed. Returns the number of messages acknowledged.
        /// </summary>
        /// <param name="key">The key of the stream.</param>
        /// <param name="groupName">The name of the consumer group that received the message.</param>
        /// <param name="messageId">The ID of the message to acknowledge.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The number of messages acknowledged.</returns>
        /// <remarks><seealso href="https://redis.io/topics/streams-intro"/></remarks>
        public async Task<long> StreamAcknowledgeAsync(RedisKey key, RedisValue groupName, RedisValue messageId, CommandFlags flags = CommandFlags.None)
        {
            return await _db.StreamAcknowledgeAsync(key, groupName, messageId, flags);
        }

        /// <summary>
        /// Allow the consumer to mark a pending message as correctly processed. Returns the number of messages acknowledged.
        /// </summary>
        /// <param name="key">The key of the stream.</param>
        /// <param name="groupName">The name of the consumer group that received the message.</param>
        /// <param name="messageIds">The IDs of the messages to acknowledge.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The number of messages acknowledged.</returns>
        /// <remarks><seealso href="https://redis.io/topics/streams-intro"/></remarks>
        public async Task<long> StreamAcknowledgeAsync(RedisKey key, RedisValue groupName, RedisValue[] messageIds, CommandFlags flags = CommandFlags.None)
        {
            return await _db.StreamAcknowledgeAsync(key, groupName, messageIds, flags);
        }

        /// <summary>
        /// Adds an entry using the specified values to the given stream key.
        /// If key does not exist, a new key holding a stream is created.
        /// The command returns the ID of the newly created stream entry.
        /// </summary>
        /// <param name="key">The key of the stream.</param>
        /// <param name="streamField">The field name for the stream entry.</param>
        /// <param name="streamValue">The value to set in the stream entry.</param>
        /// <param name="messageId">The ID to assign to the stream entry, defaults to an auto-generated ID ("*").</param>
        /// <param name="maxLength">The maximum length of the stream.</param>
        /// <param name="useApproximateMaxLength">If true, the "~" argument is used to allow the stream to exceed max length by a small number. This improves performance when removing messages.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The ID of the newly created message.</returns>
        /// <remarks><seealso href="https://redis.io/commands/xadd"/></remarks>
        public async Task<RedisValue> StreamAddAsync(RedisKey key, RedisValue streamField, RedisValue streamValue, RedisValue? messageId = null, int? maxLength = null, bool useApproximateMaxLength = false, CommandFlags flags = CommandFlags.None)
        {
            return await _db.StreamAddAsync(key, streamField, streamValue, messageId, maxLength, useApproximateMaxLength, flags);
        }

        /// <summary>
        /// Adds an entry using the specified values to the given stream key.
        /// If key does not exist, a new key holding a stream is created.
        /// The command returns the ID of the newly created stream entry.
        /// </summary>
        /// <param name="key">The key of the stream.</param>
        /// <param name="streamPairs">The fields and their associated values to set in the stream entry.</param>
        /// <param name="messageId">The ID to assign to the stream entry, defaults to an auto-generated ID ("*").</param>
        /// <param name="maxLength">The maximum length of the stream.</param>
        /// <param name="useApproximateMaxLength">If true, the "~" argument is used to allow the stream to exceed max length by a small number. This improves performance when removing messages.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The ID of the newly created message.</returns>
        /// <remarks><seealso href="https://redis.io/commands/xadd"/></remarks>
        public async Task<RedisValue> StreamAddAsync(RedisKey key, NameValueEntry[] streamPairs, RedisValue? messageId = null, int? maxLength = null, bool useApproximateMaxLength = false, CommandFlags flags = CommandFlags.None)
        {
            return await _db.StreamAddAsync(key, streamPairs, messageId, maxLength, useApproximateMaxLength, flags);
        }

        /// <summary>
        /// Change ownership of messages consumed, but not yet acknowledged, by a different consumer.
        /// Messages that have been idle for more than <paramref name="minIdleTimeInMs"/> will be claimed.
        /// </summary>
        /// <param name="key">The key of the stream.</param>
        /// <param name="consumerGroup">The consumer group.</param>
        /// <param name="claimingConsumer">The consumer claiming the messages(s).</param>
        /// <param name="minIdleTimeInMs">The minimum idle time threshold for pending messages to be claimed.</param>
        /// <param name="startAtId">The starting ID to scan for pending messages that have an idle time greater than <paramref name="minIdleTimeInMs"/>.</param>
        /// <param name="count">The upper limit of the number of entries that the command attempts to claim. If <see langword="null"/>, Redis will default the value to 100.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>An instance of <see cref="StreamAutoClaimResult"/>.</returns>
        /// <remarks><seealso href="https://redis.io/commands/xautoclaim"/></remarks>
        public async Task<StreamAutoClaimResult> StreamAutoClaimAsync(RedisKey key, RedisValue consumerGroup, RedisValue claimingConsumer, long minIdleTimeInMs, RedisValue startAtId, int? count = null, CommandFlags flags = CommandFlags.None)
        {
            return await _db.StreamAutoClaimAsync(key, consumerGroup, claimingConsumer, minIdleTimeInMs, startAtId, count, flags);
        }

        /// <summary>
        /// Change ownership of messages consumed, but not yet acknowledged, by a different consumer.
        /// Messages that have been idle for more than <paramref name="minIdleTimeInMs"/> will be claimed.
        /// The result will contain the claimed message IDs instead of a <see cref="StreamEntry"/> instance.
        /// </summary>
        /// <param name="key">The key of the stream.</param>
        /// <param name="consumerGroup">The consumer group.</param>
        /// <param name="claimingConsumer">The consumer claiming the messages(s).</param>
        /// <param name="minIdleTimeInMs">The minimum idle time threshold for pending messages to be claimed.</param>
        /// <param name="startAtId">The starting ID to scan for pending messages that have an idle time greater than <paramref name="minIdleTimeInMs"/>.</param>
        /// <param name="count">The upper limit of the number of entries that the command attempts to claim. If <see langword="null"/>, Redis will default the value to 100.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>An instance of <see cref="StreamAutoClaimIdsOnlyResult"/>.</returns>
        /// <remarks><seealso href="https://redis.io/commands/xautoclaim"/></remarks>
        public async Task<StreamAutoClaimIdsOnlyResult> StreamAutoClaimIdsOnlyAsync(RedisKey key, RedisValue consumerGroup, RedisValue claimingConsumer, long minIdleTimeInMs, RedisValue startAtId, int? count = null, CommandFlags flags = CommandFlags.None)
        {
            return await _db.StreamAutoClaimIdsOnlyAsync(key, consumerGroup, claimingConsumer, minIdleTimeInMs, startAtId, count, flags);
        }

        /// <summary>
        /// Change ownership of messages consumed, but not yet acknowledged, by a different consumer.
        /// This method returns the complete message for the claimed message(s).
        /// </summary>
        /// <param name="key">The key of the stream.</param>
        /// <param name="consumerGroup">The consumer group.</param>
        /// <param name="claimingConsumer">The consumer claiming the given message(s).</param>
        /// <param name="minIdleTimeInMs">The minimum message idle time to allow the reassignment of the message(s).</param>
        /// <param name="messageIds">The IDs of the messages to claim for the given consumer.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The messages successfully claimed by the given consumer.</returns>
        /// <remarks><seealso href="https://redis.io/topics/streams-intro"/></remarks>
        public async Task<StreamEntry[]> StreamClaimAsync(RedisKey key, RedisValue consumerGroup, RedisValue claimingConsumer, long minIdleTimeInMs, RedisValue[] messageIds, CommandFlags flags = CommandFlags.None)
        {
            return await _db.StreamClaimAsync(key, consumerGroup, claimingConsumer, minIdleTimeInMs, messageIds, flags);
        }

        /// <summary>
        /// Change ownership of messages consumed, but not yet acknowledged, by a different consumer.
        /// This method returns the IDs for the claimed message(s).
        /// </summary>
        /// <param name="key">The key of the stream.</param>
        /// <param name="consumerGroup">The consumer group.</param>
        /// <param name="claimingConsumer">The consumer claiming the given message(s).</param>
        /// <param name="minIdleTimeInMs">The minimum message idle time to allow the reassignment of the message(s).</param>
        /// <param name="messageIds">The IDs of the messages to claim for the given consumer.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The message IDs for the messages successfully claimed by the given consumer.</returns>
        /// <remarks><seealso href="https://redis.io/topics/streams-intro"/></remarks>
        public async Task<RedisValue[]> StreamClaimIdsOnlyAsync(RedisKey key, RedisValue consumerGroup, RedisValue claimingConsumer, long minIdleTimeInMs, RedisValue[] messageIds, CommandFlags flags = CommandFlags.None)
        {
            return await _db.StreamClaimIdsOnlyAsync(key, consumerGroup, claimingConsumer, minIdleTimeInMs, messageIds, flags);
        }

        /// <summary>
        /// Set the position from which to read a stream for a consumer group.
        /// </summary>
        /// <param name="key">The key of the stream.</param>
        /// <param name="groupName">The name of the consumer group.</param>
        /// <param name="position">The position from which to read for the consumer group.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns><see langword="true"/> if successful, <see langword="false"/> otherwise.</returns>
        /// <remarks><seealso href="https://redis.io/topics/streams-intro"/></remarks>
        public async Task<bool> StreamConsumerGroupSetPositionAsync(RedisKey key, RedisValue groupName, RedisValue position, CommandFlags flags = CommandFlags.None)
        {
            return await _db.StreamConsumerGroupSetPositionAsync(key, groupName, position, flags);
        }

        /// <summary>
        /// Retrieve information about the consumers for the given consumer group.
        /// This is the equivalent of calling "XINFO GROUPS key group".
        /// </summary>
        /// <param name="key">The key of the stream.</param>
        /// <param name="groupName">The consumer group name.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>An instance of <see cref="StreamConsumerInfo"/> for each of the consumer group's consumers.</returns>
        /// <remarks><seealso href="https://redis.io/topics/streams-intro"/></remarks>
        public async Task<StreamConsumerInfo[]> StreamConsumerInfoAsync(RedisKey key, RedisValue groupName, CommandFlags flags = CommandFlags.None)
        {
            return await _db.StreamConsumerInfoAsync(key, groupName, flags);
        }

        /// <summary>
        /// Create a consumer group for the given stream.
        /// </summary>
        /// <param name="key">The key of the stream.</param>
        /// <param name="groupName">The name of the group to create.</param>
        /// <param name="position">The position to begin reading the stream. Defaults to <see cref="StreamPosition.NewMessages"/>.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns><see langword="true"/> if the group was created, <see langword="false"/> otherwise.</returns>
        /// <remarks><seealso href="https://redis.io/topics/streams-intro"/></remarks>
        public async Task<bool> StreamCreateConsumerGroupAsync(RedisKey key, RedisValue groupName, RedisValue? position, CommandFlags flags)
        {
            return await _db.StreamCreateConsumerGroupAsync(key, groupName, position, flags);
        }

        /// <summary>
        /// Create a consumer group for the given stream.
        /// </summary>
        /// <param name="key">The key of the stream.</param>
        /// <param name="groupName">The name of the group to create.</param>
        /// <param name="position">The position to begin reading the stream. Defaults to <see cref="StreamPosition.NewMessages"/>.</param>
        /// <param name="createStream">Create the stream if it does not already exist.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns><see langword="true"/> if the group was created, <see langword="false"/> otherwise.</returns>
        /// <remarks><seealso href="https://redis.io/topics/streams-intro"/></remarks>
        public async Task<bool> StreamCreateConsumerGroupAsync(RedisKey key, RedisValue groupName, RedisValue? position = null, bool createStream = true, CommandFlags flags = CommandFlags.None)
        {
            return await _db.StreamCreateConsumerGroupAsync(key, groupName, position, createStream, flags);
        }

        /// <summary>
        /// Delete messages in the stream. This method does not delete the stream.
        /// </summary>
        /// <param name="key">The key of the stream.</param>
        /// <param name="messageIds">The IDs of the messages to delete.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>Returns the number of messages successfully deleted from the stream.</returns>
        /// <remarks><seealso href="https://redis.io/topics/streams-intro"/></remarks>
        public async Task<long> StreamDeleteAsync(RedisKey key, RedisValue[] messageIds, CommandFlags flags = CommandFlags.None)
        {
            return await _db.StreamDeleteAsync(key, messageIds, flags);
        }

        /// <summary>
        /// Delete a consumer from a consumer group.
        /// </summary>
        /// <param name="key">The key of the stream.</param>
        /// <param name="groupName">The name of the consumer group.</param>
        /// <param name="consumerName">The name of the consumer.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The number of messages that were pending for the deleted consumer.</returns>
        /// <remarks><seealso href="https://redis.io/topics/streams-intro"/></remarks>
        public async Task<long> StreamDeleteConsumerAsync(RedisKey key, RedisValue groupName, RedisValue consumerName, CommandFlags flags = CommandFlags.None)
        {
            return await _db.StreamDeleteConsumerAsync(key, groupName, consumerName, flags);
        }

        /// <summary>
        /// Delete a consumer group.
        /// </summary>
        /// <param name="key">The key of the stream.</param>
        /// <param name="groupName">The name of the consumer group.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns><see langword="true"/> if deleted, <see langword="false"/> otherwise.</returns>
        /// <remarks><seealso href="https://redis.io/topics/streams-intro"/></remarks>
        public async Task<bool> StreamDeleteConsumerGroupAsync(RedisKey key, RedisValue groupName, CommandFlags flags = CommandFlags.None)
        {
            return await _db.StreamDeleteConsumerGroupAsync(key, groupName, flags);
        }

        /// <summary>
        /// Retrieve information about the groups created for the given stream. This is the equivalent of calling "XINFO GROUPS key".
        /// </summary>
        /// <param name="key">The key of the stream.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>An instance of <see cref="StreamGroupInfo"/> for each of the stream's groups.</returns>
        /// <remarks><seealso href="https://redis.io/topics/streams-intro"/></remarks>
        public async Task<StreamGroupInfo[]> StreamGroupInfoAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return await _db.StreamGroupInfoAsync(key, flags);
        }

        /// <summary>
        /// Retrieve information about the given stream. This is the equivalent of calling "XINFO STREAM key".
        /// </summary>
        /// <param name="key">The key of the stream.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>A <see cref="StreamInfo"/> instance with information about the stream.</returns>
        /// <remarks><seealso href="https://redis.io/topics/streams-intro"/></remarks>
        public async Task<StreamInfo> StreamInfoAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return await _db.StreamInfoAsync(key, flags);
        }

        /// <summary>
        /// Return the number of entries in a stream.
        /// </summary>
        /// <param name="key">The key of the stream.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The number of entries inside the given stream.</returns>
        /// <remarks><seealso href="https://redis.io/commands/xlen"/></remarks>
        public async Task<long> StreamLengthAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return await _db.StreamLengthAsync(key, flags);
        }

        /// <summary>
        /// View information about pending messages for a stream.
        /// A pending message is a message read using StreamReadGroup (XREADGROUP) but not yet acknowledged.
        /// </summary>
        /// <param name="key">The key of the stream.</param>
        /// <param name="groupName">The name of the consumer group</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>
        /// An instance of <see cref="StreamPendingInfo"/>.
        /// <see cref="StreamPendingInfo"/> contains the number of pending messages.
        /// The highest and lowest ID of the pending messages, and the consumers with their pending message count.
        /// </returns>
        /// <remarks>The equivalent of calling XPENDING key group.</remarks>
        /// <remarks><seealso href="https://redis.io/commands/xpending"/></remarks>
        public async Task<StreamPendingInfo> StreamPendingAsync(RedisKey key, RedisValue groupName, CommandFlags flags = CommandFlags.None)
        {
            return await _db.StreamPendingAsync(key, groupName, flags);
        }

        /// <summary>
        /// View information about each pending message.
        /// </summary>
        /// <param name="key">The key of the stream.</param>
        /// <param name="groupName">The name of the consumer group.</param>
        /// <param name="count">The maximum number of pending messages to return.</param>
        /// <param name="consumerName">The consumer name for the pending messages. Pass RedisValue.Null to include pending messages for all consumers.</param>
        /// <param name="minId">The minimum ID from which to read the stream of pending messages. The method will default to reading from the beginning of the stream.</param>
        /// <param name="maxId">The maximum ID to read to within the stream of pending messages. The method will default to reading to the end of the stream.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>An instance of <see cref="StreamPendingMessageInfo"/> for each pending message.</returns>
        /// <remarks>Equivalent of calling XPENDING key group start-id end-id count consumer-name.</remarks>
        /// <remarks><seealso href="https://redis.io/commands/xpending"/></remarks>
        public async Task<StreamPendingMessageInfo[]> StreamPendingMessagesAsync(RedisKey key, RedisValue groupName, int count, RedisValue consumerName, RedisValue? minId = null, RedisValue? maxId = null, CommandFlags flags = CommandFlags.None)
        {
            return await _db.StreamPendingMessagesAsync(key, groupName, count, consumerName, minId, maxId, flags);
        }

        /// <summary>
        /// Read a stream using the given range of IDs.
        /// </summary>
        /// <param name="key">The key of the stream.</param>
        /// <param name="minId">The minimum ID from which to read the stream. The method will default to reading from the beginning of the stream.</param>
        /// <param name="maxId">The maximum ID to read to within the stream. The method will default to reading to the end of the stream.</param>
        /// <param name="count">The maximum number of messages to return.</param>
        /// <param name="messageOrder">The order of the messages. <see cref="Order.Ascending"/> will execute XRANGE and <see cref="Order.Descending"/> will execute XREVRANGE.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>Returns an instance of <see cref="StreamEntry"/> for each message returned.</returns>
        /// <remarks><seealso href="https://redis.io/commands/xrange"/></remarks>
        public async Task<StreamEntry[]> StreamRangeAsync(RedisKey key, RedisValue? minId = null, RedisValue? maxId = null, int? count = null, Order messageOrder = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            return await _db.StreamRangeAsync(key, minId, maxId, count, messageOrder, flags);
        }

        /// <summary>
        /// Read from a single stream.
        /// </summary>
        /// <param name="key">The key of the stream.</param>
        /// <param name="position">The position from which to read the stream.</param>
        /// <param name="count">The maximum number of messages to return.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>Returns an instance of <see cref="StreamEntry"/> for each message returned.</returns>
        /// <remarks>
        /// <para>Equivalent of calling <c>XREAD COUNT num STREAMS key id</c>.</para>
        /// <para><seealso href="https://redis.io/commands/xread"/></para>
        /// </remarks>
        public async Task<StreamEntry[]> StreamReadAsync(RedisKey key, RedisValue position, int? count = null, CommandFlags flags = CommandFlags.None)
        {
            return await _db.StreamReadAsync(key, position, count, flags);
        }

        /// <summary>
        /// Read from multiple streams.
        /// </summary>
        /// <param name="streamPositions">Array of streams and the positions from which to begin reading for each stream.</param>
        /// <param name="countPerStream">The maximum number of messages to return from each stream.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>A value of <see cref="RedisStream"/> for each stream.</returns>
        /// <remarks>
        /// <para>Equivalent of calling <c>XREAD COUNT num STREAMS key1 key2 id1 id2</c>.</para>
        /// <para><seealso href="https://redis.io/commands/xread"/></para>
        /// </remarks>
        public async Task<RedisStream[]> StreamReadAsync(StreamPosition[] streamPositions, int? countPerStream = null, CommandFlags flags = CommandFlags.None)
        {
            return await _db.StreamReadAsync(streamPositions, countPerStream, flags);
        }

        /// <summary>
        /// Read messages from a stream into an associated consumer group.
        /// </summary>
        /// <param name="key">The key of the stream.</param>
        /// <param name="groupName">The name of the consumer group.</param>
        /// <param name="consumerName">The consumer name.</param>
        /// <param name="position">The position from which to read the stream. Defaults to <see cref="StreamPosition.NewMessages"/> when <see langword="null"/>.</param>
        /// <param name="count">The maximum number of messages to return.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>Returns a value of <see cref="StreamEntry"/> for each message returned.</returns>
        /// <remarks><seealso href="https://redis.io/commands/xreadgroup"/></remarks>
        public async Task<StreamEntry[]> StreamReadGroupAsync(RedisKey key, RedisValue groupName, RedisValue consumerName, RedisValue? position, int? count, CommandFlags flags)
        {
            return await _db.StreamReadGroupAsync(key, groupName, consumerName, position, count, flags);
        }

        /// <summary>
        /// Read messages from a stream into an associated consumer group.
        /// </summary>
        /// <param name="key">The key of the stream.</param>
        /// <param name="groupName">The name of the consumer group.</param>
        /// <param name="consumerName">The consumer name.</param>
        /// <param name="position">The position from which to read the stream. Defaults to <see cref="StreamPosition.NewMessages"/> when <see langword="null"/>.</param>
        /// <param name="count">The maximum number of messages to return.</param>
        /// <param name="noAck">When true, the message will not be added to the pending message list.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>Returns a value of <see cref="StreamEntry"/> for each message returned.</returns>
        /// <remarks><seealso href="https://redis.io/commands/xreadgroup"/></remarks>
        public async Task<StreamEntry[]> StreamReadGroupAsync(RedisKey key, RedisValue groupName, RedisValue consumerName, RedisValue? position = null, int? count = null, bool noAck = false, CommandFlags flags = CommandFlags.None)
        {
            return await _db.StreamReadGroupAsync(key, groupName, consumerName, position, count, noAck, flags);
        }

        /// <summary>
        /// Read from multiple streams into the given consumer group.
        /// The consumer group with the given <paramref name="groupName"/> will need to have been created for each stream prior to calling this method.
        /// </summary>
        /// <param name="streamPositions">Array of streams and the positions from which to begin reading for each stream.</param>
        /// <param name="groupName">The name of the consumer group.</param>
        /// <param name="consumerName"></param>
        /// <param name="countPerStream">The maximum number of messages to return from each stream.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>A value of <see cref="RedisStream"/> for each stream.</returns>
        /// <remarks>
        /// <para>Equivalent of calling <c>XREADGROUP GROUP groupName consumerName COUNT countPerStream STREAMS stream1 stream2 id1 id2</c>.</para>
        /// <para><seealso href="https://redis.io/commands/xreadgroup"/></para>
        /// </remarks>
        public async Task<RedisStream[]> StreamReadGroupAsync(StreamPosition[] streamPositions, RedisValue groupName, RedisValue consumerName, int? countPerStream, CommandFlags flags)
        {
            return await _db.StreamReadGroupAsync(streamPositions, groupName, consumerName, countPerStream, flags);
        }

        /// <summary>
        /// Read from multiple streams into the given consumer group.
        /// The consumer group with the given <paramref name="groupName"/> will need to have been created for each stream prior to calling this method.
        /// </summary>
        /// <param name="streamPositions">Array of streams and the positions from which to begin reading for each stream.</param>
        /// <param name="groupName">The name of the consumer group.</param>
        /// <param name="consumerName"></param>
        /// <param name="countPerStream">The maximum number of messages to return from each stream.</param>
        /// <param name="noAck">When true, the message will not be added to the pending message list.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>A value of <see cref="RedisStream"/> for each stream.</returns>
        /// <remarks>
        /// <para>Equivalent of calling <c>XREADGROUP GROUP groupName consumerName COUNT countPerStream STREAMS stream1 stream2 id1 id2</c>.</para>
        /// <para><seealso href="https://redis.io/commands/xreadgroup"/></para>
        /// </remarks>
        public async Task<RedisStream[]> StreamReadGroupAsync(StreamPosition[] streamPositions, RedisValue groupName, RedisValue consumerName, int? countPerStream = null, bool noAck = false, CommandFlags flags = CommandFlags.None)
        {
            return await _db.StreamReadGroupAsync(streamPositions, groupName, consumerName, countPerStream, noAck, flags);
        }

        /// <summary>
        /// Trim the stream to a specified maximum length.
        /// </summary>
        /// <param name="key">The key of the stream.</param>
        /// <param name="maxLength">The maximum length of the stream.</param>
        /// <param name="useApproximateMaxLength">If true, the "~" argument is used to allow the stream to exceed max length by a small number. This improves performance when removing messages.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The number of messages removed from the stream.</returns>
        /// <remarks><seealso href="https://redis.io/topics/streams-intro"/></remarks>
        public async Task<long> StreamTrimAsync(RedisKey key, int maxLength, bool useApproximateMaxLength = false, CommandFlags flags = CommandFlags.None)
        {
            return await _db.StreamTrimAsync(key, maxLength, useApproximateMaxLength, flags);
        }

        #endregion

        #region Advanced

        /// <summary>
        /// Indicate exactly which redis server we are talking to.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The endpoint serving the key.</returns>
        public async Task<EndPoint?> IdentifyEndpointAsync(RedisKey key = default, CommandFlags flags = CommandFlags.None)
        {
            return await _db.IdentifyEndpointAsync(key, flags);
        }

        //public async Task<long> PublishAsync(string channel, string message) => await _conn.GetSubscriber().PublishAsync(channel, message);

        /// <summary>
        /// Posts a message to the given channel.
        /// </summary>
        /// <param name="channel">The channel to publish to.</param>
        /// <param name="message">The message to send.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>
        /// The number of clients that received the message *on the destination server*,
        /// note that this doesn't mean much in a cluster as clients can get the message through other nodes.
        /// </returns>
        /// <remarks><seealso href="https://redis.io/commands/publish"/></remarks>
        public async Task<long> PublishAsync(RedisChannel channel, RedisValue message, CommandFlags flags = CommandFlags.None)
        {
            return await _db.PublishAsync(channel, message, flags);
        }

        /// <summary>
        /// Execute an arbitrary command against the server; this is primarily intended for executing modules,
        /// but may also be used to provide access to new features that lack a direct API.
        /// </summary>
        /// <param name="command">The command to run.</param>
        /// <param name="args">The arguments to pass for the command.</param>
        /// <returns>A dynamic representation of the command's result.</returns>
        /// <remarks>This API should be considered an advanced feature; inappropriate use can be harmful.</remarks>
        public async Task<RedisResult> ExecuteAsync(string command, params object[] args)
        {
            return await _db.ExecuteAsync(command, args);
        }

        /// <summary>
        /// Execute an arbitrary command against the server; this is primarily intended for executing modules,
        /// but may also be used to provide access to new features that lack a direct API.
        /// </summary>
        /// <param name="command">The command to run.</param>
        /// <param name="args">The arguments to pass for the command.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>A dynamic representation of the command's result.</returns>
        /// <remarks>This API should be considered an advanced feature; inappropriate use can be harmful.</remarks>
        public async Task<RedisResult> ExecuteAsync(string command, ICollection<object>? args, CommandFlags flags = CommandFlags.None)
        {
            return await _db.ExecuteAsync(command, args, flags);
        }

        /// <summary>
        /// Subscribe to perform some operation when a message to the preferred/active node is broadcast, without any guarantee of ordered handling.
        /// </summary>
        /// <param name="channel">The channel to subscribe to.</param>
        /// <param name="handler">The handler to invoke when a message is received on <paramref name="channel"/>.</param>
        /// <remarks>
        /// <seealso href="https://redis.io/commands/subscribe"/>,
        /// <seealso href="https://redis.io/commands/psubscribe"/>
        /// </remarks>
        public void Subscribe(string channel, Action<string, string> handler) =>
            _conn.GetSubscriber().Subscribe(channel, (chn, msg) => handler(chn, msg));

        /// <summary>
        /// Subscribe to perform some operation when a message to the preferred/active node is broadcast, without any guarantee of ordered handling.
        /// </summary>
        /// <param name="channel">The channel to subscribe to.</param>
        /// <param name="handler">The handler to invoke when a message is received on <paramref name="channel"/>.</param>
        /// <remarks>
        /// <seealso href="https://redis.io/commands/subscribe"/>,
        /// <seealso href="https://redis.io/commands/psubscribe"/>
        /// </remarks>
        public async Task SubscribeAsync(string channel, Action<string, string> handler) =>
            await _conn.GetSubscriber().SubscribeAsync(channel, (chn, msg) => handler(chn, msg));

        /// <summary>
        /// 批量执行Redis操作
        /// 
        /// 摘要:
        /// Execute the batch operation, sending all queued commands to the server. Note
        /// that this operation is neither synchronous nor truly asynchronous - it simply
        /// enqueues the buffered messages. To check on completion, you should check the
        /// individual responses.
        /// </summary>
        /// <param name="operations"></param>
        /// <returns></returns>
        public void ExecuteBatch(params Action[] operations) =>
            Task.Run(() =>
            {
                var batch = _db.CreateBatch();

                foreach (var operation in operations)
                    operation();

                batch.Execute();
            });

        /// <summary>
        /// 批量执行Redis操作
        /// 
        /// 摘要:
        /// Execute the batch operation, sending all queued commands to the server. Note
        /// that this operation is neither synchronous nor truly asynchronous - it simply
        /// enqueues the buffered messages. To check on completion, you should check the
        /// individual responses.
        /// </summary>
        /// <param name="operations"></param>
        /// <returns></returns>
        public Task ExecuteBatchAsync(params Action[] operations) =>
            Task.Run(() =>
            {
                var batch = _db.CreateBatch();

                foreach (var operation in operations)
                    operation();

                batch.Execute();
            });

        /// <summary>
        /// 获取分布式锁并执行(非阻塞。加锁失败直接返回(false,null))
        /// </summary>
        /// <param name="key">要锁定的key</param>
        /// <param name="value">锁定的value，加锁时赋值value，在解锁时必须是同一个value的客户端才能解锁</param>
        /// <param name="del">加锁成功时执行的业务方法</param>
        /// <param name="expiry">持锁超时时间。超时后锁自动释放</param>
        /// <param name="args">业务方法参数</param>
        /// <returns>(success,return value of the del)</returns>
        public async Task<(bool, object)> LockExecuteAsync(string key, string value, Delegate del, TimeSpan expiry, params object[] args)
        {
            if (!await _db.LockTakeAsync(key, value, expiry))
                return (false, null);

            try
            {
                return (true, del.DynamicInvoke(args));
            }
            finally
            {
                await _db.LockReleaseAsync(key, value);
            }
        }

        /// <summary>
        /// 获取分布式锁并执行(阻塞。直到成功加锁或超时)
        /// </summary>
        /// <param name="key">要锁定的key</param>
        /// <param name="value">锁定的value，加锁时赋值value，在解锁时必须是同一个value的客户端才能解锁</param>
        /// <param name="del">加锁成功时执行的业务方法</param>
        /// <param name="result">del返回值</param>
        /// <param name="expiry">持锁超时时间。超时后锁自动释放</param>
        /// <param name="timeout">加锁超时时间(ms).0表示永不超时</param>
        /// <param name="args">业务方法参数</param>
        /// <returns>success</returns>
        public bool LockExecute(string key, string value, Delegate del, out object result, TimeSpan expiry, int timeout = 0, params object[] args)
        {
            result = null;
            if (!GetLock(key, value, expiry, timeout))
                return false;

            try
            {
                result = del.DynamicInvoke(args);
                return true;
            }
            finally
            {
                _db.LockRelease(key, value);
            }
        }

        /// <summary>
        /// 获取分布式锁并执行
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="action"></param>
        /// <param name="expiry"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public bool LockExecute(string key, string value, Action action, TimeSpan expiry, int timeout = 0)
        {
            return LockExecute(key, value, action, out var _, expiry, timeout);
        }

        /// <summary>
        /// 获取分布式锁并执行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="action"></param>
        /// <param name="arg"></param>
        /// <param name="expiry"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public bool LockExecute<T>(string key, string value, Action<T> action, T arg, TimeSpan expiry, int timeout = 0)
        {
            return LockExecute(key, value, action, out var _, expiry, timeout, arg);
        }

        /// <summary>
        /// 获取分布式锁并执行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="func"></param>
        /// <param name="result"></param>
        /// <param name="expiry"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public bool LockExecute<T>(string key, string value, Func<T> func, out T result, TimeSpan expiry, int timeout = 0)
        {
            result = default;
            if (!GetLock(key, value, expiry, timeout))
                return false;
            try
            {
                result = func();
                return true;
            }
            finally
            {
                _db.LockRelease(key, value);
            }
        }

        /// <summary>
        /// 获取分布式锁并执行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="func"></param>
        /// <param name="arg"></param>
        /// <param name="result"></param>
        /// <param name="expiry"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public bool LockExecute<T, TResult>(string key, string value, Func<T, TResult> func, T arg, out TResult result, TimeSpan expiry, int timeout = 0)
        {
            result = default;
            if (!GetLock(key, value, expiry, timeout))
                return false;
            try
            {
                result = func(arg);
                return true;
            }
            finally
            {
                _db.LockRelease(key, value);
            }
        }

        private bool GetLock(string key, string value, TimeSpan expiry, int timeout)
        {
            using var waitHandle = new AutoResetEvent(false);
            var timer = new Timer(1000);
            timer.Elapsed += (s, e) =>
            {
                if (!_db.LockTake(key, value, expiry))
                    return;
                try
                {
                    waitHandle.Set();
                    timer.Stop();
                }
                catch
                {
                }
            };
            timer.Start();

            if (timeout > 0)
                waitHandle.WaitOne(timeout);
            else
                waitHandle.WaitOne();

            timer.Stop();
            timer.Close();
            timer.Dispose();

            return _db.LockQuery(key) == value;
        }

        #endregion
    }

    /// <summary>
    /// Extension methods for StackExchange.Redis
    /// </summary>
    public static class StackExchangeRedisExtension
    {
        /// <summary>
        /// Converts a RedisKey to a string.
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static IEnumerable<string> ToStrings(this IEnumerable<RedisKey> keys)
        {
            var redisKeys = keys as RedisKey[] ?? keys.ToArray();
            return !redisKeys.Any() ? null : redisKeys.Select(k => (string)k);
        }

        /// <summary>
        /// Converts a RedisValue to a string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static RedisValue ToRedisValue<T>(this T value)
        {
            if (value == null)
                return RedisValue.Null;

            return value switch
            {
                ValueType => value.ToString(),
                string s => s,
                _ => JsonConvert.SerializeObject(value)
            };
        }

        /// <summary>
        /// Converts a string to a RedisValue.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        public static RedisValue[] ToRedisValues<T>(this IEnumerable<T> values)
        {
            var enumerable = values as T[] ?? values.ToArray();
            return !enumerable.Any() ? null : enumerable.Select(v => v.ToRedisValue()).ToArray();
        }

        /// <summary>
        /// Converts a RedisValue to an object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T ToObject<T>(this RedisValue value) //where T : class
        {
            if (!value.HasValue)
                return default;

            if (typeof(T).IsSubclassOf(typeof(ValueType)) || typeof(T) == typeof(string))
                return (T)Convert.ChangeType(value.ToString(), typeof(T));

            return JsonConvert.DeserializeObject<T>(value.ToString());
        }

        /// <summary>
        /// Converts a RedisValue[] to an object[].
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        public static IEnumerable<T> ToObjects<T>(this IEnumerable<RedisValue> values) where T : class
        {
            var redisValues = values as RedisValue[] ?? values.ToArray();
            return !redisValues.Any() ? null : redisValues.Select(v => v.ToObject<T>());
        }

        /// <summary>
        /// 转换成 HashEntry 数组
        /// </summary>
        /// <param name="entries"></param>
        /// <returns></returns>
        public static HashEntry[] ToHashEntries(this ConcurrentDictionary<string, string> entries)
        {
            if (entries == null || !entries.Any())
                return null;

            var es = new HashEntry[entries.Count];
            for (var i = 0; i < entries.Count; i++)
            {
                var name = entries.Keys.ElementAt(i);
                var value = entries[name];
                es[i] = new HashEntry(name, value);
            }

            return es;
        }

        /// <summary>
        /// Converts a HashEntry[] to a ConcurrentDictionary.
        /// </summary>
        /// <param name="entries"></param>
        /// <returns></returns>
        public static ConcurrentDictionary<string, string> ToConcurrentDictionary(this IEnumerable<HashEntry> entries)
        {
            var hashEntries = entries as HashEntry[] ?? entries.ToArray();
            if (!hashEntries.Any())
                return null;

            var dict = new ConcurrentDictionary<string, string>();
            foreach (var entry in hashEntries)
                dict[entry.Name] = entry.Value;

            return dict;
        }

        /// <summary>
        /// Converts a RedisValue[] to a ConcurrentDictionary.
        /// </summary>
        /// <param name="hashValues"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public static ConcurrentDictionary<string, string> ToConcurrentDictionary(this RedisValue[] hashValues, IEnumerable<string> fields)
        {
            var enumerable = fields as string[] ?? fields.ToArray();
            if (hashValues == null || !hashValues.Any() || !enumerable.Any())
                return null;

            var dict = new ConcurrentDictionary<string, string>();
            for (var i = 0; i < enumerable.Count(); i++)
                dict[enumerable.ElementAt(i)] = hashValues[i];

            return dict;
        }

        /// <summary>
        /// Converts a IEnumerable to a ConcurrentDictionary.
        /// </summary>
        /// <param name="entries"></param>
        /// <returns></returns>
        public static ConcurrentDictionary<string, double> ToConcurrentDictionary(this IEnumerable<SortedSetEntry> entries)
        {
            var sortedSetEntries = entries as SortedSetEntry[] ?? entries.ToArray();
            if (!sortedSetEntries.Any())
                return null;

            var dict = new ConcurrentDictionary<string, double>();
            foreach (var entry in sortedSetEntries)
                dict[entry.Element] = entry.Score;

            return dict;
        }


    }
}
