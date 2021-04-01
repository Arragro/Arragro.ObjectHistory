import * as React from 'react'
import { Switch, Route, BrowserRouter } from 'react-router-dom'

import { Containers as LibraryContainers } from '../ReactAppLibrary'
import * as Containers from './Containers'
export interface IAppProps {
    logoUrl?: string,
    applicationName?: string
}

export const App = (props: IAppProps) => {
    let { logoUrl, applicationName } = props
    if (!logoUrl) {
        logoUrl = '/public/images/svg/ArragroLogo.svg'
    }
    if (!applicationName) {
        applicationName = 'Arragro Object History'
    }

    return <BrowserRouter>
        <Switch>
            <Route path='/session/:id' component={ Containers.Session } />
            <Route path='/arragro-object-history/:objectName?' component={ LibraryContainers.Home } />
        </Switch>
    </BrowserRouter>
}
