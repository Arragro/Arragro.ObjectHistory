import { ActionType } from 'typesafe-actions'

import * as Interfaces from '../../../interfaces'
import { GlobalState, initialState } from '../../state'
import { Actions } from './actions'

export type GlobalAction = ActionType<typeof Actions>

const testStateAndPayload = (state: GlobalState, payload: any) => {
    if (state.globalQueryResultContainer === undefined ||
        (payload === undefined || payload === null)) {
        throw new Error('The globalQueryResultContainer or payload is broken')
    }
}

const showHideDetails = (state: GlobalState, index: number, expanded: boolean) => {
    let results = state.globalQueryResultContainer!.results

    results[index].expanded = expanded

    return {
        ...state,
        globalQueryResultContainer: {
            ...state.globalQueryResultContainer!,
            results
        }
    }
}

export const global = (state = initialState.global, action: any): GlobalState => {
    switch (action.type) {
    case Actions.Type.GET_GLOBAL_RECORDS_START:
        return {
            ...state,
            loading: true,
            loadingFromToken: false
        }
    case Actions.Type.GET_GLOBAL_RECORDS_SUCCESS:
        return {
            ...state,
            loading: false,
            globalQueryResultContainer: action.payload
        }
    case Actions.Type.GET_GLOBAL_RECORDS_ERROR:
        return {
            ...state,
            loading: false,
            globalQueryResultContainer: undefined
        }
    case Actions.Type.GET_GLOBAL_RECORDS_FROM_TOKEN_START:
        return {
            ...state,
            loading: false,
            loadingFromToken: true
        }
    case Actions.Type.GET_GLOBAL_RECORDS_FROM_TOKEN_SUCCESS: {
        testStateAndPayload(state, action.payload)

        let results = state.globalQueryResultContainer!.results
        let globalQueryResultContainer = action.payload as Interfaces.IObjectHistoryGlobalQueryResultContainer
        for (let i = 0; i < globalQueryResultContainer.results.length; i++) {
            results.push(globalQueryResultContainer.results[i])
        }
        return {
            ...state,
            loadingFromToken: false,
            globalQueryResultContainer: {
                ...state.globalQueryResultContainer!,
                results,
                continuationToken: globalQueryResultContainer.continuationToken
            }
        }
    }
    case Actions.Type.GET_GLOBAL_RECORDS_FROM_TOKEN_ERROR:
        return {
            ...state,
            loadingFromToken: false,
            globalQueryResultContainer: undefined
        }
    case Actions.Type.SHOW_DETAILS_START:
        return {
            ...state,
            loadingDetails: true
        }
    case Actions.Type.SHOW_DETAILS_SUCCESS: {
        testStateAndPayload(state, action.payload)

        let results = state.globalQueryResultContainer!.results
        const index = action.payload.index
        const historyDetail = action.payload.result

        results[index].historyDetail = historyDetail

        return {
            ...state,
            loadingFromToken: false,
            globalQueryResultContainer: {
                ...state.globalQueryResultContainer!,
                results
            }
        }
    }
    case Actions.Type.SHOW_DETAILS_ERROR:
        return {
            ...state,
            loadingDetails: false
        }
    case Actions.Type.SHOW_DETAILS_EXPAND: {
        testStateAndPayload(state, action.payload)
        return showHideDetails(state, action.payload, true)
    }
    case Actions.Type.SHOW_DETAILS_HIDE: {
        testStateAndPayload(state, action.payload)
        return showHideDetails(state, action.payload, false)
    }
    default:
        return state
    }
}
