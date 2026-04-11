#!/usr/bin/env bash
# Start every API used by RoomManagermentFe openapitools.json (ports 5201–5208 except 5200),
# then wait until each /openapi/v1.json responds. Idempotent: skips ports already in use.
#
# Requires: local DBs/Rabbit/Redis as each project needs (same as normal dotnet run).
# Usage: from RoomManagermentBe: ./scripts/run-apis-for-openapi-codegen.sh
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$ROOT"

WAIT_SECS="${OPENAPI_APIS_WAIT_SECS:-300}"
SLEEP_SECS="${OPENAPI_APIS_POLL_SECS:-2}"

port_busy() {
  local p="$1"
  ss -tlnp 2>/dev/null | grep -q ":$p " || return 1
  return 0
}

project_for_port() {
  case "$1" in
    5201) echo "Modules/Notification/Notification.API/Notification.API.csproj" ;;
    5202) echo "Modules/Auth/Auth.API/Auth.API.csproj" ;;
    5203) echo "Modules/Finance/Finance.API/Finance.API.csproj" ;;
    5204) echo "Api/RoomManagerment.ExternalApi/RoomManagerment.ExternalApi.csproj" ;;
    5205) echo "Modules/CRM/CRM.API/CRM.API.csproj" ;;
    5206) echo "Modules/Property/Property.API/Property.API.csproj" ;;
    5207) echo "Modules/Lease/Lease.API/Lease.API.csproj" ;;
    5208) echo "Modules/Organization/Organization.API/Organization.API.csproj" ;;
    *) echo "" ;;
  esac
}

# Order: any; parallel start
PORTS=(5201 5202 5203 5204 5205 5206 5207 5208)

for p in "${PORTS[@]}"; do
  if port_busy "$p"; then
    echo "[openapi-codegen] port $p already listening — skip start"
  else
    proj="$(project_for_port "$p")"
    if [[ -z "$proj" || ! -f "$ROOT/$proj" ]]; then
      echo "Missing project for port $p" >&2
      exit 1
    fi
    echo "[openapi-codegen] starting port $p ← $proj"
    dotnet run --project "$ROOT/$proj" --launch-profile http &
  fi
done

URLS=(
  "http://localhost:5201/openapi/v1.json"
  "http://localhost:5202/openapi/v1.json"
  "http://localhost:5203/openapi/v1.json"
  "http://localhost:5204/openapi/v1.json"
  "http://localhost:5205/openapi/v1.json"
  "http://localhost:5206/openapi/v1.json"
  "http://localhost:5207/openapi/v1.json"
  "http://localhost:5208/openapi/v1.json"
)

deadline=$((SECONDS + WAIT_SECS))
last_report="$SECONDS"

while (( SECONDS < deadline )); do
  missing=()
  for url in "${URLS[@]}"; do
    if ! curl -sf --max-time 3 "$url" -o /dev/null; then
      missing+=("$url")
    fi
  done
  if ((${#missing[@]} == 0)); then
    echo "[openapi-codegen] all OpenAPI endpoints OK (${#URLS[@]} URLs)"
    exit 0
  fi
  if ((SECONDS - last_report >= 15)); then
    echo "[openapi-codegen] still waiting (${#missing[@]} missing)…"
    for u in "${missing[@]}"; do echo "  - $u"; done
    last_report=$SECONDS
  fi
  sleep "$SLEEP_SECS"
done

echo "[openapi-codegen] timeout after ${WAIT_SECS}s — not reachable:" >&2
for url in "${URLS[@]}"; do
  if ! curl -sf --max-time 3 "$url" -o /dev/null; then
    echo "  - $url" >&2
  fi
done
echo >&2 "Fix connection strings / Docker deps, or run services manually. Logs: check terminal output of each dotnet run."
exit 1
