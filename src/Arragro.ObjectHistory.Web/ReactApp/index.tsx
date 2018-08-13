import * as React from 'react'
import * as ReactDOM from 'react-dom'
import { Provider } from 'react-redux'
import { IntlProvider } from 'react-intl'
import { ConnectedRouter } from 'react-router-redux'
import { AppContainer } from 'react-hot-loader'
import { configureStore, Redux } from '../ReactAppLibrary'
import { App } from '../ReactAppLibrary/app'
import { History } from '../ReactAppLibrary/utils'

// prepare store
const initialState = (window as any).initialReduxState as Redux.ReduxModel.StoreState
const store = configureStore(History, true, initialState)
const rootEl = document.getElementById('arragro-object-history-react-app')

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

render(App)

if (module.hot) {
    module.hot.accept([
        '../ReactAppLibrary/index',
        './index.tsx'
    ], () => {
        const app = require('../ReactAppLibrary/app').default
        render(app)
    })
}
