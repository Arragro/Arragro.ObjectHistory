import { combineReducers, Reducer } from 'redux'
import { StoreState } from './state'

import {
    objectHistory
} from './modules'
import { ActionType } from 'typesafe-actions'
import { Actions } from './modules/global/actions'

export type ReducerActions = ActionType<typeof Actions>

export const rootReducer: Reducer<StoreState> = combineReducers<StoreState, ReducerActions>({
    objectHistory
})
