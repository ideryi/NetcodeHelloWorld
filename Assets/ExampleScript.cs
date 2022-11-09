using HelloWorld;
using System.Collections.Generic;
using System.Text;
using Unity.Profiling;
using Unity.Profiling.LowLevel.Unsafe;
using UnityEngine;

public class ExampleScript : MonoBehaviour
{
    string statsText;
    ProfilerRecorder systemMemoryRecorder;
    ProfilerRecorder gcMemoryRecorder;
    ProfilerRecorder mainThreadTimeRecorder;
    ProfilerRecorder TotalBytesReceived;
    ProfilerRecorder TotalBytesSent;

    //static long totalReceive = 0;
    //static long totalSend = 0;
    //static long frameReceive = 0;
    //static long frameSend = 0;

    static double GetRecorderFrameAverage(ProfilerRecorder recorder)
    {
        var samplesCount = recorder.Capacity;
        if (samplesCount == 0)
            return 0;

        double r = 0;
        unsafe
        {
            var samples = stackalloc ProfilerRecorderSample[samplesCount];
            recorder.CopyTo(samples, samplesCount);
            for (var i = 0; i < samplesCount; ++i)
                r += samples[i].Value;
            r /= samplesCount;
        }

        return r;
    }


    static void ProcessReceiveOrSend(ProfilerRecorder recorder,bool isReceive = true)
    {
        var samplesCount = recorder.Capacity;
        if (samplesCount == 0)
            return ;

        long frmaeTotal = 0;
        unsafe
        {
            var samples = stackalloc ProfilerRecorderSample[samplesCount];
            recorder.CopyTo(samples, samplesCount);
            for (var i = 0; i < samplesCount; ++i)
            {
                if(samples[i].Value > 0)
                {
                    frmaeTotal += samples[i].Value;
                }
            }
        }

        if(isReceive)
        {
            HelloWorldManager.frameReceive = frmaeTotal;
            HelloWorldManager.totalReceive += frmaeTotal;
        }
        else
        {
            HelloWorldManager.frameSend = frmaeTotal;
            HelloWorldManager.totalSend += frmaeTotal;
        }
    }

    void OnEnable()
    {
        TotalBytesReceived = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Total Bytes Received");
        TotalBytesSent = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Total Bytes Sent");
        //EnumerateProfilerStats();
    }

    void OnDisable()
    {
        TotalBytesReceived.Dispose();
        TotalBytesSent.Dispose();
    }

    void Update()
    {
        var sb = new StringBuilder(500);

        ProcessReceiveOrSend(TotalBytesReceived);
        ProcessReceiveOrSend(TotalBytesReceived,false);

        sb.AppendLine($"Total Bytes Received:{HelloWorldManager.totalReceive} byte");
        sb.AppendLine($"Total Bytes Sent: {HelloWorldManager.totalSend} byte");

        sb.AppendLine($"Frame Bytes Received:{HelloWorldManager.frameReceive} byte");
        sb.AppendLine($"Frame Bytes Sent: {HelloWorldManager.frameSend} byte");

        statsText = sb.ToString();
    }

    void OnGUI()
    {
        GUI.TextArea(new Rect(800, 100, 250, 100), statsText);
    }

    struct StatInfo
    {
        public ProfilerCategory Cat;
        public string Name;
        public ProfilerMarkerDataUnit Unit;
    }

    static unsafe void EnumerateProfilerStats()
    {
        var availableStatHandles = new List<ProfilerRecorderHandle>();
        ProfilerRecorderHandle.GetAvailable(availableStatHandles);

        var availableStats = new List<StatInfo>(availableStatHandles.Count);
        foreach (var h in availableStatHandles)
        {
            var statDesc = ProfilerRecorderHandle.GetDescription(h);
            var statInfo = new StatInfo()
            {
                Cat = statDesc.Category,
                Name = statDesc.Name,
                Unit = statDesc.UnitType
            };

            if (statDesc.Name == "Total Bytes Received")
            {
                int a = 0;
                a++;
            }
            availableStats.Add(statInfo);
        }
        availableStats.Sort((a, b) =>
        {
            var result = string.Compare(a.Cat.ToString(), b.Cat.ToString());
            if (result != 0)
                return result;

            return string.Compare(a.Name, b.Name);
        });

        var sb = new StringBuilder("Available stats:\n");
        foreach (var s in availableStats)
        {
            sb.AppendLine($"{(int)s.Cat}\t\t - {s.Name}\t\t - {s.Unit}");
        }

        var info = sb.ToString();
        Debug.Log(info);
    }
}