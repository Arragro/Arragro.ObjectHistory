import * as ReduxModel from './state'
import { configureStore } from './store'

import { loggerMiddleware } from './middleware/logger'
import * as ObjectHistory from './modules/global'

export {
    configureStore,
    ObjectHistory,
    ReduxModel,
    loggerMiddleware
}
