namespace KomodoBarkAlerter.Model
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
            //var level = alert.Level.ToString().ToUpperInvariant();
            var level = string.Empty;
            switch (alert.Level) {
                case SeverityLevel.Critical:
                    level = "\u274C";
                    break;
                case SeverityLevel.Warning:
                    level = "\u26A0\uFE0F";
                    break;
                case SeverityLevel.Ok:
                    level = "\u2705";
                    break;
            }
            var r = alert.Data;
            return r.Type switch
            {
                "Test" => $"{level} | Test alerter {r.GetString("name")} ({r.GetString("id")})",
                "ServerVersionMismatch" => $"{level} | {r.GetString("name")}{FmtRegion(r.GetString("region"))} | server: {r.GetString("server_version")} | core: {r.GetString("core_version")}",
                "ServerUnreachable" => $"{level} | {r.GetString("name")}{FmtRegion(r.GetString("region"))} is unreachable {r.GetString("err.error")}",
                "ServerCpu" => $"{level} | {r.GetString("name")}{FmtRegion(r.GetString("region"))} CPU at {r.GetDouble("percentage"):0.0}%",
                "ServerMem" =>
                    $"{level} | {r.GetString("name")}{FmtRegion(r.GetString("region"))} memory at {Percent(r.GetDouble("used_gb"), r.GetDouble("total_gb")):0.0}% ({r.GetDouble("used_gb"):0.0} GiB / {r.GetDouble("total_gb"):0.0} GiB)",
                "ServerDisk" =>
                    $"{level} | {r.GetString("name")}{FmtRegion(r.GetString("region"))} disk at {Percent(r.GetDouble("used_gb"), r.GetDouble("total_gb")):0.0}% (path: {r.GetString("path")}) using {r.GetDouble("used_gb"):0.0} GiB / {r.GetDouble("total_gb"):0.0} GiB",
                "ContainerStateChange" =>
                    $"{level} | Deployment {r.GetString("name")} on {r.GetString("server_name")} is now {r.GetString("to")} (previously {r.GetString("from")})",
                "DeploymentImageUpdateAvailable" =>
                    $"{level} | Deployment {r.GetString("name")} on {r.GetString("server_name")} has an update available for {r.GetString("image")}",
                "DeploymentAutoUpdated" =>
                    $"{level} | Deployment {r.GetString("name")} on {r.GetString("server_name")} was auto-updated to {r.GetString("image")}",
                "StackStateChange" =>
                    $"{level} | Stack {r.GetString("name")} on {r.GetString("server_name")} is now {r.GetString("to")} (previously {r.GetString("from")})",
                "StackImageUpdateAvailable" =>
                    $"{level} | Stack {r.GetString("name")} on {r.GetString("server_name")} service {r.GetString("service")} update available: {r.GetString("image")}",
                "StackAutoUpdated" =>
                    $"{level} | Stack {r.GetString("name")} on {r.GetString("server_name")} auto-updated: {string.Join(", ", r.GetStringArray("images"))}",
                "AwsBuilderTerminationFailed" =>
                    $"{level} | Failed to terminate AWS builder instance {r.GetString("instance_id")}\n{r.GetString("message")}",
                "ResourceSyncPendingUpdates" =>
                    $"{level} | Pending resource sync updates on {r.GetString("name")}",
                "BuildFailed" =>
                    $"{level} | Build {r.GetString("name")} failed (version v{r.GetVersion()})",
                "RepoBuildFailed" =>
                    $"{level} | Repo build for {r.GetString("name")} failed",
                "ProcedureFailed" =>
                    $"{level} | Procedure {r.GetString("name")} failed",
                "ActionFailed" =>
                    $"{level} | Action {r.GetString("name")} failed",
                "ScheduleRun" =>
                    $"{level} | {r.GetString("name")} ({r.GetString("resource_type")}) scheduled run started",
                "Custom" =>
                    string.IsNullOrWhiteSpace(r.GetString("details"))
                        ? $"{level} | {r.GetString("message")}"
                        : $"{level} | {r.GetString("message")}\n{r.GetString("details")}",
                _ => $"{level} | No alert details provided"
            };
        }

        private static double Percent(double used, double total) =>
            total <= 0 ? 0 : (used / total) * 100.0;

        private static string FmtRegion(string? region) =>
            string.IsNullOrWhiteSpace(region) ? string.Empty : $" ({region})";
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
