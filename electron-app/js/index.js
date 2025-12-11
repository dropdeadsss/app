import * as homeControl from './homeControl.js';
//import * as serverControl from './serverControl.js';

const { remote } = require('electron');
const { ipcRenderer } = require('electron');
//const  signalR  = require('@microsoft/signalr');

if(localStorage.getItem('token') == null){
  ipcRenderer.send('logout');
}

const token = localStorage.getItem('token');
localStorage.clear();
localStorage.setItem('token', token);

//setTimeout(homeControl.createHomeHtml, 7000)
homeControl.createHomeHtml();