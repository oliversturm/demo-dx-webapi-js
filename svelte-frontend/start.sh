#!/bin/sh

# Remove old files to avoid conflicts
rm -rf /src/.svelte-kit /src/node_modules /src/.pnpm-store /.pnpm-store
pnpm config set store-dir /.pnpm-store
pnpm config set node-linker hoisted
pnpm config set symlink false
pnpm i
pnpm run dev
