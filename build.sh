#!/usr/bin/env bash
# HappyFrogger build script
# Requires: tools/tailwindcss binary (see README for download instructions)
# Run from the happyfrogger directory. Pass --blog <path> to specify the content
# directory (defaults to ../blog relative to this script).
set -e

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
TAILWIND="$SCRIPT_DIR/tools/tailwindcss"

# Resolve blog directory: --blog <path> arg or default ../blog
BLOG_DIR=""
PASS_ARGS=()
while [[ $# -gt 0 ]]; do
    case "$1" in
        --blog) BLOG_DIR="$2"; shift 2 ;;
        *)      PASS_ARGS+=("$1"); shift ;;
    esac
done
BLOG_DIR="${BLOG_DIR:-$(cd "$SCRIPT_DIR/../blog" && pwd)}"

if [ ! -d "$BLOG_DIR" ]; then
    echo "❌ Blog directory not found: $BLOG_DIR"
    echo "   Use --blog <path> to specify the content directory."
    exit 1
fi

if [ -f "$TAILWIND" ]; then
    echo "Building Tailwind CSS..."
    "$TAILWIND" -i "$SCRIPT_DIR/input.css" -o "$BLOG_DIR/output.css" --minify
else
    echo "Warning: Tailwind CLI not found at $TAILWIND"
    echo "Download from https://github.com/tailwindlabs/tailwindcss/releases"
    echo "Place as tools/tailwindcss and run: chmod +x tools/tailwindcss"
    echo "Skipping CSS build — using existing output.css if present."
fi

echo "Generating site..."
cd "$BLOG_DIR" && dotnet run --project "$SCRIPT_DIR/HappyFrog.csproj" -- "${PASS_ARGS[@]}"
