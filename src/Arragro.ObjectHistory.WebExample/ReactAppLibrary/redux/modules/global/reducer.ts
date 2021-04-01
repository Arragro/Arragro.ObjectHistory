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
    const results = state.resultContainer!.results

    results[index].expanded = expanded

    return {
        ...state,
        resultContainer: {
            ...state.resultContainer!,
            results
        }
    }
}

const appendResultsContainer = (state: ObjectHistoryState, objectHistoryQueryResultContainer?: Interfaces.IObjectHistoryQueryResultContainer) => {
    if (objectHistoryQueryResultContainer === undefined ||
        state.resultContainer === undefined) {
        return objectHistoryQueryResultContainer
    }

    const resultContainer = state.resultContainer === undefined ? { results: [], partitionKey: null, pagingToken: null } : state.resultContainer
    resultContainer.partitionKey = objectHistoryQueryResultContainer.partitionKey
    resultContainer.results.push(...objectHistoryQueryResultContainer.results)
        
    resultContainer.pagingToken = objectHistoryQueryResultContainer.pagingToken

    return resultContainer
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
            resultContainer: appendResultsContainer(state, action.payload)
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

        const results = state.resultContainer!.results
        const resultContainer = action.payload as Interfaces.IObjectHistoryQueryResultContainer
        for (let i = 0; i < resultContainer.results.length; i++) {
            results.push(resultContainer.results[i])
        }
        return {
            ...state,
            loadingFromToken: false,
            resultContainer: {
                ...state.resultContainer!,
                results,
                pagingToken: resultContainer.pagingToken
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

        const results = state.resultContainer!.results
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
    case Actions.Type.RESET_STATE: {
        return initialState.objectHistory
    }
    default:
        return state
    }
}
