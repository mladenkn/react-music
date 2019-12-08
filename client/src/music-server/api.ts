import { Express } from 'express'
import { TrackQueryData, TrackData, YoutubeTrackQuery, WithUserKey } from '../dataModels';
import { TrackService } from './TrackService';
import { UserPermissionService } from './UserPermissionService';
import { Optional } from 'utility-types';

export default (app: Express, trackService: TrackService, userPermissionService: UserPermissionService) => {

    app.get('/api/tracks', async (req, res) => {        
        console.log('get /tracks begin')
        console.log(req.query)

        const query = JSON.parse(req.query.query) as TrackQueryData
        const userKey = req.query.userKey as string | undefined
        
        console.log(query)
        console.log(userKey)

        const tracks = await trackService.query(query)        
        const permissions = await userPermissionService.getPermissionsFromKey(userKey)

        res.send({tracks, permissions})
    })

    app.get('/api/tracks/recommendations', async (req, res) => {
        const p = req.query as { videoID: string } & Optional<WithUserKey>
        const permissions = await userPermissionService.getPermissionsFromKey(p.userKey)
        const tracks = await trackService.getRelatedTracks(p.videoID, permissions)
        res.send({tracks, permissions})
    })

    app.get('/api/tracks-yt/', async (req, res) => {
        console.log('get /tracks-yt/ begin')
        
        const q = JSON.parse(req.query.query) as YoutubeTrackQuery
        const userKey = req.query.userKey as string | undefined
        const tracks = await trackService.queryYoutube(q)
        
        console.log(q)
        
        const permissions = await userPermissionService.getPermissionsFromKey(userKey)

        console.log('get /tracks-yt/ end')
        res.send({tracks, permissions})
    })

    app.post('/api/tracks', async (req, res) => {
        const p = req.body as {tracks: TrackData[]} & Optional<WithUserKey>
        console.log('post track begin', p.tracks)
        const permissions = await userPermissionService.getPermissionsFromKey(p.userKey)
        const tracks = await trackService.save(p.tracks, permissions)
        console.log(tracks)
        res.send({tracks, permissions})
    })
    
    // app.post('/home', async (req, res) => {
    //     const state = req.body as PersistableHomeState
    //     await homeSectionService.updateWith(state)
    //     res.sendStatus(200)
    // })
    
    app.get('/home', async (req, res) => {
        console.log('home get')
    })
}
