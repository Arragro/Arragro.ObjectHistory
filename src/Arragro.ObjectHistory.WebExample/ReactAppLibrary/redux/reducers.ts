import { combineReducers, Reducer } from 'redux'
import { StoreState } from './state'

import {
    objectHistory,
    ObjectHistoryAction
} from './modules'

export { StoreState }

export type ReducerActions = ObjectHistoryAction

export const rootReducer: Reducer<StoreState> = combineReducers<StoreState, ReducerActions>({
    objectHistory
})
