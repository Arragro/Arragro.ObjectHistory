import * as React from 'react'
import { connect } from 'react-redux'
import { Dispatch } from 'redux'

import { Services } from '../redux/modules/global'
import { StoreState } from '../redux/state'

type ComponentPropeties = Services.GlobalConnectedState & Services.GlobalConnectedDispatch

class HomePage extends React.Component<ComponentPropeties> {
    constructor (props: ComponentPropeties) {
        super(props)
    }

    componentDidMount () {
        this.props.get()
    }

    getRows = () => {
        const { global } = this.props

        if (global.globalQueryResultContainer === undefined) {
            return null
        }

        let output = []

        for (let i = 0; i < global.globalQueryResultContainer.results.length; i++) {
            let item = global.globalQueryResultContainer.results[i]

            output.push(<tr key={item.folder}>
                <td>{item.objectName}</td>
                <td>{item.folder}</td>
                <td>{item.modifiedBy}</td>
                <td>{item.modifiedDate}</td>
            </tr>)
        }

        return output
    }

    render () {
        const { global } = this.props

        if (global.loading) {
            return null
        }

        return <table className='table'>
            <thead>
                <tr>
                    <th>Object Name</th>
                    <th>Object History</th>
                    <th>ModifiedBy</th>
                    <th>ModifiedDate</th>
                </tr>
            </thead>
            <tbody>
                {this.getRows()}
            </tbody>
        </table>
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
