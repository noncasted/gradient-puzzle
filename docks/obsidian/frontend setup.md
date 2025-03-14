nginx

server {
    root  /usr/share/nginx/html;
    include /etc/nginx/mime.types;

    location / {
        index  index.html index.htm;
        try_files $uri $uri/ /index.html;
    }

    gzip on;
    gzip_types text/css application/javascript application/json image/svg+xml;
    gzip_proxied no-cache no-store private expired auth;
    gzip_comp_level 6;
    etag on;

    location ~* \.(html)$ {
        expires 1h;
        add_header Cache-Control "public, max-age=3600";
    }

    location ~* \.(css|js)$ {
        expires 1y;
        add_header Cache-Control "public, max-age=31536000, immutable";
    }

    location ~* \.(json)$ {
        expires 1y;
        add_header Cache-Control "public, max-age=31536000";
    }

    location /assets/ {
        expires 1y;
        add_header Cache-Control "public, max-age=31536000";
    }

    location ~* \.br$ {
        add_header Content-Encoding br;
        add_header Vary Accept-Encoding;
        default_type application/octet-stream;
    }
}
