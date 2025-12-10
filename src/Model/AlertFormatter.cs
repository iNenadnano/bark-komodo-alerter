using System.Text.Json;

namespace BarkKomodoAlerter.Model
{
    public static class AlertFormatter
    {
        private static readonly HashSet<string> AllowedSounds =
            new(StringComparer.OrdinalIgnoreCase)
            {
                "alarm",
                "anticipate",
                "bell",
                "birdsong",
                "bloom",
                "calypso",
                "chime",
                "choo",
                "descent",
                "electronic",
                "fanfare",
                "glass",
                "gotosleep",
                "healthnotification",
                "horn",
                "ladder",
                "mailsent",
                "minuet",
                "multiwayinvitation",
                "newmail",
                "newsflash",
                "noir",
                "paymentsuccess",
                "shake",
                "sherwoodforest",
                "silence",
                "spell",
                "suspense",
                "telegraph",
                "tiptoes",
                "typewriters",
                "update"
            };

        public static string MapLevel(SeverityLevel level) =>
            level switch
            {
                SeverityLevel.Critical => "timeSensitive",
                SeverityLevel.Warning => "active",
                _ => "active"
            };

        public static string Title(Alert alert) =>
            alert.Data.Type switch
            {
                "Test" => $"Alerter test",
                "ServerUnreachable" => $"Server unreachable",
                "ServerCpu" => $"High CPU",
                "ServerMem" => $"High memory",
                "ServerDisk" => $"Disk usage",
                "ServerVersionMismatch" => $"Version mismatch",
                "ContainerStateChange" => $"Container state",
                "DeploymentImageUpdateAvailable" => $"Deployment update available",
                "DeploymentAutoUpdated" => $"Deployment auto-updated",
                "StackStateChange" => $"Stack state",
                "StackImageUpdateAvailable" => $"Stack update available",
                "StackAutoUpdated" => $"Stack auto-updated",
                "AwsBuilderTerminationFailed" => "AWS builder termination failed",
                "ResourceSyncPendingUpdates" => $"Resource sync pending",
                "BuildFailed" => $"Build failed",
                "RepoBuildFailed" => $"Repo build failed",
                "ProcedureFailed" => $"Procedure failed",
                "ActionFailed" => $"Action failed",
                "ScheduleRun" => $"Scheduled run",
                "Custom" => alert.Data.GetString("message") ?? "Komodo alert",
                _ => "Komodo alert"
            };

        public static string Subtitle(Alert alert) =>
            alert.Data.Type switch
            {
                "Test" => $"{alert.Data.GetString("name")}",
                "ServerUnreachable" => $"{alert.Data.GetString("name")}",
                "ServerCpu" => $"{alert.Data.GetString("name")}",
                "ServerMem" => $"{alert.Data.GetString("name")}",
                "ServerDisk" => $"{alert.Data.GetString("name")}",
                "ServerVersionMismatch" => $"{alert.Data.GetString("name")}",
                "ContainerStateChange" => $"{alert.Data.GetString("name")}",
                "DeploymentImageUpdateAvailable" => $"{alert.Data.GetString("name")}",
                "DeploymentAutoUpdated" => $"{alert.Data.GetString("name")}",
                "StackStateChange" => $"{alert.Data.GetString("name")}",
                "StackImageUpdateAvailable" => $"{alert.Data.GetString("name")}",
                "StackAutoUpdated" => $"{alert.Data.GetString("name")}",
                "AwsBuilderTerminationFailed" => "AWS builder termination failed",
                "ResourceSyncPendingUpdates" => $"{alert.Data.GetString("name")}",
                "BuildFailed" => $"{alert.Data.GetString("name")}",
                "RepoBuildFailed" => $"{alert.Data.GetString("name")}",
                "ProcedureFailed" => $"{alert.Data.GetString("name")}",
                "ActionFailed" => $"{alert.Data.GetString("name")}",
                "ScheduleRun" => $"{alert.Data.GetString("name")}",
                "Custom" => alert.Data.GetString("message") ?? "Komodo alert",
                _ => "Komodo alert"
            };

