import { HttpUtils } from '../utils'
import { IFetchResult,
         IPagingToken,
         IObjectHistoryQueryResultContainer,
         IObjectLogsPostParameters,
         IObjectHistoryDetailRaw
} from '../interfaces'

export class HistoryService {

    getGlobalLogs = (pagingToken?: IPagingToken): Promise<IFetchResult<IObjectHistoryQueryResultContainer>> => {
        return HttpUtils.post('/arragro-object-history/get-global-logs', pagingToken === undefined ? {} : pagingToken)
    }

    getObjectLogs = (objectLogsPostParameters: IObjectLogsPostParameters): Promise<IFetchResult<IObjectHistoryQueryResultContainer>> => {
        return HttpUtils.post('/arragro-object-history/get-object-logs', objectLogsPostParameters)
    }

    getObjectLog = (partitionKey: string, rowKey: string): Promise<IFetchResult<IObjectHistoryDetailRaw>> => {
        return HttpUtils.get(`/arragro-object-history/get-object-log/${partitionKey}/${rowKey}`)
    }
}

const historyService = new HistoryService()

export default historyService
