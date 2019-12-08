import { MongoClient, Db } from "mongodb";
import { TrackRepository } from "./TrackRepository";
import { TrackService } from "./TrackService";
import YtTrackService from "./YtTrackService";

const settings = {
    hostAddr: 'localhost',
    port: 27017,
    dbName: 'music-testing'
}

const initDb = async () => {
    const mongoClient = new MongoClient(`mongodb://${settings.hostAddr}:${settings.port}`)
    await mongoClient.connect()
    const db = mongoClient.db(settings.dbName)
    await db.dropDatabase()
    return mongoClient.db(settings.dbName)
}

let db: Db

(async () => {
    db = await initDb()
})()

describe('TrackService', () => {

    beforeEach(async () => {
        const tracksRepo = new TrackRepository(db)
        const ytTrackServie = new YtTrackService()
        const trackService = new TrackService(tracksRepo, ytTrackServie)
    });

    test('queryYoutube', () => {
        
    })
})