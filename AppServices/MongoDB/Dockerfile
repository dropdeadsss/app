FROM mongo:latest
COPY . /MongoDB
EXPOSE 27900
RUN chmod +x /MongoDB/restore_dump/restore.sh
ENTRYPOINT ["/MongoDB/restore_dump/restore.sh"]