import * as React from 'react'
import { Switch, Route } from 'react-router-dom'

import * as Containers from './containers'

export interface IAppProps {
    logoUrl?: string,
    applicationName?: string
}

export const App: React.StatelessComponent<IAppProps> = (props) => {
    let { logoUrl, applicationName } = props
    if (!logoUrl) {
        logoUrl = '/public/images/svg/ArragroLogo.svg'
    }
    if (!applicationName) {
        applicationName = 'Arragro Object History'
    }

    return <React.Fragment>
        <Switch>
            <Route path='/arragro-object-history/:objectName?' component={ Containers.Home } />
        </Switch>
    </React.Fragment>
}
