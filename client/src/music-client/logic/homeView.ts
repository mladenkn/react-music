import { useHomeLogic } from "./home";
import { useState, useEffect } from "react";
import { TrackDataForm } from "../view/TrackQueryInteractiveForm/rootLogic";
import { YoutubeTrackQueryFormData } from "../view/YoutubeTrackQueryForm";
import { pick, omit } from "ramda";
import { TrackData, UserPermissions } from "../../dataModels";
import { debounce } from "lodash";

export enum QueryTypeSelection {
    TrackData=0, YTTrackData=1,
}

export enum QueryResultSelection {
  QueryResult=0, Recommedations=1
}

const initialTrackDataFormState: TrackDataForm = {
    channel: undefined,
    titleMatch: undefined,
    mustContainAllGenres: [],
    mustContainSomeGenres: [],
    mustContainAllTags: [],
    mustContainSomeTags: [],
    yearSpan: {},
} 

export const useHomeViewLogic = () => {

    const wrapped = useHomeLogic()

	const [state, setState] = useState({    
        querySelection: QueryTypeSelection.TrackData,
        resultSelection: QueryResultSelection.QueryResult,
		trackDataFormState: initialTrackDataFormState,
        youtubeTrackForm: {
            searchQuery: undefined,
            channelTitle: undefined   
        } as YoutubeTrackQueryFormData,
    })

    useEffect(() => {
        exeTrackDataQueryOnChange();
    }, [state.trackDataFormState])

    useEffect(() => {
        exeTrackDataYoutubeQueryOnChange();
    }, [state.youtubeTrackForm])
     
    const setQuerySelection = (value: QueryTypeSelection) => setState({...state, querySelection: value })
    const setResultSelection = (value: QueryResultSelection) => setState({...state, resultSelection: value })
    const setyoutubeTrackForm = (f: YoutubeTrackQueryFormData) => setState({...state, youtubeTrackForm: f })
    
    const trackDataFormChange = (input: TrackDataForm) => {
        setState({ ...state, trackDataFormState: input });
    }

    const exeTrackDataQueryOnChange = debounce(() => wrapped.fetchTracks(state.trackDataFormState), 500)
    const exeTrackDataYoutubeQueryOnChange = debounce(() => wrapped.fetchTracksFromYT(state.youtubeTrackForm), 500)

    const exeQuery = () => {
        if(state.querySelection === QueryTypeSelection.TrackData)
            wrapped.fetchTracks(state.trackDataFormState)
        else if(state.querySelection === QueryTypeSelection.YTTrackData)
            wrapped.fetchTracksFromYT(state.youtubeTrackForm)
    }

    const onTracksScrollToBottom = () => {
        const thereIsMore = wrapped.tracks.data.length < wrapped.tracks.totalCount;
        if(state.querySelection === QueryTypeSelection.TrackData && thereIsMore)
            wrapped.fetchMoreTracks(state.trackDataFormState!)        
    }

    const tracksToDisplay = state.resultSelection === QueryResultSelection.QueryResult ? wrapped.tracks.data : wrapped.trackRecommendations
    const displayedTracks = tracksToDisplay.map(mapTrack(wrapped.userPermissions))
    const tracksTotalCount = wrapped.tracks.totalCount
    
    const onSaveTrack = (trackEditableProps: TrackDataEditableProps & {id: string}) => {
        const track_ = wrapped.allTracks.find(t => t.ytId === trackEditableProps.id)!
        const trackEditablePropsWithoutId = omit(['id'], trackEditableProps)
        const trackUpdated = {...track_, ...trackEditablePropsWithoutId}
        return wrapped.onSaveTrack(trackUpdated)
    }
    const [selectedItemId, setSelectedItemId] = useState<undefined | string>(undefined)
    const onItemClick = setSelectedItemId

    const fetchRecommendationsOf = async (trackId: string) => {
        await wrapped.fetchRecommendationsOf(trackId)
        setState({...state, resultSelection: QueryResultSelection.Recommedations})
    }

    const full = {
        ...wrapped,
        ...state,
        tracksTotalCount,
        fetchRecommendationsOf,
        displayedTracks,
        onSaveTrack,
        selectedItemId,
        onItemClick,
        setQuerySelection,
        setResultSelection,
        setyoutubeTrackForm,
        trackDataFormChange,
        exeQuery,
        onTracksScrollToBottom,
    }

    console.log(full)

    return full
}

export type HomeViewLogic = ReturnType<typeof useHomeViewLogic>

export type TrackViewData = { 
    id: string
    title: string
    image: string
    description: string
    editableProps: TrackDataEditableProps
    canEdit: boolean
    canPlay: boolean
    canFetchRecommendations: boolean
    ytURL: string,
    discogsSearchURL: string
    channel: {
        id: string
        title: string
    }
}

export type TrackDataEditableProps = {
    year?: number
    genres: string[]
    tags: string[]
}

export const mapTrack = (permissions: UserPermissions) => (t: TrackData): TrackViewData => {
    const ytURL = `https://www.youtube.com/watch?v=${t.ytId}`
    const discogsSearchURL = `https://www.discogs.com/search/?q=${t.title}&type=all`
    const editableProps = pick(['year', 'genres', 'tags'], t)
    const rootProps = pick(['title', 'image', 'description', 'channel'], t)

    if(!rootProps.channel)
        console.log('nema channel', rootProps, t)
    if(!t.ytId){
        debugger;
        console.log('nema id', t)
    }

    return { 
        ...rootProps, 
        editableProps, 
        id: t.ytId, 
        canEdit: permissions.canEditTrackData, 
        canFetchRecommendations: permissions.canFetchTrackRecommendations,
        canPlay: true, 
        ytURL, 
        discogsSearchURL 
    }
}