import { VariousDataRepository } from "./VariousDataRepository";
import { TrackService } from "./TrackService";
import { TrackData } from "../dataModels";

export class HomeSectionService {

    constructor(private repo: VariousDataRepository, private trackService: TrackService){

    }
    
    // async updateWith(state: PersistableHomeState){
    //     await this.repo.saveHomeSectionState(state)
    // }

    // async getLastPersistedHomeSectionState(): Promise<PersistedHomeState> {
    //     const persistableState = await this.repo.getLastHomeSectionState()
    //     const tracks = await this.trackService.query({...persistableState.trackDataFormState, skip: 0, take: 30})
    //     const trackRecommendations: TrackData[] = []
    //     return {...persistableState, tracks, trackRecommendations}
    // }
}