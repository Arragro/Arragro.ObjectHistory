import Alert from './alert'
import * as Redux from './redux'
import * as Services from './services'
import * as Components from './components'
import * as Containers from './containers'
import * as Interfaces from './interfaces'
import * as utils from './utils'
import { App } from './app'

import * as dayjs from 'dayjs'
import * as utc from 'dayjs/plugin/utc'
dayjs.extend(utc)

import './scss/site.scss'

const { configureStore } = Redux

export default App

export {
    utils,
    Redux,
    Services,
    Interfaces,
    Components,
    Containers,
    App,
    Alert,
    configureStore
}
