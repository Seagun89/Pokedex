import json
import os

import aio_pika
from aio_pika import IncomingMessage, Message
from dotenv import load_dotenv

load_dotenv()


class RabbitMQConnection:
    def __init__(self, bot) -> None:
        self.host = os.getenv("RABBITMQ_HOST", "localhost")
        self.port = int(os.getenv("RABBITMQ_PORT", "5672"))
        self.username = os.getenv("RABBITMQ_USERNAME", "guest")
        self.password = os.getenv("RABBITMQ_PASSWORD", "guest")
        self.request_queue_name = "PokeDex_ChatBot_Request"
        self.response_queue_name = "PokeDex_ChatBot_Response"
        self.connection = None
        self.channel = None
        self.request_queue = None
        self.response_queue = None
        self.bot = bot

    async def connect(self) -> None:
        connection_url = (
            f"amqp://{self.username}:{self.password}@{self.host}:{self.port}/"
        )
        try:
            self.connection = await aio_pika.connect_robust(connection_url)
            self.channel = await self.connection.channel()
            await self.channel.set_qos(prefetch_count=1)
            self.request_queue = await self.channel.declare_queue(
                self.request_queue_name,
                durable=True,
                arguments={"x-queue-type": "quorum"},
            )
            self.response_queue = await self.channel.declare_queue(
                self.response_queue_name,
                durable=True,
                arguments={"x-queue-type": "quorum"},
            )
            print(f"Connected to RabbitMQ at {self.host}:{self.port}")
        except Exception as exc:
            print(f"Error connecting to RabbitMQ: {exc}")
            raise

    async def publish_response(self, payload: dict) -> None:
        if self.channel is None:
            raise RuntimeError("RabbitMQ channel is not available")

        body = json.dumps(payload).encode("utf-8")
        message = Message(
            body=body,
            delivery_mode=aio_pika.DeliveryMode.PERSISTENT,
        )
        await self.channel.default_exchange.publish(
            message,
            routing_key=self.response_queue_name,
        )
        print(f"Published response to {self.response_queue_name}")

    async def handle_message(self, message: IncomingMessage) -> None:
        async with message.process():
            try:
                payload = json.loads(message.body.decode("utf-8"))
            except Exception:
                payload = {"message": message.body.decode("utf-8", errors="ignore")}

            user_message = payload.get("UserMessage", "") # change to usermessage
            reply = self.bot.get_reply(user_message)

            response_payload = {
                "CorrelationId": payload.get("CorrelationId"),
                "Data": {"Message": reply}, # message
            }
            await self.publish_response(response_payload)

    async def start_consuming(self) -> None:
        if self.request_queue is None:
            raise RuntimeError("Request queue has not been created. Call connect() first.")

        await self.request_queue.consume(self.handle_message)
        print(f"Listening for RabbitMQ messages on {self.request_queue_name}")

    async def close(self) -> None:
        if self.connection is not None:
            await self.connection.close()
            print("RabbitMQ connection closed")
