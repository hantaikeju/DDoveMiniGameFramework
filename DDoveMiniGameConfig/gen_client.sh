#!/usr/bin/env bash
set -euo pipefail
export DOTNET_ROLL_FORWARD=LatestMajor
SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
LUBAN_DLL="$SCRIPT_DIR/Tools/Luban/Luban.dll"
CONF_ROOT="$SCRIPT_DIR"
REPO_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"
CODE_DIR="$REPO_ROOT/DDoveMiniGameClient/Assets/Game/Generate/Luban"
DATA_DIR="$REPO_ROOT/DDoveMiniGameClient/Assets/GameRes/Cfg"

dotnet "$LUBAN_DLL" \
    --conf "$CONF_ROOT/luban.conf" \
    -t client \
    -c cs-simple-json \
    -d json \
    -x outputCodeDir="$CODE_DIR" \
    -x outputDataDir="$DATA_DIR"
