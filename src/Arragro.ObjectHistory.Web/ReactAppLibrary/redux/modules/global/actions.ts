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
        GET_GLOBAL_RECORDS_FROM_TOKEN_ERROR = 'ReactApp/global/GET_GLOBAL_RECORDS_FROM_TOKEN_ERROR'
    }

    export const getGlobalHistoryStart = () => action(Type.GET_GLOBAL_RECORDS_START)
    export const getGlobalHistorySuccess = (result: Interfaces.IObjectHistoryGlobalQueryResultContainer) => action(Type.GET_GLOBAL_RECORDS_SUCCESS, result)
    export const getGlobalHistoryError = (response: Interfaces.IFetchResult<any>) => action(Type.GET_GLOBAL_RECORDS_ERROR, response)

    export const getGlobalHistoryFromTokenStart = () => action(Type.GET_GLOBAL_RECORDS_FROM_TOKEN_START)
    export const getGlobalHistoryFromTokenSuccess = (result: Interfaces.IObjectHistoryGlobalQueryResultContainer) => action(Type.GET_GLOBAL_RECORDS_FROM_TOKEN_SUCCESS, result)
    export const getGlobalHistoryFromTokenError = (response: Interfaces.IFetchResult<any>) => action(Type.GET_GLOBAL_RECORDS_FROM_TOKEN_ERROR, response)
}

export type Actions = Omit<typeof Actions, 'Type'>
