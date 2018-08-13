import * as React from 'react'
import { connect } from 'react-redux'
import { Dispatch } from 'redux'
import { formatters } from 'jsondiffpatch'
import 'moment'

import { Services } from '../redux/modules/global'
import { StoreState } from '../redux/state'
import { Aux } from '../utils'
import { ITableContinuationToken, IObjectHistoryGlobalQueryResultContainer, IObjectHistoryGlobalQueryResult } from '../interfaces'

declare var require: any
let moment = require('moment')

type ComponentPropeties = Services.GlobalConnectedState & Services.GlobalConnectedDispatch

class HomePage extends React.Component<ComponentPropeties> {
    constructor (props: ComponentPropeties) {
        super(props)

        this.onGetMoreRecordsClick = this.onGetMoreRecordsClick.bind(this)
        this.onShowDetailsClick = this.onShowDetailsClick.bind(this)
    }

    componentDidMount () {
        this.props.get()
    }

    onShowDetailsClick = (index: number, history: IObjectHistoryGlobalQueryResult) => {
        if (!history.expanded) {
            if (history.historyDetail === undefined) {
                this.props.getDetails(index, history.folder)
            }
            this.props.showDetailsExpand(index)
        } else {
            this.props.showDetailsHide(index)
        }
    }

    getShowMoreDetails = (history: IObjectHistoryGlobalQueryResult, index: number) => {
        if (history.expanded) {
            return <a onClick={() => this.onShowDetailsClick(index, history)}>Hide Details</a>
        } else {
            return <a onClick={() => this.onShowDetailsClick(index, history)}>Show Details</a>
        }
    }

    getRows = (globalQueryResultContainer: IObjectHistoryGlobalQueryResultContainer) => {

        let output = []

        for (let i = 0; i < globalQueryResultContainer.results.length; i++) {
            let item = globalQueryResultContainer.results[i]

            output.push(<tr key={item.folder}>
                <td>{item.objectName}</td>
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
        const { global } = this.props

        if (global.globalQueryResultContainer === undefined ||
            global.loading) {
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
                    {this.getRows(global.globalQueryResultContainer)}
                </tbody>
            </table>

            {this.getMoreRecords(global.globalQueryResultContainer.continuationToken)}
        </Aux>
    }
}

const mapStateToProps = (state: StoreState): Services.GlobalConnectedState => {
    return {
        global: state.global
    }
}

const mapDispatchToProps = (dispatch: Dispatch): Services.GlobalConnectedDispatch => {
    return Services.dispatchServices(dispatch)
}

export default connect(mapStateToProps, mapDispatchToProps)(HomePage)
