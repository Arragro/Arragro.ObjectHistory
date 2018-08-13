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

export interface IObjectHistoryGlobalQueryResult {
    folder: string
    rowKey: string
    objectName: string
    modifiedBy: string
    modifiedDate: Date
}

export interface IObjectHistoryQueryResult {
    folder: string
    rowKey: string
    applicationName: string
    modifiedBy: string
    modifiedDate: Date
}

export interface IObjectHistoryGlobalQueryResultContainer {
    partitionKey: string
    results: Array<IObjectHistoryGlobalQueryResult>
    continuationToken?: ITableContinuationToken
}

export interface IObjectLogsPostParameters {
    partitionKey: string
    tableContinuationToken?: ITableContinuationToken
}

export interface IObjectHistoryQueryResultContainer {
    partitionKey: string
    results: Array<IObjectHistoryQueryResult>
    continuationToken?: ITableContinuationToken
}
