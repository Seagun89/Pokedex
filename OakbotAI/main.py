import asyncio
from OakBot import OakBot
from RabbitMQConnection import RabbitMQConnection


async def main() -> None:
    oakbot = OakBot()
    rabbit = RabbitMQConnection(bot=oakbot)
    await rabbit.connect()
    await rabbit.start_consuming()

    try:
        await asyncio.Event().wait()
    finally:
        await rabbit.close()


if __name__ == "__main__":
    asyncio.run(main())