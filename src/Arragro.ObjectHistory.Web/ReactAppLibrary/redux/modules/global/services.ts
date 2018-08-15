import { Dispatch, AnyAction } from 'redux'
import { ObjectHistoryState } from '../../state'

import { Actions } from './actions'
import * as Interfaces from '../../../interfaces'

import {
    HistoryService
} from '../../../services'

export namespace Services {

    export interface ObjectHistoryConnectedState {
        objectHistory: ObjectHistoryState
    }

    export interface ObjectHistoryConnectedDispatch {
        get (): void
        getObjectRecord (objectLogPostParameters: Interfaces.IObjectLogsPostParameters): void
        getFromToken (continuationToken: Interfaces.ITableContinuationToken): void
        getDetails (index: number, folder: string): void
        showDetailsExpand (index: number): void
        showDetailsHide (index: number): void
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

    function getDetails (index: number, folder: string): any {
        return function (dispatch: (action: any) => void) {
            dispatch(Actions.showDetailsStart())

            HistoryService.getObjectLog(folder)
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

    export const dispatchServices = (dispatch: Dispatch<AnyAction>): ObjectHistoryConnectedDispatch => {
        return {
            get: () => dispatch(get()),
            getObjectRecord: (objectLogPostParameters: Interfaces.IObjectLogsPostParameters) => dispatch(getObjectRecord(objectLogPostParameters)),
            getFromToken: (continuationToken: Interfaces.ITableContinuationToken) => dispatch(getFromToken(continuationToken)),
            getDetails: (index: number, folder: string) => dispatch(getDetails(index, folder)),
            showDetailsExpand: (index: number) => dispatch(showHideDetails(index, true)),
            showDetailsHide: (index: number) => dispatch(showHideDetails(index, false))
        }
    }
}
