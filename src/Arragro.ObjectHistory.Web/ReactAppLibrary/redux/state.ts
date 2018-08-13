import { IObjectHistoryGlobalQueryResultContainer, IObjectHistoryQueryResultContainer } from '../interfaces'

export interface GlobalState {
    loading: boolean
    loadingFromToken: boolean
    globalQueryResultContainer?: IObjectHistoryGlobalQueryResultContainer
}

export interface ObjectState {
    loading: boolean
    loadingFromToken: boolean
    globalQueryResultContainer?: IObjectHistoryQueryResultContainer
}

export interface StoreState {
    global: GlobalState
    // object: ObjectState
}

export const initialState: StoreState = {
    global: {
        loading: true,
        loadingFromToken: false
    }
    // object: {
    //     loading: true,
    //     loadingFromToken: false
    // }
}
