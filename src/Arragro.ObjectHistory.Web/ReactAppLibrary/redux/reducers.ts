import { combineReducers, Reducer } from 'redux'
import { StoreState } from './state'

import {
    global,
    GlobalAction
} from './modules'

export { StoreState }

export type ReducerActions = GlobalAction

export const rootReducer: Reducer<StoreState> = combineReducers<StoreState, ReducerActions>({
    global
})
