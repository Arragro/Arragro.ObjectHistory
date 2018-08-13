import { ActionType } from 'typesafe-actions'

import * as Interfaces from '../../../interfaces'
import { GlobalState, initialState } from '../../state'
import { Actions } from './actions'

export type GlobalAction = ActionType<typeof Actions>

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
    case Actions.Type.GET_GLOBAL_RECORDS_FROM_TOKEN_SUCCESS:
        if (state.globalQueryResultContainer !== undefined &&
            (action.payload !== undefined || action.payload !== null)) {

            let results = state.globalQueryResultContainer.results
            let globalQueryResultContainer = action.payload as Interfaces.IObjectHistoryGlobalQueryResultContainer
            for (let i = 0; i < globalQueryResultContainer.results.length; i++) {
                results.push(globalQueryResultContainer.results[i])
            }
            return {
                ...state,
                loadingFromToken: false,
                globalQueryResultContainer: {
                    ...state.globalQueryResultContainer,
                    results,
                    continuationToken: globalQueryResultContainer.continuationToken
                }
            }
        }
        return {
            ...state,
            loadingFromToken: false,
            globalQueryResultContainer: undefined
        }
    case Actions.Type.GET_GLOBAL_RECORDS_FROM_TOKEN_ERROR:
        return {
            ...state,
            loadingFromToken: false,
            globalQueryResultContainer: undefined
        }
    default:
        return state
    }
}
