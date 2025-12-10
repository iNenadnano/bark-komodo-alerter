using System.Text.Json.Serialization;

namespace BarkKomodoAlerter.Model;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SeverityLevel
{
    Ok,
    Warning,
    Critical
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AlertType
{
    Test,
    ServerUnreachable,
    ServerCpu,
    ServerMem,
    ServerDisk,
    ServerVersionMismatch,
    ContainerStateChange,
    DeploymentImageUpdateAvailable,
    DeploymentAutoUpdated,
    StackStateChange,
    StackImageUpdateAvailable,
    StackAutoUpdated,
    AwsBuilderTerminationFailed,
    ResourceSyncPendingUpdates,
    BuildFailed,
    RepoBuildFailed,
    ProcedureFailed,
    ActionFailed,
    ScheduleRun,
    Custom,
    None
}