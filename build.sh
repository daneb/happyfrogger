#!/usr/bin/env bash
# HappyFrogger build script
# Requires: tools/tailwindcss binary (see README for download instructions)
set -e

TAILWIND="./tools/tailwindcss"
OUTPUT_CSS="../blog/output.css"

if [ -f "$TAILWIND" ]; then
    echo "Building Tailwind CSS..."
    "$TAILWIND" -i input.css -o "$OUTPUT_CSS" --minify
else
    echo "Warning: Tailwind CLI not found at $TAILWIND"
    echo "Download from https://github.com/tailwindlabs/tailwindcss/releases"
    echo "Place as tools/tailwindcss and run: chmod +x tools/tailwindcss"
    echo "Skipping CSS build — using existing output.css if present."
fi

echo "Generating site..."
dotnet run --project HappyFrog.csproj -- "$@"
