const CACHE_NAME = 'asg-cache-v8';
const ASSETS_TO_CACHE = [
    '/',
    '/index.html',
    '/css/app.css',
    '/icons/icon-192x192.png',
    '/icons/icon-512x512.png',
    '_framework/blazor.webassembly.js',
    '_framework/dotnet.js',
    '_content/MudBlazor/MudBlazor.min.css',
    '_content/MudBlazor/MudBlazor.min.js',
    'https://www.gstatic.com/firebasejs/9.6.1/firebase-app.js',
    'https://www.gstatic.com/firebasejs/9.6.1/firebase-auth.js'
];

self.addEventListener('message', event => {
    if (event.data === 'update-cache') {
        updateCache();
    }
});

self.addEventListener('install', event => {
    event.waitUntil(
        caches.open(CACHE_NAME).then(cache => {
            console.log('Caching assets');
            return cache.addAll(ASSETS_TO_CACHE).catch(error => {
                console.error('Failed to cache assets:', error);
                ASSETS_TO_CACHE.forEach(async asset => {
                    try {
                        const response = await fetch(asset);
                        if (!response.ok) {
                            throw new Error(`Request for ${asset} failed with status ${response.status}`);
                        }
                    } catch (err) {
                        console.error(`Failed to fetch ${asset}:`, err);
                    }
                });
                throw error;
            });
        })
    );
});

self.addEventListener('activate', event => {
    console.log('Service worker activating...');
    event.waitUntil(
        caches.keys().then(cacheNames => {
            return Promise.all(
                cacheNames.map(cacheName => {
                    if (cacheName !== CACHE_NAME) {
                        console.log('Deleting old cache:', cacheName);
                        return caches.delete(cacheName);
                    }
                })
            );
        })
    );
});

self.addEventListener('fetch', event => {
    if (event.request.method !== 'GET') {
        return;
    }

    const url = new URL(event.request.url);

    if (url.pathname.includes('api/mealplanner/weekly')) {
        event.respondWith(
            (async () => {
                const isRegenerating = await caches.match('isRegenerating');
                if (isRegenerating) {
                    // Network-first approach
                    try {
                        const networkResponse = await fetch(event.request);
                        const cache = await caches.open(CACHE_NAME);
                        cache.put(event.request, networkResponse.clone());
                        return networkResponse;
                    } catch (error) {
                        return caches.match(event.request);
                    }
                } else {
                    // Cache-first approach
                    const cachedResponse = await caches.match(event.request);
                    return cachedResponse || fetch(event.request);
                }
            })()
        );
    } else {
        event.respondWith(
            caches.match(event.request).then(cachedResponse => {
                return cachedResponse || fetch(event.request);
            })
        );
    }
});

function updateCache() {
    caches.open(CACHE_NAME).then(cache => {
        fetch('/api/mealplanner/weekly').then(response => {
            if (response.ok) {
                cache.put('/api/mealplanner/weekly', response);
            }
        });
    });
}