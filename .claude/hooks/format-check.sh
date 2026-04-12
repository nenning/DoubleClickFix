#!/usr/bin/env bash
# PostToolUse hook: verify .editorconfig compliance for edited .cs files

FILE=$(echo "$CLAUDE_TOOL_INPUT" | python -c "import sys,json; print(json.load(sys.stdin).get('file_path',''))" 2>/dev/null)

# Only check .cs files
if [[ "$FILE" != *.cs ]]; then
	exit 0
fi

# Run dotnet format in verify mode against the solution
dotnet format DoubleClickFix.slnx --verify-no-changes --include "$FILE" --severity warn 2>&1
