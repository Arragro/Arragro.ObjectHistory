import { QueryResultType } from './enums'

export interface IFetchResult<T> {
    status: any
    ok: boolean
    data: T
}

export enum StorageLocation {
    Primary = 0,
    Secondary = 1
}

export interface ITableContinuationToken {
    nextPartitionKey: string
    nextRowKey: string
    nextTableName: string
    targetLocation?: StorageLocation
}

export interface IPagingToken {
    tableContinuationToken: ITableContinuationToken | null
    page: number
    nextPage: number | null
    pageSize: number
}

export interface IObjectHistoryDetailRaw {
    partitionKey: string
    rowKey: string
    version: number
    applicationName: string
    timeStamp: Date
    folder: string
    user: boolean
    isAdd: boolean
    newJson: string
    oldJson: string
    diff: any
}

export interface IObjectHistoryQueryResult {
    partitionKey: string
    rowKey: string
    version: number
    folder: string
    applicationName: string
    queryResultType: QueryResultType
    modifiedBy: string
    modifiedDate: Date

    expanded: boolean
    historyDetail?: IObjectHistoryDetailRaw
}

export interface IObjectHistoryQueryResultContainer {
    partitionKey: string | null
    results: Array<IObjectHistoryQueryResult>
    pagingToken: IPagingToken | null
}

export interface IObjectLogsPostParameters {
    partitionKey: string
    pagingToken?: IPagingToken
}
