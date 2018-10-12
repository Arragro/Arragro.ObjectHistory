import ReactAlert from 'react-s-alert'

export class Alert {
    public AlertSettings: any = {}

    public info (msg: string | JSX.Element, data?: any): void {
        ReactAlert.info(msg, data)
    }
    public error (msg: string | JSX.Element, data?: any): void {
        ReactAlert.error(msg, data)
    }
    public warning (msg: string | JSX.Element, data?: any): void {
        ReactAlert.warning(msg, data)
    }
    public success (msg: string | JSX.Element, data?: any): void {
        ReactAlert.success(msg, data)
    }
    public close (id: number): void {
        ReactAlert.close(id)
    }
    public closeAll (): void {
        ReactAlert.closeAll()
    }
}

const alert = new Alert()

export default alert
