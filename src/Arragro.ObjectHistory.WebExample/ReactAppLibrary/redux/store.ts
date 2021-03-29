import { Store, createStore, applyMiddleware } from 'redux'
import thunk from 'redux-thunk'
import { composeWithDevTools } from 'redux-devtools-extension'
import { History } from 'history'
import { loggerMiddleware } from './middleware'
import { rootReducer } from './reducers'
import { StoreState } from './state'

let store: Store<StoreState>

export function configureStore (history: History, logging: boolean = false, initialState?: object): Store<StoreState> {
    let middleware = applyMiddleware(thunk, loggerMiddleware(logging))

    if (process.env.NODE_ENV !== 'production') {
        middleware = composeWithDevTools(middleware)
    }

    store = createStore(rootReducer as any, initialState!, middleware) as Store<StoreState>

    return store
}
