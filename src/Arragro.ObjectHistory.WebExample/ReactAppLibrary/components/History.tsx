import * as React from 'react'
import { useDispatch, useSelector } from 'react-redux'
import { useParams } from 'react-router'
import { formatters } from 'jsondiffpatch'
import { Link } from 'react-router-dom'

import { Services } from '../redux/modules/global'
import { StoreState } from '../redux/state'
import { IObjectHistoryQueryResultContainer, IObjectHistoryQueryResult, IPagingToken } from '../interfaces'
import { QueryResultType } from '../enums'
import * as dayjs from 'dayjs'
import { usePrevious } from '../utils/helpers'

const History = (props: { objectName?: string }) => {
    
    let objectName = ''
    if (props.objectName) {
        objectName = props.objectName
    } else {
        const params = useParams<{ objectName: string }>()
        objectName = params.objectName
    }

    const dispatch = useDispatch()
    const objectHistory = useSelector((storeState: StoreState) => storeState.objectHistory)
    const objectHistoryDispatchService = Services.dispatchServices(dispatch)
    const prevObjectName = usePrevious(objectName)

    const handleUrlParams = (objectName: string | undefined) => {
        if (objectName !== undefined) {
            console.log(objectName)
            objectHistoryDispatchService.getObjectRecord({ partitionKey: objectName })
        } else {
            objectHistoryDispatchService.get()
        }
    }

    React.useEffect(() => {
        handleUrlParams(objectName)
    }, [])

    React.useEffect(() => {
        if (prevObjectName &&
            prevObjectName !== objectName)
            handleUrlParams(objectName)

    }, [objectName])

    const onShowDetailsClick = (index: number, history: IObjectHistoryQueryResult) => {
        if (!history.expanded) {
            if (history.historyDetail === undefined) {
                objectHistoryDispatchService.getDetails(index, history.partitionKey, history.rowKey)
            }
            objectHistoryDispatchService.showDetailsExpand(index)
        } else {
            objectHistoryDispatchService.showDetailsHide(index)
        }
    }

    const getShowMoreDetails = (history: IObjectHistoryQueryResult, index: number) => {
        if (history.expanded) {
            return <a onClick={() => onShowDetailsClick(index, history)}>Hide Details</a>
        } else {
            return <a onClick={() => onShowDetailsClick(index, history)}>Show Details</a>
        }
    }

    const getRows = (resultContainer: IObjectHistoryQueryResultContainer) => {

        let output = []

        if (resultContainer.results !== undefined) {

            for (let i = 0; i < resultContainer.results.length; i++) {
                let item = resultContainer.results[i]

                const objectName = item.queryResultType === QueryResultType.Global ?
                    <Link to={`/arragro-object-history/${item.partitionKey}`}>{item.partitionKey}</Link> :
                    <>{resultContainer.partitionKey}</>

                output.push(<tr key={item.folder}>
                    <td>{objectName}</td>
                    <td>{item.modifiedBy}</td>
                    <td>{dayjs(item.modifiedDate).utc().local().format('DD/MM/YYYY h:mm:ss a')}</td>
                    <td>{getShowMoreDetails(item, i)}</td>
                </tr>)

                if (item.expanded && item.historyDetail !== undefined) {
                    if (item.historyDetail.isAdd) {
                        output.push(<tr key={`${item.folder}-details`}>
                            <td colSpan={6}><pre>{JSON.stringify(item.historyDetail.newJson, null, '\t')}</pre></td>
                        </tr>)
                    } else {
                        output.push(<tr key={`${item.folder}-details`}>
                            <td colSpan={6} dangerouslySetInnerHTML={{ __html: formatters.html.format(item.historyDetail.diff, item.historyDetail.oldJson) }}>
                                {/* <pre>{JSON.stringify(item.historyDetail.diff, null, '\t')}</pre> */}
                            </td>
                        </tr>)
                    }
                }
            }
        }
        return output
    }

    const onGetMoreRecordsClick = (pagingToken: IPagingToken) => {
        objectHistoryDispatchService.getFromToken(pagingToken)
    }

    const getMoreRecords = (pagingToken?: IPagingToken | null) => {
        if (pagingToken === undefined || pagingToken === null) {
            return null
        }

        return <button className='btn btn-primary' onClick={() => onGetMoreRecordsClick(pagingToken)}>Get More Records</button>
    }

    if (objectHistory.resultContainer === undefined ||
        objectHistory.loading) {
        return null
    }

    if (objectHistory.resultContainer.results && 
        objectHistory.resultContainer.results.length === 0)
        return <h3>There is no history for this object</h3>

    return <>
        <table className='table'>
            <thead>
                <tr>
                    <th>Object Name</th>
                    <th>Modified By</th>
                    <th>Modified Date</th>
                    <th></th>
                </tr>
            </thead>
            <tbody>
                {getRows(objectHistory.resultContainer)}
            </tbody>
        </table>

        {getMoreRecords(objectHistory.resultContainer.pagingToken)}
    </>
}

export default History
