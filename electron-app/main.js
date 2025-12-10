const { app, BrowserWindow } = require('electron');
const { ipcMain } = require('electron');
const Store = require('electron-store').default;
const path = require('node:path');

const store = new Store();

let authWindow;
let mainWindow;
let serverWindow;

function createAuthWindow() {
  authWindow = new BrowserWindow({
    width: 400,
    height: 600,
    autoHideMenuBar: true,
    webPreferences: {
      nodeIntegration: true,
      contextIsolation: false,
    }
  });
  authWindow.loadFile('auth.html');
}

function createMainWindow() {
  mainWindow = new BrowserWindow({
    width: 1200,
    minWidth: 1200,
    height: 800,
    minHeight: 800,
    autoHideMenuBar: true,
    webPreferences: {
      nodeIntegration: true,
      contextIsolation: false,
      //preload: `${__dirname}/preload.js`,
      experimentalFeatures: true
    }
  });
  mainWindow.loadFile('index.html');
}

ipcMain.on('server-pick', () =>{
    createServerWindow();
});

ipcMain.on('login-success', (event, token) => {
  store.set('token', token);

  if (authWindow) {
    authWindow.close();
    authWindow = null;
  }
  createMainWindow();
});

ipcMain.on('logout', () => {
  store.delete('token');
  if (mainWindow) {
    mainWindow.close();
    mainWindow = null;
  }
  createAuthWindow();
});


app.whenReady().then(() => {
  const token = store.get('token');
  if (token) {
    createMainWindow();
  } else {
    createAuthWindow();
  }
});

app.on('window-all-closed', () => {
  if (process.platform !== 'darwin') app.quit();
});