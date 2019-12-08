import express from 'express'
import bodyParser from 'body-parser'
import cors from 'cors'
import { TrackService } from './TrackService';
import { MongoClient } from 'mongodb';
import { TrackRepository } from './TrackRepository';
import YtTrackService from './YtTrackService';
import initApi from './api';
import { HomeSectionService } from './HomeSectionService';
import { VariousDataRepository } from './VariousDataRepository';
import path from 'path';
import { UserPermissionService } from './UserPermissionService';
import Keyv from 'keyv'
import { UserPermissionsRepository } from './UserPermissionsRepository';

const settings = {
    db: {
        hostAddr: 'localhost',
        port: 27017,
        dbName: 'music'
    },
    devPort: 8081,
    prodPort: 8082
};

(async () => {
    const app = express()
    app.use(cors())
    app.use(bodyParser.json())
    app.use(bodyParser.urlencoded({ extended: true }))

    const mongoClient = new MongoClient(`mongodb://${settings.db.hostAddr}:${settings.db.port}`)
    await mongoClient.connect()
    const db = mongoClient.db(settings.db.dbName)

    const tracksRepo = new TrackRepository(db)
    const trackService = new TrackService(tracksRepo, new YtTrackService())
    //const keyv = new Keyv(`mongodb://${settings.db.hostAddr}:${settings.db.port}`)
    const homeSectionService = new HomeSectionService(new VariousDataRepository(), trackService)
    initApi(app, trackService, new UserPermissionService(new UserPermissionsRepository(db)))

    const isProd = process.env.NODE_ENV == 'production'

    if(isProd){
        const buildFolder = path.join(__dirname, '..', '..', 'build');
        console.log(buildFolder)
        app.use(express.static(buildFolder));
        app.get('/', (_, res) => 
            res.sendFile(path.join(buildFolder, 'index.html'))
        );
    }

    const port = isProd ? settings.prodPort : settings.devPort
    app.listen(port, () => console.log(`Server spreman, na portu: ${port}.`))
})()