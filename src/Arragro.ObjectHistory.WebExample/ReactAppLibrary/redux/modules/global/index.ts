import { Services } from './services'
import { objectHistory } from './reducer'
import { ActionType } from 'typesafe-actions'
import { Actions } from './actions'

type ObjectHistoryAction = ActionType<typeof Actions>

export {
    Services,
    objectHistory,
    ObjectHistoryAction
}
