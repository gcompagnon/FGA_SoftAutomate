using System;
using System.Collections.Generic;

namespace Saviso.EntityFramework
{
    public static class Extensions
    {

        public static void Apply<T>(this IEnumerable<T> items, Action<T> actionToApply)
        {
            foreach (var item in items)
            {
                actionToApply(item);
            }
        }
    }
}