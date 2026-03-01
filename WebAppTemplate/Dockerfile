FROM mcr.microsoft.com/dotnet/sdk:10.0 as sdk

RUN apt-get update && \
    apt-get install -y python3 && \
    rm -rf /var/lib/apt/lists/*

RUN dotnet workload install wasm-tools

ARG C=Release
ARG ENV=Production
WORKDIR /src

# Copy only project and solution files for restore
COPY MyApp.WebUI/MyApp.WebUI.csproj MyApp.WebUI/
COPY MyApp.Application/MyApp.Application.csproj MyApp.Application/
COPY MyApp.Infrastructure/MyApp.Infrastructure.csproj MyApp.Infrastructure/
COPY Directory.Build.props .
COPY Directory.Build.targets .

# Restore as distinct layer
RUN dotnet restore ./MyApp.WebUI/MyApp.WebUI.csproj

# Copy everything else
COPY MyApp.WebUI/ MyApp.WebUI/
COPY MyApp.Application/ MyApp.Application/
COPY MyApp.Infrastructure/ MyApp.Infrastructure/

# Build and publish
RUN dotnet publish \
    ./MyApp.WebUI/MyApp.WebUI.csproj \
    -c ${C} \
    -p:WasmApplicationEnvironmentName=${ENV} \
    -o /app/publish

FROM docker.io/library/nginx:stable-alpine-slim

# Prepare directories and permissions for random, non-root UID
# OpenShift typically runs the container as a random UID in group 0 (root).
# We chgrp to 0 and use chmod g=u so that any user in group 0 has same perms as owner.
RUN mkdir -p /var/cache/nginx \
    && mkdir -p /var/run/nginx \
    && mkdir -p /var/log/nginx \
    && mkdir -p /etc/nginx/conf.d \
    && mkdir -p /run \
    && chgrp -R 0 /var/cache/nginx /var/run/nginx /var/log/nginx /etc/nginx /usr/share/nginx/html /run \
    && chmod -R g=u /var/cache/nginx /var/run/nginx /var/log/nginx /etc/nginx /usr/share/nginx/html /run 

WORKDIR /usr/share/nginx/html

# Clean default content & config
RUN rm -rf ./* && rm /etc/nginx/conf.d/default.conf

# Remove 'user' directive from nginx.conf if present
RUN sed -i '/^\s*user\s\+/d' /etc/nginx/nginx.conf

# Copy your SPA nginx config
COPY spa.conf /etc/nginx/conf.d/spa.conf

# Ensure config is also group-readable/writable if needed
RUN chgrp -R 0 /etc/nginx/conf.d/spa.conf \
    && chmod -R g=u /etc/nginx/conf.d/spa.conf

# Copy published Blazor WASM files
COPY --from=sdk /app/publish/wwwroot .

# Expose non-privileged port (must match spa.conf 'listen' directive)
EXPOSE 8080

# Run nginx in foreground
CMD ["nginx", "-g", "daemon off;"]

# podman build --build-arg ENV=Development -t loopercorp/myapp:latest -f ./Dockerfile .
# podman run -d --rm -p 8000:8080 --name myapp-webui loopercorp/myapp:latest