        public static string Body(Alert alert)
        {
            var level = FmtLevel(alert.Level);
            var r = alert.Data;

            return r.Type switch
            {
                "Test" => WithLink(
                    $"{level} | If you see this message, then Alerter {r.GetString("name")} is working",
                    ResourceLink("alerter", r.GetString("id"))),
                "ServerVersionMismatch" => alert.Level == SeverityLevel.Ok
                    ? WithLink(
                        $"{level} | {r.GetString("name")}{FmtRegion(r.GetString("region"))} | Periphery version now matches Core version ✅",
                        ResourceLink("server", r.GetString("id")))
                    : WithLink(
                        $"{level} | {r.GetString("name")}{FmtRegion(r.GetString("region"))} | Version mismatch detected ⚠️\nPeriphery: {r.GetString("server_version")} | Core: {r.GetString("core_version")}",
                        ResourceLink("server", r.GetString("id"))),
                "ServerUnreachable" => alert.Level switch
                {
                    SeverityLevel.Ok => WithLink(
                        $"{level} | {r.GetString("name")}{FmtRegion(r.GetString("region"))} is now reachable",
                        ResourceLink("server", r.GetString("id"))),
                    SeverityLevel.Critical => $"{WithLink($"{level} | {r.GetString("name")}{FmtRegion(r.GetString("region"))} is unreachable ❌", ResourceLink("server", r.GetString("id")))}{FmtError(r.Data)}",
                    _ => $"{WithLink($"{level} | {r.GetString("name")}{FmtRegion(r.GetString("region"))} is unreachable", ResourceLink("server", r.GetString("id")))}{FmtError(r.Data)}"
                },
                "ServerCpu" => WithLink(
                    $"{level} | {r.GetString("name")}{FmtRegion(r.GetString("region"))} cpu usage at {r.GetDouble("percentage"):0.0}%",
                    ResourceLink("server", r.GetString("id"))),
                "ServerMem" =>
                    WithLink(
                        $"{level} | {r.GetString("name")}{FmtRegion(r.GetString("region"))} memory usage at {Percent(r.GetDouble("used_gb"), r.GetDouble("total_gb")):0.0}%💾\n\nUsing {r.GetDouble("used_gb"):0.0} GiB / {r.GetDouble("total_gb"):0.0} GiB",
                        ResourceLink("server", r.GetString("id"))),
                "ServerDisk" =>
                    WithLink(
                        $"{level} | {r.GetString("name")}{FmtRegion(r.GetString("region"))} disk usage at {Percent(r.GetDouble("used_gb"), r.GetDouble("total_gb")):0.0}%💿\nmount point: {FmtPath(r.GetString("path"))}\nusing {r.GetDouble("used_gb"):0.0} GiB / {r.GetDouble("total_gb"):0.0} GiB",
                        ResourceLink("server", r.GetString("id"))),
                "ContainerStateChange" =>
                    WithLink(
                        $"📦Deployment {r.GetString("name")} is now {FmtDockerContainerState(r.GetString("to"))}\nserver: {r.GetString("server_name")}\nprevious: {r.GetString("from")}",
                        ResourceLink("deployment", r.GetString("id"))),
                "DeploymentImageUpdateAvailable" =>
                    WithLink(
                        $"⬆ Deployment {r.GetString("name")} has an update available\nserver: {r.GetString("server_name")}\nimage: {r.GetString("image")}",
                        ResourceLink("deployment", r.GetString("id"))),
                "DeploymentAutoUpdated" =>
                    WithLink(
                        $"⬆ Deployment {r.GetString("name")} was updated automatically\nserver: {r.GetString("server_name")}\nimage: {r.GetString("image")}",
                        ResourceLink("deployment", r.GetString("id"))),
                "StackStateChange" =>
                    WithLink(
                        $"🥞 Stack {r.GetString("name")} is now {FmtStackState(r.GetString("to"))}\nserver: {r.GetString("server_name")}\nprevious: {r.GetString("from")}",
                        ResourceLink("stack", r.GetString("id"))),
                "StackImageUpdateAvailable" =>
                    WithLink(
                        $"⬆ Stack {r.GetString("name")} has an update available\nserver: {r.GetString("server_name")}\nservice: {r.GetString("service")}\nimage: {r.GetString("image")}",
                        ResourceLink("stack", r.GetString("id"))),
                "StackAutoUpdated" =>
                    WithLink(
                        $"⬆ Stack {r.GetString("name")} was updated automatically ⏫\nserver: {r.GetString("server_name")}\n{FmtImagesLabel(r.GetStringArray("images"))}",
                        ResourceLink("stack", r.GetString("id"))),
                "AwsBuilderTerminationFailed" =>
                    $"{level} | Failed to terminate AWS builder instance\ninstance id: {r.GetString("instance_id")}\n{r.GetString("message")}",
                "ResourceSyncPendingUpdates" =>
                    WithLink(
                        $"{level} | Pending resource sync updates on {r.GetString("name")}",
                        ResourceLink("resourcesync", r.GetString("id"))),
                "BuildFailed" =>
                    WithLink(
                        $"{level} | Build {r.GetString("name")} failed\nversion: v{r.GetVersion()}",
                        ResourceLink("build", r.GetString("id"))),
                "RepoBuildFailed" =>
                    WithLink(
                        $"{level} | Repo build for {r.GetString("name")} failed",
                        ResourceLink("repo", r.GetString("id"))),
                "ProcedureFailed" =>
                    WithLink(
                        $"{level} | Procedure {r.GetString("name")} failed",
                        ResourceLink("procedure", r.GetString("id"))),
                "ActionFailed" =>
                    WithLink(
                        $"{level} | Action {r.GetString("name")} failed",
                        ResourceLink("action", r.GetString("id"))),
                "ScheduleRun" =>
                    WithLink(
                        $"{level} | {r.GetString("name")} ({r.GetString("resource_type")}) | Scheduled run started 🕝",
                        ResourceLink(r.GetString("resource_type"), r.GetString("id"))),
                "Custom" =>
                    string.IsNullOrWhiteSpace(r.GetString("details"))
                        ? $"{level} | {r.GetString("message")}"
                        : $"{level} | {r.GetString("message")}\n{r.GetString("details")}",
                "None" => string.Empty,
                _ => $"{level} | No alert details provided"
            };
        }

