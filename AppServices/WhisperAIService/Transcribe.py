from fastapi import FastAPI, UploadFile, File
import whisper
import asyncio
import os
import whisper.audio
import tempfile

app = FastAPI()

model = None

@app.on_event("startup")
async def load_model():
    global model
    model = whisper.load_model("turbo")

@app.post("/transcribe")
async def transcribe_audio(file: UploadFile = File(...)):

    contents = await file.read()

    with tempfile.NamedTemporaryFile(delete=False, suffix=".wav") as temp_file:
        temp_file.write(contents)
        temp_filename = temp_file.name
    
    try:     
        audio = whisper.load_audio(temp_filename)
        result = await asyncio.to_thread(model.transcribe, audio)
    finally:
        os.remove(temp_filename)

    return result["text"]