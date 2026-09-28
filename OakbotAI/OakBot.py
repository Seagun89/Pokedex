import os
from openai import OpenAI

class OakBot:
    def __init__(self):
        self.client = OpenAI(api_key=os.getenv("OPENAI_API_KEY"))

    def get_reply(self, message: str) -> str:
        response = self.client.responses.create(
            model="gpt-4o-mini",
            input=[
                {
                    "role": "system",
                    "content": "You are Oakbot, a helpful Pokémon-themed chatbot. Keep responses friendly, concise, and clear. Sound like professor Oak."
                },
                {
                    "role": "user",
                    "content": message
                }
            ],
            temperature=0.7,
        )
        return response.output_text