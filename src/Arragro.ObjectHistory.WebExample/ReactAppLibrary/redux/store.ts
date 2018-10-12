import { Store, createStore, applyMiddleware } from 'redux'
import thunk from 'redux-thunk'
import { composeWithDevTools } from 'redux-devtools-extension'
import { routerMiddleware } from 'react-router-redux'
import { History } from 'history'
import { loggerMiddleware } from './middleware'
import { rootReducer } from './reducers'
import { StoreState } from './state'

let store: Store<StoreState>

export function configureStore (history: History, logging: boolean = false, initialState?: object): Store<StoreState> {
    let middleware = applyMiddleware(thunk, loggerMiddleware(logging), routerMiddleware(history))

    if (process.env.NODE_ENV !== 'production') {
        middleware = composeWithDevTools(middleware)
    }

    store = createStore(rootReducer as any, initialState!, middleware) as Store<StoreState>

    if (module.hot) {
        module.hot.accept('./reducers', () => {
            const nextReducer = require('./reducers').default
            store.replaceReducer(nextReducer)
        })
    }

    return store
}
