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

  function handleLinkClick(event, url){
    event.preventDefault()
    shell.openExternal(url)
  }

  mainWindow.webContents.on('will-navigate', handleLinkClick)
  mainWindow.webContents.on('new-window', handleLinkClick)
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
