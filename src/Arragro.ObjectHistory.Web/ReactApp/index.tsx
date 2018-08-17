import * as React from 'react'
import * as ReactDOM from 'react-dom'
import { Provider } from 'react-redux'
import { IntlProvider } from 'react-intl'
import { ConnectedRouter } from 'react-router-redux'
import { AppContainer } from 'react-hot-loader'
import { configureStore, Redux, utils } from '../ReactAppLibrary'
import { App } from './app'
import '../node_modules/bootstrap/dist/css/bootstrap.min.css'
import '../ReactAppLibrary/scss/site.scss'

const { History } = utils

// prepare store
const initialState = (window as any).initialReduxState as Redux.ReduxModel.StoreState
const store = configureStore(History, true, initialState)
const rootEl = document.getElementById('react-app')

const render = (Component: any) => {
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
        rootEl
    )
}

if (rootEl !== null) {
    render(App)

    if (module.hot) {
        module.hot.accept([
            './index.tsx'
        ], () => {
            const app = require('./index.tsx').default
            render(app)
        })
    }
}