        private static double Percent(double used, double total) =>
            total <= 0 ? 0 : (used / total) * 100.0;

        private static string FmtLevel(SeverityLevel level) =>
            level switch
            {
                SeverityLevel.Critical => "CRITICAL \u1F6A8",
                SeverityLevel.Warning => "WARNING \u26A0\uFE0F",
                SeverityLevel.Ok => "OK \u2705",
                _ => "\u2753"
            };

        private static string FmtRegion(string? region) =>
            string.IsNullOrWhiteSpace(region) ? string.Empty : $" ({region})";

        private static string FmtDockerContainerState(string? state) =>
            string.IsNullOrWhiteSpace(state) ? "unknown" : state.Replace('_', ' ');

        private static string FmtStackState(string? state) =>
            string.IsNullOrWhiteSpace(state) ? "unknown" : state.Replace('_', ' ');

        private static string FmtImagesLabel(IEnumerable<string> images)
        {
            var list = images?.Where(s => !string.IsNullOrWhiteSpace(s)).ToArray() ?? Array.Empty<string>();
            if (list.Length == 0) return "image: -";
            var label = list.Length > 1 ? "images" : "image";
            return $"{label}: {string.Join(", ", list)}";
        }

        private static string FmtPath(string? path) =>
            path is null ? "null" : $"\"{path}\"";

        private static string WithLink(string text, string link) =>
            string.IsNullOrWhiteSpace(link) ? text : $"{text}\n{link}";

        private static string ResourceLink(string? variant, string? id)
        {
            if (string.IsNullOrWhiteSpace(id)) return string.Empty;
            var baseUrl = Environment.GetEnvironmentVariable("KOMODO_APP_URL");
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                return $"id: {id}";
            }

            var slug = string.IsNullOrWhiteSpace(variant) ? "resource" : variant.ToLowerInvariant();
            return $"{baseUrl.TrimEnd('/')}/{slug}/{id}";
        }

        private static string FmtError(JsonElement data) =>
            data.TryGetProperty("err", out var err) && err.ValueKind != JsonValueKind.Null && err.ValueKind != JsonValueKind.Undefined
                ? $"\nerror: {err}"
                : string.Empty;

        public static object CreateBarkPayload(
            Alert alert,
            string[] deviceKeys,
            string? prefix,
            string? group,
            string? icon,
            string? url,
            string? sound)
        {
            var title = Title(alert);
            var subtitle = Subtitle(alert);
            if (!string.IsNullOrWhiteSpace(prefix))
            {
                title = $"{prefix} - {title}";
            }
            var body = Body(alert);
            var level = MapLevel(alert.Level);

            var payload = new Dictionary<string, object?>
            {
                ["title"] = title,
                ["subtitle"] = subtitle,
                ["body"] = body,
                ["device_keys"] = deviceKeys,
                ["level"] = level,
                ["action"] = "none"
            };

            if (!string.IsNullOrWhiteSpace(group))
            {
                payload["group"] = group;
            }

            if (!string.IsNullOrWhiteSpace(icon))
            {
                payload["icon"] = icon;
            }

            if (!string.IsNullOrWhiteSpace(url))
            {
                payload["url"] = url;
                payload.Remove("action");
            }

            if (IsValidSound(sound, out var normalizedSound))
            {
                payload["sound"] = normalizedSound;
            }

            return payload;
        }

        private static bool IsValidSound(string? sound, out string normalized)
        {
            normalized = string.Empty;
            if (string.IsNullOrWhiteSpace(sound)) return false;
            var trimmed = sound.Trim();
            if (!AllowedSounds.Contains(trimmed)) return false;
            normalized = trimmed;
            return true;
        }
    }
}
