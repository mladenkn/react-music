const { app, BrowserWindow, shell } = require('electron')

let mainWindow

console.log('music-electron')

function createWindow() {
  mainWindow = new BrowserWindow({
    width: 1300,
    height: 1000,
    webPreferences: {
      nodeIntegration: true,
    },
    icon: __dirname + '/music.png'
  })

  const appUrl = 'http://localhost:3000';

  mainWindow.loadURL(appUrl)

  mainWindow.on('closed', () => {
    mainWindow = null
  })
}

app.on('ready', createWindow)

app.on('window-all-closed', () => {
  if (process.platform !== 'darwin') {
    app.quit()
  }
})

app.on('activate', () => {
  if (mainWindow === null) {
    createWindow()
  }
})

app.on('web-contents-created', (e, webContents) => {
  webContents.on('will-navigate', (event, url) => {
    if(url.startsWith('http')){
      event.preventDefault()
      shell.openExternal(url)
    }    
  })
});