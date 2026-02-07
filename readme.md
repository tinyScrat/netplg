# Read Me

```sh
podman run -d --rm \
    --hostname rabbitmq \
    --name rabbitmq \
    -v RabbitMQData:/var/lib/rabbitmq \
    -p 5672:5672 \
    -p 15672:15672 \
    -e RABBITMQ_DEFAULT_USER=rabbit \
    -e RABBITMQ_DEFAULT_PASS=rabbit \
    docker.io/library/rabbitmq:management-alpine
```
