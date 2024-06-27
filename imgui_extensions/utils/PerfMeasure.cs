using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using static GodotImGuiExtension.PerfMeasure;

namespace GodotImGuiExtension;

public static class PerfMeasure
{
    public static readonly Dictionary<MethodBase, FixedSizedQueue<int>> PerfMonitorData = new();
    private static readonly List<MethodBase> MethodsInvokedThisFrame = [];
    public static bool HasMethodInvoked(MethodBase method) => MethodsInvokedThisFrame.Contains(method);
    public static void Reset()
    {
        MethodsInvokedThisFrame.Clear();
    }
}

public class FixedSizedQueue<T>(int size) : ConcurrentQueue<T>
{
    private readonly object _syncObject = new ();

    public int Size { get; private set; } = size;

    public new void Enqueue(T obj)
    {
        base.Enqueue(obj);
        lock (_syncObject)
        {
            while (Count > Size)
            {
                T outObj;
                TryDequeue(out outObj);
            }
        }
    }
}

public static class MethodTimeLogger
{
    public static void Log(MethodBase methodBase, TimeSpan elapsed, string message)
    {
    #if DEBUG
        if (!PerfMonitorData.TryGetValue(methodBase, out var value))
        {
            value = new FixedSizedQueue<int>(120);
            PerfMonitorData[methodBase] = value;
        }
        value.Enqueue(elapsed.Microseconds);
    #endif
    }
}