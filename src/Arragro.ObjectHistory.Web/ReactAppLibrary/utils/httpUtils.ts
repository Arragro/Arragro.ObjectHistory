import * as Cookies from 'js-cookie'
import AppSettings from '../alert'
import { IFetchResult } from '../interfaces'
import History from './history'

export class HttpUtils {

    parseJSON<T> (response: Response): Promise<IFetchResult<T>> {
        return new Promise((resolve) => {
            if (response.status === 401) {
                const location = window.location
                History.push(`/account/logout?returnUrl=${encodeURIComponent(location.pathname + location.search)}&logout=true`)
                return
            } else if (response.status === 403) {
                History.push('/account/login?returnUrl=/')
                throw { message: 'Access Denied', status: response.status, response }
            }

            response.text()
                .then((text) => {
                    if (text.length > 0) {
                        resolve({
                            status: response.status,
                            ok: response.ok,
                            data: JSON.parse(text) as T
                        })
                    } else {
                        resolve({
                            status: response.status,
                            ok: response.ok,
                            data: {} as T
                        })
                    }
                })
                .catch(err => {
                    throw err
                })
        })
    }

    get<T> (url: string): Promise<IFetchResult<T>> {
        return this.futchGet(url)
    }

    post<T, R> (url: string, postData: T): Promise<IFetchResult<R>> {
        return this.futch<T, R>(url, 'POST', postData)
    }

    private getCSRFCookie (): string {
        const csrf = Cookies.get('ARRAGROCMSCSRF')
        return csrf === undefined ? '' : csrf
    }

    private futchGet<T> (url: string): Promise<IFetchResult<T>> {
        return fetch(url, {
            credentials: 'same-origin'
        })
        .then((response: Response) => this.parseJSON<T>(response))
        .catch((error) => {
            if (url !== '/api/user/current') {
                AppSettings.error(`${error.message} - ${url}`, AppSettings.AlertSettings)
            }
            throw error
        })
    }

    private futch<T, R> (url: string, verb: string, postData: T): Promise<IFetchResult<R>> {
        return fetch(url, {
            credentials: 'same-origin',
            method: verb,
            headers: {
                'Content-Type': 'application/json',
                'X-CSRF-TOKEN-ARRAGROCMS': this.getCSRFCookie()
            },
            body: JSON.stringify(postData)
        })
        .then((response: Response) => this.parseJSON<R>(response))
        .catch((error) => {
            AppSettings.error(`${error.message} - ${url}`, AppSettings.AlertSettings)
            throw error
        })
    }
}

const httpUtils = new HttpUtils()
export default httpUtils
