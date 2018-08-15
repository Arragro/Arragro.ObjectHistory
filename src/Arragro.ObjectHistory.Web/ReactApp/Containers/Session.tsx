import * as React from 'react'
import { Link } from 'react-router-dom'
import { RouteComponentProps } from 'react-router'
import { Formik, Form } from 'formik'
import * as Yup from 'yup'
import 'moment'

import * as utils from '../../ReactAppLibrary/utils'
import TextBox from '../FormikControls/TextBox'
import httpUtils from '../../ReactAppLibrary/utils/httpUtils'

declare var require: any
let moment = require('moment')

export interface ISessionPageProps {
    id: string
}

export interface ISessionPageState {
    sessionDrillContainer?: ISessionDrillContainer
}

export interface IDrillData {
    id: string
    name: string
    description: string
    dateCreated: Date
    duration: number
}

export interface ISessionDrillContainer {
    id: number
    name: string
    dateCreated: Date
    drills: Array<IDrillData>
    objectHistoryKey: string
}

export interface IDrillForm {
    name: string
    description: string
    duration: number
}

type ComponentPropeties = RouteComponentProps<ISessionPageProps>

export default class SessionPage extends React.Component<ComponentPropeties, ISessionPageState> {

    constructor (props: ComponentPropeties) {
        super(props)

        this.state = {}

        utils.HttpUtils.get<ISessionDrillContainer>(`/api/drills/forsession/${this.props.match.params.id}`)
            .then((response) => {
                if (response.ok) {
                    this.setState({
                        ...this.state,
                        sessionDrillContainer: response.data
                    })
                } else {
                    alert('Something went wrong getting drill data')
                }
            })
    }

    getDrills = () => {
        const { sessionDrillContainer } = this.state

        if (sessionDrillContainer === undefined) {
            return null
        }

        let output: Array<JSX.Element> = []

        for (let i = 0; i < sessionDrillContainer.drills.length; i++) {
            const drill = sessionDrillContainer.drills[i]

            output.push(
                <div key={drill.id} className='panel panel-default'>
                    <div className='panel-heading'>
                        {drill.name}
                    </div>
                    <div className='panel-body'>
                        {drill.description}
                    </div>
                </div>)
        }

        return output
    }

    submitForm = (drill: IDrillForm, setSubmitting: (isSubmitting: boolean) => void, resetForm: (nextValues: IDrillForm) => void) => {
        const { sessionDrillContainer } = this.state
        const drillPostData = {
            sessionId: sessionDrillContainer!.id,
            ...drill
        }
        httpUtils.post<IDrillForm, ISessionDrillContainer>('/api/drills/create', drillPostData)
            .then((response) => {
                if (response.ok) {
                    this.setState({
                        ...this.state,
                        sessionDrillContainer: response.data
                    }, () => resetForm(this.getInitialValues()))
                } else {
                    setSubmitting(false)
                    alert('Something went wrong posting drill data')
                }
            })
    }

    render () {
        const { sessionDrillContainer } = this.state

        if (sessionDrillContainer === undefined) {
            return null
        }

        return <utils.Aux>
            <h2>Training Session: {sessionDrillContainer.name} - <Link to={`/arragro-object-history/${sessionDrillContainer.objectHistoryKey}`}>History</Link></h2>
            <div className='small'>{moment.utc(sessionDrillContainer.dateCreated).local().format('DD/MM/YYYY h:mm:ss a')}</div>
            <div className='row'>
                <div className='col-md-9'>
                    <h3>Drill Count: {sessionDrillContainer.drills.length}</h3>
                    <div>
                        {this.getDrills()}
                    </div>
                </div>
                <div className='col-md-3'>
                    <div className='panel panel-primary'>
                        <div className='panel-heading'>
                            Add New Drill
                        </div>
                        <div className='panel-body'>
                            <Formik
                                initialValues={this.getInitialValues()}
                                onSubmit={(values: IDrillForm, { setSubmitting, resetForm }) => {
                                    this.submitForm(values, setSubmitting, resetForm)
                                }}
                                validationSchema={Yup.object().shape({
                                    name: Yup.string()
                                        .required('Please supply Name')
                                        .max(20, 'There is a 20 character limit to this field.'),
                                    description: Yup.string()
                                        .required('Please supply Description')
                                        .max(50, 'There is a 50 character limit to this field.'),
                                    duration: Yup.number()
                                        .required('Please supply Duration')
                                })}
                                render={({ submitCount, handleBlur, handleChange, values, errors, isSubmitting }) => (
                                    <Form className={submitCount > 0 ? 'was-validated' : ''}>
                                        <TextBox
                                            type='text'
                                            label='Name'
                                            id='name'
                                            submitCount={submitCount}
                                            value={values.name}
                                            error={errors.name}
                                            handleChange={handleChange}
                                            handleBlur={handleBlur}
                                        />

                                        <TextBox
                                            type='text'
                                            label='Description'
                                            id='description'
                                            submitCount={submitCount}
                                            value={values.description}
                                            error={errors.description}
                                            handleChange={handleChange}
                                            handleBlur={handleBlur}
                                        />

                                        <TextBox
                                            type='number'
                                            label='Duration'
                                            id='duration'
                                            submitCount={submitCount}
                                            value={values.duration.toString()}
                                            error={errors.duration}
                                            handleChange={handleChange}
                                            handleBlur={handleBlur}
                                        />
                                        <button type='submit' className='btn btn-primary' disabled={isSubmitting}>Add Drill</button>
                                    </Form>
                                )}
                            >
                            </Formik>
                        </div>
                    </div>
                </div>
            </div>
            <div className='row'>
                <div className='col-md-12'>
                    <a href='/'>Return home</a>
                </div>
            </div>
        </utils.Aux>
    }

    private getInitialValues = (): IDrillForm => {
        return {
            name: '',
            description: '',
            duration: 10
        }
    }
}
