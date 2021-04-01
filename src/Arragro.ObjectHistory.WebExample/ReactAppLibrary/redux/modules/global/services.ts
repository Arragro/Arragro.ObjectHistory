import { Dispatch, AnyAction } from 'redux'
import { ObjectHistoryState } from '../../state'

import { Actions } from './actions'
import * as Interfaces from '../../../interfaces'

import {
    HistoryService
} from '../../../services'

// eslint-disable-next-line @typescript-eslint/no-namespace
export namespace Services {

    export interface ObjectHistoryConnectedState {
        objectHistory: ObjectHistoryState
    }

    export interface ObjectHistoryConnectedDispatch {
        getGlobalHistory (): void
        getObjectRecord (objectLogPostParameters: Interfaces.IObjectLogsPostParameters): void
        getGlobalHistoryFromToken (pagingToken: Interfaces.IPagingToken): void
        getDetails (index: number, paritionKey: string, rowKey: string): void
        showDetailsExpand (index: number): void
        showDetailsHide (index: number): void
        resetState (): void
    }

    function getGlobalHistory (): any {
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

    function getObjectRecord (objectLogPostParameters: Interfaces.IObjectLogsPostParameters): any {
        return function (dispatch: (action: any) => void) {
            dispatch(Actions.getOjectHistoryStart())

            HistoryService.getObjectLogs(objectLogPostParameters)
                .then((response) => {
                    if (!response.ok) {
                        return dispatch(Actions.getObjectHistoryError(response as any))
                    } else {
                        return dispatch(Actions.getObjectHistorySuccess(response.data))
                    }
                }).catch((x: Interfaces.IFetchResult<any>) => {
                    dispatch(Actions.getObjectHistoryError(x))
                })
        }
    }

    function getGlobalHistoryFromToken (pagingToken: Interfaces.IPagingToken): any {
        return function (dispatch: (action: any) => void) {
            dispatch(Actions.getGlobalHistoryFromTokenStart())

            HistoryService.getGlobalLogs(pagingToken)
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

    function getDetails (index: number, paritionKey: string, rowKey: string): any {
        return function (dispatch: (action: any) => void) {
            dispatch(Actions.showDetailsStart())

            HistoryService.getObjectLog(paritionKey, rowKey)
                .then((response) => {
                    if (!response.ok) {
                        return dispatch(Actions.showDetailsError(response as any))
                    } else {
                        return dispatch(Actions.showDetailsSuccess(index, response.data))
                    }
                }).catch((x: Interfaces.IFetchResult<any>) => {
                    dispatch(Actions.showDetailsError(x))
                })
        }
    }

    function showHideDetails (index: number, show: boolean): any {
        return function (dispatch: (action: any) => void) {
            if (show) {
                dispatch(Actions.showDetailsExpand(index))
            } else {
                dispatch(Actions.showDetailsHide(index))
            }
        }
    }

    function resetState (): any {
        return function (dispatch: (action: any) => void) {
            dispatch(Actions.resetState())
        }
    }

    export const dispatchServices = (dispatch: Dispatch<AnyAction>): ObjectHistoryConnectedDispatch => {
        return {
            getGlobalHistory: () => dispatch(getGlobalHistory()),
            getGlobalHistoryFromToken: (pagingToken: Interfaces.IPagingToken) => dispatch(getGlobalHistoryFromToken(pagingToken)),
            getObjectRecord: (objectLogPostParameters: Interfaces.IObjectLogsPostParameters) => dispatch(getObjectRecord(objectLogPostParameters)),
            getDetails: (index: number, paritionKey: string, rowKey: string) => dispatch(getDetails(index, paritionKey, rowKey)),
            showDetailsExpand: (index: number) => dispatch(showHideDetails(index, true)),
            showDetailsHide: (index: number) => dispatch(showHideDetails(index, false)),
            resetState: () => dispatch(resetState())
        }
    }
}
