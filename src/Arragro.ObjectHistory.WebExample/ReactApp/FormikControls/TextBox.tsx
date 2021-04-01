import * as React from 'react'

export interface IProps {
    type: string
    id: string
    label: string
    error?: any
    disabled?: boolean
    submitCount: number
    value: string
    autocomplete?: string
    handleChange: React.ChangeEventHandler<HTMLInputElement>
    handleBlur: React.FocusEventHandler<HTMLInputElement>
}

export interface IState {
    changed: boolean
}

export default class TextBox extends React.Component<IProps, IState> {
    constructor (props: IProps) {
        super(props)

        this.state = {
            changed: false
        }

        this.onChange = this.onChange.bind(this)
    }

    onChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        const { handleChange } = this.props
        if (!this.state.changed) {
            this.setState({
                ...this.state,
                changed: true
            })
        }
        handleChange(event)
    }

    render () {
        const {
            type,
            id,
            label,
            error,
            value,
            handleChange,
            handleBlur,
            submitCount,
            disabled,
            ...restProps
        } = this.props

        const {
            changed
        } = this.state

        const getError = () => {
            if ((submitCount > 0 || changed) && error) {
                return <div><span className='help-block'>{error}</span></div>
            }
            return null
        }

        return <fieldset className={ (submitCount > 0 || changed) && error ? 'form-group has-error' : 'form-group' }>
            <label className='control-label' htmlFor={id}>{label}</label>
            <input
                id={id}
                name={id}
                type={type}
                onChange={this.onChange}
                onBlur={handleBlur}
                value={value}
                disabled={disabled === undefined ? false : disabled}
                className='form-control'
                {...restProps}
            />
            {getError()}
        </fieldset>
    }

}
