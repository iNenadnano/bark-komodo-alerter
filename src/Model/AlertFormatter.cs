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
                AlertType.Test => $"Alerter test",
                AlertType.ServerUnreachable => $"Server unreachable",
                AlertType.ServerCpu => $"High CPU",
                AlertType.ServerMem => $"High memory",
                AlertType.ServerDisk => $"Disk usage",
                AlertType.ServerVersionMismatch => $"Version mismatch",
                AlertType.ContainerStateChange => $"Container state",
                AlertType.DeploymentImageUpdateAvailable => $"Deployment update available",
                AlertType.DeploymentAutoUpdated => $"Deployment auto-updated",
                AlertType.StackStateChange => $"Stack state",
                AlertType.StackImageUpdateAvailable => $"Stack update available",
                AlertType.StackAutoUpdated => $"Stack auto-updated",
                AlertType.AwsBuilderTerminationFailed => "AWS builder termination failed",
                AlertType.ResourceSyncPendingUpdates => $"Resource sync pending",
                AlertType.BuildFailed => $"Build failed",
                AlertType.RepoBuildFailed => $"Repo build failed",
                AlertType.ProcedureFailed => $"Procedure failed",
                AlertType.ActionFailed => $"Action failed",
                AlertType.ScheduleRun => $"Scheduled run",
                AlertType.Custom => alert.Data.GetString("message") ?? "Komodo alert",
                _ => "Komodo alert"
            };

        public static string Subtitle(Alert alert) =>
            alert.Data.Type switch
            {
                AlertType.Test => $"{alert.Data.GetString("name")}",
                AlertType.ServerUnreachable => $"{alert.Data.GetString("name")}",
                AlertType.ServerCpu => $"{alert.Data.GetString("name")}",
                AlertType.ServerMem => $"{alert.Data.GetString("name")}",
                AlertType.ServerDisk => $"{alert.Data.GetString("name")}",
                AlertType.ServerVersionMismatch => $"{alert.Data.GetString("name")}",
                AlertType.ContainerStateChange => $"{alert.Data.GetString("name")}",
                AlertType.DeploymentImageUpdateAvailable => $"{alert.Data.GetString("name")}",
                AlertType.DeploymentAutoUpdated => $"{alert.Data.GetString("name")}",
                AlertType.StackStateChange => $"{alert.Data.GetString("name")}",
                AlertType.StackImageUpdateAvailable => $"{alert.Data.GetString("name")}",
                AlertType.StackAutoUpdated => $"{alert.Data.GetString("name")}",
                AlertType.AwsBuilderTerminationFailed => "AWS builder termination failed",
                AlertType.ResourceSyncPendingUpdates => $"{alert.Data.GetString("name")}",
                AlertType.BuildFailed => $"{alert.Data.GetString("name")}",
                AlertType.RepoBuildFailed => $"{alert.Data.GetString("name")}",
                AlertType.ProcedureFailed => $"{alert.Data.GetString("name")}",
                AlertType.ActionFailed => $"{alert.Data.GetString("name")}",
                AlertType.ScheduleRun => $"{alert.Data.GetString("name")}",
                AlertType.Custom => alert.Data.GetString("message") ?? "Komodo alert",
                _ => "Komodo alert"
            };

        public static string Body(Alert alert)
        {
            var level = FmtLevel(alert.Level);
            var r = alert.Data;

            return r.Type switch
            {
                AlertType.Test => WithLink(
                    $"{level} | If you see this message, then Alerter {r.GetString("name")} is working", ""),
                AlertType.ServerVersionMismatch => alert.Level == SeverityLevel.Ok
                    ? WithLink(
                        $"{level} | {r.GetString("name")}{FmtRegion(r.GetString("region"))} | Periphery version now matches Core version ✅", "")
                    : WithLink(
                        $"{level} | {r.GetString("name")}{FmtRegion(r.GetString("region"))} | Version mismatch detected ⚠️\nPeriphery: {r.GetString("server_version")} | Core: {r.GetString("core_version")}", ""),
                AlertType.ServerUnreachable => alert.Level switch
                {
                    SeverityLevel.Ok => WithLink(
                        $"{level} | {r.GetString("name")}{FmtRegion(r.GetString("region"))} is now reachable", ""),
                    SeverityLevel.Critical => $"{WithLink($"{level} | {r.GetString("name")}{FmtRegion(r.GetString("region"))} is unreachable ❌", "")}{FmtError(r.Data)}",
                    _ => $"{WithLink($"{level} | {r.GetString("name")}{FmtRegion(r.GetString("region"))} is unreachable", "")}{FmtError(r.Data)}"
                },
                AlertType.ServerCpu => WithLink(
                    $"{level} | {r.GetString("name")}{FmtRegion(r.GetString("region"))} cpu usage at {r.GetDouble("percentage"):0.0}%",
                    ""),
                AlertType.ServerMem =>
                    WithLink(
                        $"{level} | {r.GetString("name")}{FmtRegion(r.GetString("region"))} memory usage at {Percent(r.GetDouble("used_gb"), r.GetDouble("total_gb")):0.0}%💾\n\nUsing {r.GetDouble("used_gb"):0.0} GiB / {r.GetDouble("total_gb"):0.0} GiB",
                        ""),
                AlertType.ServerDisk =>
                    WithLink(
                        $"{level} | {r.GetString("name")}{FmtRegion(r.GetString("region"))} disk usage at {Percent(r.GetDouble("used_gb"), r.GetDouble("total_gb")):0.0}%💿\nmount point: {FmtPath(r.GetString("path"))}\nusing {r.GetDouble("used_gb"):0.0} GiB / {r.GetDouble("total_gb"):0.0} GiB",
                        ""),
                AlertType.ContainerStateChange =>
                    WithLink(
                        $"📦Deployment {r.GetString("name")} is now {FmtDockerContainerState(r.GetString("to"))}\nserver: {r.GetString("server_name")}\nprevious: {r.GetString("from")}",
                        ""),
                AlertType.DeploymentImageUpdateAvailable =>
                    WithLink(
                        $"⬆ Deployment {r.GetString("name")} has an update available\nserver: {r.GetString("server_name")}\nimage: {r.GetString("image")}",
                        ""),
                AlertType.DeploymentAutoUpdated =>
                    WithLink(
                        $"⬆ Deployment {r.GetString("name")} was updated automatically\nserver: {r.GetString("server_name")}\nimage: {r.GetString("image")}",
                        ""),
                AlertType.StackStateChange =>
                    WithLink(
                        $"🥞 Stack {r.GetString("name")} is now {FmtStackState(r.GetString("to"))}\nserver: {r.GetString("server_name")}\nprevious: {r.GetString("from")}", ""),
                AlertType.StackImageUpdateAvailable =>
                    WithLink(
                        $"⬆ Stack {r.GetString("name")} has an update available\nserver: {r.GetString("server_name")}\nservice: {r.GetString("service")}\nimage: {r.GetString("image")}",
                        ""),
                AlertType.StackAutoUpdated =>
                    WithLink(
                        $"⬆ Stack {r.GetString("name")} was updated automatically ⏫\nserver: {r.GetString("server_name")}\n{FmtImagesLabel(r.GetStringArray("images"))}",
                        ""),
                AlertType.AwsBuilderTerminationFailed =>
                    $"{level} | Failed to terminate AWS builder instance\ninstance id: {r.GetString("instance_id")}\n{r.GetString("message")}",
                AlertType.ResourceSyncPendingUpdates =>
                    WithLink(
                        $"{level} | Pending resource sync updates on {r.GetString("name")}",
                        ""),
                AlertType.BuildFailed =>
                    WithLink(
                        $"{level} | Build {r.GetString("name")} failed\nversion: v{r.GetVersion()}",
                        ""),
                AlertType.RepoBuildFailed =>
                    WithLink(
                        $"{level} | Repo build for {r.GetString("name")} failed",
                        ""),
                AlertType.ProcedureFailed =>
                    WithLink(
                        $"{level} | Procedure {r.GetString("name")} failed",
                        ""),
                AlertType.ActionFailed =>
                    WithLink(
                        $"{level} | Action {r.GetString("name")} failed",
                        ""),
                AlertType.ScheduleRun =>
                    WithLink(
                        $"{level} | {r.GetString("name")} ({r.GetString("resource_type")}) | Scheduled run started 🕝",
                        ""),
                AlertType.Custom =>
                    string.IsNullOrWhiteSpace(r.GetString("details"))
                        ? $"{level} | {r.GetString("message")}"
                        : $"{level} | {r.GetString("message")}\n{r.GetString("details")}",
                AlertType.None => string.Empty,
                _ => $"{level} | No alert details provided"
            };
        }

        private static double Percent(double used, double total) =>
            total <= 0 ? 0 : (used / total) * 100.0;

        private static string FmtLevel(SeverityLevel level) =>
            level switch
            {
                SeverityLevel.Critical => "CRITICAL \u203C\uFE0F",
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
                return string.Empty;
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

            var resourceLink = GetResourceLink(alert);
            if (!string.IsNullOrWhiteSpace(resourceLink))
            {
                payload["url"] = resourceLink;
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

        private static string GetResourceLink(Alert alert)
        {
            var r = alert.Data;
            return r.Type switch
            {
                AlertType.Test => ResourceLink("alerter", r.GetString("id")),
                AlertType.ServerUnreachable or AlertType.ServerCpu or AlertType.ServerMem or AlertType.ServerDisk or AlertType.ServerVersionMismatch
                    => ResourceLink("server", r.GetString("id")),
                AlertType.ContainerStateChange or AlertType.DeploymentImageUpdateAvailable or AlertType.DeploymentAutoUpdated
                    => ResourceLink("deployment", r.GetString("id")),
                AlertType.StackStateChange or AlertType.StackImageUpdateAvailable or AlertType.StackAutoUpdated
                    => ResourceLink("stack", r.GetString("id")),
                AlertType.ResourceSyncPendingUpdates => ResourceLink("resourcesync", r.GetString("id")),
                AlertType.BuildFailed => ResourceLink("build", r.GetString("id")),
                AlertType.RepoBuildFailed => ResourceLink("repo", r.GetString("id")),
                AlertType.ProcedureFailed => ResourceLink("procedure", r.GetString("id")),
                AlertType.ActionFailed => ResourceLink("action", r.GetString("id")),
                AlertType.ScheduleRun => ResourceLink(r.GetString("resource_type"), r.GetString("id")),
                _ => string.Empty
            };
        }
    }
}
