# Komodo - Bark Alerter

Small HTTP service that receives Komodo `Alert` payloads (the same JSON sent to custom alerter endpoints) and forwards them to a Bark server. Device keys are configured via environment variable, not per-request query params.

## How it works

- Endpoint: `POST /alert`
- Body: Komodo `Alert` JSON (as sent by `send_custom_alert` in Komodo Core).
- Device keys: loaded from `BARK_DEVICE_KEYS` (comma/semicolon/newline separated). Requests are rejected with `400` if no keys are configured.
- Mapping: The payload is parsed into `AlertData` variants and rendered into a plain-text `title` + `body` Bark request. Komodo severity is mapped to Bark `level` as `CRITICAL -> timeSensitive`, `WARNING -> active`, `OK -> passive`.
- Forwarded to Bark JSON endpoint (default `https://api.day.app/push`, override with `BARK_ENDPOINT`) with configured device keys, optional `group`, title (with optional prefix), body, and level. Optional icon and URL are applied when provided (URL removes the default “none” action).

## Configuration

- `PORT` (default `8080`)
- `BARK_DEVICE_KEYS` (required, comma/semicolon/newline separated list)
- `BARK_ENDPOINT` (default `https://api.day.app/push`)
- `BARK_GROUP` (optional Bark group tag)
- `BARK_ICON` (optional Bark icon URL)
- `BARK_TITLE_PREFIX` (optional prefix added to Bark title)
- `BARK_URL` (optional URL to open from the Bark notification)
- `BARK_ALERT_SOUND` (optional; one of `alarm`, `anticipate`, `bell`, `calypso`, `chime`, `glass`, `horn`, `ladder`, `minuet`, `news`, `noir`, `pulse`, `suspense`, `telegraph`, `healthnotification`, `update`)

## Run locally

```bash
export BARK_DEVICE_KEYS="key1,key2"
# export BARK_ENDPOINT="https://api.day.app/push" # optional override
# export BARK_TITLE_PREFIX="MyPrefix"              # optional title prefix
# export BARK_URL="https://example.com"            # optional tap-through URL
# export BARK_ALERT_SOUND="bell"                   # optional sound (validated list)
dotnet run --project src/KomodoBarkAlerter.csproj --configuration Release

curl -X POST "http://localhost:8080/alert" \
  -H "Content-Type: application/json" \
  -d @sample-alert.json
```

## Docker

```bash
docker build -t bark-komodo-alerter .
docker run --rm -p 8080:8080 \
  -e BARK_DEVICE_KEYS=key1,key2 \
  -e BARK_ENDPOINT=https://api.day.app/push \
  -e BARK_GROUP=komodo \
  ghcr.io/<your-org>/bark-komodo-alerter:latest
```

## Sample payload

This matches the structure produced by Komodo Core:

```json
{
  "ts": 1700000000000,
  "resolved": false,
  "level": "CRITICAL",
  "target": { "type": "Server", "id": "abc123" },
  "data": {
    "type": "ServerUnreachable",
    "data": {
      "id": "abc123",
      "name": "prod-server",
      "region": "eu-central",
      "err": { "error": "timed out", "trace": [] }
    }
  }
}
```

## Notes

- The service responds with `400` if `BARK_DEVICE_KEYS` is missing or empty.
- Bark responses other than 2xx are bubbled back as `502` so Komodo can surface failures.
- `BARK_ALERT_SOUND` is only sent when it matches the predefined list; unknown values are ignored.
