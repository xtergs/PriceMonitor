Run RabbitMQ: \
`docker run -it --rm --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management`

ELK:
`wsl -d docker-desktop sysctl -w vm.max_map_count=262144`