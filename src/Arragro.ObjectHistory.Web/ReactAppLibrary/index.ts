import Alert from './alert'
import * as Redux from './redux'
import * as Services from './services'
import * as Containers from './containers'
import * as Interfaces from './interfaces'
import * as utils from './utils'
import { App } from './app'

import './scss/site.scss'

const { configureStore } = Redux

export default App

export {
    utils,
    Redux,
    Services,
    Interfaces,
    Containers,
    App,
    Alert,
    configureStore
}
