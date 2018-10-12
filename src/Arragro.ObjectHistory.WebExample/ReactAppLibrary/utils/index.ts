import * as Helpers from './helpers'
import HttpUtils from './httpUtils'
import History from './history'

export {
    Helpers,
    HttpUtils,
    History
}

export * from './reactAux'

export function omit<T extends object, K extends keyof T> (target: T, ...omitKeys: K[]): Helpers.OmitString<T, K> {
    return (Object.keys(target) as K[]).reduce(
        (res, key) => {
            if (!omitKeys.includes(key)) {
                res[key] = target[key]
            }
            return res
        },
        {} as Helpers.OmitString<T, K>
  )
}
