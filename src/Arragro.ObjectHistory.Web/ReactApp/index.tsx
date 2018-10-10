import * as React from 'react'
import * as ReactDOM from 'react-dom'
import { Provider } from 'react-redux'
import { IntlProvider } from 'react-intl'
import { ConnectedRouter } from 'react-router-redux'
import { AppContainer } from 'react-hot-loader'
import { configureStore, Redux, utils, Components } from '../ReactAppLibrary'
import { App } from './app'
import '../node_modules/bootstrap/dist/css/bootstrap.min.css'
import '../ReactAppLibrary/scss/site.scss'

const { History } = utils
// prepare store
const initialState = (window as any).initialReduxState as Redux.ReduxModel.StoreState
const store = configureStore(History, true, initialState)

const renderComponent = (Component: any, el: HTMLElement) => {
    ReactDOM.render(
        <AppContainer>
            <Provider store={store}>
                <IntlProvider>
                    {Component}
                </IntlProvider>
            </Provider>
        </AppContainer>,
        el
    )
}

const render = (Component: any, el: HTMLElement) => {
    ReactDOM.render(
        <AppContainer>
            <Provider store={store}>
                <IntlProvider>
                    <ConnectedRouter history={History}>
                        <Component />
                    </ConnectedRouter>
                </IntlProvider>
            </Provider>
        </AppContainer>,
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

    if (module.hot) {
        module.hot.accept([
            './index.tsx'
        ], () => {
            const app = require('./index.tsx').default
            render(app, rootEl)
        })
    }
}
