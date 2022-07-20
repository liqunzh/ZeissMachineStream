using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;
using ZeissMachineStream.Exceptions;
using ZeissMachineStream.Models;
using ZeissMachineStream.Util;

namespace ZeissMachineStream
{
    public static class StatusDataRepository
    {
        private static Dictionary<string, FixLengthQueue<StatusDataDetail>> _cache;
        private static Object _innerLock;
        private static readonly int DEFAULT_CAPACITY = 100;

        private static readonly string[] STATUS_DEF = { "idle", "running", "finished", "repaired", "errored" };

        static StatusDataRepository()
        {
            _cache = new Dictionary<string, FixLengthQueue<StatusDataDetail>>();
            _innerLock = new object();
        }

        public static IEnumerable<StatusSummary> GetSummary(String status)
        {
            if (status != null & !CheckStatus(status))
                throw new RequestValidationException("Bad status parameter. Must be one of [idled|running|finished|errored|repaired]");

            var result = _cache.Select(entry => new StatusSummary() { MachineId = entry.Key, Status = entry.Value.Last().Status });
            if (!String.IsNullOrEmpty(status))
            {
                result = result.Where(item => item.Status.Equals(status));
            }

            return result;
        }

        public static IEnumerable<StatusDataDetail> GetMachineDetail(String machineId, int dataLimit)
        {
            if (dataLimit <= 0)
                dataLimit = DEFAULT_CAPACITY;

            if (_cache.ContainsKey(machineId))
            {
                return _cache[machineId].TakeLast(dataLimit).OrderByDescending(item => item.Timestamp);
            }
            else
            {
                return null;
            }
        }

        public static void AddStatusData(StatusData data)
        {
            if (data != null && data.Payload != null)
            {
                FixLengthQueue<StatusDataDetail> _dataCache = null;
                if (!_cache.ContainsKey(data.Payload.MachineId))
                {
                    lock (_innerLock)
                    {
                        if (!_cache.ContainsKey(data.Payload.MachineId))
                        {
                            _dataCache = new FixLengthQueue<StatusDataDetail>(DEFAULT_CAPACITY);
                            _cache.Add(data.Payload.MachineId, _dataCache);
                        }

                    }
                }

                _dataCache = _cache[data.Payload.MachineId];

                _dataCache.Enqueue(data.Payload);
            }
        }

        private static bool CheckStatus(string status)
        {
            if (status == null) return false;
            for (int i = 0; i < STATUS_DEF.Length; i++)
            {
                if (status.Equals(STATUS_DEF[i])) return true;
            }
            return false;
        }

    }
}
