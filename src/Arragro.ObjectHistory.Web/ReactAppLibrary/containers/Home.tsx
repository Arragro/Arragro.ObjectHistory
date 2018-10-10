import * as React from 'react'
import { RouteComponentProps } from 'react-router'

import History from '../components/History'

export interface IHomePageProps {
    objectName?: string
}

type ComponentPropeties = RouteComponentProps<IHomePageProps>

export default class HomePage extends React.Component<ComponentPropeties> {
    constructor (props: ComponentPropeties) {
        super(props)
    }

    render () {
        return <History />
    }
}
