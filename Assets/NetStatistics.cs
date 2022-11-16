using HelloWorld;
using System.Collections.Generic;
using System.Text;
using Unity.Profiling;
using Unity.Profiling.LowLevel.Unsafe;
using UnityEngine;
using UnityEditor;
using System;
using Unity.Netcode;
using Unity.VisualScripting;

//[CustomEditor(typeof(NetStatistics))]
public class NetStatistics : MonoBehaviour
{
    public bool showGUI = true;
    public int x = 0;
    public int y = 300;
    public int w = 250;
    public int h = 100;

    string statsText;
    ProfilerRecorder TotalBytesReceived;
    ProfilerRecorder TotalBytesSent;

    public long totalReceive = 0;
    public long totalSend = 0;
    public long frameReceive = 0;
    public long frameSend = 0;

    public double lantency = 0;
    //public double lantency2 = 0;


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
        ProcessReceiveOrSend(TotalBytesSent, false);

        if(showGUI)
        {
            //lantency = HelloWorldPlayer.lantency;
            //lantency2 = HelloWorldPlayer.lantency;

            lantency = LantencyTest.lantency;

            sb.AppendLine($"Lentancy:{lantency} ms");
            //sb.AppendLine($"Lentancy2:{lantency2} ms");

            sb.AppendLine($"Total Bytes Received:{totalReceive} byte");
            sb.AppendLine($"Total Bytes Sent: {totalSend} byte");

            sb.AppendLine($"Frame Bytes Received:{frameReceive} byte");
            sb.AppendLine($"Frame Bytes Sent: {frameSend} byte");

            statsText = sb.ToString();
        }
    }

    void OnGUI()
    {
        if(showGUI)
            GUI.TextArea(new Rect(x, y, w, h), statsText);
    }

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


    void ProcessReceiveOrSend(ProfilerRecorder recorder, bool isReceive = true)
    {
        var samplesCount = recorder.Capacity;
        if (samplesCount == 0)
            return;

        long frmaeTotal = 0;
        unsafe
        {
            var samples = stackalloc ProfilerRecorderSample[samplesCount];
            recorder.CopyTo(samples, samplesCount);
            for (var i = 0; i < samplesCount; ++i)
            {
                if (samples[i].Value > 0)
                {
                    frmaeTotal += samples[i].Value;
                    Debug.Log("++++++++++++++++++++" + samples[i].Value.ToString());
                }
                else
                {
                    Debug.Log("--------------------" + samples[i].Value.ToString());
                }
            }
        }

        if (isReceive)
        {
            frameReceive = frmaeTotal;
            totalReceive += frmaeTotal;
        }
        else
        {
            frameSend = frmaeTotal;
            totalSend += frmaeTotal;
        }

        Debug.Log(string.Format("totalReceive: {0}\ntotalSend: {1}\nframeReceive: {2}\nframeSend: {3}\n",totalReceive,totalSend,frameReceive,frameSend));
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



