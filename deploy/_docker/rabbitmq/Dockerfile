FROM rabbitmq:3.8.7-management-alpine

COPY ./certs /etc/rabbitmq/certs
COPY ./plugins ./plugins
RUN rabbitmq-plugins enable --offline rabbitmq_management
RUN rabbitmq-plugins enable rabbitmq_prometheus
RUN rabbitmq-plugins enable --offline rabbitmq_stomp
RUN rabbitmq-plugins enable --offline rabbitmq_web_stomp
RUN rabbitmq-plugins enable --offline rabbitmq_delayed_message_exchange

EXPOSE 5671 5672 15671 15672 15673 15674 61613 15692 15693