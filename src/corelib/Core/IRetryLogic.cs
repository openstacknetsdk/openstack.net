using System;
using System.Collections.Generic;

namespace net.openstack.Core
{
    public interface IRetryLogic<T, in T2>
    {
        T Execute(Func<T> logic, int retryCount = 1, int retryDelayInMs = 0);
        T Execute(Func<T> logic, IEnumerable<T2> sucessValues, int retryCount = 1, int retryDelayInMs = 0);
    }
}
