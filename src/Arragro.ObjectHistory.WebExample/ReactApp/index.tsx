import * as React from 'react'
import * as ReactDOM from 'react-dom'
import { Provider } from 'react-redux'
import { IntlProvider } from 'react-intl'
import { configureStore, Redux, utils, Components } from '../ReactAppLibrary'
import { App } from './app'
import '../node_modules/bootstrap/dist/css/bootstrap.min.css'
import '../ReactAppLibrary/scss/site.scss'
import { Router } from 'react-router'

const { History } = utils
// prepare store
const initialState = (window as any).initialReduxState as Redux.ReduxModel.StoreState
const store = configureStore(History, true, initialState)

const renderComponent = (Component: any, el: HTMLElement) => {
    ReactDOM.render(
            <Provider store={store}>
                <IntlProvider locale='en'>
                    {Component}
                </IntlProvider>
            </Provider>,
        el
    )
}

const render = (Component: any, el: HTMLElement) => {
    ReactDOM.render(
        <Provider store={store}>
            <IntlProvider locale='en'>
                <Router history={History}>
                    <Component />
                </Router>
            </IntlProvider>
        </Provider>,
        el
    )
}

const rootEl = document.getElementById('react-app')
const rootElKnownPartitionKey = document.getElementById('react-app-object-history')

if (rootElKnownPartitionKey !== null) {
    const partitionKey = rootElKnownPartitionKey.getAttribute('data-partition-key')
    if (partitionKey !== null) {
        renderComponent(<Components.History objectName={partitionKey} />, rootElKnownPartitionKey)
    }
}

if (rootEl !== null) {
    render(App, rootEl)
}
