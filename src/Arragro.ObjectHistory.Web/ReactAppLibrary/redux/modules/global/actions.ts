import { action } from 'typesafe-actions'

import * as Interfaces from '../../../interfaces'
import { Omit } from '../../../utils/helpers'

export namespace Actions {
    export enum Type {
        GET_GLOBAL_RECORDS_START = 'ReactApp/global/GET_GLOBAL_RECORDS_START',
        GET_GLOBAL_RECORDS_SUCCESS = 'ReactApp/global/GET_GLOBAL_RECORDS_SUCCESS',
        GET_GLOBAL_RECORDS_ERROR = 'ReactApp/global/GET_GLOBAL_RECORDS_ERROR',

        GET_GLOBAL_RECORDS_FROM_TOKEN_START = 'ReactApp/global/GET_GLOBAL_RECORDS_FROM_TOKEN_START',
        GET_GLOBAL_RECORDS_FROM_TOKEN_SUCCESS = 'ReactApp/global/GET_GLOBAL_RECORDS_FROM_TOKEN_SUCCESS',
        GET_GLOBAL_RECORDS_FROM_TOKEN_ERROR = 'ReactApp/global/GET_GLOBAL_RECORDS_FROM_TOKEN_ERROR',

        SHOW_DETAILS_START = 'ReactApp/global/SHOW_DETAILS_START',
        SHOW_DETAILS_SUCCESS = 'ReactApp/global/SHOW_DETAILS_SUCCESS',
        SHOW_DETAILS_ERROR = 'ReactApp/global/SHOW_DETAILS_ERROR',

        SHOW_DETAILS_EXPAND = 'ReactApp/global/SHOW_DETAILS_EXPAND',
        SHOW_DETAILS_HIDE = 'ReactApp/global/SHOW_DETAILS_HIDE'
    }

    export const getGlobalHistoryStart = () => action(Type.GET_GLOBAL_RECORDS_START)
    export const getGlobalHistorySuccess = (result: Interfaces.IObjectHistoryGlobalQueryResultContainer) => action(Type.GET_GLOBAL_RECORDS_SUCCESS, result)
    export const getGlobalHistoryError = (response: Interfaces.IFetchResult<any>) => action(Type.GET_GLOBAL_RECORDS_ERROR, response)

    export const getGlobalHistoryFromTokenStart = () => action(Type.GET_GLOBAL_RECORDS_FROM_TOKEN_START)
    export const getGlobalHistoryFromTokenSuccess = (result: Interfaces.IObjectHistoryGlobalQueryResultContainer) => action(Type.GET_GLOBAL_RECORDS_FROM_TOKEN_SUCCESS, result)
    export const getGlobalHistoryFromTokenError = (response: Interfaces.IFetchResult<any>) => action(Type.GET_GLOBAL_RECORDS_FROM_TOKEN_ERROR, response)

    export const showDetailsStart = () => action(Type.SHOW_DETAILS_START)
    export const showDetailsSuccess = (index: number, result: Interfaces.IObjectHistoryDetailRaw) => action(Type.SHOW_DETAILS_SUCCESS, { index, result })
    export const showDetailsError = (response: Interfaces.IFetchResult<any>) => action(Type.SHOW_DETAILS_ERROR, response)

    export const showDetailsExpand = (index: number) => action(Type.SHOW_DETAILS_EXPAND, index)
    export const showDetailsHide = (index: number) => action(Type.SHOW_DETAILS_HIDE, index)
}

export type Actions = Omit<typeof Actions, 'Type'>
