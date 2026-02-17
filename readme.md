# Read Me

```sh
podman run -d --rm \
  --hostname myrabbitmq \
  --name myrabbitmq \
  -e RABBITMQ_DEFAULT_USER=cmtool_local_dev \
  -e RABBITMQ_DEFAULT_PASS=cmtool123SRT \
  -e RABBITMQ_DEFAULT_VHOST=local_dev \
  -p 15672:15672 \
  -p 5672:5672 \
  -v myrabbitmqdata:/var/lib/rabbitmq \
  docker.io/library/rabbitmq:management-alpine

podman run -d --rm \
  --name postgres \
  -e POSTGRES_USER=cmtool_dev \
  -e POSTGRES_PASSWORD=cmtool_dev \
  -e POSTGRES_DB=cmtool_dev \
  -e PGDATA=/var/lib/postgresql/18/docker \
  -v mypostgres:/var/lib/postgresql \
  -p 5432:5432 \
  docker.io/library/postgres:alpine
```
