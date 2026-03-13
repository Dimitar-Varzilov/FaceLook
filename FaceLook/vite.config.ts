import { defineConfig } from 'vite';
import path from 'path';
import { glob } from 'glob';

export default defineConfig({
    build: {
        outDir: 'wwwroot/js',
        emptyOutDir: true,
        lib: {
            // Logic to flatten the folder structure
            entry: glob.sync('Scripts/**/*.ts').reduce((entries: Record<string, string>, file) => {
                // 'file' is "Scripts/chat.ts"
                // 'name' becomes just "chat" (stripping 'Scripts/' and the extension)
                const name = path.relative('Scripts', file).replace(/\.[^/.]+$/, "");

                entries[name] = path.resolve(__dirname, file);
                return entries;
            }, {}),
            formats: ['es']
        },
        rollupOptions: {
            output: {
                // [name] will now be "chat" instead of "Scripts/chat"
                entryFileNames: '[name].js',
                chunkFileNames: 'chunks/[name]-[hash].js',
                assetFileNames: 'assets/[name]-[hash][extname]'
            }
        }
    }
});