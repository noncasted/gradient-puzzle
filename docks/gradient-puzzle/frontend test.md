
server {
    root  /usr/share/nginx/html;
    include /etc/nginx/mime.types;

    types {
        application/wasm wasm;
        application/javascript js;
        application/octet-stream data;
    }

    gzip on;
    gzip_types text/css application/javascript application/json image/svg+xml application/wasm;
    gzip_proxied no-cache no-store private expired auth;
    gzip_comp_level 6;
    etag on;

    location / {
        index  index.html index.htm;
        try_files $uri $uri/ /index.html;
    }

    # Serve pre-compressed Brotli (.br) files correctly
    location ~* \.wasm\.br$ {
        add_header Content-Encoding br;
        add_header Vary Accept-Encoding;
        default_type application/wasm;
    }

    location ~* \.js\.br$ {
        add_header Content-Encoding br;
        add_header Vary Accept-Encoding;
        default_type application/javascript;
    }

    location ~* \.data\.br$ {
        add_header Content-Encoding br;
        add_header Vary Accept-Encoding;
        default_type application/octet-stream;
    }

    location ~* \.html$ {
        expires 1h;
        add_header Cache-Control "public, max-age=3600";
    }

    location ~* \.(css|js|wasm|data|json)$ {
        expires 1y;
        add_header Cache-Control "public, max-age=31536000, immutable";
    }

    location /assets/ {
        expires 1y;
        add_header Cache-Control "public, max-age=31536000";
    }
}