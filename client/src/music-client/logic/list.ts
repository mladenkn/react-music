import { useState } from "react";

export type ListLogicState<TDataModel, TSource> = {
    data: TDataModel[]
    loading: boolean
    canLoadMore: boolean
    source: TSource
}

export type ListLogicProps<TDataModel, TSource> = {
    initialState: ListLogicState<TDataModel, TSource>
}

export const useListLogic = <TDataModel, TSource> (initialState: ListLogicState<TDataModel, TSource>) => {
    const [state, setState] = useState(initialState)

}