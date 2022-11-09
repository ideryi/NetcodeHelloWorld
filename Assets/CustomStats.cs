using Unity.Multiplayer.Tools.NetStats;
using Unity.Multiplayer.Tools.NetStatsMonitor;
using UnityEngine;

// User-defined metrics can be defined using the MetricTypeEnum attribute
[MetricTypeEnum(DisplayName = "CustomMetric")]
enum CustomMetric
{
    // Metadata for each user-defined metric can be defined using the MetricMetadata Attribute

    [MetricMetadata(Units = Units.Hertz, MetricKind = MetricKind.Gauge)]
    Framerate,

    [MetricMetadata(Units = Units.None, MetricKind = MetricKind.Gauge)]
    TriangleCount,

    [MetricMetadata(Units = Units.None, MetricKind = MetricKind.Gauge, DisplayAsPercentage = true)]
    CpuUsage,
}

public class CustomStats : MonoBehaviour
{
    RuntimeNetStatsMonitor m_NetStatsMonitor;

    void Start()
    {
        m_NetStatsMonitor = FindObjectOfType<RuntimeNetStatsMonitor>();
    }

    void Update()
    {
        // Once you have access to an instance of the RuntimeNetStatsMonitor
        // you can provide custom data to it using AddCustomValue,
        // using an enum with the MetricTypeEnum attribute.
        foreach(var item in m_NetStatsMonitor.Configuration.DisplayElements)
        {
            //item.Stats.
        }

        if(m_NetStatsMonitor.Configuration.DisplayElements.Count > 0)
        {
            var item = m_NetStatsMonitor.Configuration.DisplayElements[0];
            var str = item.ToString();
            var tt = item.GetType().ToString();
        }

        m_NetStatsMonitor.AddCustomValue(
            MetricId.Create(CustomMetric.Framerate),
            Random.Range(40, 60f));

        m_NetStatsMonitor.AddCustomValue(
            MetricId.Create(CustomMetric.TriangleCount),
            Random.Range(10000, 100000));

        m_NetStatsMonitor.AddCustomValue(
            MetricId.Create(CustomMetric.CpuUsage),
            Random.Range(0.05f, 0.4f));
    }
}