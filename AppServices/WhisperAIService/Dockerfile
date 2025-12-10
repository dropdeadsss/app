FROM python:3.12.12-trixie
RUN apt-get update && \
DEBIAN_FRONTEND=noninteractive apt-get install -y ffmpeg
WORKDIR /app
COPY . .
RUN pip install --no-cache-dir \
fastapi \
uvicorn \
openai-whisper \
python-multipart
EXPOSE 45000
CMD ["uvicorn", "Transcribe:app", "--host", "0.0.0.0", "--port", "45000"]