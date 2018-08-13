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

export interface IObjectHistoryDetailRaw {
    partitionKey: string
    rowKey: string
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
    folder: string
    rowKey: string
    objectName: string
    applicationName: string
    modifiedBy: string
    modifiedDate: Date

    expanded: boolean
    historyDetail?: IObjectHistoryDetailRaw
}

export interface IObjectHistoryQueryResultContainer {
    partitionKey: string
    results: Array<IObjectHistoryQueryResult>
    continuationToken?: ITableContinuationToken | null
}

export interface IObjectLogsPostParameters {
    partitionKey: string
    tableContinuationToken?: ITableContinuationToken
}