import { Middleware, MiddlewareAPI, Dispatch } from 'redux'

export const loggerMiddleware = (enabled: boolean = false): Middleware<any, {}, any> =>
    ({ getState }: MiddlewareAPI) =>
        (next: Dispatch) =>
            (action: any): any => {
                if (enabled) {
                    console.log('will dispatch', action)
                }
                // Call the next dispatch method in the middleware chain.
                const returnValue = next(action)

                if (enabled) {
                    console.log('state after dispatch', action, getState())
                }

                // This will likely be the action itself, unless
                // a middleware further in chain changed it.
                return returnValue
            }
