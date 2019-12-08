import { TrackRepository } from "../music-server/TrackRepository";
import { MongoClient } from "mongodb";
import YtTrackService from "../music-server/YtTrackService";
import { TrackData } from "../dataModels";

const settings = {
    db: {
        hostAddr: 'localhost',
        port: 27017,
        dbName: 'music'
    },
}

const setChannelToTracks = () => {

    const ytTrackService = new YtTrackService()

    updateAllTracks(async (allTracksFromDb) => {
        
        const allTracksFromYt = await ytTrackService.getWithIds(allTracksFromDb.map(t => t.ytId))

        const updatedTracksFromDb = allTracksFromYt.map(ytTrack => {
            const trackFromDb = allTracksFromDb.find(t => t.ytId === ytTrack.ytID)
            if(!trackFromDb){
                console.log('nije našlo', ytTrack.ytID)
                return undefined
            }
            return { ...trackFromDb, channel: ytTrack.channel }
        })
        .filter(t => t) as TrackData[]

        return updatedTracksFromDb
    })
}

const addDurationToTracks = async () => {

    const ytTrackService = new YtTrackService()

    await updateAllTracks(async (allTracksFromDb) => {
        
        const allTracksFromYt = await ytTrackService.getWithIds(allTracksFromDb.map(t => t.ytId))

        const updatedTracksFromDb = allTracksFromYt.map(ytTrack => {
            const trackFromDb = allTracksFromDb.find(t => t.ytId === ytTrack.ytID)
            if(!trackFromDb){
                console.log('nije našlo', ytTrack.ytID)
                return undefined
            }
            return { ...trackFromDb, /*duration: ytTrack.duration*/ }
        })
        .filter(t => t) as TrackData[]

        return updatedTracksFromDb
    })
}

const updateAllTracks = async (applyUpdates: (list: TrackData[]) => Promise<TrackData[]>) => {    
    const mongoClient = new MongoClient(`mongodb://${settings.db.hostAddr}:${settings.db.port}`)
    await mongoClient.connect()
    const db = mongoClient.db(settings.db.dbName)
    
    const trackCollection = db.collection<TrackData>('tracks')
    
    const allTracksFromDb = await trackCollection.find({}).toArray()
    const updated = await applyUpdates(allTracksFromDb)
    console.log(updated)
    for (const track of updated) {
        await trackCollection.replaceOne({ytId: track.ytId}, track)
    }
}

(async () => {
    await addDurationToTracks()
})()