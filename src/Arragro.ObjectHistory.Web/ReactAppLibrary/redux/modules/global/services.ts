import { Dispatch, AnyAction } from 'redux'
import { GlobalState } from '../../state'

import { Actions } from './actions'
import * as Interfaces from '../../../interfaces'

import {
    HistoryService
} from '../../../services'

export namespace Services {

    export interface GlobalConnectedState {
        global: GlobalState
    }

    export interface GlobalConnectedDispatch {
        get (): void
        getFromToken (continuationToken: Interfaces.ITableContinuationToken): void
    }

    function get (): any {
        return function (dispatch: (action: any) => void) {
            dispatch(Actions.getGlobalHistoryStart())

            HistoryService.getGlobalLogs()
                .then((response) => {
                    if (!response.ok) {
                        return dispatch(Actions.getGlobalHistoryError(response as any))
                    } else {
                        return dispatch(Actions.getGlobalHistorySuccess(response.data))
                    }
                }).catch((x: Interfaces.IFetchResult<any>) => {
                    dispatch(Actions.getGlobalHistoryError(x))
                })
        }
    }

    function getFromToken (continuationToken: Interfaces.ITableContinuationToken): any {
        return function (dispatch: (action: any) => void) {
            dispatch(Actions.getGlobalHistoryFromTokenStart())

            HistoryService.getGlobalLogs(continuationToken)
                .then((response) => {
                    if (!response.ok) {
                        return dispatch(Actions.getGlobalHistoryFromTokenError(response as any))
                    } else {
                        return dispatch(Actions.getGlobalHistoryFromTokenSuccess(response.data))
                    }
                }).catch((x: Interfaces.IFetchResult<any>) => {
                    dispatch(Actions.getGlobalHistoryFromTokenError(x))
                })
        }
    }

    export const dispatchServices = (dispatch: Dispatch<AnyAction>): GlobalConnectedDispatch => {
        return {
            get: () => dispatch(get()),
            getFromToken: (continuationToken: Interfaces.ITableContinuationToken) => dispatch(getFromToken(continuationToken))
        }
    }
}
