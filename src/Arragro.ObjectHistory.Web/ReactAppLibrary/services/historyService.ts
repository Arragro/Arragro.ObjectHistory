import { HttpUtils } from '../utils'
import { IFetchResult,
         ITableContinuationToken,
         IObjectHistoryGlobalQueryResultContainer,
         IObjectLogsPostParameters,
         IObjectHistoryQueryResultContainer,
         IObjectHistoryDetailRaw
} from '../interfaces'

export class HistoryService {

    getGlobalLogs = (tableContinuationToken?: ITableContinuationToken): Promise<IFetchResult<IObjectHistoryGlobalQueryResultContainer>> => {
        return HttpUtils.post('/arragro-object-history/get-global-logs', tableContinuationToken === undefined ? {} : tableContinuationToken)
    }

    getObjectLogs = (objectLogsPostParameters: IObjectLogsPostParameters): Promise<IFetchResult<Array<IObjectHistoryQueryResultContainer>>> => {
        return HttpUtils.post('/arragro-object-history/get-object-logs', objectLogsPostParameters)
    }

    getObjectLog = (folder: string): Promise<IFetchResult<IObjectHistoryDetailRaw>> => {
        return HttpUtils.get(`/arragro-object-history/get-object-log/${folder}`)
    }
}

const historyService = new HistoryService()

export default historyService
