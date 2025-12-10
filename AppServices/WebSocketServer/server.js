const WebSocket = require('ws');
const crypto = require('crypto');

const wss = new WebSocket.Server({ port: 40000 });
const clients = new Map();

wss.on('connection', (ws) => {

  const id = crypto.randomUUID();
  clients.set(id, ws);

  ws.send(JSON.stringify({ type: 'registered', id: id }));
  console.log(`Client connected: ${id}`);

  ws.on('message', (message) => {
    const data = JSON.parse(message);
    if (data.to && clients.has(data.to)) {
      const targetWs = clients.get(data.to);
      targetWs.send(JSON.stringify({ from: id, data: data.data }));
    } else {
      console.log(`Unknown target ID: ${data.to}`);
    }
  });

  ws.on('close', () => {
    clients.delete(id);
    console.log(`Client disconnected: ${id}`);
  });
});

console.log(`WebSocket signaling server running on`);