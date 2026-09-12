#!/usr/bin/env python3
"""Scan DDoveMiniGameWiki/ concepts for required type + status. Skip reserved and _* dirs."""
from __future__ import annotations

import sys
from pathlib import Path

ALLOWED_STATUS = {"draft", "stable", "deprecated"}
SKIP_DIR_NAMES = {"_log", "_tools", "_spec", ".obsidian"}


def repo_root() -> Path:
    return Path(__file__).resolve().parents[1]


def parse_frontmatter(text: str) -> dict[str, str] | None:
    if not text.startswith("---"):
        return None
    lines = text.splitlines()
    if not lines or lines[0].strip() != "---":
        return None
    end = None
    for i in range(1, len(lines)):
        if lines[i].strip() == "---":
            end = i
            break
    if end is None:
        return None
    meta: dict[str, str] = {}
    for line in lines[1:end]:
        if ":" not in line or line.strip().startswith("#"):
            continue
        key, _, raw = line.partition(":")
        key = key.strip()
        if key and key not in meta:
            meta[key] = raw.strip()
    return meta


def is_skipped_dir(path: Path, wiki: Path) -> bool:
    try:
        rel = path.relative_to(wiki)
    except ValueError:
        return True
    return any(part in SKIP_DIR_NAMES for part in rel.parts)


def main() -> int:
    wiki = repo_root() / "DDoveMiniGameWiki"
    errors: list[str] = []
    checked = 0

    for path in sorted(wiki.rglob("*.md")):
        if is_skipped_dir(path, wiki):
            continue
        rel = path.relative_to(wiki).as_posix()
        name = path.name
        text = path.read_text(encoding="utf-8")

        if name == "log.md":
            continue
        if name == "index.md":
            meta = parse_frontmatter(text)
            if rel == "index.md":
                if meta is None or "okf_version" not in meta:
                    errors.append(f"{rel}: 根 index.md 需要 okf_version")
            elif meta is not None and "type" in meta:
                errors.append(f"{rel}: 分类 index.md 不应有 type（保留名，无概念 frontmatter）")
            continue

        meta = parse_frontmatter(text)
        checked += 1
        if meta is None:
            errors.append(f"{rel}: 概念缺少 YAML frontmatter")
            continue
        if not meta.get("type"):
            errors.append(f"{rel}: 缺 type")
        status = meta.get("status")
        if not status:
            errors.append(f"{rel}: 缺 status")
        elif status not in ALLOWED_STATUS:
            errors.append(f"{rel}: status={status!r} 必须是 draft|stable|deprecated")

    if errors:
        print("wiki check failed:")
        for e in errors:
            print(f"  - {e}")
        return 1
    print(f"wiki check ok ({checked} concepts)")
    return 0


if __name__ == "__main__":
    sys.exit(main())
