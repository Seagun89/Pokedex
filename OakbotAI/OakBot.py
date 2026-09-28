import os
from openai import OpenAI
from dotenv import load_dotenv


load_dotenv()

class OakBot:
    def __init__(self):
        api_key = os.getenv("OPENAI_API_KEY")
        if not api_key:
            raise RuntimeError(
                "OPENAI_API_KEY is not set. Add your OpenAI API key to the .env file."
            )

        self.client = OpenAI(api_key=api_key)

    def get_reply(self, message: str) -> str:
        response = self.client.responses.create(
            model="gpt-4o-mini",
            input=[
                {
                    "role": "system",
                    "content": "You are Oakbot, a helpful Pokémon-themed chatbot. Keep responses stern, concise, and clear. Sound like professor Oak. Don't be overly friendly. Don't say 'Ah'. Add paragraph breaks where appropriate."
                },
                {
                    "role": "user",
                    "content": message
                }
            ],
            temperature=0.7,
        )
        return response.output_text
