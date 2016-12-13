using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.Threading;
using Microsoft.Extensions.Primitives;

namespace RuntimeConfig
{
    public abstract class PollingConfigurationProvider : IConfigurationProvider
    {
        private Timer _timer;
        private readonly int _pollingIntervalMilliseconds;
        private Dictionary<string, string> _data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private ConfigurationReloadToken _reloadToken = new ConfigurationReloadToken();

        public PollingConfigurationProvider(int pollingIntervalSeconds)
        {
            _pollingIntervalMilliseconds = pollingIntervalSeconds * 1000;
        }

        public void Load()
        {
            var data = GetConfiguration();
            Interlocked.Exchange(ref _data, data);

            var autoEvent = new AutoResetEvent(false);
            _timer = new Timer(ReloadConfiguration, autoEvent, _pollingIntervalMilliseconds, _pollingIntervalMilliseconds);
        }

        public bool TryGet(string key, out string value)
        {
            return _data.TryGetValue(key, out value);
        }

        public void Set(string key, string value)
        {
            _data[key] = value;
        }

        public IChangeToken GetReloadToken()
        {
            return _reloadToken;
        }

        public IEnumerable<string> GetChildKeys(IEnumerable<string> earlierKeys, string parentPath)
        {
            var prefix = parentPath == null ? string.Empty : parentPath + ConfigurationPath.KeyDelimiter;

            return _data
                .Where(kv => kv.Key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                .Select(kv => Segment(kv.Key, prefix.Length))
                .Concat(earlierKeys)
                .OrderBy(k => k, ConfigurationKeyComparer.Instance);
        }

        private static string Segment(string key, int prefixLength)
        {
            var indexOf = key.IndexOf(ConfigurationPath.KeyDelimiter, prefixLength, StringComparison.OrdinalIgnoreCase);
            return indexOf < 0 ? key.Substring(prefixLength) : key.Substring(prefixLength, indexOf - prefixLength);
        }

        protected void OnReload()
        {
            var previousToken = Interlocked.Exchange(ref _reloadToken, new ConfigurationReloadToken());
            previousToken.OnReload();
        }

        private void ReloadConfiguration(object stateInfo)
        {
            var data = GetConfiguration();
            Interlocked.Exchange(ref _data, data);
        }

        protected abstract Dictionary<string, string> GetConfiguration();
    }
}
