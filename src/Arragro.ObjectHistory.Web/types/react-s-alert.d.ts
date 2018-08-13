declare module 'react-s-alert' {
    import * as React from 'react'
    
    abstract class IAlert extends React.Component<any, any> {
        static info(msg: any, data: any): any
        static error(msg: any, data: any): any
        static warning(msg: any, data: any): any
        static success(msg: any, data: any): any
        static close(id: any): any
        static closeAll(): any
    }

    export default class Alert extends IAlert { }
}