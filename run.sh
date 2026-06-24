#!/bin/bash
# H-Editor 로컬 실행 스크립트
# 브라우저에서 http://localhost:5200 으로 접속하세요.

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
cd "$SCRIPT_DIR"

echo "H-Editor 시작 중..."
echo "브라우저: http://localhost:5200"
echo "종료: Ctrl+C"
echo ""

dotnet run
