export type Omit<T, K extends keyof T> = Pick<T, Exclude<keyof T, K>>
export type Diff<T extends string, U extends string> = ({[P in T]: P } & {[P in U]: never } & { [x: string]: never })[T]
export type OmitString<T, K extends keyof T> = Pick<T, Diff<keyof T, K>>

export const isExisty = (value: any) => {
    return value !== null && value !== undefined
}

export const isEmpty = (value: string) => {
    return value === ''
}

export const makeEmptyString = (value: string | undefined | null) => {
    if (value === undefined || value === null) {
        return ''
    }
    return value
}

export const makeObjectKeyEmptyString = (data: any, key: string) => {
    if ((data === undefined || data === null) &&
        (data[key] === undefined || data[key] === null)) {
        return ''
    }
    return data[key]
}

export const makeEmptyStringNull = (value: string | undefined | null): string | null => {
    if (value !== undefined && value !== null && value.length === 0) {
        return null
    }
    if (value === undefined) {
        return null
    }
    return value
}

export const setDepth = (obj: any, path: string, value: any) => {
    let tags = path.split('.')
    let len = tags.length - 1
    for (let i = 0; i < len; i++) {
        obj = obj[tags[i]]
    }
    obj[tags[len]] = value
}

export const getDepth = (obj: any, path: string): any => {
    let tags = path.split('.')
    let len = tags.length - 1
    for (let i = 0; i < len; i++) {
        obj = obj[tags[i]]
    }
    return obj[tags[len]]
}

export const expand = (str: string, val = {}) => {
    return str.split('.').reduceRight((acc, currentValue) => {
        return { [currentValue]: acc }
    }, val)
}

export const mergeRecursive = (obj1: any, obj2: any) => {
    for (let p in obj2) {
        if (obj2[p] !== undefined && obj2[p].constructor === Object) {
            if (obj1[p]) {
                mergeRecursive(obj1[p], obj2[p])
                continue
            }
        }
        obj1[p] = obj2[p]
    }
    return obj1
}
