import * as React from 'react'
import { connect } from 'react-redux'
import { withRouter, RouteComponentProps } from 'react-router'
import { Dispatch } from 'redux'
import { formatters } from 'jsondiffpatch'
import { Link } from 'react-router-dom'
import 'moment'

import { Services } from '../redux/modules/global'
import { StoreState } from '../redux/state'
import { Aux } from '../utils'
import { ITableContinuationToken, IObjectHistoryQueryResultContainer, IObjectHistoryQueryResult } from '../interfaces'
import { QueryResultType } from '../enums'

declare var require: any
let moment = require('moment')

export interface IHistoryPageProps {
    objectName?: string
}

type ComponentPropeties = Services.ObjectHistoryConnectedState & Services.ObjectHistoryConnectedDispatch & RouteComponentProps<IHistoryPageProps>

class History extends React.Component<ComponentPropeties> {
    constructor (props: ComponentPropeties) {
        super(props)

        this.onGetMoreRecordsClick = this.onGetMoreRecordsClick.bind(this)
        this.onShowDetailsClick = this.onShowDetailsClick.bind(this)
    }

    handleUrlParams (objectName: string | undefined) {
        if (objectName !== undefined) {
            console.log(objectName)
            this.props.getObjectRecord({ partitionKey: objectName })
        } else {
            this.props.get()
        }
    }

    componentDidMount () {
        this.handleUrlParams(this.props.match.params.objectName)
    }

    componentWillReceiveProps (nextProps: ComponentPropeties) {
        if (nextProps.match.params.objectName !== this.props.match.params.objectName) {
            this.handleUrlParams(nextProps.match.params.objectName)
        }
    }

    onShowDetailsClick = (index: number, history: IObjectHistoryQueryResult) => {
        if (!history.expanded) {
            if (history.historyDetail === undefined) {
                this.props.getDetails(index, history.objectName, history.rowKey)
            }
            this.props.showDetailsExpand(index)
        } else {
            this.props.showDetailsHide(index)
        }
    }

    getShowMoreDetails = (history: IObjectHistoryQueryResult, index: number) => {
        if (history.expanded) {
            return <a onClick={() => this.onShowDetailsClick(index, history)}>Hide Details</a>
        } else {
            return <a onClick={() => this.onShowDetailsClick(index, history)}>Show Details</a>
        }
    }

    getRows = (resultContainer: IObjectHistoryQueryResultContainer) => {

        let output = []

        if (resultContainer.results !== undefined) {

            for (let i = 0; i < resultContainer.results.length; i++) {
                let item = resultContainer.results[i]

                const objectName = item.queryResultType === QueryResultType.Global ?
                    <Link to={`/arragro-object-history/${item.objectName}`}>{item.objectName}</Link> :
                    <Aux>{resultContainer.partitionKey}</Aux>

                output.push(<tr key={item.folder}>
                    <td>{objectName}</td>
                    <td>{item.modifiedBy}</td>
                    <td>{moment.utc(item.modifiedDate).local().format('DD/MM/YYYY h:mm:ss a')}</td>
                    <td>{this.getShowMoreDetails(item, i)}</td>
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

    onGetMoreRecordsClick = (tableContinuationToken: ITableContinuationToken) => {
        this.props.getFromToken(tableContinuationToken)
    }

    getMoreRecords = (tableContinuationToken?: ITableContinuationToken | null) => {
        if (tableContinuationToken === undefined || tableContinuationToken === null) {
            return null
        }

        return <button className='btn btn-primary' onClick={() => this.onGetMoreRecordsClick(tableContinuationToken)}>Get More Records</button>
    }

    render () {
        const { objectHistory } = this.props

        if (objectHistory.resultContainer === undefined ||
            objectHistory.loading) {
            return null
        }

        return <Aux>
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
                    {this.getRows(objectHistory.resultContainer)}
                </tbody>
            </table>

            {this.getMoreRecords(objectHistory.resultContainer.continuationToken)}
        </Aux>
    }
}

const mapStateToProps = (state: StoreState, ownProps: IHistoryPageProps): Services.ObjectHistoryConnectedState => {
    return {
        objectHistory: state.objectHistory
    }
}

const mapDispatchToProps = (dispatch: Dispatch): Services.ObjectHistoryConnectedDispatch => {
    return Services.dispatchServices(dispatch)
}

export default withRouter(connect(mapStateToProps, mapDispatchToProps)(History))
