import { IObjectHistoryQueryResultContainer } from '../interfaces'

export interface ObjectHistoryState {
    loading: boolean
    loadingFromToken: boolean
    loadingDetails: boolean
    resultContainer?: IObjectHistoryQueryResultContainer
}

export interface StoreState {
    objectHistory: ObjectHistoryState
    // object: ObjectState
}

export const initialState: StoreState = {
    objectHistory: {
        loading: true,
        loadingFromToken: false,
        loadingDetails: false
    }
    // object: {
    //     loading: true,
    //     loadingFromToken: false
    // }
}
