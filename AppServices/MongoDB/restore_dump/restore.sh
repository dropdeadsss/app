#!/bin/sh

BACKUP_DIR="/MongoDB/restore_dump/db_dump"
DATA_DIR="/data/Appdb"
PORT=27900
MONGO_BIN="/usr/bin/mongod"
RESTORE_BIN="/usr/bin/mongorestore"

set -eux

if [ -d "$DATA_DIR" ] && [ "$(ls -A "$DATA_DIR")" ]; then
    echo "Тестовые данные уже есть"
    "$MONGO_BIN" --port "$PORT" --dbpath "$DATA_DIR" --bind_ip 0.0.0.0 --logpath /var/log/mongodb.log
else
    echo "Данных нет. Создаю директорию и восстанавливаю базу..."
    mkdir -p "$DATA_DIR"
    "$MONGO_BIN" --port "$PORT" --dbpath "$DATA_DIR" --bind_ip 0.0.0.0 --logpath /var/log/mongodb.log --fork
    sleep 15
    echo "Начинаю восстановление базы..."
    "$RESTORE_BIN" --gzip --host 0.0.0.0 --port "$PORT" "$BACKUP_DIR"
    "$MONGO_BIN" --shutdown --dbpath "$DATA_DIR"
    echo "Тестовые данные добавлены"
    "$MONGO_BIN" --port "$PORT" --dbpath "$DATA_DIR" --bind_ip 0.0.0.0 --logpath /var/log/mongodb.log
fi