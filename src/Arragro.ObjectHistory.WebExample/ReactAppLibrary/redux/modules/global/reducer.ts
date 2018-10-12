import { ActionType } from 'typesafe-actions'

import * as Interfaces from '../../../interfaces'
import { ObjectHistoryState, initialState } from '../../state'
import { Actions } from './actions'

export type ObjectHistoryAction = ActionType<typeof Actions>

const testStateAndPayload = (state: ObjectHistoryState, payload: any) => {
    if (state.resultContainer === undefined ||
        (payload === undefined || payload === null)) {
        throw new Error('The globalQueryResultContainer or payload is broken')
    }
}

const showHideDetails = (state: ObjectHistoryState, index: number, expanded: boolean) => {
    let results = state.resultContainer!.results

    results[index].expanded = expanded

    return {
        ...state,
        resultContainer: {
            ...state.resultContainer!,
            results
        }
    }
}

export const objectHistory = (state = initialState.objectHistory, action: any): ObjectHistoryState => {
    switch (action.type) {
    case Actions.Type.GET_GLOBAL_RECORDS_START:
    case Actions.Type.GET_OBJECT_RECORDS_START:
        return {
            ...state,
            loading: true,
            loadingFromToken: false
        }
    case Actions.Type.GET_GLOBAL_RECORDS_SUCCESS:
    case Actions.Type.GET_OBJECT_RECORDS_SUCCESS:
        return {
            ...state,
            loading: false,
            resultContainer: action.payload
        }
    case Actions.Type.GET_GLOBAL_RECORDS_ERROR:
    case Actions.Type.GET_OBJECT_RECORDS_ERROR:
        return {
            ...state,
            loading: false,
            resultContainer: undefined
        }
    case Actions.Type.GET_GLOBAL_RECORDS_FROM_TOKEN_START:
        return {
            ...state,
            loading: false,
            loadingFromToken: true
        }
    case Actions.Type.GET_GLOBAL_RECORDS_FROM_TOKEN_SUCCESS: {
        testStateAndPayload(state, action.payload)

        let results = state.resultContainer!.results
        let resultContainer = action.payload as Interfaces.IObjectHistoryQueryResultContainer
        for (let i = 0; i < resultContainer.results.length; i++) {
            results.push(resultContainer.results[i])
        }
        return {
            ...state,
            loadingFromToken: false,
            resultContainer: {
                ...state.resultContainer!,
                results,
                continuationToken: resultContainer.continuationToken
            }
        }
    }
    case Actions.Type.GET_GLOBAL_RECORDS_FROM_TOKEN_ERROR:
        return {
            ...state,
            loadingFromToken: false,
            resultContainer: undefined
        }
    case Actions.Type.SHOW_DETAILS_START:
        return {
            ...state,
            loadingDetails: true
        }
    case Actions.Type.SHOW_DETAILS_SUCCESS: {
        testStateAndPayload(state, action.payload)

        let results = state.resultContainer!.results
        const index = action.payload.index
        const historyDetail = action.payload.result

        results[index].historyDetail = historyDetail

        return {
            ...state,
            loadingFromToken: false,
            resultContainer: {
                ...state.resultContainer!,
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
