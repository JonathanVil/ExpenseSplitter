import tailwindcss from '@tailwindcss/vite';
import {sveltekit} from '@sveltejs/kit/vite';
import {defineConfig} from 'vite';

export default defineConfig({
    plugins: [tailwindcss(), sveltekit()],
    server: {
        host: true,
        port: parseInt(process.env.PORT ?? "5173"),
        proxy: {
            '/api': {
                target: process.env.services__api__https__0 || process.env.services__api__http__0,
                changeOrigin: true,
                rewrite: path => path.replace(/^\/api/, ''),
                secure: false
            }
        }
    },
});
