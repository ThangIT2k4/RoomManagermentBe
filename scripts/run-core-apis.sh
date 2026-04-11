#!/usr/bin/env bash
# Start Gateway (5200), Notification (5201), External (5204) with http launch profiles.
# Requires: Redis (Gateway), RabbitMQ (Notification + External), .env.local at repo root if you use it.
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$ROOT"

for p in 5200 5201 5204; do
  if ss -tlnp 2>/dev/null | grep -q ":$p "; then
    echo "Port $p is already in use. Stop the other process first:"
    echo "  ss -tlnp | grep :$p"
    exit 1
  fi
done

echo "Starting Gateway, Notification, External (Ctrl+C stops this script; use 'kill' on PIDs if processes keep running)..."
dotnet run --project "$ROOT/Api/RoomManagerment.Gateway/RoomManagerment.Gateway.csproj" --launch-profile http &
dotnet run --project "$ROOT/Modules/Notification/Notification.API/Notification.API.csproj" --launch-profile http &
dotnet run --project "$ROOT/Api/RoomManagerment.ExternalApi/RoomManagerment.ExternalApi.csproj" --launch-profile http &
wait
