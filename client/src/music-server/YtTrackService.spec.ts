import YtTrackService from "./YtTrackService";

test('YtTrackService', async () => {
    const service = new YtTrackService()
    const results = await service.query('oliver lieb', 3)
    console.log(results)
})